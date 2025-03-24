using System.Text;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.DancingLinks;
using Core.Extensions;
using MahApps.Metro.Controls.Dialogs;
using SudokuUI.Dialogs;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly PuzzleService puzzle_service;
    private readonly SelectionService selection_service;
    private readonly SettingsService settings_service;
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

    public MainViewModel(PuzzleService puzzle_service,
                         UndoRedoService undo_service,
                         SelectionService selection_service,
                         SettingsService settings_service,
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
        this.debug_service = debug_service;

        GridVM = gridVM;
        DigitSelectionVM = digitSelectionVM;
        SettingsVM = settingsVM;
        SettingsOverlayVM = settingsOverlayVM;
        WaitingOverlayVM = waitingOverlayVM;
        VictoryOverlayVM = victoryOverlayVM;
        SolverOverlayVM = solverOverlayVM;

        UndoService = undo_service;

        // Disable keyboard if settings, waiting overlay or victory overlay is showing
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
        SolverOverlayVM.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SolverOverlayViewModel.IsOpen))
                IsKeyboardDisabled = SolverOverlayVM.IsOpen;
        };

        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        timer.Tick += (s, e) => Elapsed = puzzle_service.GetElapsedTime();
        timer.Start();
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

        if (SolverOverlayVM.IsOpen)
        {
            SolverOverlayVM.Close();
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
        selection_service.Clear();
        SolverOverlayVM.Show();
    }
}