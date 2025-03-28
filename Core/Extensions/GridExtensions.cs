using System.Text.RegularExpressions;
using Core.Models;

namespace Core.Extensions;

public static class GridExtensions
{
    public static Grid Load(this Grid grid, string puzzle, bool fill_candidates = false)
    {
        // Replace 0 with . (if 0 is used to denote an empty cell)
        puzzle = puzzle.Replace('0', '.');

        // Try to validate the puzzle here
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

        if (fill_candidates)
            grid.FillCandidates();

        return grid;
    }

    public static Grid Copy(this Grid grid)
    {
        var str = grid.ToSimpleString();
        return new Grid().Load(str);
    }
}
