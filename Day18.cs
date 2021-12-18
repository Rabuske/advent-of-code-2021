class Snailfish {
    public Snailfish? Right {get; set; }
    public Snailfish? Left {get; set;} 
    public Snailfish? Parent {get; set;}
    public int Value {get; set;} = -1;

    public Snailfish(){}
    public Snailfish(Snailfish parent){
        Parent = parent;
    }

    public bool IsPair => Value == -1;

    override public string ToString() {
        if(!IsPair) return Value.ToString();
        return $"[{Left?.ToString()},{Right?.ToString()}]";
    } 

   public static Snailfish operator +(Snailfish s1, Snailfish s2) {
        var result = new Snailfish();
        result.Left = s1;
        result.Right = s2;
        s1.Parent = result;
        s2.Parent = result;
        while(result.Reduce());
        return result;
    }

    public bool ReduceExplode() {
        if(GetDepthIfPair() > 4) {
            Explode();
            return true;
        }        

        if(Left is null || Right is null) return false;

        return Left.ReduceExplode() || Right.ReduceExplode();
    }

    public bool ReduceSplit() {
        if(Value > 9) {
            Split();
            return true;
        }       

        if(Left is null || Right is null) return false;

        return Left.ReduceSplit() || Right.ReduceSplit();
    }

    public bool Reduce() {
        return ReduceExplode() || ReduceSplit();
    } 

    public Snailfish? GetNearLeftValue(HashSet<Snailfish> visited) {
        // When the current node is on the left, we navigate until it is on the right
        var currentNode = this;
        if(Parent?.Left is not null && Parent?.Left == currentNode) {
            while(currentNode?.Parent?.Left == currentNode) {
                currentNode = currentNode?.Parent;
            }
        }

        if(currentNode is null) return null;

        // When the current node is on the left, the next value is the next right value on the left node
        if(currentNode.Parent?.Right == currentNode) {
            currentNode = currentNode?.Parent?.Left ?? new Snailfish();
            while(currentNode.IsPair && currentNode.Right is not null) {
                currentNode = currentNode.Right;
            }
            return currentNode;
        }        

        return null;
    }

    public Snailfish? GetNearRightValue(HashSet<Snailfish> visited) {
        // When the current node is on the right, we navigate until it is on the left
        var currentNode = this;
        if(Parent?.Right is not null && Parent?.Right == currentNode) {
            while(currentNode?.Parent?.Right == currentNode) {
                currentNode = currentNode?.Parent;
            }
        }

        if(currentNode is null) return null;

        // When the current node is on the left, the next value is the next right value on the right node
        if(currentNode.Parent?.Left == currentNode) {
            currentNode = currentNode?.Parent?.Right ?? new Snailfish();
            while(currentNode.IsPair && currentNode.Left is not null) {
                currentNode = currentNode.Left;
            }
            return currentNode;
        }        

        return null;        
    }

    public void Explode() {
        var leftValue = GetNearLeftValue(new HashSet<Snailfish>(){this}) ?? new Snailfish();
        leftValue.Value += Left?.Value ?? -1;
        
        var rightValue = GetNearRightValue(new HashSet<Snailfish>(){this}) ?? new Snailfish();
        rightValue.Value += Right?.Value ?? -1;

        var newSnailfish = new Snailfish(Parent ?? new Snailfish()); 
        newSnailfish.Value = 0;
        if(Parent?.Left == this) Parent.Left = newSnailfish;
        if(Parent?.Right == this) Parent.Right = newSnailfish;        
    }

    public void Split() {
        var newPair = new Snailfish();
        newPair.Parent = Parent;
        newPair.Left = new Snailfish(newPair);
        newPair.Left.Value = (int) Math.Floor(Value / 2.0);
        newPair.Right = new Snailfish(newPair);
        newPair.Right.Value = (int) Math.Ceiling(Value / 2.0);
        if(Parent?.Left == this) Parent.Left = newPair;
        if(Parent?.Right == this) Parent.Right = newPair;
    }

    public int GetDepthIfPair() {
        if(!IsPair) {
            return 0;
        }

        var depth = 1;
        var currentSnailfish = this;
        while(currentSnailfish.Parent != null) {
            depth += 1;
            currentSnailfish = currentSnailfish.Parent;
        }

        return depth;
    }

    public int Magnitude() {
        if(!IsPair) return Value;
        if(Left is null || Right is null) return 0;
        return 3 * Left.Magnitude() + 2 * Right.Magnitude();
    }
}

class Day18: IDayCommand {

    public Snailfish ParseSnailfish(string line) {
        var mainSnailfish = new Snailfish();
        Snailfish currentSnailfish = mainSnailfish;
        foreach (var c in line)
        {
            switch (c)
            {
                case '[':
                    currentSnailfish.Left = new Snailfish(currentSnailfish);
                    currentSnailfish = currentSnailfish.Left;
                    break;
                case ']':
                    // The null check is just to stop the compiler from complaining
                    currentSnailfish = currentSnailfish.Parent ?? new Snailfish(); 
                    break;
                case ',':
                    currentSnailfish = currentSnailfish.Parent ?? new Snailfish(); 
                    currentSnailfish.Right = new Snailfish(currentSnailfish);
                    currentSnailfish = currentSnailfish.Right;
                    break;
                default:
                    currentSnailfish.Value = (int) char.GetNumericValue(c);
                    break;
            }
        }
        return mainSnailfish;
    }

    public string Execute() {
        var snailFishStrings = new FileReader(18).Read().ToList();        
        var allSnailFish = snailFishStrings.Select(line => ParseSnailfish(line)).ToList();
        var allFishSummed = allSnailFish.Skip(1).Aggregate(allSnailFish.First(), (result, sf) => result + sf);

        var maxMagnitudeForPair = 0;

        for (int i = 0; i < snailFishStrings.Count() - 1; i++)
        {
            for (int j = i; j < snailFishStrings.Count(); j++)
            {
                var snailFish1 = ParseSnailfish(snailFishStrings[i]);
                var snailFish2 = ParseSnailfish(snailFishStrings[j]);
                var magnitude = (snailFish1 + snailFish2).Magnitude();
                maxMagnitudeForPair = Math.Max(magnitude, maxMagnitudeForPair);

                snailFish1 = ParseSnailfish(snailFishStrings[i]);
                snailFish2 = ParseSnailfish(snailFishStrings[j]);
                magnitude = (snailFish2 + snailFish1).Magnitude();
                maxMagnitudeForPair = Math.Max(magnitude, maxMagnitudeForPair);
            }
        }

        return $"The magnitude of all snailfish summed is {allFishSummed.Magnitude()}" + Environment.NewLine +
               $"The max magnitude of the sum of two pairs is {maxMagnitudeForPair}";
    }

}