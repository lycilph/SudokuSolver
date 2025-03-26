using System.Text;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.DancingLinks;
using Core.Extensions;
using MahApps.Metro.Controls.Dialogs;
using SudokuUI.Dialogs;
using SudokuUI.Messages;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableRecipient, IRecipient<ShowNotificationMessage>
{
    private readonly PuzzleService puzzle_service;
    private readonly SelectionService selection_service;
    private readonly SettingsService settings_service;
    private readonly SolverService solver_service;
    private readonly DebugService debug_service;

    [ObservableProperty]
    private bool isKeyboardDisabled = false;

    [ObservableProperty]
    private GridViewModel gridVM;

    [ObservableProperty]
    private DigitSelectionViewModel digitSelectionVM;

    [ObservableProperty]
    private SettingsViewModel settingsVM;

    [ObservableProperty]
    private SettingsOverlayViewModel settingsOverlayVM;

    [ObservableProperty]
    private WaitingOverlayViewModel waitingOverlayVM;

    [ObservableProperty]
    private VictoryOverlayViewModel victoryOverlayVM;

    [ObservableProperty]
    private SolverOverlayViewModel solverOverlayVM;

    [ObservableProperty]
    private UndoRedoService undoService;

    [ObservableProperty]
    private TimeSpan elapsed;
    
    [ObservableProperty]
    private bool showMessage = false;

    [ObservableProperty]
    private string notification = string.Empty;

    public MainViewModel(PuzzleService puzzle_service,
                         UndoRedoService undo_service,
                         SelectionService selection_service,
                         SettingsService settings_service,
                         SolverService solver_service,
                         DebugService debug_service,
                         GridViewModel gridVM,
                         DigitSelectionViewModel digitSelectionVM,
                         SettingsViewModel settingsVM,
                         SettingsOverlayViewModel settingsOverlayVM,
                         WaitingOverlayViewModel waitingOverlayVM,
                         VictoryOverlayViewModel victoryOverlayVM,
                         SolverOverlayViewModel solverOverlayVM)
    {
        this.puzzle_service = puzzle_service;
        this.selection_service = selection_service;
        this.settings_service = settings_service;
        this.solver_service = solver_service;
        this.debug_service = debug_service;

        GridVM = gridVM;
        DigitSelectionVM = digitSelectionVM;
        SettingsVM = settingsVM;
        SettingsOverlayVM = settingsOverlayVM;
        WaitingOverlayVM = waitingOverlayVM;
        VictoryOverlayVM = victoryOverlayVM;
        SolverOverlayVM = solverOverlayVM;

        UndoService = undo_service;

        // Disable keyboard if any of the overlays is showing
        settings_service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SettingsService.IsShown))
                IsKeyboardDisabled = settings_service.IsShown;
        };
        WaitingOverlayVM.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(WaitingOverlayViewModel.IsOpen))
                IsKeyboardDisabled = WaitingOverlayVM.IsOpen;
        };
        VictoryOverlayVM.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(VictoryOverlayViewModel.IsOpen))
                IsKeyboardDisabled = VictoryOverlayVM.IsOpen;
        };
        solver_service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SolverService.IsShown))
                IsKeyboardDisabled = solver_service.IsShown;
        };

        // Handle the puzzle solved event
        puzzle_service.PuzzleSolved += (s, e) =>
        {
            SolverOverlayVM.Close();

            VictoryOverlayVM.Elapsed = puzzle_service.GetElapsedTime();
            VictoryOverlayVM.Show();
        };

        // Handle events from the victory overlay
        VictoryOverlayVM.RequestNewGame += async (s, e) => await NewPuzzle();
        VictoryOverlayVM.RequestClearGame += (s, e) => ClearPuzzle();

        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        timer.Tick += (s, e) => Elapsed = puzzle_service.GetElapsedTime();
        timer.Start();

        WeakReferenceMessenger.Default.RegisterAll(this);
    }
    
    [RelayCommand]
    private async Task NewPuzzle()
    {
        WaitingOverlayVM.Show();

        await puzzle_service.New();

        WaitingOverlayVM.Close();
    }

    [RelayCommand]
    private void ClearPuzzle()
    {
        puzzle_service.Clear();
    }

    [RelayCommand]
    private async Task Import()
    {
        var vm = new ImportDialogViewModel();
        var view = new ImportDialogView { DataContext = vm };
        var dialog = new CustomDialog { Title = "Import Puzzle", Content = view };

        await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
        var result = await vm.DialogResult;
        await DialogCoordinator.Instance.HideMetroDialogAsync(this, dialog);

        if (!string.IsNullOrWhiteSpace(result))
            puzzle_service.Import(result);
    }

    [RelayCommand]
    private async Task Export()
    {
        var vm = new ExportDialogViewModel { Puzzle = puzzle_service.Export() };
        var view = new ExportDialogView { DataContext = vm };
        var dialog = new CustomDialog { Title = "Export Puzzle", Content = view };

        IsKeyboardDisabled = true;

        await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
        await vm.DialogResult;
        await DialogCoordinator.Instance.HideMetroDialogAsync(this, dialog);
    }

    [RelayCommand]
    private async Task ShowSolutionCount()
    {
        var copy = puzzle_service.Grid.Copy();
        copy.FillCandidates();

        (var solutions, var stats) = DancingLinksSolver.Solve(copy, true);

        var sb = new StringBuilder();
        sb.AppendLine($"The puzzle has {solutions.Count} solutions");
        sb.AppendLine($"Execution Time: {stats.ElapsedTime} ms");
        await DialogCoordinator.Instance.ShowMessageAsync(this, "Solution Count", sb.ToString());
    }

    [RelayCommand]
    private void ClearCandidates()
    {
        puzzle_service.ClearCandidates();
    }

    [RelayCommand]
    private void FillCandidates()
    {
        puzzle_service.FillCandidates();
    }

    [RelayCommand]
    private void NumberKeyPressed(string number)
    {
        if (int.TryParse(number, out int digit))
            selection_service.Digit = digit;
    }

    [RelayCommand]
    private void Escape()
    {
        if (settings_service.IsShown)
        {
            settings_service.Hide();
            return;
        }

        if (solver_service.IsShown)
        {
            solver_service.Hide();
            return;
        }

        selection_service.Clear();
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
    private void ToggleInputMode()
    {
        selection_service.ToggleInputMode();
    }

    [RelayCommand]
    private void ShowSettings()
    {
        settings_service.Show();
    }

    [RelayCommand]
    private void HideSettings()
    {
        settings_service.Hide();
    }

    [RelayCommand]
    private void ShowDebugWindow()
    {
        debug_service.ShowDebugWindow();
    }

    [RelayCommand]
    private void ShowHint()
    {
        var has_hint = solver_service.NextHint();

        if (has_hint)
        {
            selection_service.Clear();
            solver_service.Show();
        }
        else
        {
            //Show notification if not more hints
            WeakReferenceMessenger.Default.Send(new ShowNotificationMessage("No more hints found"));
        }
    }

    public void Receive(ShowNotificationMessage message)
    {
        Notification = message.Notification;
        ShowMessage = true;
    }
}