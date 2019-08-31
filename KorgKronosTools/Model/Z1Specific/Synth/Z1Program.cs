// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using PcgTools.Model.Common;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.Z1Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class Z1Program : MntxProgram
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBank"></param>
        /// <param name="index"></param>
        public Z1Program(IBank programBank, int index)
            : base(programBank, index)
        {
            Id = $"{programBank.Id}{(index).ToString("00")}";
        }
     

        /// <summary>
        /// 
        /// </summary>
        public override int MaxNameLength => 16;


        /// <summary>
        /// 
        /// </summary>
        public override bool IsEmptyOrInit => ((Name == string.Empty) || (Name.Contains("INIT") && Name.Contains("Prog")));


        /// <summary>
        /// As overridden, but without changing genre/category
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
            IParameter parameter = null;

            switch (name)
            {
                // No OSC Mode

                case ParameterNames.ProgramParameterName.Category:
                    parameter = new FixedParameter();
                    ((FixedParameter)parameter).Set(PcgRoot, PcgRoot.Content, FixedParameter.EType.Category, this);
                    break;
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
                case FixedParameter.EType.Category:
                    value = Util.GetInt(PcgRoot.Content, ByteOffset + 16, 1);
                    break;

                default:
                    throw new NotSupportedException("Unsupported fixed parameter type");
            }

            return value;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public override void SetFixedParameterValue(FixedParameter.EType type, int value)
        {
            switch (type)
            {
                case FixedParameter.EType.Category:
                    Util.SetInt(PcgRoot, PcgRoot.Content, ByteOffset + 16, 1, value);
                    OnPropertyChanged("", false);
                    break;

                default:
                    throw new NotSupportedException("Illegal fixed parameter type");
            }
        }
    }
}

