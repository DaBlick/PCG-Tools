// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Text;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.ZeroSeries.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class ZeroSeriesCombi : MntxCombi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBank"></param>
        /// <param name="index"></param>
        public ZeroSeriesCombi(IBank combiBank, int index)
            : base(combiBank, index)
        {
            if (combiBank.Type == BankType.EType.Int)
            {
                Id = $"{combiBank.Id}{(index).ToString("00")}";
            }
            else if (combiBank.Type == BankType.EType.Virtual)
            {
                Id = $"{combiBank.Id}-{index.ToString("00")}";
            }
            else
            {
                throw new NotSupportedException("Unsupported bank type");
            }

            Timbres = new ZeroSeriesTimbres(this);
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
