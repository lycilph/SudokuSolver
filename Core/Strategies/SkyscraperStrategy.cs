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

        //FindSkyscrapers(grid.Rows);
        FindSkyscrapers(grid, grid.Columns);

        return command.IsValid() ? command : null;
    }

    private void FindSkyscrapers(Grid grid, Unit[] units)
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
                    var link = new Link(link_candidates[0], link_candidates[1]);
                    Console.WriteLine($"Link found for {value} in {unit.FullName} and cells {link}");
                    if (links_per_value.ContainsKey(value))
                        links_per_value[value].Add(link);
                    else
                        links_per_value.Add(value, [link]);
                }
            }
        }

        foreach (var pair in links_per_value)
        {
            if (pair.Value.Count >= 2)
            {
                //Console.WriteLine($"Potential for a skyscraper in {pair.Key} (found {pair.Value.Count} strong links)");
                foreach (var (link1, link2) in pair.Value.GetPairCombinations())
                {
                    Console.WriteLine($"Testing {link1} and {link2} for a skyscraper in {pair.Key}");
                    CheckLinksForSkyscraper(grid, link1, link2, pair.Key);
                }
            }
        }
    }

    private void CheckLinksForSkyscraper(Grid grid, Link link1, Link link2, int value)
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

            // Try to find the skyscraper base
            if (link1.Start.Row == link2.Start.Row)
            {
                Console.WriteLine($" * Found a base in row {link1.Start.Row}");

            }
            else if (link1.End.Row == link2.End.Row)
            {
                Console.WriteLine($" * Found a base in row {link1.End.Row}");

                var overlap = link1.Start.Peers.Intersect(link2.Start.Peers).ToArray();
                Console.WriteLine($" * Overlap: {string.Join(',', overlap.Select(c => c.Index))}");

                foreach (var cell in overlap.Where(c => c.Contains(value)))
                    Console.WriteLine($" * Value {value} can be removed from cell {cell.Index}");
            }
        }
    }

    private Unit FindBoxContainingCell(Grid grid, Cell cell)
    {
        return grid.Boxes.Where(b => b.Cells.Contains(cell)).First();
    }
}
