using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImportUI.MVVM;

public partial class ExtractGridViewModel : ObservableObject
{
    private readonly ImageImporter importer;

    [ObservableProperty]
    private Image<Rgb, byte> inputImage = null!;
    
    [ObservableProperty]
    private Image<Rgb, byte> gridImage = null!;

    [ObservableProperty]
    private int margin = 0;

    public ExtractGridViewModel(MainViewModel main, ImageImporter importer)
    {
        this.importer = importer;

        main.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.SelectedImageFilename))
            {
                InputImage = new Image<Rgb, byte>(MainViewModel.path + main.SelectedImageFilename);
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
