using Core.Model;
using Core.Strategy;
using NLog;
using System.ComponentModel;

namespace SudokuUI.Services;

public class PuzzleService
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private string input = "4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9";

    public Grid Grid { get; private set; }

    public event EventHandler GridValuesChanged = null!;

    public PuzzleService()
    {
        Grid = new Grid(input);
        BasicEliminationStrategy.Temp(Grid);

        foreach (var cell in Grid.Cells)
            cell.PropertyChanged += CellChanged;
    }

    private void CellChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Cell.Value))
            OnGridValuesChanged();
    }

    private void OnGridValuesChanged()
    {
        logger.Debug("Grid values changed");
        GridValuesChanged?.Invoke(this, EventArgs.Empty);
    }

    public IEnumerable<int> CountDigits()
    {
        return Grid.PossibleValues.Select(i => Grid.FilledCells().Count(c => c.Value == i));
    }

    public void SetValue(Cell cell, int value)
    {

    }

    public void ToggleCandidate(Cell cell, int value)
    {

    }
}
