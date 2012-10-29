#ifndef HARDWARE_H_
#define HARDWARE_H_
//****************************************************************************************
// 1. Processor Definitions
//****************************************************************************************
// ATMega32: 32KBytes FLASH, 2048Byte SRAM, 1024Byte EEPROM
#define F_CPU 12000000UL
//****************************************************************************************
// 2. Ports Definition
//****************************************************************************************
// Port B
#define EN_MAIN					0	//PB0 (XCK/T0)		pin 1
#define LED_SHORT_MAIN			1	//PB1 (T1)			pin 2
#define EN_PROG					2	//PB2 (INT2/AIN0)	pin 3
#define LED_SHORT_PROG			3	//PB3 (OC0/AIN1)	pin 4
#define LED_MSG					4	//PB4 (SS)			pin 5
//								5	//PB5 (MOSI)		pin 6
//								6	//PB6 (MISO)		pin 7
//								7	//PB7 (SCK)			pin 8
//****************************************************************************************
// Port D
#define MY_RXD					0	//PD0 (RXD)			pin 14
#define MY_TXD					1	//PD1 (TXD)			pin 15
#define USB_DPLUS				2	//PD2 (INT0) 		pin 16
#define USB_DMINUS				3	//PD3 (INT1) 		pin 17
#define NDCC					4	//PD4 (OC1B) 		pin 18
#define DCC						5	//PD5 (OC1A) 		pin 19
//								6	//PD6 (ICP) 		pin 20
//								7	//PD7 (OC2) 		pin 21
//****************************************************************************************
// Port A
//								0	//PA0 (ADC0)		pin 40
//								1	//PA1 (ADC1)		pin 39
//								2	//PA2 (ADC2)		pin 38
//								3	//PA3 (ADC3)		pin 37
//								4	//PA4 (ADC4)		pin 36
//								5	//PA5 (ADC5)		pin 35
#define ACK_DETECT				6	//PA6 (ADC6)		pin 34
//								7	//PA7 (ADC7)		pin 33
//****************************************************************************************
// Port C
#define SHORT_PROG				0	//PC0 (SCL)			pin 22
#define SHORT_MAIN				1	//PC1 (SD)			pin 23
#define TCK						2	//PC2 (TCK)			pin 24
#define TMS						3	//PC3 (TMS)			pin 25
#define TDO						4	//PC4 (TDO)			pin 26
#define TDI						5	//PC5 (TDI)			pin 27
//								6	//PC6 (TOSC1)		pin 28
//								7	//PC7 (TOSC2)		pin 29
//****************************************************************************************
// 3. LED-Control and IO-Macros
//****************************************************************************************
#define MAIN_TRACK_ON    		PORTB |= (1<<EN_MAIN)
#define MAIN_TRACK_OFF   		PORTB &= ~(1<<EN_MAIN)
#define MAIN_IS_ON 				(PINB & (1<<EN_MAIN))
#define MAIN_SHORT_DETECTED		(!(PINC & (1<<SHORT_MAIN)))
#define MAIN_SHORT_BLOCKED		(PINB & (1<<LED_SHORT_MAIN))

#define PROG_TRACK_ON    		PORTB |= (1<<EN_PROG)
#define PROG_TRACK_OFF   		PORTB &= ~(1<<EN_PROG)
#define PROG_IS_ON		 		(PINB & (1<<EN_PROG))
#define PROG_SHORT_DETECTED		(!(PINC & (1<<SHORT_PROG)))
#define PROG_SHORT_BLOCKED		(PINB & (1<<LED_SHORT_PROG))

#define ACK_DETECTED	  		(!(PINA & (1<<ACK_DETECT)))

#define LED_MSG_ON	     		PORTB |= (1<<LED_MSG)
#define LED_MSG_OFF	    		PORTB &= ~(1<<LED_MSG)

#define LED_SHORT_MAIN_ON     	PORTB |= (1<<LED_SHORT_MAIN)
#define LED_SHORT_MAIN_OFF    	PORTB &= ~(1<<LED_SHORT_MAIN)

#define LED_SHORT_PROG_ON     	PORTB |= (1<<LED_SHORT_PROG)
#define LED_SHORT_PROG_OFF    	PORTB &= ~(1<<LED_SHORT_PROG)
//****************************************************************************************
#endif /* HARDWARE_H_ */