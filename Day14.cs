// PS: you can find the naive implementation in the commit history ;)

class Day14 : IDayCommand{

    private Dictionary<string, long> Polymerize(Dictionary<string, long> pairsOfPolymers, Dictionary<string, char> rules) {
        var result = rules.Keys.ToDictionary(key => key, key => (long) 0);
        foreach (var key in pairsOfPolymers.Keys)
        {            
            var pair1 = "" + key[0] + rules[key];
            var pair2 = "" + rules[key] + key[1];
            result[pair1] += pairsOfPolymers[key];
            result[pair2] += pairsOfPolymers[key];
        }
        return result;
    }

    private long GetDifferenceBetweenMostCommonAndLeastCommonElement(Dictionary<string, long> pairsOfPolymers) {
        var elementCount = pairsOfPolymers.SelectMany(pair => new List<(char, long)>() {
            {(pair.Key[0], pair.Value)},
            {(pair.Key[1], pair.Value)}
        }).GroupBy(pair => pair.Item1, pair => pair.Item2)
          .ToDictionary(keyValue => keyValue.Key, keyValue => keyValue.Sum());
        var mostCommon = elementCount.Max(e => e.Value);
        var leastCommon = elementCount.Min(e => e.Value);
        return 1 + (mostCommon - leastCommon) / 2;
    }

    public string Execute() {
        var input = new FileReader(14).Read().ToList();                 
        var rules = input.Skip(2)
            .Select(line => line.Split(" -> "))
            .GroupBy(pair => pair[0], pair => pair[1][0])
            .ToDictionary(g => g.Key, g => g.First());

        var pairsOfPolymers = rules.Keys.ToDictionary(key => key, key => (long) 0);

        // Populate the template
        for (int i = 0; i < input[0].Length - 1; i++)
        {
            var pair = input[0][i..(i+2)];
            pairsOfPolymers[pair] += 1;
        }

        for (int i = 0; i < 10; i++)
        {
            pairsOfPolymers = Polymerize(pairsOfPolymers, rules);
        }
    
        var differenceAfter10Steps = GetDifferenceBetweenMostCommonAndLeastCommonElement(pairsOfPolymers);

        for (int i = 10; i < 40; i++)
        {
            pairsOfPolymers = Polymerize(pairsOfPolymers, rules);
        }

        var differenceAfter40Steps = GetDifferenceBetweenMostCommonAndLeastCommonElement(pairsOfPolymers);

        return $"After 10 steps, the difference between most common and least common elements is {differenceAfter10Steps}" + Environment.NewLine +
               $"After 40 steps, the difference between most common and least common elements is {differenceAfter40Steps}";
    }
}