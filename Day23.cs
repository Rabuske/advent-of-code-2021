
enum BurrowNodeType {
    ROOM, 
    HALLWAY
}

class BurrowNode {

    public BurrowNodeType Type {get; init;}
    public List<(BurrowNode node, int cost)> Adjacent {get; init;} 
    
    public BurrowNode(BurrowNodeType type) {
        Type = type;
        OccupyingAmphipod = occupyingAmphipod;
        Adjacent = adjacent.Select(a => (a.node.Clone(), a.cost)).ToList();
    }

    public BurrowNode(BurrowNodeType type, char occupyingAmphipod) : this(type, occupyingAmphipod, new()) {}

    public BurrowNode Clone(){
        return new BurrowNode(Type, OccupyingAmphipod, Adjacent);
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

    public void AddAdjacent(BurrowNode adj, int cost) {
        Adjacent.Add((adj, cost));
        adj.Adjacent.Add((this, cost));
    }

}

class Day23 : IDayCommand {

    public List<BurrowNode> BuildNodes() {


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

        // Build the necessary graph
        List<BurrowNode> nodes = new() {
            new BurrowNode(BurrowNodeType.ROOM, 'A'),
            new BurrowNode(BurrowNodeType.ROOM, 'B'),
            new BurrowNode(BurrowNodeType.ROOM, 'D'),
            new BurrowNode(BurrowNodeType.ROOM, 'C'),
            new BurrowNode(BurrowNodeType.ROOM, 'C'),
            new BurrowNode(BurrowNodeType.ROOM, 'B'),
            new BurrowNode(BurrowNodeType.ROOM, 'A'),
            new BurrowNode(BurrowNodeType.ROOM, 'D'),
        };

        nodes.AddRange(Enumerable.Repeat(new BurrowNode(BurrowNodeType.HALLWAY, '.'), 7));

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

    }

}