using DesktopNote.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace DesktopNote.Converters;

public class NoteTypeDateTimeVisibilityConverter : IValueConverter
{
    public object Convert(
    object value,
    Type targetType,
    object parameter,
    string language)
    {
        if (value is not NoteType type)
            return Visibility.Collapsed;
    return type == NoteType.TASK ||
           type == NoteType.RMNDR ||
           type == NoteType.MTING
        ? Visibility.Visible
        : Visibility.Collapsed;
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
