// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Globalization;
using System.Windows;
using System.Windows.Controls;

using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.PcgToolsResources;


namespace PcgTools.Edit
{
    /// <summary>
    /// Interaction logic for WindowEdit.xaml
    /// </summary>
    public partial class WindowEditSingleSetList // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        readonly ISetList _setList;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="setList"></param>
        public WindowEditSingleSetList(ISetList setList)
        {
            InitializeComponent();
            _setList = setList;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowEditLoaded(object sender, RoutedEventArgs e)
        {
            textBoxId.Text = _setList.Id.ToString(CultureInfo.InvariantCulture);
            textBoxName.Text = _setList.Name;

            Check();
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
        private void Check()
        {
            var usedSize = textBoxName.Text.Length;

            labelNameLength.Text = string.Format('(' + Strings.XOfYCharacters_editw + ')', usedSize, 
                _setList.MaxNameLength);

            labelError.Content = EditUtils.CheckText(textBoxName.Text, _setList.MaxNameLength, 
                EditUtils.ECheckType.Name);

            buttonOk.IsEnabled = labelError.Content.Equals(string.Empty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            // Set name.
            _setList.Name = textBoxName.Text;

            _setList.Update("ContentChanged");

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
