class Day15 : IDayCommand {

    public List<List<Node<int>>> GenerateExtendedGrid(List<List<Node<int>>> grid, int expansionSize) {
        // Extend lines
        var extendedGridLines = grid.Select(line => {
            var resultingLine = new List<Node<int>>();
            for (int i = 0; i < expansionSize; i++)
            {
                resultingLine.AddRange(line.Select(item => {
                    var newRisk = item.Value + i;
                    newRisk = newRisk > 9 ? newRisk % 9 : newRisk;
                    return new Node<int>(newRisk);   
                }));
            }
            return resultingLine;
        }).ToList();

        var extendedGrid = extendedGridLines.Select(l => l).ToList();

        // Extend columns
        for (int i = 1; i < expansionSize; i++)
        {
            extendedGridLines.ForEach(line => {
                var newLine = line.Select(item => {
                    var newRisk = item.Value + i;
                    newRisk = newRisk > 9 ? newRisk % 9 : newRisk;
                    return new Node<int>(newRisk);  
                }).ToList();
                extendedGrid.Add(newLine);
            });
        }

        return extendedGrid;
    }

    public string Execute()
    {
        var shortGrid = new FileReader(15).Read()
            .Select(line => line
                .Select(i => new Node<int>((int)char.GetNumericValue(i)))
                .ToList())
            .ToList();

        
        var longGrid = GenerateExtendedGrid(shortGrid, 5);
        
        shortGrid[0][0].Value = 0;    
        longGrid[0][0].Value = 0;    
        
        var shortGridStart = shortGrid[0][0];
        var shortGridEnd = shortGrid.Last().Last();
        var optimalPathShortVersion = new Map<int>(shortGrid).GetOptimalPath(shortGridStart, shortGridEnd);
        var totalRiskLevelShortVersion = optimalPathShortVersion.Sum(p => p.Value);


        var longGridStart = longGrid[0][0];
        var longGridEnd = longGrid.Last().Last();
        var optimalPathLongVersion = new Map<int>(longGrid).GetOptimalPath(longGridStart, longGridEnd);
        var totalRiskLevelLongVersion = optimalPathLongVersion.Sum(p => p.Value);        

        return $"Total risk Level for the short version is {totalRiskLevelShortVersion}" + Environment.NewLine +
               $"Total risk Level for the larger version is {totalRiskLevelLongVersion}";
    }
}
