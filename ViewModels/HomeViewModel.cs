namespace VideoLinkSaver.ViewModels
{
    /// <summary>
    /// ViewModel for the Home screen.
    /// Exposes navigation commands for the three main options.
    /// </summary>
    public class HomeViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;

        public RelayCommand GoToCreateLinkCommand { get; }
        public RelayCommand GoToSearchCommand { get; }
        public RelayCommand GoToDataLocationCommand { get; }

        public HomeViewModel(MainViewModel main)
        {
            _main = main;

            GoToCreateLinkCommand = new RelayCommand(() => _main.GoToCreateLink());
            GoToSearchCommand = new RelayCommand(() => _main.GoToSearch());
            GoToDataLocationCommand = new RelayCommand(() => _main.GoToDataLocation());
        }
    }
}
