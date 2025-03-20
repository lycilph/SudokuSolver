using System.Text.RegularExpressions;
using Core.Models;

namespace Core.Extensions;

public static class GridExtensions
{
    public static Grid Load(this Grid grid, string puzzle)
    {
        if (string.IsNullOrWhiteSpace(puzzle) || puzzle.Length != Grid.Size() || !Regex.IsMatch(puzzle, @"^[1-9.]+$"))
            throw new ArgumentException("The input string must be 81 characters long and consist only of digits and .");

        grid.Reset();

        foreach (var i in Grid.AllCellIndices)
        {
            if (puzzle[i] != '.')
            {
                grid[i].Value = puzzle[i] - '0';
                grid[i].IsClue = true;
            }
        }

        return grid;
    }
}
