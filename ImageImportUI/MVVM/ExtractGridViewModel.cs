using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImportUI.MVVM;

public partial class ExtractGridViewModel : ObservableObject
{
    private readonly MainViewModel mainVM;
    private readonly ImageImporter importer;

    [ObservableProperty]
    private Image<Rgb, byte> inputImage = null!;
    
    [ObservableProperty]
    private Image<Rgb, byte> gridImage = null!;

    [ObservableProperty]
    private int margin = 0;

    public ExtractGridViewModel(MainViewModel mainVM, ImageImporter importer)
    {
        this.mainVM = mainVM;
        this.importer = importer;

        mainVM.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.SelectedImageFilename))
            {
                InputImage = new Image<Rgb, byte>(MainViewModel.path + mainVM.SelectedImageFilename);
                Update();
            }
        };
    }

    [RelayCommand]
    private void Update()
    {
        GridImage = importer.ExtractGrid(InputImage, Margin, false);
    }
}
