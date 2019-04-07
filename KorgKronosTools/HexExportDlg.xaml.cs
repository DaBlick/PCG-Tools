using System.Windows;

namespace PcgTools
{
    /// <summary>
    /// Interaction logic for HexExportDlg.xaml
    /// </summary>
    public partial class HexExportDlg : Window
    {
        public HexExportDlg(string text)
        {
            InitializeComponent();
            TextBlock.Text = text;
        }
    }
}
