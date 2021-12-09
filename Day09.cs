class MapLocation {
    public int Height {get;set;}
    public List<MapLocation> Neighbors {get;set;}

    public MapLocation(int height) {
        Height = height;
        Neighbors = new List<MapLocation>();
    }

    public bool IsALowPoint() {
        return Height < Neighbors.Select(n => n.Height).Min();
    }

    public List<MapLocation> GetBasin() {
        var flowingNeighbors = Neighbors.Where(n => n.Height > Height);
        var accountedLocations = new HashSet<MapLocation>();
        accountedLocations.Add(this);
        return flowingNeighbors.SelectMany(n => n.GetBasin(accountedLocations)).Append(this).ToList();
    }

    public List<MapLocation> GetBasin(HashSet<MapLocation> alreadyAccountedLocations) {
        if(alreadyAccountedLocations.Contains(this) || Height == 9) return new List<MapLocation>();
        alreadyAccountedLocations.Add(this);
        var flowingNeighbors = Neighbors.Where(n => n.Height > Height);
        return flowingNeighbors.SelectMany(n => n.GetBasin(alreadyAccountedLocations)).Append(this).ToList();
    }

}

class Day09 : IDayCommand {

    public string Execute() {

        var map = new FileReader(09)
            .Read()
            .Select(line => line.Select(element => {
                return new MapLocation((int)Char.GetNumericValue(element));
            })
            .ToList())
            .ToList();

        // Set neighbors
        for (int i = 0; i < map.Count(); i++)
        {
            for (int j = 0; j < map[i].Count(); j++)
            {
                var currentLocation = map[i][j];
                if(i - 1 >= 0) currentLocation.Neighbors.Add(map[i-1][j]);
                if(i + 1 < map.Count()) currentLocation.Neighbors.Add(map[i+1][j]);
                if(j - 1 >= 0) currentLocation.Neighbors.Add(map[i][j-1]);
                if(j + 1 < map[i].Count()) currentLocation.Neighbors.Add(map[i][j+1]);
            }                
        }        

        var flatMap = map.SelectMany(m => m);

        var lowestPoints = flatMap.Where(location => location.IsALowPoint());
        var riskLevel = lowestPoints.Sum(location => location.Height + 1);
        var basins = lowestPoints.Select(lp => lp.GetBasin());
        var valueOfLargestBasins = basins.OrderByDescending(b => b.Count()).Take(3).Aggregate(1, (r, b) => r * b.Count());

        return $"The risk level is {riskLevel} and the multiplied size of the 3 largest basins is {valueOfLargestBasins}";
    }

}