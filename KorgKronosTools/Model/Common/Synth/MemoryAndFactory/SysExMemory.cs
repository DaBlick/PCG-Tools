// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Linq;

namespace PcgTools.Model.Common.Synth.MemoryAndFactory
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SysExMemory : PcgMemory, ISysExMemory
    {
        /// <summary>
        /// 
        /// </summary>
        public int SysExStartOffset { get; set; }


        /// <summary>
        /// 
        /// </summary>
        int SysExEndOffset { get; set; }


        /// <summary>
        /// Number of bytes after converting the 7 to 8 bits conversion.
        /// </summary>
        int _targetConvertedBytes;


        /// <summary>
        /// True if converted from 7 to 8 bits.
        /// </summary>
        bool _convertedFrom7To8Bits;

        
        /// <summary>
        /// True if patch names should duplicated in the beginning of the file before saving.
        /// </summary>
        readonly bool _patchNamesCopyNeeded;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="modelType"></param>
        /// <param name="contentType"></param>
        /// <param name="sysExStartOffset"></param>
        /// <param name="sysExEndOffset"></param>
        /// <param name="patchNamesCopyNeeded"></param>
        protected SysExMemory(string fileName, Models.EModelType modelType,
            ContentType contentType, int sysExStartOffset, int sysExEndOffset, bool patchNamesCopyNeeded)
            : base(fileName, modelType)
        {
            ContentTypeType = contentType;
            SysExStartOffset = sysExStartOffset;
            SysExEndOffset = sysExEndOffset;
            _convertedFrom7To8Bits = false;
            _patchNamesCopyNeeded = patchNamesCopyNeeded;
        }


        /// <summary>
        /// Convert everything from SysExStartOffset to _sysExEndOffset from 7 to 8 bits where the
        /// first out of eight target bytes contains the MS bits of the seven source bytes.
        /// /// This method is for READING a sysex file.
        /// </summary>
        public void Convert7To8Bits()
        {
            _convertedFrom7To8Bits = true;

            var newContent = new LinkedList<byte>();

            // Copy header as normal.
            int index;
            for (index = 0; index < SysExStartOffset + ModeChangesOffset; index++)
            {
                newContent.AddLast(Content[index]);
            }

            ConvertMain(newContent);

            // Copy footer.
            for (index = SysExEndOffset; index < Content.Length; index++)
            {
                newContent.AddLast(Content[index]);
            }

            Content = newContent.ToArray();

#if DEBUG
            System.IO.File.WriteAllBytes(@"D:\SYSEX.syx", Content);
#endif
        }


        /// <summary>
        /// Convert sysex data from 7 bits to 8 bits.
        /// </summary>
        /// <param name="newContent"></param>
        private void ConvertMain(LinkedList<byte> newContent)
        {
            var convert = true;
            int index = SysExStartOffset + ModeChangesOffset;
            _targetConvertedBytes = 0;

            while (convert)
            {
                index = ConvertMainPart(newContent, index, ref convert);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="newContent"></param>
        /// <param name="index"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        private int ConvertMainPart(LinkedList<byte> newContent, int index, ref bool convert)
        {
            var models = new List<Models.EModelType>
            {
                Models.EModelType.M1,
                Models.EModelType.M3R,
                Models.EModelType.MicroKorgXl,
                Models.EModelType.MicroKorgXlPlus,
                Models.EModelType.Ms2000,
                Models.EModelType.TSeries,
                Models.EModelType.XSeries,
                Models.EModelType.Z1,
                Models.EModelType.ZeroSeries,
                Models.EModelType.Zero3Rw
            };

            if (models.Contains(ModelType))
            {
                return ConvertMainPart7To8Bytes(newContent, index, out convert);
            }

            throw new ApplicationException("No conversion possible");
        }


        /// <summary>
        /// Convert max. 7 bytes to 8 bytes. See Manual of MS2000.
        /// </summary>
        /// <param name="newContent"></param>
        /// <param name="index"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        private int ConvertMainPart7To8Bytes(LinkedList<byte> newContent, int index, out bool convert)
        {
            int offset;
            for (offset = 1; offset < Math.Min(8, SysExEndOffset - index); offset++)
            {
                var bit7Set = ((byte) (Content[index] & (0x01 << (offset - 1))) != 0);

                newContent.AddLast((byte) ((bit7Set ? 0x80 : 0) + Content[index + offset]));
                _targetConvertedBytes++;
            }

            index += offset;
            convert = (index < SysExEndOffset);
            return index;
        }


        /// <summary>
        /// Convert everything from ConvertedSysExStartOffset to _convertedSysExEndOffset from 8 to 7 bits where the
        /// first out of eight target bytes contains the MS bits of the seven source bytes.
        /// This method is for WRITING a sysex file.
        /// </summary>
        void Convert8To7Bits()
        {
            var newContent = new List<byte>();

            // Copy header as normal.
            int index;
            for (index = 0; index < SysExStartOffset; index++)
            {
                newContent.Add(Content[index]);
            }

            // Convert sysex data from 8 to 7 bits.
            index = SysExStartOffset;
            var msbBitsIndex = 0;
            var convertedBytes = 0;

            while (convertedBytes < _targetConvertedBytes)
            {
                // Write MSB byte every 7 bytes.
                if (convertedBytes % 7 == 0)
                {
                    newContent.Add(0); // MSB bits, will be filled in later.
                    msbBitsIndex = newContent.Count - 1;
                }

                // Convert byte.
                var value = Content[index];
                
                newContent.Add((byte) (value & 0x7F));
                
                // Write MSB bit if needed.
                if ((value & 0x80) > 0)
                {
                    newContent[msbBitsIndex] |= (byte)(0x01 << (convertedBytes % 7));
                }

                index++;
                convertedBytes++;
            }

            // Convert footer.
            for (index = SysExStartOffset + _targetConvertedBytes; index < Content.Length; index++)
            {
                newContent.Add(Content[index]);
            }

            Content = newContent.ToArray();
        }


        /// <summary>
        /// Save and convert before if needed.
        /// </summary>
        /// <param name="saveAs"></param>
        /// <param name="saveToFile"></param>
        public override void SaveFile(bool saveAs, bool saveToFile)
        {
            if (_convertedFrom7To8Bits)
            {
                var originalContent = (byte[]) Content.Clone();
                Convert8To7Bits();
                base.SaveFile(saveAs, saveToFile);
                Content = originalContent;
            }
            else
            {
                if (_patchNamesCopyNeeded)
                {
                    CopyPatchNames();
                    base.SaveFile(saveAs, saveToFile);
                }
            }
        }


        /// <summary>
        /// Copy patch names into the beginning part of the file (before saving).
        /// </summary>
        protected virtual void CopyPatchNames()
        {
            // Do nothing.
        }


        /// <summary>
        /// Number of bytes of mode changes system exclusive: 
        /// F0 42 30 19 4E .. .. F7 
        /// Can be repeated multiple times, always 8 bytes.
        /// The offset should be taken into account for the StartSysExOffset for
        /// the 7/8 bits conversions.
        /// </summary>
        private int ModeChangesOffset { get; set; }
    }
}
