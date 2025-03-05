namespace Core.Model;

public interface ISolveAction
{
    string Description { get; set; }
    List<SolveActionElement> Elements { get; set; }

    void Apply();
    void Undo();

    string ToString();
}
