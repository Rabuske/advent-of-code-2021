interface ALUOperation {
    public void Process(Dictionary<string, int> variables, string a, string? b, Stack<int> input);
}



class ALUOperationInput : ALUOperation {
    public void Process(Dictionary<string, int> variables, string a, string? b, Stack<int> input) {
        variables[a] = input.Pop();
    }
}

class ALUOperationAdd : ALUOperation {
    public void Process(Dictionary<string, int> variables, string a, string? b, Stack<int> input) {
        int bNumber = int.TryParse(b, out var parsed)? parsed : variables[b ?? ""];
        variables[a] = variables[a] + bNumber;
    }
}

class ALUOperationMultiply : ALUOperation {
    public void Process(Dictionary<string, int> variables, string a, string? b, Stack<int> input) {
        int bNumber = int.TryParse(b, out var parsed)? parsed : variables[b ?? ""];
        variables[a] = variables[a] * bNumber;
    }
}

class ALUOperationDiv : ALUOperation {
    public void Process(Dictionary<string, int> variables, string a, string? b, Stack<int> input) {
        decimal bNumber = int.TryParse(b, out var parsed)? parsed : variables[b ?? ""];
        variables[a] = (int)Math.Floor((decimal)variables[a] / bNumber);
    }
}

class ALUOperationMod : ALUOperation {
    public void Process(Dictionary<string, int> variables, string a, string? b, Stack<int> input) {
        int bNumber = int.TryParse(b, out var parsed)? parsed : variables[b ?? ""];
        variables[a] = variables[a] % bNumber;
    }
}

class ALUOperationEquals : ALUOperation {
    public void Process(Dictionary<string, int> variables, string a, string? b, Stack<int> input) {
        int bNumber = int.TryParse(b, out var parsed)? parsed : variables[b ?? ""];
        variables[a] = variables[a] == bNumber? 1 : 0;
    }
}

class ALUOperationFactory {
    public static ALUOperation GetOperation(string code) {
        return code switch {
            "inp" => new ALUOperationInput(),
            "add" => new ALUOperationAdd(),
            "mul" => new ALUOperationMultiply(),
            "div" => new ALUOperationDiv(),
            "mod" => new ALUOperationMod(),
            "eql" => new ALUOperationEquals(),
            _ =>  new ALUOperationAdd()
        };
    }
}

class ALUComputer {
    public Dictionary<string, int> Variables {get; set;} = new();
    public List<(ALUOperation operation, string varA, string varB)> Operations {get; set;} = new();
    
    public ALUComputer(List<(ALUOperation operation, string varA, string varB)> operations) {
        Operations = operations;
        Variables = new() {
            ["w"] = 0,
            ["x"] = 0,
            ["y"] = 0,
            ["z"] = 0,
        };
    }

    public void Execute(Stack<int> input) {
        foreach (var oper in Operations)
        {
            oper.operation.Process(Variables, oper.varA, oper.varB, input);
        }
    }

}

class Day24 : IDayCommand {

    // Brute Force is not an option, so lets transform the input into real code and deduct the input
    public (string max, string min) GetSolution(List<(int p1, int p2, int p3)> values) {

        // Figured out that there is no programmatic way to do this challenge. Hence, I'm using the logic described here (https://github.com/mrphlip/aoc/blob/master/2021/24.md)
        // And programing the values :)

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
        /*  
             A  B  C  D  E  F  G   H   I  J  K  L   M  N
        p1 [ 1  1  1  1 26  1  1  26  26 26  1 26  26 26]
        p2 [14 15 12 11 -5 14 15 -13 -16 -8 15 -8   0 -4]
        p3 [12  7  1  2  4 15 11   5   3  9  2  3   3 11]

        z.push(A + 12)

        z.push(B + 7)

        z.push(C + 1)

        z.push(D + 2)

        if E != z.pop() - 5: // Pops D
            z.push(E + 4)

        z.push(F + 15)

        z.push(G + 11)

        if H != z.pop() - 13: // Pops G
            z.push(E + 5)

        if I != z.pop() - 16: // Pops F
            z.push(I + 3)

        if J != z.pop() - 8: // Pops C
            z.push(J + 9)

        z.push(K + 2) 

        if L != z.pop() - 8: // Pops K
            z.push(L + 3)

        if M != z.pop() - 0: // Pops B
            z.push(M + 26)

        if N != z.pop() - 4: // Pops A
            z.push(N + 11)

        
        Conditions are:
        D + ( +2 -5 )  == D - 3  == E - met by 4 and 1 - 5 and 2 ... 9 and 6
        G + ( +11 -13) == G - 2  == H - met by 3 and 1 - 4 and 2 ... 9 and 7
        F + ( +15 -16) == F - 1  == I - met by 2 and 1 - 4 and 3 ... 9 and 8
        C + ( +1 -8)   == C - 7  == J - met by 8 and 1 - 9 and 2
        K + ( +2 -8)   == K - 6  == L - met by 7 and 1 - 8 and 2 ... 9 and 3
        B + ( +7 -0)   == B + 7  == M - met by 1 and 8 - 2 and 9
        A + ( +12 - 4) == A + 8  == N - met by 1 and 9

        Hence the ranges are:
        A [1, 1]
        B [1, 2]
        C [8, 9]
        D [4, 9]
        E [1, 6]
        F [2 ,9]
        G [3 ,9]
        H [1, 7]
        I [1 ,8]
        J [1, 2]
        L [1, 3]
        M [8, 9]
        N [9 ,9]

             
        1184123111189 1299699782399 
        */        

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