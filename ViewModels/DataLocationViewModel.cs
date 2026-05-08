namespace VideoLinkSaver.ViewModels
{
    /// <summary>
    /// ViewModel for the Data Location screen.
    /// Shows the Excel file path and lets the user open its folder.
    /// </summary>
    public class DataLocationViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;

        public string FilePath { get; }

        public RelayCommand OpenFolderCommand { get; }
        public RelayCommand BackCommand { get; }

        public DataLocationViewModel(MainViewModel main)
        {
            _main = main;
            FilePath = _main.ExcelService.FilePath;

            OpenFolderCommand = new RelayCommand(() =>
            {
                _main.ExcelService.OpenFolderInExplorer();
            });

            BackCommand = new RelayCommand(() => _main.GoHome());
        }
    }
}
