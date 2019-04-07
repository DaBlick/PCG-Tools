// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using PcgTools.Model.Common;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.Ms2000Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class Ms2000Program : Program
    {
        /// <summary>
        /// 
        /// </summary>
        public enum EMode 
        {
            Single,
            Split,
            Layer,
            Vocoder
        };


        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBank"></param>
        /// <param name="index"></param>
        public Ms2000Program(IBank programBank, int index)
            : base(programBank, index)
        {
            Id = $"{programBank.Id}{(index + 1).ToString("00")}";
        }


        /// <summary>
        /// Remove characters above ASCII 127.
        /// </summary>
        public override string Name
        {
            get
            {
                var name = GetChars(0, MaxNameLength);
                return name;
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
        public override int MaxNameLength => 12;


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
                // No OSC Mode

                case ParameterNames.ProgramParameterName.Category:
                    parameter = new FixedParameter();
                    ((FixedParameter)parameter).Set(PcgRoot, PcgRoot.Content, FixedParameter.EType.Category, this);
                    break;

                case ParameterNames.ProgramParameterName.Mode:
                    parameter = new FixedParameter();
                    ((FixedParameter)parameter).Set(PcgRoot, PcgRoot.Content, FixedParameter.EType.Mode, this);
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
            var value = -1;

            switch (type)
            {
                case FixedParameter.EType.Category:
                    value = GetFixedParameterCategoryValue(value);
                    break;

                case FixedParameter.EType.Mode:
                    value = GetFixedParameterModeValue(value);

                    break;

                default:
                    throw new ApplicationException("Illegal case");
            }

            return value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int GetFixedParameterModeValue(int value)
        {
            var program = (IProgram) (GetParam(ParameterNames.ProgramParameterName.Mode).Patch);
            var voiceMode = Util.GetBits(program.PcgRoot.Content, program.ByteOffset + 16, 5, 4);

            // There is an error in the MS2000 Midi exclusive document, values of 1 and 2 are swapped
            switch (voiceMode)
            {
                case 0:
                    value = (int) EMode.Single;
                    break;

                case 1: // Error in MS2000 MIDI exclusive document: swapped with 2
                    value = (int) EMode.Split;
                    break;

                case 2: // Error in MS2000 MIDI exclusive document: swapped with 1
                    value = (int) EMode.Layer;
                    break;

                case 3:
                    value = (int) EMode.Vocoder;
                    break;
            }
            return value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int GetFixedParameterCategoryValue(int value)
        {
            var program = (IProgram) (GetParam(ParameterNames.ProgramParameterName.Category).Patch);
            var category = Util.GetBits(program.PcgRoot.Content, program.ByteOffset + 16, 7, 6);
            switch (category)
            {
                case 0:
                    value = (int) EMode.Single;
                    break;

                case 1:
                    value = (int) EMode.Split;
                    break;

                case 2:
                    value = (int) EMode.Layer;
                    break;

                case 3:
                    value = (int) EMode.Vocoder;
                    break;
            }
            return value;
        }
    }
}
