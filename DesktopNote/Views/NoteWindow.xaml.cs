using DesktopNote.Models;
using DesktopNote.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace DesktopNote.Views;

public sealed partial class NoteWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly Note? _note;

    private bool _isInitialized;

    public NoteWindow(
        MainViewModel viewModel)
    {
        InitializeComponent();

        ResizeWindow();

        _viewModel = viewModel;
        _note = null;

        _isInitialized = false;

        TypeComboBox.SelectedIndex =
            (int)NoteType.NOTE;

        ColorComboBox.SelectedIndex =
            (int)NoteColor.Butter;

        DeadlineTimePicker.Time =
            new TimeSpan(17, 0, 0);

        _isInitialized = true;

        UpdateDeadlineVisibility();
    }

    public NoteWindow(
        MainViewModel viewModel,
        Note note)
    {
        InitializeComponent();

        ResizeWindow();

        _viewModel = viewModel;
        _note = note;

        _isInitialized = false;

        TitleTextBox.Text =
            note.Title;

        ContentTextBox.Text =
            note.Content;

        TypeComboBox.SelectedIndex =
            (int)note.Type;

        ColorComboBox.SelectedIndex =
            (int)note.Color;

        if (note.ReminderAt.HasValue)
        {
            DateTime dateTime =
                note.ReminderAt.Value;

            DeadlineDatePicker.Date =
                dateTime;

            DeadlineTimePicker.Time =
                dateTime.TimeOfDay;
        }
        else
        {
            DeadlineTimePicker.Time =
                new TimeSpan(17, 0, 0);
        }

        _isInitialized = true;

        UpdateDeadlineVisibility();
    }

    private void ResizeWindow()
    {
        AppWindow.Resize(
            new Windows.Graphics.SizeInt32(700, 780));
    }

    private void TypeComboBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (!_isInitialized)
            return;

        UpdateDeadlineVisibility();
    }

    private void UpdateDeadlineVisibility()
    {
        if (!_isInitialized)
            return;

        if (TypeComboBox == null ||
            DeadlinePanel == null ||
            DeadlineLabel == null ||
            DeadlineValidationText == null)
        {
            return;
        }

        if (TypeComboBox.SelectedIndex < 0)
            return;

        NoteType type =
            (NoteType)TypeComboBox.SelectedIndex;

        bool showDateTime =
            type == NoteType.TASK ||
            type == NoteType.RMNDR ||
            type == NoteType.MTING;

        DeadlinePanel.Visibility =
            showDateTime
                ? Visibility.Visible
                : Visibility.Collapsed;

        DeadlineValidationText.Visibility =
            Visibility.Collapsed;

        if (!showDateTime)
            return;

        DeadlineLabel.Text =
            type switch
            {
                NoteType.TASK =>
                    "Tenggat",

                NoteType.RMNDR =>
                    "Pengingat",

                NoteType.MTING =>
                    "Jadwal Meeting",

                _ =>
                    "Tanggal dan Jam"
            };
    }

    private bool RequiresDateTime()
    {
        if (TypeComboBox.SelectedIndex < 0)
            return false;

        NoteType type =
            (NoteType)TypeComboBox.SelectedIndex;

        return type == NoteType.TASK ||
               type == NoteType.RMNDR ||
               type == NoteType.MTING;
    }

    private DateTime? GetDateTime()
    {
        if (!DeadlineDatePicker.Date.HasValue)
            return null;

        DateTime date =
            DeadlineDatePicker.Date.Value.Date;

        TimeSpan time =
            DeadlineTimePicker.Time;

        return date.Add(time);
    }

    private void Save_Click(
        object sender,
        RoutedEventArgs e)
    {
        string title =
            TitleTextBox.Text.Trim();

        string content =
            ContentTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            title = "Tanpa Judul";
        }

        if (TypeComboBox.SelectedIndex < 0)
        {
            return;
        }

        NoteType type =
            (NoteType)TypeComboBox.SelectedIndex;

        NoteColor color =
            (NoteColor)ColorComboBox.SelectedIndex;

        DateTime? dateTime =
            GetDateTime();

        if (RequiresDateTime() &&
            !dateTime.HasValue)
        {
            DeadlineValidationText.Text =
                type switch
                {
                    NoteType.TASK =>
                        "Tanggal dan jam tenggat wajib diisi.",

                    NoteType.RMNDR =>
                        "Tanggal dan jam pengingat wajib diisi.",

                    NoteType.MTING =>
                        "Tanggal dan jam meeting wajib diisi.",

                    _ =>
                        "Tanggal dan jam wajib diisi."
                };

            DeadlineValidationText.Visibility =
                Visibility.Visible;

            return;
        }

        if (_note == null)
        {
            Note newNote =
                _viewModel.AddNote(
                    title,
                    content,
                    type,
                    color);

            _viewModel.SetReminder(
                newNote,
                dateTime);
        }
        else
        {
            _viewModel.UpdateNote(
                _note,
                title,
                content,
                type,
                color);

            _viewModel.SetReminder(
                _note,
                dateTime);
        }

        Close();
    }

    private void Cancel_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}