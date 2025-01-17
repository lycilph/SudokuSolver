namespace Sandbox;

/* Links to stuff:
 * https://github.com/farhiongit/sudoku-solver
 * https://github.com/HappyCerberus/sudoku
 * https://hodoku.sourceforge.net/en/techniques.php
 * https://norvig.com/sudoku.html
 * 
 * Techniques:
 * https://rakhman.info/blog/solving-sudoku-with-graph-theory/
 * https://opensourc.es/blog/sudoku/
 * Exact cover problem
 * Bipartite graph matching
 * Constraint satisfaction problem
 * 
 * Example of implementation:
 * https://github.com/kurtanr/SimpleSudokuSolver
 * 
 * Test data:
 * https://stackoverflow.com/questions/59973969/high-quality-test-cases-for-sudoku-solver
 * (info about datasets: https://github.com/t-dillon/tdoku/blob/master/benchmarks/README.md)
 * 
 * Todo:
 * Make a sudoku solver that can solve any sudoku puzzle
 * Make a sudoku generator that can generate any sudoku puzzle
 * Make a sudoku validator that can validate any sudoku puzzle
 * Make a ui in blazor (and hosted on github pages)
 * 
 * Make CountBits and CandidateToValue (both in the Grid class) to a utility class
 * 
 */

internal class Program
{
    static void Main()
    {
        //var puzzle = PuzzleFileReader.ReadPuzzle(@"..\..\..\..\data\puzzles0_kaggle");

        //var g = new Grid(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4"); // Easy
        var g = new Grid("4...3.......6..8..........1....5..9..8....6...7.2........1.27..5.3....4.9........"); // Moderate?
        //var g = new Grid("98.7..6..75..4......3..8.7.8....9.5..3.2..1.....4....6.7...4.3....8..4......1...2"); // Hard
        Console.WriteLine(g);
        Console.WriteLine();

        Solver.Solve(g);
        Console.WriteLine(g);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
