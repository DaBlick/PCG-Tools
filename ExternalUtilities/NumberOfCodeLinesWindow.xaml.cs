using System;
using System.IO;
using System.Text;
using System.Windows;

namespace ExternalUtilities
{
    /// <summary>
    /// Interaction logic for NumberOfCodeLinesWindow.xaml
    /// </summary>
    public partial class NumberOfCodeLinesWindow : Window
    {
        private const string ProjectFolder = @"C:/Users/Michel/OneDrive/PcgTools2013";

        private StringBuilder output = new StringBuilder();

        public NumberOfCodeLinesWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
               IterateThroughFolder(ProjectFolder, 0, 0);
            }
            catch (Exception)
            {
                TextBoxOutput.Text = output.ToString();
                throw;
            }

            TextBoxOutput.Text = output.ToString();
        }


        private int IterateThroughFolder(string folder, int level, int numberOfLines)
        {
            // Read all files.
            var files = Directory.GetFiles(folder);
            var indentation = new string(' ', level * 3);
            var numberOfLinesInFolder = 0;

            output.AppendFormat("\n\n{0}Folder: {1}\n", indentation, folder.Remove(0, ProjectFolder.Length));
            foreach (var file in files)
            {
                if (HandleFile(file))
                {
                    var lineCount = ReadLineCount(file);
                    numberOfLinesInFolder += lineCount;
                    output.AppendFormat("{0}{1,-120}{2,8}\n", indentation, file.Remove(0, ProjectFolder.Length + 1), lineCount);
                }
            }

            // Raed all folders.
            var subFolders = Directory.GetDirectories(folder);
            foreach (var subFolder in subFolders)
            {
                if (HandleFolder(subFolder))
                {
                    numberOfLinesInFolder += IterateThroughFolder(subFolder, level + 1, numberOfLinesInFolder);
                }
            }
            
            output.AppendFormat("{0}Total number of lines in this folder and Total:{1}{2} / {3}\n", 
                indentation, new string(' ', 85), numberOfLinesInFolder, numberOfLines + numberOfLinesInFolder);

            return numberOfLinesInFolder;
        }

        private bool HandleFolder(string subFolder)
        {
            subFolder = subFolder.Remove(0, ProjectFolder.Length);
            //var ignore = new[] {".git"};
            return
                subFolder != @"\.git" &&
                subFolder != @"\Debug" &&
                subFolder != @"\Documentation" &&
                subFolder != @"\Extended WPF Toolkit Binaries" &&
                subFolder != @"\Installation\Installation"&& 
                subFolder != @"\KorgKronosTools\Help\External Links" &&
                subFolder != @"\KorgKronosTools\Gui\Images" &&
                subFolder != @"\KorgKronosTools\Gui\Tool Bar Icons" &&
                subFolder != @"\KorgKronosTools\WPF.MDI\Backup" &&
                subFolder != @"\KorgKronosTools\WPF.MDI\Example" &&
                subFolder != @"\KorgKronosTools\WPF.MDI\WPF.MDI\Themes\Resources" &&
                subFolder != @"\Model" &&
                subFolder != @"\Release" &&
                subFolder != @"\TestResults" &&
                subFolder != @"\WPF.MDI" &&
                subFolder != @"\_Website\Counters" &&
                subFolder != @"\_Website\Files" &&
                subFolder != @"\_Website\Pictures" &&

                !subFolder.EndsWith(@"\bin") && // Generic
                !subFolder.EndsWith(@"\obj"); // Generic
        }

        private bool HandleFile(string file)
        {
            return file.EndsWith(".sln") ||
                   file.EndsWith(".csproj") ||
                   file.EndsWith(".cs") ||
                   file.EndsWith(".xaml") ||
                   file.EndsWith(".config") ||
                   file.EndsWith(".DotSettings") ||
                   file.EndsWith(".pfx") ||
                   file.EndsWith(".isl") || // Installer
                   file.EndsWith(".isproj") ||  // Installer
                   file.EndsWith(".psess") ||
                   file.EndsWith(".ReSharper") ||
                   file.EndsWith(".testsettings") ||
                   file.EndsWith(".user") ||
                   file.EndsWith(".php") ||
                   file.EndsWith(".html") ||
                   file.EndsWith(".css");
        }


        private int ReadLineCount(string file)
        {

            return File.ReadAllLines(file).Length;
        }

        
    }
}
