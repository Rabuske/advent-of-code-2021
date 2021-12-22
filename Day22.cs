record CustomRange(int Start, int End);
record CubeInstruction (CustomRange x, CustomRange y, CustomRange z, bool isOn);

record Cube(int minX, int maxX, int minY, int maxY, int minZ, int maxZ) {
    public bool IsValid => minX <= maxX && minY <= maxY && minZ <= maxZ;
};

class Day22 : IDayCommand {

    public bool IsWithinRange(CubeInstruction ranges, Point3D point){
        return ranges.x.Start <= point.x && point.x < ranges.x.End &&
               ranges.y.Start <= point.y && point.y < ranges.y.End &&
               ranges.z.Start <= point.z && point.z < ranges.z.End;
    }

    // Kept brute force approach here :)
    public int Part01(List<CubeInstruction> steps) {
        var cubesThatAreOn = new HashSet<Point3D>();

        foreach (var step in steps)
        {   
            for (int x = Math.Max(-50, step.x.Start); x <= Math.Min(50, step.x.End); x++)
            {
                for (int y = Math.Max(-50, step.y.Start); y <= Math.Min(50, step.y.End); y++)
                {
                    for (int z = Math.Max(-50, step.z.Start); z <= Math.Min(50, step.z.End); z++)
                    {
                        var cube = new Point3D(x,y,z);
                        if(step.isOn && !cubesThatAreOn.Contains(cube)) {
                            cubesThatAreOn.Add(cube);
                        }
                        if(!step.isOn && cubesThatAreOn.Contains(cube)) {
                            cubesThatAreOn.Remove(cube);
                        }
                    }                
                }                
            }
        }

        return cubesThatAreOn.Count();
    }

    public long Part02(List<CubeInstruction> steps) {
        var cubes = new Dictionary<Cube, long>();

        foreach(var step in steps)
        {
            long newSign = step.isOn ? 1 : -1;
            var newCube = new Cube(step.x.Start, step.x.End, step.y.Start, step.y.End, step.z.Start, step.z.End);
            var newCubes = new Dictionary<Cube, long>();

            // For each existing cube (from previous steps), we must check if the current cube intersects with it
            // Then, we add the corresponding intersections as separated cubes, so in the final tally, they cancel each other
            foreach(var cube in cubes)
            {
                // Determine the intersecting cube
                var intersectingCube = new Cube(
                    Math.Max(cube.Key.minX, newCube.minX),
                    Math.Min(cube.Key.maxX, newCube.maxX),
                    Math.Max(cube.Key.minY, newCube.minY),
                    Math.Min(cube.Key.maxY, newCube.maxY),
                    Math.Max(cube.Key.minZ, newCube.minZ),
                    Math.Min(cube.Key.maxZ, newCube.maxZ)
                );
                if (intersectingCube.IsValid) newCubes[intersectingCube] = newCubes.GetValueOrDefault(intersectingCube, 0) - cube.Value;
            }
            if (newSign == 1) newCubes[newCube] = newCubes.GetValueOrDefault(newCube, 0) + newSign;

            foreach(var cube in newCubes)
            {
                cubes[cube.Key] = cubes.GetValueOrDefault(cube.Key, 0) + cube.Value;
            }
        }
        return cubes.Sum(a => (a.Key.maxX - a.Key.minX + 1L) * (a.Key.maxY - a.Key.minY + 1) * (a.Key.maxZ - a.Key.minZ + 1) * a.Value);        
    }

    public string Execute() {
        var input = new FileReader(22).Read().Select(line => line.Replace("x=", "").Replace("y=", "").Replace("z=", ""));
        var ranges = input.Select(line => {
            var onOff = line.Split(" ");
            var coords = onOff[1].Split(",");
            var coordsRange = coords.Select(c => c.Split("..").Select(n => int.Parse(n)).ToArray()).ToArray();
            return new CubeInstruction(
                x: new CustomRange(coordsRange[0][0], coordsRange[0][1]), 
                y: new CustomRange(coordsRange[1][0], coordsRange[1][1]), 
                z: new CustomRange(coordsRange[2][0], coordsRange[2][1]), 
                isOn: onOff[0].Equals("on")
            );
        }).ToList();

        var part01 = Part01(ranges);
        var part02 = Part02(ranges);

        return $"Cubes that are on after initialization {part01} cubes that are on in total {part02}";
    }
}