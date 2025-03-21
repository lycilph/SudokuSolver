using Core.Models;
using Core.Strategies;
using System.Diagnostics;

namespace Core.DancingLinks;

/*
 * Info:
 * constraint matrix (See here: https://www.stolaf.edu/people/hansonr/sudoku/exactcovermatrix.htm)
 * https://www.geeksforgeeks.org/implementation-of-exact-cover-problem-and-algorithm-x-using-dlx/?utm_source=chatgpt.com
 */

public static class DancingLinksSolver
{
    private const int Size = 9;
    private const int BoxSize = 3;
    private const int TotalColumns = 324; // 81 (cell) + 81 (row) + 81 (column) + 81 (box)

    public static (List<Grid>, Statistics) Solve(Grid grid, bool run_basic_elimination = true, int max_solutions_to_find = 0)
    {
        if (run_basic_elimination)
            BasicEliminationStrategy.PlanAndExecute(grid);

        Stopwatch stopwatch = Stopwatch.StartNew();

        // Setup the data structures
        var constraint_matrix = BuildConstraintMatrix(grid);
        var root = CreateDancingLinksMatrix(constraint_matrix);
        var solution = new Stack<DLXNode>();
        var all_solutions = new List<int[]>();

        // Run the dancing links algorithm
        Search(root, solution, 0, all_solutions, max_solutions_to_find);

        stopwatch.Stop();

        // Fill in statistics
        var stats = new Statistics
        {
            ElapsedTime = stopwatch.ElapsedMilliseconds,
            CluesGiven = grid.ClueCount()
        };

        // Get solutions
        var solutions = all_solutions.Select(ExtractSolution).ToList();

        return (solutions, stats);
    }

    private static List<int[]> BuildConstraintMatrix(Grid grid)
    {
        var matrix = new List<int[]>();

        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                var cell = grid[row, col];
                if (cell.IsFilled)
                {
                    for (int num = 0; num < Size; num++)
                    {
                        if (num == cell.Value - 1)
                            matrix.Add(CreateConstraintRow(row, col, num));
                        else
                            matrix.Add(new int[TotalColumns]); // Add empty rows to keep the structure of the constraint matrix
                    }
                }
                else
                {
                    for (int num = 0; num < Size; num++)
                    {
                        if (cell.Candidates.Contains(num + 1))
                            matrix.Add(CreateConstraintRow(row, col, num));
                        else
                            matrix.Add(new int[TotalColumns]); // Add empty rows to keep the structure of the constraint matrix
                    }
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
        int rowConstraint = Size * Size + row * Size + num;
        int colConstraint = 2 * Size * Size + col * Size + num;
        int boxIndex = row / BoxSize * BoxSize + col / BoxSize;
        int boxConstraint = 3 * Size * Size + boxIndex * Size + num;

        // Set the constraints
        constraints[cellConstraint] = 1;
        constraints[rowConstraint] = 1;
        constraints[colConstraint] = 1;
        constraints[boxConstraint] = 1;

        return constraints;
    }

    private static DLXNode CreateDancingLinksMatrix(List<int[]> matrix)
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

                    // Store first node in the row (for circular linking) if it is null
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

    private static void Search(DLXNode root, Stack<DLXNode> solution, int level, List<int[]> all_solutions, int max_solutions_to_find)
    {
        if (max_solutions_to_find > 0 && all_solutions.Count >= max_solutions_to_find)
            return;

        if (root.Right == root) // All constraints are satisfied!
        {
            all_solutions.Add(solution.OrderBy(n => n.RowID).Select(n => n.RowID % 9 + 1).ToArray());
            return;
        }

        // Select the smallest column
        var column = SelectColumn((ColumnNode)root);

        Cover(column);

        for (DLXNode row = column.Down; row != column; row = row.Down)
        {
            solution.Push(row);
            for (DLXNode node = row.Right; node != row; node = node.Right)
                Cover(node.Column);

            Search(root, solution, level + 1, all_solutions, max_solutions_to_find);

            // Backtrack
            solution.Pop();
            for (DLXNode node = row.Left; node != row; node = node.Left)
                Uncover(node.Column);
        }

        Uncover(column);
    }

    private static ColumnNode SelectColumn(ColumnNode root)
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

    private static void Cover(ColumnNode column)
    {
        // Remove column from the header list
        column.Right.Left = column.Left;
        column.Left.Right = column.Right;

        // Iterate over each row (down) in the column
        for (DLXNode row = column.Down; row != column; row = row.Down)
        {
            // Iterate over all nodes in this row
            for (DLXNode node = row.Right; node != row; node = node.Right)
            {
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

    private static Grid ExtractSolution(int[] solution)
    {
        var grid = new Grid();

        for (int i = 0; i < solution.Length; i++)
            grid.Cells[i].Value = solution[i];

        return grid;
    }
}