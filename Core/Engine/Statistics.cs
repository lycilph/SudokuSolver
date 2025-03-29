namespace Core.Engine;

/* Statistics about the solving of a puzzle should be kept here
 */

public class Statistics
{
    public int Iterations { get; set; } = 0;
    public long ElapsedTime { get; set; } = 0;
    public int CluesGiven { get; set; } = 0;
    public Dictionary<string, int> StrategyCount { get; set; } = [];

    public void Reset()
    {
        Iterations = 0;
        ElapsedTime = 0;
        CluesGiven = 0;
    }

    public void IncrementStrategyCount(string strategy)
    {
        if (StrategyCount.ContainsKey(strategy))
            StrategyCount[strategy]++;
        else
            StrategyCount[strategy] = 1;
    }

    public override string ToString()
    {
        return $"Iterations: {Iterations}, Elapsed time: {ElapsedTime} ms, Clues given: {CluesGiven}";
    }
}
