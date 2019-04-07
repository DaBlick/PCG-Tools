using System.Windows;
using Common.Mvvm;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.ViewModels;

namespace PcgTools.Edit
{
    /// <summary>
    /// Interaction logic for WindowEditParameter.xaml
    /// </summary>
    public partial class WindowEditParameter : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public EditParameterViewModel ViewModel { get; private set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public WindowEditParameter(ObservableCollectionEx<IPatch> patches)
        {
            InitializeComponent();
            ViewModel = new EditParameterViewModel(patches);
            DataContext = ViewModel;
        }
    }
}
