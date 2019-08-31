// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Common.Utils;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.OldParameters
{
    /// <summary>
    /// 
    /// </summary>
    public class IntParameter : Parameter
    {
        /// <summary>
        /// 
        /// </summary>
        private int _highBit;

        /// <summary>
        /// 
        /// </summary>
        private int _lowBit;


        /// <summary>
        /// 
        /// </summary>
        private int _nrOfBytes;


        /// <summary>
        /// 
        /// </summary>
        private bool _msbToLsb;


        /// <summary>
        /// 
        /// </summary>
        private bool _signed;


        /// <summary>
        /// 
        /// </summary>
        private static IntParameter _instance;


        /// <summary>
        /// 
        /// </summary>
        public static IntParameter Instance => _instance ?? (_instance = new IntParameter());


        /// <summary>
        /// Set for single byte.
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="pcgData"></param>
        /// <param name="pcgOffsetStart"></param>
        /// <param name="highBit"></param>
        /// <param name="lowBit"></param>
        /// <param name="signed"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        public IntParameter Set(
            IMemory memory, byte[] pcgData, int pcgOffsetStart, int highBit, int lowBit, bool signed, IPatch patch)
        {
            Set(memory, pcgData, pcgOffsetStart, patch);

            _nrOfBytes = 1;
            _highBit = highBit;
            _lowBit = lowBit;
            _msbToLsb = true;
            _signed = signed;

            return this;
        }


        /// <summary>
        /// Set for bits over multiple bytes where the byes are in order MSB to LSB.
        /// </summary>
        /// <returns></returns>
        public IntParameter SetMultiBytes(
            IMemory memory, byte[] pcgData, int pcgOffsetStart, int nrOfBytes, bool msbToLsb, bool signed, IPatch patch)
        {
            Set(memory, pcgData, pcgOffsetStart, patch);

            Debug.Assert(nrOfBytes == 2);
            _nrOfBytes = nrOfBytes;
            _highBit = 7;
            _lowBit = 0;
            _msbToLsb = msbToLsb;
            _signed = signed;

            return this;
        }


        /// <summary>
        /// Returns the int value.
        /// </summary>
        public override dynamic Value
        {
            get
            {
                Debug.Assert(PcgData != null);

                int value;

                switch (_nrOfBytes)
                {
                    case 1:
                        Debug.Assert(_msbToLsb);
                        value = BitsUtil.GetBits(PcgData, PcgOffset, _highBit, _lowBit);
                        value = value - (((_signed && (value & 0x80) == 0x80)) ? 256 : 0);
                        break;

                    case 2:
                        value = GetTwoBytesValue();
                        break;

                    default:
                        throw new NotSupportedException();
                }

                return value;
            }

            set
            {
                Debug.Assert(PcgData != null);

                switch (_nrOfBytes)
                {

                    case 1:
                        Debug.Assert(_msbToLsb);
                        PcgMemory.IsDirty |= BitsUtil.SetBits(PcgData, PcgOffset, _highBit, _lowBit, value);
                        break;

                    case 2:
                        SetTwoBytesValue(value);
                        break;

                    default:
                        throw new NotSupportedException();
                }


                if (Patch != null)
                {
                    Patch.RaisePropertyChanged(string.Empty, false);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetTwoBytesValue()
        {
            int value;

            Debug.Assert(_highBit == 7);
            Debug.Assert(_lowBit == 0);

            var byte0 = PcgMemory.Content[PcgOffset];
            var byte1 = PcgMemory.Content[PcgOffset + 1];

            if (_msbToLsb)
            {
                // Like on M50, Detune
                value = byte0*256 + byte1 - (((byte0 & 0xF0) == 0xF0) ? 65536 : 0);
            }
            else
            {
                // Like on the Kronos, Detune
                value = byte1*256 + byte0;
            }
            return value;
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetTwoBytesValue(int value)
        {
            Debug.Assert(_highBit == 7);
            Debug.Assert(_lowBit == 0);

            PcgMemory.IsDirty |= (PcgMemory.Content[PcgOffset] != 0) ||
                                 (PcgMemory.Content[PcgOffset + 1] != 0);
            PcgMemory.Content[PcgOffset] = _msbToLsb ? (byte) (value / 256) : (byte) (value % 256);
            PcgMemory.Content[PcgOffset + 1] = _msbToLsb ? (byte) (value % 256) : (byte) (value / 256);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArray"></param>
        // ReSharper disable once UnusedMember.Local
        private void CheckReverse(ref byte[] byteArray)
        {
            if ((_msbToLsb && BitConverter.IsLittleEndian) ||
                (!_msbToLsb && !BitConverter.IsLittleEndian))
            {
                var bytes = new List<byte>(byteArray);
                bytes.Reverse();
            }
        }
    }
}