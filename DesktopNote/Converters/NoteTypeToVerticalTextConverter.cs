using DesktopNote.Models;
using Microsoft.UI.Xaml.Data;
using System;
using System.Linq;

namespace DesktopNote.Converters;

public class NoteTypeToVerticalTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not NoteType type)
            return string.Empty;

        string text = type.ToString().ToUpperInvariant();

        return string.Join(
            Environment.NewLine,
            text.Select(character => character.ToString()));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}