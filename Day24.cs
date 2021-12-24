class Day24 : IDayCommand {

    public (string max, string min) GetSolution(List<(int p1, int p2, int p3)> values) {

        // Figured out that there is no programmatic way to do this challenge. Hence, I'm using the logic described here (https://github.com/mrphlip/aoc/blob/master/2021/24.md) and programming the values
        // (No, I did not reversed engineering the program as you're supposed to do, as I'm here to code)

        Stack<(int p3, char id)> stack = new();
        Dictionary<char, (int min, int max)> range = new();
        char id = 'A';

        foreach (var value in values)
        {
            if(value.p1 == 1) {
                stack.Push((value.p3,id));
            }

            if(value.p1 == 26) {
                var pair1 = stack.Pop();
                var pair1Values = new List<int>();
                var pair2Values = new List<int>();
                for(int i = 1; i <= 9; i++) {
                    var pair2 = i + (pair1.p3 + value.p2);
                    if(pair2 > 0 && pair2 <= 9) {
                        pair1Values.Add(i);
                        pair2Values.Add(pair2);
                    }
                }
                range.Add(pair1.id, (pair1Values.Min(), pair1Values.Max()));
                range.Add(id, (pair2Values.Min(), pair2Values.Max()));
            }
            id++;
        }

        var max = string.Concat(range.Keys.OrderBy(k => k).Select(k => range[k].max.ToString()));
        var min = string.Concat(range.Keys.OrderBy(k => k).Select(k => range[k].min.ToString()));
       
        return (max, min);
    }

    public string Execute() {
        
        var input = new FileReader(24).Read().ToList();
        var codeInput = new List<(int p1, int p2, int p3)>();
        var currentRecord = (p1: 0, p2: 0, p3: 0);

        for (int i = 0; i < input.Count(); i++)
        {
            if(i % 18 == 4) {
                currentRecord = (int.Parse(input[i].Split(" ")[2]), currentRecord.p2, currentRecord.p3);
            }
            if(i % 18 == 5) {
                currentRecord = (currentRecord.p1, int.Parse(input[i].Split(" ")[2]), currentRecord.p3);
            }            
            if(i % 18 == 15) {
                currentRecord = (currentRecord.p1, currentRecord.p2, int.Parse(input[i].Split(" ")[2]));
                codeInput.Add(currentRecord);
            }                 
        }

        var (max, min) = GetSolution(codeInput);

        return $"The largest monad code is {max} and the minimum is {min}";
    }

}