﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Models;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class GridViewModel : ObservableObject
{
    [ObservableProperty]
    private SelectionService selectionService;

    [ObservableProperty]
    private ObservableCollection<BoxViewModel> boxes = null!;

    public GridViewModel(PuzzleService puzzle_service, SelectionService selectionService)
    {
        SelectionService = selectionService;

        Boxes = puzzle_service.Grid.Boxes.Select(b => new BoxViewModel(b)).ToObservableCollection();
    }

    public CellViewModel Map(Cell cell)
    {
        return Boxes.SelectMany(b => b.Cells).Single(c => c.WrappedObject == cell);
    }
}
