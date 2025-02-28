using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Core.Archive.DancingLinks;
using Core.Model;
using Core.Strategies;
using SudokuUI.Messages;

namespace SudokuUI.Services;

public partial class PuzzleService : ObservableRecipient, IRecipient<WindowShownMessage>
{
    private string input = "4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9";
    private Puzzle puzzle;

    public Grid Grid { get => puzzle.Grid; }

    public PuzzleService()
    {
        puzzle = new Puzzle();
        IsActive = true; // Make sure we are active to receive messages
    }

    public void Reset()
    {
        puzzle.Set(input);
        BasicEliminationStrategy.ExecuteAndApply(Grid);
    }

    public int GetSolutionCount()
    {
        var results = DancingLinksSolver.Solve(puzzle);
        return results.Count;
    }

    public Statistics GetStatistics() => puzzle.Stats;

    public void SetDigit(Cell cell, int digit)
    {

    }

    public void ToggleHint(int digit)
    {

    }

    public void Receive(WindowShownMessage message)
    {
        Reset();
        WeakReferenceMessenger.Default.Send(new RefreshFromModelMessage());
    }
}
