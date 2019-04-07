using System.Windows.Controls;

namespace Common.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class ListBoxExtended : ListBox
    {
        /// <summary>
        /// 
        /// </summary>
        public ListBoxExtended()
        {
            SelectionChanged += ListBoxExtended_SelectionChanged;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ListBoxExtended_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScrollIntoView(SelectedItem);
        }
    }
}
