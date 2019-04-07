using System.Collections.Generic;
using System.Linq;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Tools
{
    /// <summary>
    /// Parses rules.
    /// </summary>
    public class RuleParser
    {

        /// <summary>
        /// 
        /// </summary>
        private readonly IPcgMemory _memory;


        /// <summary>
        /// Parse has been ok.
        /// </summary>
        public bool HasParsedOk { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        public RuleParser(IPcgMemory memory)
        {
            _memory = memory;
            _parsedRules = new Dictionary<IPatch, IPatch>();
        }


        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<IPatch, IPatch> _parsedRules;


        /// <summary>
        /// 
        /// </summary>
        public Dictionary<IPatch, IPatch> ParsedRules => _parsedRules;


        /// <summary>
        /// Only used if Parse returns false.
        /// </summary>
        public int ParseErrorInLine { get; private set; }
        

        /// <summary>
        /// Sets _parsedRules, HasParsedOk and ParseErrorInLine (if !HasParsedOk).
        /// </summary>
        public void Parse(string rules)
        {
            var lines = rules.Split('\n');
            var lineNumber = 0;
            foreach (var trimmedLine2 in from line in lines select line.Trim() 
                                             into trimmedLine select trimmedLine.Replace("->", ">") 
                                             into trimmedLine1 select trimmedLine1.Replace("=>", ">"))
            {
                ParseLine(trimmedLine2, ref lineNumber);
                if (!HasParsedOk)
                {
                    return;
                }
            }
        }


        /// <summary>
        /// Parse single line.
        /// </summary>
        /// <param name="trimmedLine"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        private void ParseLine(string trimmedLine, ref int lineNumber)
        {
            HasParsedOk = false;

            if (trimmedLine == string.Empty)
            {
                // Empty line.
                HasParsedOk = true;
                return;
            }

            ParseErrorInLine = lineNumber;
            if (!trimmedLine.Contains(">"))
            {
                return;
            }

            var parts = trimmedLine.Split('>');
            if ((parts.Count() != 2) || parts.Any(part => part.Length == 0))
            {
                return;
            }

            var parser = new ProgramPatchParser(_memory);
            var fromPatches = parser.Parse(parts[0]);
            if (fromPatches == null)
            {
                return;
            }

            var toPatches = parser.Parse(parts[1], fromPatches);
            if (toPatches == null)
            {
                return;
            }

            if (fromPatches.Count != toPatches.Count)
            {
                HasParsedOk = false;
                return;
            }
            
            for (var index = 0; index < fromPatches.Count; index++)
            {
                _parsedRules[fromPatches[index]] = toPatches[index];
            }

            lineNumber++;
            HasParsedOk = true;
        }
    }
}
