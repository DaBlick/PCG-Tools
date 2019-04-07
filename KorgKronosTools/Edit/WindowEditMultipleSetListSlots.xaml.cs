// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Windows;
using System.Windows.Controls;

using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.PcgToolsResources;

namespace PcgTools.Edit
{
    /// <summary>
    /// Interaction logic for WindowEdit.xaml
    /// </summary>
    public partial class WindowEditMultipleSetListSlot // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        readonly ISetListSlot _patch;


        /// <summary>
        /// 
        /// </summary>
        bool _ok = true;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        public WindowEditMultipleSetListSlot(ISetListSlot patch)
        {
            InitializeComponent();
            _patch = patch;
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
            
            textBoxDescription.Text = _patch.Description;
            intupdownVolume.Value = _patch.Volume;
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
            labelNameLength.Content = string.Format(Strings.XOfYCharacters_editw, usedSize, _patch.MaxNameLength);
            labelError.Content = EditUtils.CheckText(textBoxName.Text, _patch.MaxNameLength, EditUtils.ECheckType.Name);
        
            // Check set list slot description.
            if (labelError.Content.Equals(string.Empty))
            {
                // Set length.
                var usedDescriptionSize = textBoxDescription.Text.Length;
                var maxDescriptionLength = _patch.MaxDescriptionLength;
                labelDescriptionLength.Text =
                    $"{usedDescriptionSize} of {maxDescriptionLength} characters";

                // Check description.
                labelError.Content = EditUtils.CheckText(
                    textBoxDescription.Text, _patch.MaxDescriptionLength, EditUtils.ECheckType.Description);
            }

            _ok = labelError.Content.Equals(string.Empty);
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

                // Notify others.
                _patch.Update("ContentChanged");
            }

            DialogResult = true;
            Close();
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
