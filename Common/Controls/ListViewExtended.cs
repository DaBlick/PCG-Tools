using System.Windows.Controls;

namespace Common.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class ListViewExtended : ListView
    {
        /// <summary>
        /// 
        /// </summary>
        public ListViewExtended()
        {
            SelectionChanged += ComboBoxExtended_SelectionChanged;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ComboBoxExtended_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScrollIntoView(SelectedItem);
        }
    }
}
