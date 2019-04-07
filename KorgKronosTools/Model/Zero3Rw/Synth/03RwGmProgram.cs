// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.Zero3Rw.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class Zero3RwGmProgram : Zero3RwProgram
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly string _name;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBank"></param>
        /// <param name="index"></param>
        /// <param name="name"></param>
        public Zero3RwGmProgram(IProgramBank programBank, int index, string name)
            : base(programBank, index)
        {
            _name = name;
            Id = $"{programBank.Id}{UserIndex:000}";
        }


        /// <summary>
        /// The user index is the same as index, except for GM programs which are named as GM001 instead of GM000 etc.
        /// </summary>
        public override int UserIndex => Index + 1;


        /// <summary>
        /// 
        /// </summary>
        public override string Name => _name;
    }
}
