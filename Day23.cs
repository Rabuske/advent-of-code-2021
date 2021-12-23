
enum BurrowNodeType {
    ROOM, 
    HALLWAY
}

record Amphipod(char id, List<BurrowNode> dest, BurrowNode current, int uniqueId) {
    public bool IsOk => dest.Contains(current);
    public override string ToString()
    {
        return $"[{uniqueId} - {id}: Location: {current.Id} - {current.Type}]";
    }
}

class BurrowNode {

    public int Id {get; init;}

    public BurrowNodeType Type {get; init;}

    public List<(BurrowNode node, int steps)> AdjacentBurrows {get; init;}     
    
    public BurrowNode(BurrowNodeType type, int id) {
        Type = type;
        AdjacentBurrows = new();
        Id = id;
    }

    public void AddAdjacent(BurrowNode adj, int steps) {
        AdjacentBurrows.Add((adj, steps));
        adj.AdjacentBurrows.Add((this, steps));
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

    public void GetAvailableDestinations(int steps, List<(BurrowNode, int)> currentPaths, List<Amphipod> amphipods) {
        if(currentPaths.Any(p => p.Item1 == this) || amphipods.Any(a => a.current == this)) {
            return;
        }

        currentPaths.Add((this, steps));

        foreach (var adjacent in AdjacentBurrows)
        {
            adjacent.node.GetAvailableDestinations(steps + adjacent.steps, currentPaths, amphipods);
        }
    }

    public override string ToString()
    {
        return $"{Id} - [{string.Join(",", AdjacentBurrows.Select(a => a.node.Id))}]";
    }
}

class Day23 : IDayCommand {

    public List<BurrowNode> BuildNodes(bool part02 = false) {
        List<BurrowNode> nodes = Enumerable.Range(0,8).Select(n => new BurrowNode(BurrowNodeType.ROOM, n)).ToList();
        nodes.AddRange(Enumerable.Range(8, 7).Select(n => new BurrowNode(BurrowNodeType.HALLWAY, n)));
        if(part02) {
            nodes.AddRange(Enumerable.Range(-22, 8).Reverse().Select(n => new BurrowNode(BurrowNodeType.ROOM, n)));
        }

       /* 
          Mapping of the indexes (hex)
          #############
          #89.A.B.C.DE#
          ### 1# 3# 5# 7###
            # 0# 2# 4# 6#
        (-) #21#19#17#15#
        (-) #22#20#18#16#
            #########  
        */        

        if(part02) 
        {
            nodes[22].AddAdjacent(nodes[21],1);
            nodes[21].AddAdjacent(nodes[0],1);
            nodes[20].AddAdjacent(nodes[19],1);
            nodes[19].AddAdjacent(nodes[2],1);
            nodes[18].AddAdjacent(nodes[17],1);
            nodes[17].AddAdjacent(nodes[4],1);
            nodes[16].AddAdjacent(nodes[15],1);
            nodes[15].AddAdjacent(nodes[6],1);
        }

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
        return string.Join(",", amphipods.OrderBy(a => a.id).Select(a => $"{a.id}:{a.current.Id}"));
    }

