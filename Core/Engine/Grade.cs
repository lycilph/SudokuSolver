namespace Core.Engine;

public class Grade(int difficulty, int effort)
{
    public int Difficulty { get; set; } = difficulty;
    public int Effort { get; set; } = effort;

    public override string ToString() => $"Difficulty: {Difficulty}, Effort: {Effort}";
}
