#include <stdlib.h>
//#include <stdbool.h>
//#include <inttypes.h>
#include <avr/io.h>
#include <avr/interrupt.h>
//#include <avr/pgmspace.h>         // put var to program memory
//#include <avr/eeprom.h>
//#include <string.h>
#include "parser.h"
#include "rs232.h"
#include "hardware.h"
#include "dccout.h"
//----------------------------------------------------------------------------------------
#define MAX_PCC_SIZE 7 				// pc message max bytes

unsigned char pcc[MAX_PCC_SIZE];    // pc message
unsigned char pccIdx = 0;        	// current write or read index
unsigned char pccSize = 0;			// pc message byte count

void ParseCommand();
//----------------------------------------------------------------------------------------
void RunParser()
{
    unsigned char c = USARTReadChar();
	if (c != 0)
	{
		if (c != '*') // '*' - is end of command
		{
			pcc[pccIdx] = c;
			pccIdx++;
			if (pccIdx >= MAX_PCC_SIZE)
				pccIdx = 0;
		}
		else
		{
			pcc[pccIdx] = 0;
			pccSize = pccIdx;
			pccIdx = 0;
			ParseCommand(); // pcc is w/o '*' at the end
		}
	}
}
//----------------------------------------------------------------------------------------
void ParseCommand()
{
	if (pccSize == 0)
		return;

	LED_RS232_ON;

	unsigned char dccSize = 0;
	unsigned char i;

	switch (pcc[0])
	{
		case 'D': // dcc command
			dccSize = (unsigned char)pcc[1];
			NextMessage.Size = dccSize;
			for (i = 0; i < dccSize; i++)
				NextMessage.Dcc[i] = (unsigned char)pcc[i + 2];
			NextMessageCount = 1;
			break;
		case 'S': // station command
			if (pcc[1] == 'R' && NextMessageCount == 0) // query for ready
				USARTWriteString("SR*");
			if (pcc[1] == '0') // main track on
				MAIN_TRACK_ON;
			if (pcc[1] == '1') // main track off
				MAIN_TRACK_OFF;
			if (pcc[1] == '2') // prog track on
				PROG_TRACK_ON;
			if (pcc[1] == '3') // prog track off
				PROG_TRACK_OFF;
			if (pcc[1] == '4') // Railcom On
				DCCOutEnableRailcom();
			if (pcc[1] == '5') // Railcom Off
				DCCOutDisableRailcom();
			break;
	}

	LED_RS232_OFF;
}
