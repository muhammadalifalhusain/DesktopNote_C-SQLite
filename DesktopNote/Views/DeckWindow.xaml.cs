
using DesktopNote.Helpers;
using DesktopNote.Models;
using DesktopNote.Services;
using DesktopNote.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace DesktopNote.Views;

public sealed partial class DeckWindow : Window
{
    public MainViewModel ViewModel { get; } =
        new MainViewModel();

    private Border? _expandedBorder;

    public DeckWindow()
    {
        InitializeComponent();

        WindowService.Configure(this);

        WindowService.SetSize(
            this,
            380,
            700);

        WindowService.MoveToRightCenter(
            this,
            380,
            700);
    }

    private void NewNote_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window =
            new NoteWindow(ViewModel);

        window.Activate();
    }

    private void Note_Tapped(
        object sender,
        TappedRoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
            return;

        if (element.DataContext is not Note note)
            return;

        var window =
            new NoteWindow(
                ViewModel,
                note);

        window.Activate();
    }

    private void NotesList_ContainerContentChanging(
        ListViewBase sender,
        ContainerContentChangingEventArgs args)
    {
        if (args.ItemContainer?
            .ContentTemplateRoot is not Grid grid)
            return;

        if (grid.FindName("NoteCard")
            is not Border border)
            return;

        NoteCardHelper.Reset(border);
    }

    private void Note_PointerEntered(
        object sender,
        PointerRoutedEventArgs e)
    {
        if (sender is not Border border)
            return;

        NoteCardHelper.Expand(
            border,
            _expandedBorder);

        _expandedBorder =
            border;
    }

    private void Note_PointerExited(
        object sender,
        PointerRoutedEventArgs e)
    {
        if (sender is not Border border)
            return;

        if (ReferenceEquals(
            _expandedBorder,
            border))
        {
            _expandedBorder = null;
        }

        NoteCardHelper.Collapse(border);
    }

    private void ChangeHoveredNoteColor(
        KeyboardAccelerator sender,
        KeyboardAcceleratorInvokedEventArgs args)
    {
        if (_expandedBorder is null)
            return;

        if (_expandedBorder.DataContext
            is not Note note)
            return;

        NoteColorService.Next(note);

        args.Handled = true;
    }
}