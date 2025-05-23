using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageImporter.Models;

namespace ImageImporterUI.ViewModels;

public partial class ImproveNumberRecognitionViewModel(MainViewModel main) : ObservableObject
{
    public string Header { get; private set; } = "Improve";

    [ObservableProperty]
    private ObservableCollection<Number> numbers = [];

    [ObservableProperty]
    private Number? selectedNumber = null;

    public void Update()
    {
        Numbers = new ObservableCollection<Number>(main.puzzle.Numbers.Where(n => n.ContainsNumber));
        SelectedNumber = Numbers.FirstOrDefault();
    }
}
