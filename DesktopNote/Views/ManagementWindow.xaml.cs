using DesktopNote.Models;
using DesktopNote.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DesktopNote.Views;

public sealed partial class ManagementWindow : Window
{
    private readonly MainViewModel _viewModel;

    private readonly ObservableCollection<Note> _filteredNotes =
        new ObservableCollection<Note>();

    private string _selectedFilter = "ALL";

    public ManagementWindow(
        MainViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        NotesList.ItemsSource =
            _filteredNotes;

        _viewModel.Notes.CollectionChanged +=
            Notes_CollectionChanged;

        RefreshNotes();

        Closed += ManagementWindow_Closed;
    }

    private void Notes_CollectionChanged(
        object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RefreshNotes();
    }

    private void RefreshNotes()
    {
        string searchText =
            SearchTextBox?.Text?.Trim() ?? string.Empty;

        var notes =
            _viewModel.Notes.AsEnumerable();

        if (_selectedFilter != "ALL")
        {
            if (Enum.TryParse<NoteType>(
                _selectedFilter,
                out NoteType selectedType))
            {
                notes =
                    notes.Where(
                        note => note.Type == selectedType);
            }
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            notes =
                notes.Where(
                    note =>
                        note.Title.Contains(
                            searchText,
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        note.Content.Contains(
                            searchText,
                            StringComparison.OrdinalIgnoreCase));
        }

        var result =
            notes.ToList();

        _filteredNotes.Clear();

        foreach (var note in result)
        {
            _filteredNotes.Add(note);
        }

        EmptyTextBlock.Visibility =
            _filteredNotes.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
    }

    private void SearchTextBox_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        RefreshNotes();
    }

    private void Filter_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.Tag is not string filter)
            return;

        _selectedFilter =
            filter;

        RefreshNotes();
    }

    private void NewNote_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window =
            new NoteWindow(_viewModel);

        window.Activate();
    }

    private void EditNote_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.Tag is not Note note)
            return;

        var window =
            new NoteWindow(
                _viewModel,
                note);

        window.Activate();
    }

    private void DeleteNote_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.Tag is not Note note)
            return;

        _viewModel.DeleteNote(note);
    }

    private void ManagementWindow_Closed(
        object sender,
        WindowEventArgs args)
    {
        _viewModel.Notes.CollectionChanged -=
            Notes_CollectionChanged;
    }
}