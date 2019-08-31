// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.MicroKorgXlSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class MicroKorgXlProgram : Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBank"></param>
        /// <param name="index"></param>
        public MicroKorgXlProgram(IProgramBank programBank, int index)
            : base(programBank, index)
        {
            Id = $"{programBank.Id}{index/8 + 1}{index%8 + 1}";
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
        public override int MaxNameLength => 8;


        /// <summary>
        /// 
        /// </summary>
        public override bool IsEmptyOrInit => ((Name == string.Empty) || (Name.Contains("Init") && Name.Contains("Prog")));


        /// <summary>
        /// As overridden, but without changing genre/category (is fixed in MicroKorg XL).
        /// </summary>
        public override void Clear()
        {
            Name = string.Empty;
            RaisePropertyChanged(string.Empty, false);
        }


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
        public override IParameter GetParam(ParameterNames.ProgramParameterName name)
        {
            IParameter parameter;

            switch (name)
            {
                // No OSC Mode

                case ParameterNames.ProgramParameterName.Category:
                    parameter = new FixedParameter();
                    ((FixedParameter)parameter).Set(PcgRoot, PcgRoot.Content, FixedParameter.EType.Genre, this);
                    break;

                case ParameterNames.ProgramParameterName.SubCategory:
                    parameter = new FixedParameter();
                    ((FixedParameter)parameter).Set(PcgRoot, PcgRoot.Content, FixedParameter.EType.Category, this);
                    break;

                default:
                    throw new ApplicationException("Illegal case");
            }
            return parameter;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override int GetFixedParameterValue(FixedParameter.EType type)
        {
            int value;

            switch (type)
            {
                case FixedParameter.EType.Genre:
                    value = Index/8;
                    break;

                case FixedParameter.EType.Category:
                    value = Index%8;
                    break;

                default:
                    throw new ApplicationException("Illegal case");
            }

            return value;
        }
    }
}
