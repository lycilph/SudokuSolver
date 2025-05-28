using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Commands;
using Core.Engine;
using Core.Import;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using SudokuUI.Dialogs;
using SudokuUI.Infrastructure;
using SudokuUI.Messages;
using SudokuUI.Services;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableRecipient, IRecipient<MainWindowLoadedMessage>
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly PuzzleService puzzle_service;
    private readonly SolverService solver_service;
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
    private HighlightService highlightService;

    [ObservableProperty]
    private bool isKeyboardDisabled = false;

    [ObservableProperty]
    private bool isInputBindingsDisabled = false;

    [ObservableProperty]
    private TimeSpan elapsed;

    public MainViewModel(PuzzleService puzzle_service,
                         SolverService solver_service,
                         SelectionService selection_service,
                         DebugService debug_service,
                         UndoRedoService undo_service,
                         HighlightService highlight_service,
                         GridViewModel gridVM,
                         DigitSelectionViewModel digitSelectionVM,
                         NotificationViewModel notificationVM,
                         OverlayViewModel overlayVM,
                         SettingsViewModel settingsVM)
    {
        this.puzzle_service = puzzle_service;
        this.solver_service = solver_service;
        this.selection_service = selection_service;
        this.debug_service = debug_service;

        GridVM = gridVM;
        DigitSelectionVM = digitSelectionVM;
        NotificationVM = notificationVM;
        OverlayVM = overlayVM;
        SettingsVM = settingsVM;
        UndoService = undo_service;
        highlightService = highlight_service;

        OverlayVM.EscapeCommand = EscapeCommand;
        OverlayVM.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(OverlayViewModel.IsOpen))
                IsKeyboardDisabled = OverlayVM.IsOpen;
        };

        // Handle the puzzle solved event
        puzzle_service.PuzzleSolved += async (s, e) => await OnPuzzleSolved(s, e);

        // Handle the request to visualize a command
        solver_service.VisualizeCommandRequest += async (s, e) => await ShowHint(e);

        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        timer.Tick += (s, e) => Elapsed = puzzle_service.GetElapsedTime();
        timer.Start();

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    // Misc general methods

    private async Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative)
    {
        IsInputBindingsDisabled = true;
        var result = await DialogCoordinator.Instance.ShowMessageAsync(this, title, message, style);
        IsInputBindingsDisabled = false;
        
        return result;
    }

    private async Task<T> ShowDialogAsync<T>(IDialogViewModel<T> vm, UserControl view, string title)
    {
        IsInputBindingsDisabled = true;
        
        var dialog = new CustomDialog { Title = title, Content = view };

        await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
        var result = await vm.DialogResult;
        await DialogCoordinator.Instance.HideMetroDialogAsync(this, dialog);

        IsInputBindingsDisabled = false;

        return result;
    }

    // General event handlers

    private async Task OnPuzzleSolved(object? sender, EventArgs e)
    {
        var victory_result = await OverlayVM.ShowVictory(puzzle_service.Source, puzzle_service.GetElapsedTime());

        switch (victory_result.Result)
        {
            case VictoryResult.ResultType.NewGame:
                await OverlayVM.Show(true);

                ClearPuzzle();
                await NewPuzzle();
                break;
            case VictoryResult.ResultType.Clear:
                using (var scope = OverlayVM.GetWaitingSpinnerScope(true))
                {
                    await scope.OpenAnimationTask;
                    ClearPuzzle();
                }
                break;
            case VictoryResult.ResultType.Reset:
                UndoService.Reset();
                break;
        }
    }

    // Window commands (ie. buttons in the title bar)

    [RelayCommand]
    private void ToggleSettings()
    {
        if (!OverlayVM.CanToggleSettings())
            return;

        if (SettingsVM.IsActive)
            OverlayVM.Hide();
        else
            OverlayVM.ShowSettings();
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
        // The overlay cannot be cancelled if eg the spinner is shown (ie. it is working)
        if (OverlayVM.IsOpen && OverlayVM.CanHide())
        {
            OverlayVM.Hide();
            return;
        }

        selection_service.Clear();
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
            using var scope = OverlayVM.GetWaitingSpinnerScope(true);
            await puzzle_service.New(difficulty);
        }
    }

    [RelayCommand]
    private void ClearPuzzle()
    {
        puzzle_service.Clear();
    }

    [RelayCommand]
    private async Task ImageImport()
    {
        using (var scope = OverlayVM.GetWaitingSpinnerScope(true))
        {
            await scope.OpenAnimationTask;
            await Task.Run(() => { var importer = new PuzzleImporter(); });
        }
    }

    [RelayCommand]
    private async Task Import()
    {
        var vm = new ImportDialogViewModel();
        var view = new ImportDialogView { DataContext = vm };

        var result = await ShowDialogAsync(vm, view, "Import Puzzle");

        if (!string.IsNullOrWhiteSpace(result))
            puzzle_service.Import(result);
    }

    [RelayCommand]
    private async Task Export()
    {
        var vm = new ExportDialogViewModel { Puzzle = puzzle_service.Export() };
        var view = new ExportDialogView { DataContext = vm };
        
        await ShowDialogAsync(vm, view, "Export Puzzle");
    }

    [RelayCommand]
    private async Task ShowSolutionCount()
    {
        var vm = new SolutionInformationDialogViewModel(puzzle_service.Source);
        var view = new SolutionInformationDialogView { DataContext = vm };

        await ShowDialogAsync(vm, view, "Solution Information");
    }

    [RelayCommand]
    private void ValidateSolution()
    {
        selection_service.Clear();
        HighlightService.ValidateGrid();
    }

    // Right side buttons

    [RelayCommand]
    private async Task ShowHint(BaseCommand? cmd = null)
    {
        var any_candidates = puzzle_service.Grid.Cells.Any(c => c.Count() > 0);
        if (!any_candidates)
        {
            var result = await ShowMessageAsync("No candidates found", "Do you want to add them automatically?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
                puzzle_service.FillCandidates();
            else
                return;
        }

        if (cmd != null || solver_service.HasNextHint())
        {
            selection_service.Clear();
            await OverlayVM.ShowHint(cmd);
        }
        else
        {
            //Show notification if not more hints
            WeakReferenceMessenger.Default.Send(new ShowNotificationMessage("No more hints found"));
        }
    }
    
    [RelayCommand]
    private async Task SolveNakedSingles()
    {
        var any_candidates = puzzle_service.Grid.Cells.Any(c => c.Count() > 0);
        if (!any_candidates)
        {
            var result = await ShowMessageAsync("No candidates found", "Do you want to add them automatically?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
                puzzle_service.FillCandidates();
            else
                return;
        }

        solver_service.SolveNakedSingles();
    }

    [RelayCommand]
    private async Task RunSolver()
    {
        var any_candidates = puzzle_service.Grid.Cells.Any(c => c.Count() > 0);
        if (!any_candidates)
        {
            IsInputBindingsDisabled = false;
            var result = await ShowMessageAsync("No candidates found", "Do you want to add them automatically?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
                puzzle_service.FillCandidates();
            else
                return;
        }

        (var commands, var stats) = Solver.Solve(puzzle_service.Grid);

        if (puzzle_service.Grid.IsSolved())
        {
            OverlayVM.AddVictoryStatistics(stats);
            UndoService.Execute(commands); // This will also trigger the PuzzleSolved event, and show the victory overlay
        }
        else
        {
            UndoService.Execute(commands);
            WeakReferenceMessenger.Default.Send(new ShowNotificationMessage("Puzzle couldn't be solved!"));
        }
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

    // Message handling

    public async void Receive(MainWindowLoadedMessage message)
    {
        logger.Info("Received the main window loaded message");

        // Try to load and old puzzle first
        if (!puzzle_service.Load())
        {
            // Getting here means that there were no old puzzle to load, so show the new game overlay instead
            await NewPuzzle();
        }
    }
}