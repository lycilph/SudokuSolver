namespace Sandbox;
public static class PuzzleFileReader
{
    public static string ReadPuzzle(string filename)
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

        Console.WriteLine($"There are {puzzles} puzzles in the file.");

        return string.Empty;
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
