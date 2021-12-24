class AmphipodMap : IEqualityComparer<AmphipodMap> {
    public List<char> BurrowForA {get; set;}
    public List<char> BurrowForB {get; set;}
    public List<char> BurrowForC {get; set;}
    public List<char> BurrowForD {get; set;}
    public List<char> UpperChamber {get; set;}

    public bool IsFinished => 
        BurrowForA.All(b => b == 'A') &&
        BurrowForB.All(b => b == 'B') &&
        BurrowForC.All(b => b == 'C') &&
        BurrowForD.All(b => b == 'D');

    public AmphipodMap(int burrowSize = 2) {
        BurrowForA = Enumerable.Repeat(' ', burrowSize).ToList();
        BurrowForB = Enumerable.Repeat(' ', burrowSize).ToList();
        BurrowForC = Enumerable.Repeat(' ', burrowSize).ToList();
        BurrowForD = Enumerable.Repeat(' ', burrowSize).ToList();
        UpperChamber = Enumerable.Repeat(' ', 7).ToList();
    }

    public override string ToString() {
        string output = "#############" + Environment.NewLine + 
                        "#EF.G.H.I.JK#" + Environment.NewLine + 
                        "###O#S#X#1###" + Environment.NewLine + 
                        "  #N#R#W#0#  " + Environment.NewLine + 
                        "  #M#Q#U#Z#  " + Environment.NewLine + 
                        "  #L#P#T#Y#  " + Environment.NewLine + 
                        "  #########  " + Environment.NewLine;

        output = output.Replace('E', UpperChamber[0]);      
        output = output.Replace('F', UpperChamber[1]);      
        output = output.Replace('G', UpperChamber[2]);      
        output = output.Replace('H', UpperChamber[3]);      
        output = output.Replace('I', UpperChamber[4]);      
        output = output.Replace('J', UpperChamber[5]);      
        output = output.Replace('K', UpperChamber[6]);      

        output = output.Replace('L', BurrowForA[0]);      
        output = output.Replace('M', BurrowForA[1]);      
        output = output.Replace('N', BurrowForA.ElementAtOrDefault(2));      
        output = output.Replace('O', BurrowForA.ElementAtOrDefault(3));      

        output = output.Replace('P', BurrowForB[0]);      
        output = output.Replace('Q', BurrowForB[1]);      
        output = output.Replace('R', BurrowForB.ElementAtOrDefault(2));      
        output = output.Replace('S', BurrowForB.ElementAtOrDefault(3));      

        output = output.Replace('T', BurrowForC[0]);      
        output = output.Replace('U', BurrowForC[1]);      
        output = output.Replace('W', BurrowForC.ElementAtOrDefault(2));      
        output = output.Replace('X', BurrowForC.ElementAtOrDefault(3));      

        output = output.Replace('Y', BurrowForD[0]);      
        output = output.Replace('Z', BurrowForD[1]);      
        output = output.Replace('0', BurrowForD.ElementAtOrDefault(2));      
        output = output.Replace('1', BurrowForD.ElementAtOrDefault(3));

        return output;      
    }

    public bool Equals(AmphipodMap? a1, AmphipodMap? a2) {
        if(a1 is null) return a2 is null;
        if(a2 is null) return a1 is null;
        return a1.GetHashCode() == a2.GetHashCode();
    }

    public int GetHashCode(AmphipodMap a) {
        string total = string.Empty;
        total += string.Join(",", BurrowForA);
        total += string.Join(",", BurrowForB);
        total += string.Join(",", BurrowForC);
        total += string.Join(",", BurrowForD);
        total += string.Join(",", UpperChamber);
        return total.GetHashCode();
    }

    public override bool Equals(Object? obj) {
        if(obj is null) return false;
        return ((AmphipodMap)obj).GetHashCode() == GetHashCode();
    }

    public override int GetHashCode() {
        return GetHashCode(this);
    }    

    private bool NeedsToMoveFromBurrowToUpperChamber(char expected) {
        return !GetBurrow(expected).All(e => e == expected || e == ' ');
    }

    private bool CanMoveFromUpperChamberToBurrow(char expected) {
        // This method does not check if the path is free
        return GetBurrow(expected).All(e => e == expected || e == ' ');
    }    

    public List<(AmphipodMap map, int cost)> GetAllPossibleMoves(int currentCost) {
        var result = new List<(AmphipodMap map, int cost)>();
        
        if(NeedsToMoveFromBurrowToUpperChamber('A'))
        {
            result = result.Concat(MoveBurrowToChamber(GetLeftUpperChamberIndexRelativeTo('A'), currentCost, 'A')).ToList();
        }

        if(NeedsToMoveFromBurrowToUpperChamber('B'))
        {
            result = result.Concat(MoveBurrowToChamber(GetLeftUpperChamberIndexRelativeTo('B'), currentCost, 'B')).ToList();
        }

        if(NeedsToMoveFromBurrowToUpperChamber('C'))
        {
            result = result.Concat(MoveBurrowToChamber(GetLeftUpperChamberIndexRelativeTo('C'), currentCost, 'C')).ToList();
        }

        if(NeedsToMoveFromBurrowToUpperChamber('D'))
        {
            result = result.Concat(MoveBurrowToChamber(GetLeftUpperChamberIndexRelativeTo('D'), currentCost, 'D')).ToList();
        }

        // Try to move from chamber to burrow
        for (int i = 0; i < UpperChamber.Count(); i++)
        {
            char processingChar = UpperChamber[i];
            if(processingChar == ' ') continue;
            if(!CanMoveFromUpperChamberToBurrow(processingChar)) continue;

            // Left to right
            if(i <= GetLeftUpperChamberIndexRelativeTo(processingChar)) {
                var currentPosition = i;
                var steps = i == 0? 1 : 2;
                var clearPath = true;
                while(currentPosition < GetLeftUpperChamberIndexRelativeTo(processingChar)) {
                    currentPosition += 1;
                    steps += 2;
                    if(UpperChamber[currentPosition] != ' ') clearPath = false;
                }
                if(clearPath) {
                    steps += GetBurrow(processingChar).Count(e => e == ' ') - 1; // We already start with 2 steps
                    var newMap = Clone();
                    var indexAtBurrow = newMap.GetBurrow(processingChar).IndexOf(' ');
                    newMap.GetBurrow(processingChar)[indexAtBurrow] = processingChar;
                    newMap.UpperChamber[i] = ' ';
                    result.Add((newMap, currentCost + steps * GetEnergy(processingChar)));
                }
            }
            // Right to left  
            else {
                var currentPosition = i;
                var steps = i == UpperChamber.Count() - 1? 1 : 2;
                var clearPath = true;
                while(currentPosition > GetLeftUpperChamberIndexRelativeTo(processingChar) + 1) {
                    currentPosition -= 1;
                    steps += 2;
                    if(UpperChamber[currentPosition] != ' ') clearPath = false;
                }
                if(clearPath) {
                    steps += GetBurrow(processingChar).Count(e => e == ' ') - 1; // We already start with 2 steps
                    var newMap = Clone();
                    var indexAtBurrow = newMap.GetBurrow(processingChar).IndexOf(' ');
                    newMap.GetBurrow(processingChar)[indexAtBurrow] = processingChar;
                    newMap.UpperChamber[i] = ' ';
                    result.Add((newMap, currentCost + steps * GetEnergy(processingChar)));
                }                
            }
        }
        return result;
    }

    private List<(AmphipodMap map, int cost)> MoveBurrowToChamber(int leftIndex, int currentCost, char expected)
    {
        var result = new List<(AmphipodMap map, int cost)>();
        var workingBurrow = GetBurrow(expected);
        var indexOfElement = workingBurrow.IndexOf(' ');

        if(indexOfElement == 0) return new();
        indexOfElement = indexOfElement > 0 ? indexOfElement : workingBurrow.Count();
        indexOfElement -= 1;
        char element = workingBurrow[indexOfElement];

        // Move Left
        var steps = 2; // Moving from burrow to chamber costs alway 2 at the beginning
        steps += workingBurrow.Count(e => e == ' '); // Adds the movement within the burrow
        for (int i = leftIndex; i >= 0; i--)
        {
            if (UpperChamber[i] != ' ') break;
            var newMap = this.Clone();
            newMap.GetBurrow(expected)[indexOfElement] = ' ';
            newMap.UpperChamber[i] = element;
            result.Add((newMap, currentCost + steps * GetEnergy(element)));
            if (i == 1) steps += 1; else steps += 2;
        }

        // Move right
        steps = 2; // Moving from burrow to chamber costs alway 2 at the beginning
        steps += workingBurrow.Count(e => e == ' '); // Adds the movement within the burrow
        for (int i = leftIndex + 1; i < UpperChamber.Count(); i++)
        {
            if (UpperChamber[i] != ' ') break;
            var newMap = this.Clone();
            newMap.GetBurrow(expected)[indexOfElement] = ' ';
            newMap.UpperChamber[i] = element;
            result.Add((newMap, currentCost + steps * GetEnergy(element)));
            if (i == UpperChamber.Count() - 2) steps += 1; else steps += 2;
        }
        return result;
    }

    public List<char> GetBurrow(char element) {
        return element switch {
            'A' => BurrowForA,
            'B' => BurrowForB,
            'C' => BurrowForC,
            'D' => BurrowForD,
             _  => new List<char>()
        };
    } 

    public int GetEnergy(char element) {
        return element switch {
            'A' => 1,
            'B' => 10,
            'C' => 100,
            'D' => 1000,
             _  => 0
        };
    }

    public int GetLeftUpperChamberIndexRelativeTo(char element) {
        return element switch {
            'A' => 1,
            'B' => 2,
            'C' => 3,
            'D' => 4,
             _  => 0
        };
    } 

    public AmphipodMap Clone() {
        var newMap = new AmphipodMap();
        newMap.BurrowForA = BurrowForA.Select(c => c).ToList();
        newMap.BurrowForB = BurrowForB.Select(c => c).ToList();
        newMap.BurrowForC = BurrowForC.Select(c => c).ToList();
        newMap.BurrowForD = BurrowForD.Select(c => c).ToList();
        newMap.UpperChamber = UpperChamber.Select(c => c).ToList();
        return newMap;
    }
}
class Day23 : IDayCommand {

