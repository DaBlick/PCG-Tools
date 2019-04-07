using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;

namespace PcgTools.Tools
{
    /// <summary>
    /// 
   /// </summary>
    public class ReferenceChanger
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IPcgMemory _memory;

        private RuleParser _ruleParser;

        // Processed set list slots/timbres
        private List<ISetListSlot> _processedSetListSlots;
        private List<ITimbre> _processedTimbres;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        public ReferenceChanger(IPcgMemory memory)
        {
            _memory = memory;
            _processedSetListSlots = new List<ISetListSlot>();
            _processedTimbres = new List<ITimbre>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public  void ParseRules(RuleParser ruleParser, string rules)
        {
            _ruleParser = ruleParser;
            _ruleParser.Parse(rules);
        }


        /// <summary>
        /// Change references. Should be called after ParseRules.
        /// </summary>
        public void ChangeReferences()
        {
            if (_ruleParser.HasParsedOk)
            {
                ChangeReferences(_ruleParser.ParsedRules);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="changes"></param>
        void ChangeReferences(ICollection<KeyValuePair<IPatch, IPatch>> changes)
        {
            var ruleNumber = 0;
            var currentPercentage = 0;
            foreach (var rule in changes)
            {
                ruleNumber++;
                var newPercentage = (int) ((ruleNumber*100.0)/changes.Count);
                if (currentPercentage != newPercentage)
                {
                    OnProgress(newPercentage);
                    currentPercentage = newPercentage;
                }

                ChangeReferencesInSetListSlots(rule);

                ChangeReferencesInCombis(rule);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        private void ChangeReferencesInSetListSlots(KeyValuePair<IPatch, IPatch> rule)
        {
            foreach (var setList in _memory.SetLists.BankCollection.Where(setList => setList.IsFilled))
            {
                for (var index = 0; index < setList.Patches.Count; index++)
                {
                    var setListSlot = (ISetListSlot) setList[index];
                    if ((setListSlot.SelectedPatchType == SetListSlot.PatchType.Program) &&
                        (setListSlot.UsedPatch == rule.Key) &&
                        !_processedSetListSlots.Contains(setListSlot))
                    {
                        setListSlot.UsedPatch = rule.Value;
                        _processedSetListSlots.Add(setListSlot);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        private void ChangeReferencesInCombis(KeyValuePair<IPatch, IPatch> rule)
        {
            foreach (var combi in from combiBank in _memory.CombiBanks.BankCollection 
                                  where combiBank.IsFilled from ICombi combi in combiBank.Patches select combi)
            {
                ChangeReferencesInTimbres(rule, combi);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="combi"></param>
        private void ChangeReferencesInTimbres(KeyValuePair<IPatch, IPatch> rule, ICombi combi)
        {
            foreach (var timbre in combi.Timbres.TimbresCollection.Where(timbre => timbre.UsedProgram == rule.Key))
            {
                if (((timbre.GetParam(ParameterNames.TimbreParameterName.Status).Value == "Off") ||
                     (timbre.GetParam(ParameterNames.TimbreParameterName.Status).Value == "Int")) &&
                    !_processedTimbres.Contains(timbre))
                {
                    timbre.UsedProgram = (IProgram)rule.Value;
                    _processedTimbres.Add(timbre);
                }
            }
        }

        
        /// <summary>
        /// Delegate for progress handler.
        /// </summary>
        /// <param name="args"></param>
        public delegate void ProgressMadeHandler(ProgressChangedEventArgs args);

        
        /// <summary>
        /// Event for progress.
        /// </summary>
        public event ProgressMadeHandler OnProgressHandler;


        /// <summary>
        /// Called when progress changed.
        /// </summary>
        /// <param name="percentage"></param>
        private void OnProgress(int percentage)
        {
            if (OnProgressHandler != null)
            {
                var args = new ProgressChangedEventArgs(percentage, null);
                OnProgressHandler(args);
            }
        }

        /*

        /// <summary>
        /// Delegate for parse complete.
        /// </summary>
        /// <param name="args"></param>
        public delegate void ParseCompleteHandler(RunWorkerCompletedEventArgs args);


        /// <summary>
        /// Event for progress.
        /// </summary>
        public event ParseCompleteHandler OnParseCompleteHandler;


        /// <summary>
        /// Called when progress changed.
        /// </summary>
        private void OnParseComplete()
        {
            if (OnParseCompleteHandler != null)
            {
                var args = new RunWorkerCompletedEventArgs(
                    _ruleParser.HasParsedOk ? -1 : _ruleParser.ParseErrorInLine, null, false);
                OnParseCompleteHandler(args);
            }
        }
        */
    }
}
