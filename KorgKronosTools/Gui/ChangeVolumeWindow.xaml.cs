using System;
using System.Windows;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchSorting;
using PcgTools.Properties;
using Common.Extensions;
using PcgTools.ViewModels.Commands.PcgCommands;

namespace PcgTools.Gui
{
    /// <summary>
    /// Interaction logic for SelectSortWindow.xaml
    /// </summary>
    public partial class ChangeVolumeWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        ChangeVolumeParameters _parameters;


        /// <summary>
        /// 
        /// </summary>
        public ChangeVolumeParameters ChangeVolumeParameters { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        public ChangeVolumeWindow(ChangeVolumeParameters changeVolumeParameters)
        {
            _parameters = changeVolumeParameters;
            InitializeComponent();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            radioButtonFixed.IsChecked = true;
            RadioButtonFixed_Checked(null, null);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonFixed_Checked(object sender, RoutedEventArgs e)
        {
            NumericUpDownValue.Value = 127;
            NumericUpDownValue.Minimum = 0;
            NumericUpDownValue.Maximum = 127;

            labelTo.IsEnabled = false;

            NumericUpDownToValue.IsEnabled = false;
            NumericUpDownToValue.Value = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonRelative_Checked(object sender, RoutedEventArgs e)
        {
            NumericUpDownValue.Value = 0;
            NumericUpDownValue.Minimum = -127;
            NumericUpDownValue.Maximum = 127;

            labelTo.IsEnabled = false;

            NumericUpDownToValue.IsEnabled = false;
            NumericUpDownToValue.Value = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonPercentage_Checked(object sender, RoutedEventArgs e)
        {
            NumericUpDownValue.Value = 100;
            NumericUpDownValue.Minimum = 0;
            NumericUpDownValue.Maximum = 1000;

            labelTo.IsEnabled = false;

            NumericUpDownToValue.IsEnabled = false;
            NumericUpDownToValue.Value = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonMapped_Checked(object sender, RoutedEventArgs e)
        {
            NumericUpDownValue.Value = 0;
            NumericUpDownValue.Minimum = 0;
            NumericUpDownValue.Maximum = 127;

            labelTo.IsEnabled = true;

            NumericUpDownToValue.IsEnabled = true;
            NumericUpDownToValue.Value = 127;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonSmartMapped_Checked(object sender, RoutedEventArgs e)
        {
            NumericUpDownValue.Value = 0;
            NumericUpDownValue.Minimum = 0;
            NumericUpDownValue.Maximum = 127;

            labelTo.IsEnabled = true;

            NumericUpDownToValue.IsEnabled = true;
            NumericUpDownToValue.Value = 127;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            if (radioButtonMapped.IsReallyChecked() && 
                NumericUpDownValue.Value.Value > NumericUpDownToValue.Value.Value)
            {
                MessageBox.Show("Value cannot be higher than to value", "Change Volume values incorrect",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                DialogResult = false;
            }
            else
            {
                labelTo.IsEnabled = false;
                NumericUpDownToValue.IsEnabled = false;

                if (radioButtonFixed.IsReallyChecked())
                {
                    _parameters.ChangeType = ChangeVolumeParameters.EChangeType.Fixed;
                }
                else if (radioButtonRelative.IsReallyChecked())
                {
                    _parameters.ChangeType = ChangeVolumeParameters.EChangeType.Relative;
                }
                else if (radioButtonPercentage.IsReallyChecked())
                {
                    _parameters.ChangeType = ChangeVolumeParameters.EChangeType.Percentage;
                }
                else if (radioButtonMapped.IsReallyChecked())
                {
                    _parameters.ChangeType = ChangeVolumeParameters.EChangeType.Mapped;
                }
                else if (radioButtonSmartMapped.IsReallyChecked())
                {
                    _parameters.ChangeType = ChangeVolumeParameters.EChangeType.SmartMapped;
                }

                _parameters.Value = NumericUpDownValue.Value.Value;
                _parameters.ToValue = NumericUpDownToValue.Value.Value;

                DialogResult = true;
                Close();
            }
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
