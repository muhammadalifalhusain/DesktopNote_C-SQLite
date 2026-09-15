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
            Type = NoteType.Note,
            Color = NoteColor.Butter,
            IsPinned = true,
            IsArchived = false,
            IsCompleted = false,
            ReminderAt = null,
            CreatedAt = DateTime.Now.AddDays(-2),
            UpdatedAt = DateTime.Now.AddHours(-1)
        });

        Notes.Add(new Note
        {
            Id = 2,
            Title = "Belanja Mingguan",
            Content = "Beli susu, roti, kopi, telur, dan sabun.",
            Type = NoteType.Checklist,
            Color = NoteColor.Yellow,
            IsPinned = false,
            IsArchived = false,
            IsCompleted = false,
            ReminderAt = null,
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = DateTime.Now.AddHours(-2)
        });

        Notes.Add(new Note
        {
            Id = 3,
            Title = "Selesaikan UI Dashboard",
            Content = "Merapikan tampilan dashboard dan menyelesaikan bagian card note.",
            Type = NoteType.Task,
            Color = NoteColor.Butter,
            IsPinned = true,
            IsArchived = false,
            IsCompleted = false,
            ReminderAt = null,
            CreatedAt = DateTime.Now.AddDays(-3),
            UpdatedAt = DateTime.Now.AddMinutes(-30)
        });

        Notes.Add(new Note
        {
            Id = 4,
            Title = "Meeting dengan Tim",
            Content = "Membahas progress aplikasi DesktopNote dan pembagian tugas berikutnya.",
            Type = NoteType.Meeting,
            Color = NoteColor.Yellow,
            IsPinned = false,
            IsArchived = false,
            IsCompleted = false,
            ReminderAt = null,
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = DateTime.Now.AddHours(-3)
        });

        Notes.Add(new Note
        {
            Id = 5,
            Title = "Pengingat",
            Content = "Jangan lupa mengirim laporan sebelum sore.",
            Type = NoteType.Reminder,
            Color = NoteColor.Butter,
            IsPinned = false,
            IsArchived = false,
            IsCompleted = false,
            ReminderAt = DateTime.Now.AddHours(3),
            CreatedAt = DateTime.Now.AddHours(-5),
            UpdatedAt = DateTime.Now.AddHours(-1)
        });

        Notes.Add(new Note
        {
            Id = 6,
            Title = "Ide Fitur Baru",
            Content = "Tambahkan fitur pencarian, filter berdasarkan tipe, dan shortcut keyboard.",
            Type = NoteType.Idea,
            Color = NoteColor.Yellow,
            IsPinned = false,
            IsArchived = false,
            IsCompleted = false,
            ReminderAt = null,
            CreatedAt = DateTime.Now.AddDays(-4),
            UpdatedAt = DateTime.Now.AddHours(-4)
        });

        Notes.Add(new Note
        {
            Id = 7,
            Title = "Task Selesai",
            Content = "Membuat struktur model Note dan menambahkan dummy data.",
            Type = NoteType.Task,
            Color = NoteColor.Butter,
            IsPinned = false,
            IsArchived = false,
            IsCompleted = true,
            ReminderAt = null,
            CreatedAt = DateTime.Now.AddDays(-5),
            UpdatedAt = DateTime.Now.AddDays(-1)
        });

        SortNotes();
    }
    public Note AddNote(
        string title,
        string content,
        NoteType type = NoteType.Note)
        {
            int nextId = Notes.Count == 0
                ? 1
                : Notes.Max(note => note.Id) + 1;

            var note = new Note
            {
                Id = nextId,
                Title = title,
                Content = content,
                Type = type,
                Color = NoteColor.Butter,
                IsPinned = false,
                IsArchived = false,
                IsCompleted = false,
                ReminderAt = null,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            Notes.Add(note);

            SortNotes();

            return note;
    }

    public void UpdateNote(
        Note note,
        string title,
        string content,
        NoteType type)
    {
        note.Title = title;
        note.Content = content;
        note.Type = type;
        note.UpdatedAt = DateTime.Now;

        SortNotes();
    }

    public void TogglePin(Note note)
    {
        note.IsPinned = !note.IsPinned;
        note.UpdatedAt = DateTime.Now;

        SortNotes();
    }

    public void ToggleCompleted(Note note)
    {
        note.IsCompleted = !note.IsCompleted;
        note.UpdatedAt = DateTime.Now;

        SortNotes();
    }

    public void SetReminder(Note note, DateTime? reminderAt)
    {
        note.ReminderAt = reminderAt;
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