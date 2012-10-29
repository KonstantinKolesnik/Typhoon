#include "Hardware.h"
#include <avr/io.h>
#include <avr/iom32.h>
#include <util/delay.h>
#include <avr/interrupt.h>
#include "USB.h"
#include "ShortCircuitDetector.h"
#include "DCCOut.h"
#include "AckDetector.h"
//****************************************************************************************
void InitHardware()
{
	MCUCR &= 0b01111111;			// PUD bit = Off: pull up enabled
	//ACSR = 0b10000000;				// ACD bit = On: Analog comparator disabled
	ACSR |= (1 << ADC);				// ACD bit = On: Analog comparator disabled
	ADCSRA = 0b00000000;			// ADEN bit (#7) = Off: AD convertor disabled
	//ADCSRA |= (0 << ADEN);			// ADEN bit (#7) = Off: AD convertor disabled
	
	// Port A
	PORTA |= (1<<ACK_DETECT);			// pull up
	DDRA  |= (0<<ACK_DETECT);    		// in
		  
	// Port B	  
	PORTB	|= (0<<EN_MAIN)				// off
			|  (0<<LED_SHORT_MAIN)		// off
			|  (0<<EN_PROG)				// off
			|  (0<<LED_SHORT_PROG)		// off
			|  (0<<LED_MSG);			// off
	DDRB	|= (1<<EN_MAIN)				// out
			|  (1<<LED_SHORT_MAIN)		// out
			|  (1<<EN_PROG)				// out
			|  (1<<LED_SHORT_PROG)		// out
			|  (1<<LED_MSG);			// out
		  
	// Port C
	PORTC	|= (1<<SHORT_MAIN)			// pull up
			|  (1<<SHORT_PROG);			// pull up
	DDRC	|= (0<<SHORT_MAIN)			// in
			|  (0<<SHORT_PROG);			// in
	
	// Port D
	PORTD	|= (1<<MY_RXD)				// pull up
			|  (1<<MY_TXD)				// pull up
	        |  (0<<USB_DPLUS)			// pull down
			|  (0<<USB_DMINUS)			// pull down
			|  (0<<DCC)					// off
			|  (0<<NDCC);				// off
	DDRD	|= (0<<MY_RXD)    			// in
			|  (1<<MY_TXD)   			// out
	        |  (0<<USB_DPLUS)     		// in
			|  (0<<USB_DMINUS)     		// in
			|  (1<<DCC)       			// out
			|  (1<<NDCC);     			// out
}
void InitInterrupt()
{
    TIMSK = (1<<OCIE1A)     // Timer1/Counter1 Compare A Interrupt; reassigned in InitDCCOut
          | (0<<OCIE1B)     // Timer1/Counter1 Compare B Interrupt
          | (0<<TOIE1)      // Timer1/Counter1 Overflow Interrupt
		  | (0<<TICIE1)     // Timer1/Counter1 Capture Interrupt
		  
	      | (0<<OCIE2)      // Timer2/Counter2 Compare Interrupt
          | (0<<TOIE2)		// Timer2/Counter2 Overflow Interrupt
		  
		  | (1<<OCIE0)      // Timer0/Counter0 Compare Interrupt
		  | (0<<TOIE0);     // Timer0/Counter0 Overflow Interrupt

	sei();
}
//****************************************************************************************
int main()
{
	InitUSB();
	InitHardware();
	InitShortCircuitDetector();
	InitDCCOut();
	InitInterrupt();
	
	MAIN_TRACK_OFF;
	PROG_TRACK_OFF;
	
    while (1)
    {
		RunUSB();
		CheckAcknowledgement();
    }
}