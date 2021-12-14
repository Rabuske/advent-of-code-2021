class Day14 : IDayCommand{

    private void PolymerizeNaive(LinkedList<char> polymer, Dictionary<string, char> rules) {
        var firstChar = polymer.First;        
        while(firstChar?.Next is not null){
            var next = firstChar.Next;
            var key = "" + firstChar.Value + next.Value;
            var insert = rules[key];
            polymer.AddAfter(firstChar, insert);
            firstChar = next;
        }
    }

    public string Execute() {
        var input = new FileReader(14).Read().ToList();
        var polymer = new LinkedList<char>(input[0]);
        var rules = input.Skip(2)
            .Select(line => line.Split(" -> "))
            .GroupBy(pair => pair[0], pair => pair[1][0])
            .ToDictionary(g => g.Key, g => g.First());

        for (int i = 0; i < 10; i++)
        {
            PolymerizeNaive(polymer, rules);
        }
    
        var elementCount = polymer.GroupBy(p => p).ToDictionary(g => g.Key, g => g.Count());
        var mostCommon = elementCount.Max(e => e.Value);
        var leastCommon = elementCount.Min(e => e.Value);
        var differenceAfter10Steps = mostCommon - leastCommon;

        for (int i = 10; i < 40; i++)
        {
            Console.WriteLine($"Executing step {i}");
            PolymerizeNaive(polymer, rules);
        }

        elementCount = polymer.GroupBy(p => p).ToDictionary(g => g.Key, g => g.Count());
        mostCommon = elementCount.Max(e => e.Value);
        leastCommon = elementCount.Min(e => e.Value);
        var differenceAfter40Steps = mostCommon - leastCommon;

        return $"After 10 steps, the difference between most common and least common elements is {differenceAfter10Steps}" + Environment.NewLine +
               $"After 40 steps, the difference between most common and least common elements is {differenceAfter40Steps}";
    }
}