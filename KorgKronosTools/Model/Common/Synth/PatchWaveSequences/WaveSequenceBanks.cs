// (c) 2011 Michel Keijzers

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.PatchWaveSequences
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class WaveSequenceBanks : Banks<IBank>, IWaveSequenceBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        protected WaveSequenceBanks(IPcgMemory pcgMemory) : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract void CreateBanks();
        

        /// <summary>
        /// 
        /// </summary>
        public override void Fill()
        {
            CreateBanks();
            FillWaveSequences();
        }
        

        /// <summary>
        /// 
        /// </summary>
        void FillWaveSequences()
        {
            foreach (var bank in BankCollection)
            {
                for (var index = 0; index < bank.NrOfPatches; index++)
                {
                    bank.CreatePatch(index);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string Name => "n.a.";


        /// <summary>
        /// 
        /// </summary>
        public int Wsq2PcgOffset { get; set; }
    }
}
