﻿using VisualStrategyDebugger.ViewModels;

namespace VisualStrategyDebugger.Temp;

public class BasicEliminationVisualizer(BasicEliminationCommand command) : IVisualizer
{
    public void Show(GridViewModel vm)
    {
        foreach (var element in command.Elements)
        {
            foreach (var cells in element.Cells)
            {
                // Map cell to its vm
                var cell_vm = vm.Cells[cells.Index];
                cell_vm.Candidates[element.Number-1].Highlight = true;
            }
        }
    }
}
