using CommunityToolkit.Mvvm.ComponentModel;
using Core.Strategies;

namespace SudokuUI.ViewModels;

public partial class StrategyViewModel : ObservableObject
{
    [ObservableProperty]
    private IStrategy wrappedObject;

    public StrategyViewModel(IStrategy strategy)
    {
        WrappedObject = strategy;
    }
}
