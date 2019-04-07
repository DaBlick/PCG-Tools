using PcgTools.Model.Common.Synth.PatchCombis;

namespace PcgTools.Model.Common.Synth.SongsRelated
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISongTimbre : ITimbre
    {
        /// <summary>
        /// Returns the raw/uncoverted program index of the timbre.
        /// </summary>
        int ProgramRawIndex { get; }


        /// <summary>
        /// Returns the raw/uncoverted program bank index of the timbre.
        /// </summary>
        int ProgramRawBankIndex { get; }
    }
}
