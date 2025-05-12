using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImportUI.MVVM;

public partial class RecognizeDigitViewModel : ObservableObject
{
    private readonly ImageImporter importer;

    [ObservableProperty]
    private ExtractDigitsViewModel digitsVM;

    [ObservableProperty]
    private Image<Rgb, byte> image = null!;

    [ObservableProperty]
    private Image<Gray, byte> processed = null!;

    [ObservableProperty]
    private string digit = string.Empty;

    [ObservableProperty]
    private int kernelSize = 1;
    
    [ObservableProperty]
    private int lowerThreshold = 5;

    [ObservableProperty]
    private int iterations = 1;

    [ObservableProperty]
    private int operation = 1;

    public RecognizeDigitViewModel(ExtractDigitsViewModel digitsVM, ImageImporter importer)
    {
        this.importer = importer;

        DigitsVM = digitsVM;
    }

    [RelayCommand]
    private void Update()
    {
        if (DigitsVM.SelectedCell == null)
            return;

        importer.CleanupCell(DigitsVM.SelectedCell, LowerThreshold, KernelSize, Iterations, Operation);
        Image = DigitsVM.SelectedCell.Image;
        Processed = DigitsVM.SelectedCell.Processed;
        Digit = DigitsVM.SelectedCell.Digit;
    }
}
