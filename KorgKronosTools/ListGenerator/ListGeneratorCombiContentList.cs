// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using PcgTools.Common;

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.ListGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public class ListGeneratorCombiContentList : ListGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="useFileWriter"></param>
        /// <returns></returns>
        protected override string RunAfterFilteringBanks(bool useFileWriter = true)
        {
            var textFileName = OutputFileName;

            //var stream = new MemoryStream();
            //var writer = new StreamWriter(stream);
            var stream = useFileWriter ? null : new MemoryStream();
            using (var writer = useFileWriter ? File.CreateText(textFileName) : new StreamWriter(stream))
            {
                // Write to file.
                WriteToFile(writer);
                writer.Close();

                //IMPROVE: Use memory stream optionally for unit testing
                //var fs = new FileStream(textFileName, FileMode.OpenOrCreate);
                //fs.Write(stream.ToArray(), 0, (int) stream.Length);
                //fs.Close();
                //writer.Close();

                if (ListOutputFormat == OutputFormat.Xml)
                {
                    WriteXslFile();
                }
            }

            return textFileName;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void WriteToFile(TextWriter writer)
        {
            // Print header, assuming a 16 timbres per combi.
            // Do not print directly in foreach loop below, but store, calc max, then loop again and print

            if (ListSubType == SubType.Long)
            {
                WriteLongListToFile(writer);
            }
            else
            {
                WriteOtherTypeOfListToFile(writer);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void WriteOtherTypeOfListToFile(TextWriter writer)
        {
            const int maxTimbresPerCombi = 16; // Impr: Calculate real max timbres per line.

            WriteOtherTypeOfListHeaderToFile(writer, maxTimbresPerCombi);

            WriteOtherTypeOfListLineToFile(writer, maxTimbresPerCombi);

            WriteOtherTypeOfListFooterToFile(writer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="maxTimbresPerCombi"></param>
        private void WriteOtherTypeOfListHeaderToFile(TextWriter writer, int maxTimbresPerCombi)
        {
            switch (ListOutputFormat)
            {
                case OutputFormat.AsciiTable:
// ReSharper disable RedundantStringFormatCall
                    writer.WriteLine($"+------+{new string('-', maxTimbresPerCombi*8 - 1)}+");
// ReSharper restore RedundantStringFormatCall

                    var columnText = "Used Program IDs";
                    if (ListSubType == SubType.Compact)
                    {
                        columnText += " (sorted by bank/index and duplicates removed)";
                    }

// ReSharper disable RedundantStringFormatCall
                    writer.WriteLine(
                        $"|Combi |{columnText}{new string(' ', maxTimbresPerCombi*8 - columnText.Count() - 1)}|");
// ReSharper restore RedundantStringFormatCall
                    writer.WriteLine(
                        ListSubType == SubType.Compact
                            ? "| ID   | Timbres                                                                                                                       |"
                            : "| ID   | Tim 1 | Tim 2 | Tim 3 | Tim 4 | Tim 5 | Tim 6 | Tim 7 | Tim 8 | Tim 9 |Tim 10 |Tim 11 |Tim 12 |Tim 13 |Tim 14 |Tim 15 |Tim 16 |");
                    writer.WriteLine(
                        "+------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+");
                    break;

                case OutputFormat.Xml:
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
// ReSharper disable RedundantStringFormatCall
                    writer.WriteLine(
                        $"<?xml-stylesheet type=\"text/xsl\" href=\"{Path.ChangeExtension(OutputFileName, "xsl")}\"?>");
// ReSharper restore RedundantStringFormatCall
                    writer.WriteLine("<combi_content_list> xml:lang=\"en\">");
                    break;

                    // default:
                    // No action required.
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="maxTimbresPerCombi"></param>
        private void WriteOtherTypeOfListLineToFile(TextWriter writer, int maxTimbresPerCombi)
        {
// Print lines.
            foreach (var combi in from combiBank in SelectedCombiBanks
                from combi in combiBank.Patches
                where combiBank.IsLoaded &&
                      combi.UseInList(IgnoreInitCombis, FilterOnText, FilterText, FilterCaseSensitive,
                          ListFilterOnFavorites, false)
                select combi)
            {
                var programIds = new LinkedList<string>();

                // For the compact list, sort and make IDs unique.
                if (ListSubType == SubType.Compact)
                {
                    WriteOtherTypeOfCompactListLineToFile(combi, programIds);
                }
                else
                {
                    var usedPrograms = ((Combi) combi).Timbres.TimbresCollection.Select(
                        timbre => ShowTimbre(timbre) ? timbre.UsedProgram : null).ToList();

                    foreach (var program in usedPrograms)
                    {
                        if ((program == null) || !SelectedProgramBanks.Contains(program.Parent))
                        {
                            // Can only occuring for short sub type list.
                            programIds.AddLast("      ");
                        }
                        else
                        {
                            programIds.AddLast(program.Id);
                        }
                    }
                }
                WriteLineToFile(writer, combi, programIds, maxTimbresPerCombi);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        /// <param name="programIds"></param>
        private void WriteOtherTypeOfCompactListLineToFile(IPatch combi, LinkedList<string> programIds)
        {
            var usedPrograms = from timbre in ((Combi) combi).Timbres.TimbresCollection
                where ShowTimbre(timbre)
                select timbre.UsedProgram;

            var unorderedProgramIds =
                (from program in usedPrograms
                    where ((program != null) &&
                           SelectedProgramBanks.Contains(program.Parent))
                    select program.Id).ToList();
            unorderedProgramIds.Sort();

            for (var n = 0; n < unorderedProgramIds.Count; n++)
            {
                if (n == 0)
                {
                    programIds.AddLast(unorderedProgramIds[n]);
                }
                else if (unorderedProgramIds[n - 1] != unorderedProgramIds[n])
                {
                    programIds.AddLast(unorderedProgramIds[n]);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void WriteOtherTypeOfListFooterToFile(TextWriter writer)
        {
// Print footer.
            switch (ListOutputFormat)
            {
                case OutputFormat.AsciiTable:
                    writer.WriteLine(
                        "+------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+-------+");
                    break;

                case OutputFormat.Xml:
                    writer.WriteLine("</combi_content_list>");
                    break;

                    //default:
                    // No action required.
                    //break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbre"></param>
        /// <returns></returns>
        bool ShowTimbre(ITimbre timbre)
        {
            return (!IgnoreMutedOffTimbres ||
                ((timbre.GetParam(ParameterNames.TimbreParameterName.Mute) == null) || 
                (!timbre.GetParam(ParameterNames.TimbreParameterName.Mute).Value) &&
                    new List<string> {"Int", "On", "Both"}.Contains(timbre.GetParam(ParameterNames.TimbreParameterName.Status).Value))) &&
                        (!IgnoreFirstProgram ||
                            ((PcgMemory.ProgramBanks.BankCollection.IndexOf(timbre.UsedProgramBank) != 0) &&
                                (timbre.UsedProgramBank.Patches.IndexOf(timbre.UsedProgram) != 0))) &&
                                (SelectedProgramBanks.Contains(timbre.UsedProgramBank));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="combi"></param>
        /// <param name="programIds"></param>
        /// <param name="maxTimbresPerCombi"></param>
        void WriteLineToFile(TextWriter writer, IPatch combi, IEnumerable<string> programIds, 
            int maxTimbresPerCombi) // IEnumerable<Program> programs)
        {
            switch (ListOutputFormat)
            {
                case OutputFormat.AsciiTable:
                    WriteLineToAsciiTableFile(writer, combi, programIds, maxTimbresPerCombi);
                    break;

                case OutputFormat.Text:
                    WriteLineToTextFile(writer, combi, programIds);
                    break;

                case OutputFormat.Csv:
                    WriteLineToCsvFile(writer, combi, programIds);
                    break;

                case OutputFormat.Xml:
                    WriteLineToXmlFile(writer, combi, programIds);
                    break;

                default:
                    throw new NotSupportedException("Unsupported output");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="combi"></param>
        /// <param name="programIds"></param>
        /// <param name="maxTimbresPerCombi"></param>
        private static void WriteLineToAsciiTableFile(TextWriter writer, IPatch combi, IEnumerable<string> programIds,
            int maxTimbresPerCombi)
        {
            writer.Write("|{0,-6}", combi.Id);
            var enumerable = programIds as IList<string> ?? programIds.ToList();
            foreach (var programId in enumerable)
            {
                writer.Write("|{0,-7}", programId);
            }

            for (var n = enumerable.Count(); n < maxTimbresPerCombi; n++)
            {
                writer.Write("|       ");
            }
            writer.WriteLine("|");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="combi"></param>
        /// <param name="programIds"></param>
        private static void WriteLineToTextFile(TextWriter writer, IPatch combi, IEnumerable<string> programIds)
        {
            writer.Write("{0,-6}: ", combi.Id);
            foreach (var programId in programIds)
            {
                writer.Write("{0,-7} ", programId);
            }
            writer.WriteLine();
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="combi"></param>
        /// <param name="programIds"></param>
        private static void WriteLineToCsvFile(TextWriter writer, IPatch combi, IEnumerable<string> programIds)
        {
            writer.Write("{0},", combi.Id);
            foreach (var programId in programIds)
            {
                writer.Write("{0},", programId);
            }
            writer.WriteLine();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="combi"></param>
        /// <param name="programIds"></param>
        private static void WriteLineToXmlFile(TextWriter writer, IPatch combi, IEnumerable<string> programIds)
        {
            writer.WriteLine("  <combi>");
            writer.WriteLine("    <id>{0}</id>", combi.Id);
            writer.WriteLine("    <timbres>");
            foreach (var programId in programIds)
            {
                writer.WriteLine("      <program>");
                writer.WriteLine("        <id>{0}</id>", programId);
                writer.WriteLine("      </program>");
            }
            writer.WriteLine("    </timbres>");
            writer.WriteLine("  </combi>");
        }


        /// <summary>
        /// 
        /// </summary>
        void WriteXslFile()
        {
            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\"?>");
            builder.AppendLine(" <xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">");
            builder.AppendLine(" <xsl:template match=\"/\">");
            builder.AppendLine("   <html>");
            builder.AppendLine("   <body>");
            builder.AppendLine("     <h2>Combi Content List</h2>");
            builder.AppendLine("     <table border=\"1\">");
            builder.AppendLine("       <tr bgcolor=\"#80a0ff\">");
            builder.AppendLine("         <th>Combi</th>");
            builder.AppendLine("         <th align=\"left\">Used in</th>");
            builder.AppendLine("       </tr>");
            builder.AppendLine("       <xsl:for-each select=\"combi_content_list/combi\">");
            builder.AppendLine("         <tr>");
            builder.AppendLine("           <td><xsl:value-of select=\"id\"/></td>");
            builder.AppendLine("           <table border=\"1\">");
            builder.AppendLine("             <tr>");
            builder.AppendLine("               <xsl:for-each select=\"timbres/program\">");
            builder.AppendLine("                 <td><xsl:value-of select=\"id\"/></td>");
            builder.AppendLine("               </xsl:for-each>");
            builder.AppendLine("             </tr>");
            builder.AppendLine("           </table>");
            builder.AppendLine("         </tr>");
            builder.AppendLine("       </xsl:for-each>");
            builder.AppendLine("     </table>");
            builder.AppendLine("   </body>");
            builder.AppendLine("   </html>");
            builder.AppendLine(" </xsl:template>");
            builder.AppendLine(" </xsl:stylesheet>");
            File.WriteAllText(Path.ChangeExtension(OutputFileName, "xsl"), builder.ToString());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void WriteLongListToFile(TextWriter writer)
        {
            foreach (var patch in from combiBank in SelectedCombiBanks
              from combi in combiBank.Patches
                                    where combiBank.IsLoaded && 
                                    combi.UseInList(IgnoreInitCombis, FilterOnText, FilterText, FilterCaseSensitive,
                                    ListFilterOnFavorites, false)
              select combi)
            {
                var combi = (ICombi) patch;
                // Gather info for header.
                var categoryString = combi.CategoryAsName;

                //var subCategory = combi.GetParam("SubCategory");
                var subCategoryString = combi.SubCategoryAsName;
                var favorite = combi.PcgRoot.AreFavoritesSupported ? combi.GetParam(ParameterNames.CombiParameterName.Favorite) : null;
                var favoriteString = (favorite == null) ? "-" : (favorite.Value) ? "Yes" : "No";
                var paramTempo = combi.GetParam(ParameterNames.CombiParameterName.Tempo);
                var tempo = paramTempo == null ? "-" : string.Format("{0,6:0.00}", paramTempo.Value);

                // Print header.
                writer.WriteLine("+------------+-----------------------------+----------------------------+--------------------------------+------------+-------+---------------------------------+");
// ReSharper disable RedundantStringFormatCall
                writer.WriteLine(string.Format(new CultureInfo("en-US"), 
// ReSharper restore RedundantStringFormatCall
                    "|Combi {0,-6}|Name:{1,-24}|Cat:{2,-24}|Sub Cat:{3,-24}|Tempo:{4,-6}|Fav:{5,-3}|                                 |", 
                    combi.Id, combi.Name, categoryString, subCategoryString, tempo, favoriteString));
                writer.WriteLine("+------------+-----------------------------+----------------------------+--------------------------------+------------+-------+---------------------------------+");
                writer.WriteLine("|Timbres List                                                                                                                                                   |");
                writer.WriteLine("+---+----------+------------------------+------------------------+------------------------+---+---+----+----+----+---------+-------+----+----+---+-----+---+----+");
                writer.WriteLine("|Tim|Program   |Name of the program     |Category                |Sub Category            |Vol|Sta|Mute|Prio|MIDI|Key Zone |Veloc. |OSC |OSC |Tra| De- |Por|Bend|");
                writer.WriteLine("|bre|          |                        |                        |                        |ume|tus|    |rity|Ch. |         |Zone   |Mode|Sel.|nsp|tune |tam|Rng.|");
                writer.WriteLine("+---+----------+------------------------+------------------------+------------------------+---+---+----+----+----+---------+-------+----+----+---+-----+---+----+");

                // Print timbres.);
                for (var index = 0; index < combi.Timbres.TimbresCollection.Count; index++)
                {
                    PrintTimbre(writer, combi, index);
                }

                // Print footer.
                writer.WriteLine("+---+----------+------------------------+------------------------+------------------------+---+---+----+----+----+---------+-------+----+----+---+-----+---+----+\r\n");
            }
        }


        /// <summary>
        /// IMPR: Reduce complexity by creating a 'result' structure and split method in several parts.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="combi"></param>
        /// <param name="index"></param>
        private void PrintTimbre(TextWriter writer, ICombi combi, int index)
        {
            var timbre = combi.Timbres.TimbresCollection[index];
            var status = timbre.GetParam(ParameterNames.TimbreParameterName.Status).Value;
            var muteParam = timbre.GetParam(ParameterNames.TimbreParameterName.Mute);
            var mute = muteParam != null && (bool) muteParam.Value;
            var muteString = mute.ToYesNo();

            if (!ShowTimbre(timbre))
            {
                return;
            }

            var usedProgram = timbre.UsedProgram;
            var timbreId = (usedProgram == null) ? "???" : timbre.ColumnProgramId;

            var isGmProgram = (usedProgram != null) && ((ProgramBank) (timbre.UsedProgram.Parent)).Type == BankType.EType.Gm;
            var byteOffset = (usedProgram == null) ? 0 : timbre.UsedProgram.ByteOffset;
            var name = isGmProgram ? "-" : (byteOffset == 0) ? "???" : timbre.ColumnProgramName;
            var category = isGmProgram ? "-" : (byteOffset == 0) ? "???" : timbre.UsedProgram.CategoryAsName;
            var subCategory = isGmProgram ? "-" : ((byteOffset == 0) ? "???" : timbre.UsedProgram.SubCategoryAsName);
            var volume = (string) (timbre.GetParam(ParameterNames.TimbreParameterName.Volume).Value.ToString());
            var priority = timbre.GetParam(ParameterNames.TimbreParameterName.Priority);
            var priorityString = (isGmProgram || (priority == null))
                ? "No"
                : ((byteOffset == 0) ? "???" : ((bool) priority.Value).ToYesNo());

            var midiChannelString = ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.MidiChannel, 
                (int) timbre.GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value);

            var bottomKey = (string) ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.BottomKey, 
                timbre.GetParam(ParameterNames.TimbreParameterName.BottomKey).Value);

            var topKey = (string)ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.TopKey, 
                timbre.GetParam(ParameterNames.TimbreParameterName.TopKey).Value);

            var bottomVelocity = (string) timbre.GetParam(ParameterNames.TimbreParameterName.BottomVelocity).Value.ToString();
            var topVelocity = (string) timbre.GetParam(ParameterNames.TimbreParameterName.TopVelocity).Value.ToString();

            var paramOscMode = timbre.GetParam(ParameterNames.TimbreParameterName.OscMode);
            var oscMode = (paramOscMode == null) ? "-" : (string) paramOscMode.Value;

            var paramOscSelect = timbre.GetParam(ParameterNames.TimbreParameterName.OscSelect);
            var oscSelect = (paramOscSelect == null) ? "-" : (string) paramOscSelect.Value;

            var transpose = (string) ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.Transpose, 
                timbre.GetParam(ParameterNames.TimbreParameterName.Transpose).Value);

            var paramDetune = timbre.GetParam(ParameterNames.TimbreParameterName.Detune);
            var detune = (paramDetune == null) ? "-" : (string) ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.Detune, paramDetune.Value);

            var paramPortamento = timbre.GetParam(ParameterNames.TimbreParameterName.Portamento);
            var portamento = (paramPortamento == null)
                ? "-"
                : (string) ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.Portamento, paramPortamento.Value);

            var bendRange = (string) ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.BendRange,
                timbre.GetParam(ParameterNames.TimbreParameterName.BendRange).Value);

            writer.Write(
                $"|{index + 1,2} |{timbreId,-10}|{name,-24}|{category,-24}|{subCategory,-24}|{volume,3}|{status,-3}|{muteString,-4}|{priorityString,-4}|{midiChannelString,-4}|");

// ReSharper disable RedundantStringFormatCall
            writer.WriteLine(
                $"{bottomKey,-4}~{topKey,-4}|{bottomVelocity,3}~{topVelocity,3}|{oscMode,-4}|{oscSelect,-4}|{transpose,3}|{detune,5}|{portamento,3}|{bendRange,-3} |");
        }
    }
}
