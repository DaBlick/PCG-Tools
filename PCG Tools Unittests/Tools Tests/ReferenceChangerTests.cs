using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.Model.KronosSpecific.Pcg;
using PcgTools.Model.KronosSpecific.Synth;
using PcgTools.Tools;

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class ReferenceChangerTests
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ChangeCombi()
        {
            var pcg = CreatePcg();
            var programIa000 = (IProgram) pcg.ProgramBanks[0][0];
            var combiIa000 = (ICombi) (pcg.CombiBanks[0])[0];
            combiIa000.Name = "NonEmpty";

            combiIa000.Timbres.TimbresCollection[0].UsedProgram = programIa000;
            
            // Set most virtual banks loaded to save time.
            foreach (var bank in pcg.CombiBanks.BankCollection.Where(
                bank => (bank.Type == BankType.EType.Virtual) && (bank.Id !="V-0A")))
            {
                bank.IsLoaded = false;
            }

            // Run actual test.
            var referenceChanger = new ReferenceChanger(pcg);
            var ruleParser = new RuleParser(pcg);
            referenceChanger.ParseRules(ruleParser, "I-A000->I-B000");
            referenceChanger.ChangeReferences();

            Debug.Assert(combiIa000.Timbres.TimbresCollection[0].UsedProgram == pcg.ProgramBanks[1][0]);
        }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ChangeSetListSlot()
        {
            var pcg = CreatePcg();
            var programIa000 = (IProgram)pcg.ProgramBanks[0][0];
            var setListSlot000000 = (ISetListSlot)(pcg.SetLists[0])[0];
            setListSlot000000.SelectedPatchType = SetListSlot.PatchType.Program;
            setListSlot000000.UsedPatch = programIa000;

            var referenceChanger = new ReferenceChanger(pcg);
            var ruleParser = new RuleParser(pcg);
            referenceChanger.ParseRules(ruleParser, "I-A000->I-B000");
            referenceChanger.ChangeReferences();

            Debug.Assert(setListSlot000000.UsedPatch == pcg.ProgramBanks[1][0]);
        }


        /// <summary>
        /// Tests not too many patches will be changed.
        /// </summary>
        [TestMethod]
        public void ChangeSetListSlotNotTooMany()
        {
            var pcg = CreatePcg();

            var programIa000 = (IProgram)pcg.ProgramBanks[0][0];
            var setListSlot000000 = (ISetListSlot)(pcg.SetLists[0])[0];
            setListSlot000000.SelectedPatchType = SetListSlot.PatchType.Program;
            setListSlot000000.UsedPatch = programIa000;

            var programIa001 = (IProgram)pcg.ProgramBanks[0][1];
            var setListSlot000001 = (ISetListSlot)(pcg.SetLists[0])[1];
            setListSlot000001.SelectedPatchType = SetListSlot.PatchType.Program;
            setListSlot000001.UsedPatch = programIa001;

            var referenceChanger = new ReferenceChanger(pcg);
            var ruleParser = new RuleParser(pcg);
            referenceChanger.ParseRules(ruleParser, "I-A000->I-B000");
            referenceChanger.ChangeReferences();

            Debug.Assert(setListSlot000000.UsedPatch == pcg.ProgramBanks[1][0]); // Changed
            Debug.Assert(setListSlot000001.UsedPatch == programIa001); // Not changed
        }


        /// <summary>
        /// Creates PCG memory.
        /// Byte offsets are non real.
        /// </summary>
        /// <returns></returns>
        private static IPcgMemory CreatePcg()
        {
            IPcgMemory memory = new KronosPcgMemory("test.pcg");
            memory.Model = new Model(Models.EModelType.Kronos, Models.EOsVersion.EOsVersionKronos3x, "3.0");
            memory.Content = new byte[10000000]; // Enough for timbre parameters

            var byteOffset = 1000;
            
            memory.ProgramBanks = new KronosProgramBanks(memory);
            memory.ProgramBanks.Fill();

            foreach (var bank in memory.ProgramBanks.BankCollection)
            {
                bank.IsLoaded = true;

                foreach (var patch in bank.Patches)
                {
                    patch.ByteOffset = byteOffset;
                    byteOffset += 100;
                }
            }
            
            memory.CombiBanks = new KronosCombiBanks(memory);
            memory.CombiBanks.Fill();

            foreach (var bank in memory.CombiBanks.BankCollection)
            {
                bank.IsLoaded = true;

                foreach (var patch in bank.Patches)
                {
                    patch.ByteOffset = byteOffset;
                    byteOffset += 100;
                }
            }

            memory.SetLists = new KronosSetLists(memory);
            memory.SetLists.Fill();
            
            foreach (var bank in memory.SetLists.BankCollection)
            {
                bank.IsLoaded = true;

                foreach (var patch in bank.Patches)
                {
                    patch.ByteOffset = byteOffset;
                    byteOffset += 100;
                }
            }

            memory.DrumKitBanks = new KronosDrumKitBanks(memory);

            memory.DrumPatternBanks = new KronosDrumPatternBanks(memory);

            memory.WaveSequenceBanks = new KronosWaveSequenceBanks(memory);

            memory.Global = new KronosGlobal(memory);

            
            return memory;
        }
    }
}
