class Day05 : IDayCommand {

    private int CalculateNumberOfCrossingPoints(List<LineSegment> segments) {
        HashSet<Point2D> crossingPoints = new HashSet<Point2D>();
        for (int i = 0; i < segments.Count() - 1; i++)
        {
            for (int j = i + 1; j < segments.Count(); j++)
            {
                segments[i].GetMultipleIntersectionPoints(segments[j]).ForEach(point => {
                    if(point.x % 1 == 0 && point.y % 1 == 0) {
                        crossingPoints.Add(point);
                    }
                });
            }
        } 

        return crossingPoints.Count();
    }

    public string Execute() {
        var input = new FileReader(05).Read().Select(line => line.Replace(" -> ", ","));
        var lineSegments = input.Select(line => line.Split(",")).Select(p => {
            var p1 = new Point2D(p[0], p[1]);
            var p2 = new Point2D(p[2], p[3]);
            return new LineSegment(p1, p2);
        }).ToList();
        var horizontalAndVerticalLineSegments = lineSegments.Where(ls => ls.p1.x == ls.p2.x || ls.p1.y == ls.p2.y).ToList();

        var crossingPointsHorAndVert = CalculateNumberOfCrossingPoints(horizontalAndVerticalLineSegments);
        var crossingPointsTotal = CalculateNumberOfCrossingPoints(lineSegments);

        return $"Number of crossing points considering only vertical and horizontal lines is {crossingPointsHorAndVert}" + Environment.NewLine +
               $"Number of crossing points considering all lines is {crossingPointsTotal}";
    }
}