
using DesktopNote.Data;
using DesktopNote.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DesktopNote.ViewModels;

public class MainViewModel
{
    private readonly NoteDatabase _database;

    public ObservableCollection<Note> Notes { get; }
    public ObservableCollection<Note> ActiveNotes { get; }

    public MainViewModel()
    {
        _database = new NoteDatabase();

        var notes = _database
            .GetNotes()
            .ToList();

        Notes = new ObservableCollection<Note>(
            notes);

        ActiveNotes = new ObservableCollection<Note>(
            notes
                .Where(note => !note.IsCompleted)
                .OrderByDescending(note => note.IsPinned)
                .ThenByDescending(note => note.UpdatedAt));
    }

    public Note AddNote(
        string title,
        string content,
        NoteType type,
        NoteColor color)
    {
        var note = new Note
        {
            Title = title,
            Content = content,
            Type = type,
            Color = color,
            IsPinned = false,
            IsArchived = false,
            IsCompleted = false,
            ReminderAt = null,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _database.InsertNote(note);

        Notes.Add(note);
        ActiveNotes.Add(note);

        SortNotes();
        SortActiveNotes();

        return note;
    }

    public void UpdateNote(
        Note note,
        string title,
        string content,
        NoteType type,
        NoteColor color)
    {
        note.Title = title;
        note.Content = content;
        note.Type = type;
        note.Color = color;
        note.UpdatedAt = DateTime.Now;

        _database.UpdateNote(note);

        SortNotes();
        SortActiveNotes();
    }

    public void UpdateColor(
        Note note,
        NoteColor color)
    {
        note.Color = color;
        note.UpdatedAt = DateTime.Now;

        _database.UpdateNote(note);

        SortNotes();
        SortActiveNotes();
    }

    public void TogglePin(
        Note note)
    {
        note.IsPinned = !note.IsPinned;
        note.UpdatedAt = DateTime.Now;

        _database.UpdateNote(note);

        SortNotes();
        SortActiveNotes();
    }

    public void ToggleCompleted(
        Note note)
    {
        note.IsCompleted = !note.IsCompleted;
        note.UpdatedAt = DateTime.Now;

        _database.UpdateNote(note);

        if (note.IsCompleted)
        {
            ActiveNotes.Remove(note);
        }
        else if (!ActiveNotes.Contains(note))
        {
            ActiveNotes.Add(note);
        }

        SortNotes();
        SortActiveNotes();
    }

    public void SetReminder(
        Note note,
        DateTime? reminderAt)
    {
        note.ReminderAt = reminderAt;
        note.UpdatedAt = DateTime.Now;

        _database.UpdateNote(note);

        SortNotes();
        SortActiveNotes();
    }

    public void DeleteNote(
        Note note)
    {
        _database.DeleteNote(note.Id);

        Notes.Remove(note);
        ActiveNotes.Remove(note);
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

    private void SortActiveNotes()
    {
        var sortedNotes = ActiveNotes
            .Where(note => !note.IsCompleted)
            .OrderByDescending(note => note.IsPinned)
            .ThenByDescending(note => note.UpdatedAt)
            .ToList();

        ActiveNotes.Clear();

        foreach (var note in sortedNotes)
        {
            ActiveNotes.Add(note);
        }
    }
}
