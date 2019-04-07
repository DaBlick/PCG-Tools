using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.PcgToolsResources;
using MessageBox = System.Windows.MessageBox;

namespace PcgTools.Tools
{
    /// <summary>
    /// Interaction logic for ProgramReferenceChangerWindow.xaml
    /// </summary>
    public partial class ProgramReferenceChangerWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IPcgMemory _memory;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        public ProgramReferenceChangerWindow(IPcgMemory memory)
        {
            _memory = memory;
            InitializeComponent();

            LabelProgress.Visibility = Visibility.Hidden;
            ProgressBarRules.Visibility = Visibility.Hidden;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Title = Strings.SelectFileToRead,
                Filter = "Text Files (*.txt)|*.txt",
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    TextBoxRules.Text  = System.IO.File.ReadAllText(dialog.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(this, $"{Strings.LinkWarning}.\n{Strings.Message}: {exception.Message}",
                     Strings.PcgTools,
                     MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            if (!_worker.IsBusy)
            {
                LabelProgress.Visibility = Visibility.Visible;
                ProgressBarRules.Visibility = Visibility.Visible;

                _worker.DoWork += DoWork_ChangeReferences;
                _worker.ProgressChanged += DoWork_OnProgressChanged;
                _worker.RunWorkerCompleted += DoWork_RunWorkerCompleted;
                object[] parameters = {TextBoxRules.Text};
                _worker.RunWorkerAsync(parameters);
            }
        }


        /// <summary>
        /// Notified when process changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoWork_OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarRules.Value = e.ProgressPercentage;
        }


        private RuleParser _ruleParser;

        /// <summary>
        /// Actual work.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoWork_ChangeReferences(object sender, DoWorkEventArgs e)
        {
            var rules = ((object[]) (e.Argument))[0] as string;

            var referenceChanger = new ReferenceChanger(_memory);
            _ruleParser = new RuleParser(_memory);
            referenceChanger.OnProgressHandler += OnProgress;
            referenceChanger.ParseRules(_ruleParser, rules);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ProgressBarRules.Maximum = 100;
            })); 
            referenceChanger.ChangeReferences();
        }


        /// <summary>
        /// Called when completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LabelProgress.Visibility = Visibility.Hidden;
            ProgressBarRules.Visibility = Visibility.Hidden;

            if (_ruleParser.HasParsedOk)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    DialogResult = true;
                    Close();
                }));
            }
            else
            {
                MessageBox.Show(this, $"Error in rules, line {_ruleParser.ParseErrorInLine + 1}",
                    Strings.PcgTools,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _worker.DoWork -= DoWork_ChangeReferences;
            _worker.ProgressChanged -= DoWork_OnProgressChanged;
            _worker.RunWorkerCompleted -= DoWork_RunWorkerCompleted;
        }


        /// <summary>
        /// Background worker for processing rules.
        /// </summary>
        private readonly BackgroundWorker _worker = new BackgroundWorker();



        /// <summary>
        /// 
        /// </summary>
        void OnProgress(ProgressChangedEventArgs args)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ProgressBarRules.Value = args.ProgressPercentage;
            }));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
