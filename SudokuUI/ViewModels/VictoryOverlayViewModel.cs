using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Extensions;
using Core.Models;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class VictoryOverlayViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isOpen = false;

    [ObservableProperty]
    public ObservableCollection<Cell> cells;

    public VictoryOverlayViewModel(PuzzleService puzzle_service)
    {
        puzzle_service.PuzzleSolved += (s,e) => Show();

        Cells = puzzle_service.Grid.Cells.ToObservableCollection();
    }

    public void Show() => IsOpen = true;
    public void Close() => IsOpen = false;
    public void Toggle() => IsOpen = !IsOpen;

    [RelayCommand]
    private void New()
    {
        Close();
    }

    [RelayCommand]
    private void Clear()
    {
        Close();
    }
}