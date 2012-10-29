using System.Collections;

namespace Typhoon.DCC
{
    public enum DCCCommandPriority
    {
        Normal,
        High
    }

    public class DCCCommand
    {
        #region Constants
        private const int OperPreambleLength = 15; // min 14
        private const int ProgPreambleLength = 22; // min 20
        private const uint Period1Time = 2 * 58; // 2x58=116us for DCC 1 pulse; 2x(55...61)
        private const uint Period0Time = 2 * 116; // 2x116=232us for DCC 0 pulse; 2x(95...9900); totally < 12000
        //private const uint CUTOUT_GAP = 38;  // 38us gap after last bit of xor

        public const byte StopRepeats = 3;
        public const byte SpeedRepeat = 3; // dcc speed commands are repeated this time
        public const byte FuncRepeat = 3; // dcc func commands are repeated this time
        public const byte AccRepeat = 2; // dcc accessory commands are repeated this time
        public const byte PomRepeat = 3; // dcc pom commands are repeated this time (min 2)
        public const byte ServiceRepeat = 1; // dcc service prog commands are repeated this time
        #endregion

        #region Fields
        private ArrayList data = new ArrayList(); // only instruction bytes
        private DCCCommandPriority priority = DCCCommandPriority.Normal;
        private DCCCommandType type = DCCCommandType.None;
        private byte repeats = 1;
        #endregion

        #region Properties
        public ArrayList Data
        {
            get { return data; }
        }
        public DCCCommandPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }
        public DCCCommandType Type
        {
            get { return type; }
            set { type = value; }
        }
        public byte Repeats
        {
            get { return repeats; }
            set { repeats = value; }
        }
        #endregion

        #region Constructors
        private DCCCommand(ArrayList dataBytes)
        {
            if (dataBytes != null)
                data = dataBytes;
        }
        private DCCCommand(DCCCommandPriority priority, DCCCommandType type, ArrayList dataBytes)
            : this(dataBytes)
        {
            this.priority = priority;
            this.type = type;

            switch (type)
            {
                case DCCCommandType.None: repeats = 1; break;
                case DCCCommandType.Stop: repeats = StopRepeats; break;
                case DCCCommandType.Speed: repeats = SpeedRepeat; break;
                case DCCCommandType.Function: repeats = FuncRepeat; break;
                case DCCCommandType.Accessory: repeats = AccRepeat; break;
                case DCCCommandType.POM: repeats = PomRepeat; break;
                case DCCCommandType.Service: repeats = ServiceRepeat; break;
                default: repeats = 1; break;
            }
        }
        #endregion

        #region DCC Command Set
        /*
        Byte 1 (bin)	    Byte 1 (dec)  |  Byte 2 (bin)        Byte 2 (dec)	ќписание
        00000000	        0	          |  x                   x               Ўирокое вещание
        00000001Е01111111	1Е127	      |  x                   x               ћультифункциональные декодеры с 7-битными адресами
        10000000Е10111111	128Е191	      |  x                   x               Ѕазовые аксессуарные декодеры с 9-битными адресами;
                                          |                                      –асширенные аксессуарные декодеры с 11-битными адресами
        11000000Е11100111	192Е231	      |  00000000...11111111 0...255         ћультифункциональные декодеры с 14-битными адресами (0...10239)
        11101000Е11111110	232Е254	      |  00000000...11111111 0...255         «арезервировано дл€ использовани€ в будущем (10240...16127)
        11111111	        255	          |  x                   x               јдрес дл€ пакета просто€
        */

        private static DCCCommand LocoBroadcast(DCCCommandPriority priority, DCCCommandType type, byte[] instruction)
        {
            ArrayList list = new ArrayList();
            list.Add(DCC.LocoBroadcastAddress);
            foreach (byte b in instruction)
                list.Add(b);

            return new DCCCommand(priority, type, list);
        }
        private static DCCCommand LocoBroadcastBrakeStop(bool forward, bool ignoreDirection, bool emergencyStop)
        {
            //01DC000S

            byte instruction = (byte)(
                (forward ? DCC.SpeedForward : DCC.SpeedReverse) | // 01D00000
                (ignoreDirection ? 0x10 : 0x00) | // 000C0000
                (emergencyStop ? 0x01 : 0x00) // 0000000S
                );

            return LocoBroadcast(DCCCommandPriority.High, DCCCommandType.Stop, new byte[] { instruction });
        }
        //==========================================================
        public static DCCCommand Idle()
        {
            ArrayList list = new ArrayList();
            list.Add(DCC.IdleAddress);
            list.Add((byte)0);
            return new DCCCommand(list);
        }

