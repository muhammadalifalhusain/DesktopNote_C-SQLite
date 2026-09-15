using DesktopNote.Models;
using System.Collections.ObjectModel;

namespace DesktopNote.ViewModels;

public class MainViewModel
{
    public ObservableCollection<Note> Notes { get; set; }

    public MainViewModel()
    {
        Notes = new ObservableCollection<Note>();
    }
}