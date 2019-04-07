using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.KronosSpecific.Pcg;
using PcgTools.Model.KronosSpecific.Synth;
using PcgTools.Tools;

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class RuleParserTests
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void NoRules()
        {
            var pcg = CreatePcg();
            var ruleParser = new RuleParser(pcg);
            ruleParser.Parse("");
            Debug.Assert(ruleParser.HasParsedOk);
            Debug.Assert(ruleParser.ParsedRules.Count == 0);
        }

        
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void SingleRule()
        {
            var pcg = CreatePcg();
            var ruleParser = new RuleParser(pcg);
            ruleParser.Parse("I-A000->I-B000");
            Debug.Assert(ruleParser.HasParsedOk);
            
            Debug.Assert(ruleParser.ParsedRules.Count == 1);
            var rule = ruleParser.ParsedRules[pcg.ProgramBanks[0][0]]; // I-A000
            Debug.Assert(rule == pcg.ProgramBanks[1][0]); // I-B000
        }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TwoRules()
        {
            var pcg = CreatePcg();
            pcg.Content = new byte[10000];
            var ruleParser = new RuleParser(pcg);
            ruleParser.Parse("I-A000->I-B000\nI-A001->I-B001");
            Debug.Assert(ruleParser.HasParsedOk);

            Debug.Assert(ruleParser.ParsedRules.Count == 2);
            var rule = ruleParser.ParsedRules[pcg.ProgramBanks[0][0]]; // I-A000
            Debug.Assert(rule == pcg.ProgramBanks[1][0]); // I-B000
            rule = ruleParser.ParsedRules[pcg.ProgramBanks[0][1]]; // I-A001
            Debug.Assert(rule == pcg.ProgramBanks[1][1]); // I-B001
        }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void RangeRule()
        {
            var pcg = CreatePcg();
            var ruleParser = new RuleParser(pcg);
            ruleParser.Parse("I-A000..010->I-B000..010");
            Debug.Assert(ruleParser.HasParsedOk);

            Debug.Assert(ruleParser.ParsedRules.Count == 11);
            var rule = ruleParser.ParsedRules[pcg.ProgramBanks[0][10]]; // I-A010
            Debug.Assert(rule == pcg.ProgramBanks[1][10]); // I-B010
        }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void BankRule()
        {
            var pcg = CreatePcg();
            var ruleParser = new RuleParser(pcg);
            ruleParser.Parse("I-A->I-B");
            Debug.Assert(ruleParser.HasParsedOk);

            Debug.Assert(ruleParser.ParsedRules.Count == pcg.ProgramBanks[0].CountPatches);
            var rule = ruleParser.ParsedRules[pcg.ProgramBanks[0][127]]; // I-A127
            Debug.Assert(rule == pcg.ProgramBanks[1][127]); // I-B127
        }


        /// <summary>
        /// Creates PCG memory.
        /// </summary>
        /// <returns></returns>
        private static IPcgMemory CreatePcg()
        {
            IPcgMemory memory = new KronosPcgMemory("test.pcg");
            memory.ProgramBanks = new KronosProgramBanks(memory);
            memory.ProgramBanks.Fill();
            memory.CombiBanks = new KronosCombiBanks(memory);
            memory.DrumKitBanks = new KronosDrumKitBanks(memory);
            memory.DrumPatternBanks = new KronosDrumPatternBanks(memory);
            memory.WaveSequenceBanks = new KronosWaveSequenceBanks(memory);
            memory.Global = new KronosGlobal(memory);

            return memory;
        }
    }
}
