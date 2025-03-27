using Core.Commands;
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
        FindSkyscrapers(grid.Columns);

        return command.IsValid() ? command : null;
    }

    private void FindSkyscrapers(Unit[] units)
    {
        foreach(var value in Grid.PossibleValues) 
        {
            foreach (var unit in units)
            {
                var link = unit.EmptyCells().Where(c => c.Contains(value)).ToList();
                if (link.Count == 2)
                    Console.WriteLine($"Found a strong link for {value} in {unit.FullName} and cells ({string.Join(',',link.Select(c => c.Index))})");
            }
        }
    }
}
