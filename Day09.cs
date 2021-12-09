class Day09 : IDayCommand {

    public string Execute() {

        var map = new FileReader(09)
            .Read()
            .Select(line => line.Select(element => (int)Char.GetNumericValue(element))
            .ToList())
            .ToList();

        var lowestPoints = new List<int>();

        for (int i = 0; i < map.Count(); i++)
        {
            for (int j = 0; j < map[i].Count(); j++)
            {
                var locations = new List<int>();
                if(i - 1 >= 0) locations.Add(map[i-1][j]);
                if(i + 1 < map.Count()) locations.Add(map[i+1][j]);
                if(j - 1 >= 0) locations.Add(map[i][j-1]);
                if(j + 1 < map[i].Count()) locations.Add(map[i][j+1]);
                if(map[i][j] < locations.Min()) lowestPoints.Add(map[i][j]);
            }                
        }        

        var riskLevel = lowestPoints.Sum(point => point + 1);

        return $"The risk level is {riskLevel}";
    }

}