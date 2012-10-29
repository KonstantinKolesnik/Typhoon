#include <stdlib.h>
//#include <stdbool.h>
#include <inttypes.h>
#include <avr/io.h>
#include <avr/interrupt.h>
//#include <avr/pgmspace.h>         // put var to program memory
//#include <avr/eeprom.h>
#include <string.h>
#include "hardware.h"
#include "rs232.h"
//----------------------------------------------------------------------------------------
//========================================================================
// USART definitions
//========================================================================
#define my_UCSRA  UCSRA
	#define my_RXC    RXC
    #define my_TXC    TXC
    #define my_UDRE   UDRE
    #define my_FE     FE
    #define my_DOR    DOR
    #define my_UPE    UPE
    #define my_U2X    U2X
    //#define my_MPCP    MPCP
#define my_UCSRB  UCSRB
	#define my_RXCIE  RXCIE
	//#define my_TXCIE  TXCIE
    #define my_UDRIE  UDRIE
    #define my_RXEN   RXEN
    #define my_TXEN   TXEN
    #define my_UCSZ2  UCSZ2
	//#define my_RXB8   RXB8
	#define my_TXB8   TXB8
#define my_UCSRC  UCSRC
	//#define my_URSEL  URSEL // ?????
    #define my_UMSEL  UMSEL
    #define my_UPM1   UPM1 
    #define my_UPM0   UPM0
    #define my_USBS   USBS
    #define my_UCSZ1  UCSZ1 
    #define my_UCSZ0  UCSZ0
    #define my_UCPOL  UCPOL
#define my_UBRRL  UBRRL
#define my_UBRRH  UBRRH
#define my_UDR    UDR

BaudRate actualBaudRate;

//----------------------------------------------------------------------------------------
void InitRS232(BaudRate baudRate)
{
    unsigned char sreg = SREG;
    cli();

    actualBaudRate = baudRate;
	uint16_t ubrr; // baud rate value

	// calculations are done at mult 100 to avoid integer cast errors +50 is added
    
	// ubrr = baudRate * 16 / F_CPU - 1
	switch (baudRate)
	{
	    default:
		case BAUD_2400:
		    // ubrr = (uint16_t) ((uint32_t) F_CPU/(16*2400L) - 1);
            ubrr = (uint16_t) ((uint32_t)(F_CPU/(16*24L) - 100L + 50L) / 100);
			my_UCSRA = (1 << my_RXC) | (1 << my_TXC);
		    break;
		case BAUD_4800:
		    // ubrr = (uint16_t) ((uint32_t) F_CPU/(16*4800L) - 1);
            ubrr = (uint16_t) ((uint32_t) (F_CPU/(16*48L) - 100L + 50L) / 100);
			my_UCSRA = (1 << my_RXC) | (1 << my_TXC);
		    break;
		case BAUD_9600:
		    //   UBRRL = 103; // 19200bps @ 16.00MHz
			// ubrr = (uint16_t) ((uint32_t) F_CPU/(16*9600L) - 1);
            ubrr = (uint16_t) ((uint32_t) (F_CPU/(16*96L) - 100L + 50L) / 100);
			my_UCSRA = (1 << my_RXC) | (1 << my_TXC);
		    break;
		case BAUD_19200:
		    //   UBRRL = 51; // 19200bps @ 16.00MHz
			//ubrr = (uint16_t) ((uint32_t) F_CPU/(16*19200L) - 1);
            ubrr = (uint16_t) ((uint32_t) (F_CPU/(16*192L) - 100L + 50L) / 100);
			my_UCSRA = (1 << my_RXC) | (1 << my_TXC);
		    break;
		case BAUD_38400:
		    //   UBRRL = 25; // 38400bps @ 16.00MHz
			// ubrr = (uint16_t) ((uint32_t) F_CPU/(16*38400L) - 1);
            ubrr = (uint16_t) ((uint32_t) (F_CPU/(16*384L) - 100L + 50L) / 100);
			my_UCSRA = (1 << my_RXC) | (1 << my_TXC) ; // High Speed Mode, nur div 8
		    break;
		case BAUD_57600:
		    // ubrr = (uint16_t) ((uint32_t) F_CPU/(8*57600L) - 1);
            ubrr = (uint16_t) ((uint32_t) (F_CPU/(8*576L) - 100L + 50L) / 100);
			my_UCSRA = (1 << my_RXC) | (1 << my_TXC) | (1 << my_U2X);  // High Speed Mode, nur div 8
		    break;
		case BAUD_115200:
		    // ubrr = (uint16_t) ((uint32_t) F_CPU/(8*115200L) - 1);
            ubrr = (uint16_t)((uint32_t)(F_CPU/(8*1152L) - 100L + 50L) / 100);
			my_UCSRA = (1 << my_RXC) | (1 << my_TXC) | (1 << my_U2X);  // High Speed Mode
		    break;
	}
    my_UBRRH = (uint8_t)(ubrr>>8);
    my_UBRRL = (uint8_t)ubrr;

	// enable Receiver, Transmitter
    my_UCSRB = 0; // stop everything
    my_UCSRB = (1 << my_RXEN) | (1 << my_TXEN);

    // Data mode 8N1, async
    my_UCSRC = (0 << my_UMSEL)        // 0 = asynchronous mode
             | (0 << my_UPM1)         // 00 = parity disabled
             | (0 << my_UPM0)         
             | (0 << my_USBS)         // 1 = tx with 2 stop bits, 0 - 1 stop bit
             | (1 << my_UCSZ1)        // 11 = 8 or 9 bits, depends on my_UCSZ2
             | (1 << my_UCSZ0)
             | (0 << my_UCPOL);		  // clock polarity; 0 for async

    // Flush Receive-Buffer
	unsigned char dummy;
    do
    {
        dummy = my_UDR;
    }
    while (my_UCSRA & (1 << my_RXC));
    my_UCSRA |= (1 << my_RXC);
    my_UCSRA |= (1 << my_TXC);
    dummy = my_UDR;

    LED_RS232_OFF;
    
    SREG = sreg;
}
//----------------------------------------------------------------------------------------
unsigned char USARTReadChar()
{
	unsigned char res;

	//Wait untill a data is available
	//while(!(my_UCSRA & (1<<my_RXC))); // this leads to continuous waiting for PC command
	if (!(my_UCSRA & (1<<my_RXC)))
		return 0;

	//Now USART has got data from host and is available is buffer
	res = my_UDR;

	// В случае ошибки вернуть 0
	if (my_UCSRA & ((1<<my_FE)|(1<<my_DOR)|(1<<my_UPE)))
		return 0;
	return res;
}
//----------------------------------------------------------------------------------------
void USARTWriteChar(unsigned char data)
{
	//Wait untill the transmitter is ready
	while(!(my_UCSRA & (1<<my_UDRE)));
	//Now write the data to USART buffer
	my_UDR = data;
}
//----------------------------------------------------------------------------------------
void USARTWriteString(const char* s)
{
    LED_RS232_ON;
	
	char c;
	while ((c = *s++)) USARTWriteChar(c);

	LED_RS232_OFF;
}
//----------------------------------------------------------------------------------------









