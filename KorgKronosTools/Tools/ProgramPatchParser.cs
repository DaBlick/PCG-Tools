using System;
using System.Collections.Generic;
using System.Linq;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class ProgramPatchParser
    {
        /// <summary>
        /// A part can be a 'From' or 'To' syntax.
        /// From syntax: 
        /// bank_name                               e.g. I-A
        /// bank_name start_index..end_index        e.g. I-A040..080
        /// bank_name index>                        e.g. I-A040
        /// 
        /// To syntax, same as From syntax but with addition:
        /// bank name start_index..                 e.g. I-A040..      (uses From patch to find end index)
        /// 
        /// </summary>

        private readonly IPcgMemory _memory;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        public ProgramPatchParser(IPcgMemory memory)
        {
            _memory = memory;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="part"></param>
        /// <param name="toPatches"></param>
        /// <returns></returns>
        public List<IPatch> Parse(string part, List<IPatch> toPatches = null)
        {
            part = part.Trim();

            int startIndex;
            int endIndex;
            if (!ParseIndices(ref part, out endIndex, out startIndex))
            {
                return null;
            }

            var bank = ParseBank(part);
            if (bank == null)
            {
                return null;
            }

            // Reduce the offset from a bank (e.g. bank GM starts with 1).
            if (startIndex != -1)
            {
                if (startIndex < bank.IndexOffset)
                {
                    startIndex -= bank.IndexOffset;
                }
            }

            if (endIndex != -1)
            {
                endIndex -= bank.IndexOffset;
            }

            // Now the bank is known, take the whole bank if both indices are -1.
            if ((startIndex == -1) && (endIndex == -1))
            {
                startIndex = 0;
                endIndex = bank.Patches.Count - 1;
            }

            var list = new List<IPatch>();
            for (var index = startIndex; index <= endIndex; index++)
            {
                list.Add(bank[index]);
            }
            
            return list;
        }


        /// <summary>
        /// Strip bank (until last three digits).
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private IProgramBank ParseBank(string part)
        {
            return _memory.ProgramBanks.BankCollection.FirstOrDefault(bankToCheck => bankToCheck.Id == part) as IProgramBank;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="part"></param>
        /// <param name="endIndex"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private static bool ParseIndices(ref string part, out int endIndex, out int startIndex)
        {
            startIndex = -1;
            if (Int32.TryParse(part.Substring(part.Length - 3), out endIndex))
            {
                part = part.Substring(0, part.Length - 3);
            }
            else
            {
                endIndex = -1;
            }

            if (part.Contains(".."))
            {
                // Format I-A040..
                if (!part.EndsWith(".."))
                {
                    return false;
                }

                part = part.Replace("..", "");
                if (!Int32.TryParse(part.Substring(part.Length - 3), out startIndex))
                {
                    return false;
                }

                part = part.Substring(0, part.Length - 3);
            }

            if (startIndex == -1)
            {
                startIndex = endIndex;
                // If both indices are -1, later set it the whole bank (when the bank is known).
            }

            return true;
        }
    }
}
