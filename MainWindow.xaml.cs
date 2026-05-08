using System.Windows;
using VideoLinkSaver.ViewModels;

namespace VideoLinkSaver
{
    /// <summary>
    /// Code-behind for MainWindow. Thin — all logic lives in MainViewModel.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
