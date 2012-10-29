using System.ComponentModel;

namespace Typhoon.Decoders
{
    public enum SpeedSteps
    {
        // int value is max speed step
        [Description("DCC 14")]
        Speed14 = 14,
        [Description("DCC 28")]
        Speed28 = 28,
        [Description("DCC 128")]
        Speed128 = 126
    }
}
