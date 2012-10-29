#ifndef __DCCOUT_H__
#define __DCCOUT_H__

#define MAX_DCC_SIZE 4

/*
typedef enum {is_void,      // message with no special handling (like functions)
              is_stop,      // broadcast
              is_loco,      // standard dcc speed command
              is_acc,       // accessory command
              is_feedback,  // accessory command with feedback
              is_prog,      // service mode - longer preambles
              is_prog_ack}  MessageType;
*/

typedef struct 
{
    unsigned char Size;
    unsigned char Dcc[MAX_DCC_SIZE];
    //MessageType   Type;
} Message;

extern Message NextMessage;
extern volatile unsigned char NextMessageCount; // load message and set count
                                                // if > 0 -> output next_message
											    // if = 0 -> ready for next_message

void InitDCCOut();
void DCCOutEnableRailcom();  // create railcom cutout
void DCCOutDisableRailcom();
unsigned char DCCOutQueryRailcom();
#endif
