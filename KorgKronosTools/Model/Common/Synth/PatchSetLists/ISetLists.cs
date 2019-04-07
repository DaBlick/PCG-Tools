// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.Common.Synth.PatchSetLists
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISetLists : IBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="changes"></param>
        void ChangeProgramReferences(Dictionary<IProgram, IProgram> changes);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="changes"></param>
        void ChangeCombiReferences(Dictionary<ICombi, ICombi> changes);


        /// <summary>
        /// 
        /// </summary>
        int Stl2PcgOffset { get; set; }
    }
}
