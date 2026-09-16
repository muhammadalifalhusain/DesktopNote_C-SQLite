using Microsoft.UI.Xaml.Data;
using System;

namespace DesktopNote.Converters;

public class DateTimeToStringConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        string language)
    {
        if (value is DateTime dateTime)
            return dateTime.ToString("dd MMMM yyyy HH:mm");

        return string.Empty;
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        string language)
    {
        throw new NotImplementedException();
    }
}