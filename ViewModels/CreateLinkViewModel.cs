using VideoLinkSaver.Models;

namespace VideoLinkSaver.ViewModels
{
    /// <summary>
    /// ViewModel for the Create Link screen.
    /// Handles input, validation, and saving to the Excel file.
    /// </summary>
    public class CreateLinkViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;

        // ---------------------------------------------------------------
        // Bound input fields
        // ---------------------------------------------------------------
        private string _linkOfVideo = string.Empty;
        public string LinkOfVideo
        {
            get => _linkOfVideo;
            set
            {
                SetProperty(ref _linkOfVideo, value);
                LinkError = string.Empty;
            }
        }

        private string _platform = string.Empty;
        public string Platform
        {
            get => _platform;
            set
            {
                SetProperty(ref _platform, value);
                PlatformError = string.Empty;
            }
        }

        private string _nameOfChannel = string.Empty;
        public string NameOfChannel
        {
            get => _nameOfChannel;
            set
            {
                SetProperty(ref _nameOfChannel, value);
                ChannelError = string.Empty;
            }
        }

        private string _purpose = string.Empty;
        public string Purpose
        {
            get => _purpose;
            set
            {
                SetProperty(ref _purpose, value);
                PurposeError = string.Empty;
            }
        }

        private string _category = string.Empty;
        public string Category
        {
            get => _category;
            set
            {
                SetProperty(ref _category, value);
                CategoryError = string.Empty;
            }
        }

        // ---------------------------------------------------------------
        // Validation error messages
        // ---------------------------------------------------------------
        private string _linkError = string.Empty;
        public string LinkError { get => _linkError; set => SetProperty(ref _linkError, value); }

        private string _platformError = string.Empty;
        public string PlatformError { get => _platformError; set => SetProperty(ref _platformError, value); }

        private string _channelError = string.Empty;
        public string ChannelError { get => _channelError; set => SetProperty(ref _channelError, value); }

        private string _purposeError = string.Empty;
        public string PurposeError { get => _purposeError; set => SetProperty(ref _purposeError, value); }

        private string _categoryError = string.Empty;
        public string CategoryError { get => _categoryError; set => SetProperty(ref _categoryError, value); }

        // ---------------------------------------------------------------
        // Success notification
        // ---------------------------------------------------------------
        private bool _showSuccess;
        public bool ShowSuccess { get => _showSuccess; set => SetProperty(ref _showSuccess, value); }

        // ---------------------------------------------------------------
        // Busy state (disables Save button while writing)
        // ---------------------------------------------------------------
        private bool _isSaving;
        public bool IsSaving { get => _isSaving; set => SetProperty(ref _isSaving, value); }

        // ---------------------------------------------------------------
        // Commands
        // ---------------------------------------------------------------
        public RelayCommand SaveCommand { get; }
        public RelayCommand BackCommand { get; }

        // ---------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------
        public CreateLinkViewModel(MainViewModel main)
        {
            _main = main;

            SaveCommand = new RelayCommand(
                async _ => await SaveAsync(),
                _ => !IsSaving);

            BackCommand = new RelayCommand(() => _main.GoHome());
        }

        // ---------------------------------------------------------------
        // Validation
        // ---------------------------------------------------------------
        private bool Validate()
        {
            bool valid = true;

            if (string.IsNullOrWhiteSpace(LinkOfVideo))
            {
                LinkError = "Video link is required.";
                valid = false;
            }
            else if (!Uri.TryCreate(LinkOfVideo.Trim(), UriKind.Absolute, out var uri)
                     || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                LinkError = "Please enter a valid URL (starting with http:// or https://).";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(Platform))
            {
                PlatformError = "Platform is required.";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(NameOfChannel))
            {
                ChannelError = "Channel name is required.";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(Purpose))
            {
                PurposeError = "Purpose is required.";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(Category))
            {
                CategoryError = "Category is required.";
                valid = false;
            }

            return valid;
        }

        // ---------------------------------------------------------------
        // Save to Excel
        // ---------------------------------------------------------------
        private async Task SaveAsync()
        {
            if (!Validate()) return;

            IsSaving = true;
            try
            {
                var link = new VideoLink
                {
                    LinkOfVideo = LinkOfVideo.Trim(),
                    Platform = Platform.Trim(),
                    NameOfChannel = NameOfChannel.Trim(),
                    Purpose = Purpose.Trim(),
                    Category = Category.Trim()
                };

                await _main.ExcelService.SaveLinkAsync(link);

                // Show success notification briefly, then go home
                ShowSuccess = true;
                await Task.Delay(1500);
                _main.GoHome();
            }
            catch (Exception ex)
            {
                LinkError = $"Save failed: {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }
    }
}
