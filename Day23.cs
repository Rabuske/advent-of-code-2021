
enum BurrowNodeType {
    ROOM, 
    HALLWAY
}

record Room(int id, char expected, char current, List<int> nextOnes);



record Amphipod(char id, List<BurrowNode> dest, BurrowNode current, int uniqueId) {
    public bool IsOk => dest.Contains(current);
}

class BurrowNode {

    public int Id {get; init;}

    public BurrowNodeType Type {get; init;}

    public List<(BurrowNode node, int steps)> Adjacent {get; init;}     
    
    public BurrowNode(BurrowNodeType type, int id) {
        Type = type;
        Adjacent = new();
        Id = id;
    }

    public void AddAdjacent(BurrowNode adj, int steps) {
        Adjacent.Add((adj, steps));
        adj.Adjacent.Add((this, steps));
    }

    public static int GetStepCost(char amphipod) {
        return amphipod switch {
            'A' => 1,
            'B' => 10,
            'C' => 100,
            'D' => 1000,
             _  => 0
        };        
    }
}

class Day23 : IDayCommand {

    public List<BurrowNode> BuildNodes() {
        List<BurrowNode> nodes = Enumerable.Range(0,8).Select(n => new BurrowNode(BurrowNodeType.ROOM, n)).ToList();
        nodes.AddRange(Enumerable.Range(8, 7).Select(n => new BurrowNode(BurrowNodeType.HALLWAY, n)));
        
       /* 
          Mapping of the indexes (hex)
          #############
          #89.A.B.C.DE#
          ###1#3#5#7###
            #0#2#4#6#
            #########  
        */        
        nodes[0].AddAdjacent(nodes[1],1);
        nodes[1].AddAdjacent(nodes[9],2);
        nodes[1].AddAdjacent(nodes[10],2);
        
        nodes[2].AddAdjacent(nodes[3],1);
        nodes[3].AddAdjacent(nodes[10],2);
        nodes[3].AddAdjacent(nodes[11],2);

        nodes[4].AddAdjacent(nodes[5],1);
        nodes[5].AddAdjacent(nodes[11],2);
        nodes[5].AddAdjacent(nodes[12],2);        

        nodes[6].AddAdjacent(nodes[7],1);
        nodes[7].AddAdjacent(nodes[12],2);
        nodes[7].AddAdjacent(nodes[13],2);       

        nodes[8].AddAdjacent(nodes[9],1);
        nodes[9].AddAdjacent(nodes[10],2);
        nodes[10].AddAdjacent(nodes[11],2);
        nodes[11].AddAdjacent(nodes[12],2);
        nodes[12].AddAdjacent(nodes[13],2);
        nodes[13].AddAdjacent(nodes[14],1);
        return nodes;   
    }

    public string GetConfigurationString(List<Amphipod> amphipods) {
        return string.Join(",", amphipods.OrderBy(a => a.id).Select(a => $"{a.uniqueId}:{a.current.Id}"));
    }

    public int FindLowestCost(List<BurrowNode> nodes, List<Amphipod> amphipods) {
        
        var queue = new PriorityQueue<(List<Amphipod> amphipods, int cost), int>();
        HashSet<string> evaluated = new ();
        queue.Enqueue((amphipods.Select(c => c).ToList(), 0), 0);

        while(true) 
        {
            var setup = queue.Dequeue();
            if(evaluated.Contains(GetConfigurationString(setup.amphipods))) continue;
            evaluated.Add(GetConfigurationString(setup.amphipods));
            if(setup.amphipods.All(c => c.IsOk)) {
                return setup.cost;
            }

            //PrintMap(setup.amphipods);

            foreach (var currAmphipod in setup.amphipods)
            {
                var candidates = GetCandidates(currAmphipod, setup.amphipods, nodes);
                foreach (var candidate in candidates)
                {
                    var cost = candidate.steps * BurrowNode.GetStepCost(currAmphipod.id);
                    var newAmphipodes = setup.amphipods.Where(a => a != currAmphipod).Append(new Amphipod(currAmphipod.id, currAmphipod.dest, candidate.node, currAmphipod.uniqueId)).ToList();                    
                    if(!evaluated.Contains(GetConfigurationString(newAmphipodes))){
                        queue.Enqueue((newAmphipodes, cost + setup.cost), cost + setup.cost);
                    }
                }
            }
        }

    }

