// (c) Copyright 2011-2016 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Text;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.M3rSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class M3RProgram : MntxProgram
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBank"></param>
        /// <param name="index"></param>
        public M3RProgram(ProgramBank programBank, int index)
            : base(programBank, index)
        {
            Id = string.Format("{0}{1}", programBank.Id, (index).ToString("00"));
        }


        /// <summary>
        /// Note: M3r program names work different than M3r combi names.
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
        public override int MaxNameLength
        {
            get { return 10; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsEmptyOrInit
        {
            get { return ((Name == String.Empty) || (Name.Contains("INIT") && Name.Contains("Prog"))); }
        }


        /// <summary>
        /// As overridden, but without changing genre/category (is fixed in MicroKorg XL).
        /// </summary>
        public override void Clear()
        {
            Name = String.Empty;
            RaisePropertyChanged(String.Empty, false);
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
        public override IParameter GetParam(string name)
        {
            IParameter parameter;

            switch (name)
            {
                case "Category":
                    parameter = new FixedParameter();
                    ((FixedParameter)parameter).Set(Root, Root.Content, FixedParameter.EType.Category, this);
                    break;

                default:
                    throw new ApplicationException("Illegal case");
            }

            return parameter;
        }
    }
}

