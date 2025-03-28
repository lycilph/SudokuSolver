using Core.Commands;
using Core.Extensions;
using Core.Misc;
using Core.Models;

namespace Core.Strategies;

/* Strategy name: Skyscraper
 * 
 * The description of the pattern sounds more complicated than it really is: Concentrate on one digit. 
 * 
 * Find two rows (or columns) that contain only two candidates for that digit. 
 * If two of those candidates are in the same column (or row), one of the other two candidates must be true. 
 * 
 * All candidates that see both of those cells can therefore be eliminated. 
 * 
 * (Source: https://hodoku.sourceforge.net/en/tech_sdp.php)
 * 
 * Test puzzle(s): 
 * 697.....2..1972.63..3..679.912...6.737426.95.8657.9.241486932757.9.24..6..68.7..9  (Source: https://hodoku.sourceforge.net/en/tech_sdp.php)
 * ..1.28759.879.5132952173486.2.7..34....5..27.714832695....9.817.78.5196319..87524  (Source: https://hodoku.sourceforge.net/en/tech_sdp.php)
 */

public class SkyscraperStrategy : BaseStrategy<SkyscraperStrategy>
{
    public override string Name => "Skyscraper";

    public override ICommand? Plan(Grid grid)
    {
        var command = new SkyscraperCommand(Name);

        FindSkyscrapers(grid, grid.Rows, UnitType.Column, command);
        FindSkyscrapers(grid, grid.Columns, UnitType.Row, command);

        return command.IsValid() ? command : null;
    }

    // Here the base_unit_type determines if the base of the skyscraper should be found in a row or column
    private void FindSkyscrapers(Grid grid, Unit[] units, UnitType base_unit_type, SkyscraperCommand command)
    {
        var links_per_value = new Dictionary<int, List<Link>>();
        foreach (var value in Grid.PossibleValues) 
        {
            foreach (var unit in units)
            {
                // Find strong links in the unit
                var link_candidates = unit.EmptyCells().Where(c => c.Contains(value)).ToList();
                if (link_candidates.Count == 2)
                {
                    var link = new Link(value, link_candidates[0], link_candidates[1]);
                    if (links_per_value.ContainsKey(value))
                        links_per_value[value].Add(link);
                    else
                        links_per_value.Add(value, [link]);
                }
            }
        }

        var skyscraper_units = base_unit_type == UnitType.Row ? UnitType.Column : UnitType.Row;
        foreach (var pair in links_per_value)
        {
            if (pair.Value.Count >= 2)
            {
                foreach (var (link1, link2) in pair.Value.GetPairCombinations())
                    CheckLinksForSkyscraper(grid, link1, link2, pair.Key, base_unit_type, command);
            }
        }
    }

    private void CheckLinksForSkyscraper(Grid grid, Link link1, Link link2, int value, UnitType base_unit_type, SkyscraperCommand command)
    {
        var box_indices = new List<int>
        {
            FindBoxContainingCell(grid, link1.Start).Index,
            FindBoxContainingCell(grid, link1.End).Index,
            FindBoxContainingCell(grid, link2.Start).Index,
            FindBoxContainingCell(grid, link2.End).Index
        };
        var box_indices_count = box_indices.Distinct().Count();
        if (box_indices_count == 4)
        {
            Console.WriteLine($" * Links starts and ends in different boxes");

            // Try to find the skyscraper base and roof
            var roof_overlaps = new List<Cell>();
            if (link1.Start.GetIndexInUnit(base_unit_type) == link2.Start.GetIndexInUnit(base_unit_type)) // The start of the links are the base
                roof_overlaps = link1.End.Peers.Intersect(link2.End.Peers).ToList(); // Find overlap of peers of the roof
            else if (link1.End.GetIndexInUnit(base_unit_type) == link2.End.GetIndexInUnit(base_unit_type)) // The end of the links are the base
                roof_overlaps = link1.Start.Peers.Intersect(link2.Start.Peers).ToList(); // Find overlap of peers of the roof

            var cells_with_candidates_to_eliminate = roof_overlaps.Where(c => c.Contains(value)).ToList();

            // Update command
            var skyscraper_units = base_unit_type == UnitType.Row ? UnitType.Column : UnitType.Row;
            if (cells_with_candidates_to_eliminate.Count > 0)
                command.Add(new CommandElement
                {
                    Description = $"A skyscraper on {value} in {skyscraper_units}s found for {link1} and {link2}, eliminating {value} in cells ({cells_with_candidates_to_eliminate.ToIndexString()})",
                    Numbers = [value],
                    Cells = cells_with_candidates_to_eliminate,
                    CellsToVisualize = [link1.Start, link1.End, link2.Start, link2.End, .. roof_overlaps]
                });
        }
    }

    private static Unit FindBoxContainingCell(Grid grid, Cell cell)
    {
        return grid.Boxes.Where(b => b.Cells.Contains(cell)).First();
    }
}
