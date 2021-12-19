class LineSegment {
    public Point2D p1 {get; init;}
    public Point2D p2 {get; init;}

    public LineSegment(Point2D p1, Point2D p2) {
        this.p1 = p1;
        this.p2 = p2;
    }

    public (decimal a, decimal b, decimal c) Coefficients() {
        decimal a = p2.y - p1.y;
        decimal b = p1.x - p2.x;
        decimal c = a * p1.x + b * p1.y;
        return (a, b, c);
    }

    private bool IsPointOnSegmentRange(Point2D p) {
        bool isXOnLine = (p1.x <= p.x && p.x <= p2.x) || (p2.x <= p.x && p.x <= p1.x);
        bool isYOnLine = (p1.y <= p.y && p.y <= p2.y) || (p2.y <= p.y && p.y <= p1.y);
        return isXOnLine && isYOnLine;
    }

    public bool IsPointOnLine(Point2D p) {
        var dxc = p.x - p1.x;
        var dyc = p.y - p1.y;
        var dxl = p2.x - p1.x;
        var dyl = p2.y - p1.y;

        var cross = dxc * dyl - dyc * dxl;
        if(cross != 0) return false;

        return IsPointOnSegmentRange(p);
    }    

    public Point2D? GetIntersectionPoint(LineSegment line) {
        var coefficientsLine1 = Coefficients();
        var coefficientsLine2 = line.Coefficients();
        var det = (coefficientsLine1.a * coefficientsLine2.b) - (coefficientsLine2.a * coefficientsLine1.b);
        if(det == 0) {
            return null;
        }

        var iX = ((coefficientsLine2.b * coefficientsLine1.c) - (coefficientsLine1.b * coefficientsLine2.c)) / det;
        var iY = ((coefficientsLine1.a * coefficientsLine2.c) - (coefficientsLine2.a * coefficientsLine1.c)) / det;
        var resultingPoint = new Point2D(iX, iY);

        if(IsPointOnSegmentRange(resultingPoint) && line.IsPointOnSegmentRange(resultingPoint)) {
            return resultingPoint;
        }

        return null;
    }

    public bool IsParallel(LineSegment line) {
        var coefficientsLine1 = Coefficients();
        var coefficientsLine2 = line.Coefficients();
        var det = (coefficientsLine1.a * coefficientsLine2.b) - (coefficientsLine2.a * coefficientsLine1.b);
        return det == 0;        
    }

    public decimal? GetSlope() {
        var coefficients = Coefficients();
        if(coefficients.b == 0) return null; // Vertical Line
        return coefficients.a / coefficients.b;
    }

    public decimal? GetYForX(decimal x) {
        var slope = GetSlope();
        if(slope == null) return null; // Vertical Line
        
        return (slope * (x - p1.x)) - p1.y;
    }

    // Return the intersection point if not parallel, otherwise, return a list of intersection points for segments that intersect each other in multiple places
    public List<Point2D> GetMultipleIntersectionPoints(LineSegment line) {
        List<Point2D> resultingPoints = new List<Point2D>();
        if(IsParallel(line)) {
            decimal? yAtX0ForLine1 = GetYForX(0);
            decimal? yAtX0ForLine2 = line.GetYForX(0);
            if(
                ((yAtX0ForLine1 == null || yAtX0ForLine2 == null) && this.p1.x == line.p1.x) // Vertical lines that are under the same X
                || (yAtX0ForLine1 == yAtX0ForLine2 && yAtX0ForLine1 != null)) // Parallel lines that have the same slope and cross the origin at the same Y
            { 
                var (smallestX, largestX) = p1.x < p2.x? (p1.x, p2.x) : (p2.x, p1.x);
                var (smallestY, largestY) = p1.y < p2.y? (p1.y, p2.y) : (p2.y, p1.y);
                for (int i = (int) smallestX; i <= largestX; i++)
                {
                    for (int j = (int) smallestY; j <= largestY; j++)
                    {

                        var point = new Point2D(i,j);
                        if(this.IsPointOnLine(point) && line.IsPointOnLine(point)) {
                            resultingPoints.Add(point);
                        }
                    }
                }

            } 
        } else {
            var intersectionPoint = GetIntersectionPoint(line);
            if(!(intersectionPoint is null)) {
                resultingPoints.Add(intersectionPoint);
            }
        }

        return resultingPoints;
    }
}