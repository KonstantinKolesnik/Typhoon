#ifndef __HARDWARE_H__
#define __HARDWARE_H__

//========================================================================
// 1. Processor Definitions
//========================================================================
// ATTiny2313: 2KBytes FLASH, 128Byte SRAM, 128Byte EEPROM
#define F_CPU 8000000UL

//========================================================================
// 2. Ports Definition
//========================================================================

// Port A
//#define ???	    	0     // XTAL1
//#define ???	    	1     // XTAL2
//#define ???	    	2     // RESET/DW

// Port B
#define EN_MAIN    		0     // AIN0/PCINT0
#define EN_PROG  		1     // AIN1/PCINT1
#define ACK_DETECT 		2     // OC0A/PCINT2
#define DCC   			3     // OC1A/PCINT3
#define NDCC    		4     // OC1B/PCINT4
#define SHORT_MAIN     	5     // MOSI/DI/SDA/PCINT5
#define SHORT_PROG     	6     // MISO/DO/PCINT6
#define LED_RS232  		7     // UCSK/SCL/PCINT7

// Port D
#define MY_RXD     		0     // RXD
#define MY_TXD     		1     // TXD
//#define ???    		2     // CKOUT/XC/INT0
//#define ???    		3     // INT3
#define LED_SHORT_PROG  4     // T0
#define LED_SHORT_MAIN	5     // OC0B/T1
//#define LED_ACK		 	6     // ICP

//========================================================================
// 3. LED-Control and IO-Macros
//========================================================================

#define MAIN_TRACK_ON    		PORTB |= (1<<EN_MAIN)
#define MAIN_TRACK_OFF   		PORTB &= ~(1<<EN_MAIN)
#define MAIN_IS_ON 				(PINB & (1<<EN_MAIN))
#define MAIN_IS_SHORT    		(!(PINB & (1<<SHORT_MAIN)))

#define PROG_TRACK_ON    		PORTB |= (1<<EN_PROG)
#define PROG_TRACK_OFF   		PORTB &= ~(1<<EN_PROG)
#define PROG_IS_ON		 		(PINB & (1<<EN_PROG))
#define PROG_IS_SHORT    		(!(PINB & (1<<SHORT_PROG)))

#define ACK_IS_DETECTED  		(!(PINB & (1<<ACK_DETECT)))

#define LED_RS232_ON     		PORTB |= (1<<LED_RS232)
#define LED_RS232_OFF    		PORTB &= ~(1<<LED_RS232)

#define LED_SHORT_MAIN_ON     	PORTD |= (1<<LED_SHORT_MAIN)
#define LED_SHORT_MAIN_OFF    	PORTD &= ~(1<<LED_SHORT_MAIN)

#define LED_SHORT_PROG_ON     	PORTD |= (1<<LED_SHORT_PROG)
#define LED_SHORT_PROG_OFF    	PORTD &= ~(1<<LED_SHORT_PROG)

//#define LED_ACK_ON     			PORTB |= (1<<LED_ACK)
//#define LED_ACK_OFF    			PORTB &= ~(1<<LED_ACK)
#endif