        public static DCCCommand LocoBroadcastReset()
        {
            /*
            When a Digital Decoder receives a Digital Decoder Reset Packet, it shall erase all
            volatile memory (including any speed and direction data), and return to its normal power-up state. If the Digital
            Decoder is operating a locomotive at a non-zero speed when it receives a Digital Decoder Reset, it shall bring the
            locomotive to an immediate stop.
            Following a Digital Decoder Reset Packet, a Command Station shall not send any packets 
            with an address data byte between the range "01100100" and "01111111" inclusive within 
            20 milliseconds, unless it is the intent to enter service mode.
            */

            // 00000000

            byte instruction = 0;
            return LocoBroadcast(DCCCommandPriority.High, DCCCommandType.Stop, new byte[] { instruction });
        }
        public static DCCCommand LocoBroadcastBrake()
        {
            return LocoBroadcastBrakeStop(true, true, false);
        }
        public static DCCCommand LocoBroadcastStop()
        {
            return LocoBroadcastBrakeStop(true, true, true);
        }

        #region Loco Speed

        // speed < 0 => emergency-stop
        // speed = 0 => brake
        // speed = 1...14 => normal speed
        public static DCCCommand LocoSpeed14(LocomotiveAddress address, int speed, bool forward, bool light)
        {
            // 01DCSSSS
            // CV29 bit1=0 => 14 steps (older compartible), C is used for headlight
            /*
            SSSS    Speed parameter
            ----|---------------
         0  0000    0 - Brake
         1  0001    <0 - Emergency-Stop
         2  0010    1
         3  0011    2
            0100    3
            0101    4
            0110    5
            0111    6
            1000    7
            1001    8
            1010    9
            1011    10
            1100    11
        13  1101    12
        14  1110    13
        15  1111    14
            */

            int ssss = 0;
            if (speed < 0)
                ssss = 1;
            else if (speed == 0)
                ssss = 0;
            else if (speed > 0 && speed <= 14)
                ssss = speed + 1;
            else if (speed > 14)
                ssss = 15; // always max speed

            ArrayList list = new ArrayList();
            foreach (byte b in address.GetBytes())
                list.Add(b);
            list.Add((byte)(
                (ssss & 0x0F) | // 0000SSSS
                (light ? 0x10 : 0x00) | // 000[F0]SSSS
                (forward ? DCC.SpeedForward : DCC.SpeedReverse) // 01DCSSSS
                ));

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.Speed, list);
        }

        // speed < 0 => emergency-stop
        // speed = 0 => brake
        // speed = 1...28 => normal speed
        public static DCCCommand LocoSpeed28(LocomotiveAddress address, int speed, bool forward)
        {
            // 01DCSSSS
            // CV29bit1=1 => 28 steps, C is intermediate speed bit, least significant bit
            // FL is controlled by LocoFunctionGroup1, bit C
            /*
            SSSSC  Speed parameter
            -----|---------------
         0  00000   0 - Stop-brake
         1  00001   0 - Stop-brake Direction bit may be ignored for directional sensitive functions. (Optional)
         2  00010   <0 - Emergency-Stop
         3  00011   <0 - Emergency-Stop, Direction bit may be ignored for directional sensitive functions. (Optional)
         4  00100   1
         5  00101   2
         6  00110   3
         7  00111   4
            ............
        30  11110   27
        31  11111   28
            */

            int ssssc = 0;
            if (speed < 0)
                ssssc = 2; // 2 or 3
            else if (speed == 0)
                ssssc = 0; // 0 or 1
            else if (speed > 0 && speed <= 28)
                ssssc = speed + 3;
            else if (speed > 28)
                ssssc = 31;

            ArrayList list = new ArrayList();
            foreach (byte b in address.GetBytes())
                list.Add(b);

            byte c = (byte)((ssssc & 0x01) << 4);	 // 000C0000
            byte ssss = (byte)((ssssc & 0x1E) >> 1); // 0000SSSS
            list.Add((byte)(
                (ssss | c) | // 000CSSSS
                (forward ? DCC.SpeedForward : DCC.SpeedReverse) // 01DCSSSS
                ));

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.Speed, list);
        }

        // speed < 0 => emergency-stop
        // speed = 0 => brake
        // speed = 1...126 => normal speed
        public static DCCCommand LocoSpeed128(LocomotiveAddress address, int speed, bool forward)
        {
            // through NMRA.AdvancedOperation => 00100000
            // 001CCCCC DSSSSSSS;
            // CCCCC = 11111 => 128 speed steps
            // FL is controlled by LocoFunctionGroup1, bit C
            /*
            SSSSSSS| Speed parameter
            -------|---------------
         0  0000000 0 - Stop-brake
         1  0000001 <0 - Emergency-Stop
         2  0000010 1
         3  0000011 2
         4  0000100 3
         5  0000101 4
            ............
       126  1111110 125
       127  1111111 126
            */

            int sssssss = 0;
            if (speed < 0)
                sssssss = 1;
            else if (speed == 0)
                sssssss = 0;
            else if (speed >= 1 && speed <= 126)
                sssssss = speed + 1;
            else if (speed > 126)
                sssssss = 127;

            ArrayList list = new ArrayList();
            foreach (byte b in address.GetBytes())
                list.Add(b);
            list.Add((byte)(DCC.AdvancedOperation | 0x1F)); // 00111111
            list.Add((byte)(
                (sssssss & 0x7F) | // 0SSSSSSS
                (forward ? 0x80 : 0x00) // DSSSSSSS
                ));

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.Speed, list);
        }

        //// ???
        //public static NMRACommand LocoSpeedRestricted(LocomotiveAddress address, int speed, bool enabled)
        //{
        //    // 001CCCCC E*SSSSSS; advanced operation instruction
        //    // CCCCCC = 111110 - restricted speed steps instruction
        //    // In 128 speed step mode, the maximum restricted speed is scaled from 28 speed mode.

        //    List<byte> list = new List<byte>();
        //    list.AddRange(BuildLocoAddressBytes(address));
        //    list.Add(NMRA.AdvancedOperation | 0x1E); // 00111110
        //    list.Add((byte)(
        //        (speed & 0x3F) | // 00SSSSSS
        //        (enabled ? 0x00 : 0x80) // E*SSSSSS
        //    ));

        //    return new NMRACommand(NMRACommandPriority.Normal, NMRACommandType.Speed, list);
        //}

        #endregion

        #region Loco Functions
        public static DCCCommand LocoFunctionGroup1(LocomotiveAddress address, bool F0, bool F1, bool F2, bool F3, bool F4)
        {
            // 100CFFFF
            // CFFFF => F0, F4, F3, F2, F1
            // CV29 bit1=1 => C controls F0 (flight), 28/128 speed steps
            // CV29 bit1=0 => C not used here; flight is controlled by (C in speed14 )
            //If CV29 bit1 has a value of one (1), then bit 4(F0) controls function FL, otherwise bit 4 has no meaning!!!

            ArrayList list = new ArrayList();
            foreach (byte b in address.GetBytes())
                list.Add(b);
            list.Add((byte)(
                DCC.FunctionGroup1 | // 10000000
                (F1 ? 0x01 : 0) |
                (F2 ? 0x02 : 0) |
                (F3 ? 0x04 : 0) |
                (F4 ? 0x08 : 0) |
                (F0 ? 0x10 : 0) // if speed format != speed14
                ));

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.Function, list);
        }
        public static DCCCommand LocoFunctionGroup2(LocomotiveAddress address, bool F5, bool F6, bool F7, bool F8)
        {
            // 101SFFFF
            // S=1 FFFF => F8, F7, F6, F5

            ArrayList list = new ArrayList();
            foreach (byte b in address.GetBytes())
                list.Add(b);
            list.Add((byte)(
                DCC.FunctionGroup2 | 0x10 | // 10110000
                (F5 ? 0x01 : 0) |
                (F6 ? 0x02 : 0) |
                (F7 ? 0x04 : 0) |
                (F8 ? 0x08 : 0)
                ));

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.Function, list);
        }
        public static DCCCommand LocoFunctionGroup3(LocomotiveAddress address, bool F9, bool F10, bool F11, bool F12)
        {
            // 101SFFFF
            // S=0 FFFF => F12, F11, F10, F9

            ArrayList list = new ArrayList();
            foreach (byte b in address.GetBytes())
                list.Add(b);
            list.Add((byte)(
                DCC.FunctionGroup2 | // 10100000
                (F9 ? 0x01 : 0) |
                (F10 ? 0x02 : 0) |
                (F11 ? 0x04 : 0) |
                (F12 ? 0x08 : 0)
                ));

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.Function, list);
        }
        public static DCCCommand LocoFunctionGroup4(LocomotiveAddress address, bool F13, bool F14, bool F15, bool F16, bool F17, bool F18, bool F19, bool F20)
        {
            // 11011110 FFFFFFFF
            // FFFFFFFF => F20, F19, F18, F17, F16, F15, F14, F13

            ArrayList list = new ArrayList();
            foreach (byte b in address.GetBytes())
                list.Add(b);
            list.Add((byte)(
                DCC.Reserved | 0x1E | // 11000000 | 00011110
                (F13 ? 0x01 : 0) |
                (F14 ? 0x02 : 0) |
                (F15 ? 0x04 : 0) |
                (F16 ? 0x08 : 0) |
                (F17 ? 0x10 : 0) |
                (F18 ? 0x20 : 0) |
                (F19 ? 0x40 : 0) |
                (F20 ? 0x80 : 0)
                ));

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.Function, list);
        }
        public static DCCCommand LocoFunctionGroup5(LocomotiveAddress address, bool F21, bool F22, bool F23, bool F24, bool F25, bool F26, bool F27, bool F28)
        {
            // 11011111 FFFFFFFF
            // FFFFFFFF => F28, F27, F26, F25, F24, F23, F22, F21

            ArrayList list = new ArrayList();
            foreach (byte b in address.GetBytes())
                list.Add(b);
            list.Add((byte)(
                DCC.Reserved | 0x1F | // 11000000 | 00011111
                (F21 ? 0x01 : 0) |
                (F22 ? 0x02 : 0) |
                (F23 ? 0x04 : 0) |
                (F24 ? 0x08 : 0) |
                (F25 ? 0x10 : 0) |
                (F26 ? 0x20 : 0) |
                (F27 ? 0x40 : 0) |
                (F28 ? 0x80 : 0)
                ));

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.Function, list);
        }
        #endregion

        public static DCCCommand LocoConsist(LocomotiveAddress address, byte consistAddress, bool forward)
        {
            // 0001CCCC 0AAAAAAA

            // When Consist Control is in effect, the decoder will ignore any speed or direction instructions addressed to its
            // normal locomotive address (unless this address is the same as its consist address). Speed and direction instructions
            // now apply to the consist address only.
            // Functions controlled by instruction 100 and 101 will continue to respond to the decoders baseline address and also respond
            // to the consist address if the appropriate bits in CVs #21,22 have been activated.
            // By default all forms of Bi-directional communication are not activated in response to commands sent to the consist
            // address until specifically activated by a Decoder Control instruction. Operations mode acknowledgement and Data
            // Transmission applies to the appropriate commands at the respective decoder addresses.
            // A value of У1Ф in bit 7 of the second byte is reserved for future use.
            // CCCC contains a consist setup instruction, and the AAAAAAA in the second byte is a seven bit consist address.
            // If the address is "0000000" then the consist is deactivated. If the address is non-zero, then the consist is activated.
            // If the consist is deactivated (by setting the consist to С0000000Т), the Bi-Directional communications settings are set as specified in CVs 26-28.

            // When operations-mode acknowledgement is enabled, all consist commands must be acknowledged via operations mode acknowledgement. The format for CCCC shall be:
            // CCCC=0010 (0x02)
            // Set the consist address as specified in the second byte, and activate the consist. The consist
            // address is stored in bits 0-6 of CV #19, and bit 7 of CV #19 is set to a value of 0. The direction
            // of this unit in the consist is the normal direction. If the consist address is 0000000 the consist is deactivated.
            // CCCC=0011 (0x03)
            // Set the consist address as specified in the second byte and activate the consist. The consist
            // address is stored in bits 0-6 of CV #19, and bit 7 of CV#19 is set to a value of 1. The direction
            // of this unit in the consist is opposite its normal direction. If the consist address is 0000000 the consist is deactivated.

            // All other values of CCCC are reserved for future use.

            if (consistAddress > 127 || consistAddress < 0)
                return null;

            ArrayList list = new ArrayList();
            foreach (byte b in address.GetBytes())
                list.Add(b);
            list.Add((byte)(
                DCC.ConsistControl | // 00010000
                (forward ? 0x02 : 0x03)
                ));
            list.Add((byte)(consistAddress & 0x7F));

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.None, list);
        }

        #region POM mode
        // it's equal to pomLocoCVWrite only for CV#23/CV#24
        //public static NMRACommand pomLocoAccelerationDeceleration(LocomotiveAddress address, bool acceleration, byte value)
        //{
        //    // through NMRA.CVAccess instruction, short form
        //    //                  111xxxxx xxxxxxxx 
        //    // short form:      1111CCCC DDDDDDDD
        //    //                      0000 -------- unused
        //    //                      0010 xxxxxxxx = Acceleration Value (CV#23), 0x02
        //    //                      0011 xxxxxxxx = Deceleration Value (CV#24), 0x03
        //    //                      1001 xxxxxxxx = See RP-9.2.3, Appendix B: Service Mode Decoder Lock Instruction, service mode

        //    List<byte> list = new List<byte>();
        //    list.AddRange(BuildLocoAddressBytes(address));
        //    list.Add((byte)(
        //        NMRA.CVAccess | 0x10 | // 11110000
        //        (acceleration ? 0x02 : 0x03) // 11110010 or 11110011
        //        ));
        //    list.Add(value);

        //    return new NMRACommand(NMRACommandPriority.High, NMRACommandType.POM, list);
        //}
        //public static NMRACommand pomLocoCV(LocomotiveAddress address, uint cv, byte value, bool verify = false)
        //{
        //    // through NMRA.CVAccess instruction, long form
        //    // 1110CCAA AAAAAAAA DDDDDDDD
        //    // CC=00 Reserved for future use
        //    // CC=01 Verify byte, 0x04
        //    // CC=11 Write byte, 0x0C
        //    // CC=10 Bit manipulation, 0x08
        //    /*
        //     Two identical packets are needed before the decoder shall modify a CV.
        //     These two packets need not be back to back on the track.
        //     However any other packet to the same decoder will invalidate the write operation.
        //     (This includes broadcast packets.) If the decoder successfully receives this second
        //     identical packet, it shall respond with a CV access acknowledgment. 
        //    */
        //    /*
        //    The contents of the CV as indicated by the 10-bit address are compared with the data
        //    byte (DDDDDDDD). If the decoder successfully receives this packet and the values are
        //    identical, the Digital Decoder shall respond with the contents of the CV as the 
        //    Decoder Response Transmission, if enabled. 
        //    */

        //    if (cv < 1 || cv > 1024)
        //        return null;

        //    uint cv_adr = cv - 1;

        //    List<byte> list = new List<byte>();
        //    list.AddRange(BuildLocoAddressBytes(address));
        //    list.Add((byte)(
        //        NMRA.CVAccess |                 // 11100000
        //        (verify ? 0x04 : 0x0C) |        // CC
        //        ((cv_adr >> 8) & 0x03)          // 2 left bits of cv_adr
        //        ));
        //    list.Add((byte)(cv_adr & 0xFF));    // 8 right bits of cv_adr
        //    list.Add(value);

        //    return new NMRACommand(NMRACommandPriority.High, NMRACommandType.POM, list);
        //}
        //public static NMRACommand pomLocoCVBit(LocomotiveAddress address, uint cv, uint bitPosition, byte bitValue, bool verify = false)
        //{
        //    // through NMRA.CVAccess instruction, long form
        //    // 1110CCAA AAAAAAAA 111KDBBB
        //    // CC=00 Reserved for future use
        //    // CC=01 Verify byte, 0x04
        //    // CC=11 Write byte, 0x0C
        //    // CC=10 Bit manipulation, 0x08
        //    // K = (1=write, 0=verify), D = Bitvalue, BBB = bitpos
        //    // we use CC=10

        //    /*
        //    The bit manipulation instructions use a special format for the data byte (DDDDDDDD):
        //    111KDBBB
        //    Where BBB represents the bit position within the CV,
        //    D contains the value of the bit to be verified or written,
        //    and K describes whether the operation is a verify bit or a write bit operation.
        //    K = "1" WRITE BIT
        //    K = "0" VERIFY BIT
        //    The VERIFY BIT and WRITE BIT instructions operate in a manner similar to the VERIFY BYTE and WRITE
        //    BYTE instructions (but operates on a single bit). Using the same criteria as the VERIFY BYTE instruction, an
        //    operations mode acknowledgment will be generated in response to a VERIFY BIT instruction if appropriate. Using
        //    the same criteria as the WRITE BYTE instruction, a configuration variable access acknowledgment will be
        //    generated in response to the second identical WRITE BIT instruction if appropriate.
        //    */

        //    if (cv < 1 || cv > 1024)
        //        return null;

        //    uint cv_adr = cv - 1;

        //    List<byte> list = new List<byte>();
        //    list.AddRange(BuildLocoAddressBytes(address));
        //    list.Add((byte)(
        //        NMRA.CVAccess |                 // 11100000
        //        0x08 |                          // CC=10 Bit manipulation, 0x08
        //        ((cv_adr >> 8) & 0x03)          // 2 left bits of cv_adr
        //        ));
        //    list.Add((byte)(cv_adr & 0xFF));    // 8 right bits of cv_adr
        //    list.Add((byte)(
        //        0xE0 |                          // 11100000
        //        (verify ? 0x00 : 0x10) |        // K
        //        ((bitValue & 0x01) << 3) |      // 0000D000, bitValue
        //        (bitPosition & 0x07)            // bitPosition & 00000111
        //        ));

        //    return new NMRACommand(NMRACommandPriority.High, NMRACommandType.POM, list);
        //}
        #endregion

        #region Service mode

        public static DCCCommand progIdle()
        {
            DCCCommand cmd = Idle();
            cmd.Priority = DCCCommandPriority.Normal;
            cmd.Type = DCCCommandType.Service;
            return cmd;
        }
        public static DCCCommand progReset()
        {
            DCCCommand cmd = DCCCommand.LocoBroadcastReset();
            cmd.Priority = DCCCommandPriority.Normal;
            cmd.Type = DCCCommandType.Service;
            return cmd;
        }

        #region Direct mode
        //public static NMRACommand progDirectModeCV(uint cv, byte value, bool verify = false)
        //{
        //    // 0111CCAA AAAAAAAA DDDDDDDD
        //    //     CC=01 Verify byte, 0x04
        //    //     CC=11 Write byte, 0x0C
        //    //     CC=10 Bit manipulation, 0x08

        //    if (cv < 1 || cv > 1024)
        //        return null;

        //    uint cv_adr = cv - 1;

        //    List<byte> list = new List<byte>();
        //    list.Add((byte)(
        //        NMRA.ServiceMode |              // 01110000
        //        (verify ? 0x04 : 0x0C) |        // CC=01 Verify byte (0x04) or CC=11 Write byte (0x0C)
        //        (byte)((cv_adr >> 8) & 0x03)    // 2 left bits of cv_adr
        //        ));
        //    list.Add((byte)(cv_adr & 0xFF));    // 8 right bits of cv_adr
        //    list.Add(value);

        //    return new NMRACommand(NMRACommandPriority.Normal, NMRACommandType.Service, list);
        //}
        //public static NMRACommand progDirectModeCVBit(uint cv, uint bitPosition, byte bitValue, bool verify = false)
        //{                       //DDDDDDDD
        //    // 0111CCAA AAAAAAAA 111KDBBB
        //    //     CC=01 Verify byte, 0x04
        //    //     CC=11 Write byte, 0x0C
        //    //     CC=10 Bit manipulation, 0x08
        //    // K = (1=write, 0=verify), D = Bitvalue, BBB = bitpos
        //    // we use CC=10

        //    if (cv < 1 || cv > 1024)
        //        return null;
        //    if (bitPosition > 7)
        //        return null;
        //    if (bitValue > 1)
        //        return null;

        //    uint cv_adr = cv - 1;

        //    List<byte> list = new List<byte>();
        //    list.Add((byte)(
        //        NMRA.ServiceMode |              // 01110000
        //        0x08 |                          // CC=10 Bit manipulation, 0x08
        //        (cv_adr >> 8 & 0x03)            // 2 left bits of cv_adr
        //        ));
        //    list.Add((byte)(cv_adr & 0xFF));    // 8 right bits of cv_adr
        //    list.Add((byte)(
        //        0xE0 |                          // 11100000
        //        (verify ? 0x00 : 0x10) |        // K
        //        ((bitValue & 0x01) << 3) |      // 0000D000, bitValue
        //        (bitPosition & 0x07)            // bitPosition & 00000111
        //        ));

        //    return new NMRACommand(NMRACommandPriority.Normal, NMRACommandType.Service, list);
        //}
        #endregion

        #region Physical register mode
        public static DCCCommand progPagePreset()
        {
            // 01111101 00000001

            ArrayList list = new ArrayList();
            list.Add(0x7D);     // 01111101
            list.Add(0x01);     // 00000001

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.Service, list);
        }
        public static DCCCommand progRegisterMode(uint cv, byte value, bool verify = false)
        {
            // 0111CRRR DDDDDDDD
            //     C=0 Verify address, 0x00
            //     C=1 Write address, 0x08

            // cv   acc CV          loco CV
            // 1    513             1
            // 2    Mfgs. doc       2
            // 3    Mfgs. doc       3
            // 4    Mfgs. doc       4
            // 5    Mfgs. doc       29
            // 6    Mfgs. doc       Reserved for Page Register
            // 7    7               7
            // 8    520             8

            if (cv < 1 || cv > 8)
                return null;

            uint reg_adr = cv - 1;

            ArrayList list = new ArrayList();
            list.Add((byte)(
                DCC.ServiceMode |          // 01110000
                (verify ? 0x00 : 0x08) |    // C
                (reg_adr & 0x07)            // RRR
                ));
            list.Add(value);

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.Service, list);
        }
        #endregion

        #region Address Only mode
        // is equal to progRegisterMode(0, value, verify)
        public static DCCCommand progAddressOnlyMode(byte value, bool verify = false)
        {
            // 0111C000 0DDDDDDD
            //     C=0 Verify address, 0x00
            //     C=1 Write address, 0x08

            if (value < 1 || value > 127)
                return null;

            return progRegisterMode(0, value, verify);
        }
        #endregion

        #endregion

        #region Basic Accessory
        // decoderAddress: [1...510]; 511 is acc broadcast address; 511 total addresses, 510 for decoders
        // decoderOutput: [0...3]
        // coilNumber: 0 => straight, 1 => diverging
        // on: on, off, [0/1]; note: intellibox only sends on, never off
        public static DCCCommand BasicAccessory(ushort decoderAddress, byte decoderOutput, byte coilNumber, bool on)
        {
            // 10AAAAAA 1AAACDDP
            //           AAA => bit 7...9 of address, but is transmitted inverted (in complement form (0=>1, 1=>0))!
            //              C => =0 - off, =1 - on
            //               DD => output number, 00..11
            //                 P => coil number, 0...1

            /*
            Duration of time each output is active being controlled by CVs #515...518. Since most devices are paired,
            the convention is that bit "0" of the second byte is used to distinguish between which of a pair of outputs
            the accessory decoder is activating or deactivating.
            Bits 1 and 2 of byte two are used to indicate which of 4 pairs of outputs the packet is controlling. The
            most significant bits of the 9-bit address are bits 4-6 of the second data byte. By convention these bits (bits 4-6 of
            the second data byte) are in ones complement.
            */

            byte addressLSBMasked = (byte)(0x80 | (decoderAddress & 0x3F)); // 0...5 LSB bits; = CV1
            byte addressMSBMasked = (byte)(0x80 | (((decoderAddress / 0x40) ^ 0x07) * 0x10)); // 6...8 MSB bits; inverted; = CV9
            byte outputNumberMasked = (byte)((decoderOutput << 1) & 0x06);
            byte coilNumberMasked = (byte)(coilNumber & 0x01);
            byte onoffMasked = (byte)(((on ? (byte)1 : (byte)0) & 0x01) * 0x08); // 0000[on]000

            ArrayList list = new ArrayList();
            list.Add(addressLSBMasked);
            list.Add((byte)(addressMSBMasked | outputNumberMasked | coilNumberMasked | onoffMasked));

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.Accessory, list);
        }
        public static DCCCommand BasicAccessoryBroadcast(byte decoderOutput, byte coilNumber, bool on)
        {
            // 10111111 1000CDDP - broadcast for basic accessory decoders (3 MSB bits are inverted!!!, so 111);
            return BasicAccessory(DCC.BasicAccessoryBroadcastAddress, decoderOutput, coilNumber, on);
        }

        // accessoryNumber: [0...2040]
        public static DCCCommand BasicAccessory(ushort accessoryNumber, byte coilNumber, bool on)
        {
            ushort n = (ushort)(accessoryNumber - 1);
            ushort decoderAddress = (ushort)(n / 4 + 1);
            byte outputNumber = (byte)(n % 4);

            return BasicAccessory(decoderAddress, outputNumber, coilNumber, on);
        }
        #endregion

        #region Extended accessory
        // decoderAddress: [0...510]; 511 is acc broadcast address
        // aspect: aspect number, [0...31]
        public static DCCCommand ExtendedAccessory(ushort decoderAddress, byte aspect)
        {
            // 10AAAAAA 0AAA0AA1 000XXXXX
            //                      XXXXX = 0 => absolute stop aspect
            //           AAA => bit 7...9 of address, but is transmitted inverted!

            byte addressLSBMasked = (byte)(0x80 | (decoderAddress & 0x3F)); // address 0...6 lsb bits
            byte addressMSBMasked = (byte)(0x80 | (((decoderAddress / 0x40) ^ 0x07) * 0x10)); // address 7...9 msb bits; shift down, invert, shift up
            byte aspectMasked = (byte)(aspect & 0x1F);

            ArrayList list = new ArrayList();
            list.Add(addressLSBMasked);
            list.Add((byte)(addressMSBMasked | aspectMasked));

            return new DCCCommand(DCCCommandPriority.Normal, DCCCommandType.Accessory, list);
        }
        //public static DCCCommand ExtendedAccessoryBroadcast(byte aspect)
        //{
        //    // 10111111 0 00000111 0 000XXXXX
        //    return ExtendedAccessory(, aspect);
        //}
        #endregion

        #endregion

        #region Timings
        private void AddBitTimings(ArrayList timings, byte bit)
        {
            uint t = (bit == 1 ? Period1Time : Period0Time) / 2;
            timings.Add(t);
            timings.Add(t);
        }
        private void AddByteTimings(ArrayList timings, byte value)
        {
            ArrayList bits = new ArrayList();
            for (byte i = 0; i < 8; i++)
            {
                byte bit = (value & 0x01) == 1 ? (byte)1 : (byte)0;
                bits.Add(bit);
                value = (byte)(value >> 1);
            }

            // bits now is from [0]=LSB to [7]=MSB
            // as we have to peek from MSB to LSB, we loop in backward direction:
            for (int i = bits.Count - 1; i >= 0; i--)
                AddBitTimings(timings, (byte)bits[i]);
        }

        public uint[] ToTimings(bool serviceMode = false) // for single dcc output pin; half periods used; in microseconds
        {
            ArrayList timings = new ArrayList();

            byte checkSum = 0;

            // preamble
            int length = (serviceMode ? ProgPreambleLength : OperPreambleLength);
            for (int i = 0; i < length; i++)
                AddBitTimings(timings, 1);

            // data bytes
            for (int i = 0; i < data.Count; i++)
            {
                // start bit
                AddBitTimings(timings, 0);

                // data byte
                byte b = (byte)data[i];
                AddByteTimings(timings, b);

                // check sum update
                checkSum ^= b;
            }

            // start bit
            AddBitTimings(timings, 0);

            // check sum
            AddByteTimings(timings, checkSum);

            // stop bit
            AddBitTimings(timings, 1);

            // convert to uint[]
            uint[] res = new uint[timings.Count];
            timings.CopyTo(res);
            return res;
        }
        #endregion
    }
}
