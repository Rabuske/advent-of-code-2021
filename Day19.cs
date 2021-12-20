class Scanner {
    public int Id {get; init;}
    public HashSet<Point3D> Beacons {get; set;}
    private List<HashSet<Point3D>> _arrangements;  

    private Dictionary<Point3D, Point3D> _vectorBetweenBeacons;

    public Point3D Translation {get; set;} 
    

    public Scanner(int id) {
        Id = id;
        Beacons = new HashSet<Point3D>();
        _arrangements = new List<HashSet<Point3D>>();
        _vectorBetweenBeacons = new Dictionary<Point3D, Point3D>();
        Translation = new (0, 0, 0);
    }


    public IEnumerable<Func<Point3D, Point3D>> GetArrangementFunctions() {
        yield return v => new(v.x, -v.z, v.y);
        yield return v => new(v.x, -v.y, -v.z);
        yield return v => new(v.x, v.z, -v.y);

        yield return v => new(-v.y, v.x, v.z);
        yield return v => new(v.z, v.x, v.y);
        yield return v => new(v.y, v.x, -v.z);
        yield return v => new(-v.z, v.x, -v.y);

        yield return v => new(-v.x, -v.y, v.z);
        yield return v => new(-v.x, -v.z, -v.y);
        yield return v => new(-v.x, v.y, -v.z);
        yield return v => new(-v.x, v.z, v.y);

        yield return v => new(v.y, -v.x, v.z);
        yield return v => new(v.z, -v.x, -v.y);
        yield return v => new(-v.y, -v.x, -v.z);
        yield return v => new(-v.z, -v.x, v.y);

        yield return v => new(-v.z, v.y, v.x);
        yield return v => new(v.y, v.z, v.x);
        yield return v => new(v.z, -v.y, v.x);
        yield return v => new(-v.y, -v.z, v.x);

        yield return v => new(-v.z, -v.y, -v.x);
        yield return v => new(-v.y, v.z, -v.x);
        yield return v => new(v.z, v.y, -v.x);
        yield return v => new(v.y, -v.z, -v.x);
    }

    public bool TryCombine(Scanner s, out Scanner result) {
        result = new Scanner(this.Id);            

        foreach (var arrange in GetArrangementFunctions())
        {
            if (IsValidArrangement(s.Beacons, arrange, out var translation))
            {
                result.Beacons = this.Beacons.Select(s => s).ToHashSet();    
                foreach (var beacon in s.Beacons)
                {  
                    s.Translation = translation;
                    var resultingBeacon = arrange(beacon);
                    resultingBeacon = resultingBeacon + translation;
                    result.Beacons.Add(resultingBeacon);
                }
                return true;                
            }
        }

        return false;
    }

    private bool IsValidArrangement(HashSet<Point3D> beacons, Func<Point3D, Point3D> arrange, out Point3D translation)
    {
        var vectorsBetweenPoints = GetVectorsBetweenBeacons();
        int count = 0;
        foreach (var b1 in beacons)
        {
            Point3D b1Rotated = arrange(b1);
            foreach (var b2 in beacons)
            {
                if (b1 == b2) continue;

                Point3D b2Rotated = arrange(b2);
                Point3D vector = b2Rotated - b1Rotated;

                if (vectorsBetweenPoints.ContainsKey(vector) && ++count == 11)
                {
                    translation = vectorsBetweenPoints[vector] - b1Rotated;
                    return true;
                }
            }
        }

        translation = new (0,0,0);
        return false;
    }

    private Dictionary<Point3D, Point3D> GetVectorsBetweenBeacons()
    {
        if(_vectorBetweenBeacons.Count() > 0) return _vectorBetweenBeacons;
        foreach (var b1 in Beacons)
        {
            foreach (var b2 in Beacons)
            {
                if (b1 == b2) continue;
                Point3D vector = b1 - b2;
                if (!_vectorBetweenBeacons.ContainsKey(vector))
                {
                    _vectorBetweenBeacons.Add(vector, b2);
                }
            }
        }
        return _vectorBetweenBeacons;
    }
}

class Day19 : IDayCommand {

    public List<Scanner> Parse(List<string> input) {
        var parsed = new List<Scanner>();

        foreach (var line in input)
        {
            if(string.IsNullOrEmpty(line)) continue;
            if(line.StartsWith("---")){
                var scannerId = int.Parse(line.Replace("--- scanner", "").Replace("-", "").Trim());
                parsed.Add(new Scanner(scannerId));
                continue;
            }
            var numbers = line.Split(",").Select(i => int.Parse(i)).ToArray();
            parsed.Last().Beacons.Add(new Point3D(numbers[0], numbers[1], numbers[2]));
        }

        return parsed;
    }

    public string Execute() {
        var scanners = Parse(new FileReader(19).Read().ToList());

        var resultScanner = scanners.First();
        var remainingScanners = scanners.Skip(1).ToList();

        while(remainingScanners.Count() > 0) {
            var scannerToRemove = new List<Scanner>();
            foreach (var scanner in remainingScanners)
            {
                if(resultScanner.TryCombine(scanner, out var combined)){
                    scannerToRemove.Add(scanner);
                    resultScanner = combined;
                }   
            }
            scannerToRemove.ForEach(x => remainingScanners.Remove(x));
        }

        decimal maxDistance = 0;

        for (int i = 0; i < scanners.Count() - 1; i++)
        {
            for (int j = i + 1; j < scanners.Count(); j++)
            {
                var distance = scanners[i].Translation.ManhattanDistance(scanners[j].Translation);
                maxDistance = Math.Max(maxDistance, distance);
            }            
        }
        
        return $"The number of beacons is {resultScanner.Beacons.Count()} and the max distance between scanners is {maxDistance}";
    }
}