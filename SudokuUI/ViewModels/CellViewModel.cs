using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Extensions;
using Core.Model;
using Microsoft.Extensions.DependencyInjection;
using SudokuUI.Messages;
using SudokuUI.Services;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class CellViewModel : ObservableRecipient, IRecipient<RefreshFromModelMessage>
{
    private readonly Cell cell;
    private readonly SelectionService selection_service;
    private readonly PuzzleService puzzle_service;

    [ObservableProperty]
    private ObservableCollection<HintViewModel> hints;

    [ObservableProperty]
    private int value = 0;

    public CellViewModel(Cell cell)
    {
        this.cell = cell;
        selection_service = App.Current.Services.GetRequiredService<SelectionService>();
        puzzle_service = App.Current.Services.GetRequiredService<PuzzleService>();

        hints = Grid.PossibleValues.Select(i => new HintViewModel(0)).ToObservableCollection();

        IsActive = true; // Make sure we are active to receive messages
    }

    [RelayCommand]
    private void CellClicked()
    {
        if (cell.IsFilled)
        {
            selection_service.Digit = cell.Value;
        }
        else
        {
            if (selection_service.Digit != -1)
            {
                if (selection_service.InputMode == SelectionService.Mode.Digits)
                    puzzle_service.SetDigit(cell, selection_service.Digit);
                    //Value = selection_service.Digit;
                //else
                //    ToggleHint(selection_service.Digit);
            }
        }
    }

    //private void ToggleHint(int digit)
    //{
    //    if (Hints[digit-1].Digit == 0)
    //        Hints[digit-1].Digit = digit;
    //    else
    //        Hints[digit-1].Digit = 0;
    //}

    public void Receive(RefreshFromModelMessage message)
    {
        Value = cell.Value;
        foreach (var digit in Grid.PossibleValues)
            if (cell.Candidates.Contains(digit))
                Hints[digit - 1].Digit = digit;
            else
                Hints[digit - 1].Digit = 0;
    }
}
