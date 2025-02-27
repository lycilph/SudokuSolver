using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Extensions;
using Core.Model;
using SudokuUI.Messages;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class CellViewModel : ObservableObject
{
    private Cell cell;

    [ObservableProperty]
    private ObservableCollection<HintViewModel> hints;

    [ObservableProperty]
    private int value = 0;

    public CellViewModel(Cell cell)
    {
        this.cell = cell;
        Value = cell.Value;

        hints = Enumerable.Range(1, 9).Select(i => new HintViewModel(cell.Candidates.Contains(i) ? i : 0)).ToObservableCollection();
    }

    [RelayCommand]
    private void CellClicked()
    {
        if (cell.IsFilled)
            StrongReferenceMessenger.Default.Send(new SelectDigitMessage(cell.Value));
        else
        {
            int digit_selected = StrongReferenceMessenger.Default.Send(new RequestSelectedDigitMessage());
            if (digit_selected != 0)
                Value = digit_selected;
        }
    }
}
