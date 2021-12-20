record Point3DRecord(int x, int y, int z){
    public static Point3DRecord operator -(Point3DRecord p1, Point3DRecord p2) {
        return new Point3DRecord(p2.x - p1.x, p2.y - p1.y, p2.z - p1.z);
    }   

    public static Point3DRecord operator +(Point3DRecord p1, Point3DRecord p2) {
        return new Point3DRecord(p2.x + p1.x, p2.y + p1.y, p2.z + p1.z);
    }       

    public int ManhattanDistance(Point3DRecord p) {
        return Math.Abs(this.x - p.x) + Math.Abs(this.y - p.y) +  Math.Abs(this.z - p.z);
    } 
}
record DistanceBetweenPoints(decimal distance, Point3DRecord point1, Point3DRecord point2);

class Scanner {
    public int Id {get; init;}
    public HashSet<Point3DRecord> Beacons {get; set;}
    private List<HashSet<Point3DRecord>> _arrangements;  

    private Dictionary<Point3DRecord, Point3DRecord> _vectorBetweenBeacons;

    public Point3DRecord Translation {get; set;} 
    

    public Scanner(int id) {
        Id = id;
        Beacons = new HashSet<Point3DRecord>();
        _arrangements = new List<HashSet<Point3DRecord>>();
        _vectorBetweenBeacons = new Dictionary<Point3DRecord, Point3DRecord>();
        Translation = new (0, 0, 0);
    }


    public IEnumerable<Func<Point3DRecord, Point3DRecord>> GetArrangementFunctions() {
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

    private bool IsValidArrangement(HashSet<Point3DRecord> beacons, Func<Point3DRecord, Point3DRecord> arrange, out Point3DRecord translation)
    {
        var vectorsBetweenPoints = GetVectorsBetweenBeacons();
        int count = 0;
        foreach (var b1 in beacons)
        {
            Point3DRecord b1Rotated = arrange(b1);
            foreach (var b2 in beacons)
            {
                if (b1 == b2) continue;

                Point3DRecord b2Rotated = arrange(b2);
                Point3DRecord vector = b1Rotated - b2Rotated;

                if (vectorsBetweenPoints.ContainsKey(vector) && ++count == 11)
                {
                    translation = b1Rotated - vectorsBetweenPoints[vector];
                    return true;
                }
            }
        }

        translation = new (0,0,0);
        return false;
    }

    private Dictionary<Point3DRecord, Point3DRecord> GetVectorsBetweenBeacons()
    {
        if(_vectorBetweenBeacons.Count() > 0) return _vectorBetweenBeacons;
        foreach (var b1 in Beacons)
        {
            foreach (var b2 in Beacons)
            {
                if (b1 == b2) continue;
                Point3DRecord vector = b2 - b1;
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
            parsed.Last().Beacons.Add(new Point3DRecord(numbers[0], numbers[1], numbers[2]));
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

        var maxDistance = 0;

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