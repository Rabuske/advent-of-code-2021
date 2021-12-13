class Day13 : IDayCommand {

    public string Execute() {

        var input = new FileReader(13).Read();
        var map = new List<Point2D>();
        var folds = new List<(char axis, int pos)>();

        foreach (var line in input)
        {
            if(line.Length < 1) continue;
            if(line.StartsWith("fold")) 
            {
                var foldInst = line.Replace("fold along ", "").Split("=").ToArray();
                folds.Add((foldInst[0][0], int.Parse(foldInst[1])));
                continue;
            }
            var coordinates = line.Split(",").Select(n => int.Parse(n)).ToArray();
            map.Add(new Point2D(coordinates[0], coordinates[1]));            
        }

        var singleFold = Fold(map, folds[0]);
        var folded     = folds.Aggregate(map, (map, fold) => Fold(map, fold)).ToList();

        return $"After the first fold, the number of points is {singleFold.Count()}" + Environment.NewLine +  
               "This is the code generated after folding it completely"  + Environment.NewLine + 
               MapToString(folded);
    }

    private string MapToString(List<Point2D> map)
    {
        var maxX = (int?) map.MaxBy(p => p.x)?.x;
        var maxY = (int?) map.MaxBy(p => p.y)?.y;
        if(maxX is null || maxY is null) return "";
        var result = "";
        for (int y = maxY.Value; y >= 0; y--)
        {
            for (int x = maxX.Value; x >=0; x--)
            {
                result += map.Any(p => p.x == x && p.y == y) ? '#' : '.';
            }
            result += Environment.NewLine;
        }
        return result;
    }

    private List<Point2D> Fold(List<Point2D> map, (char axis, int pos) fold)
    {
        var result =  map.Select(point => {
            if(fold.axis == 'x') {
                return new Point2D(Math.Abs(fold.pos - point.x) - 1, point.y);
            } else {
                return new Point2D(point.x, Math.Abs(fold.pos - point.y) - 1);                
            }
        }).Distinct().ToList();        
        return result;
    }
}