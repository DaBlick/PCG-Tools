// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.KronosSpecific.Synth;

namespace PcgTools.ClipBoard
{
    /// <summary>
    /// 
    /// </summary>
    public class ClipBoardDrumKit : ClipBoardPatch, IClipBoardDrumKit
    {
        /// <summary>
        /// 
        /// </summary>
        public int KronosOs1516Bank { get; private set; }
        

        /// <summary>
        /// 
        /// </summary>
        public int KronosOs1516Patch { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumKit"></param>
        public ClipBoardDrumKit(IDrumKit drumKit)
            : base(drumKit.PcgRoot.Content, drumKit.ByteOffset, drumKit.ByteLength)
        {
            OriginalLocation = drumKit;

            var memory = drumKit.Root as PcgMemory;
            if ((memory != null) && (memory.PcgRoot.Model.OsVersion == Models.EOsVersion.EOsVersionKronos15_16))
            {
                KronosOs1516Bank = Util.GetInt(memory.Content, ((KronosDrumKit) drumKit).Drk2BankOffset, 1);
                KronosOs1516Patch = Util.GetInt(memory.Content, ((KronosDrumKit) drumKit).Drk2PatchOffset, 1);
            }
        }
    }
}
