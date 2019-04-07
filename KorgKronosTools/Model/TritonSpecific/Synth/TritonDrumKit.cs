using System.Diagnostics;
using System.Text.RegularExpressions;


// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;

namespace PcgTools.Model.TritonSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TritonDrumKit : DrumKit
    {
        /// <summary>
        /// Override the Id set by the base class, since for Triton-models, the index-part of the Id seems to
        /// continue into the next bank (it does not restart at 000 for the first DrumKit in the bank).
        /// </summary>
        /// <param name="drumKitBank"></param>
        /// <param name="index"></param>
        protected TritonDrumKit(IBank drumKitBank, int index)
            : base(drumKitBank, index) 
        {
            var drumKitBankIndex = PcgRoot.DrumKitBanks.BankCollection.IndexOf(drumKitBank);
            Debug.Assert(drumKitBankIndex >= 0);
            var indexInId = index;
            if (drumKitBankIndex > 0)
            {
                // INT and USER Drumkit banks have same size for Tritons, so no use in checking bank types here.
                indexInId += drumKitBankIndex*drumKitBank.NrOfPatches;
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
                                               (new Regex("Drumkit[0-9]*").IsMatch(Name)));
    }
}
