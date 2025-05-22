using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageImporter.Models;

namespace ImageImporterUI.ViewModels;

public partial class NumbersViewModel(MainViewModel main) : ObservableObject
{
    public string Header { get; private set; } = "Numbers";

    [ObservableProperty]
    private ObservableCollection<Number> numbers = [];

    public void Update()
    {
        Numbers = new ObservableCollection<Number>(main.puzzle.Numbers);
    }
}
