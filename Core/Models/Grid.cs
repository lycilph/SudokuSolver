using System.Text;
using Core.Extensions;

namespace Core.Models;

public class Grid
{
    public static readonly IEnumerable<int> PossibleValues = Enumerable.Range(1, 9);
    public static readonly IEnumerable<int> AllCellIndices = Enumerable.Range(0, 81);
    public static readonly IEnumerable<int> AllUnitIndices = Enumerable.Range(0, 9);

    public static int Size() => AllCellIndices.Count();

    public Cell[] Cells { get; set; }
    public Unit[] Rows { get; set; }
    public Unit[] Columns { get; set; }
    public Unit[] Boxes { get; set; }
    public Unit[] AllUnits { get; set; }
    public Chute[] HorizontalChutes { get; set; }
    public Chute[] VerticalChutes { get; set; }
    public Chute[] AllChutes { get; set; }

    public Grid()
    {
        Cells = AllCellIndices.Select(i => new Cell(i)).ToArray();

        Rows = AllUnitIndices
            .Select(r => new Unit { Name = "Row", Index = r, Cells = [.. GetRowIndices(r).Select(i => Cells[i])] }).ToArray();

        Columns = AllUnitIndices
            .Select(c => new Unit { Name = "Column", Index = c, Cells = [.. GetColumnIndices(c).Select(i => Cells[i])] }).ToArray();

        Boxes = AllUnitIndices
            .Select(c => new Unit { Name = "Box", Index = c, Cells = [.. GetBoxIndices(c).Select(i => Cells[i])] }).ToArray();

        AllUnits = Rows.Concat(Columns).Concat(Boxes).ToArray();

        foreach (var cell in Cells)
            cell.Peers = GetPeerIndices(cell.Index).Select(i => Cells[i]).ToArray();

        //Chutes = new Chute[6];
        HorizontalChutes = new Chute[3];
        VerticalChutes = new Chute[3];
        for (int i = 0; i < 3; i++)
        {
            HorizontalChutes[i] = new Chute { Name = "Chute Horizontal", Index = i, Boxes = Enumerable.Range(0, 3).Select(n => Boxes[n + i * 3]).ToArray() };
            VerticalChutes[i] = new Chute { Name = "Chute Vertical", Index = i, Boxes = Enumerable.Range(0, 3).Select(n => Boxes[n * 3 + i]).ToArray() };
        }
        AllChutes = HorizontalChutes.Concat(VerticalChutes).ToArray();
    }

    public Cell this[int i]
    {
        get => Cells[i];
    }

    public Cell this[int row, int col]
    {
        get => Cells[row * 9 + col];  // Convert 2D indices to 1D
    }

    public void Reset() => Cells.ForEach(c => c.Reset());
    public void FillCandidates() => EmptyCells().ForEach(c => c.FillCandidates());
    public IEnumerable<Cell> FilledCells() => Cells.Where(c => c.IsFilled);
    public IEnumerable<Cell> EmptyCells() => Cells.Where(c => c.IsEmpty);
    public int ClueCount() => Cells.Count(c => c.IsClue);
    public bool IsSolved() => Cells.All(c => c.IsFilled);

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
        int startRow = box / 3 * 3;
        int startCol = box % 3 * 3;
        for (int i = 0; i < 9; i++)
        {
            indices[i] = (startRow + i / 3) * 9 + startCol + i % 3;
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

    public string CandidatesToString(bool skip_cells_with_values = false)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 81; i++)
        {
            if (skip_cells_with_values && Cells[i].IsFilled)
                continue;
            sb.AppendLine($"Cell {i}: {string.Join(' ', Cells[i].Candidates.Order())}");
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

        foreach (var unit in AllUnits)
            sb.AppendLine($"{unit.Name} {unit.Index}: {string.Join(' ', unit.Cells.Select(c => c.Index))}");

        return sb.ToString();
    }

    public string ToSimpleString()
    {
        var sb = new StringBuilder();
        foreach (var cell in Cells)
            if (cell.IsFilled)
                sb.Append(cell.Value);
            else
                sb.Append('.');
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
