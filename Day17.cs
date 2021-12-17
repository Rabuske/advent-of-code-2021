
record Area(int start, int end);
class Day17: IDayCommand {

    public bool HitsTarget(Area x, Area y, int initialVelocityX, int initialVelocityY, out List<Point2D> positions) {
        int velX = initialVelocityX;
        int velY = initialVelocityY;
        int posX = 0;
        int posY = 0;
        positions = new List<Point2D>();
        while(posY > y.start) {
            posX += velX;
            posY += velY;
            if(velX != 0) {
                velX += velX > 0? -1 : 1;
            }
            velY -= 1;

            positions.Add(new Point2D(posX, posY));

            if((x.start <= posX && posX <= x.end) && (y.start <= posY && posY <= y.end)){
                return true;
            }
        }
        return false;
    }

    public string Execute() {
        FileReader fileReader = new FileReader(17);
        var ranges = fileReader.Read().First()
            .Replace("target area: x=", "")
            .Replace(" y=", "")
            .Split(",")
            .Select(l => {
                var positions = l.Split("..");
                return new Area(start: int.Parse(positions[0]), end: int.Parse(positions[1]));
            }).ToArray();

        var velocities = new List<(int x, int y, List<Point2D> trajectory)>();


        // I'm using brute force and I'm <not> ashamed of it 
        for (int x = -500; x < 500; x++)
        {
            for (int y = -500; y < 500; y++)
            {
                if(HitsTarget(ranges[0], ranges[1], x, y, out var trajectory)){
                    velocities.Add((x, y, trajectory));
                }
            }
        }

        var maxY = velocities.Max(v => v.trajectory.Max(t => t.y));

        return $"The maximum height you can reach is {maxY} and the total velocities that hit the target is {velocities.Count()}";
    }
}