    public int FindLowestCost(List<BurrowNode> nodes, List<Amphipod> initialAmphipods) {
        
        var queue = new PriorityQueue<(List<Amphipod> amphipods, int cost), int>();
        HashSet<string> evaluated = new ();
        queue.Enqueue((initialAmphipods.Select(c => c).ToList(), 0), 0);

        while(true) 
        {
            var setup = queue.Dequeue();
            if(evaluated.Contains(GetConfigurationString(setup.amphipods))) continue;
            evaluated.Add(GetConfigurationString(setup.amphipods));
            if(setup.amphipods.All(c => c.IsOk)) {
                PrintMapPart2(setup.amphipods);
                return setup.cost;
            }

            var positions = setup.amphipods.ToDictionary(a => a.current.Id, a => a);
            
            foreach (var currAmphipod in setup.amphipods)
            {
                var candidates = GetCandidates(currAmphipod, positions, nodes);
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

    private void PrintMapPart2(List<Amphipod> amphipods) {
          var s = String.Empty;
          s += "#############"+ Environment.NewLine;
          s += "#89.G.H.I.JK#"+ Environment.NewLine;
          s += "###1#3#5#7###"+ Environment.NewLine;
          s += "  #0#2#4#6#  "+ Environment.NewLine;
          s += "  #R#P#N#L#  "+ Environment.NewLine;
          s += "  #S#Q#O#M#  "+ Environment.NewLine;
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
          s = s.Replace('L', locations.GetValueOrDefault(-15, ' '));
          s = s.Replace('M', locations.GetValueOrDefault(-16, ' '));
          s = s.Replace('N', locations.GetValueOrDefault(-17, ' '));
          s = s.Replace('O', locations.GetValueOrDefault(-18, ' '));
          s = s.Replace('P', locations.GetValueOrDefault(-19, ' '));
          s = s.Replace('Q', locations.GetValueOrDefault(-20, ' '));
          s = s.Replace('R', locations.GetValueOrDefault(-21, ' '));
          s = s.Replace('S', locations.GetValueOrDefault(-22, ' '));

        Console.WriteLine(s);
    }


    public Amphipod? GetAmphipodInNode(BurrowNode node, List<Amphipod> amphipods) {
        return amphipods.FirstOrDefault(a => a?.current == node, null);
    }

    private List<(BurrowNode node, int steps)> GetCandidates(Amphipod amphipod, Dictionary<int, Amphipod> positionAndAmphipods, List<BurrowNode> nodes)
    {   
        var finalDestinations = new List<(BurrowNode node, int steps)>();        
        var occupied = positionAndAmphipods.Select(a => a.Value).ToList();
        amphipod.current.AdjacentBurrows.ForEach(burrow => burrow.node.GetAvailableDestinations(burrow.steps, finalDestinations,occupied));
        
        if(amphipod.current.Type == BurrowNodeType.ROOM) {
            // If in the expected destination, only moves if blocking someone else 
            if(amphipod.IsOk) {
                // Determines if there is an node "behind" that is not equal to the expected one
                var destIndex = amphipod.dest.FindIndex(a => a == amphipod.current);
                var isBlocking = false;
                for (int i = destIndex - 1; i >= 0; i--)
                {            
                    var nodeBehind = amphipod.dest[i];
                    if(positionAndAmphipods.ContainsKey(nodeBehind.Id)) {
                        isBlocking = isBlocking || positionAndAmphipods[nodeBehind.Id].id != amphipod.id;
                    }
                }
                
                if(!isBlocking) {
                    return new();              
                }
            } 
        } 

        var hallways = finalDestinations.Where(dest => dest.node.Type == BurrowNodeType.HALLWAY).ToList();
        if(amphipod.current.Type == BurrowNodeType.HALLWAY) {
            hallways.Clear();
        }

        // Do not move to rooms that are not destination and are all empty or containing others of the same type
        if(!amphipod.dest.All(d => !positionAndAmphipods.ContainsKey(d.Id) || positionAndAmphipods[d.Id].id == amphipod.id)) {                
            return hallways;
        }
        finalDestinations = finalDestinations.Where(dest => amphipod.dest.Contains(dest.node)).OrderBy(dest => dest.node.Id).ToList();
        if(finalDestinations.Count() > 0){
            return hallways.Append(finalDestinations.First()).ToList();
        }
        return hallways;
    }

    public string Execute() {
        /* 
        My puzzle input (I will not parse it):
#############
#...........#
###B#C#B#D###
  #D#C#B#A#
  #D#B#A#C#
  #A#D#C#A#
  #########
         
          ### 1# 3# 5# 7###
            # 0# 2# 4# 6#
        (-) #21#19#17#15#
        (-) #22#20#18#16#
            #########  
        */
        var nodesPart02 = BuildNodes(true);          
        var destA = new List<BurrowNode>(){nodesPart02[22], nodesPart02[21], nodesPart02[0], nodesPart02[1]};
        var destB = new List<BurrowNode>(){nodesPart02[20], nodesPart02[19], nodesPart02[2], nodesPart02[3]};
        var destC = new List<BurrowNode>(){nodesPart02[18], nodesPart02[17], nodesPart02[4], nodesPart02[5]};
        var destD = new List<BurrowNode>(){nodesPart02[16], nodesPart02[15], nodesPart02[6], nodesPart02[7]};


        List<Amphipod> amphipodsPart02 = new () {
            new ('A', destA, nodesPart02[ 1], 0),
            new ('A', destA, nodesPart02[ 6], 1),
            new ('A', destA, nodesPart02[17], 2),
            new ('A', destA, nodesPart02[20], 3),
            new ('B', destB, nodesPart02[ 5], 4),
            new ('B', destB, nodesPart02[ 4], 5),
            new ('B', destB, nodesPart02[19], 6),
            new ('B', destB, nodesPart02[22], 7),
            new ('C', destC, nodesPart02[ 2], 8),
            new ('C', destC, nodesPart02[ 3], 9),
            new ('C', destC, nodesPart02[16], 10),
            new ('C', destC, nodesPart02[15], 11),
            new ('D', destD, nodesPart02[ 7], 12),
            new ('D', destD, nodesPart02[ 0], 13),
            new ('D', destD, nodesPart02[21], 14),
            new ('D', destD, nodesPart02[18], 15),
        };

        PrintMapPart2(amphipodsPart02);

        var nodesPart01 = BuildNodes();             
        List<Amphipod> amphipodsPart01 = new () {
            new ('A', new (){nodesPart01[0], nodesPart01[1]}, nodesPart01[1], 0),
            new ('A', new (){nodesPart01[0], nodesPart01[1]}, nodesPart01[2], 1),
            new ('B', new (){nodesPart01[2], nodesPart01[3]}, nodesPart01[0], 2),
            new ('B', new (){nodesPart01[2], nodesPart01[3]}, nodesPart01[5], 3),
            new ('C', new (){nodesPart01[4], nodesPart01[5]}, nodesPart01[3], 4),
            new ('C', new (){nodesPart01[4], nodesPart01[5]}, nodesPart01[6], 5),
            new ('D', new (){nodesPart01[6], nodesPart01[7]}, nodesPart01[4], 6),
            new ('D', new (){nodesPart01[6], nodesPart01[7]}, nodesPart01[7], 7)
        };        

        PrintMap(amphipodsPart01);

        return $"The Lowest steps for part 01 is {FindLowestCost(nodesPart01, amphipodsPart01)} and for part 02 is {FindLowestCost(nodesPart02, amphipodsPart02)}";

    }

}