using DesktopNote.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DesktopNote.ViewModels;

public class MainViewModel
{
    public ObservableCollection<Note> Notes { get; set; }

    public MainViewModel()
    {
        Notes = new ObservableCollection<Note>();

        Notes.Add(new Note
        {
            Id = 1,
            Title = "Catatan Pertama",
            Content = "Ini adalah catatan pertama saya.",
            Color = NoteColor.Butter,
            IsPinned = true,
            IsArchived = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });

        Notes.Add(new Note
        {
            Id = 2,
            Title = "Belanja",
            Content = "Beli susu, roti, dan kopi.",
            Color = NoteColor.Yellow,
            IsPinned = false,
            IsArchived = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
    }

    public void AddNote(string title, string content)
    {
        int nextId = Notes.Count == 0
            ? 1
            : Notes.Max(note => note.Id) + 1;

        var note = new Note
        {
            Id = nextId,
            Title = title,
            Content = content,
            Color = NoteColor.Butter,
            IsPinned = false,
            IsArchived = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        Notes.Add(note);

        SortNotes();
    }

    public void UpdateNote(Note note, string title, string content)
    {
        note.Title = title;
        note.Content = content;
        note.UpdatedAt = DateTime.Now;

        SortNotes();
    }

    public void TogglePin(Note note)
    {
        note.IsPinned = !note.IsPinned;
        note.UpdatedAt = DateTime.Now;

        SortNotes();
    }

    private void SortNotes()
    {
        var sortedNotes = Notes
            .OrderByDescending(note => note.IsPinned)
            .ThenByDescending(note => note.UpdatedAt)
            .ToList();

        Notes.Clear();

        foreach (var note in sortedNotes)
        {
            Notes.Add(note);
        }
    }
}