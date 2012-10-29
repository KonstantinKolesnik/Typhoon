#include <stdbool.h>
#include <avr/interrupt.h>
#include <string.h>
#include "hardware.h"
#include "dccout.h"
#include "rs232.h"
//----------------------------------------------------------------------------------------
#define PERIOD_1   116L                  // 116us for DCC 1 pulse - do not change
#define PERIOD_0   232L                  // 232us for DCC 0 pulse - do not change
#define CUTOUT_GAP  38L                  // 38us gap after last bit of xor


#if ((F_CPU / 1024 * PERIOD_0) > 4177000)
#warning: Overflow in calculation of constant
// if this warning is issued, please split the "/1000000L" into two parts
#endif


volatile unsigned char NextMessageCount = 0;
Message NextMessage, Msg;

Message DCCIdle;
//Message DCCReset;

/*
#if (TURNOUT_FEEDBACK_ENABLED == 1)
volatile unsigned int feedbacked_accessory; // this is the current turnout under query; this includes the coil address!
volatile unsigned char feedback_required;   // 1: make a query at end of next preamble
volatile unsigned char feedback_ready;      // MSB: if 1: the query is done (communication flag)
                                            // LSB: if 1: there was no feedback
                                            //            == position error
*/

enum States
{
     Idle,
     Preamble,
     StartBit,
     Byte,
     XOR,
     EndBit,
     Cutout1,
     Cutout2
};

struct
{
    enum States   State;
    unsigned char Dcc[MAX_DCC_SIZE];    // current message in output processing
    unsigned char ByteIdx;              // current index of byte in message
    unsigned char ByteCount;            // number of bytes remained (decremented)
    unsigned char BitCount;             // number of bits remained (decremented)
    
	unsigned char CurrentByte;          // current processing byte
    unsigned char XORByte;              // actual XOR byte

    unsigned char RailcomEnabled;       // if true: create cutout
    //MessageType   Type;                 // type (for feedback)
} doi;

