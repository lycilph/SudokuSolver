using CommunityToolkit.Mvvm.ComponentModel;
using Core.Archive.Model;
using System.Collections.ObjectModel;

namespace SudokuUI;

public partial class CellViewModel : ObservableObject
{
    private readonly Cell cell;

    [ObservableProperty]
    private ObservableCollection<string> _candidates;

    [ObservableProperty]
    private bool _selected = false;

    public CellViewModel(Cell cell)
    {
        this.cell = cell;

        Candidates = new ObservableCollection<string>(Grid.PossibleValues.Select(x => cell.Candidates.Contains(x) ? x.ToString() : " "));
    }

    public int Value
    {
        get => cell.Value;
        set 
        {
            SetProperty(cell.Value, value, cell, (c, v) => c.Value = v);
            OnPropertyChanged(nameof(HasValue));
        }
    }

    public bool HasValue { get => cell.HasValue; }

    public void UpdateSelection(int value)
    {
        Selected = HasValue && Value == value || !HasValue && cell.Candidates.Contains(value);
    }
}
