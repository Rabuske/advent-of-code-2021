record BeaconDistance(Point3D beacon1, Point3D beacon2, decimal distance);

class Scanner {
    public int Id {get; init;}
    public List<Point3D> Detected {get; init;}
    private List<BeaconDistance> _distances;    

    List<(Point3D original, List<Point3D> arrangements)> _allArrangements;

    public Scanner(int id) {
        Id = id;
        Detected = new List<Point3D>();
        _distances = new List<BeaconDistance>();
        _allArrangements = new List<(Point3D original, List<Point3D> arrangements)>();
    }

    public List<(Point3D original, List<Point3D> arrangements)> GetAllArrangements() {
        if(_allArrangements.Count() > 0) return _allArrangements;        
        _allArrangements = Detected.Select(beacon => (beacon, GetAllBeaconArrangements(beacon))).ToList();
        return _allArrangements;
    }

    public List<Point3D> GetAllBeaconArrangements(Point3D beacon) {
        var arrangements = new List<Point3D>();
        arrangements.Add(beacon);
        arrangements.Add(new Point3D(-beacon.x, beacon.y, beacon.z));
        arrangements.Add(new Point3D(beacon.x, -beacon.y, beacon.z));
        arrangements.Add(new Point3D(beacon.x, beacon.y, -beacon.z));
        arrangements.Add(new Point3D(-beacon.x, -beacon.y, beacon.z));
        arrangements.Add(new Point3D(beacon.x, -beacon.y, -beacon.z));
        arrangements.Add(new Point3D(-beacon.x, beacon.y, -beacon.z));
        arrangements.Add(new Point3D(-beacon.x, -beacon.y, -beacon.z));

        var arr2 = arrangements.Select(a => new Point3D(a.z, a.x, a.y));
        arr2 = arr2.Concat(arrangements.Select(a => new Point3D(a.y, a.z, a.x)));
        arr2.Concat(arrangements);
        return arr2.ToList();
    }

    public bool TryCombine(Scanner s, out Scanner? result) {
        var arrangements = s.GetAllArrangements();
        result = new Scanner(this.Id);
        // Brute force again
        foreach (var arrangement in arrangements)
        {
            foreach (var point1 in Detected)
            {
                var translation1 = new Point3D(-point1.x, -point1.y, -point1.z);
                foreach (var point2 in arrangement.arrangements)
                {
                    var translation2 = new Point3D(-point2.x, -point2.y, -point2.z);
                    if(IsMatching(translation1, translation2, s)) {
                        result.Detected.AddRange(this.Detected);
                        result.Detected.AddRange(arrangement.arrangements);
                    }
                }
            }
        }

        return false;
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
            var numbers = line.Split(",").Select(i => decimal.Parse(i)).ToArray();
            parsed.Last().Detected.Add(new Point3D(numbers[0], numbers[1], numbers[2]));
        }

        return parsed;
    }

    public string Execute() {
        var scanners = Parse(new FileReader(19).Read().ToList());
        var numberOfBeacons = scanners.Sum(s => s.Detected.Count());


        
        return $"{numberOfBeacons}";
    }
}