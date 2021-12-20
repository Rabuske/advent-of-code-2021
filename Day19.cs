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
    public HashSet<Point3DRecord> Detected {get; set;}
    private List<HashSet<Point3DRecord>> _arrangements;  

    public Point3DRecord Translation {get; set;} 
    

    public Scanner(int id) {
        Id = id;
        Detected = new HashSet<Point3DRecord>();
        _arrangements = new List<HashSet<Point3DRecord>>();
        Translation = new (0, 0, 0);
    }


    public IEnumerable<Func<Point3DRecord, Point3DRecord>> GetArrangements() {
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
        var vectors = GetVectors();

        foreach (var arrangement in GetArrangements())
        {
            if (IsValidArrangement(vectors, s.Detected, arrangement, out var translation))
            {
                result.Detected = this.Detected.Select(s => s).ToHashSet();    
                foreach (var beacon in s.Detected)
                {  
                    s.Translation = translation;
                    var resultingBeacon = arrangement(beacon);
                    resultingBeacon = resultingBeacon + translation;
                    result.Detected.Add(resultingBeacon);
                }
                return true;                
            }
        }

        return false;
    }

    private static bool IsValidArrangement(Dictionary<Point3DRecord, Point3DRecord> vectors, HashSet<Point3DRecord> beacons, Func<Point3DRecord, Point3DRecord> arrangement, out Point3DRecord translation)
    {
        int count = 0;
        foreach (var b1 in beacons)
        {
            Point3DRecord b1Rotated = arrangement(b1);
            foreach (var b2 in beacons)
            {
                if (b1 == b2) continue;

                Point3DRecord b2Rotated = arrangement(b2);
                Point3DRecord vector = b1Rotated - b2Rotated;

                if (vectors.ContainsKey(vector) && ++count == 11)
                {
                    translation = b1Rotated - vectors[vector];
                    return true;
                }
            }
        }

        translation = new (0,0,0);
        return false;
    }

    private Dictionary<Point3DRecord, Point3DRecord> GetVectors()
    {
        Dictionary<Point3DRecord, Point3DRecord> vectors = new();
        foreach (var p1 in Detected)
        {
            foreach (var p2 in Detected)
            {
                if (p1 == p2) continue;
                Point3DRecord vector = p2 - p1;
                if (!vectors.ContainsKey(vector))
                {
                    vectors.Add(vector, p2);
                }
            }
        }
        return vectors;
    }
}

class Day19 : IDayCommand {

    public List<Scanner> Parse(List<string> input) {
        var parsed = new List<Scanner>();

        foreach (var line in input)
        {
            if(line.Length <= 1) continue;
            if(line.StartsWith("---")){
                var scannerId = int.Parse(line.Replace("--- scanner", "").Replace("-", "").Trim());
                parsed.Add(new Scanner(scannerId));
                continue;
            }
            var numbers = line.Split(",").Select(i => int.Parse(i)).ToArray();
            parsed.Last().Detected.Add(new Point3DRecord(numbers[0], numbers[1], numbers[2]));
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
        
        return $"The number of beacons is {resultScanner.Detected.Count()} and the max distance between scanners is {maxDistance}";
    }
}