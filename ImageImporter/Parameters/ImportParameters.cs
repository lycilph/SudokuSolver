﻿namespace ImageImporter.Parameters;

public class ImportParameters
{
    public GridExtractionParameters GridParameters { get; private set; }
    public List<CellsExtractionParameters> CellsParameters { get; private set; }
    public List<NumberRecognitionParameters> NumberParameters { get; private set; }

    public ImportParameters()
    {
        GridParameters = new GridExtractionParameters(0, 3000);
        CellsParameters = [new(5, 1), new(5, 3), new(2, 3), new(10, 3)];
        NumberParameters = 
            [new(5, 1, 1, 0), new(5, 1, 1, 1), new(5, 2, 1, 1), 
             new(2, 3, 1, 1), new(3, 1, 1, 1), new(1, 5, 1, 1), 
             new(5, 5, 1, 2), new(1, 2, 1, 1), new(2, 3, 3, 2),
             new(2, 5, 5, 2), new(2, 5, 9, 2), new(2, 3, 5, 2),
             new(3, 3, 7, 2)];
    }
}
