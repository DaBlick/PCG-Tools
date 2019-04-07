// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common;
using PcgTools.Model.Common.File;
using PcgTools.Model.Common.Synth.MemoryAndFactory;

namespace PcgTools.Model.TritonSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TritonPcgFileReader : PcgFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        protected TritonPcgFileReader(IPcgMemory currentPcgMemory, byte[] content)
            : base(currentPcgMemory, content)
        {
            // PcgFileHeader PcgFileHeader;
            // Pcg1Chunk Pcg1Chunk;
            // Div1Chunk Div1Chunk;
            // Ini2Chunk Ini2Chunk;
            // Ini1Chunk Ini1Chunk;
            // Prg1Chunk Prg1Chunk;
            // Cmb1Chunk Cmb1Chunk;
            // Dkt1Chunk Dkt1Chunk;
            // Arp1Chunk Arp1Chunk;
            // Glb1Chunk Glb1Chunk;
        }


        /// <summary>
        /// LV: I noticed that these are overridden again in TritonExtreme and 
        /// TritonKarma subclasses with exactly the same values. Remove them there?
        /// </summary>
        protected override int Div1Offset => 0x18;


        /// <summary>
        /// 
        /// </summary>
        protected override int BetweenChunkGapSize => 8;


        /// <summary>
        /// 
        /// </summary>
        protected override int GapSizeAfterMbk1ChunkName => 0;


        /// <summary>
        /// 
        /// </summary>
        protected override int Pbk1NumberOfProgramsOffset => 8;


        /// <summary>
        /// 
        /// </summary>
        protected override int SizeBetweenCmb1AndCbk1 => 4;


        /// <summary>
        /// 
        /// </summary>
        protected override int Cbk1NumberOfCombisOffset => 8;


        /// <summary>
        /// 
        /// </summary>
        protected override int Dbk1NumberOfDrumKitsOffset => 8;


        /// <summary>
        /// Somehow there is a 4 byte too long GLB1 chunk in Tritons or the starting index should be 8 but then
        /// the categories offset will not be correct. Maybe that should be the correct solution though.
        /// </summary>
        protected override void ReadGlb1Chunk(int chunkSizeNotUsed)
        {
            var chunkSize = Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4);
            Index += 12; // Skip 'GLB1", chunk size and 4 other/unknown bytes.
            //CurrentPcgMemory.Chunks.Add(new Chunk("GLB1", Index, chunkSize));
            CurrentPcgMemory.Global.ByteOffset = Index;
            Index += chunkSize - 4; // 4 because initial 12 was too much (see above).
        }



        protected override int Dpi1NumberOfDrumPatternsOffset
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
