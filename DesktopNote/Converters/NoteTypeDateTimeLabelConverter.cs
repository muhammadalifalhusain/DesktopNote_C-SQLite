using DesktopNote.Models;
using Microsoft.UI.Xaml.Data;
using System;

namespace DesktopNote.Converters;

public class NoteTypeDateTimeLabelConverter : IValueConverter
{
    public object Convert(
    object value,
    Type targetType,
    object parameter,
    string language)
    {
        if (value is not NoteType type)
            return string.Empty;
    return type switch
    {
        NoteType.TASK => "Tenggat",
        NoteType.RMNDR => "Pengingat",
        NoteType.MTING => "Jadwal Meeting",
        _ => string.Empty
    };
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
