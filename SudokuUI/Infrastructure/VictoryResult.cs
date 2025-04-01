namespace SudokuUI.Infrastructure;

public class VictoryResult(VictoryResult.ResultType result)
{
    public enum ResultType { NewGame, Clear, Reset };

    public ResultType Result { get; set; } = result;
}
