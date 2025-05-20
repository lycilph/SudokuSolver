namespace ImageImporter.Parameters;

public class ImportParameters
{
    public GridExtractionParameters GridParameters { get; private set; }
    public List<CellsExtractionParameters> CellsParameters { get; private set; }
    public List<NumberRecognitionParameters> NumberParameters { get; private set; }

    public ImportParameters()
    {
        GridParameters = new GridExtractionParameters(0, 1000);
        CellsParameters = [new(5, 1), new(5, 3), new(2, 3)];
        NumberParameters = [new(5, 1, 1, 0), new(5, 1, 1, 1), new(5, 2, 1, 1), new(2, 3, 1, 1), new(3, 1, 1, 1), new(1, 5, 1, 1), new(5, 5, 1, 2)];
    }
}
