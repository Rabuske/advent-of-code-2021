record Beacon(int x, int y, int z){
    public static Beacon operator -(Beacon p1, Beacon p2) {
        return new Beacon(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
    }   

    public int ManhattanDistance(Beacon p) {
        return Math.Abs(this.x - p.x) + Math.Abs(this.y - p.y) +  Math.Abs(this.z - p.z);
    } 
}
record DistanceBetweenPoints(decimal distance, Beacon point1, Beacon point2);

class Scanner {
    public int Id {get; init;}
    public HashSet<Beacon> Detected {get; set;}
    private List<HashSet<Beacon>> _arrangements;  
    private List<DistanceBetweenPoints> _distances;
    

    public Scanner(int id) {
        Id = id;
        Detected = new HashSet<Beacon>();
        _arrangements = new List<HashSet<Beacon>>();
        _distances = new List<DistanceBetweenPoints>();
    }


    public List<HashSet<Beacon>> GetAllArrangements() {
        if(_arrangements.Count() > 0) return _arrangements;
        //positive x
        _arrangements.Add(Detected.Select(p => new Beacon(+p.x,+p.y,+p.z)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(+p.x,-p.z,+p.y)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(+p.x,-p.y,-p.z)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(+p.x,+p.z,-p.y)).ToHashSet());
        //negative x
        _arrangements.Add(Detected.Select(p => new Beacon(p.x,-p.y,+p.z)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(p.x,+p.z,+p.y)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(p.x,+p.y,-p.z)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(p.x,-p.z,-p.y)).ToHashSet());
        //positive y
        _arrangements.Add(Detected.Select(p => new Beacon(+p.y,+p.z,+p.x)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(+p.y,-p.x,+p.z)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(+p.y,-p.z,-p.x)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(+p.y,+p.x,-p.z)).ToHashSet());
        //negative y
        _arrangements.Add(Detected.Select(p => new Beacon(-p.y,-p.z,+p.x)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(-p.y,+p.x,+p.z)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(-p.y,+p.z,-p.x)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(-p.y,-p.x,-p.z)).ToHashSet());
        //positive z
        _arrangements.Add(Detected.Select(p => new Beacon(+p.z,+p.x,+p.y)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(+p.z,-p.y,+p.x)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(+p.z,-p.x,-p.y)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(+p.z,+p.y,-p.x)).ToHashSet());
        //negative z
        _arrangements.Add(Detected.Select(p => new Beacon(-p.z,-p.x,+p.y)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(-p.z,+p.y,+p.x)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(-p.z,+p.x,-p.y)).ToHashSet());
        _arrangements.Add(Detected.Select(p => new Beacon(-p.z,-p.y,-p.x)).ToHashSet());

        return _arrangements;        
    }

    public bool TryCombine(Scanner s, out Scanner result) {
        var arrangements = s.GetAllArrangements();
        result = new Scanner(this.Id);
        // Brute force again

        if(!ShouldTryToCombine(s)) {
            return false;
        }

        foreach (var arrangement in arrangements)
        {
            foreach (var point1 in Detected)
            {
                foreach (var point2 in arrangement)
                {
                    var translation = point2 - point1;
                    if(IsMatching(translation, arrangement)) {
                        result.Detected = this.Detected.Select(s => s).ToHashSet();    
                        foreach (var beacon in arrangement)
                        {
                            result.Detected.Add(beacon - translation);
                        }
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool ShouldTryToCombine(Scanner s)
    {
        var distance1 = s.GetDistancesBetweenPoints();
        var distance2 = GetDistancesBetweenPoints();
        var sameDistance = distance1.Where(d1 => distance2.Any(d2 => d2.distance == d1.distance));
        var distinctPoints = sameDistance.SelectMany(d => new List<Beacon>(){d.point1, d.point2}).Distinct().ToList();
        var candidates = distinctPoints.Select(p1 => sameDistance.Count(p2 => (p2.point1 == p1 || p2.point2 == p1))).Where(c => c >= 12);
        return candidates.Count() > 0;
    }

    private List<DistanceBetweenPoints> GetDistancesBetweenPoints() {
        if(_distances.Count() > 0) return _distances;
        foreach (var p1 in Detected)
        {
            foreach (var p2 in Detected)
            {
                if(p1 == p2) continue;
                _distances.Add(new DistanceBetweenPoints(p1.ManhattanDistance(p2), p1, p2));         
            }   
        }
        return _distances;
    }

    private bool IsMatching(Beacon translation, HashSet<Beacon> arrangement)
    {
        var tranlatedPoints = arrangement.Select(p => p - translation);

        var resulting = tranlatedPoints.Where(t => Detected.Contains(t));
        return resulting.Count() >= 12;
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
            parsed.Last().Detected.Add(new Beacon(numbers[0], numbers[1], numbers[2]));
        }

        return parsed;
    }

    public string Execute() {
        var scanners = Parse(new FileReader(19).Read().ToList());

        var scannersToInclude = scanners.Skip(1).ToList();
        var resultScanner = scanners.First();

        var scannedPairs = new HashSet<(Scanner p1, Scanner p2)>();
        while (scanners.Count() > 1)
        {
            Console.WriteLine($"Scanners left: {scanners.Count()}");

            var removeScanners = new List<Scanner>();

            for (int i = 0; i < scanners.Count() - 1; i++)
            {
                for (int j = i + 1; j < scanners.Count(); j++)
                {
                    if (scannedPairs.Contains((scanners[i], scanners[j])))
                    {
                        continue;
                    }

                    scannedPairs.Add((scanners[i], scanners[j]));

                    var (first, last) = scanners[i].Detected.Count() > scanners[j].Detected.Count()? (scanners[j], scanners[i]) : (scanners[i], scanners[j]);

                    if (first.TryCombine(last, out var resulting))
                    {
                        Console.WriteLine($"Found pair {i} => {j}");

                        scanners[i] = resulting;
                        removeScanners.Add(scanners[j]);
                    }
                }
                removeScanners.ForEach(x => scanners.Remove(x));
            }   
        }        
        
        
        return $"{resultScanner.Detected.Count()}";
    }
}