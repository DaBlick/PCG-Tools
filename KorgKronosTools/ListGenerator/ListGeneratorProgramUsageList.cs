// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Common.Extensions;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;

namespace PcgTools.ListGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public class ListGeneratorProgramUsageList : ListGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        Dictionary<Tuple<IProgramBank, IProgram>, LinkedList<IPatch>> _dict;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="useFileWriter"></param>
        /// <returns></returns>
        protected override string RunAfterFilteringBanks(bool useFileWriter = true)
        {
            using (var writer = File.CreateText(OutputFileName))
            {
                //var programDict = new Dictionary<Program, List<Combi>> ();
                _dict = new Dictionary<Tuple<IProgramBank, IProgram>, LinkedList<IPatch>>();

                // Create dictionary.
                CreateDictionary();

                FillDictionary();

                // Write to file.
                WriteToFile(writer);

                writer.Close();

                if (ListOutputFormat == OutputFormat.Xml)
                {
                    WriteXslFile();
                }
            }

            return OutputFileName;
        }

        
        /// <summary>
        /// 
        /// </summary>
        void CreateDictionary()
        {
            if (_dict == null) throw new ArgumentNullException();
            foreach (var programBank in SelectedProgramBanks)
            {
                var bank = programBank;
                foreach (var patch in (from program in programBank.Patches
                                         where !((IBank) (program.Parent)).IsLoaded || 
                                         (((IBank) (program.Parent)).IsLoaded && (!IgnoreInitPrograms || !program.IsEmptyOrInit))
                    select program).Where(program => !IgnoreFirstProgram ||
                        (PcgMemory.ProgramBanks.BankCollection.IndexOf(bank) != 0) || 
                        (bank.Patches.IndexOf(program) != 0)).Where(
                        program => (!PcgMemory.AreFavoritesSupported ||
                            ((((ProgramBank)(program.Parent)).Type == BankType.EType.Gm) && 
                            (ListFilterOnFavorites != FilterOnFavorites.Yes)) ||
                            ((IProgram) program).GetParam(ParameterNames.ProgramParameterName.Favorite) == null) ||
                            (ListFilterOnFavorites == FilterOnFavorites.All) ||
                            (((ListFilterOnFavorites == FilterOnFavorites.No) && 
                            !((IProgram)program).GetParam(ParameterNames.ProgramParameterName.Favorite).Value) || 
                            ((ListFilterOnFavorites == FilterOnFavorites.Yes) && 
                            ((IProgram)program).GetParam(ParameterNames.ProgramParameterName.Favorite).Value))))
                {
                    var program = (IProgram) patch;
                    _dict.Add(new Tuple<IProgramBank, IProgram>(programBank, program), new LinkedList<IPatch>());
                }
            }
        }


        /// <summary>
        /// Fills combis. Ignore favorites for combis.
        /// </summary>
        void FillDictionary()
        {
            if (_dict == null) throw new ArgumentNullException();
            FillDictionaryWithCombis();

            if ((PcgMemory.SetLists == null) || !SetListsEnabled || !PcgMemory.SetLists.BankCollection.Any(setList => setList.IsFilled))
            {
                return;
            }

            FillDictionaryWithSetListSlots();
        }


        /// <summary>
        /// 
        /// </summary>
        private void FillDictionaryWithCombis()
        {
            foreach (var combiBank in SelectedCombiBanks)
            {
                foreach (var combi in combiBank.Patches)
                {
                    if (!combiBank.IsLoaded || (IgnoreInitCombis && combi.IsEmptyOrInit))
                    {
                        continue;
                    }

                    var combi1 = combi;
                    var bank = combiBank;
                    foreach (var key in from timbre in ((ICombi) combi).Timbres.TimbresCollection
                        where !bank.IsLoaded || (bank.IsLoaded && !IgnoreMutedOffTimbres ||
                                                      (((timbre.GetParam(ParameterNames.TimbreParameterName.Mute) == null) ||
                                                      !timbre.GetParam(ParameterNames.TimbreParameterName.Mute).Value) &&
                                                       (new List<string> {"Int", "On", "Both"}.Contains(
                                                           timbre.GetParam(ParameterNames.TimbreParameterName.Status).Value))))
                        select new Tuple<IProgramBank, IProgram>(timbre.UsedProgramBank, timbre.UsedProgram)
                        into key
                        where _dict.ContainsKey(key) && !_dict[key].Contains(combi1)
                        select key)
                    {
                        _dict[key].AddLast(combi);
                    }
                }
            }
        }

        private void FillDictionaryWithSetListSlots()
        {
            for (var setListIndex = 0; setListIndex < 128; setListIndex++)
            {
                if ((setListIndex < SetListsRangeFrom) ||
                    (setListIndex > SetListsRangeTo))
                {
                    continue;
                }

                var setList = ((ISetList) PcgMemory.SetLists[setListIndex]);
                foreach (var setListSlot in setList.Patches.Where(
                    setListSlot => setList.IsLoaded && !setListSlot.IsEmptyOrInit))
                {
                    switch (((ISetListSlot) setListSlot).SelectedPatchType)
                    {
                        case SetListSlot.PatchType.Program:
                        {
                            var usedProgramBank = ((ISetListSlot) setListSlot).UsedPatch.Parent as IProgramBank;
                            var key = new Tuple<IProgramBank, IProgram>(usedProgramBank,
                                ((ISetListSlot) setListSlot).UsedPatch as IProgram);

                            if (_dict.ContainsKey(key) && !_dict[key].Contains(setListSlot))
                            {
                                _dict[key].AddLast(setListSlot);
                            }
                        }
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void WriteToFile(TextWriter writer)
        {
            string columnText;
            var maxTimbresPerCombi = PrintHeader(writer, out columnText);

            PrintLines(writer, maxTimbresPerCombi, columnText);

            PrintFooter(writer, maxTimbresPerCombi, columnText);
        }


        /// <summary>
        /// Prints header, find maximum elements for a line (key).
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="columnText"></param>
        /// <returns></returns>
        private int PrintHeader(TextWriter writer, out string columnText)
        {
            var maxTimbresPerCombi = (from programBank in SelectedProgramBanks
                from program in programBank.Patches
                select
                    new Tuple<IProgramBank, IProgram>(programBank, (IProgram) program)
                into key
                where
                    _dict.ContainsKey(key) && (_dict[key].Count > 0)
                select key).Aggregate(
                    0, (current, key) => Math.Max(current, _dict[key].Count()));

            columnText = "Used in Combi and Set List Slot IDs"; // Only used for Ascii Table

            switch (ListOutputFormat)
            {
                case OutputFormat.AsciiTable:
                    writer.WriteLine("+--------+{0}+", new string('-', Math.Max(maxTimbresPerCombi*9, columnText.Length)));
                    writer.WriteLine("|PRG ID  |{0}{1}|", columnText, new string(
                        ' ',
                        Math.Max(0, maxTimbresPerCombi*9 - columnText.Length)));
                    writer.WriteLine(
                        "+--------+{0}+", new string('-', Math.Max(maxTimbresPerCombi*9, columnText.Length)));
                    break;

                case OutputFormat.Xml:
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    writer.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"{0}\"?>",
                        Path.ChangeExtension(OutputFileName, "xsl"));
                    writer.WriteLine("<program_usage_list name=\"Program usage list\" xml:lang=\"en\">");
                    break;

                    //default:
                    // No action required.
                    //break;

                    // ReSharper restore RedundantStringFormatCall
            }
            return maxTimbresPerCombi;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="maxTimbresPerCombi"></param>
        /// <param name="columnText"></param>
        private void PrintLines(TextWriter writer, int maxTimbresPerCombi, string columnText)
        {
            foreach (var programBank in SelectedProgramBanks)
            {
                foreach (var program in programBank.Patches)
                {
                    PrintLine(writer, maxTimbresPerCombi, columnText, programBank, program);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="maxTimbresPerCombi"></param>
        /// <param name="columnText"></param>
        /// <param name="programBank"></param>
        /// <param name="program"></param>
        private void PrintLine(TextWriter writer, int maxTimbresPerCombi, string columnText, IProgramBank programBank,
            IPatch program)
        {
            var key = new Tuple<IProgramBank, IProgram>(programBank, (Program) program);
            if (!_dict.ContainsKey(key) || (_dict[key].Count <= 0))
            {
                return;
            }

            switch (ListOutputFormat)
            {
                case OutputFormat.AsciiTable:
                    PrintAsciiTableLine(writer, maxTimbresPerCombi, columnText, program, key);
                    break;

                case OutputFormat.Text:
                    PrintTextLine(writer, program, key);
                    break;

                case OutputFormat.Csv:
                    PrintCsvLine(writer, program, key);
                    break;

                case OutputFormat.Xml:
                    PrintXmlLine(writer, program, key);
                    break;

                default:
                    throw new NotSupportedException("Unsupported output");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="maxTimbresPerCombi"></param>
        /// <param name="columnText"></param>
        /// <param name="program"></param>
        /// <param name="key"></param>
        private void PrintAsciiTableLine(TextWriter writer, int maxTimbresPerCombi, string columnText, IPatch program, 
            Tuple<IProgramBank, IProgram> key)
        {
            writer.Write("|{0,-8}|", program.Id);
            foreach (var item in _dict[key])
            {
                writer.Write("{0,-8} ", item.Id);
            }
// ReSharper disable RedundantStringFormatCall
            writer.WriteLine(
                $"{new string(' ', /* ReSharper restore RedundantStringFormatCall */ Math.Max((maxTimbresPerCombi - _dict[key].Count)*9, columnText.Length - _dict[key].Count*9))}|");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="program"></param>
        /// <param name="key"></param>
        private void PrintTextLine(TextWriter writer, IPatch program, Tuple<IProgramBank, IProgram> key)
        {
            writer.Write("{0,-8}: ", program.Id);
            foreach (var item in _dict[key])
            {
                writer.Write("{0,-8} ", item.Id);
            }
            writer.WriteLine();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="program"></param>
        /// <param name="key"></param>
        private void PrintCsvLine(TextWriter writer, IPatch program, Tuple<IProgramBank, IProgram> key)
        {
            writer.Write("{0},", program.Id);
            foreach (var item in _dict[key])
            {
                writer.Write("{0},", item.Id);
            }
            writer.WriteLine();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="program"></param>
        /// <param name="key"></param>
        private void PrintXmlLine(TextWriter writer, IPatch program, Tuple<IProgramBank, IProgram> key)
        {
            writer.WriteLine("  <program>");
            writer.WriteLine("    <id>{0}</id>", program.Id);
            writer.WriteLine("    <patches>");

            foreach (var item in _dict[key])
            {
                writer.WriteLine("      <patch>");
                writer.WriteLine("        <type>{0}</type>", item is Combi ? "Combi" : "Set List Slot");
                writer.WriteLine("        <id>{0}</id>", item.Id.ConvertToXml());
                writer.WriteLine("      </patch>");
            }

            writer.WriteLine("    </patches>");
            writer.WriteLine("  </program>");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="maxTimbresPerCombi"></param>
        /// <param name="columnText"></param>
        private void PrintFooter(TextWriter writer, int maxTimbresPerCombi, string columnText)
        {
            switch (ListOutputFormat)
            {
                case OutputFormat.AsciiTable:
                    writer.WriteLine("+--------+{0}+",
                        new string('-', Math.Max(maxTimbresPerCombi*9, columnText.Length)));
                    break;

                case OutputFormat.Xml:
                    writer.WriteLine("</program_usage_list>");
                    break;

                    //default:
                    // No action required.
                    //break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void WriteXslFile()
        {
            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\"?>");
            builder.AppendLine(" <xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">");
            builder.AppendLine(string.Empty);
            builder.AppendLine(" <xsl:template match=\"/\">");
            builder.AppendLine("   <html>");
            builder.AppendLine("   <body>");
            builder.AppendLine("     <h2>Program Usage List</h2>");
            builder.AppendLine("     <table border=\"1\">");
            builder.AppendLine("       <tr bgcolor=\"#80a0ff\">");
            builder.AppendLine("         <th>Program ID</th>");
            builder.AppendLine("         <th align=\"left\">Used in</th>");
            builder.AppendLine("       </tr>");
            builder.AppendLine("       <xsl:for-each select=\"program_usage_list/program\">");
            builder.AppendLine("         <tr>");
            builder.AppendLine("           <td><xsl:value-of select=\"id\"/></td>");
            builder.AppendLine("           <td>");
            builder.AppendLine("             <table>");
            builder.AppendLine("               <tr>");
            builder.AppendLine("                 <xsl:for-each select=\"patches/patch\">");
            builder.AppendLine("                   <td><xsl:value-of select=\"id\"/></td>");
            builder.AppendLine("                 </xsl:for-each>");
            builder.AppendLine("               </tr>");
            builder.AppendLine("             </table>");
            builder.AppendLine("           </td>");
            builder.AppendLine("         </tr>");
            builder.AppendLine("       </xsl:for-each>");
            builder.AppendLine("     </table>");
            builder.AppendLine("   </body>");
            builder.AppendLine("   </html>");
            builder.AppendLine(" </xsl:template>");
            builder.AppendLine(string.Empty);
            builder.AppendLine(" </xsl:stylesheet>");
            File.WriteAllText(Path.ChangeExtension(OutputFileName, "xsl"), builder.ToString());
        }
    }
}
