using CommunityToolkit.Mvvm.ComponentModel;
using NLog;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly PuzzleService puzzle_service;

    [ObservableProperty]
    private GridViewModel gridVM;

    public MainViewModel(PuzzleService puzzle_service)
    {
        this.puzzle_service = puzzle_service;

        GridVM = new GridViewModel(puzzle_service.Grid);
    }
}