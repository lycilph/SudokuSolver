using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Strategies;
using NLog;

namespace SudokuUI.ViewModels;

public partial class StrategyViewModel : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public event EventHandler<IStrategy> ExecuteCommandRequest = null!;
    public event EventHandler<IStrategy> VisualizeCommandRequest = null!;

    [ObservableProperty]
    private IStrategy wrappedObject;

    [ObservableProperty]
    private bool selected = true;

    public StrategyViewModel(IStrategy strategy) => WrappedObject = strategy;

    [RelayCommand]
    private void Execute()
    {
        logger.Info($"Executing strategy {WrappedObject.Name}");
        ExecuteCommandRequest?.Invoke(this, WrappedObject);
   }

    [RelayCommand]
    private void Visualize()
    {
        logger.Info($"Visualizing strategy {WrappedObject.Name}");
        VisualizeCommandRequest?.Invoke(this, WrappedObject);

        //if (WrappedObject.Plan(puzzle_service.Grid) is BaseCommand command)
        //{
        //    solver_service.SetHint(command);
        //    solver_service.Show();
        //}
    }
}
