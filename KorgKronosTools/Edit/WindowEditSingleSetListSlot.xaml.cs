// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Common.Extensions;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.PcgToolsResources;
using Color = System.Windows.Media.Color;

namespace PcgTools.Edit
{
    /// <summary>
    /// Interaction logic for WindowEdit.xaml
    /// </summary>
    public partial class WindowEditSingleSetListSlot // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly ISetListSlot _patch;


        /// <summary>
        /// 
        /// </summary>
        private bool _ok = true;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        public WindowEditSingleSetListSlot(ISetListSlot patch)
        {
            _patch = patch;
            InitializeComponent();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowEditLoaded(object sender, RoutedEventArgs e)
        {
            textBoxId.Text = _patch.Id;
            textBoxName.Text = _patch.Name;

            WindowEditLoadedSetReferences();

            textBoxDescription.Text = _patch.Description;
            intupdownVolume.Value = _patch.Volume;

            if (_patch.Color == null)
            {
                expanderColor.Visibility = Visibility.Collapsed;
            }
            else
            {
                comboBoxColor.SelectedIndex = _patch.Color.Value%16;
            }

            WindowEditLoadedSetTextSize();

            // Set transpose.
            if (_patch.PcgRoot.Model.OsVersion == Models.EOsVersion.EOsVersionKronos3x)
            {
                intupdownTranspose.Value = _patch.Transpose;
            }
            else
            {
                expanderTranspose.Visibility = Visibility.Collapsed;
            }

            Check();
        }


        /// <summary>
        /// 
        /// </summary>
        private void WindowEditLoadedSetReferences()
        {
            var usedPatch = _patch.UsedPatch;
            if (usedPatch == null)
            {
                // If not filled in, it must be a song.
                TextBoxReferenceType.Text = Strings.ReferenceTypeSong;
                TextBoxReferenceId.Text = "-";
                TextBoxReferenceName.Text = "-";
            }
            else
            {
                if (usedPatch is Program)
                {
                    TextBoxReferenceType.Text = Strings.ReferenceTypeProgram;
                    TextBoxReferenceId.Text = usedPatch.Id;
                    TextBoxReferenceName.Text = usedPatch.Name;
                }
                else if (usedPatch is Combi)
                {
                    TextBoxReferenceType.Text = Strings.ReferenceTypeCombi;
                    TextBoxReferenceId.Text = usedPatch.Id;
                    TextBoxReferenceName.Text = usedPatch.Name;
                }
                else
                {
                    throw new ApplicationException("Illegal used patch type");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void WindowEditLoadedSetTextSize()
        {
            if (_patch.PcgRoot.Model.OsVersion == Models.EOsVersion.EOsVersionKronos3x)
            {
                switch (_patch.SelectedTextSize)
                {
                    case SetListSlot.TextSize.Xs:
                        radioButtonTextSizeXs.IsChecked = true;
                        break;

                    case SetListSlot.TextSize.S:
                        radioButtonTextSizeS.IsChecked = true;
                        break;

                    case SetListSlot.TextSize.M:
                        radioButtonTextSizeM.IsChecked = true;
                        break;

                    case SetListSlot.TextSize.L:
                        radioButtonTextSizeL.IsChecked = true;
                        break;

                    case SetListSlot.TextSize.Xl:
                        radioButtonTextSizeXl.IsChecked = true;
                        break;

                    default:
                        throw new ApplicationException("Illegal text size");
                }
            }
            else
            {
                radioButtonTextSizeXl.Visibility = Visibility.Collapsed;
                radioButtonTextSizeL.Visibility = Visibility.Collapsed;
                radioButtonTextSizeM.Visibility = Visibility.Collapsed;
                radioButtonTextSizeS.Visibility = Visibility.Collapsed;
                radioButtonTextSizeXs.Visibility = Visibility.Collapsed;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxNameTextChanged(object sender, TextChangedEventArgs e)
        {
            Check();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxDescriptionTextChanged(object sender, TextChangedEventArgs e)
        {
            Check();
        }


        /// <summary>
        /// 
        /// </summary>
        private void Check()
        {
            var usedSize = textBoxName.Text.Length;
            labelNameLength.Text = string.Format('(' + Strings.XOfYCharacters_editw + ')', usedSize,
                _patch.MaxNameLength);
            labelErrorName.Content = EditUtils.CheckText(textBoxName.Text, _patch.MaxNameLength,
                EditUtils.ECheckType.Name);

            // Check set list slot description.
            // Set length.
            var usedDescriptionSize = textBoxDescription.Text.Length;
            var maxDescriptionLength = _patch.MaxDescriptionLength;
            labelDescriptionLength.Text = string.Format('(' + Strings.XOfYCharacters_editw + ')',
                usedDescriptionSize, maxDescriptionLength);

            // Check description.
            labelErrorDescription.Content = EditUtils.CheckText(
                textBoxDescription.Text, _patch.MaxDescriptionLength, EditUtils.ECheckType.Description);

            _ok = labelErrorName.Content.Equals(string.Empty) && labelErrorDescription.Content.Equals(string.Empty);
            buttonOk.IsEnabled = _ok;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            if (_ok)
            {
                // Set name.
                _patch.Name = textBoxName.Text;

                // Set description.
                _patch.Description = textBoxDescription.Text;

                // Set volume.
                _patch.Volume = intupdownVolume.Value.HasValue ? (int) intupdownVolume.Value : 0;

                // Set color.
                if (_patch.Color != null)
                {
                    _patch.Color.Value = comboBoxColor.SelectedIndex;
                }

                // Set transpose/ text size.
                if (_patch.PcgRoot.Model.OsVersion == Models.EOsVersion.EOsVersionKronos3x)
                {
                    _patch.Transpose = intupdownTranspose.Value.HasValue ? (int) intupdownTranspose.Value : 0;

                    ButtonOkSetTextSize();
                }

                // Notify others.
                _patch.Update("ContentChanged");
            }

            DialogResult = true;
            Close();
        }


        /// <summary>
        /// 
        /// </summary>
        private void ButtonOkSetTextSize()
        {
            if (radioButtonTextSizeXs.IsReallyChecked())
            {
                _patch.SelectedTextSize = SetListSlot.TextSize.Xs;
            }
            else if (radioButtonTextSizeS.IsReallyChecked())
            {
                _patch.SelectedTextSize = SetListSlot.TextSize.S;
            }
            else if (radioButtonTextSizeM.IsReallyChecked())
            {
                _patch.SelectedTextSize = SetListSlot.TextSize.M;
            }
            else if (radioButtonTextSizeL.IsReallyChecked())
            {
                _patch.SelectedTextSize = SetListSlot.TextSize.L;
            }
            else if (radioButtonTextSizeXl.IsReallyChecked())
            {
                _patch.SelectedTextSize = SetListSlot.TextSize.Xl;
            }
            else
            {
                throw new ApplicationException("Illegal text size");
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonTextSize_Checked(object sender, RoutedEventArgs e)
        {
            //var fontFamily = "Arial";
            var fontSize = -1;
            //var fontWeight = FontWeights.Normal;

            if (radioButtonTextSizeXl.IsReallyChecked())
            {
                fontSize = 44;
                // According to Korg:
                // fontSize = 48;
                // fontWeight = FontWeights.Bold;
            }
            else if (radioButtonTextSizeL.IsReallyChecked())
            {
                fontSize = 20;
                // According to Korg:
                // fontSize = 22;
                // fontWeight = FontWeights.Bold;
            }
            else if (radioButtonTextSizeM.IsReallyChecked())
            {
                fontSize = 13;
                // According to Korg:
                // fontFamily = "Helvetica";
                // fontSize = 18;
                // fontWeight = FontWeights.Bold;
            }
            else if (radioButtonTextSizeS.IsReallyChecked())
            {
                fontSize = 10;
                // According to Korg:
                // fontFamily = "Verdana";
                // fontSize = 10;
                // fontWeight = FontWeights.Bold;
            }
            else if (radioButtonTextSizeXs.IsReallyChecked())
            {
                fontSize = 9;
                // According to Korg:
                // fontFamily = "Verdana";
                // fontSize = 8;
                // fontWeight = FontWeights.Bold;

            }

            // textBoxDescription.FontFamily = new System.Windows.Media.FontFamily(fontFamily);
            textBoxDescription.FontSize = fontSize;
            // textBoxDescription.FontWeight = fontWeight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((comboBoxColor.SelectedIndex >= 0) && (comboBoxColor.SelectedIndex <= 15))
            {
                var colors = new List<Color>
                {
                    new Color {A = 255, R = 77, G = 77, B = 77}, // Default
                    new Color {A = 255, R = 47, G = 47, B = 47}, // Charcoal
                    new Color {A = 255, R = 178, G = 63, B = 63}, // Brick
                    new Color {A = 255, R = 105, G = 27, B = 27}, // Burgundy
                    new Color {A = 255, R = 145, G = 167, B = 48}, // Ivy
                    new Color {A = 255, R = 55, G = 69, B = 32}, // Olive
                    new Color {A = 255, R = 170, G = 132, B = 42}, // Gold
                    new Color {A = 255, R = 127, G = 66, B = 54}, // Cacao
                    new Color {A = 255, R = 83, G = 96, B = 165}, // Indigo
                    new Color {A = 255, R = 26, G = 43, B = 136}, // Navy
                    new Color {A = 255, R = 171, G = 129, B = 162}, // Rose
                    new Color {A = 255, R = 146, G = 103, B = 186}, // Lavender
                    new Color {A = 255, R = 136, G = 164, B = 197}, // Azure
                    new Color {A = 255, R = 106, G = 127, B = 150}, // Denim
                    new Color {A = 255, R = 128, G = 128, B = 128}, // Silver
                    new Color {A = 255, R = 98, G = 98, B = 98}, //  Slate
                };

                textBoxColor.Background = new SolidColorBrush(colors[comboBoxColor.SelectedIndex]);
            }
        }
        
        /*
         Buttons to test the width/font of the description control.
        private void ButtonPlusClick(object sender, RoutedEventArgs e)
        {
            textBoxDescription.Width++;
            TextBoxReferenceId.Text = textBoxDescription.Width.ToString();
        }

        private void ButtonMinusClick(object sender, RoutedEventArgs e)
        {
            textBoxDescription.Width--;
            TextBoxReferenceId.Text = textBoxDescription.Width.ToString();
        }
        */

    }
}
