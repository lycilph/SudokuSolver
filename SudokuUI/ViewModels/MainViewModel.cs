using CommunityToolkit.Mvvm.ComponentModel;
using NLog;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    [ObservableProperty]
    private GridViewModel gridVM;

    [ObservableProperty]
    private DigitSelectionViewModel digitSelectionVM;

    public MainViewModel(GridViewModel gridVM, DigitSelectionViewModel digitSelectionVM)
    {
        GridVM = gridVM;
        DigitSelectionVM = digitSelectionVM;
    }
}