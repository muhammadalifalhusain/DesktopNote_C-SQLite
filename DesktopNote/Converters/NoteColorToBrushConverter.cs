using DesktopNote.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace DesktopNote.Converters;

public class NoteColorToBrushConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        string language)
    {
        if (value is not NoteColor color)
        {
            return new SolidColorBrush(Colors.White);
        }

        return color switch
        {
            NoteColor.Butter => new SolidColorBrush(
                Color.FromArgb(255, 255, 248, 220)),

            NoteColor.Yellow => new SolidColorBrush(
                Color.FromArgb(255, 255, 243, 176)),

            NoteColor.Green => new SolidColorBrush(
                Color.FromArgb(255, 211, 239, 194)),

            NoteColor.Blue => new SolidColorBrush(
                Color.FromArgb(255, 197, 225, 242)),

            NoteColor.Purple => new SolidColorBrush(
                Color.FromArgb(255, 225, 210, 240)),

            NoteColor.Pink => new SolidColorBrush(
                Color.FromArgb(255, 247, 210, 224)),

            NoteColor.Orange => new SolidColorBrush(
                Color.FromArgb(255, 255, 220, 180)),

            NoteColor.Red => new SolidColorBrush(
                Color.FromArgb(255, 245, 200, 200)),

            _ => new SolidColorBrush(Colors.White)
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