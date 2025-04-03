namespace Core.Engine;

public class Difficulty(int min, int max, string name)
{
    public int Min { get; private set; } = min;
    public int Max { get; private set; } = max;
    public string Name { get; private set; } = name;

    public static Difficulty Easy() => new(1, 3, "Easy");
    public static Difficulty Medium() => new(3, 5, "Medium");
    public static Difficulty Hard() => new(5, 8, "Hard");
    public static Difficulty VeryHard() => new(8, 11, "Very Hard");
    public static List<Difficulty> All => [Easy(), Medium(), Hard(), VeryHard()];

    public static string GetDifficultyName(int difficulty) => All.First(d => difficulty >= d.Min && difficulty <= d.Max).Name;
}
