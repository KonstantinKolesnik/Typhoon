﻿/* Original source was in Java from the JMRI project. */

/**
 * Utilities for coding/decoding NMRA S&RP DCC packets.
 *<P>
 * Packets are (now) represented by an array of bytes. Preamble/postamble
 * not included. Note that this is a data representation, _not_ a representation
 * of the waveform!  But this is a class, which might eventually also
 * form a representation object.
 *<P>
 * This is meant to be a general Java NMRA implementation, so does NOT use
 * JMRI utilities. In particular, it returns NULL instead of throwing JmriException
 * for invalid requests. Callers need to check upstream.
 *<P>
 * The function is provided by static member functions; objects of this
 * class should not be created.
 *<P>
 * Note that these functions are structured by packet type, not by what want to do.  E.g.
 * there are functions to create specific packet formats instead of a general "loco speed
 * packet" routine which figures out which type of packet to use.  Those decisions
 * are to be made somewhere else.
 * <P>
 * Range and value checking is intended to be aggressive; if we can check, we
 * should.  Problems are reported as warnings.
 *<P>
 * The basic function is to build a packet with proper addressing, etc:
 * <UL>
 * <li>oneBytePacket
 * <li>twoBytePacket
 * <li>threeBytePacket
 * <li>fourBytePacket
 * </ul>
 * On top of those are built various special-purpose packet formats.
 * <hr>
 * This file is part of JMRI.
 * <P>
 * JMRI is free software; you can redistribute it and/or modify it under
 * the terms of version 2 of the GNU General Public License as published
 * by the Free Software Foundation. See the "COPYING" file for a copy
 * of this license.
 * <P>
 * JMRI is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * for more details.
 * <P>
 *
 * @author      Bob Jacobsen Copyright (C) 2001, 2003
 * @version     $Revision: 1.30 $
 */

using System.IO;

namespace Typhoon.NMRA
{
    static class _NmraPacket
    {
        public const int NO_ADDRESS = 1;
        public const int LOCO_SHORT_ADDRESS = 2;
        public const int LOCO_LONG_ADDRESS = 4;
        public const int ACCESSORY_SHORT_ADDRESS = 8;

