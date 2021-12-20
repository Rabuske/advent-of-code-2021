record Point2D {
    public decimal x {get; init;}
    public decimal y {get; init;}

    public Point2D(decimal x, decimal y) {
        this.x = x;
        this.y = y;
    }

    public Point2D(string x, string y) {
        this.x = decimal.Parse(x);
        this.y = decimal.Parse(y);
    }    

    public static Point2D operator -(Point2D p1, Point2D p2) => new Point2D(p1.x - p2.x, p1.y - p2.y);
    public static Point2D operator +(Point2D p1, Point2D p2) => new Point2D(p1.x + p2.x, p1.y + p2.y);
    public static Point2D operator *(Point2D p1, Point2D p2) => new Point2D(p1.x * p2.x, p1.y * p2.y);
    public static Point2D operator *(Point2D p1, decimal m) => new Point2D(p1.x * m, p1.y * m);

    public List<Point2D> GenerateAdjacent(bool includeDiagonal = false, bool includePoint = false) {
        var result = new List<Point2D>();
        if(includeDiagonal) result.Add(new (x - 1, y - 1));
        result.Add(new (x - 1, y));
        if(includeDiagonal) result.Add(new (x - 1, y + 1));
        
        result.Add(new (x, y - 1));
        if(includePoint) result.Add(this);
        result.Add(new (x, y + 1));

        if(includeDiagonal) result.Add(new (x + 1, y - 1));
        result.Add(new (x + 1, y));
        if(includeDiagonal) result.Add(new (x + 1, y + 1));

        return result;
    }

    public decimal ManhattanDistance(Point2D p) => Math.Abs(this.x - p.x) + Math.Abs(this.y - p.y);
    public decimal CrossProduct(Point2D p) => this.x * p.y - this.y * p.x;
}