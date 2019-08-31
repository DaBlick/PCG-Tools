// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.PcgToolsResources;

namespace PcgTools.Model.KronosSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosSongMemory : SongMemory
    {
        /// <summary>
        /// 
        /// </summary>
        public Models.EOsVersion InitOsVersion { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public KronosSongMemory(string fileName)
            : base(fileName)
        {
            Model = Models.Find(Models.EOsVersion.EOsVersionKronos2x); // Songs are always considered Kronos 2.x files
        }


        /// <summary>
        /// Returns the program ID; only supported by Kronos.
        /// </summary>
        /// <param name="rawBankIndex"></param>
        /// <param name="rawProgramIndex"></param>
        /// <returns></returns>
        public override string ProgramIdByIndex(int rawBankIndex, int rawProgramIndex)
        {
            var names = new[]
            {
                // 0     1      2      3      4      5    6      7       8       9      10      11      12      13 
                "I-A", "I-B", "I-C", "I-D", "I-E", "I-F", "GM", "g(1)", "g(2)", "g(3)", "g(4)", "g(5)", "g(6)", "g(7)",

                // 14     15      16
                "g(8)", "g(9)", "g(d)",

                // 17   18     19     20     21     22     23      24      25      26      27      28      29      30
                "U-A", "U-B", "U-C", "U-D", "U-E", "U-F", "U-G", "U-AA", "U-BB", "U-CC", "U-DD", "U-EE", "U-FF", "U-GG"
            };

            var bankId = (rawBankIndex >= 0) && (rawBankIndex < names.Length) ? names[rawBankIndex] : Strings.Unknown;
            var indexId = (rawProgramIndex >= 6 && rawProgramIndex <= 16) ? rawProgramIndex + 1 : rawProgramIndex;
            return $"{bankId}{indexId:000}";
        }
    }
}