        //Create a packet containing a one-byte instruction.
        public static int oneBytePacket(ref byte[] retVal, int address, bool longAddr, byte arg1)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            // end sanity check, format output
            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2]);
                return 4;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)(retVal[0] ^ retVal[1]);
                return 3;
            }
        }
        //Create a packet containing a two-byte instruction.
        public static int twoBytePacket(ref byte[] retVal, int address, bool longAddr, byte arg1, byte arg2)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            // end sanity check, format output
            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)arg2;
                retVal[4] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3]);
                return 5;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)arg2;
                retVal[3] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2]);
                return 4;
            }
        }
        //Create a packet containing a three-byte instruction.
        public static int threeBytePacket(ref byte[] retVal, int address, bool longAddr, byte arg1, byte arg2, byte arg3)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            // end sanity check, format output
            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)arg2;
                retVal[4] = (byte)arg3;
                retVal[5] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3] ^ retVal[4]);
                return 6;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)arg2;
                retVal[3] = (byte)arg3;
                retVal[4] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3]);
                return 5;
            }
        }
        //Create a packet containing a four-byte instruction.
        public static int fourBytePacket(ref byte[] retVal, int address, bool longAddr, byte arg1, byte arg2, byte arg3, byte arg4)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            // end sanity check, format output
            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)arg2;
                retVal[4] = (byte)arg3;
                retVal[5] = (byte)arg4;
                retVal[6] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3] ^ retVal[4] ^ retVal[5]);
                return 7;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)arg2;
                retVal[3] = (byte)arg3;
                retVal[4] = (byte)arg4;
                retVal[5] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3] ^ retVal[4]);
                return 6;
            }
        }
        public static byte[] anyBytePacket(int address, bool longAddr, byte[] args)
        {
            if (!AddressCheck(address, longAddr))
                return null;

            byte[] retVal = new byte[(longAddr ? 2 : 1) + args.GetLength(0) + 1];

            int n = 0;
            if (longAddr)
            {
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                n += 2; // n = 2
            }
            else
            {
                retVal[0] = (byte)(address & 0xFF);
                n++; // n = 1
            }

            int argscount = args.GetLength(0);
            // assign args
            for (int i = 0; i < argscount; i++)
                retVal[n + i] = args[i];
            // calc errorsum
            byte errorsum = 0;
            for (int i = 0; i < n + argscount + 1; i++)
                errorsum ^= retVal[i];

            retVal[n + argscount + 1] = errorsum;
            return retVal;
        }










        /**
         * Provide an accessory control packet via a simplified interface
         * @param number Address of accessory output, starting with 1
         * @param closed true if the output is to be configured to the "closed", a.k.a. the "normal" or "unset" position
         */
        public static int accDecoderPkt(ref byte[] retVal, int number, bool closed)
        {
            // dBit is the "channel" info, least 7 bits, for the packet
            // The lowest channel bit represents CLOSED (1) and THROWN (0)
            int dBits = (((number - 1) & 0x03) << 1);  // without the low CLOSED vs THROWN bit
            dBits = closed ? (dBits | 1) : dBits;

            // aBits is the "address" part of the nmra packet, which starts with 1
            // 07/01/05 R.Scheffler - Removed the mask, this will allow any 'too high' numbers
            // through to accDecoderPkt() above which will log the error if out of bounds. If we
            // mask it here, then the number will 'wrap' without any indication that it did so.
            int aBits = (number - 1) >> 2;      // Divide by 4 to get the 'base'
            aBits += 1;                       // Base is +1

            // cBit is the control bit, we're always setting it active
            int cBit = 1;

            // get the packet
            return accDecoderPkt2(ref retVal, aBits, cBit, dBits);
        }
        public static int accDecoderPkt2(ref byte[] retVal, int addr, int active, int outputChannel)
        {
            // From the NMRA RP:
            // 0 10AAAAAA 0 1AAACDDD 0 EEEEEEEE 1
            // Accessory Digital Decoders can be designed to control momentary or
            // constant-on devices, the duration of time each output is active being controlled
            // by configuration variables CVs #515 through 518. Bit 3 of the second byte "C" is
            // used to activate or deactivate the addressed device. (Note if the duration the
            // device is intended to be on is less than or equal the set duration, no deactivation
            // is necessary.) Since most devices are paired, the convention is that bit "0" of
            // the second byte is used to distinguish between which of a pair of outputs the
            // accessory decoder is activating or deactivating. Bits 1 and 2 of byte two is used
            // to indicate which of 4 pairs of outputs the packet is controlling. The significant
            // bits of the 9 bit address are bits 4-6 of the second data byte. By convention
            // these three bits are in ones complement. The use of bit 7 of the second byte
            // is reserved for future use.

            // Note that A=1 is the first (lowest) valid address field, and the
            // largest is 512!  I don't know why this is, but it gets the
            // right hardware addresses

            if (addr < 1 || addr > 511)
            {
                //printf("invalid address %d\n", addr);
                return 0;
            }
            if (active < 0 || active > 1)
            {
                //printf("invalid active (C) bit %d\n", addr);
                return 0;
            }
            if (outputChannel < 0 || outputChannel > 7)
            {
                //printf("invalid output channel %d\n", addr);
                return 0;
            }

            int lowAddr = addr & 0x3F;
            int highAddr = ((~addr) >> 6) & 0x07;

            retVal[0] = (byte)(0x80 | lowAddr);
            retVal[1] = (byte)(0x80 | (highAddr << 4) | (active << 3) | outputChannel & 0x07);
            retVal[2] = (byte)(retVal[0] ^ retVal[1]);

            return 3;
        }

        /**
         * From the RP:
         * Extended Accessory Decoder Control Packet Format
         * The Extended Accessory Decoder Control Packet is included for the purpose
         *  of transmitting aspect control to signal decoders or data bytes to more
         *  complex accessory decoders. Each signal head can display one aspect at a time.
         *
         * {preamble} 0 10AAAAAA 0 0AAA0AA1 0 000XXXXX 0 EEEEEEEE 1
         *
         * XXXXX is for a single head. A value of 00000 for XXXXX indicates the absolute
         *  stop aspect. All other aspects represented by the values for XXXXX are
         *  determined by the signaling system used and the prototype being modeled.
         *
         * @param outputAddr Address of accessory output, starting with 1 and a maximum of 2044
         * @param aspect Aspect Number starting with 0 and a maximum of 31
         */
        public static int accSignalDecoderPkt(ref byte[] retVal, int outputAddr, int aspect)
        {
            if (outputAddr < 1 || outputAddr > 2044)
            {
                //printf("invalid address %d\n", outputAddr);
                return 0;
            }

            if (aspect < 0 || aspect > 31)
            {
                //printf("invalid aspect %d\n", aspect);
                return 0;
            }

            outputAddr -= 1; // Make the address 0 based
            int lowAddr = (outputAddr & 0x03);  // Output Pair Address
            int boardAddr = (outputAddr >> 2) + 1; // Board Address
            int midAddr = boardAddr & 0x3F;
            int highAddr = ((~boardAddr) >> 6) & 0x07;

            retVal[0] = (byte)(0x80 | midAddr);
            retVal[1] = (byte)(0x01 | (highAddr << 4) | (lowAddr << 1));
            retVal[2] = (byte)(0x1F & aspect);
            retVal[3] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2]);

            return 4;
        }


        /**
         * From the NMRA RP:
         * Basic Accessory Decoder Packet address for operations mode programming
         * 10AAAAAA 0 1AAACDDD 0 1110CCVV 0 VVVVVVVV 0 DDDDDDDD
         * Where DDD is used to indicate the output whose CVs are being modified and C=1.
         * If CDDD= 0000 then the CVs refer to the entire decoder. The resulting packet would be
         * {preamble} 10AAAAAA 0 1AAACDDD 0 (1110CCVV   0   VVVVVVVV   0   DDDDDDDD) 0 EEEEEEEE 1
         * Accessory Decoder Address   (Configuration Variable Access Instruction)     Error Byte
         */
        public static int accDecoderPktOpsMode2(ref byte[] retVal, int addr, int active, int outputChannel, int cvNum, int data)
        {
            if (addr < 1 || addr > 511)
            {
                //printf("invalid address %d\n" + addr);
                return 0;
            }
            if (active < 0 || active > 1)
            {
                //printf("invalid active (C) bit %d\n", addr);
                return 0;
            }
            if (outputChannel < 0 || outputChannel > 7)
            {
                //printf("invalid output channel %d\n", addr);
                return 0;
            }

            if (cvNum < 1 || cvNum > 1023)
            {
                //printf("invalid CV number %d\n", cvNum);
                return 0;
            }

            if (data < 0 || data > 255)
            {
                //printf("invalid data %d\n", data);
                return 0;
            }

            int lowAddr = addr & 0x3F;
            int highAddr = ((~addr) >> 6) & 0x07;

            int lowCVnum = (cvNum - 1) & 0xFF;
            int highCVnum = ((cvNum - 1) >> 8) & 0x03;

            retVal[0] = (byte)(0x80 | lowAddr);
            retVal[1] = (byte)(0x80 | (highAddr << 4) | (active << 3) | outputChannel & 0x07);
            retVal[2] = (byte)(0xEC | highCVnum);
            retVal[3] = (byte)(lowCVnum);
            retVal[4] = (byte)(0xFF & data);
            retVal[5] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3] ^ retVal[4]);

            return 6;
        }
        /**
         * From the NMRA RP:
         * The format for Accessory Decoder Configuration Variable Access Instructions is:
         * {preamble} 0 10AAAAAA 0 0AAA11VV 0 VVVVVVVV 0 DDDDDDDD 0 EEEEEEEE 1
         * Where:
         *  A = Decoder address bits
         *  V = Desired CV address - (CV 513 = 10 00000000)
         *  D = Data for CV
         *
         *  This is the old "legacy" format, newer decoders use the "Basic Accessory Decoder Packet"
         */
        public static int accDecPktOpsModeLegacy(ref byte[] retVal, int addr, int cvNum, int data)
        {
            if (addr < 1 || addr > 511)
            {
                //printf("invalid address %d\n", addr);
                return 0;
            }

            if (cvNum < 1 || cvNum > 1023)
            {
                //printf("invalid CV number  %d\n", cvNum);
                return 0;
            }

            if (data < 0 || data > 255)
            {
                //printf("invalid data  %d\n", data);
                return 0;
            }

            int lowAddr = addr & 0x3F;
            int highAddr = ((~addr) >> 6) & 0x07;

            int lowCVnum = (cvNum - 1) & 0xFF;
            int highCVnum = ((cvNum - 1) >> 8) & 0x03;

            retVal[0] = (byte)(0x80 | lowAddr);
            retVal[1] = (byte)(0x0C | (highAddr << 4) | highCVnum);
            retVal[2] = (byte)(lowCVnum);
            retVal[3] = (byte)(0xFF & data);
            retVal[4] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3]);

            return 5;
        }
        /**
         * Provide an operation mode accessory control packet via a simplified interface
         * @param number Address of accessory, starting with 1
         * @param cvNum CV number to access
         * @param data Data to be written
         */
        public static int accDecoderPktOpsMode(ref byte[] retVal, int number, int cvNum, int data)
        {
            // dBit is the "channel" info, least 7 bits, for the packet
            // The lowest channel bit represents CLOSED (1) and THROWN (0)
            int dBits = (((number - 1) & 0x03) << 1);  // without the low CLOSED vs THROWN bit

            // aBits is the "address" part of the nmra packet, which starts with 1
            int aBits = (number - 1) >> 2;      // Divide by 4 to get the 'base'
            aBits += 1;                       // Base is +1

            // cBit is the control bit, we're always setting it active
            int cBit = 1;

            // get the packet
            return accDecoderPktOpsMode2(ref retVal, aBits, cBit, dBits, cvNum, data);
        }
        /**
         * Provide a legacy operation mode accessory control packet via a simplified interface
         * @param number Address of accessory, starting with 1
         * @param cvNum CV number to access
         * @param data Data to be written
         */
        public static int accDecoderPktOpsModeLegacy(ref byte[] retVal, int number, int cvNum, int data)
        {
            // aBits is the "address" part of the nmra packet, which starts with 1
            int aBits = (number - 1) >> 2;      // Divide by 4 to get the 'base'
            aBits += 1;                       // Base is +1

            // get the packet
            return accDecPktOpsModeLegacy(ref retVal, aBits, cvNum, data);
        }





        public static int opsCvWriteByte(ref byte[] retVal, int address, bool longAddr, int cvNum, int data)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            if (data < 0 || data > 255)
            {
                //printf("invalid data " + data);
                return 0;
            }
            if (cvNum < 1 || cvNum > 512)
            {
                //printf("invalid CV number " + cvNum);
                return 0;
            }

            // end sanity checks, format output
            int arg1 = 0xEC + (((cvNum - 1) >> 8) & 0x03);
            int arg2 = (cvNum - 1) & 0xFF;
            int arg3 = data & 0xFF;

            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)arg2;
                retVal[4] = (byte)arg3;
                retVal[5] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3] ^ retVal[4]);
                return 6;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)arg2;
                retVal[3] = (byte)arg3;
                retVal[4] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3]);
                return 5;
            }
        }

        public static int speedStep128Packet(ref byte[] retVal, int address, bool longAddr, int speed, bool fwd)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            if (speed < 0 || speed > 127)
            {
                //printf("invalid speed %d\n", speed);
                return 0;
            }

            // end sanity checks, format output
            int arg1 = 0x3F;
            int arg2 = (speed & 0x7F) | (fwd ? 0x80 : 0);

            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)arg2;
                retVal[4] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3]);
                return 5;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)arg2;
                retVal[3] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2]);
                return 4;
            }
        }
        /**
         * From NMRA RP 9.2.1
        * A speed and direction instruction is used send information to motors
        * connected to Multi Function Digital Decoders. Instruction "010" indicates
        * a Speed and Direction Instruction for reverse operation and instruction
        * "011" indicates a Speed and Direction Instruction for forward operation.
        * In these instructions the data is used to control speed with bits 0-3
        * being defined exactly as in S-9.2 Section B. If Bit 1 of CV#29 has a
        * value of one (1), then bit 4 is used as an intermediate speed step, as
        * defined in S-9.2, Section B. If Bit 1 of CV#29 has a value of zero (0),
        * then bit 4 shall be used to control FL4. In this mode, Speed U0000 is
        * stop, speed U0001 is emergency stop, speed U0010 is the first speed step
        * and speed U1111 is full speed. This provides 14 discrete speed steps in
        * each direction.
        */
        public static int speedStep28Packet(ref byte[] retVal, int address, bool longAddr, int speed, bool fwd)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            if (speed < 0 || speed > 28)
            {
                //printf("invalid speed %d\n", speed);
                return 0;
            }
            int speedC = (speed & 0x1F) >> 1;
            if (speed > 0)
                speedC = speedC + 1;
            int c = (speed & 0x01) << 4;	// intermediate speed step

            speedC = speedC + c;

            // end sanity checks, format output
            int arg1 = (fwd ? 0x60 : 0x40) | speedC;

            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2]);
                return 4;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)(retVal[0] ^ retVal[1]);
                return 3;
            }
        }
        public static int speedStep14Packet(ref byte[] retVal, int address, bool longAddr, int speed, bool fwd, bool F0)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            if (speed < 0 || speed > 15)
            {
                //printf("invalid speed %d\n", speed);
                return 0;
            }

            int speedC = (speed & 0xF);
            if (F0)
                speedC = speedC + 0x10;

            // end sanity checks, format output
            int arg1 = (fwd ? 0x60 : 0x40) | speedC;

            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2]);
                return 4;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)(retVal[0] ^ retVal[1]);
                return 3;
            }
        }

        public static int function0Through4Packet(ref byte[] retVal, int address, bool longAddr, bool f0, bool f1, bool f2, bool f3, bool f4)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            // end sanity check, format output

            int arg1 = 0x80 |
                      (f0 ? 0x10 : 0) |
                      (f1 ? 0x01 : 0) |
                      (f2 ? 0x02 : 0) |
                      (f3 ? 0x04 : 0) |
                      (f4 ? 0x08 : 0);

            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2]);
                return 4;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)(retVal[0] ^ retVal[1]);
                return 3;
            }
        }
        public static int function5Through8Packet(ref byte[] retVal, int address, bool longAddr, bool f5, bool f6, bool f7, bool f8)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            // end sanity check, format output
            int arg1 = 0xB0 |
                      (f8 ? 0x08 : 0) |
                      (f7 ? 0x04 : 0) |
                      (f6 ? 0x02 : 0) |
                      (f5 ? 0x01 : 0);

            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2]);
                return 4;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)(retVal[0] ^ retVal[1]);
                return 3;
            }
        }
        public static int function9Through12Packet(ref byte[] retVal, int address, bool longAddr, bool f9, bool f10, bool f11, bool f12)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            // end sanity check, format output
            int arg1 = 0xA0 |
                      (f12 ? 0x08 : 0) |
                      (f11 ? 0x04 : 0) |
                      (f10 ? 0x02 : 0) |
                      (f9 ? 0x01 : 0);

            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2]);
                return 4;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)(retVal[0] ^ retVal[1]);
                return 3;
            }
        }
        public static int function13Through20Packet(ref byte[] retVal, int address, bool longAddr, bool f13, bool f14, bool f15, bool f16, bool f17, bool f18, bool f19, bool f20)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            // end sanity check, format output
            int arg1 = 0xDE;
            int arg2 = (f20 ? 0x80 : 0) |
                        (f19 ? 0x40 : 0) |
                        (f18 ? 0x20 : 0) |
                        (f17 ? 0x10 : 0) |
                        (f16 ? 0x08 : 0) |
                        (f15 ? 0x04 : 0) |
                        (f14 ? 0x02 : 0) |
                        (f13 ? 0x01 : 0);

            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)arg2;
                retVal[4] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3]);
                return 5;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)arg2;
                retVal[3] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2]);
                return 4;
            }
        }
        public static int function21Through28Packet(ref byte[] retVal, int address, bool longAddr, bool f21, bool f22, bool f23, bool f24, bool f25, bool f26, bool f27, bool f28)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            // end sanity check, format output
            int arg1 = 0xDF;
            int arg2 = (f28 ? 0x80 : 0) |
                        (f27 ? 0x40 : 0) |
                        (f26 ? 0x20 : 0) |
                        (f25 ? 0x10 : 0) |
                        (f24 ? 0x08 : 0) |
                        (f23 ? 0x04 : 0) |
                        (f22 ? 0x02 : 0) |
                        (f21 ? 0x01 : 0);

            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)arg2;
                retVal[4] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3]);
                return 5;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)arg2;
                retVal[3] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2]);
                return 4;
            }
        }
        
        /**
         * Provide an NMRA analog control instruction
         *<P>Note that the NMRA draft of Fall 2004 only defines the value
         * of "1" for the "function parameter", calling that the value for
         * "volume control".  However, DCC systems in the wild have been
         * observed to use 0x7F for the function byte for volume control.
         * @param address  DCC locomotive address
         * @param longAddr true if this is a long address, false if short address
         * @param function see note above
         * @param value  value to be sent in analog control instruction
         */
        public static int analogControl(ref byte[] retVal, int address, bool longAddr, int function, int value)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!

            // end sanity check, format output
            int arg1 = 0x3D;  // analog instruction tag

            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)(function & 0xFF);
                retVal[4] = (byte)(value & 0xFF);
                retVal[5] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3] ^ retVal[4]);
                return 6;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)(function & 0xFF);
                retVal[3] = (byte)(value & 0xFF);
                retVal[4] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3]);
                return 5;
            }
        }
        /**
         * Provide an NMRA consist control instruction
         * @param address  DCC locomotive address
         * @param longAddr true if this is a long address, false if short address
         * @param consist the consist address to set for this locomotive. Send
         * 00 as consist address if deleteing from consist.
         * @param directionNormal true if the normal direction of travel for this
         * address is the normal direction of travel for the consist.
         */
        public static int consistControl(ref byte[] retVal, int address, bool longAddr, int consist, bool directionNormal)
        {
            if (!AddressCheck(address, longAddr))
                return 0;  // failed!
            else if (!AddressCheck(consist, false))
                return 0;  // failed - Consist address is not a short address!

            // end sanity check, format output
            int arg1 = 0x10;  // Consist Control instruction tag
            if (directionNormal)
                arg1 |= 0x02;   // Forward Direction
            else
                arg1 |= 0x03;   // Reverse Direction

            if (longAddr)
            {
                // long address form
                retVal[0] = (byte)(192 + ((address / 256) & 0x3F));
                retVal[1] = (byte)(address & 0xFF);
                retVal[2] = (byte)arg1;
                retVal[3] = (byte)(consist & 0xFF);
                retVal[4] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2] ^ retVal[3]);
                return 5;
            }
            else
            {
                // short address form
                retVal[0] = (byte)(address & 0xFF);
                retVal[1] = (byte)arg1;
                retVal[2] = (byte)(consist & 0xFF);
                retVal[3] = (byte)(retVal[0] ^ retVal[1] ^ retVal[2]);
                return 4;
            }
        }

        static bool AddressCheck(int address, bool longAddr)
        {
            if (address < 0)
            {  // zero is valid broadcast
                //printf("invalid address %d\n", address);
                return false;
            }
            if (longAddr && (address > (255 + (231 - 192) * 256)))
            {
                //printf("invalid address %d\n", address);
                return false;
            }
            if (!longAddr && (address > 127))
            {
                //printf("invalid address %d\n", address);
                return false;
            }
            return true;  // passes test, hence OK
        }

        //public static int createStream(ref Stream serialStream, ref byte[] packet, int packetsize)
        //{
        //    serialStream = (packet != null ? new MemoryStream(packet) : null);
        //    return (serialStream == null ? 0 :(int)serialStream.Length);
        //}
    }
}