    private void PrintMap(List<Amphipod> amphipods) {
          var s = String.Empty;
          s += "#############"+ Environment.NewLine;
          s += "#89.G.H.I.JK#"+ Environment.NewLine;
          s += "###1#3#5#7###"+ Environment.NewLine;
          s += "  #0#2#4#6#  "+ Environment.NewLine;
          s += "  #########  "+ Environment.NewLine;

          var locations = amphipods.ToDictionary(a => a.current.Id, a => a.id);

          s = s.Replace('0', locations.GetValueOrDefault(0, ' '));
          s = s.Replace('1', locations.GetValueOrDefault(1, ' '));
          s = s.Replace('2', locations.GetValueOrDefault(2, ' '));
          s = s.Replace('3', locations.GetValueOrDefault(3, ' '));
          s = s.Replace('4', locations.GetValueOrDefault(4, ' '));
          s = s.Replace('5', locations.GetValueOrDefault(5, ' '));
          s = s.Replace('6', locations.GetValueOrDefault(6, ' '));
          s = s.Replace('7', locations.GetValueOrDefault(7, ' '));
          s = s.Replace('8', locations.GetValueOrDefault(8, ' '));
          s = s.Replace('9', locations.GetValueOrDefault(9, ' '));
          s = s.Replace('G', locations.GetValueOrDefault(10, ' '));
          s = s.Replace('H', locations.GetValueOrDefault(11, ' '));
          s = s.Replace('I', locations.GetValueOrDefault(12, ' '));
          s = s.Replace('J', locations.GetValueOrDefault(13, ' '));
          s = s.Replace('K', locations.GetValueOrDefault(14, ' '));

        Console.WriteLine(s);
    }

    private List<(BurrowNode node, int steps)> GetCandidates(Amphipod amphipod, List<Amphipod> amphipods, List<BurrowNode> nodes)
    {
        // Assume everything is valid
        var result = amphipod.current.Adjacent.Select(a => a);

        // Cannot move if there is someone else there
        result = result.Where(adj => !amphipods.Any(a => a.current == adj.node));

        // Cannot move from HALLWAY to a room that is not its destiny
        if(amphipod.current.Type == BurrowNodeType.HALLWAY) 
        {
            result = result.Where(adj => amphipod.dest.Contains(adj.node) || adj.node.Type == BurrowNodeType.HALLWAY);
        } 

        if(amphipod.current.Type == BurrowNodeType.ROOM && amphipod.dest.Contains(amphipod.current))
        {
            // Determines if there is an node "behind" that is not equal to the expected one
            var destIndex = amphipod.dest.FindIndex(a => a == amphipod.current);
            var isBlocking = false;
            for (int i = destIndex - 1; i >= 0; i--)
            {            
                var nodeBehind = amphipod.dest[i];
                isBlocking = isBlocking || amphipods.Any(a => a.current == nodeBehind && a.id != amphipod.id);
            }
            
            if(isBlocking) {
                // Only allow to move forward
                result = result.Where(adj => adj.node.Id > amphipod.current.Id);
            } else {
                // Only allow to move backwards
                result = result.Where(adj => adj.node.Id < amphipod.current.Id);
            }
        }

        return result.ToList();
    }

    public string Execute() {
        /* 
        My puzzle input (I will not parse it):
          #############
          #...........#
          ###A#C#B#D###
            #B#A#D#C#
            #########

        Sample input:
          #############
          #...........#
          ###B#C#B#D###
            #A#D#C#A#
            #########        
        */
        var nodes = BuildNodes();     
        
        List<Amphipod> amphipods = new () {
            new ('A', new (){nodes[0], nodes[1]}, nodes[1], 0),
            new ('A', new (){nodes[0], nodes[1]}, nodes[2], 1),
            new ('B', new (){nodes[2], nodes[3]}, nodes[0], 2),
            new ('B', new (){nodes[2], nodes[3]}, nodes[5], 3),
            new ('C', new (){nodes[4], nodes[5]}, nodes[3], 4),
            new ('C', new (){nodes[4], nodes[5]}, nodes[6], 5),
            new ('D', new (){nodes[6], nodes[7]}, nodes[4], 6),
            new ('D', new (){nodes[6], nodes[7]}, nodes[7], 7)
        };

        return $"The Lowest steps is {FindLowestCost(nodes, amphipods)}";

    }

}