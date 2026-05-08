using System.IO;
using System.Windows;

namespace VideoLinkSaver.Services
{
    /// <summary>
    /// Manages light/dark theme switching and persists the user's preference
    /// using application settings stored in a local JSON file.
    /// </summary>
    public class ThemeService
    {
        private const string SettingsFileName = "settings.json";
        private readonly string _settingsPath;

        public bool IsDarkMode { get; private set; }

        public ThemeService(ExcelService excelService)
        {
            _settingsPath = Path.Combine(excelService.FolderPath, SettingsFileName);
            IsDarkMode = LoadThemePreference();
        }

        /// <summary>
        /// Applies the current theme to the application resource dictionary.
        /// </summary>
        public void ApplyTheme()
        {
            var dict = Application.Current.Resources.MergedDictionaries;

            // Remove any existing theme dictionaries
            var toRemove = dict
                .Where(d => d.Source != null &&
                       (d.Source.OriginalString.Contains("LightTheme") ||
                        d.Source.OriginalString.Contains("DarkTheme")))
                .ToList();
            foreach (var d in toRemove)
                dict.Remove(d);

            // Add the correct theme
            string themeName = IsDarkMode ? "DarkTheme" : "LightTheme";
            dict.Add(new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/Themes/{themeName}.xaml")
            });
        }

        /// <summary>
        /// Toggles between dark and light mode, applies the theme, and saves the preference.
        /// </summary>
        public void Toggle()
        {
            IsDarkMode = !IsDarkMode;
            ApplyTheme();
            SaveThemePreference();
        }

        private bool LoadThemePreference()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    // Simple parsing without external JSON lib
                    return json.Contains("\"isDarkMode\":true");
                }
            }
            catch { /* Default to light mode on any error */ }
            return false;
        }

        private void SaveThemePreference()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
                var json = $"{{\"isDarkMode\":{IsDarkMode.ToString().ToLower()}}}";
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ThemeService] SaveThemePreference failed: {ex.Message}");
            }
        }
    }
}
