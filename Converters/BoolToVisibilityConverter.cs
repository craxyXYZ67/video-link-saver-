using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VideoLinkSaver.Converters
{
    /// <summary>
    /// Converts bool to Visibility. True = Visible, False = Collapsed.
    /// Use ConverterParameter="Invert" to reverse the logic.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolVal = value is bool b && b;
            bool invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) == true;

            if (invert) boolVal = !boolVal;
            return boolVal ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = value is Visibility v && v == Visibility.Visible;
            bool invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) == true;
            return invert ? !isVisible : isVisible;
        }
    }

    /// <summary>
    /// Converts a string to Visibility. Non-empty string = Visible.
    /// </summary>
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasContent = !string.IsNullOrWhiteSpace(value?.ToString());
            bool invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) == true;
            if (invert) hasContent = !hasContent;
            return hasContent ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Converts bool to a string: "True" value -> first param, "False" value -> second param.
    /// Example: ConverterParameter="Save|Saving..." shows "Saving..." when true.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolVal = value is bool b && b;
            var parts = parameter?.ToString()?.Split('|');
            if (parts != null && parts.Length == 2)
                return boolVal ? parts[0] : parts[1];
            return boolVal.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
