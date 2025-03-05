namespace Core.Model;

/* Statistics about the solving of a puzzle should be kept here
 */
public class Statistics
{
    public int Iterations { get; set; } = 0;
    public long ElapsedTime { get; set; } = 0;

    public void Reset()
    {
        Iterations = 0;
        ElapsedTime = 0;
    }
}
