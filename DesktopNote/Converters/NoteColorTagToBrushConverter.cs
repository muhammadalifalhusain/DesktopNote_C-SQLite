using DesktopNote.Models;
using Microsoft.UI.Xaml.Data;
using System;

namespace DesktopNote.Converters;

public class NoteColorTagToBrushConverter : IValueConverter
{
    private static readonly NoteColorToBrushConverter BaseConverter = new();

    public object Convert(
        object value,
        Type targetType,
        object parameter,
        string language)
    {
        if (value is string tag &&
            Enum.TryParse<NoteColor>(tag, out var color))
        {
            return BaseConverter.Convert(
                color,
                targetType,
                parameter,
                language);
        }

        return BaseConverter.Convert(
            NoteColor.Butter,
            targetType,
            parameter,
            language);
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