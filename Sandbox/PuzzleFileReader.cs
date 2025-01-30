using Core.Model;

namespace Sandbox;
public class PuzzleFileReader(string filename)
{
    public IEnumerable<Grid> ReadPuzzle()
    {
        using (var reader = new StreamReader(filename))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!line.StartsWith('#'))
                    yield return new Grid(line);
            }
        }
    }

    public static int CountPuzzles(string filename)
    {
        var puzzles = 0;

        // Ensure the file exists before attempting to read it
        if (File.Exists(filename))
        {
            try
            {
                // Read the file line by line
                using StreamReader reader = new(filename);
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.StartsWith('#'))
                        puzzles++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("The file does not exist.");
        }

        return puzzles;
    }
}
