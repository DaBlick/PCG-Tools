using System.Diagnostics;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;

namespace PcgTools.Model.MSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MDrumKit : DrumKit
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumKitBank"></param>
        /// <param name="index"></param>
        protected MDrumKit(IBank drumKitBank, int index)
            : base(drumKitBank, index) 
        {
            // Override the Id set by the base class, since for M-models, the index-part of the Id seems to
            // continue into the next bank (it does not restart at 000 for the first DrumKit in the bank).
            var drumKitBankIndex = PcgRoot.DrumKitBanks.BankCollection.IndexOf(drumKitBank);
            Debug.Assert(drumKitBankIndex >= 0);
            var indexInId = index;
            if (drumKitBankIndex > 0) // M-synths have one INT drumkit bank
            {
                //IMPR Check if difference is needed.
                //indexInId += drumKitBank.NrOfPatchesIntBank + (drumKitBankIndex - 1) * MDrumKitBank.NrOfPatchesUserBank;
            }
            Id = $"{drumKitBank.Id}{indexInId.ToString("000")}";
        }

        
        /// <summary>
        /// 
        /// </summary>
        public override string Name
        {
            get
            {
                return GetChars(0, MaxNameLength);
            }

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
        public override int MaxNameLength => 24;


        /// <summary>
        /// 
        /// </summary>
        public override bool IsEmptyOrInit => ((Name == string.Empty) || (Name.Contains("Init") && Name.Contains("Drum") && Name.Contains("Kit")) || 
                                               (Name.Contains("Drumkit    U")));
    }
}
