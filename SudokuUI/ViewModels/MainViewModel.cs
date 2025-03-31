using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.DancingLinks;
using Core.Extensions;
using MahApps.Metro.Controls.Dialogs;
using SudokuUI.Dialogs;
using SudokuUI.Services;
using System.Text;
using System.Windows.Threading;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly PuzzleService puzzle_service;
    private readonly SettingsService settings_service;
    private readonly SelectionService selection_service;
    private readonly DebugService debug_service;

    [ObservableProperty]
    private GridViewModel gridVM;

    [ObservableProperty]
    private DigitSelectionViewModel digitSelectionVM;

    [ObservableProperty]
    private NotificationViewModel notificationVM;

    [ObservableProperty]
    private OverlayViewModel overlayVM;

    [ObservableProperty]
    private SettingsViewModel settingsVM;

    [ObservableProperty]
    private UndoRedoService undoService;

    [ObservableProperty]
    private VisualizationService visualizationService;

    [ObservableProperty]
    private bool isKeyboardDisabled = false;

    [ObservableProperty]
    private bool isInputBindingsDisabled = false;

    [ObservableProperty]
    private TimeSpan elapsed;

    public MainViewModel(PuzzleService puzzle_service,
                         SettingsService settings_service,
                         SelectionService selection_service,
                         DebugService debug_service,
                         UndoRedoService undo_service,
                         VisualizationService visualization_service,
                         GridViewModel gridVM,
                         DigitSelectionViewModel digitSelectionVM,
                         NotificationViewModel notificationVM,
                         OverlayViewModel overlayVM,
                         SettingsViewModel settingsVM)
    {
        this.puzzle_service = puzzle_service;
        this.settings_service = settings_service;
        this.selection_service = selection_service;
        this.debug_service = debug_service;

        GridVM = gridVM;
        DigitSelectionVM = digitSelectionVM;
        NotificationVM = notificationVM;
        OverlayVM = overlayVM;
        SettingsVM = settingsVM;
        UndoService = undo_service;
        VisualizationService = visualization_service;

        OverlayVM.EscapeCommand = EscapeCommand;
        OverlayVM.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(OverlayViewModel.IsOpen))
                IsKeyboardDisabled = OverlayVM.IsOpen;
        };

        settings_service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SettingsService.IsOpen))
                OverlayVM.IsOpen = settings_service.IsOpen;
        };

        // Handle the puzzle solved event
        puzzle_service.PuzzleSolved += (s, e) =>
        {
            //SolverOverlayVM.Close();

            //VictoryOverlayVM.Elapsed = puzzle_service.GetElapsedTime();
            //VictoryOverlayVM.Show();

            OverlayVM.ShowVictory();
        };

        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        timer.Tick += (s, e) => Elapsed = puzzle_service.GetElapsedTime();
        timer.Start();
    }

    // Window commands (ie. buttons in the title bar)

    [RelayCommand]
    private void ShowSettings()
    {
        settings_service.Show();
    }

    // Input binding commands

    [RelayCommand]
    private void NumberKeyPressed(string number)
    {
        if (int.TryParse(number, out int digit))
            selection_service.Digit = digit;
    }

    [RelayCommand]
    private void Escape()
    {
        if (settings_service.IsOpen)
        {
            settings_service.Hide();
            return;
        }

        // The overlay cannot be cancelled if the spinner is shown (ie. it is working)
        if (OverlayVM.IsOpen && !OverlayVM.ShowSpinner)
        {
            OverlayVM.Hide();
            return;
        }
    }

    [RelayCommand]
    private void ToggleInputMode()
    {
        selection_service.ToggleInputMode();
    }

    [RelayCommand]
    private void NextDigit()
    {
        selection_service.Next();
    }

    [RelayCommand]
    private void PreviousDigit()
    {
        selection_service.Previous();
    }

    [RelayCommand]
    private void ShowDebugWindow()
    {
        debug_service.ToggleDebugWindow();
    }

    // Left side buttons

    [RelayCommand]
    private async Task NewPuzzle()
    {
        var difficulty = await OverlayVM.ShowNewGame();

        if (difficulty != null)
        {
            OverlayVM.Show(true);
            await puzzle_service.New(difficulty);
            OverlayVM.Hide();
        }
    }

    [RelayCommand]
    private async Task ClearPuzzle()
    {
        OverlayVM.IsOpen = true;
        OverlayVM.ShowSpinner = true;
        await Task.Delay(1000);
        OverlayVM.ShowSpinner = false;
        OverlayVM.IsOpen = false;
    }

    [RelayCommand]
    private async Task Import()
    {
        IsInputBindingsDisabled = true;

        var vm = new ImportDialogViewModel();
        var view = new ImportDialogView { DataContext = vm };
        var dialog = new CustomDialog { Title = "Import Puzzle", Content = view };

        await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
        var result = await vm.DialogResult;
        await DialogCoordinator.Instance.HideMetroDialogAsync(this, dialog);

        if (!string.IsNullOrWhiteSpace(result))
            puzzle_service.Import(result);

        IsInputBindingsDisabled = false;
    }

    [RelayCommand]
    private async Task Export()
    {
        IsInputBindingsDisabled = true;

        var vm = new ExportDialogViewModel { Puzzle = puzzle_service.Export() };
        var view = new ExportDialogView { DataContext = vm };
        var dialog = new CustomDialog { Title = "Export Puzzle", Content = view };

        await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
        await vm.DialogResult;
        await DialogCoordinator.Instance.HideMetroDialogAsync(this, dialog);

        IsInputBindingsDisabled = false;
    }

    // Right side buttons

    [RelayCommand]
    private async Task ShowSolutionCount()
    {
        IsInputBindingsDisabled = true;

        var copy = puzzle_service.Grid.Copy();
        copy.FillCandidates();

        (var solutions, var stats) = DancingLinksSolver.Solve(copy, true);

        var sb = new StringBuilder();
        sb.AppendLine($"The puzzle has {solutions.Count} solutions");
        sb.AppendLine($"Execution Time: {stats.ElapsedTime} ms");
        await DialogCoordinator.Instance.ShowMessageAsync(this, "Solution Count", sb.ToString());

        IsInputBindingsDisabled = false;
    }

    [RelayCommand]
    private void FillCandidates()
    {
        puzzle_service.FillCandidates();
    }

    [RelayCommand]
    private void ClearCandidates()
    {
        puzzle_service.ClearCandidates();
    }
}