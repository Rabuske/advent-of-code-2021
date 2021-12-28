class Day25 : IDayCommand {

    public int MoveUntilItStops(Dictionary<Point2D, char> inputCucumbers, Point2D limits) {
        var steps = 0;
        bool moved = true;
        var cucumbers = inputCucumbers.ToDictionary(a => a.Key, a => a.Value);

        while(moved) {
            moved = false;
            steps += 1;

            var types = new char[]{'>', 'v'};

            foreach (var type in types)
            {
                var resultingCucumbers = new Dictionary<Point2D, char>();
                foreach (var c in cucumbers)
                {
                    var newPosition = c.Key;
                    if(c.Value != type) 
                    {
                        resultingCucumbers.Add(c.Key, c.Value);
                        continue;
                    } 
                    if(c.Value == 'v') newPosition += new Point2D(1,0);
                    if(c.Value == '>') newPosition += new Point2D(0,1);

                    if(newPosition.x > limits.x) newPosition = new Point2D(0, newPosition.y);
                    if(newPosition.y > limits.y) newPosition = new Point2D(newPosition.x, 0);

                    if(cucumbers.ContainsKey(newPosition))
                    {
                        newPosition = c.Key;
                    } else
                    {
                        moved = true;
                    }
                    resultingCucumbers.Add(newPosition, c.Value);
                }
                cucumbers = resultingCucumbers;
            }
        }

        return steps;
    }

    public string Execute() {
        
        var cucumbers = new Dictionary<Point2D, char>();
        var seaMap = new FileReader(25).Read().ToArray();
        for (int i = 0; i < seaMap.Length; i++)
        {
            for (int j = 0; j < seaMap[i].Length; j++)
            {
                if(seaMap[i][j] == '.') 
                {
                    continue;
                }
                cucumbers.Add(new Point2D(i, j), seaMap[i][j]);
            }   
        }

        var stepWhenStoppedMoving = MoveUntilItStops(cucumbers, new Point2D(seaMap.Length - 1, seaMap[0].Length - 1));

        return $"They stop moving after {stepWhenStoppedMoving} steps";
    }

}