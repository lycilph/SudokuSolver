using NLog;
using SudokuUI.ViewModels;

namespace SudokuUI.Services;

public class HighlightService
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly GridViewModel grid;

    public HighlightService(GridViewModel grid)
    {
        this.grid = grid;
    }

    public void HighlightNumber(int number)
    {
        logger.Debug($"Highlighting number {number}");

        
    }
}
