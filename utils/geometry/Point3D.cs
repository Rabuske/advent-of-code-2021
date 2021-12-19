class Point3D {

    public decimal x {get; init;}
    public decimal y {get; init;}
    public decimal z {get; init;}

    public Point3D(decimal x, decimal y, decimal z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Point3D operator +(Point3D p1, Point3D p2) {
        return new Point3D(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
    }

    public static Point3D operator *(Point3D p1, decimal scalar) {
        return new Point3D(p1.x * scalar, p1.y * scalar, p1.z * scalar);
    }
    
    public decimal ManhattanDistance(Point3D p) {
        return Math.Abs(this.x - p.x) + Math.Abs(this.y - p.y) +  Math.Abs(this.z - p.z);
    }

    public override string ToString(){
        return $"({this.x},{this.y},{this.z})";
    }
}