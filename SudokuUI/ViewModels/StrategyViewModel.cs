using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Strategies;
using NLog;

namespace SudokuUI.ViewModels;

public partial class StrategyViewModel : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public event EventHandler<IStrategy> ExecuteStrategyRequest = null!;
    public event EventHandler<IStrategy> VisualizeStrategyRequest = null!;

    [ObservableProperty]
    private IStrategy wrappedObject;

    [ObservableProperty]
    private bool selected = true;

    public StrategyViewModel(IStrategy strategy) => WrappedObject = strategy;

    [RelayCommand]
    private void Execute()
    {
        logger.Info($"Executing strategy {WrappedObject.Name}");
        ExecuteStrategyRequest?.Invoke(this, WrappedObject);
   }

    [RelayCommand]
    private void Visualize()
    {
        logger.Info($"Visualizing strategy {WrappedObject.Name}");
        VisualizeStrategyRequest?.Invoke(this, WrappedObject);
    }
}
