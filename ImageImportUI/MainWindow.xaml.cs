using System.Windows;
using Emgu.CV.Structure;
using Emgu.CV;
using System.IO;
using System.Collections.ObjectModel;

namespace ImageImportUI;

public partial class MainWindow : Window
{
    private string path = "C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\ImageImportTest\\Data\\";
    private ImageImporter importer = new();

    public ObservableCollection<string> Images
    {
        get { return (ObservableCollection<string>)GetValue(ImagesProperty); }
        set { SetValue(ImagesProperty, value); }
    }
    public static readonly DependencyProperty ImagesProperty =
        DependencyProperty.Register(nameof(Images), typeof(ObservableCollection<string>), typeof(MainWindow), new PropertyMetadata(null));

    public string SelectedImage
    {
        get { return (string)GetValue(SelectedImageProperty); }
        set { SetValue(SelectedImageProperty, value); }
    }
    public static readonly DependencyProperty SelectedImageProperty =
        DependencyProperty.Register(nameof(SelectedImage), typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty, OnSelectImageChanged));

    public Image<Rgb, byte> InputImage
    {
        get { return (Image<Rgb, byte>)GetValue(InputImageProperty); }
        set { SetValue(InputImageProperty, value); }
    }
    public static readonly DependencyProperty InputImageProperty =
        DependencyProperty.Register(nameof(InputImage), typeof(Image<Rgb, byte>), typeof(MainWindow), new PropertyMetadata(null));

    public Image<Rgb, byte> GridImage
    {
        get { return (Image<Rgb, byte>)GetValue(GridImageProperty); }
        set { SetValue(GridImageProperty, value); }
    }
    public static readonly DependencyProperty GridImageProperty =
        DependencyProperty.Register(nameof(GridImage), typeof(Image<Rgb, byte>), typeof(MainWindow), new PropertyMetadata(null));

    public Image<Rgb, byte> CellsImage
    {
        get { return (Image<Rgb, byte>)GetValue(CellsImageProperty); }
        set { SetValue(CellsImageProperty, value); }
    }
    public static readonly DependencyProperty CellsImageProperty =
        DependencyProperty.Register(nameof(CellsImage), typeof(Image<Rgb, byte>), typeof(MainWindow), new PropertyMetadata(null));

    public List<Cell> Cells
    {
        get { return (List<Cell>)GetValue(CellsProperty); }
        set { SetValue(CellsProperty, value); }
    }
    public static readonly DependencyProperty CellsProperty =
        DependencyProperty.Register(nameof(Cells), typeof(List<Cell>), typeof(MainWindow), new PropertyMetadata(null));

    public int GridMargin
    {
        get { return (int)GetValue(GridMarginProperty); }
        set { SetValue(GridMarginProperty, value); }
    }
    public static readonly DependencyProperty GridMarginProperty =
        DependencyProperty.Register(nameof(GridMargin), typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    public int CellsCount
    {
        get { return (int)GetValue(CellsCountProperty); }
        set { SetValue(CellsCountProperty, value); }
    }
    public static readonly DependencyProperty CellsCountProperty =
        DependencyProperty.Register(nameof(CellsCount), typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    public int LowerThreshold
    {
        get { return (int)GetValue(LowerThresholdProperty); }
        set { SetValue(LowerThresholdProperty, value); }
    }
    public static readonly DependencyProperty LowerThresholdProperty =
        DependencyProperty.Register(nameof(LowerThreshold), typeof(int), typeof(MainWindow), new PropertyMetadata(5));

    public int Iterations
    {
        get { return (int)GetValue(IterationsProperty); }
        set { SetValue(IterationsProperty, value); }
    }
    public static readonly DependencyProperty IterationsProperty =
        DependencyProperty.Register(nameof(Iterations), typeof(int), typeof(MainWindow), new PropertyMetadata(1));

    public int RecognitionFailures
    {
        get { return (int)GetValue(RecognitionFailuresProperty); }
        set { SetValue(RecognitionFailuresProperty, value); }
    }
    public static readonly DependencyProperty RecognitionFailuresProperty =
        DependencyProperty.Register(nameof(RecognitionFailures), typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        Images = [.. Directory
            .EnumerateFiles(path)
            .Select(s => Path.GetFileName(s))];
        SelectedImage = Images.First();
    }

    private void Update()
    {
        var img = new Image<Rgb, byte>(path + SelectedImage);

        InputImage = img;
        GridImage = importer.ExtractGrid(img, GridMargin, false);
        (CellsImage, Cells, CellsCount) = importer.ExtractCells(GridImage, LowerThreshold, Iterations, false);
        RecognitionFailures = importer.ExtractDigits(Cells);
    }

    private static void OnSelectImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MainWindow win)
            win.Update();
    }

    private void UpdateButtonClick(object sender, RoutedEventArgs e)
    {
        Update();
    }
}