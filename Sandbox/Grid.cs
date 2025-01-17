using System.Text;

namespace Sandbox;

public class Grid
{
    public int[] Values { get; init; } = new int[81];
    public int[] Candidates { get; init; } = new int[81];
    public int[][] Peers { get; init; } = new int[81][];

    public Grid()
    {
        var peers_set = new HashSet<int>();

        // Initialize the candidates with all possible values (1-9 represented as bits 1-9)
        for (int i = 0; i < 81; i++)
        {
            Values[i] = 0;
            Candidates[i] = 0b111111111; // Binary: 111111111 (all digits possible)

            peers_set.Clear();
            // Find peers in row
            for (int j = 0; j < 9; j++)
            {
                peers_set.Add(i - i % 9 + j);
            }
            // Find peers in column
            for (int j = 0; j < 9; j++)
            {
                peers_set.Add(i % 9 + j * 9);
            }
            // Find peers in box
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    peers_set.Add(i - i % 27 + j * 9 + k + i % 9 - i % 3);
                }
            }
            peers_set.Remove(i);
            Peers[i] = peers_set.Order().ToArray();
        }
    }

    public Grid(string puzzle) : this()
    {
        for (int i = 0; i < 81; i++)
        {
            if (puzzle[i] != '.')
                SetValue(i, puzzle[i] - '0');
        }
    }

    public void SetValue(int cell, int value)
    {
        Values[cell] = value;
        Candidates[cell] = 0;
    }

    public int CountEmptyCells() => Values.Count(x => x == 0);

    public bool IsSolved() => Values.All(x => x != 0);

    public int CandidateCount(int cell) => CountBits(Candidates[cell]);

    // Method to count the number of bits set in an integer
    private static int CountBits(int number)
    {
        int count = 0;
        while (number != 0)
        {
            count += number & 1;
            number >>= 1;
        }
        return count;
    }

    // Convert a candidate bitmask back to the value (if only one bit is set)
    public static int CandidateToValue(int candidate)
    {
        if (CountBits(candidate) != 1) throw new InvalidOperationException("Candidate must have exactly one bit set.");
        int value = 0;
        while (candidate > 1)
        {
            candidate >>= 1;
            value++;
        }
        return value + 1;
    }

    public string PrintCandidates()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 81; i++)
        {
            sb.Append(i);
            sb.Append(": ");
            sb.Append(Convert.ToString(Candidates[i], 2));
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                sb.Append(Values[row*9+col]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
