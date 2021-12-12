class Cave {
    public List<Cave> AdjacentCaves {get; init;}
    public string Identifier {get; init;}
    public bool IsSmall => Identifier.All(c => char.IsLower(c));

    public Cave(string id) {
        Identifier = id;
        AdjacentCaves = new List<Cave>();
    }

    public List<List<Cave>> GetPathsTo(Cave destiny, int allowedVisitsToSmallCaves) {
        return GetPathsTo(this, destiny, new List<Cave>(), allowedVisitsToSmallCaves);
    }

    private List<List<Cave>> GetPathsTo(Cave start, Cave destiny, List<Cave> currentPath, int allowedVisitsToSmallCaves) {
        var cloneOfCurrentPath = currentPath.Select(c => c).ToList();
        if(destiny == this) {
            cloneOfCurrentPath.Add(this);
            return new List<List<Cave>>(){ cloneOfCurrentPath };
        }

        // When a small cave is already in the path
        if(IsSmall && cloneOfCurrentPath.Contains(this)) {
            // Remove this path if returning to start node or already contains the max number of visits to small caves
            if(this == start) {
                return new List<List<Cave>>();
            }   
            if(cloneOfCurrentPath.Where(c => c.IsSmall && c != start)
                                 .GroupBy(c => c.Identifier, c => c)
                                 .Any(g => g.Count() >= allowedVisitsToSmallCaves)) {
                return new List<List<Cave>>();
            };            
        }

        cloneOfCurrentPath.Add(this);
        return AdjacentCaves.SelectMany(ac => ac.GetPathsTo(start, destiny, cloneOfCurrentPath, allowedVisitsToSmallCaves)).ToList();
    }
    
}


class Day12 : IDayCommand {

    public string Execute(){
        var paths = new FileReader(12).Read().Select(line => line.Split('-')).ToList();
        var caves = new Dictionary<string, Cave>();
        
        paths.ForEach(path => {
            if(!caves.ContainsKey(path[0])) caves.Add(path[0], new Cave(path[0]));
            if(!caves.ContainsKey(path[1])) caves.Add(path[1], new Cave(path[1]));
            caves[path[0]].AdjacentCaves.Add(caves[path[1]]);
            caves[path[1]].AdjacentCaves.Add(caves[path[0]]);
        });

        var pathsToEndSingleVisit = caves["start"].GetPathsTo(caves["end"], 1);
        var pathsToEndDoubleVisit = caves["start"].GetPathsTo(caves["end"], 2);
        
        return $"The number of Paths that visit small caves most once is {pathsToEndSingleVisit.Count()}" + Environment.NewLine +
               $"The number of Paths that visit a single small cave at most twice is {pathsToEndDoubleVisit.Count()}";
    }
}