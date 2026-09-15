using DesktopNote.Models;
using System;

namespace DesktopNote.Services;

public static class NoteColorService
{
    private static readonly NoteColor[] Colors =
    {
        NoteColor.Butter,
        NoteColor.Yellow,
        NoteColor.Green,
        NoteColor.Blue,
        NoteColor.Purple,
        NoteColor.Pink,
        NoteColor.Orange,
        NoteColor.Red
    };

    public static void Next(Note note)
    {
        int currentIndex =
            Array.IndexOf(
                Colors,
                note.Color);

        int nextIndex =
            currentIndex < 0
                ? 0
                : (currentIndex + 1) % Colors.Length;

        note.Color = Colors[nextIndex];
    }
}