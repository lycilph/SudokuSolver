using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ImageImportUI.MVVM;

public partial class ExtractDigitsViewModel : ObservableObject
{
    private readonly ImageImporter importer;

    [ObservableProperty]
    private ExtractCellsViewModel cellsVM;

    [ObservableProperty]
    private List<Cell> cells = [];

    [ObservableProperty]
    private Cell selectedCell = null!;

    [ObservableProperty]
    private int recognitionFailures = 0;

    public ExtractDigitsViewModel(ExtractCellsViewModel cellsVM, ImageImporter importer)
    {
        this.importer = importer;

        CellsVM = cellsVM;

        CellsVM.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ExtractCellsViewModel.Cells))
            {
                Update();
            }
        };
    }

    [RelayCommand]
    private void Update()
    {
        Cells = CellsVM.Cells;
        RecognitionFailures = importer.ExtractDigits(Cells);

        if (RecognitionFailures > 0 && Cells.Count > 0)
            SelectedCell = Cells.First(c => c.RecognitionFailed);
        else
            SelectedCell = Cells.First();
    }
}
