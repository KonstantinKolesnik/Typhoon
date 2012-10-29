#include <stdbool.h>
#include <avr/io.h>
#include <util/delay.h>
#include "hardware.h"
#include "rs232.h"
//----------------------------------------------------------------------------------------
//bool shortMain = false;
//bool shortProg = false;
bool ackDetect = false;

void CheckState()
{
	/*
	// main track
	if (MAIN_IS_SHORT && !shortMain)
	{
		shortMain = true;
		MAIN_TRACK_OFF;
		LED_SHORT_MAIN_ON;
		USARTWriteString("MTS1*");
	}
	// program track
	if (PROG_IS_SHORT && !shortProg)
	{
		shortProg = true;
		PROG_TRACK_OFF;
		LED_SHORT_PROG_ON;
		USARTWriteString("PTS1*");
	}
	// ack detect
	if (ACK_IS_DETECTED && !ackDetect)
	{
		ackDetect = true;
		USARTWriteString("ACK1*");
	}

	if (!MAIN_IS_SHORT && shortMain)
	{
		_delay_ms(6000);
		shortMain = false;
		MAIN_TRACK_ON;
		LED_SHORT_MAIN_OFF;
		USARTWriteString("MTS0*");
	}
	if (!PROG_IS_SHORT && shortProg)
	{
		_delay_ms(6000);
		shortProg = false;
		PROG_TRACK_ON;
		LED_SHORT_PROG_OFF;
		USARTWriteString("PTS0*");
	}
	if (!ACK_IS_DETECTED && ackDetect)
	{
		ackDetect = false;
		USARTWriteString("ACK0*");
	}
	*/




	// main track
	if (MAIN_IS_SHORT)
	{
		MAIN_TRACK_OFF;
		LED_SHORT_MAIN_ON;
		USARTWriteString("MTS1*");
		_delay_ms(6000);
		LED_SHORT_MAIN_OFF;
		USARTWriteString("MTS0*");
	}
	// program track
	if (PROG_IS_SHORT)
	{
		PROG_TRACK_OFF;
		LED_SHORT_PROG_ON;
		USARTWriteString("PTS1*");
		_delay_ms(6000);
		LED_SHORT_PROG_OFF;
		USARTWriteString("PTS0*");
	}

	// ack detect
	if (ACK_IS_DETECTED && !ackDetect)
	{
		ackDetect = true;
		USARTWriteString("ACK1*");
	}
	if (!ACK_IS_DETECTED && ackDetect)
	{
		ackDetect = false;
		USARTWriteString("ACK0*");
	}
}

