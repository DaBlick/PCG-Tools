// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Diagnostics;

namespace Common.Utils
{
    public abstract class BitsUtil
    {
        // ReSharper disable CSharpWarnings::CS1570
        /// <summary>
        /// 
        /// Example might be reversed in bit order
        /// highbit=6 lowbit=5      highbit=4 lowbit=2      Step    Algorithm
        /// 7654 3210               7654 3210               A       1111 1111
        /// 1000 0000               1110 0000               D       A < (highbit + 1)
        /// 0001 1111               0000 0011               E       A > (8 - lowbit)
        /// 1001 1111               1110 0011               F       D | E
        /// 0VV0 0000               000V VV00               G       V < lowbit
        /// dVVd dddd               dddV VVdd               H       Data & F | G
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="highBit"></param>
        /// <param name="lowBit"></param>
        /// <param name="value"></param>
        /// <returns>True if changed, false otherwise</returns>
        // ReSharper restore CSharpWarnings::CS1570
        public static bool SetBits(byte[] data, int offset, int highBit, int lowBit, int value)
        {
            Debug.Assert(data != null);
            Debug.Assert(offset < data.Length);
            Debug.Assert(0 <= lowBit);
            Debug.Assert(lowBit <= highBit);
            Debug.Assert(highBit <= 7);
            if (value < 0)
            {
                Console.WriteLine("x");
            }
            Debug.Assert(value >= 0);
            Debug.Assert(value < (2 << (highBit - lowBit + 1)));

            var orgData = data[offset];
            var highPart = (byte) (0xFF << (highBit + 1));
            var lowPart = (byte) (0xFF >> (8 - lowBit));
            var combined = highPart | lowPart;
            data[offset] = (byte) (data[offset] & combined | (value << lowBit));
            return (orgData != data[offset]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteValue"></param>
        /// <param name="highBit"></param>
        /// <param name="lowBit"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static int SetBits(byte byteValue, int highBit, int lowBit, int newValue)
        {
            Debug.Assert(0 <= lowBit);
            Debug.Assert(lowBit <= highBit);
            Debug.Assert(highBit <= 7);
            Debug.Assert(newValue >= 0);
            Debug.Assert(newValue < (2 << (highBit - lowBit + 1)));

            var highPart = (byte) (0xFF << (highBit + 1));
            var lowPart = (byte) (0xFF >> (8 - lowBit));
            var combined = highPart | lowPart;
            byte newByteValue = (byte) (byteValue & combined | (newValue << lowBit));
            return newByteValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int BitsNeeded(int value)
        {
            var bitsUsed = 0;
            do
            {
                bitsUsed++;
                value >>= 1;
            } while (value > 0);

            return bitsUsed;
        }


        /// <summary>
        /// Set multiple bytes, starting on startBit and with length nrOfBits, so starting/ending somewhere
        /// in two different bytes.
        /// Warning: only works for 32 bits max.
        /// Returns true when changed.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startByte"></param>
        /// <param name="startBit"></param>
        /// <param name="nrOfBits"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetMultiByteBits(byte[] data, int startByte, int startBit, int nrOfBits, int value)
        {
            Debug.Assert(data != null);
            Debug.Assert(startByte >= 0);
            Debug.Assert(startByte < data.Length);
            Debug.Assert(startBit >= 0);
            Debug.Assert(startBit <= 7);
            Debug.Assert((startByte + ((startBit + nrOfBits + 7)/8)) < data.Length);
            Debug.Assert(nrOfBits >= 0);
            Debug.Assert(nrOfBits <= 32);
            Debug.Assert(BitsNeeded(value) <= nrOfBits);

            var changed = false;
            while (nrOfBits > 0)
            {
                changed |= SetBit(data, startByte, startBit, (value >> (nrOfBits - 1)) & 0x01); // Set MSB
                startBit--;
                if (startBit < 0)
                {
                    startByte++;
                    startBit = 7;
                }

                nrOfBits--;
            }

            return changed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="highBit"></param>
        /// <param name="lowBit"></param>
        /// <returns></returns>
        public static int GetBits(byte[] data, int offset, int highBit, int lowBit)
        {
            if (data == null)
            {
                return 0;
            }

            Debug.Assert(offset < data.Length);
            Debug.Assert(0 <= lowBit);
            Debug.Assert(lowBit <= highBit);
            Debug.Assert(highBit <= 7);

            return (data[offset] & ((0xFF >> (7 - highBit + lowBit)) << lowBit)) >> lowBit;            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="highBit"></param>
        /// <param name="lowBit"></param>
        /// <returns></returns>
        public static int GetBits(byte value, int highBit, int lowBit)
        {
            Debug.Assert(0 <= lowBit);
            Debug.Assert(lowBit <= highBit);
            Debug.Assert(highBit <= 7);

            return (value & ((0xFF >> (7 - highBit + lowBit)) << lowBit)) >> lowBit;            
        }


        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="bit"></param>
        /// <param name="bitValue"></param>
        /// <returns>True if changed, false otherwise</returns>
        public static bool SetBit(byte[] data, int offset, int bit, int bitValue = 1)
        {
            Debug.Assert(offset < data.Length);
            Debug.Assert(0 <= bit);
            Debug.Assert(bit <= 7);
            Debug.Assert((bitValue == 0) || (bitValue == 1));

            var orgData = data[offset];
            if (bitValue == 1)
            {
                data[offset] |= (byte) (0x01 << bit);
            }
            else
            {
                data[offset] &= (byte) (~(0x01 << bit));
            }

            return (orgData != data[offset]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="bit"></param>
        /// <param name="bitValue"></param>
        /// <returns>True if changed, false otherwise</returns>
        public static bool SetBit(byte[] data, int offset, int bit, bool bitValue)
        {
            return SetBit(data, offset, bit, bitValue ? 1 : 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static bool GetBit(byte[] data, int offset, int bit)
        {
            if (data == null)
            {
                return false;
            }

            Debug.Assert(offset < data.Length);
            Debug.Assert(0 <= bit);
            Debug.Assert(bit <= 7);
            return (((data[offset] >> bit) & 0x01) == 0x01);
        }



        /// <summary>
        /// Treat left bit (bit number is totalBits as signed).
        /// </summary>
        /// <param name="totalBits"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToSignedBit(int totalBits, int value)
        {
            Debug.Assert(totalBits >= 1);
            Debug.Assert(value >= 0);
            Debug.Assert(value < (1 << totalBits));

            // Calculate maximum value allowed for value (otherwise it is negative)
            var maxValue = (1 << (totalBits - 1)) - 1;

            // Substract min_value and remove signed bit if negative.
            return value >= maxValue ? -(maxValue + 1) + (value & maxValue) : value;
        }
    }
}
