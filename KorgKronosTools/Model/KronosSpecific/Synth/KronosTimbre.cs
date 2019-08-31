// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.KronosOasysSpecific.Synth;

namespace PcgTools.Model.KronosSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosTimbre : KronosOasysTimbre
    {
        /// <summary>
        /// 
        /// </summary>
        static int TimbresSizeConstant => 188;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        public KronosTimbre(ITimbres timbres, int index)
            : base(timbres, index, TimbresSizeConstant)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override IParameter GetParam(ParameterNames.TimbreParameterName name)
        {
            IParameter parameter;

            switch (name)
            {
            case ParameterNames.TimbreParameterName.Detune:
                parameter = IntParameter.Instance.SetMultiBytes(
                    Root, Root.Content, TimbresOffset + 8, 2, false, true, Parent as IPatch);
                break;

            default:
                parameter = base.GetParam(name);
                break;
            }
            return parameter;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherTimbre"></param>
        public override void Swap(ITimbre otherTimbre)
        {
            base.Swap(otherTimbre);

            if (PcgRoot.Model.OsVersion == Models.EOsVersion.EOsVersionKronos15_16)
            {
                var tempProgram = UsedProgram;
                UsedProgram = otherTimbre.UsedProgram;
                otherTimbre.UsedProgram = tempProgram;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override int UsedProgramBankId
        {
            get
            {
                int bankOffset;
                if (PcgRoot.Model.OsVersion == Models.EOsVersion.EOsVersionKronos15_16)
                {
                    var cmb2PcgOffset = ((KronosCombiBank)(Combi.Parent)).Cbk2PcgOffset;
                    if (cmb2PcgOffset == 0)
                    {
                        bankOffset = TimbresOffset + 1;
                    }
                    else
                    {
                        bankOffset = cmb2PcgOffset + Combi.Index * ((Timbres)Parent).TimbresPerCombi + Index;
                    }
                }
                else
                {
                    bankOffset = TimbresOffset + 1;
                }
                return Combi.PcgRoot.Content[bankOffset];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override IProgramBank UsedProgramBank
        {
            get
            {
                var pcgData = Combi.PcgRoot.Content;
                return pcgData == null ? null : (IProgramBank) PcgRoot.ProgramBanks.GetBankWithPcgId(UsedProgramBankId);
            }
            protected set
            {
                // Set in CMB1.
                ((Combi)(Parent.Parent)).PcgRoot.Content[TimbresOffset + 1] =
                    (PcgRoot.Model.OsVersion == Models.EOsVersion.EOsVersionKronos15_16) && 
                    (value.Type == BankType.EType.UserExtended) ?
                    (byte)(((ProgramBank)(PcgRoot.ProgramBanks[12])).PcgId) : (byte)(value.PcgId);

                // Set in CMB2.
                if (PcgRoot.Model.OsVersion == Models.EOsVersion.EOsVersionKronos15_16)
                {
                    // Set item in CBK2.
                    var cmb2PcgOffset = ((KronosCombiBank) (Combi.Parent)).Cbk2PcgOffset;
                    var bankOffset = cmb2PcgOffset + Combi.Index * ((Timbres) Parent).TimbresPerCombi + Index;
                    Combi.PcgRoot.Content[bankOffset] = (byte)(value.PcgId);
                }
                RefillColumns();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override int UsedProgramId => Combi.PcgRoot.Content[GetProgramOffset()];


        /// <summary>
        /// 
        /// </summary>
        public override int ProgramRawIndex => ((ISongMemory)Root).Content[TimbresOffset];


        /// <summary>
        /// 
        /// </summary>
        public override int ProgramRawBankIndex => ((ISongMemory)Root).Content[TimbresOffset + 1];


        /// <summary>
        /// 
        /// </summary>
        public override IProgram UsedProgram
        {
            get
            {
                if (Parent is ISongTimbres)
                {
                    var connectedPcgMemory = ((ISongMemory) Root).ConnectedPcgMemory;
                    if (connectedPcgMemory != null)
                    {
                        return connectedPcgMemory.GetPatchByRawIndices(ProgramRawBankIndex, ProgramRawIndex);
                    }
                    else
                    {
                        return null; // no connected PCG file.
                    }
                }

                return Combi.PcgRoot.Content == null ? null : base.UsedProgram;
            }
            set
            {
                if (Parent is ISongTimbres)
                {
                    // Do something //TODO
                    return;
                }

                // Set in CMB1, if a new bank is used, set index to 127.
                Combi.PcgRoot.Content[TimbresOffset] = ((PcgRoot.Model.OsVersion == Models.EOsVersion.EOsVersionKronos15_16) && 
                    (((IProgramBank)(value.Parent)).Type == BankType.EType.UserExtended)) ? 
                    (byte) 127 : (byte) value.Index;

                // Set in CMB2.
                if (PcgRoot.Model.OsVersion == Models.EOsVersion.EOsVersionKronos15_16)
                {
                    // Set item in CBK2.
                    var cmb2PcgOffset = ((KronosCombiBank) (Combi.Parent)).Cbk2PcgOffset;
                    var programOffset = cmb2PcgOffset + ((CombiBank) (Combi.Parent)).NrOfPatches * ((Timbres) Parent).TimbresPerCombi +
                        ((Combi) Parent.Parent).Index * ((Timbres) Parent).TimbresPerCombi + Index;
                    Combi.PcgRoot.Content[programOffset] = (byte)value.Index;
                }

                // Set bank.
                UsedProgramBank = (IProgramBank)value.Parent;
                RefillColumns();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetProgramOffset()
        {
            int programOffset;
            if (PcgRoot.Model.OsVersion == Models.EOsVersion.EOsVersionKronos15_16)
            {
                var cmb2PcgOffset = ((KronosCombiBank)(Combi.Parent)).Cbk2PcgOffset;
                if (cmb2PcgOffset == 0)
                {
                    programOffset = TimbresOffset;
                }
                else
                {
                    programOffset = cmb2PcgOffset + ((CombiBank)(Combi.Parent)).NrOfPatches * ((Timbres)Parent).TimbresPerCombi +
                       Combi.Index * ((Timbres)Parent).TimbresPerCombi + Index;
                }
            }
            else
            {
                programOffset = TimbresOffset;
            }
            return programOffset;
        }
    }
}
