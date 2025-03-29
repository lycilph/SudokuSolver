using Core.Commands;
using Core.Extensions;
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
    public override int Difficulty => 10;

    public override ICommand? Plan(Grid grid)
    {
        var command = new SkyscraperCommand(this);

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
            var links = grid.GetLinks(units, value);
            if (links.Count > 0)
                links_per_value.Add(value, links);
        }

        var skyscraper_units = base_unit_type == UnitType.Row ? UnitType.Column : UnitType.Row;
        foreach (var pair in links_per_value)
        {
            // We can only have a skyscraper if there are at least 2 strong links for a value
            if (pair.Value.Count >= 2)
            {
                foreach (var (link1, link2) in pair.Value.GetPairCombinations())
                    CheckLinksForSkyscraper(grid, link1, link2, base_unit_type, command);
            }
        }
    }

    private void CheckLinksForSkyscraper(Grid grid, Link link1, Link link2, UnitType base_unit_type, SkyscraperCommand command)
    {
        // If the links overlap in a box, then we can't have a skyscraper
        if (link1.OverlapsInBox(link2) is not null)
            return;

        // Get the value from one of the links
        var value = link1.Value;

        // Try to find the skyscraper base and roof
        var roof_overlaps = new List<Cell>();
        Link base_link = null!;
        if (link1.Start.GetIndexInUnit(base_unit_type) == link2.Start.GetIndexInUnit(base_unit_type)) // The start of the links are the base
        {
            base_link = new Link(value, link1.Start, link2.Start);
            roof_overlaps = link1.End.Peers.Intersect(link2.End.Peers).ToList(); // Find overlap of peers of the roof
        }
        else if (link1.End.GetIndexInUnit(base_unit_type) == link2.End.GetIndexInUnit(base_unit_type)) // The end of the links are the base
        {
            base_link = new Link(value, link1.End, link2.End);
            roof_overlaps = link1.Start.Peers.Intersect(link2.Start.Peers).ToList(); // Find overlap of peers of the roof
        }

        var cells_with_candidates_to_eliminate = roof_overlaps.Where(c => c.Contains(value)).ToList();

        // Update command
        var skyscraper_units = base_unit_type == UnitType.Row ? UnitType.Column : UnitType.Row;
        if (cells_with_candidates_to_eliminate.Count > 0)
            command.Add(new CommandElement
            {
                Description = $"A skyscraper on {value} in {skyscraper_units}s found for {link1} and {link2}, eliminating {value} in cells ({cells_with_candidates_to_eliminate.ToIndexString()})",
                Numbers = [value],
                Cells = cells_with_candidates_to_eliminate,
                CellsToVisualize = roof_overlaps,
                LinksToVisualize = [link1, link2, base_link]
            });
    }
}
