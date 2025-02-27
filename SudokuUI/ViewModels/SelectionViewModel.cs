using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Model;
using SudokuUI.Extensions;
using SudokuUI.Messages;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class SelectionViewModel : ObservableObject, IRecipient<SelectDigitMessage>
{
    private Grid grid;

    public ObservableCollection<DigitSelectionViewModel> DigitSelections { get; private set; }

    [ObservableProperty]
    private bool _inputtingHints = false;

    public SelectionViewModel(Grid grid)
    {
        this.grid = grid;
        DigitSelections = [.. Enumerable.Range(1, 9).Select(d => new DigitSelectionViewModel(d, Select))];
        
        RefreshFromModel();

        StrongReferenceMessenger.Default.RegisterAll(this);
        StrongReferenceMessenger.Default.Register<SelectionViewModel, RequestSelectedDigitMessage>(this, (vm, m) => m.Reply(vm.GetSelection()));
    }

    public void RefreshFromModel()
    {
        foreach (var digit in Grid.PossibleValues)
        {
            var cells_with_digit = grid.Cells.Count(c => c.Value == digit);
            DigitSelections[digit - 1].Missing = 9 - cells_with_digit;
        }
    }

    [RelayCommand]
    public void ClearSelection()
    {
        DigitSelections.ForEach(s => s.Selected = false);
    }

    public void Select(int digit)
    {
        DigitSelections.ForEach(s => s.Selected = (digit == s.Digit));
    }

    public int GetSelection()
    {
        return DigitSelections.FirstOrDefault(s => s.Selected)?.Digit ?? 0;
    }

    public void ToggleInput() => InputtingHints = !InputtingHints;

    public void Receive(SelectDigitMessage message)
    {
        Select(message.digit);
    }
}
