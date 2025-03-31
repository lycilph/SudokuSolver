using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using SudokuUI.Messages;
using SudokuUI.ViewModels;

namespace SudokuUI.Views;

public partial class MainWindow
{
    private InputBindingCollection original_input_bindings;

    public MainWindow()
    {
        InitializeComponent();

        original_input_bindings = new InputBindingCollection(InputBindings);

        Loaded += MainWindowLoaded;
        //DataContextChanged += MainWindowDataContextChanged;
    }

    //private void MainWindowDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    //{
    //    if (DataContext is MainViewModel vm)
    //    {
    //        vm.DisableKeybindingsRequest += OnDisableKeybindingsRequest;
    //        vm.EnableKeybindingsRequest += OnEnableKeybindingsRequest;
    //    }
    //}

    //private void OnEnableKeybindingsRequest(object? sender, EventArgs e)
    //{
    //    InputBindings.AddRange(original_input_bindings);
    //}

    //private void OnDisableKeybindingsRequest(object? sender, EventArgs e)
    //{
    //    InputBindings.Clear();
    //}

    private void MainWindowLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= MainWindowLoaded;
        WeakReferenceMessenger.Default.Send(new MainWindowLoadedMessage());
    }
}