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
                if(i - 1 >= 0) currentNode.AdjacentNodes.Add(mapAsArray[i-1][j], -1);
                if(i + 1 < map.Count()) currentNode.AdjacentNodes.Add(mapAsArray[i+1][j], -1);
                if(j - 1 >= 0) currentNode.AdjacentNodes.Add(mapAsArray[i][j-1], -1);
                if(j + 1 < mapAsArray[i].Count()) currentNode.AdjacentNodes.Add(mapAsArray[i][j+1], -1);

                if(considerDiagonals) {
                    if(i - 1 >= 0 && j - 1 >= 0) currentNode.AdjacentNodes.Add(mapAsArray[i-1][j-1], -1);
                    if(i - 1 >= 0 && j + 1 < mapAsArray[i].Count()) currentNode.AdjacentNodes.Add(mapAsArray[i-1][j+1], -1);
                    if(i + 1 < map.Count() && j - 1 >= 0) currentNode.AdjacentNodes.Add(mapAsArray[i+1][j-1], -1);
                    if(i + 1 < map.Count() && j + 1 < mapAsArray[i].Count()) currentNode.AdjacentNodes.Add(mapAsArray[i+1][j+1], -1);
                }
            }
        }
    }

    public List<Node<T>> GetOptimalPath(Node<T> start, Node<T> end) {
        var alreadyVisitedNodes = new HashSet<Node<T>>();
        var paths = new PriorityQueue<(int costSum, Node<T>[] path),int>();

        paths.Enqueue((0, new Node<T>[]{start}), 0);
        
        do{     
            // There is no solution       
            if(alreadyVisitedNodes.Count() == Nodes.Count()) {
                return new List<Node<T>>();
            }
            var currentPath = paths.Dequeue();            
            var currentNode = currentPath.path.Last();
            if(currentNode == end) {
                return currentPath.path.ToList();
            }
            if(alreadyVisitedNodes.Contains(currentNode)){
                continue;
            }
            alreadyVisitedNodes.Add(currentNode);

            paths.EnqueueRange(
                currentNode.AdjacentNodes.Where(n => !alreadyVisitedNodes.Contains(n.Key))
                .Select(adjNode => {
                    var risk = currentPath.costSum + currentNode.GetTravelCostTo(adjNode.Key);
                    return ((risk, currentPath.path.Append(adjNode.Key).ToArray()), risk);
                })
            );
        } while(true);
    }    
}