    public int GetSmallestCost(AmphipodMap initialMap) {
        var queue = new PriorityQueue<(AmphipodMap map, int cost), int>();
        var processed = new HashSet<AmphipodMap>();

        queue.Enqueue((initialMap, 0), 0);

        while(true) {
            var (currentMap, currentCost) = queue.Dequeue();
            if(currentMap.IsFinished) {
                return currentCost;
            }

            if(processed.Contains(currentMap)) {
                continue;
            }

            processed.Add(currentMap);
            //Console.WriteLine(currentMap.ToString());
            //Console.WriteLine(currentCost.ToString());

            var possibleMoves = currentMap.GetAllPossibleMoves(currentCost); 
            foreach (var move in possibleMoves)
            {
                if(!processed.Contains(move.map)) {
                    queue.Enqueue(move, move.cost);
                }
            }

        }
    }

    public AmphipodMap Part01Map(List<string> input) {
        var map = new AmphipodMap();

        map.BurrowForA[0] = input[3][3];
        map.BurrowForA[1] = input[2][3];

        map.BurrowForB[0] = input[3][5];
        map.BurrowForB[1] = input[2][5];

        map.BurrowForC[0] = input[3][7];
        map.BurrowForC[1] = input[2][7];

        map.BurrowForD[0] = input[3][9];
        map.BurrowForD[1] = input[2][9];
        return map;
    }


    public AmphipodMap Part02Map(List<string> input) {
        var map = Part01Map(input);

        map.BurrowForA.Insert(1, 'D');
        map.BurrowForA.Insert(2, 'D');

        map.BurrowForB.Insert(1, 'B');
        map.BurrowForB.Insert(2, 'C');

        map.BurrowForC.Insert(1, 'A');
        map.BurrowForC.Insert(2, 'B');

        map.BurrowForD.Insert(1, 'C');
        map.BurrowForD.Insert(2, 'A');

        return map;
    }    

    public string Execute() {
        
        var input = new FileReader(23).Read().ToList();
        var map01 = Part01Map(input);
        var map02 = Part02Map(input);

        return $"Smallest energy part 01 {GetSmallestCost(map01)} and part 02 {GetSmallestCost(map02)}";    
    }
}