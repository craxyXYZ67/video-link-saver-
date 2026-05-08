using VideoLinkSaver.Services;

namespace VideoLinkSaver.ViewModels
{
    /// <summary>
    /// Root ViewModel that owns navigation state and shared services.
    /// All child ViewModels receive services through the constructor.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        // ---------------------------------------------------------------
        // Services (shared singletons)
        // ---------------------------------------------------------------
        public ExcelService ExcelService { get; }
        public ThemeService ThemeService { get; }

        // ---------------------------------------------------------------
        // Navigation state
        // ---------------------------------------------------------------
        private BaseViewModel _currentView;
        public BaseViewModel CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        // ---------------------------------------------------------------
        // Theme toggle command (accessible from every view via MainViewModel)
        // ---------------------------------------------------------------
        public RelayCommand ToggleThemeCommand { get; }

        private bool _isDarkMode;
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set => SetProperty(ref _isDarkMode, value);
        }

        // ---------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------
        public MainViewModel()
        {
            ExcelService = new ExcelService();
            ThemeService = new ThemeService(ExcelService);

            // Ensure the Excel file exists right at startup
            ExcelService.EnsureFileExists();

            // Apply the saved theme preference
            ThemeService.ApplyTheme();
            _isDarkMode = ThemeService.IsDarkMode;

            // Set initial view to Home
            _currentView = new HomeViewModel(this);

            ToggleThemeCommand = new RelayCommand(() =>
            {
                ThemeService.Toggle();
                IsDarkMode = ThemeService.IsDarkMode;
            });
        }

        // ---------------------------------------------------------------
        // Navigation helpers
        // ---------------------------------------------------------------
        public void NavigateTo(BaseViewModel viewModel)
        {
            CurrentView = viewModel;
        }

        public void GoHome()
        {
            CurrentView = new HomeViewModel(this);
        }

        public void GoToCreateLink()
        {
            CurrentView = new CreateLinkViewModel(this);
        }

        public void GoToSearch()
        {
            CurrentView = new SearchViewModel(this);
        }

        public void GoToDataLocation()
        {
            CurrentView = new DataLocationViewModel(this);
        }
    }
}
