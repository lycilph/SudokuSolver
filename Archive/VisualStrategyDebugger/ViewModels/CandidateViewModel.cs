using CommunityToolkit.Mvvm.ComponentModel;
using Core.Model;
using System.Diagnostics;

namespace VisualStrategyDebugger.ViewModels;

[DebuggerDisplay("{value} visible={isVisible}")]
public partial class CandidateViewModel : ObservableObject
{
    private readonly Cell cell;

    [ObservableProperty]
    private int value;

    [ObservableProperty]
    private bool isVisible;

    [ObservableProperty]
    private bool highlight;

    public CandidateViewModel(Cell cell, int value)
    {
        this.cell = cell;

        Value = value;
        IsVisible = true;
        Highlight = false;
    }
}
