namespace Core.Engine;

public class Difficulty(int min, int max)
{
    public int Min { get; private set; } = min;
    public int Max { get; private set; } = max;

    public static Difficulty Easy() => new(1, 3);
    public static Difficulty Medium() => new(3, 5);
    public static Difficulty Hard() => new(5, 8);
    public static Difficulty VeryHard() => new(8, 11);
}
