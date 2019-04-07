// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using PcgTools.Model.Common;

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using PcgTools.PcgToolsResources;

namespace PcgTools.ListGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public class ListGeneratorFileContentList : ListGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        private LinkedList<IBank> _list;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="useFileWriter"></param>
        /// <returns></returns>
        protected override string RunAfterFilteringBanks(bool useFileWriter = true)
        {
            using (var writer = File.CreateText(OutputFileName))
            {
                _list = new LinkedList<IBank>();
                CreateFileContentList();

                //TestPatchIdsString(writer);
                WriteToFile(writer);
                writer.Close();

                WriteXslFile();
            }

            return OutputFileName;
        }


        /// <summary>
        /// 
        /// </summary>
        private void CreateFileContentList()
        {
            CreateFileContentListOfProgramBanks();
            CreateFileContentListOfCombiBanks();
            CreateFileContentListOfSetLists();
            CreateFileContentListOfWaveSequenceBanks();
            CreateFileContentListOfDrumKitBanks();
            CreateFileContentListOfDrumPatternBanks();
        }


        /// <summary>
        /// 
        /// </summary>
        private void CreateFileContentListOfProgramBanks()
        {
            if (PcgMemory.ProgramBanks != null)
            {
                foreach (var bank in PcgMemory.ProgramBanks.BankCollection.Where(
                    programBank => programBank.IsWritable))
                {
                    _list.AddLast(bank);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void CreateFileContentListOfCombiBanks()
        {
            if (PcgMemory.CombiBanks != null)
            {
                foreach (var bank in PcgMemory.CombiBanks.BankCollection.Where(
                    combiBank => combiBank.IsWritable))
                {
                    _list.AddLast(bank);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void CreateFileContentListOfSetLists()
        {
            if (PcgMemory.SetLists != null)
            {
                // Although a PCG contains either ALL or NO setlists, we decide to print only the setlists that are 
                // not empty.
                foreach (IBank bank in PcgMemory.SetLists.BankCollection.Where(
                    setList => setList.IsWritable && setList.IsLoaded &&
                               setList.Patches.Any(setListSlot => !setListSlot.IsEmptyOrInit)))
                {
                    _list.AddLast(bank);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void CreateFileContentListOfWaveSequenceBanks()
        {
            if (PcgMemory.WaveSequenceBanks != null)
            {
                foreach (
                    var bank in PcgMemory.WaveSequenceBanks.BankCollection.Where(
                    waveSeqBank => waveSeqBank.IsWritable))
                {
                    _list.AddLast(bank);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void CreateFileContentListOfDrumKitBanks()
        {
            if (PcgMemory.DrumKitBanks != null)
            {
                foreach (var bank in PcgMemory.DrumKitBanks.BankCollection.Where(
                    drumKitBank => drumKitBank.IsWritable))
                {
                    _list.AddLast(bank);
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        private void CreateFileContentListOfDrumPatternBanks()
        {
            if (PcgMemory.DrumPatternBanks != null)
            {
                foreach (var bank in PcgMemory.DrumPatternBanks.BankCollection.Where(
                    drumPatternBank => drumPatternBank.IsWritable))
                {
                    _list.AddLast(bank);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void WriteToFile(TextWriter writer)
        {
            var lines = new List<string>();
                // Only used for asci tables. First add all lines. Then find out the longest line and create
            // right table vertical line.

            switch (ListOutputFormat)
            {
                case OutputFormat.AsciiTable:
                    // Warning: Font is slightly proportional, do not remove spaces by sight.
                    lines.Add("+-----------+-----------------------+-------+----------+--------+-------+");
                    lines.Add("|Bank Type  |Content Type           |Bank ID|# Writable|# Filled|# Empty|Patch IDs of filled patches");
                    lines.Add("+-----------+-----------------------+-------+----------+--------+-------+");
                    break;

                case OutputFormat.Xml:
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
// ReSharper disable RedundantStringFormatCall
                    writer.WriteLine(
                        $"<?xml-stylesheet type=\"text/xsl\" href=\"{Path.ChangeExtension(OutputFileName, "xsl")}\"?>");
// ReSharper restore RedundantStringFormatCall
                    writer.WriteLine("<file_content_list xml:lang=\"en\">");
                    break;

                    //default:
                    // No action required.
                    //break;
            }

            ParseItems(writer, lines);


            switch (ListOutputFormat)
            {
                case OutputFormat.AsciiTable:
                    lines.Add("+-----------+-----------------------+-------+----------+--------+-------+");

                    CreateVerticalRightLine(lines);
                    // Write all lines.
                    foreach (var line in lines)
                    {
                        writer.WriteLine(line);
                    }
                    break;

                case OutputFormat.Xml:
                    writer.WriteLine("</file_content_list>");
                    break;

                    //default:
                    // No action required.
                    //break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="lines"></param>
        private void ParseItems(TextWriter writer, List<string> lines)
        {
            foreach (var bank in _list)
            {
                var bankType = string.Empty;
                var contentType = string.Empty;
                var bankId = string.Empty;
                var writablePatches = 0;
                var filledPatches = 0;
                var emptyPatches = 0;
                var filledPatchList = new List<IPatch>();

                var bank1 = bank as ProgramBank;
                if (bank1 != null)
                {
                    var programBank = bank1;

                    bankType = "ProgramBank";
                    bankId = programBank.Id;
                    contentType =
                        $"{ProgramBank.SynthesisTypeAsString(programBank.BankSynthesisType)} {Strings.Programs}";
                    writablePatches = programBank.Patches.Count(program => programBank.IsWritable);
                    filledPatches = programBank.CountFilledAndNonEmptyPatches;
                    emptyPatches = writablePatches - filledPatches;

                    filledPatchList.AddRange(
                        programBank.Patches.Where(program => programBank.IsLoaded && !program.IsEmptyOrInit));
                }
                else
                {
                    var combiBank1 = bank as CombiBank;
                    if (combiBank1 != null)
                    {
                        var combiBank = combiBank1;

                        bankType = "CombiBank";
                        bankId = combiBank.Id;
                        contentType = "Combis";
                        writablePatches = combiBank.Patches.Count(combi => combiBank.IsWritable);
                        filledPatches = combiBank.CountFilledPatches;
                        emptyPatches = writablePatches - filledPatches;

                        var filledCombis = combiBank.Patches.Where(combi => combiBank.IsLoaded && !combi.IsEmptyOrInit);
                        filledPatchList.AddRange(filledCombis);
                    }
                    else
                    {
                        var list = bank as SetList;
                        if (list != null)
                        {
                            var setList = list;

                            bankType = "SetList";
                            bankId = setList.Id;
                            contentType = "SetListSlots";
                            writablePatches = setList.Patches.Count(setListSlot => setList.IsWritable);
                            filledPatches = setList.CountFilledPatches;
                            emptyPatches = writablePatches - filledPatches;

                            filledPatchList.AddRange(setList.Patches.Where(
                                setListSlot => setList.IsLoaded && !setListSlot.IsEmptyOrInit));
                        }
                        else
                        {
                            var seqBank = bank as WaveSequenceBank;
                            if (seqBank != null)
                            {
                                var waveSeqBank = seqBank;

                                bankType = "WaveSeqBank";
                                bankId = waveSeqBank.Id;
                                contentType = "WaveSequences";
                                writablePatches = waveSeqBank.Patches.Count(waveSeq => waveSeqBank.IsWritable);
                                filledPatches = waveSeqBank.CountFilledPatches;
                                emptyPatches = writablePatches - filledPatches;

                                filledPatchList.AddRange(waveSeqBank.Patches.Where(
                                    waveSeq => waveSeqBank.IsLoaded && !waveSeq.IsEmptyOrInit));
                            }
                            else
                            {
                                var kitBank = bank as DrumKitBank;
                                if (kitBank != null)
                                {
                                    var drumKitBank = kitBank;

                                    bankType = "DrumKitBank";
                                    bankId = drumKitBank.Id;
                                    contentType = "DrumKits";
                                    writablePatches = drumKitBank.Patches.Count(drumKit => drumKitBank.IsWritable);
                                    filledPatches = drumKitBank.CountFilledPatches;
                                    emptyPatches = writablePatches - filledPatches;

                                    filledPatchList.AddRange(drumKitBank.Patches.Where(
                                        drumKit => drumKitBank.IsLoaded && !drumKit.IsEmptyOrInit));
                                }
                                else
                                {
                                    var patternBank = bank as DrumPatternBank;
                                    if (patternBank != null)
                                    {
                                        var drumPatternBank = patternBank;

                                        bankType = "DrumPatternBank";
                                        bankId = drumPatternBank.Id;
                                        contentType = "DrumPatterns";
                                        writablePatches = drumPatternBank.Patches.Count(
                                            drumPattern => drumPatternBank.IsWritable);
                                        filledPatches = drumPatternBank.CountFilledPatches;
                                        emptyPatches = writablePatches - filledPatches;

                                        filledPatchList.AddRange(drumPatternBank.Patches.Where(
                                            drumPattern => drumPatternBank.IsLoaded && !drumPattern.IsEmptyOrInit));
                                    }
                                    else
                                    {
                                        Debug.Fail("Error in switch");
                                    }
                                }
                            }
                        }
                    }
                }


                WriteItem(writer, lines, bankType, contentType, bankId, writablePatches, filledPatches, emptyPatches, filledPatchList);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="lines"></param>
        /// <param name="bankType"></param>
        /// <param name="contentType"></param>
        /// <param name="bankId"></param>
        /// <param name="writablePatches"></param>
        /// <param name="filledPatches"></param>
        /// <param name="emptyPatches"></param>
        /// <param name="filledPatchList"></param>
        private void WriteItem(TextWriter writer, ICollection<string> lines, string bankType, string contentType, string bankId,
            int writablePatches, int filledPatches, int emptyPatches, List<IPatch> filledPatchList)
        {
            switch (ListOutputFormat)
            {
                case OutputFormat.AsciiTable:
                    lines.Add(
                        $"|{bankType,-11}|{contentType,-23}| {bankId,-6}|{writablePatches,5}     |{filledPatches,5}   |{emptyPatches,5}  |{Util.GetPatchIdsString(filledPatchList)}");
                    break;

                case OutputFormat.Text:
                    writer.WriteLine("{0} {1} {2}: {3}/{4}/{5}: {6}",
                        bankType, contentType, bankId, writablePatches, filledPatches, emptyPatches,
                        Util.GetPatchIdsString(filledPatchList));
                    break;

                case OutputFormat.Csv:
                    writer.WriteLine("{0},{1},{2},{3},{4},{5},{6}",
                        bankType, contentType, bankId, writablePatches, filledPatches, emptyPatches,
                        Util.GetPatchIdsString(filledPatchList).Replace(",", string.Empty));
                    break;

                case OutputFormat.Xml:
                    writer.WriteLine("  <bank>");
                    writer.WriteLine("    <type>{0}</type>", bankType);
                    writer.WriteLine("    <content_type>{0}</content_type>", contentType);
                    writer.WriteLine("    <id>{0}</id>", bankId);
                    writer.WriteLine("    <nr_writable_patches>{0}</nr_writable_patches>", writablePatches);
                    writer.WriteLine("    <nr_filled_patches>{0}</nr_filled_patches>", filledPatches);
                    writer.WriteLine("    <nr_empty_patches>{0}</nr_empty_patches>", emptyPatches);
                    writer.WriteLine("    <patch_ids>{0}</patch_ids>", Util.GetPatchIdsString(filledPatchList));
                    writer.WriteLine("  </bank>");
                    break;

                default:
                    throw new NotSupportedException("Unsupported output");
            }
        }


        /// <summary>
        /// Create a vertical right line because now all line lengths are known.
        /// </summary>
        /// <param name="lines"></param>
        private void CreateVerticalRightLine(List<string> lines)
        {
            var maxLength = lines.Max(line => line.Length) + 1; // +1 for right line |

            lines[0] += new string('-', maxLength - lines[0].Length) + '+';
            lines[1] += new string(' ', maxLength - lines[1].Length) + '|';
            lines[2] += new string('-', maxLength - lines[2].Length) + '+';

            for (var index = 3; index < lines.Count - 1; index++)
            {
                lines[index] += new string(' ', maxLength - lines[index].Length) + '|';
            }

            lines[lines.Count - 1] += new string('-', maxLength - lines[lines.Count - 1].Length) + '+';
        }


        /// <summary>
        /// 
        /// </summary>
        private void WriteXslFile()
        {
            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\"?>");
            builder.AppendLine(" <xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">");
            builder.AppendLine(string.Empty);
            builder.AppendLine(" <xsl:template match=\"/\">");
            builder.AppendLine("   <html>");
            builder.AppendLine("   <body>");
            builder.AppendLine("     <h2>File Content List</h2>");
            builder.AppendLine("     <table border=\"1\">");
            builder.AppendLine("       <tr bgcolor=\"#80a0ff\">");
            builder.AppendLine("         <th>BankType</th>");
            builder.AppendLine("         <th>ContentType</th>");
            builder.AppendLine("         <th>BankId</th>");
            builder.AppendLine("         <th>NrWritable</th>");
            builder.AppendLine("         <th>NrFilled</th>");
            builder.AppendLine("         <th>NrEmpty</th>");
            builder.AppendLine("         <th>PatchIds</th>");
            builder.AppendLine("       </tr>");
            builder.AppendLine("       <xsl:for-each select=\"file_content_list/bank\">");
            builder.AppendLine("         <tr>");
            builder.AppendLine("           <td><xsl:value-of select=\"type\"/></td>");
            builder.AppendLine("           <td><xsl:value-of select=\"content_type\"/></td>");
            builder.AppendLine("           <td><xsl:value-of select=\"id\"/></td>");
            builder.AppendLine("           <td><xsl:value-of select=\"nr_writable_patches\"/></td>");
            builder.AppendLine("           <td><xsl:value-of select=\"nr_filled_patches\"/></td>");
            builder.AppendLine("           <td><xsl:value-of select=\"nr_empty_patches\"/></td>");
            builder.AppendLine("           <td><xsl:value-of select=\"patch_ids\"/></td>");
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


        /// <summary>
        /// IMPR: Move to test code.
        /// Test code for Util.GetPatchIdsString(). Needs to be put elsewhere, e.g. in unit tests. Now it's called when 
        /// this list is generated (commented out).
        /// Usage: Please execute this only with the Korg Kronos Factory PRELOAD.PCG loaded because it makes assumptions
        ///  of the banks (and set lists) there!
        /// </summary>
        /// <param name="writer"></param>
        private void TestPatchIdsString(TextWriter writer)
        {
            var tests = new Dictionary<string, List<IPatch>>
            {
                {
                    "Empty list",
                    new List<IPatch>()
                },
                {
                    "Single patch",
                    new List<IPatch>
                    {
                        PcgMemory.ProgramBanks[0][1]
                    }
                },
                {
                    "Twice same patch",
                    new List<IPatch>
                    {
                        PcgMemory.ProgramBanks[1][2],
                        PcgMemory.ProgramBanks[1][2]
                    }
                },
                {
                    "Range of two",
                    new List<IPatch>
                    {
                        PcgMemory.ProgramBanks[2][3],
                        PcgMemory.ProgramBanks[2][4]
                    }
                },
                {
                    "Range of three",
                    new List<IPatch>
                    {
                        PcgMemory.ProgramBanks[3][4],
                        PcgMemory.ProgramBanks[3][5],
                        PcgMemory.ProgramBanks[3][6]
                    }
                },
                {
                    "Two separate patches",
                    new List<IPatch>
                    {
                        PcgMemory.ProgramBanks[4][5],
                        PcgMemory.ProgramBanks[4][7]
                    }
                },
                {
                    "Three separate patches",
                    new List<IPatch>
                    {
                        PcgMemory.ProgramBanks[4][5],
                        PcgMemory.ProgramBanks[4][7],
                        PcgMemory.ProgramBanks[4][12]
                    }
                },
                {
                    "Range and a separate patch",
                    new List<IPatch>
                    {
                        PcgMemory.ProgramBanks[7][2],
                        PcgMemory.ProgramBanks[7][3],
                        PcgMemory.ProgramBanks[7][12]
                    }
                },
                {
                    "Separate patch and a range",
                    new List<IPatch>
                    {
                        PcgMemory.ProgramBanks[10][4],
                        PcgMemory.ProgramBanks[10][11],
                        PcgMemory.ProgramBanks[10][12]
                    }
                },
                {
                    "Two patches in separate banks",
                    new List<IPatch>
                    {
                        PcgMemory.ProgramBanks[12][4],
                        PcgMemory.ProgramBanks[13][5]
                    }
                },
                {
                    "Two ranges in separate banks",
                    new List<IPatch>
                    {
                        PcgMemory.ProgramBanks[13][4],
                        PcgMemory.ProgramBanks[13][5],
                        PcgMemory.ProgramBanks[15][6],
                        PcgMemory.ProgramBanks[15][7]
                    }
                },
                {
                    "Mix of program, combi and slot",
                    new List<IPatch>
                    {
                        PcgMemory.ProgramBanks[14][12],
                        PcgMemory.CombiBanks[4][20],
                        PcgMemory.SetLists[24][120]
                    }
                }
                // IMPROVE LV: More testing might be advisable, but above tests are executed correctly...
            };

            writer.WriteLine("BEGIN TEST");
            foreach (var test in tests)
            {
                writer.WriteLine("    {0,-30}: {1}", test.Key, Util.GetPatchIdsString(test.Value));
            }
            writer.WriteLine("END TEST");
        }
    }
}
