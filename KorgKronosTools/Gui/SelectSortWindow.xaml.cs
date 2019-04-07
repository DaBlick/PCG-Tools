using System;
using System.Windows;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchSorting;
using PcgTools.Properties;
using Common.Extensions;

namespace PcgTools.Gui
{
    /// <summary>
    /// Interaction logic for SelectSortWindow.xaml
    /// </summary>
    public partial class SelectSortWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IPcgMemory _memory;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        public SelectSortWindow(IPcgMemory memory)
        {
            InitializeComponent();
            _memory = memory;
        }


        /// <summary>
        /// 
        /// </summary>
        public PatchSorter.SortOrder SortOrder { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // If no split character selected, remove title/artist options.
            if (Settings.Default.Sort_SplitCharacter == string.Empty)
            {
                radioButtonArtistTitleCategory.IsEnabled = false;
                radioButtonTitleArtistCategory.IsEnabled = false;
                radioButtonCategoryArtistTitle.IsEnabled = false;
                radioButtonCategoryTitleArtist.IsEnabled = false;
            }

            WindowLoaded_SelectRadioButtons();
            WindowLoaded_HideRadioButtonsIfNoCombiBanks();
            WindowLoaded_SelectValidRadioButton();
        }


        /// <summary>
        /// Select radio button according setting.
        /// </summary>
        private void WindowLoaded_SelectRadioButtons()
        {
            switch ((PatchSorter.SortOrder)Settings.Default.Sort_Order)
            {
                case PatchSorter.SortOrder.ESortOrderArtistTitleCategory:
                    radioButtonArtistTitleCategory.IsChecked = true;
                    break;

                case PatchSorter.SortOrder.ESortOrderCategoryArtistTitle:
                    radioButtonCategoryArtistTitle.IsChecked = true;
                    break;

                case PatchSorter.SortOrder.ESortOrderCategoryName:
                    radioButtonCategoryName.IsChecked = true;
                    break;

                case PatchSorter.SortOrder.ESortOrderCategoryTitleArtist:
                    radioButtonCategoryTitleArtist.IsChecked = true;
                    break;

                case PatchSorter.SortOrder.ESortOrderNameCategory:
                    radioButtonNameCategory.IsChecked = true;
                    break;

                case PatchSorter.SortOrder.ESortOrderTitleArtistCategory:
                    radioButtonTitleArtistCategory.IsChecked = true;
                    break;

                default:
                    throw new ApplicationException("Illegal case");
            }
        }


        /// <summary>
        /// Hide radio buttons when no combis are used.
        /// </summary>
        private void WindowLoaded_HideRadioButtonsIfNoCombiBanks()
        {
            if (_memory.CombiBanks == null)
            {
                radioButtonTitleArtistCategory.Visibility = Visibility.Hidden;
                radioButtonArtistTitleCategory.Visibility = Visibility.Hidden;
                radioButtonCategoryTitleArtist.Visibility = Visibility.Hidden;
                radioButtonCategoryArtistTitle.Visibility = Visibility.Hidden;

                if (radioButtonTitleArtistCategory.IsReallyChecked() ||
                    radioButtonTitleArtistCategory.IsReallyChecked())
                {
                    radioButtonNameCategory.IsChecked = true;
                }
                else if (radioButtonTitleArtistCategory.IsReallyChecked() ||
                         radioButtonTitleArtistCategory.IsReallyChecked())
                {
                    radioButtonCategoryName.IsChecked = true;
                }
            }
        }


        /// <summary>
        /// In case a radio button is selected that is disabled above, select a valid radio button.
        /// </summary>
        private void WindowLoaded_SelectValidRadioButton()
        {
            if ((radioButtonTitleArtistCategory.IsReallyChecked() ||
                radioButtonArtistTitleCategory.IsReallyChecked()) &&
                string.IsNullOrEmpty(Settings.Default.Sort_SplitCharacter))
            {
                radioButtonNameCategory.IsChecked = true;
            }

            if ((radioButtonCategoryArtistTitle.IsReallyChecked() ||
                radioButtonCategoryTitleArtist.IsReallyChecked()) &&
                string.IsNullOrEmpty(Settings.Default.Sort_SplitCharacter))
            {
                radioButtonCategoryName.IsChecked = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            if (radioButtonNameCategory.IsReallyChecked())
            {
                SortOrder = PatchSorter.SortOrder.ESortOrderNameCategory;
            }
            else if (radioButtonTitleArtistCategory.IsReallyChecked())
            {
                SortOrder = PatchSorter.SortOrder.ESortOrderTitleArtistCategory;
            }
            else if (radioButtonArtistTitleCategory.IsReallyChecked())
            {
                SortOrder = PatchSorter.SortOrder.ESortOrderArtistTitleCategory;
            }
            if (radioButtonCategoryName.IsReallyChecked())
            {
                SortOrder = PatchSorter.SortOrder.ESortOrderCategoryName;
            }
            else if (radioButtonCategoryTitleArtist.IsReallyChecked())
            {
                SortOrder = PatchSorter.SortOrder.ESortOrderCategoryTitleArtist;
            }
            else if (radioButtonCategoryArtistTitle.IsReallyChecked())
            {
                SortOrder = PatchSorter.SortOrder.ESortOrderCategoryArtistTitle;
            }
            
            if (checkBoxRemember.IsReallyChecked())
            {
                Settings.Default.Sort_Order = (int) SortOrder;
                Settings.Default.Save();
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
