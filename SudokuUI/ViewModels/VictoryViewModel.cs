using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;
using SudokuUI.Services;
using System.Windows.Threading;

namespace SudokuUI.ViewModels;

public partial class VictoryViewModel : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly PuzzleService puzzle_service;
    private DispatcherTimer _timer;

    [ObservableProperty]
    private bool _visible = false;

    [ObservableProperty]
    private bool showButton = false;

    public VictoryViewModel(PuzzleService puzzle_service)
    {
        this.puzzle_service = puzzle_service;
        puzzle_service.PuzzleSolved += StartAnimation;

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(3);
        _timer.Tick += StopAnimation;
    }

    private void StopAnimation(object? sender, EventArgs e)
    {
        _timer.Stop();
        ShowButton = true;
    }

    private void StartAnimation(object? sender, EventArgs e)
    {
        logger.Info("Puzzle solved");
        Visible = true;
        _timer.Start();
    }

    [RelayCommand]
    private void NewGame()
    {
        Visible = false;
        ShowButton = false;
        puzzle_service.NewGame();
    }
}
