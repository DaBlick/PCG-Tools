using System.Windows;
using PatchDatabaseBackEnd;

namespace PatchDbFrontEnd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            PatchList = new PatchDataList();
                
            InitializeComponent();

            Read();
        }


        /// <summary>
        /// 
        /// </summary>
        public PatchDataList PatchList;


        private void Read()
        {
            PatchDataTextBox.Text = CsvHelper.ReadAsCsv().ToString();
        }

    }
}
