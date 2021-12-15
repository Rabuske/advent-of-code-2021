class RiskLevel {
    public int Value {get; set; }
    public List<RiskLevel> AdjacentNodes {get; set;}

    public RiskLevel(int value) {
        Value = value;
        AdjacentNodes = new List<RiskLevel>();
    }

}

struct RiskAndPath {
    public int RiskLevelSum {get; set;}
    public List<RiskLevel> Path {get; set;}

    public RiskAndPath(int risk, List<RiskLevel> path){
        RiskLevelSum = risk;
        Path = path;
    }
}

class Day15 : IDayCommand {

    public List<RiskLevel> GetOptimalPath(List<List<RiskLevel>> map) {
        RiskLevel endNode = map[map.Count() - 1][map[0].Count() - 1];

        var alreadyVisitedNodes = new HashSet<RiskLevel>();
        var paths = new List<RiskAndPath>();

        paths.Add(new RiskAndPath(0, new List<RiskLevel>(){map[0][0]}));

        do{
            var currentPath = paths.First();
            paths.RemoveAt(0);
            
            var currentNode = currentPath.Path.Last();
            if(currentNode == endNode) {
                return currentPath.Path;
            }
            if(alreadyVisitedNodes.Contains(currentNode)){
                continue;
            }
            alreadyVisitedNodes.Add(currentNode);

            paths = currentNode.AdjacentNodes.Except(alreadyVisitedNodes)
                .Select(adjacent => new RiskAndPath(
                    currentPath.RiskLevelSum + adjacent.Value,
                    currentPath.Path.Select(p => p).Append(adjacent).ToList()
                )
            ).Concat(paths).OrderBy(p => p.RiskLevelSum).ToList();

        } while(true);
    }

    private static void GenerateAdjacent(List<List<RiskLevel>> map)
    {
        for (int i = 0; i < map.Count(); i++)
        {
            for (int j = 0; j < map[i].Count(); j++)
            {
                var currentRisk = map[i][j];
                if (i - 1 >= 0) currentRisk.AdjacentNodes.Add(map[i - 1][j]);
                if (i + 1 < map.Count()) currentRisk.AdjacentNodes.Add(map[i + 1][j]);
                if (j - 1 >= 0) currentRisk.AdjacentNodes.Add(map[i][j - 1]);
                if (j + 1 < map[i].Count()) currentRisk.AdjacentNodes.Add(map[i][j + 1]);
            }
        }
    }

    public List<List<RiskLevel>> GenerateExtendedMap(List<List<RiskLevel>> map, int expansionSize) {
        // Extend lines
        var extendedMapLines = map.Select(line => {
            var resultingLine = new List<RiskLevel>();
            for (int i = 0; i < expansionSize; i++)
            {
                resultingLine.AddRange(line.Select(item => {
                    var newRisk = item.Value + i;
                    newRisk = newRisk > 9 ? newRisk % 9 : newRisk;
                    return new RiskLevel(newRisk);   
                }));
            }
            return resultingLine;
        }).ToList();

        var extendedMap = extendedMapLines.Select(l => l).ToList();

        // Extend columns
        for (int i = 1; i < expansionSize; i++)
        {
            extendedMapLines.ForEach(line => {
                var newLine = line.Select(item => {
                    var newRisk = item.Value + i;
                    newRisk = newRisk > 9 ? newRisk % 9 : newRisk;
                    return new RiskLevel(newRisk);  
                }).ToList();
                extendedMap.Add(newLine);
            });
        }

        return extendedMap;
    }

    public string Execute()
    {
        var shortMap = new FileReader(15).Read()
            .Select(line => line
                .Select(i => new RiskLevel((int)char.GetNumericValue(i)))
                .ToList())
            .ToList();

        
        var longMap = GenerateExtendedMap(shortMap, 5);
        
        shortMap[0][0].Value = 0;    
        longMap[0][0].Value = 0;    

        // Set the adjacent ones
        GenerateAdjacent(shortMap);
        GenerateAdjacent(longMap);
        
        var optimalPathShortVersion = GetOptimalPath(shortMap);
        var totalRiskLevelShortVersion = optimalPathShortVersion.Sum(p => p.Value);


        var optimalPathLongVersion = GetOptimalPath(longMap);
        var totalRiskLevelLongVersion = optimalPathLongVersion.Sum(p => p.Value);        

        return $"Total risk Level for the short version is {totalRiskLevelShortVersion}" + Environment.NewLine +
               $"Total risk Level for the larger version is {totalRiskLevelLongVersion}";
    }
}
