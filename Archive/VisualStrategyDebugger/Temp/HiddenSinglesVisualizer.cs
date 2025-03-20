using VisualStrategyDebugger.ViewModels;

namespace VisualStrategyDebugger.Temp;

public class HiddenSinglesVisualizer(HiddenSinglesCommand command) : IVisualizer
{
    public void Show(GridViewModel vm)
    {
        foreach (var element in command.Elements)
        {
            var index = element.Cells.FirstOrDefault()?.Index ?? -1;
            if (index != -1)
            {
                // Map cell to its 
                var cell_vm = vm.Cells[index];
                cell_vm.Candidates[element.Number - 1].Highlight = true;
            }
        }
    }
}
