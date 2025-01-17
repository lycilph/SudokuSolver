using System.Text;

namespace Sandbox.Model;

public class Grid
{
    public Cell[] Cells { get; set; }
    public Unit[] Rows { get; set; }
    public Unit[] Columns { get; set; }
    public Unit[] Boxes { get; set; }

    public Grid()
    {
        Cells = Enumerable.Range(0, 81).Select(i => new Cell(i)).ToArray();

        Rows = Enumerable.Range(0, 9)
            .Select(r => new Unit { Index = r, Cells = GetRowIndices(r).Select(i => Cells[i]).ToArray() }).ToArray();

        Columns = Enumerable.Range(0, 9)
            .Select(c => new Unit { Index = c, Cells = GetColumnIndices(c).Select(i => Cells[i]).ToArray() }).ToArray();

        Boxes = Enumerable.Range(0, 9)
            .Select(c => new Unit { Index = c, Cells = GetBoxIndices(c).Select(i => Cells[i]).ToArray() }).ToArray();

        foreach (var cell in Cells)
            cell.Peers = GetPeerIndices(cell.Index).Select(i => Cells[i]).ToArray();
    }

    public Grid(string puzzle) : this()
    {
        for (int i = 0; i < 81; i++)
        {
            if (puzzle[i] != '.')
                Cells[i].Value = puzzle[i] - '0';
        }
    }

    private static int[] GetRowIndices(int row)
    {
        var indices = new int[9];
        for (int i = 0; i < 9; i++)
        {
            indices[i] = row * 9 + i;
        }
        return indices;
    }

    private static int[] GetColumnIndices(int col)
    {
        var indices = new int[9];
        for (int i = 0; i < 9; i++)
        {
            indices[i] = col + i * 9;
        }
        return indices;
    }

    private static int[] GetBoxIndices(int box)
    {
        var indices = new int[9];
        int startRow = (box / 3) * 3;
        int startCol = (box % 3) * 3;
        for (int i = 0; i < 9; i++)
        {
            indices[i] = (startRow + i / 3) * 9 + (startCol + i % 3);
        }
        return indices;
    }

    private static int[] GetPeerIndices(int cell)
    {
        var peers = new HashSet<int>();
        for (int i = 0; i < 9; i++)
        {
            peers.Add(cell - cell % 9 + i); // Row peers
            peers.Add(cell % 9 + i * 9); // Column peers
        }
        // Box peers
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                peers.Add(cell - cell % 27 + i * 9 + j + cell % 9 - cell % 3);
            }
        }
        peers.Remove(cell);
        return peers.Order().ToArray();
    }

    public string CandidatesToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 81; i++)
        {
            sb.AppendLine($"Cell {i}: {string.Join(' ', Cells[i].Candidates)}");
        }
        return sb.ToString();
    }

    public string PeersToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 81; i++)
        {
            sb.AppendLine($"Peers for Cell {i}: {string.Join(' ', Cells[i].Peers.Select(p => p.Index))}");
        }
        return sb.ToString();
    }

    public string UnitsToString()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < 9; i++)
            sb.AppendLine($"Row {i}: {string.Join(' ', Rows[i].Cells.Select(c => c.Index))}");

        for (int i = 0; i < 9; i++)
            sb.AppendLine($"Col {i}: {string.Join(' ', Columns[i].Cells.Select(c => c.Index))}");

        for (int i = 0; i < 9; i++)
            sb.AppendLine($"Box {i}: {string.Join(' ', Boxes[i].Cells.Select(c => c.Index))}");

        return sb.ToString();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                sb.Append(Cells[row * 9 + col].Value);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
