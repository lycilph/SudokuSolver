namespace Core.Models;

/* Statistics about the solving of a puzzle should be kept here
 */

public class Statistics
{
    public int Iterations { get; set; } = 0;
    public long ElapsedTime { get; set; } = 0;
    public int CluesGiven { get; set; } = 0;

    public void Reset()
    {
        Iterations = 0;
        ElapsedTime = 0;
        CluesGiven = 0;
    }

    public override string ToString()
    {
        return $"Iterations: {Iterations}, Elapsed time: {ElapsedTime} ms, Clues given: {CluesGiven}";
    }
}
