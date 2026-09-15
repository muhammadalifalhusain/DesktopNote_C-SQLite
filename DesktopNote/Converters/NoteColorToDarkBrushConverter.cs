
using DesktopNote.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace DesktopNote.Converters;

public class NoteColorToDarkBrushConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        string language)
    {
        if (value is not NoteColor color)
            return new SolidColorBrush(Colors.Gray);

        Color baseColor = color switch
        {
            NoteColor.Butter => Color.FromArgb(255, 255, 244, 180),
            NoteColor.Yellow => Color.FromArgb(255, 255, 235, 100),
            NoteColor.Green => Color.FromArgb(255, 170, 220, 140),
            NoteColor.Blue => Color.FromArgb(255, 150, 200, 240),
            NoteColor.Purple => Color.FromArgb(255, 200, 170, 230),
            NoteColor.Pink => Color.FromArgb(255, 245, 175, 200),
            NoteColor.Orange => Color.FromArgb(255, 245, 190, 120),
            NoteColor.Red => Color.FromArgb(255, 240, 140, 140),
            _ => Color.FromArgb(255, 200, 200, 200)
        };

        byte Darken(byte channel)
        {
            return (byte)(channel * 0.82);
        }

        return new SolidColorBrush(
            Color.FromArgb(
                255,
                Darken(baseColor.R),
                Darken(baseColor.G),
                Darken(baseColor.B)));
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