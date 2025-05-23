using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageImporter.Models;
using ImageImporter.Parameters;

namespace ImageImporterUI.ViewModels;

public partial class ImproveNumberRecognitionViewModel(MainViewModel main) : ObservableObject
{
    public string Header { get; private set; } = "Improve";

    [ObservableProperty]
    private ObservableCollection<Number> numbers = [];

    [ObservableProperty]
    private Number? selectedNumber = null;

    [ObservableProperty]
    private ObservableCollection<NumberRecognitionParameters> parameters = [];

    [ObservableProperty]
    private NumberRecognitionParameters selectedParameters = null!;

    [ObservableProperty]
    private int threshold = 0;

    [ObservableProperty]
    private int kernelSize = 0;

    [ObservableProperty]
    private int iterations = 0;

    [ObservableProperty]
    private int operation = 0;

    public void Update()
    {
        Numbers = new ObservableCollection<Number>(main.puzzle.Numbers.Where(n => n.ContainsNumber));
        SelectedNumber = Numbers.FirstOrDefault();

        Parameters = [.. main.parameters.NumberParameters];
    }

    partial void OnSelectedNumberChanged(Number? value)
    {
        if (SelectedNumber == null)
            return;

        SelectedParameters = SelectedNumber.ParametersUsed;
    }

    partial void OnSelectedParametersChanged(NumberRecognitionParameters value)
    {
        if (SelectedNumber == null)
            return;

        Threshold = SelectedParameters.Threshold;
        KernelSize = SelectedParameters.KernelSize;
        Iterations = SelectedParameters.Iterations;
        Operation = SelectedParameters.Operation;
    }

    [RelayCommand]
    public void Add()
    {
        var parameters = new NumberRecognitionParameters(0, 0, 0, 0);
        main.parameters.NumberParameters.Add(parameters);

        Parameters.Add(parameters);
        SelectedParameters = parameters;
    }

    [RelayCommand]
    private void Run()
    {
        SelectedParameters.Threshold = Threshold;
        SelectedParameters.KernelSize = KernelSize;
        SelectedParameters.Iterations = Iterations;
        SelectedParameters.Operation = Operation;

        if (SelectedNumber == null)
            return;

        // Run the algorithm
        var num = main.importer.RecognizeNumber(main.puzzle, SelectedNumber.Cell, SelectedParameters);

        // Update the view
        var index = Numbers.IndexOf(SelectedNumber);
        Numbers[index] = num;
        SelectedNumber = num;
    }

    [RelayCommand]
    private void Optimize()
    {
        if (SelectedNumber == null)
            return;

        main.importer.OptimizeNumberRecognition(SelectedNumber.Cell);
    }
}
