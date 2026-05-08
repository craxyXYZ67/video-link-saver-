using System.Collections.ObjectModel;
using VideoLinkSaver.Models;

namespace VideoLinkSaver.ViewModels
{
    /// <summary>
    /// ViewModel for the Search screen.
    /// Loads all records at startup and applies live, stacking filters.
    /// Results are always sorted newest-first (by row index).
    /// </summary>
    public class SearchViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;
        private List<VideoLink> _allLinks = new();

        // ---------------------------------------------------------------
        // Filter fields — updating any one re-runs the filter
        // ---------------------------------------------------------------
        private string _filterPlatform = string.Empty;
        public string FilterPlatform
        {
            get => _filterPlatform;
            set { SetProperty(ref _filterPlatform, value); ApplyFilters(); }
        }

        private string _filterPurpose = string.Empty;
        public string FilterPurpose
        {
            get => _filterPurpose;
            set { SetProperty(ref _filterPurpose, value); ApplyFilters(); }
        }

        private string _filterCategory = string.Empty;
        public string FilterCategory
        {
            get => _filterCategory;
            set { SetProperty(ref _filterCategory, value); ApplyFilters(); }
        }

        private string _filterChannel = string.Empty;
        public string FilterChannel
        {
            get => _filterChannel;
            set { SetProperty(ref _filterChannel, value); ApplyFilters(); }
        }

        // ---------------------------------------------------------------
        // Results
        // ---------------------------------------------------------------
        public ObservableCollection<VideoLink> FilteredLinks { get; } = new();

        // ---------------------------------------------------------------
        // State
        // ---------------------------------------------------------------
        private bool _isLoading;
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

        private string _statusMessage = string.Empty;
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        // ---------------------------------------------------------------
        // Commands
        // ---------------------------------------------------------------
        public RelayCommand BackCommand { get; }
        public RelayCommand OpenLinkCommand { get; }

        // ---------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------
        public SearchViewModel(MainViewModel main)
        {
            _main = main;

            BackCommand = new RelayCommand(() => _main.GoHome());

            OpenLinkCommand = new RelayCommand(param =>
            {
                if (param is VideoLink link && !string.IsNullOrWhiteSpace(link.LinkOfVideo))
                {
                    try
                    {
                        // Open the URL in the user's default browser
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = link.LinkOfVideo,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[SearchViewModel] OpenLink failed: {ex.Message}");
                    }
                }
            });

            // Load data asynchronously on creation
            _ = LoadDataAsync();
        }

        // ---------------------------------------------------------------
        // Load all data from the Excel file
        // ---------------------------------------------------------------
        private async Task LoadDataAsync()
        {
            IsLoading = true;
            StatusMessage = "Loading...";
            try
            {
                _allLinks = await _main.ExcelService.LoadAllLinksAsync();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ---------------------------------------------------------------
        // Apply all active filters cumulatively
        // ---------------------------------------------------------------
        private void ApplyFilters()
        {
            var results = _allLinks.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(FilterPlatform))
                results = results.Where(l =>
                    l.Platform.Contains(FilterPlatform, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(FilterPurpose))
                results = results.Where(l =>
                    l.Purpose.Contains(FilterPurpose, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(FilterCategory))
                results = results.Where(l =>
                    l.Category.Contains(FilterCategory, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(FilterChannel))
                results = results.Where(l =>
                    l.NameOfChannel.Contains(FilterChannel, StringComparison.OrdinalIgnoreCase));

            FilteredLinks.Clear();
            foreach (var item in results)
                FilteredLinks.Add(item);

            StatusMessage = FilteredLinks.Count == 0
                ? "No results found."
                : $"{FilteredLinks.Count} result{(FilteredLinks.Count == 1 ? "" : "s")} found.";
        }
    }
}
