using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ActiviaAPP.Classes
{
    // Multi-value converter: expects [0] = ObservableCollection<string> RegisteredUsers, [1] = current user id (string)
    public class RegisteredToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return Visibility.Collapsed;

            var list = values[0] as ObservableCollection<string>;
            var currentUserId = values[1] as string;

            if (list == null || string.IsNullOrWhiteSpace(currentUserId))
                return Visibility.Collapsed;

            return list.Contains(currentUserId) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}