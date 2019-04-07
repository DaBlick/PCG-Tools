// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PcgTools.ListGenerator;
using PcgTools.PcgToolsResources;

namespace PcgTools
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CommandLineArgumentException: ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public CommandLineArgumentException(string message) : base(message)
        {
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class CommandLineArguments
    {
        /// <summary>
        /// 
        /// </summary>
        private string _diagnosticOutput;


        /// <summary>
        /// 
        /// </summary>
        List<string> _parameters;


        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, string> _options;


        /// <summary>
        /// 
        /// </summary>
        public string PcgFileName { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public ListGenerator.ListGenerator ListGenerator { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        private bool StartWindowsVersion { get; set; }

        
        /// <summary>
        /// Returns true if handled as command line argument.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public void Run(string[] arguments)
        {
            ListGenerator = null;
            bool helpOrError;
            try
            {
                helpOrError = HandleArguments(arguments);
            }
            catch (CommandLineArgumentException exc)
            {
                ShowError(exc);
                helpOrError = true;
            }

            StartWindowsVersion = (ListGenerator == null) &&
                                  !helpOrError && (_options.Count == 0) && (_parameters.Count == 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private bool HandleArguments(string[] arguments)
        {
            if ((arguments.Length == 1) && ((arguments[0] == "-h") || (arguments[0] == "-help")))
            {
                ShowHelp();
                return true;
            }

            ConvertArguments(arguments);
            ParseParameters();
            ParseOptions();
            SetDefaults();

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exc"></param>
        /// <returns></returns>
        private void ShowError(CommandLineArgumentException exc)
        {
            _diagnosticOutput += exc.Message + "\n\n";
            ShowHelp();
            ListGenerator = null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        void ConvertArguments(IList<string> arguments)
        {
            _parameters = new List<string>();
            _options = new Dictionary<string, string>();
            for (var i = 0; i < arguments.Count; i++)
            {
                if (arguments[i].StartsWith("-"))
                {
                    if (_parameters.Count > 0)
                    {
                        throw new CommandLineArgumentException(string.Format(Strings.CliOptionError, arguments[i]));
                    }
                    
                    if (i + 1 >= arguments.Count)
                    {
                        throw new CommandLineArgumentException(string.Format(Strings.CliIllegalOptionError, arguments[i]));
                    }

                    _options[arguments[i]] = arguments[i + 1];
                    i++;
                }
                else
                {
                    _parameters.Add(arguments[i]);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ParseParameters()
        {
            // Check there are exactly 4 parameters.
            if (_parameters.Count != 4)
            {
                throw new CommandLineArgumentException(Strings.FourParametersNeededError);
            }

            PcgFileName = _parameters[0];

            // Create generator.
            switch (GetMatch(new List<string> {"PATCH_LIST", "PROGRAM_USAGE_LIST", "COMBI_CONTENT_LIST", "DIFFERENCES_LIST",
             "FILE_CONTENT_LIST"}, _parameters[1]))
            {
            case "PATCH_LIST":
                ListGenerator = new ListGeneratorPatchList();
                break;

            case "PROGRAM_USAGE_LIST":
                ListGenerator = new ListGeneratorProgramUsageList();
                break;

            case "COMBI_CONTENT_LIST":
                ListGenerator = new ListGeneratorCombiContentList();
                break;

            case "DIFFERENCES_LIST":
                ListGenerator = new ListGeneratorDifferencesList();
                break;

            case "FILE_CONTENT_LIST":
                ListGenerator = new ListGeneratorFileContentList();
                break;

            default:
                throw new CommandLineArgumentException(string.Format(Strings.IllegalListType, _parameters[1]));
            } // Default not needed (exception thrown)

            switch (GetMatch(new List<string> {"COMPACT", "SHORT", "DEFAULT"}, _parameters[2]))
            {
            case "COMPACT":
                ListGenerator.ListSubType = PcgTools.ListGenerator.ListGenerator.SubType.Compact;
                break;

            case "SHORT":
                ListGenerator.ListSubType = PcgTools.ListGenerator.ListGenerator.SubType.Short;
                break;

            case "DEFAULT":
                // Do nothing
                break;

            default:
                throw new CommandLineArgumentException(string.Format(Strings.IllegalSubType, _parameters[2]));
            }

            ListGenerator.OutputFileName = _parameters[3];
        }


        /// <summary>
        /// 
        /// </summary>
        void ParseOptions()
        {
            foreach (var optionPair in _options)
            {
                ParseOption(optionPair);

                if (StartWindowsVersion)
                {
                    break;
                }
            }

            // Check if -h(elp) is used (Quit is true).
            if (StartWindowsVersion && (_options.Count > 1))
            {
                throw new CommandLineArgumentException(Strings.CannotMingleOptions);
            }

            // Check set list range from/to
            if ((ListGenerator != null) && ListGenerator.SetListsEnabled && (ListGenerator.SetListsRangeFrom > ListGenerator.SetListsRangeTo))
            {
                throw new CommandLineArgumentException(Strings.SetListError);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionPair"></param>
        private void ParseOption(KeyValuePair<string, string> optionPair)
        {
            switch (optionPair.Key)
            {
                case "-dlmnod":
                    ParseOptionDlmnod(optionPair);
                    break;

                case "-dlipn":
                    ParseOptionDlipn(optionPair);
                    break;

                case "-dlislsd":
                    ParseOptionDlislsd(optionPair);
                    break;

                case "-dlsbd":
                    ParseOptionDlsbd(optionPair);
                    break;

                case "-f":
                    ListGenerator.FilterOnText = GetBooleanOptionValue(optionPair);
                    break;

                case "-fcs":
                    CheckOption(optionPair, "-f");
                    ListGenerator.FilterCaseSensitive = GetBooleanOptionValue(optionPair);
                    break;

                case "-fpn":
                    CheckOption(optionPair, "-f");
                    ListGenerator.FilterProgramNames = GetBooleanOptionValue(optionPair);
                    break;

                case "-fcn":
                    CheckOption(optionPair, "-f");
                    ListGenerator.FilterCombiNames = GetBooleanOptionValue(optionPair);
                    break;

                case "-fslsn":
                    CheckOption(optionPair, "-f");
                    ListGenerator.FilterSetListSlotNames = GetBooleanOptionValue(optionPair);
                    break;

                case "-fslsd":
                    CheckOption(optionPair, "-f");
                    ListGenerator.FilterSetListSlotDescription = GetBooleanOptionValue(optionPair);
                    break;

                case "-fdkn":
                    CheckOption(optionPair, "-f");
                    ListGenerator.FilterDrumKitNames = GetBooleanOptionValue(optionPair);
                    break;

                    //TODO: Drum paterns

                case "-fwsn":
                    CheckOption(optionPair, "-f");
                    ListGenerator.FilterWaveSequenceNames = GetBooleanOptionValue(optionPair);
                    break;

                case "-ft":
                    CheckOption(optionPair, "-f");
                    ListGenerator.FilterText = optionPair.Value;
                    break;

                case "-fpb":
                    ListGenerator.FilterProgramBankNames = GetBankNamesOptionValue(optionPair);
                    break;

                case "-ieip":
                    ListGenerator.IgnoreInitPrograms = GetBooleanOptionValue(optionPair);
                    break;

                case "-ifp":
                    ListGenerator.IgnoreFirstProgram = GetBooleanOptionValue(optionPair);
                    break;

                case "-fcb":
                    ListGenerator.FilterCombiBankNames = GetBankNamesOptionValue(optionPair);
                    break;

                case "-ieic":
                    ListGenerator.IgnoreInitCombis = GetBooleanOptionValue(optionPair);
                    break;

                case "-imot":
                    ListGenerator.IgnoreMutedOffTimbres = GetBooleanOptionValue(optionPair);
                    break;

                case "-ifipr":
                    ListGenerator.IgnoreMutedOffFirstProgramTimbre = GetBooleanOptionValue(optionPair);
                    break;

                case "-fsl":
                    ListGenerator.SetListsEnabled = GetBooleanOptionValue(optionPair);
                    break;

                case "-ieis":
                    ListGenerator.IgnoreInitSetListSlots = GetBooleanOptionValue(optionPair);
                    break;

                case "-fslrf":
                    CheckOption(optionPair, "-fsl");
                    ListGenerator.SetListsRangeFrom = GetIntOptionValue(optionPair, 0, 128);
                    break;

                case "-fslrt":
                    CheckOption(optionPair, "-fsl");
                    ListGenerator.SetListsRangeTo = GetIntOptionValue(optionPair, 0, 128);
                    break;

                case "-fd":
                    ListGenerator.DrumKitsEnabled = GetBooleanOptionValue(optionPair);
                    break;

                    //TODO: Drum paterns

                case "-ieid":
                    ListGenerator.IgnoreInitDrumKits = GetBooleanOptionValue(optionPair);
                    break;

                case "-fw":
                    ListGenerator.WaveSequencesEnabled = GetBooleanOptionValue(optionPair);
                    break;

                case "-ieiw":
                    ListGenerator.IgnoreInitWaveSequences = GetBooleanOptionValue(optionPair);
                    break;

                case "-s":
                    ParseOptionS(optionPair);
                    break;

                case "-o":
                    ParseOptionO(optionPair);
                    break;

                default:
                    throw new CommandLineArgumentException(string.Format(Strings.IllegalOption, optionPair.Key));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionPair"></param>
        private void ParseOptionDlmnod(KeyValuePair<string, string> optionPair)
        {
            {
                var diffListGenerator = ListGenerator as ListGeneratorDifferencesList;
                if (diffListGenerator == null)
                {
                    throw new CommandLineArgumentException(
                        string.Format(Strings.OnlyDifferencesError, optionPair.Key));
                }

                diffListGenerator.MaxDiffs = GetIntOptionValue(optionPair, 0, 1000);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionPair"></param>
        private void ParseOptionDlipn(KeyValuePair<string, string> optionPair)
        {
            {
                var diffListGenerator = ListGenerator as ListGeneratorDifferencesList;
                if (diffListGenerator == null)
                {
                    throw new CommandLineArgumentException(
                        string.Format(Strings.OnlyDifferencesError, optionPair.Key));
                }

                diffListGenerator.IgnorePatchNames = GetBooleanOptionValue(optionPair);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionPair"></param>
        private void ParseOptionDlislsd(KeyValuePair<string, string> optionPair)
        {
            {
                var diffListGenerator = ListGenerator as ListGeneratorDifferencesList;
                if (diffListGenerator == null)
                {
                    throw new CommandLineArgumentException(
                        string.Format(Strings.OnlyDifferencesError, optionPair.Key));
                }

                diffListGenerator.IgnoreSetListSlotDescriptions = GetBooleanOptionValue(optionPair);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionPair"></param>
        private void ParseOptionDlsbd(KeyValuePair<string, string> optionPair)
        {
            {
                var diffListGenerator = ListGenerator as ListGeneratorDifferencesList;
                if (diffListGenerator == null)
                {
                    throw new CommandLineArgumentException(
                        string.Format(Strings.OnlyDifferencesError, optionPair.Key));
                }

                diffListGenerator.SearchBothDirections = GetBooleanOptionValue(optionPair);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionPair"></param>
        private void ParseOptionS(KeyValuePair<string, string> optionPair)
        {
            switch (GetMatch(new List<string> {"TYPE_BANK_INDEX", "TBI", "ALPHABETICAL"}, optionPair.Value))
            {
                case "TYPE_BANK_INDEX": // Fall Through
                case "TB":
                    ListGenerator.SortMethod = PcgTools.ListGenerator.ListGenerator.Sort.TypeBankIndex;
                    break;

                case "CATEGORICAL":
                    ListGenerator.SortMethod = PcgTools.ListGenerator.ListGenerator.Sort.Categorical;
                    break;

                case "ALPHABETICAL":
                    ListGenerator.SortMethod = PcgTools.ListGenerator.ListGenerator.Sort.Alphabetical;
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionPair"></param>
        private void ParseOptionO(KeyValuePair<string, string> optionPair)
        {
            switch (GetMatch(new List<string>
            {
                "ASCII_TABLE",
                "ASCII",
                "TXT",
                "TEXT",
                "CSV",
                "COMMA_SEPARATED_VALUES",
                "XML"
            }, optionPair.Value))
            {
                case "ASCII_TABLE":
                case "ASCII":
                    ListGenerator.ListOutputFormat = PcgTools.ListGenerator.ListGenerator.OutputFormat.AsciiTable;
                    break;

                case "TEXT": // Fall Through
                case "TXT":
                    ListGenerator.ListOutputFormat = PcgTools.ListGenerator.ListGenerator.OutputFormat.Text;
                    break;

                case "COMMA_SEPARATED_VALUES": // Fall through
                case "CSV":
                    ListGenerator.ListOutputFormat = PcgTools.ListGenerator.ListGenerator.OutputFormat.Csv;
                    break;

                case "XML":
                    ListGenerator.ListOutputFormat = PcgTools.ListGenerator.ListGenerator.OutputFormat.Xml;
                    break;
            } // break not needed  
        }


        /// <summary>
        /// Set defaults not set by options. Use the defaults from ShowHelp.
        /// </summary>
        void SetDefaults()
        {
            if (_options.Keys.Contains("-h"))
            {
                return;
            }

            if (ListGenerator is ListGeneratorDifferencesList)
            {
                SetDefaultsListGeneratorDifferencesList();
            }

            if (_options.Keys.Contains("-f"))
            {
                SetDefaultsF();
            }

            if (!_options.Keys.Contains("-fpb"))
            {
                SetDefaultsFpb();
            }

            if (!_options.Keys.Contains("-ieip"))
            {
                ListGenerator.IgnoreInitPrograms = true;
            }

            if (!_options.Keys.Contains("-fcb"))
            {
                SetDefaultsFcb();
            }

            if (!_options.Keys.Contains("-ieic"))
            {
                ListGenerator.IgnoreInitCombis = true;
            }

            if (!_options.Keys.Contains("-imot"))
            {
                ListGenerator.IgnoreMutedOffTimbres = true;
            }

            if (!_options.Keys.Contains("-ifipr"))
            {
                ListGenerator.IgnoreMutedOffFirstProgramTimbre = true;
            }

            if (!_options.Keys.Contains("-fsl"))
            {
                ListGenerator.SetListsEnabled = true;
            }

            if (!_options.Keys.Contains("-ieis"))
            {
                ListGenerator.IgnoreInitSetListSlots = true;
            }

            if (!_options.Keys.Contains("-fslrf"))
            {
                ListGenerator.SetListsRangeFrom = 0;
            }

            if (!_options.Keys.Contains("-fslrt"))
            {
                ListGenerator.SetListsRangeTo = 127;
            }

            if (!_options.Keys.Contains("-ieid"))
            {
                ListGenerator.IgnoreInitDrumKits = true;
            }

            //TODO drum patterns

            if (!_options.Keys.Contains("-ieiw"))
            {
                ListGenerator.IgnoreInitWaveSequences = true;
            }

            if (!_options.Keys.Contains("-s"))
            {
                ListGenerator.SortMethod = PcgTools.ListGenerator.ListGenerator.Sort.Alphabetical;
            }

            if (!_options.Keys.Contains("-o"))
            {
                ListGenerator.ListOutputFormat = PcgTools.ListGenerator.ListGenerator.OutputFormat.Text;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetDefaultsListGeneratorDifferencesList()
        {
            var diffListGenerator = ListGenerator as ListGeneratorDifferencesList;
            Debug.Assert(diffListGenerator != null);

            if (!_options.Keys.Contains("-dlmnod"))
            {
                diffListGenerator.MaxDiffs = 10;
            }

            if (!_options.Keys.Contains("-dlipn"))
            {
                diffListGenerator.IgnorePatchNames = true;
            }

            if (!_options.Keys.Contains("-dlislsd"))
            {
                diffListGenerator.IgnoreSetListSlotDescriptions = true;
            }

            if (!_options.Keys.Contains("-dlsbd"))
            {
                diffListGenerator.SearchBothDirections = false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetDefaultsF()
        {
            if (!_options.Keys.Contains("-fpn"))
            {
                ListGenerator.FilterProgramNames = true;
            }

            if (!_options.Keys.Contains("-fcn"))
            {
                ListGenerator.FilterCombiNames = true;
            }

            if (!_options.Keys.Contains("-fslsn"))
            {
                ListGenerator.FilterSetListSlotNames = true;
            }

            if (!_options.Keys.Contains("-fslsd"))
            {
                ListGenerator.FilterSetListSlotDescription = true;
            }

            if (!_options.Keys.Contains("-fwsn"))
            {
                ListGenerator.FilterWaveSequenceNames = true;
            }

            if (!_options.Keys.Contains("-fdkn"))
            {
                ListGenerator.FilterDrumKitNames = true;
            }

            // TODO drum patterns
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetDefaultsFpb()
        {
            ListGenerator.FilterProgramBankNames =
                new List<string>
                {
                    "A",
                    "B",
                    "C",
                    "D",
                    "E",
                    "F",
                    "GM",
                    "H",
                    "I",
                    "J",
                    "K",
                    "L",
                    "M",
                    "N",
                    "I-A",
                    "I-B",
                    "I-C",
                    "I-D",
                    "I-E",
                    "I-F",
                    "U-A",
                    "U-B",
                    "U-C",
                    "U-D",
                    "U-E",
                    "U-F",
                    "U-G",
                    "U-AA",
                    "U-BB",
                    "U-CC",
                    "U-DD",
                    "U-EE",
                    "U-FF",
                    "U-GG"
                };
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetDefaultsFcb()
        {
            ListGenerator.FilterCombiBankNames =
                new List<string>
                {
                    "A",
                    "B",
                    "C",
                    "D",
                    "E",
                    "F",
                    "G",
                    "H",
                    "I",
                    "J",
                    "K",
                    "L",
                    "M",
                    "N",
                    "I-A",
                    "I-B",
                    "I-C",
                    "I-D",
                    "I-E",
                    "I-F",
                    "I-G",
                    "U-A",
                    "U-B",
                    "U-C",
                    "U-D",
                    "U-E",
                    "U-F",
                    "U-G"
                };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <param name="requiredOption"></param>
        void CheckOption(KeyValuePair<string, string> option, string requiredOption)
        {
            if (!_options.Keys.Contains(requiredOption))
            {
                throw new CommandLineArgumentException(string.Format(Strings.CliRequiresError, option.Key, requiredOption));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionPair"></param>
        /// <returns></returns>
        static bool GetBooleanOptionValue(KeyValuePair<string, string> optionPair)
        {
            var matchedValue = GetMatch(new List<string> {"1", "ON", "TRUE", "0", "OFF", "FALSE"}, optionPair.Value);
            return (new List<string> {"1", "ON", "TRUE"}).Contains(matchedValue);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionPair"></param>
        /// <param name="minimumValue"></param>
        /// <param name="maximumValue"></param>
        /// <returns></returns>
        static int GetIntOptionValue(KeyValuePair<string, string> optionPair, int minimumValue, int maximumValue)
        {
            int value;
            Int32.TryParse(optionPair.Value, out value);
            if ((value < minimumValue) || (value > maximumValue))
            {
                throw new CommandLineArgumentException(
                    string.Format(Strings.CliOutOfRangeError, optionPair.Value, optionPair.Key,
                        $"[{minimumValue}..{maximumValue}]"));
            }
            return value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionPair"></param>
        /// <returns></returns>
        static List<string> GetBankNamesOptionValue(KeyValuePair<string, string> optionPair)
        {
            var bankNames = new string[0];

            var banks = optionPair.Value.Trim().ToUpper();
            if (banks != "NONE")
            {
                bankNames = banks.Split(',');
            }

            var adaptedBankNames = new List<string>();
            foreach (var bank in bankNames)
            {
                var adaptedBankName = bank.Trim().ToUpper();
                GetBankNameOptionValue(optionPair, adaptedBankName, bank);

                adaptedBankNames.Add(adaptedBankName);
            }
            return adaptedBankNames;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionPair"></param>
        /// <param name="adaptedBankName"></param>
        /// <param name="bank"></param>
        private static void GetBankNameOptionValue(KeyValuePair<string, string> optionPair, string adaptedBankName, string bank)
        {
            bool error;
            switch (adaptedBankName.Length)
            {
                case 1:
                    error = HandleBankeNameLength1(adaptedBankName);
                    break;

                case 2:
                    error = (adaptedBankName != "GM");
                    break;

                case 3:
                    error = HandleBankeNameLength3(adaptedBankName);
                    break;

                default:
                    error = true;
                    break;
            }

            if (error)
            {
                throw new CommandLineArgumentException(string.Format(Strings.CliIllegalBankName,
                    optionPair.Key, optionPair.Value, bank));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="adaptedBankName"></param>
        /// <returns></returns>
        private static bool HandleBankeNameLength1(string adaptedBankName)
        {
            return ((adaptedBankName[0] < 'A') || ((adaptedBankName[0] > 'F') && (adaptedBankName[0] < 'H')) ||
                    (adaptedBankName[0] > 'N'));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="adaptedBankName"></param>
        /// <returns></returns>
        private static bool HandleBankeNameLength3(string adaptedBankName)
        {
            return (((adaptedBankName[0] != 'I') && (adaptedBankName[0] != 'U')) ||
                          (adaptedBankName[1] != '-') ||
                          ((adaptedBankName[2] < 'A') || ((adaptedBankName[2] > 'F') && (adaptedBankName[2] < 'H')) ||
                           (adaptedBankName[2] > 'N')));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="valueToFind"></param>
        /// <returns></returns>
        static string GetMatch(ICollection<string> values, string valueToFind)
        {
            Debug.Assert(values.Count > 0);
            var upperValues = (from value in values select value.ToUpper()).ToList();
            valueToFind = valueToFind.ToUpper();
            string matchedValue;

            // If an exact value can be found: ok.
            if (upperValues.Contains(valueToFind))
            {
                matchedValue = valueToFind;
            }
            else
            {
                // Remove all values not starting with the value to find.
                var startingValues = (from value in upperValues where value.StartsWith(valueToFind) select value).ToList();

                if (startingValues.Count == 0)
                {
                    throw new CommandLineArgumentException(string.Format(Strings.CliUnknownOption, valueToFind));
                }
                
                if (startingValues.Count > 1)
                {
                    throw new CommandLineArgumentException(string.Format(Strings.CliAmbiguousOption, valueToFind));
                }
                
                matchedValue = startingValues[0];
            }
            return matchedValue;
        }


        /// <summary>
        /// 
        /// </summary>
        private void ShowHelp()
        {
            var builder = new StringBuilder();
            builder.AppendLine("pcgtools.exe [options] pcg_file_name list_type sub_list_type output_file_name\n");
            builder.AppendLine("where:");
            builder.AppendLine("  pcg_file_name          File name of the PCG file name [options].");
            builder.AppendLine("  list_type    : [patch_list | program_usage_list | combi_content_list | differences_list | file_content_list].");
            builder.AppendLine("  sub_list_type: [default | compact | short].");
            builder.AppendLine("  output_file_name         File name of the output file.\n");
            builder.AppendLine("Options:");
            builder.AppendLine("-dlmnod <number>         Max Number of Differences in differences list.");
            builder.AppendLine("                         Only valid if a differences list is selected.");
            builder.AppendLine("                         Default: 10.\n");
            builder.AppendLine("-dlipn <on|off>          Ignore Patch Names in differences list.");
            builder.AppendLine("                         Only valid if a differences list is selected.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-dlislsd <on|off>        Ignore Set List Slot Descriptions in differences list.");
            builder.AppendLine("                         Only valid if a differences list is selected.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-dlsbd <on|off>          Search Both Directions in differences list.");
            builder.AppendLine("                         Only valid if a differences list is selected.");
            builder.AppendLine("                         Default: Off.\n");
            builder.AppendLine("-f <text>                Filter with text.\n");
            builder.AppendLine("-fcs <on|off>            Filter Case Sensitive.\n");
            builder.AppendLine("                         Only valid if -f option is used.");
            builder.AppendLine("                         Default: Off.\n");
            builder.AppendLine("-fpn <on|off>            Filter Program Names.");
            builder.AppendLine("                         Only valid if -f option is used.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-fcn <on|off>            Filter Combi Names.");
            builder.AppendLine("                         Only valid if -f option is used.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-fslsn <on|off>          Filter Set List Slot Names.");
            builder.AppendLine("                         Only valid if -f option is used.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-fslsd <on|off>          Filter Set List Slot Descriptions.");
            builder.AppendLine("                         Only valid if -f option is used.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-ft <text>               Text to filter on.");
            builder.AppendLine("                         Only valid if -f option is used.\n");
            builder.AppendLine("-fpb \"A, B, C, GM, ...\"  Filter Program Banks with list of bank names.");
            builder.AppendLine("-fpb \"I-A, U-G, ...\"      Id names depend on Korg model.");
            builder.AppendLine("                         Id names are case insensitive, spaces are ignored.");
            builder.AppendLine("                         Use None for removing all program banks.");
            builder.AppendLine("                         Default: All banks used.\n");
            builder.AppendLine("-h                       Shows this help file.\n");
            builder.AppendLine("-ieip <on|off>           Ignore Empty/Init Programs.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-ifp <on|off>            Ignore First Program.");
            builder.AppendLine("                         Default: Off.\n");
            builder.AppendLine("-fcb \"A, B, C, ...\"       Filter Combi Banks with list of bank names.");
            builder.AppendLine("-fcb \"I-A, U-G, ...\"      Id names depend on Korg model.");
            builder.AppendLine("                         Use None for removing all combi banks.");
            builder.AppendLine("                         Default: All banks used\n");
            builder.AppendLine("-ieic <on|off>           Ignore Empty/Init Combis.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-imot <on|off>           Ignore Muted/Off Timbres.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-fsl <on|off>            Filter Set Lists.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-ieis <on|off>           Ignore Empty/Init Set List Slots.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-fslrf <int>             Filter Set Lists Range From <lower set list>.");
            builder.AppendLine("                         Only valid if -fsl option is used.");
            builder.AppendLine("                         Default: 0.\n");
            builder.AppendLine("-fslrt <int>             Filter Set Lists Range From <upper set list>.");
            builder.AppendLine("                         Only valid if -fsl option is used.");
            builder.AppendLine("                         Default: 127.\n");
            builder.AppendLine("-fd <on|off>             Filter Drum Kits.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-ieid <on|off>           Ignore Empty/Init Drum Kits.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-fw <on|off>             Filter Wave Sequences.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-ieiw <on|off>           Ignore Empty/Init Wave Sequences.");
            builder.AppendLine("                         Default: On.\n");
            builder.AppendLine("-s (type_bank_index|alphabetical");
            builder.AppendLine("                         Sorting method:");
            builder.AppendLine("                         tbi  : ListSubType/Id/Index");
            builder.AppendLine("                         alpha: Alphabetical");
            builder.AppendLine("                         Default: alpha\n");
            builder.AppendLine("-o (ascii|text|csv|xml)  OutputFormat file format:");
            builder.AppendLine("                         ascii: ASCII table text");
            builder.AppendLine("                         text: Plain text");
            builder.AppendLine("                         csv: Comma separated value output file");
            builder.AppendLine("                         xml: XML output file");
            builder.AppendLine("                         Default: text\n\n");
            builder.AppendLine("on:  can be on , 1, true.");
            builder.AppendLine("off: can be off, 0, false.");
            builder.AppendLine("all argument can be truncated and are case insensitive (when unambiguous).");
            _diagnosticOutput = builder.ToString();
        }
    }
}