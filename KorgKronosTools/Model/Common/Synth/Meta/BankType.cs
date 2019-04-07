namespace PcgTools.Model.Common.Synth.Meta
{
    /// <summary>
    /// 
    /// </summary>
    public static class BankType
    {
        /// <summary>
        /// Used for programs, combis, set list slots etc.
        /// Gm is only used for programs.
        /// </summary>
        public enum EType
        {
            Int, // Internal banks like A.. F or I-A..I-F
            Gm, // GM (FUTURE: GM, g(1)..g(9), g(d)
            User, // User bank like H..N or I-A..I-G
            UserExtended, // User bank like U-AA..U-GG for Kronos update OS 1.5 and further.
            Virtual // Bank not available on real synth.
        }
    }
}
