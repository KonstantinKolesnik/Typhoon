using System.Collections.Generic;

namespace Typhoon.Decoders
{
    //public enum LocomotiveDecoderType
    //{
    //    Standard,
    //    Branchline36554
    //}
    /*
    public class LocomotiveDecoder
    {
        #region Fields
        private LocomotiveDecoderType type = LocomotiveDecoderType.Standard;
        private string name = "Standard";
        private static List<CVInfo> allCV = new List<CVInfo>();
        private List<CVInfo> cvs = new List<CVInfo>();
        #endregion

        #region Properties
        public LocomotiveDecoderType Type
        {
            get { return type; }
        }
        public string Name
        {
            get { return name; }
        }
        public List<CVInfo> CVs
        {
            get { return cvs; }
        }
        #endregion

        #region Constructor
        static LocomotiveDecoder()
        {
            FillAllCV();
        }
        public LocomotiveDecoder(LocomotiveDecoderType type)
        {
            this.type = type;
            switch (type)
            {
                default:
                case LocomotiveDecoderType.Standard:
                    name = "Standard";
                    cvs.AddRange(allCV.GetRange(0, allCV.Count));
                    break;
                case LocomotiveDecoderType.Branchline36554:
                    name = "Branchline 36554";
                    cvs.Add(CVInfo(1));
                    cvs.Add(CVInfo(2).Alter(1, 63, 3, 3));
                    cvs.Add(CVInfo(3).Alter(0, 63, 8, 8));
                    cvs.Add(CVInfo(4).Alter(0, 63, 6, 6));
                    cvs.Add(CVInfo(5).Alter(0, 63, 63, 63));
                    cvs.Add(CVInfo(7).Alter(null, null, 0, 0));
                    cvs.Add(CVInfo(8).Alter(null, null, 151, 151));
                    cvs.Add(CVInfo(17).Alter(192, 231, 192, 192));
                    cvs.Add(CVInfo(18).Alter(0, 255, 0, 0));
                    cvs.Add(new CVInfo(29, null, null, 6, 6, "Configurations Supported",
                        "Bit 0 = Locomotive Direction: 0 = normal, 1 = reversed. This bit controls the locomotive's forward and backward direction in digital mode only. Directional sensitive functions, such as headlights (FL and FR), will also be reversed so that they line up with the locomotive’s new forward direction. See RP-9.1.1 for more information." +
                        "\nBit 1 = Speed Steps: 0 = 14 steps, 1 = 28/128 steps." +
                        "\nBit 2 = Operation on DC: 0 = Disabled, 1 = Enabled." +
                        "\nBits 3-4 not used." +
                        "\nBit 5 = 0 = one byte addressing, 1 = two byte addressing (also known as extended addressing), See RP 9.2.1 for more information." +
                        "\nBits 6-7 not used."));
                    cvs.Add(new CVInfo(49, 0, 1, 1, 1, "Back EMF Selector",
                        "Bit 0 = Enable/Disable. 0 = Disabled 1 = Enabled." +
                        "\nBits 1-7 not used."
                        ));
                    cvs.Add(new CVInfo(51, 0, 1, 1, 1, "DC Brake Control",
                        "Bit 0 = Enable/Disable Lenz DC brake mode. 0 = Disabled 1 = Enabled." +
                        "\nBits 1-7 not used."
                        ));
                    cvs.Add(new CVInfo(54, 0, 63, 32, 32, "Feedback Parameter K", "Determines the load control effect. The higher the value, the stronger the impact on the motor."));
                    cvs.Add(new CVInfo(55, 0, 63, 24, 24, "Feedback Parameter I", "Determines the momentum of the motor. Motors with large flywheels of large diameter require a smaller value."));
                    cvs.Add(new CVInfo(63, 0, 7, 7, 7, "Function Brightness", "Applies to both F0 and F1.."));
                    break;


            }
        }
        #endregion

        #region Public methods
        public CVInfo CVInfo(uint number)
        {
            foreach (CVInfo el in allCV)
                if (el.Number == number)
                    return el;
            return null;
        }
        public override string ToString()
        {
            return name;
        }
        #endregion

        #region Private methods
        private static void FillAllCV()
        {
            allCV.Clear();
            allCV.Add(new CVInfo(1, 1, 127, 3, 3, "Primary Address", "Bits 0-6 contain an address with a value between 1 and 127. Bit seven must have a value of 0. If the value of Configuration Variable #1 is 0 then the decoder will go out of NMRA digital mode and convert to the alternate power source as defined by CV#12. This setting will not affect the Digital Decoder's ability to respond to service mode packets (see RP 9.2.3). The default value for this Configuration Variable is 3, if the decoder is not installed in a locomotive or other unit when shipped from the manufacturer."));
            allCV.Add(new CVInfo(2, 0, 255, null, null, "Vstart", "Vstart is used to define the voltage drive level used as the start voltage on the motor. The voltage drive levels shall correspond linearly to the voltage applied to the motor at speed step one, as a fraction of available rectified supply voltage. When the voltage drive level is equal to zero, there shall be zero voltage applied to the motor. When it is at maximum 255, the full available rectified voltage shall be applied."));
            allCV.Add(new CVInfo(3, null, null, null, null, "Acceleration Rate", "Determines the decoder's acceleration rate. The formula for the acceleration rate shall be equal to (the contents of CV#3 * 0.896)/(number of speed steps in use). For example, if the contents of CV#3 = 2, then the acceleration is 0.064 sec/step for a decoder currently using 28 speed steps. If the content of this parameter equals 0 then there is no programmed momentum during acceleration."));
            allCV.Add(new CVInfo(4, null, null, null, null, "Deceleration Rate", "Determines a decoders braking rate, in the same fashion as acceleration (CV#3)."));
            allCV.Add(new CVInfo(5, 0, 255, null, null, "Vhigh", "Vhigh is used to specify the motor voltage drive levels at the maximum speed step. This value shall be specified as a fraction of available rectified supply voltage. When the contents of CV#5 equal 255, the full available rectified voltage shall be applied. Values of 0 or 1 shall indicate that Vhigh is not used in the calculation of the speed table."));
            allCV.Add(new CVInfo(6, null, null, null, null, "Vmid", "Vmid specifies the voltage drive level at the middle speed step. Vmid is used to generate a performance curve in the decoder that translate speed step values into motor voltage drive levels and is specified as a fraction of available rectified supply voltage. Values of 0 or 1 shall indicate that Vmid is not used in the calculation of the speed table."));
            allCV.Add(new CVInfo(7, null, null, null, null, "Manufacturer Version Number", "This is reserved for the manufacturer to store information regarding the version of the decoder."));
            allCV.Add(new CVInfo(8, null, null, null, null, "Manufacturer ID", "CV#8 shall contain the NMRA assigned id number of the manufacturer of this decoder. The currently assigned manufacturer ID codes are listed in Appendix A of this Recommended Practice. The use of a value not assigned by the NMRA shall immediately cause the decoder to not be in conformance to this RP. The CV shall be implemented as a read-only value, which cannot be modified."));
            allCV.Add(new CVInfo(9, null, null, null, null, "Total PWM Period", "The value of CV#9 sets the nominal PWM period at the decoder output and therefore the frequency is proportional to the reciprocal of the value. The recommend formula for PWM period should be: PWM period (uS) = (131 + MANTISSA x 4) x 2 EXP, where MANTISSA is bits 0-4 of CV#9 (low order) and EXP is bits 5-7 for CV#9. If the value programmed into CV#9 falls outside a decoder's capability, it is suggested (but not required) that the decoder \"adjust\" the value to the appropriate highest or lowest setting supported by the decoder."));
            allCV.Add(new CVInfo(10, null, null, null, null, "EMF Feedback Cutout", "Contains a value between 1 and 128 that indicates the speed step above which the back EMF motor control cuts off. When 14 or 28 speed steps are used the LSB's of the value are truncated appropriately."));
            allCV.Add(new CVInfo(11, null, null, null, null, "Packet time-out Value", "Contains the maximum time period that the decoder will maintain its speed without receiving a valid packet. See RP 9.2.4 Section C for further information."));
            allCV.Add(new CVInfo(12, null, null, null, null, "Power Source Conversion", "Contains the identity of the alternate power source to which the decoder will be converted should CV#1 contain all zeros. This is also the primary alternative power source selected should the decoder perform power source conversion. The currently assigned Power Source Conversion codes are listed in Appendix B of RP 9.2.2."));
            allCV.Add(new CVInfo(13, null, null, null, null, "Alternate Mode Function Status", "Indicates the status of each function (F1 through F8) when the unit is operating in alternate power mode, which cannot control the functions. If a function can be controlled, then the corresponding bit is ignored. A value of 0 indicates the function is off, while a value of 1 indicates the function is on. Bit 0 corresponds to F1, while Bit 7 corresponds to F8."));
            allCV.Add(new CVInfo(14, null, null, null, null, "Alternate Mode Function 2 Status", "Indicates the status of each function (F9 through F12, & FL) when the unit is operating in alternate power mode, which cannot control the functions. If a function can be controlled, then the corresponding bit is ignored. A value of 0 indicates the function is off, while a value of 1 indicates the function is on. FL in the forward direction is controlled by bit 0, FL in the reverse direction is controlled by bit 1. Bit 2 corresponds to F9, while Bit 5 corresponds to F12."));
            allCV.Add(new CVInfo(15, null, null, null, null, "Decoder Lock", "The Decoder Lock is used to change CVs in only one of several decoders with the same short address (CV#1) or long address (CV#17 and CV#18) that are installed in the same locomotive. Assign a number to CV#16 in each decoder (i.e. 1 to motor decoder, 2 to sound decoder, 3 or higher to other decoders) before the decoders are installed in the locomotive. To change a value in another CV of one of the installed decoders, first write the number 1 (motor), 2 (sound), or 3 or higher (other) into CV#15, then send the new value to the CV to be changed. The decoders will compare CV#15 to CV#16 and, if the values are equal, the CV to be changed will be changed. If the values in CV#15 and CV#16 are different, the update will be ignored."));
            allCV.Add(new CVInfo(16, null, null, null, null, "Decoder Lock", "The Decoder Lock is used to change CVs in only one of several decoders with the same short address (CV#1) or long address (CV#17 and CV#18) that are installed in the same locomotive. Assign a number to CV#16 in each decoder (i.e. 1 to motor decoder, 2 to sound decoder, 3 or higher to other decoders) before the decoders are installed in the locomotive. To change a value in another CV of one of the installed decoders, first write the number 1 (motor), 2 (sound), or 3 or higher (other) into CV#15, then send the new value to the CV to be changed. The decoders will compare CV#15 to CV#16 and, if the values are equal, the CV to be changed will be changed. If the values in CV#15 and CV#16 are different, the update will be ignored."));
            allCV.Add(new CVInfo(17, 192, 231, null, null, "Extended Address, MSB", "The Extended Address is the locomotives address when the decoder is set up for extended addressing (indicated by a value of 1 in bit location 5 of CV#29). CV#17 contains the most significant bits of the two 135 byte address and must have a value between 11000000 and 11100111, inclusive, in order for this two byte address to be valid. CV#18 contains the least significant bits of the address and may contain any value."));
            allCV.Add(new CVInfo(18, 0, 255, null, null, "Extended Address, LSB", "The Extended Address is the locomotives address when the decoder is set up for extended addressing (indicated by a value of 1 in bit location 5 of CV#29). CV#17 contains the most significant bits of the two 135 byte address and must have a value between 11000000 and 11100111, inclusive, in order for this two byte address to be valid. CV#18 contains the least significant bits of the address and may contain any value."));
            allCV.Add(new CVInfo(19, null, null, null, null, "Consist Address", "Contains a seven bit address in bit positions 0-6. Bit 7 indicates the relative direction of this unit within a consist, with a value of 0 indicating normal direction, and a value of 1 indicating a direction opposite the unit's normal direction. If the seven bit address in bits 0-6 is 0 the unit is not in a consist."));
            //allCV.Add(new CVInfo(20, null, null, null, null, "NMRA Reserved", ""));
            allCV.Add(new CVInfo(21, null, null, null, null, "Consist Address Active for F1-F8", "Defines for functions F1-F8 whether the function is controlled by the consist address. For each Bit a value of 1 indicates that the function will respond to instructions addressed to the consist address. A value of 0 indicates that the function will only respond to instructions addressed to the locomotive address. F1 is indicated by bit 0. F8 by bit 7."));
            allCV.Add(new CVInfo(22, null, null, null, null, "Consist Address Active for FL and F9-F12", "Defines for function FL whether the function is controlled by the consist address. For each Bit a value of 1 indicates that the function will respond to instructions addressed to the consist address. A value of 0 indicates that the function will only respond to instructions addressed to the locomotive address. FL in the forward direction is indicated by bit 0, FL in the reverse direction is controlled by bit 1. Bit 2 corresponds to F9, while Bit 5 corresponds to F12."));
            allCV.Add(new CVInfo(23, null, null, null, null, "Acceleration Adjustment", "Contains additional acceleration rate information that is to be added to or subtracted from the base value contained in CV#3 using the formula (the contents of CV#23  * 0.896)/(number of speed steps in use). This is a 7 bit value (bits 0-6) with bit 7 being reserved for a sign bit (0-add, 1-subtract). In case of overflow the maximum acceleration rate shall be used. In case of underflow no acceleration shall be used. The expected use is for changing momentum to simulate differing train lengths/loads, most often when operating in a consist."));
            allCV.Add(new CVInfo(24, null, null, null, null, "Deceleration Adjustment", "Contains additional braking rate information that is to be added to or subtracted from the base value contained in CV#4 using the formula (the contents of CV#24 * 0.896)/(number of speed steps in use). This is a 7 bit value (bits 0-6) with bit 7 being reserved for a sign bit (0-add,1-subtract). In case of overflow the maximum deceleration rate shall be used. In case of underflow no deceleration shall be used. The expected use is for changing momentum to simulate differing train lengths/loads, most often when operating in a consist."));
            allCV.Add(new CVInfo(25, null, null, null, null, "Speed Table/Mid Range Cab Speed Step", "A value between 2 and 127 shall be used to indicate 1 of 126 factory preset speed tables. A value of 2 indicates that the curve shall be linear. A value between 128 and 154 defines the 28-speed step position (1-26) which will define where the mid range decoder speed value will be applied. In 14-speed mode the decoder will utilize this value divided by two If the value in this variable is outside the range, the default mid cab speed of 14 (for 28 speed mode or 7 for 14 speed mode) shall be used as the mid speed value. Values of 0 or 1 shall indicate that this CV is not used in the calculation of the speed table."));
            //allCV.Add(new CVInfo(26, null, null, null, null, "NMRA Reserved", ""));
            allCV.Add(new CVInfo(27, null, null, null, null, "Decoder Automatic Stopping Configuration", "Used to configure which actions will cause the decoder to automatically stop. Bit 0 = Enable/Disable Auto Stop in the presence of an asymmetrical DCC signal which is more positive on the right rail. 0 = Disabled, 1 = Enabled. Bit 1 = Enable/Disable Auto Stop in the presence of an asymmetrical DCC signal which is more positive on the left rail. 0 = Disabled, 1 = Enabled. Bit 2 = Enable/Disable Auto Stop in the presence of an Signal Controlled Influence cutout signal. 0 = Disabled 1 = Enabled. Bit 3 = Reserved for Future Use. Bit 4 = Enable/Disable Auto Stop in the presence of reverse polarity DC. 0 = Disabled 1 = Enabled. Bit 5 = Enable/Disable Auto Stop in the presence forward polarity DC. 0 = Disabled 1 = Enabled. Bits 6-7 = Reserved for future use. Note If the decoder does not support a feature contained in this table, it shall not allow the corresponding bit to be set improperly (i.e. the bit should always contain it’s default value)."));
            allCV.Add(new CVInfo(28, null, null, null, null, "Bi-Directional Communication Configuration",
                "Used to configure decoder’s Bi-Directional communication characteristics when CV29-Bit 3 is set." +
                "\nBit 0 = Enable/Disable Unsolicited Decoder Initiated Transmission. 0 = Disabled 1 = Enabled" +
                "\nBit 1 = Enable/Disable Initiated Broadcast Transmission using Asymmetrical DCC Signal. 0 = Disabled 1 = Enabled" +
                "\nBit 2 = Enable/Disable Initiated Broadcast Transmission using Signal Controlled Influence Signal. 0 = Disabled 1 = Enabled" +
                "\nBits 3-5 = Reserved for future use" +
                "\nBits 6-7 = Flag Bits, Reserved for future use"));
            allCV.Add(new CVInfo(29, "Configurations Supported",
                "Bit 0 = Locomotive Direction: 0 = normal, 1 = reversed. This bit controls the locomotive's forward and backward direction in digital mode only. Directional sensitive functions, such as headlights (FL and FR), will also be reversed so that they line up with the locomotive’s new forward direction. See RP-9.1.1 for more information." +
                "\n\nBit 1 = FL location: 0 = bit 4 in Speed and Direction instructions control FL, 1 = bit 4 in function group one instruction controls FL. See RP-9.2.1 for more information." +
                "\n\nBit 2 = Power Source Conversion: 0 = NMRA Digital Only, 1 = Power Source Conversion Enabled, See CV#12 for more information." +
                "\n\nBit 3 = Bi-Directional Communications: 0 = Bi-Directional Communications disabled, 1 = Bi-Directional Communications enabled. See RP-9.3.2 for more information." +
                "\n\nBit 4 = Speed Table: 0 = speed table set by CV#2, CV#5, CV#6, 1 = Speed Table set by CV#66-#95." +
                "\n\nBit 5 = 0 = one byte addressing, 1 = two byte addressing (also known as extended addressing), See RP 9.2.1 for more information." +
                "\n\nBit 6 = Reserved for future use." +
                "\n\nBit 7 = Accessory Decoder: 0 = Multifunction Decoder, 1 = Accessory Decoder (see CV#541 for a description of assignments for bits 0-6)"));
            allCV.Add(new CVInfo(30, "ERROR Information", "In the case where the decoder has an error condition this CV shall contain the error condition as specified by the manufacturer. A value of 0 indicates that no error has occurred."));
            allCV.Add(new CVInfo(31, "Index High Byte", "The Indexed Address is the address of the indexed CV page when the decoder is set up for indexed CV operation. CV#31 contains the most significant bits of the two byte address and may have any value between 16 and 255 inclusive. Values of 0 thru 15 are reserved by the NMRA for future use. (4096 indexed pages) CV#32 contains the least significant bits of the index address and may contain any value. This gives a total of 61,440 indexed pages, each with 256 bytes of CV data available to manufacturers."));
            allCV.Add(new CVInfo(32, "Index Low Byte", "The Indexed Address is the address of the indexed CV page when the decoder is set up for indexed CV operation. CV#31 contains the most significant bits of the two byte address and may have any value between 16 and 255 inclusive. Values of 0 thru 15 are reserved by the NMRA for future use. (4096 indexed pages) CV#32 contains the least significant bits of the index address and may contain any value. This gives a total of 61,440 indexed pages, each with 256 bytes of CV data available to manufacturers."));
            for (uint i = 33; i <= 46; i++)
                allCV.Add(new CVInfo(i, "Output Locations 1-14 for Functions FL(f), FL(r), and F1-F12", "Contains a matrix indication of which function inputs control which Digital Decoder outputs. This allows the user to customize which outputs are controlled by which input commands. The outputs that Function FL(f) controls are indicated in CV#33, FL (r) in CV#34, F1 in CV#35 to F12 in CV#46. A value of 1 in each bit location indicates that the function controls that output. This allows a single function to control multiple outputs, or the same output to be controlled by multiple functions. CV#33-37 control outputs 1-8. CV#38-42 control outputs 4-11. CV#43-46 control outputs 7-14. The defaults is that FL (f) controls output 1, FL (r) controls output 2, F1 controls output 3 to F12 controls output 14. The lowest numbered output is in the LSB of the CV."));
            for (uint i = 47; i <= 64; i++)
                allCV.Add(new CVInfo(i, "Manufacturer unique", ""));
            allCV.Add(new CVInfo(65, "Kick Start", "Specifies the amount of extra Kick that will supplied to the motor when transitioning between stop and the first speed step."));
            allCV.Add(new CVInfo(66, "Forward Trim", "Specifies a scale factor by which a voltage drive level should be multiplied, when the controller is driving the unit in the forward direction. It is interpreted as n/128. If the Forward Trim configuration variable contains a value of 0 then forward trim is not implemented."));
            for (uint i = 67; i <= 94; i++)
                allCV.Add(new CVInfo(i, "Speed Table", "The speed table is defined to be 28 bytes wide, consisting of 28 values for forward speeds. A digital controller that uses this table shall have at least 64 voltage drive levels and can have as many as 256 so that a smooth power curve can be constructed. Note that voltage drive levels are specified in integer values, in the same way as most other parameters. This means that a drive level of 1/4 maximum voltage corresponds to 0100000, not 0010000, as you would expect if the number specified a fraction with a fixed denominator, i.e. value 32 out of a fixed 128 levels (see Definitions section)."));
            allCV.Add(new CVInfo(95, "Reverse Trim", "Specifies a scale factor by which a voltage drive level should be multiplied, when the controller is driving the unit in reverse. It is interpreted as n/128. If the Reverse Trim configuration variable contains a value of 0 then reverse trim is not implemented."));
            //for (uint i = 96; i <= 104; i++)
            //    allCV.Add(new CVInfo(i, "NMRA Reserved", ""));
            allCV.Add(new CVInfo(105, "User Identification #1", "CV#105 and CV#106 are reserved for use by the owner of the decoder to store identification information, e.g. NMRA membership number. CV#105 is ID#1 and CV#106 is ID#2"));
            allCV.Add(new CVInfo(106, "User Identification #2", "CV#105 and CV#106 are reserved for use by the owner of the decoder to store identification information, e.g. NMRA membership number. CV#105 is ID#1 and CV#106 is ID#2"));
            //for (uint i = 107; i <= 111; i++)
            //    allCV.Add(new CVInfo(i, "NMRA Reserved", ""));
            for (uint i = 112; i <= 256; i++)
                allCV.Add(new CVInfo(i, "Manufacturer unique", ""));
            for (uint i = 257; i <= 512; i++)
                allCV.Add(new CVInfo(i, "Indexed area", "This is the indexed area. It contains a total of 65536 pages, each 256 bytes in length. The first 4096 pages are reserved for NMRA use. The remaining 61440 pages are available to manufacturers for their own purposes. For the manufacturer that needs only 256 additional bytes of CVs, he can simply specify a base address in CV#31-32 and not respond if that address is not enabled without actually paging data."));
            //for (uint i = 513; i <= 879; i++)
            //    allCV.Add(new CVInfo(i, "NMRA Reserved", ""));
            //for (uint i = 880; i <= 891; i++)
            //    allCV.Add(new CVInfo(i, "NMRA Reserved", ""));
            allCV.Add(new CVInfo(892, "Decoder Load", "Specifies the current load of the decoder. The load is volatile and is not stored across power interruptions. Bits 0-6 indicate the value of the load with 0 indicating no load. Bit 7 indicates a positive or negative load."));
            allCV.Add(new CVInfo(893, "Dynamic Flags", "Up to 8 dynamic flags can be transmitted. Bits 0-7 Reserved for future use."));
            allCV.Add(new CVInfo(894, "Fuel/Coal", "Specifies the amount of Fuel/Coal left before the decoder will stop the locomotive. A value of 0 indicates that the Fuel/Coal is totally consumed, a value of 254 indicates totally full and a value of 255 indicates that this CV is not currently supported and its contents should not be transmitted."));
            allCV.Add(new CVInfo(895, "Water", "Specifies the amount of water left before the decoder will stop the locomotive. A value of 0 indicates that the water is totally consumed, a value of 254 indicates totally full and a value of 255 indicates that this CV is not currently supported and its contents should not be transmitted."));
            for (uint i = 896; i <= 1024; i++)
                allCV.Add(new CVInfo(i, "SUSI Sound and Function Modules", "See Technical Note TI-9.2.3 for details."));
        }
        #endregion
    }
     * */
}
