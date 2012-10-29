#ifndef __RS232_H__
#define __RS232_H__

typedef enum {
	BAUD_9600 = 0,
	BAUD_19200 = 1,
	BAUD_38400 = 2,
	BAUD_57600 = 3,
	BAUD_115200 = 4,
	BAUD_2400 = 5,        // used for Intellibox
	BAUD_4800 = 6,        // used for Intellibox
} BaudRate;

void InitRS232(BaudRate baudRate);
unsigned char USARTReadChar();
void USARTWriteChar(unsigned char data);
void USARTWriteString(const char *s);
#endif