//----------------------------------------------------------------------------------------
/*
void pc_send(unsigned char out) // send single char
{
    while (!TXBufferReady());  // busy, waiting!!!
    TXBufferWrite(out);        // send character
}
void pc_send_string_P(const char *progmem_string) // send string in PROGMEM
{
    char c;
    while ((c = pgm_read_byte(progmem_string++))) // read from Flash
        pc_send(c);
}
void pc_send_answer_P(const char *progmem_string)   // send string in PROGMEM + cr ]
{
    char c;
    while (c = pgm_read_byte(progmem_string++))
        pc_send(c);
    pc_send(0x0D);
    pc_send(']');   // answer sequence is '<CR>]' 
}
void pc_send_num(unsigned int num) // send an int + leading blank
{
    char buffer[6];         // max. 5 digits
    unsigned char i = 0;

    itoa(num, buffer, 10);         // 10 byte long
    pc_send(' ');
    while (buffer[i])
        pc_send(buffer[i++]);
}
void pc_send_int(unsigned int num) // send an int
{
    char buffer[6];         // max. 5 digits
    unsigned char i = 0;

    itoa(num, buffer, 10);         // 10 byte long
    while (buffer[i])
        pc_send(buffer[i++]);
}

const char hexdig[] PROGMEM = "0123456789ABCDEF";
void pc_send_hex(unsigned char value)                  // send a char as hex, leading 0
  {
    unsigned char i=0;

    i = value >> 4;
    i &= 0x0f;
    pc_send(pgm_read_byte(&hexdig[i]));

    i = value & 0x0f;
    pc_send(pgm_read_byte(&hexdig[i]));
  }

void pc_send_bin(unsigned char value)                  // send a char as bin, leading 0
{
    unsigned char i = 0;

    for (i = 0; i < 8; i++)
    {
        if (value & 0x80)    pc_send('1');
        else                 pc_send('0');
        value = value << 1;
    }
}
*/




