// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

using PcgTools.Model.Common.Synth.Global;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.PcgToolsResources;

namespace PcgTools.Edit
{
    /// <summary>
    /// Interaction logic for WindowEditSingleProgram.xaml
    /// </summary>
    public partial class WindowEditSingleProgram // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        readonly IProgram _patch;


        /// <summary>
        /// 
        /// </summary>
        bool _ok = true;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        public WindowEditSingleProgram(IProgram patch)
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

            if (_patch.Root.AreFavoritesSupported)
            {
                threeStateCheckBoxIsFavorite.IsChecked = _patch.GetParam(
                    ParameterNames.ProgramParameterName.Favorite).Value;
            }
            else
            {
                threeStateCheckBoxIsFavorite.Visibility = Visibility.Hidden;
            }

            // Set control texts for categories.
            if (!_patch.PcgRoot.HasProgramCategories)
            {
                labelCategory.Visibility = Visibility.Hidden;
                comboBoxCategory.Visibility = Visibility.Hidden;
                labelSubCategory.Visibility = Visibility.Hidden;
                comboBoxSubCategory.Visibility = Visibility.Hidden;
                return;
            }

            if (!_patch.PcgRoot.UsesCategoriesAndSubCategories)
            {
                labelCategory.Content = Strings.Genre_control;
                labelSubCategory.Content = Strings.CategoryControl;
            }

            FillCategories();

            Check();
        }


        /// <summary>
        /// IMPR: Duplicated code with other edit windows.
        /// </summary>
        private void FillCategories()
        {
            var global = _patch.PcgRoot.Global;

            if (global == null)
            {
                var masterPcgMemory = MasterFiles.MasterFiles.Instances.FindMasterPcg(_patch.Root.Model);
                if (masterPcgMemory == null)
                {
                    // Only number is shown, therefore disable it.
                    labelCategory.IsEnabled = false;
                    comboBoxCategory.Items.Add(_patch.CategoryAsName); 
                    comboBoxCategory.SelectedIndex = 0;
                    comboBoxCategory.IsEnabled = false;

                    labelSubCategory.IsEnabled = false;
                    if (_patch.PcgRoot.HasSubCategories)
                    {
                        // Only number is shown, therefore disable it.
                        comboBoxSubCategory.Items.Add(_patch.SubCategoryAsName); 
                        comboBoxSubCategory.SelectedIndex = 0;
                        comboBoxSubCategory.IsEnabled = false;
                    }
                }
                else
                {
                    var masterGlobal = masterPcgMemory.Global;
                    FillCategoryComboboxes(masterGlobal);
                }
            }
            else
            {
                if (_patch.PcgRoot.AreCategoriesEditable)
                {
                    FillCategoryComboboxes(global);
                }
                else
                {
                    labelCategory.Visibility = Visibility.Hidden;
                    comboBoxCategory.Visibility = Visibility.Hidden;
                    labelSubCategory.Visibility = Visibility.Hidden;
                    comboBoxSubCategory.Visibility = Visibility.Hidden;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="global"></param>
        void FillCategoryComboboxes(IGlobal global)
        {
            Debug.Assert(global != null);

            foreach (var categoryName in global.GetCategoryNames(Global.ECategoryType.Program))
            {
                comboBoxCategory.Items.Add(categoryName);
            }

            comboBoxCategory.SelectedIndex = _patch.GetParam(ParameterNames.ProgramParameterName.Category).Value;

            if (!_patch.PcgRoot.HasSubCategories)
            {
                labelSubCategory.Visibility = Visibility.Hidden;
                comboBoxSubCategory.Visibility = Visibility.Hidden;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="global"></param>
        void FillSubcategoryCombobox(IGlobal global)
        {
            comboBoxSubCategory.Items.Clear();
            foreach (var subCategoryName in global.GetSubCategoryNames(
             Global.ECategoryType.Program, comboBoxCategory.SelectedIndex))
            {
                comboBoxSubCategory.Items.Add(subCategoryName);
            }

            comboBoxSubCategory.SelectedIndex = _patch.GetParam(ParameterNames.ProgramParameterName.SubCategory).Value;
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
                _patch.MaxNameLength);

            labelError.Content = EditUtils.CheckText(textBoxName.Text, _patch.MaxNameLength, 
                EditUtils.ECheckType.Name);

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

                // Set category.
                var memory = _patch.PcgRoot;
                if ((memory.AreCategoriesEditable) && (comboBoxCategory.IsEnabled))
                {
                    var param = _patch.GetParam(ParameterNames.ProgramParameterName.Category);
                    if (param != null)
                    {
                        param.Value = comboBoxCategory.SelectedIndex;
                        if (_patch.PcgRoot.HasSubCategories)
                        {
                            _patch.GetParam(ParameterNames.ProgramParameterName.SubCategory).Value = 
                                comboBoxSubCategory.SelectedIndex;
                        }
                    }
                }

                // Set favorite.
                if (threeStateCheckBoxIsFavorite.IsVisible && memory.AreFavoritesSupported)
                {
                    _patch.GetParam(ParameterNames.ProgramParameterName.Favorite).Value = 
                        threeStateCheckBoxIsFavorite.IsChecked;
                }

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxCategorySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_patch.PcgRoot.HasSubCategories)
            {
                if (_patch.PcgRoot.Global == null)
                {
                    var masterPcgMemory = MasterFiles.MasterFiles.Instances.FindMasterPcg(_patch.Root.Model);
                    if (masterPcgMemory != null)
                    {
                        var masterGlobal = masterPcgMemory.Global;
                        FillSubcategoryCombobox(masterGlobal);
                    }
                }

                else
                {
                    FillSubcategoryCombobox(_patch.PcgRoot.Global);
                }
            }
        }
    }
}
