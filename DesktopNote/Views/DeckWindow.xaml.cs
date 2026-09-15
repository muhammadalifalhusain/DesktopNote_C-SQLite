using DesktopNote.Models;
using DesktopNote.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace DesktopNote.Views
{
    public sealed partial class DeckWindow : Window
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public DeckWindow()
        {
            InitializeComponent();
        }

        private void NewNote_Click(object sender, RoutedEventArgs e)
        {
            var noteWindow = new NoteWindow(ViewModel);
            noteWindow.Activate();
        }

        private void Note_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement element &&
                element.DataContext is Note note)
            {
                var noteWindow = new NoteWindow(ViewModel, note);
                noteWindow.Activate();
            }
        }

        private void Pin_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element &&
                element.Tag is Note note)
            {
                ViewModel.TogglePin(note);
            }
        }
    }
}