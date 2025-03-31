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
            throw new ArgumentException("Invalid puzzle (valid 0-9 and .)");

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

    public static Grid Copy(this Grid grid, bool fill_candidates = false)
    {
        var str = grid.ToSimpleString();
        return new Grid().Load(str, fill_candidates);
    }

    public static Link? GetLink(this Grid grid, Unit unit, int value)
    {
        // Find strong links in the unit
        var cells = unit.EmptyCells().Where(c => c.Contains(value)).ToList();
        if (cells.Count == 2)
        {
            var link = new Link(value, cells[0], cells[1]);

            // Check that a link starts and ends in different boxes
            var start_box = grid.Boxes.Where(b => b.Cells.Contains(link.Start)).First();
            var end_box = grid.Boxes.Where(b => b.Cells.Contains(link.End)).First();
            if (start_box != end_box)
            {
                link.SetBoxes(start_box, end_box);
                return link;
            }
        }
        return null;
    }

    public static List<Link> GetLinks(this Grid grid, Unit[] units, int value)
    {
        var links = new List<Link>();

        foreach (var unit in units)
        {
            var link = grid.GetLink(unit, value);
            if (link != null)
                links.Add(link);
        }

        return links;
    }
}
