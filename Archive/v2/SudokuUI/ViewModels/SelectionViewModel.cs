﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Model;
using SudokuUI.Messages;
using SudokuUI.Services;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class SelectionViewModel : ObservableRecipient, IRecipient<RefreshFromModelMessage>
{
    private Grid grid;

    public ObservableCollection<DigitSelectionViewModel> DigitSelections { get; private set; }
    public SelectionService SelectionService { get; private set; }

    public SelectionViewModel(Grid grid, SelectionService selection_service)
    {
        this.grid = grid;
        SelectionService = selection_service;

        DigitSelections = [.. Enumerable.Range(1, 9).Select(d => new DigitSelectionViewModel(d, selection_service)), new DigitSelectionViewModel(0, selection_service)];
       
        IsActive = true; // Make sure we are active to receive messages
    }

    [RelayCommand]
    private void ClearSelection()
    {
        SelectionService.ClearDigitSelection();
    }

    public void Receive(RefreshFromModelMessage message)
    {
        foreach (var digit in Grid.PossibleValues)
        {
            var cells_with_digit = grid.Cells.Count(c => c.Value == digit);
            DigitSelections[digit - 1].Missing = 9 - cells_with_digit;
        }
    }
}
