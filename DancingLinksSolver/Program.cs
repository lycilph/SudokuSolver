using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace DancingLinksSolver;

/*
 * First step:
 * Generate constraint matrix (See here: https://www.stolaf.edu/people/hansonr/sudoku/exactcovermatrix.htm)
 */

internal class Program
{
    private const int Size = 9;
    private const int BoxSize = 3;
    private const int TotalColumns = 324; // 81 (cell) + 81 (row) + 81 (column) + 81 (box)
    private const int TotalRows = 729; // 9 possibilities for each of 81 cells

    static void Main(string[] args)
    {
        //var puzzle = ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4"; // East
        var puzzle = "..............3.85..1.2.......5.7.....4...1...9.......5......73..2.1........4...9"; // Hard for backtracking

        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        var matrix = BuildConstraintMatrix(puzzle);
        Console.WriteLine($"Constraint matrix built with {matrix.Count} rows and {matrix[0].Length} columns");
        
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");

        var filename = "contraint_matrix.txt";
        PrintToFile(matrix, filename);

        stopwatch.Restart();

        var root = CreateDancingLinksMatrix(matrix);
        var solution = new Stack<DLXNode>();
        Search(root, solution, 0);

        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void PrintSolution(Stack<DLXNode> solution)
    {
        var grid = solution.OrderBy(n => n.RowID).Select(n => n.RowID % 9 + 1).ToArray();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
                Console.Write(grid[row*9+col]);
            Console.WriteLine();
        }
    }

    public static void Search(DLXNode root, Stack<DLXNode> solution, int level)
    {
        if (root.Right == root) // All constraints are satisfied!
        {
            Console.WriteLine($"Found solution (level={level})");
            PrintSolution(solution);
            return;
        }

        // Select the smallest column
        var column = SelectColumn((ColumnNode)root);
        //Console.WriteLine($"Selected column {column.Name} (size = {column.Size})");

        Cover(column);

        for (DLXNode row = column.Down; row != column; row = row.Down)
        {
            solution.Push(row);
            for (DLXNode node = row.Right; node != row; node = node.Right)
                Cover(node.Column);

            Search(root, solution, level + 1);

            // Backtrack
            solution.Pop();
            for (DLXNode node = row.Left; node != row; node = node.Left)
                Uncover(node.Column);
        }

        Uncover(column);
    }

    private static void Cover(ColumnNode column)
    {
        // Remove column from the header list
        column.Right.Left = column.Left;
        column.Left.Right = column.Right;

        //Console.WriteLine($"Covering column {column.Name} (size = {column.Size})");

        // Iterate over each row (down) in the column
        for (DLXNode row = column.Down; row != column; row = row.Down)
        {
            // Iterate over all nodes in this row
            for (DLXNode node = row.Right; node != row; node = node.Right)
            {
                //Console.WriteLine($" * Unlinking {node.Column.Name}");

                node.Down.Up = node.Up; // Unlink node vertically
                node.Up.Down = node.Down;
                node.Column.Size--; // Reduce count of nodes in this column
            }
        }
    }

    private static void Uncover(ColumnNode column)
    {
        // Iterate upwards through each row in the column
        for (DLXNode row = column.Up; row != column; row = row.Up)
        {
            // Iterate left through the row, restoring all columns
            for (DLXNode node = row.Left; node != row; node = node.Left)
            {
                node.Down.Up = node;
                node.Up.Down = node;
                node.Column.Size++; // Restore column size
            }
        }

        // Restore column into the header list
        column.Right.Left = column;
        column.Left.Right = column;
    }    
        
    public static ColumnNode SelectColumn(ColumnNode root)
    {
        ColumnNode selected = null!;
        int min_size = int.MaxValue;

        // Start from the root column header and iterate through all columns
        for (ColumnNode column = (ColumnNode)root.Right; column != root; column = (ColumnNode)column.Right)
        {
            if (column.Size < min_size) // Choose the column with the fewest 1s
            {
                selected = column;
                min_size = column.Size;
            }
        }

        return selected;
    }

    public static void PrintToFile(List<int[]> matrix, string filename)
    {
        try
        {
            using (StreamWriter writer = new(filename))
            {
                foreach (var row in matrix)
                {
                    foreach (var col in row)
                        writer.Write(col == 1 ? "1" : " ");
                    writer.WriteLine();
                }
            }
            Console.WriteLine($"Constraint matrix successfully written to {filename}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to file: {ex.Message}");
        }
    }

    public static DLXNode CreateDancingLinksMatrix(List<int[]> matrix)
    {
        var root = new ColumnNode("Root");
        var columns = matrix[0].Length;
        var column_nodes = new List<ColumnNode>();

        // Create column headers
        for (int i = 0; i < columns; i++)
        {
            var column = new ColumnNode("C" + i);
            column_nodes.Add(column);

            root.Left.Right = column;
            column.Left = root.Left;
            column.Right = root;
            root.Left = column;
        }

        // Populate rows
        //foreach (var row in matrix)
        for (int row_index = 0; row_index < matrix.Count; row_index++)
        {
            int[] row = matrix[row_index];
            DLXNode? first_in_row = null; // First node in the row (for circular linking)
            DLXNode? prev = null;

            for (int column_index = 0; column_index < row.Length; column_index++)
            {
                if (row[column_index] == 1)
                {
                    var column = column_nodes[column_index];
                    var node = new DLXNode { Column = column, RowID = row_index };

                    // Store first node in the row (for circular linking)
                    first_in_row ??= node;

                    // prev = node if prev == null
                    prev ??= node;

                    // Link horizontally
                    prev.Right = node;
                    node.Left = prev;

                    prev = node;

                    // Link vertically
                    node.Down = column;
                    node.Up = column.Up;
                    column.Up.Down = node;
                    column.Up = node;

                    column.Size++;
                }
            }

            // Close horizontal circular links
            if (prev != null && first_in_row != null) // Ensure row is not empty
            {
                prev.Right = first_in_row; // Link last node to first node
                first_in_row.Left = prev;  // Link first node to last node
            }
        }

        return root;
    }

    public static List<int[]> BuildConstraintMatrix(string puzzle)
    {
        var matrix = new List<int[]>();

        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                var index = row * 9 + col;

                if (puzzle[index] == '.')
                {
                    for (int num = 0; num < Size; num++)
                    {
                        matrix.Add(CreateConstraintRow(row, col, num));
                    }
                }
                else
                {
                    var hint = puzzle[index] - '0';
                    for (int num = 0; num < Size; num++)
                    {
                        if (num == hint - 1)
                            matrix.Add(CreateConstraintRow(row, col, num));
                        else
                            matrix.Add(new int[TotalColumns]); // Add empty rows to keep the structure of the constraint matrix
                    }
                    //matrix.Add(CreateConstraintRow(row, col, hint - 1));
                }
            }
        }

        return matrix;
    }

    private static int[] CreateConstraintRow(int row, int col, int num)
    {
        int[] constraints = new int[TotalColumns];

        // Compute column indices
        int cellConstraint = row * Size + col;
        int rowConstraint = Size * Size + (row * Size + num);
        int colConstraint = 2 * Size * Size + (col * Size + num);
        int boxIndex = (row / BoxSize) * BoxSize + (col / BoxSize);
        int boxConstraint = 3 * Size * Size + (boxIndex * Size + num);

        // Set the constraints
        constraints[cellConstraint] = 1;
        constraints[rowConstraint] = 1;
        constraints[colConstraint] = 1;
        constraints[boxConstraint] = 1;

        return constraints;
    }
}
