using System.Windows.Controls;
using System.Windows;
using SudokuUI.ViewModels;

namespace SudokuUI.Templates;

public class DigitSelectionTemplateSelector : DataTemplateSelector
{
    public DataTemplate DigitTemplate { get; set; } = null!;
    public DataTemplate EmptyTemplate { get; set; } = null!;

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var vm = item as DigitSelectionViewModel;
        if (vm != null && vm.Digit == 0)
        {
            return EmptyTemplate;
        }
        return DigitTemplate;
    }
}