//----------------------------------------------------------------------------------------
static inline void SetBit(bool bit) __attribute__((always_inline));
void SetBit(bool bit)
{
	TCCR1A = (1<<COM1A1) | (0<<COM1A0)  //  "0" OC1A(=DCC) on compare match
           | (1<<COM1B1) | (1<<COM1B0)  //  "1" OC1B(=NDCC) on compare match
           | (0<<WGM11)  | (0<<WGM10);  //  CTC (+ WGM12, WGM13) (сброс при совпадении), TOP = OCR1A

	OCR1A = OCR1B = F_CPU * (bit == 0 ? PERIOD_0 : PERIOD_1) / 2 / 1000000L; // 928/464 ticks
}
//----------------------------------------------------------------------------------------
// this is the code for cutout - lead_in
/*
static inline void SetBit_no_B(bool bit) __attribute__((always_inline));
void SetBit_no_B(bool bit)
{
    TCCR1A = (1<<COM1A1) | (0<<COM1A0)  //  "0" OC1A(=DCC) on compare match
           | (1<<COM1B1) | (1<<COM1B0)  //  "1" OC1B(=NDCC) on compare match
           | (0<<WGM11)  | (0<<WGM10);  //  CTC (+ WGM12, WGM13) (сброс при совпадении), TOP = OCR1A

	if (bit == 0)
    {                                       //  0 - make a long pwm pulse
        OCR1A = F_CPU * PERIOD_0 / 2 / 1000000L;     //1856 (for 16MHz)
        OCR1B = F_CPU * PERIOD_0 / 2 / 1000000L * 4; // extended (cutout starts after OCR1A)
    }
    else
    {                                       //  1 - make a short pwm puls
        //OCR1A = F_CPU * PERIOD_1  / 2 / 1000000L;    // 928
        OCR1A = F_CPU * CUTOUT_GAP / 1000000L;       // 608
        OCR1B = F_CPU * PERIOD_1 / 2 / 1000000L * 8; // extended (cutout starts after OCR1A)
    }
}
*/
//----------------------------------------------------------------------------------------
ISR(TIMER1_COMPA_vect)
{
    // phase 0: just repeat same duration, but invert output.
    // phase 1: create new bit.

    if (!(PINB & (1<<DCC))) // phase 0: just repeat same duration, but invert output.
	{
        /*
		if (doi.State == Cutout2 && doi.RailcomEnabled)
        {
			TCCR1A = (1<<COM1A1) | (1<<COM1A0)  //  "1" OC1A(=DCC) on compare match
                   | (1<<COM1B1) | (1<<COM1B0)  //  "1" OC1B(=NDCC) on compare match
                   | (0<<WGM11)  | (0<<WGM10);  //  CTC (together with WGM12 and WGM13)
            // OCR1A = F_CPU / 1000000L * PERIOD_1 * 7 / 2;   // create extended timing: 3.5 * PERIOD_1 for DCC
    		// OCR1B = F_CPU / 1000000L * PERIOD_1 * 8 / 2;   //                         4 * PERIOD_1 for NDCC
    		OCR1A = (F_CPU / 1000000L * PERIOD_1 * 4)
                    - (F_CPU / 1000000L * CUTOUT_GAP);    // create extended timing: 4 * PERIOD_1 for DCC - GAP
    		OCR1B = (F_CPU / 1000000L * PERIOD_1 * 9 / 2)   //                            4.5 * PERIOD_1 for NDCC - GAP
                    - (F_CPU / 1000000L * CUTOUT_GAP);
    	}
        else
		*/
        {
			// invert output
            TCCR1A = (1<<COM1A1) | (1<<COM1A0)  //  "1" OC1A(=DCC) on compare match
                   | (1<<COM1B1) | (0<<COM1B0)  //  "0" OC1B(=NDCC) on compare match
                   | (0<<WGM11)  | (0<<WGM10);  //  CTC (together with WGM12 and WGM13)
    	}
    }
	else // phase 1: create new bit
	{
		/*
		//for testing impulses
		//TCCR1A = (1<<COM1A1) | (0<<COM1A0)  //  "0" OC1A(=DCC) on compare match
	    //       | (1<<COM1B1) | (1<<COM1B0)  //  "1" OC1B(=NDCC) on compare match
	    //       | (0<<WGM11)  | (0<<WGM10);  //  CTC (+ WGM12, WGM13) (сброс при совпадении), TOP = OCR1A
		//return;
		*/

		switch (doi.State)
		{
	        case Idle:
				/*
				if (NextMessageCount == 0)
					LED_SHORT_PROG_ON;
				else
					LED_SHORT_PROG_OFF;
				*/

				//Msg = (NextMessageCount == 0 ? (PROG_IS_ON ? DCCReset : DCCIdle) : NextMessage);
				Msg = (NextMessageCount == 0 ? DCCIdle : NextMessage);
				memcpy(doi.Dcc, Msg.Dcc, sizeof(doi.Dcc));
				doi.ByteCount = Msg.Size;
	            doi.ByteIdx = 0;
	            doi.XORByte = 0;
	            //doi.Type = NextMessage.Type;      	// remember type in case feedback is required
				doi.BitCount = (PROG_IS_ON ? 22 : 15); 	// min 20; min 14;
				doi.State = Preamble;
				if (NextMessageCount > 0)
				{
					NextMessageCount--;
					if (NextMessageCount == 0)
						USARTWriteString("SR*");
				}
				break;
	        case Preamble:
	            SetBit(1);
	            doi.BitCount--;
	            if (doi.BitCount == 0) // preample finished; now send start bit
	            {
	                doi.State = StartBit;
	                /*
					#if (TURNOUT_FEEDBACK_ENABLED == 1)
	                if (feedback_required)
	                {
	                    if (EXT_STOP_PRESSED) 
	                        // feedback received -> yes, turnout has this position
	                        feedback_ready = (1 << FB_READY) | (1 << FB_OKAY);
	                    else
	                        // no feedback: 
	                        feedback_ready = (1 << FB_READY);
	                    feedback_required = 0;
	                }
	                #endif
					*/
	            }
	            break;
	        case StartBit:
	            SetBit(0);
	            if (doi.ByteCount == 0) // all bytes are sent, now send XOR byte
	            {
	                doi.CurrentByte = doi.XORByte;
	                doi.BitCount = 8;
	                doi.State = XOR;
	            }
	            else // get next byte
	            {
	                doi.ByteCount--;
	                doi.CurrentByte = doi.Dcc[doi.ByteIdx++];
	                doi.XORByte ^= doi.CurrentByte;
	                doi.BitCount = 8;
	                doi.State = Byte;
	            }
	            break;
	        case Byte: // data byte
	            SetBit((doi.CurrentByte & 0x80 ?  1: 0)); // 0b10000000 - most left bit
	            doi.CurrentByte <<= 1; // bit sent, shift to next bit
	            doi.BitCount--;
	            if (doi.BitCount == 0)
	                doi.State = StartBit;
	            break;
	        case XOR: // error sum
	            SetBit((doi.CurrentByte & 0x80 ?  1: 0)); // 0b10000000 - most left bit
	            doi.CurrentByte <<= 1; // bit sent, shift to next bit
	            doi.BitCount--;
	            if (doi.BitCount == 0) // XOR is sent
	            {
	                doi.State = EndBit;

	                /*
					#if (TURNOUT_FEEDBACK_ENABLED == 1)
	                if (doi.Type == is_feedback)
	                {
	                    // message1 message0 -> ...||......|
	                    // -aaa-ccc --AAAAAA => aaaAAAAAAccc
	                    feedbacked_accessory = ((doi.dcc[0] & 0b00111111) << 3)   // addr low
	                                       | ((~doi.dcc[1] & 0b01110000) << 5)    // addr high
	                                       | (doi.dcc[1] & 0b00000111);           // output
	                    feedback_ready = 0;
	                    feedback_required = 1;
	                }
	                #endif // feedback
					*/
	            }
	            break;
			case EndBit:
	            SetBit(1);
	            doi.State = Idle;
	            //doi.State = Cutout1;
	            break;
			/*
	        case Cutout1:
	            if (doi.RailcomEnabled)
					SetBit_no_B(1);     // first 1 after message gets extended
	            else
					SetBit(1); // stolen from preamble
	            doi.State = Cutout2;
	            break;
	        case Cutout2:
	            SetBit(1); // stolen from preamble
	            doi.State = Idle;
	            break;
			*/
			default:
				break;
		}
	}
}
//----------------------------------------------------------------------------------------
void InitDCCOut()
{
	DCCIdle.Size = 2;
    DCCIdle.Dcc[0] = 0xFF;
    DCCIdle.Dcc[1] = 0x00;

	//DCCReset.Size = 2;
    //DCCReset.Dcc[0] = 0x00;
    //DCCReset.Dcc[1] = 0x00;

	NextMessageCount = 0;

    doi.State = Idle;
	DCCOutDisableRailcom();

    //#if (TURNOUT_FEEDBACK_ENABLED == 1)
    //	feedback_ready = 0;
	//	feedback_required = 0;
    //#endif

    SetBit(1); // init COMP regs.

	// set Timer/Counter1
    TCNT1 = 0; // no prescaler
    
	TCCR1A = (1<<COM1A1) | (0<<COM1A0)  //  "0" OC1A(=DCC) on compare match
           | (1<<COM1B1) | (1<<COM1B0)  //  "1" OC1B(=NDCC) on compare match
           | (0<<WGM11)  | (0<<WGM10);  //  CTC (+ WGM12, WGM13) (сброс при совпадении), TOP = OCR1A
    
	TCCR1B = (0<<ICNC1)  | (0<<ICES1)   // Noise Canceler: Off
           | (0<<WGM13)  | (1<<WGM12)
           | (0<<CS12)   | (0<<CS11)    | (1<<CS10);  // no prescaler, source = sys_clk

	TCCR1C = (0<<FOC1A)  | (0<<FOC1B);  //  reserved in PWM, set to zero

    TIMSK |= (1<<OCIE1A);// | 				// разрешения прерывания по событию "совпадение А" таймера/счетчика Т1
			 //(0<<OCIE1B);				// разрешения прерывания по событию "совпадение B" таймера/счетчика Т1
}
//-------------------------------------------------------------------------------------
// RailCom Interface
//-------------------------------------------------------------------------------------
void DCCOutEnableRailcom()
{
    doi.RailcomEnabled = 1;
}
void DCCOutDisableRailcom()
{
    doi.RailcomEnabled = 0;
}
unsigned char DCCOutQueryRailcom()
{
    return doi.RailcomEnabled;
}
//-------------------------------------------------------------------------------------

