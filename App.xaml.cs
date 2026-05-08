using System.Windows;

namespace VideoLinkSaver
{
    /// <summary>
    /// Entry point for the application. Sets up global exception handling.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Catch any unhandled exceptions and show a friendly message
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                MessageBox.Show(
                    $"An unexpected error occurred:\n\n{ex.ExceptionObject}",
                    "Video Link Saver — Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            };

            DispatcherUnhandledException += (s, ex) =>
            {
                MessageBox.Show(
                    $"An unexpected error occurred:\n\n{ex.Exception.Message}",
                    "Video Link Saver — Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                ex.Handled = true;
            };
        }
    }
}
