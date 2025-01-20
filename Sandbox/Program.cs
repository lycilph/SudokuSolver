using Sandbox.Model;
using Sandbox.Strategies;

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
 * Goals:
 * Make a sudoku solver that can solve any sudoku puzzle
 * Make a sudoku generator that can generate any sudoku puzzle
 * Make a sudoku validator that can validate any sudoku puzzle
 * Make a ui in blazor (and hosted on github pages)
 * 
 * Todo:
 * Make a log of what the solver does (fix outputting from strategies that makes no changes to the grid state)
 * Fix/make proper tests for strategies
 */

internal class Program
{
    static void Main()
    {
        //var puzzle = PuzzleFileReader.ReadPuzzle(@"..\..\..\..\data\puzzles0_kaggle");
        //var g = new Grid(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4"); // Easy
        //var g = new Grid("4...3.......6..8..........1....5..9..8....6...7.2........1.27..5.3....4.9........"); // Moderate?
        var g = new Grid("98.7..6..75..4......3..8.7.8....9.5..3.2..1.....4....6.7...4.3....8..4......1...2"); // Hard
        //var g = new Grid(".6.8...4.....4...22.46....9..1..93...96...45...83.....1.7..32.59.2.5.....35..1.7."); // Hidden singles test
        //var g = new Grid("9..4..1...56.......78..62......23.......5..73....7...........8..................."); // Naked pairs test
        //var g = new Grid(".7.4.8.29..2.....4854.2...7..83742...2.........32617......936122.....4.313.642.7."); // Naked triples test
        //var g = new Grid("....3..86....2..4..9..7852.3718562949..1423754..3976182..7.3859.392.54677..9.4132"); // Naked quads test
        //var g = new Grid(".........9.46.7....768.41..3.97.1.8...8...3...5.3.87.2..75.261....4.32.8........."); // Hidden pairs test
        //var g = new Grid(".16..78.3.9.8.....87...126..48...3..65...9.82.39...65..6.9...2..8...29369246..51."); // Locked candidates (claiming) test
        //var g = new Grid("..............3.85..1.2.......5.7.....4...1...9.......5......73..2.1........4...9"); // Backtracking test (https://en.wikipedia.org/wiki/Sudoku_solving_algorithms#Backtracking)        

        Console.WriteLine(g);
        g = Solver.Solve(g);
        Console.WriteLine(g);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
