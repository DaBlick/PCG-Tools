// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Properties;

namespace PcgTools.Model.TrinitySpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TrinityCombi : Combi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBank"></param>
        /// <param name="index"></param>
        public TrinityCombi(IBank combiBank, int index)
            : base(combiBank, index)
        {
            Timbres = new TrinityTimbres(this);
        }

        
        /// <summary>
        /// 
        /// </summary>
        public override string Name
        {
            get { return GetChars(0, MaxNameLength); }

            set
            {
                if (Name != value)
                {
                    SetChars(0, MaxNameLength, value);
                    OnPropertyChanged("Name");
                }
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public override int MaxNameLength => 16;


        /// <summary>
        /// 
        /// </summary>
        public override bool IsEmptyOrInit => ((Name == string.Empty) || (Name.Contains("Init") && Name.Contains("Combi")));


        /// <summary>
        /// Sets parameters after initialization.
        /// </summary>
        public override void SetParameters()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override IParameter GetParam(ParameterNames.CombiParameterName name)
        {
            IParameter parameter;

            switch (name)
            {
                case ParameterNames.CombiParameterName.Category:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, PcgRoot.Content, ByteOffset + 16,
                        Settings.Default.TrinityCategorySetA ? 3 : 7,
                        Settings.Default.TrinityCategorySetA ? 0 : 4,
                        false, this);
                    break;

                case ParameterNames.CombiParameterName.Tempo:
                    // Tempo on a Triton is only 1 byte (int) iso 2 for M series, Oasys/Kronos and is a float.
                    parameter = IntParameter.Instance.Set(PcgRoot, PcgRoot.Content, ByteOffset + 541, 7, 0, false, this);
                    break;

                default:
                    parameter = base.GetParam(name);
                    break;
            }
            return parameter;
        }
    }
}
