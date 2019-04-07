// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.Text;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.ZeroSeries.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class ZeroSeriesProgram : MntxProgram
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBank"></param>
        /// <param name="index"></param>
        public ZeroSeriesProgram(IBank programBank, int index)
            : base(programBank, index)
        {
            switch (programBank.Type)
            {
                case BankType.EType.Int:
                    Id = $"{programBank.Id}{index.ToString("00")}";
                    break;

                case BankType.EType.Virtual:
                    Id = $"{programBank.Id}-{index.ToString("00")}";
                    break;

                // default: Do nothing.
            }
        }


        /// <summary>
        /// Note: ZeroSeries program names work different than ZeroSeries combi names.
        /// </summary>
        public override string Name
        {
            get
            {
                if (PcgRoot.Content == null)
                {
                    return string.Empty;
                }

                var name = new StringBuilder();

                for (var index = 0; index < MaxNameLength; index++)
                {
                    var character = PcgRoot.Content[ByteOffset + index];
                    // A 0 does not mean end of string.
                    name.Append((character == 0x00) ? ' ' : (char) character);
                }

                return name.ToString().Trim();
            }

            set
            {
                if (value != Name)
                {
                    SetChars(0, MaxNameLength, value);
                    OnPropertyChanged("Name");
                }

            }
        }
     

        /// <summary>
        /// 
        /// </summary>
        public override int MaxNameLength => 10;


        /// <summary>
        /// 
        /// </summary>
        public override bool IsEmptyOrInit => ((Name == string.Empty) || (Name.Contains("INIT") && Name.Contains("Prog")));


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
                case ParameterNames.ProgramParameterName.OscMode:
                    parameter = EnumParameter.Instance.Set(Root, Root.Content, ByteOffset + 10, 2, 0,
                        new List<string> { "Single", "Double", "Drums" }, this);
                    break;

                case ParameterNames.ProgramParameterName.Category:
                    var fixedParameter = new FixedParameter();
                    fixedParameter.Set(PcgRoot, PcgRoot.Content, FixedParameter.EType.Category, this);
                    parameter = fixedParameter;
                    break;

                default:
                    parameter = (IFixedParameter)base.GetParam(name);
                    break;
            }

            return parameter;
        }
    }
}

