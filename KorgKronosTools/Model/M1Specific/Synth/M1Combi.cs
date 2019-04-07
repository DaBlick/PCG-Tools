// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Text;

using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.M1Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class M1Combi : MntxCombi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBank"></param>
        /// <param name="index"></param>
        public M1Combi(CombiBank combiBank, int index)
            : base(combiBank, index)
        {
            Id = $"{combiBank.Id}{(index).ToString("00")}";
            Timbres = new M1Timbres(this);
        }


        /// <summary>
        /// Substibute chars.
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

                    if (character == 0x00)
                    {
                        name.Append(' ');
                    }
                    else
                    {
                        name.Append((char) (character));
                    }
                }

                return name.ToString().Trim();
            }

            set
            {
                if (value != Name)
                {
                    SetChars(0, MaxNameLength, value);

                    // Add spaces.
                    for (var index = value.Length; index < MaxNameLength; index++)
                    {
                        PcgRoot.Content[ByteOffset + index] = (byte)' ';
                    }

                    OnPropertyChanged("Name");
                }

            }
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
        public override IParameter GetParam(ParameterNames.CombiParameterName name)
        {
            IParameter parameter;

            switch (name)
            {

            default:
                parameter = base.GetParam(name);
                break;
            }
            return parameter;
        }
    }
}
