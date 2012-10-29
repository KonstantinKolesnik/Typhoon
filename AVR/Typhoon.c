//#include <stdlib.h>
//#include <stdbool.h>
//#include <inttypes.h>
//#include <avr/io.h>
#include <avr/interrupt.h>
//#include <avr/pgmspace.h>
//#include <avr/eeprom.h>
//#include <string.h>
//#include <util/delay.h>
#include "hardware.h"
#include "dccout.h"
#include "rs232.h"
#include "parser.h"
#include "state.h"
//----------------------------------------------------------------------------------------
void InitHardware()
{
    // Timer1 initialization done in InitDCCOut();

	// Port B
	PORTB = (0<<EN_MAIN)
	      | (0<<EN_PROG)
		  | (1<<ACK_DETECT)    	// pull up (0 active)
		  | (0<<DCC)
	      | (0<<NDCC)
		  | (1<<SHORT_MAIN)    	// pull up (0 active)
		  | (1<<SHORT_PROG)     // pull up (0 active)
		  | (0<<LED_RS232);    	// low = turn off
	DDRB  = (1<<EN_MAIN)  		// out
		  | (1<<EN_PROG)  		// out
		  | (0<<ACK_DETECT)		// in
		  | (1<<DCC)       		// out
	      | (1<<NDCC)     		// out
		  | (0<<SHORT_MAIN)		// in
		  | (0<<SHORT_PROG)		// in
		  | (1<<LED_RS232);		// out

	// Port D
	PORTD |= (1<<MY_RXD)			// pullup
	      | (1<<MY_TXD)
		  //| (0<<LED_ACK)
	      | (0<<LED_SHORT_MAIN)
	      | (0<<LED_SHORT_PROG);
	DDRD  |= (0<<MY_RXD)    	// in
	      | (1<<MY_TXD)   		// out
		  //| (1<<LED_ACK)		// out
	      | (1<<LED_SHORT_MAIN)	// out
	      | (1<<LED_SHORT_PROG);// out

    ACSR=0x80; // 0b100000000 (ACD = On) - Analog comparator disabled
	//SFIOR=0x00;
	MCUCR &= 0b01111111; // PUD = 0: no "pull up disabled"
}
//----------------------------------------------------------------------------------------
void InitInterrupt()
{
    TIMSK = (1<<OCIE1A)      // Timer1/Counter1 Compare A Interrupt; reassigned in InitDCCOut
          | (0<<OCIE1B)      // Timer1/Counter1 Compare B Interrupt
          | (0<<TOIE1)       // Timer1/Counter1 Overflow Interrupt
		  | (0<<ICIE1)       // Timer1/Counter1 Capture Interrupt
          | (0<<OCIE0A)      // Timer0/Counter0 Compare A Interrupt
          | (0<<OCIE0B)      // Timer0/Counter0 Compare B Interrupt
		  | (0<<TOIE0);      // Timer0/Counter0 Overflow Interrupt

	//GIMSK = (1<<PCIE);		 // pin change interrupt enabled
	//PCMSK = 0b01100100;		 // PCINT 2,5,7 enabled

	sei();
}
//----------------------------------------------------------------------------------------
int main()
{
	InitHardware();
	InitInterrupt();
    InitDCCOut();
    InitRS232(BAUD_57600);//BAUD_19200);

	MAIN_TRACK_OFF;
	PROG_TRACK_OFF;

	/*
    #if (XPRESSNET_ENABLED == 1)
    	InitRS485();
    	InitXpressNet();
    #endif
    InitState();                      // 5ms timer tick 
    InitParser();                     // command parser
    InitOrganizer();                  // engine for command repetition, memory of loco speeds and types
    InitProgrammer();             	  // State Engine des Programmers
    #if (S88_ENABLED == 1)
    	InitS88(READ_FROM_EEPROM);  
    #endif
    #if (DMX_ENABLED == 1)
    	InitDMXOut();
    #endif

    if (eeprom_read_byte((void*)eadr_OpenDCC_Version) != OPENDCC_VERSION)
    {
        // oops, no data loaded or wrong version! 
        error_blink_leds();
        while (1)
        {
            // new: at this point we need running timerval
            _delay_us(5000L);
            timerval++;
            led_tick();
        }
    }

    SetOpenDCCState(RUN_OFF);         // start up with power off
    //SetOpenDCCState(RUN_OKAY);         // start up with power enabled
    //SendStartupMessages();          // issue defined power up sequence on tracks
	*/
    
    while (1)
    {
    	CheckState();     // check short
        //RunOrganizer(); // run command organizer, depending on state, it will execute normal track operation or programming
        //RunProgrammer();
        //#if (XPRESSNET_ENABLED == 1 && LOCO_DATABASE == NAMED)
        //	run_database();                  // check transfer of loco database 
        //#endif
        RunParser(); // check commands from PC
        //#if (S88_ENABLED == 1)
        //    if (!is_prog_state()) run_s88(); // s88 has busy loops, we block it when programming
        //                                     // this is no longer true - but keep it blocked
        //#endif

        //#if (XPRESSNET_ENABLED == 1)
        //    RunXpressNet();
        //#endif
    }

	return 0;
}
//----------------------------------------------------------------------------------------
