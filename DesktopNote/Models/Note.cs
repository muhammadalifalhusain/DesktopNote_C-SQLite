using System;
using System.ComponentModel;

namespace DesktopNote.Models;

public class Note : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private string _content = string.Empty;
    private NoteType _type = NoteType.NOTE;
    private NoteColor _color = NoteColor.Butter;
    private bool _isPinned;
    private bool _isArchived;
    private bool _isCompleted;
    private DateTime? _reminderAt;
    private DateTime _updatedAt;

    public int Id { get; set; }

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value)
                return;

            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    public string Content
    {
        get => _content;
        set
        {
            if (_content == value)
                return;

            _content = value;
            OnPropertyChanged(nameof(Content));
        }
    }

    public NoteType Type
    {
        get => _type;
        set
        {
            if (_type == value)
                return;

            _type = value;
            OnPropertyChanged(nameof(Type));
        }
    }

    public NoteColor Color
    {
        get => _color;
        set
        {
            if (_color == value)
                return;

            _color = value;
            OnPropertyChanged(nameof(Color));
        }
    }

    public bool IsPinned
    {
        get => _isPinned;
        set
        {
            if (_isPinned == value)
                return;

            _isPinned = value;
            OnPropertyChanged(nameof(IsPinned));
        }
    }

    public bool IsArchived
    {
        get => _isArchived;
        set
        {
            if (_isArchived == value)
                return;

            _isArchived = value;
            OnPropertyChanged(nameof(IsArchived));
        }
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (_isCompleted == value)
                return;

            _isCompleted = value;
            OnPropertyChanged(nameof(IsCompleted));
        }
    }

    public DateTime? ReminderAt
    {
        get => _reminderAt;
        set
        {
            if (_reminderAt == value)
                return;

            _reminderAt = value;
            OnPropertyChanged(nameof(ReminderAt));
        }
    }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt
    {
        get => _updatedAt;
        set
        {
            if (_updatedAt == value)
                return;

            _updatedAt = value;
            OnPropertyChanged(nameof(UpdatedAt));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}