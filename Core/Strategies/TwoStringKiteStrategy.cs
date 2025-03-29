using Core.Commands;
using Core.Extensions;
using Core.Models;

namespace Core.Strategies;

/* Strategy name: 2 String Kite
 * 
 * The description: Concentrate again on one digit. 
 * 
 * Find a row and a column that have only two candidates left (the "strings"). 
 * One candidate from the row and one candidate from the column have to be in the same box. 
 * The candidate that sees the two other cells can be eliminated.
 * 
 * 2-String Kites work similar to Skyscrapers: One of the two ends of the strings must be true.
 * 
 * (Source: https://hodoku.sourceforge.net/en/tech_sdp.php, https://sudoku.coach/en/learn/two-string-kite)
 * 
 * Test puzzle(s): 
 * .81.2.6...42.6..89.568..24.69314275842835791617568932451..3689223...846.86.2.....  (Source: https://hodoku.sourceforge.net/en/tech_sdp.php)
 * 3617..295842395671.5.2614831.8526.34625....18.341..5264..61.85258...2167216857349  (Source: https://hodoku.sourceforge.net/en/tech_sdp.php)
 */

public class TwoStringKiteStrategy : BaseStrategy<TwoStringKiteStrategy>
{
    public override string Name => "2-String Kite";
    public override int Difficulty => 10;

    public override ICommand? Plan(Grid grid)
    {
        var command = new TwoStringKiteCommand(this);

        foreach (var value in Grid.PossibleValues)
            FindKite(grid, value, command);

        return command.IsValid() ? command : null;
    }

    private void FindKite(Grid grid, int value, TwoStringKiteCommand command)
    {
        // Find columns and row links (must start and end in different boxes)
        var row_links = grid.GetLinks(grid.Rows, value);
        var column_links = grid.GetLinks(grid.Columns, value);

        foreach (var row_link in row_links)
        {
            foreach (var column_link in column_links)
            {
                var box_overlap = row_link.OverlapsInBox(column_link);

                // Check that the row and column link cells does NOT overlap (ie. one end of each link is the same cell) &&
                // Check that the row and column link DOES overlap in a box
                if (row_link.OverlapsInCells(column_link) == null && box_overlap != null)
                {
                    // Find the cells that sees the other ends of the links
                    var row_link_other_end = row_link.GetOtherEnd(box_overlap);
                    var column_link_other_end = column_link.GetOtherEnd(box_overlap);

                    var cell_overlap = row_link_other_end.Peers
                        .Intersect(column_link_other_end.Peers)
                        .Where(c => !box_overlap.Cells.Contains(c)) // Exclude the box overlap
                        .Where(c => c.Contains(value))
                        .FirstOrDefault();

                    // A link to visualize the weak link of the kite
                    var weak_link = new Link(value, row_link.GetEnd(box_overlap), column_link.GetEnd(box_overlap));

                    // If so, eliminate the candidate that sees the other ends of the links
                    if (cell_overlap != null)
                    {
                        command.Add(new CommandElement
                        {
                            Description = $"A 2-string kite on {value} found for link {row_link} and {column_link} eliminating {value} in cell {cell_overlap.Index}",
                            Numbers = [value],
                            Cells = [cell_overlap],
                            CellsToVisualize = [.. row_link.Cells.ToList(), .. column_link.Cells.ToList()],
                            LinksToVisualize = [row_link, column_link, weak_link]
                        });
                    }
                }
            }
        }
    }
}
