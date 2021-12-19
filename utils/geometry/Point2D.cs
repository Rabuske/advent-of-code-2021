class Point2D : IEqualityComparer<Point2D>{
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
    public static bool operator ==(Point2D? p1, Point2D? p2) {
        if (p1 is null) return p2 is null;
        if (p2 is null) return p1 is null;
        return p1.x == p2.x && p1.y == p2.y;
    } 
    public static bool operator !=(Point2D p1, Point2D p2) => !(p1 == p2);

    public decimal ManhattanDistance(Point2D p) => Math.Abs(this.x - p.x) + Math.Abs(this.y - p.y);
    public decimal CrossProduct(Point2D p) => this.x * p.y - this.y * p.x;

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        return (Point2D) obj == this;
    }
    
    public override int GetHashCode()
    {
        return $"X:{x}Y:{y}".GetHashCode();
    }

    public bool Equals(Point2D? p1, Point2D? p2)
    {
        if (p1 is null) return p2 is null;
        if (p2 is null) return p1 is null;
        return p1.Equals(p2);
    }

    public int GetHashCode(Point2D obj)
    {
        return obj.GetHashCode();
    }
}