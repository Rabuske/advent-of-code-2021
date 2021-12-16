class Map <T> {
    public List<Node<T>> Nodes {get; init; }

    public Map() {
        Nodes = new List<Node<T>>();
    }
    
    public Map(IEnumerable<IEnumerable<Node <T>>> map, bool considerDiagonals=false) {
        Nodes = new List<Node<T>>();
        // Set the adjacent ones
        for (int i = 0; i < map.Count(); i++)
        {
            var mapAsArray = map.Select(line => line.ToArray()).ToArray();
            for (int j = 0; j < mapAsArray[i].Count(); j++)
            {
                var currentNode = mapAsArray[i][j];
                Nodes.Add(currentNode);
                if(i - 1 >= 0) currentNode.AdjacentNodes.Add(mapAsArray[i-1][j]);
                if(i + 1 < map.Count()) currentNode.AdjacentNodes.Add(mapAsArray[i+1][j]);
                if(j - 1 >= 0) currentNode.AdjacentNodes.Add(mapAsArray[i][j-1]);
                if(j + 1 < mapAsArray[i].Count()) currentNode.AdjacentNodes.Add(mapAsArray[i][j+1]);

                if(considerDiagonals) {
                    if(i - 1 >= 0 && j - 1 >= 0) currentNode.AdjacentNodes.Add(mapAsArray[i-1][j-1]);
                    if(i - 1 >= 0 && j + 1 < mapAsArray[i].Count()) currentNode.AdjacentNodes.Add(mapAsArray[i-1][j+1]);
                    if(i + 1 < map.Count() && j - 1 >= 0) currentNode.AdjacentNodes.Add(mapAsArray[i+1][j-1]);
                    if(i + 1 < map.Count() && j + 1 < mapAsArray[i].Count()) currentNode.AdjacentNodes.Add(mapAsArray[i+1][j+1]);
                }
            }
        }
    }
}