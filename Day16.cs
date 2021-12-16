interface IOperation {
    decimal Compute(List<PacketBase> oper);
}

class OperationSum : IOperation {
    public decimal Compute(List<PacketBase> oper) => oper.Sum(o => o.Evaluate());
}

class OperationProduct : IOperation {
    public decimal Compute(List<PacketBase> oper) => oper.Aggregate((decimal) 1, (r, o) => o.Evaluate() * r);
}

class OperationMinimum : IOperation {
    public decimal Compute(List<PacketBase> oper) => oper.Min(o => o.Evaluate());
}

class OperationMaximum : IOperation {
    public decimal Compute(List<PacketBase> oper) => oper.Max(o => o.Evaluate());
}

class OperationGreaterThan : IOperation {
    public decimal Compute(List<PacketBase> oper) => oper.First().Evaluate() > oper.Last().Evaluate()? 1 : 0;
}

class OperationLessThan : IOperation {
    public decimal Compute(List<PacketBase> oper) => oper.First().Evaluate() < oper.Last().Evaluate()? 1 : 0;
}

class OperationEquals : IOperation {
    public decimal Compute(List<PacketBase> oper) => oper.First().Evaluate() == oper.Last().Evaluate()? 1 : 0;
}

class NullOperation : IOperation {
    public decimal Compute(List<PacketBase> oper) {
        Console.WriteLine("Null Operation Found");
        return 0;
    }
}

class OperationFactory {
    public static IOperation Get(int type) {
        return type switch {
            0 => new OperationSum(),
            1 => new OperationProduct(),
            2 => new OperationMinimum(),
            3 => new OperationMaximum(),
            5 => new OperationGreaterThan(),
            6 => new OperationLessThan(),
            7 => new OperationEquals(),
            _ => new NullOperation()
        };
    }
}

abstract class PacketBase {    
    public int Version {get; set;}
    public int TypeId {get; set;}

    abstract public decimal Evaluate();
    abstract public int SumOfVersions();
}

class OperatorPacket : PacketBase {
    public List<PacketBase> InnerPackets {get; set;}
    public IOperation Operation {get; set;}

    public OperatorPacket(int version, int typeId) {
        InnerPackets = new List<PacketBase>();
        Version = version;
        TypeId = typeId;
        Operation = OperationFactory.Get(typeId);
    }

    override public decimal Evaluate() => Operation.Compute(InnerPackets);

    override public int SumOfVersions() => Version + InnerPackets.Sum(p => p.SumOfVersions());
}

class LiteralPacket : PacketBase {
    public decimal Value {get; set;}

    public LiteralPacket(long value, int version, int typeId) {
        Value = value;
        Version = version;
        TypeId = typeId;
    }

    override public decimal Evaluate() => Value;

    override public int SumOfVersions() => Version;
}

class Day16 : IDayCommand {

    public int TakeNumber(List<char> input, int size) {
        var binaryString = string.Concat(input.Take(size));
        input.RemoveRange(0, size);
        return Convert.ToInt32(binaryString, 2);
    }

    public List<PacketBase> ParseOperator(List<char> input, int allowedPackets=int.MaxValue) {
        var result = new List<PacketBase>();
        var currentNumberOfPackets = allowedPackets;
        while(input.Count() > 0 && currentNumberOfPackets > 0 && input.Any(c => c == '1')) {
            currentNumberOfPackets -= 1;
            int version = TakeNumber(input, 3);
            int typeId = TakeNumber(input, 3);
            
            if(typeId == 4){
                var hasMoreLiterals = true;
                var literalAsBinary = string.Empty;
                while(hasMoreLiterals) {
                    hasMoreLiterals = Convert.ToBoolean(TakeNumber(input, 1));
                    literalAsBinary += string.Concat(input.Take(4));
                    input.RemoveRange(0, 4);
                }
                result.Add(new LiteralPacket(Convert.ToInt64(literalAsBinary, 2), version, typeId));
            } else {
                var newOperatorPacket = new OperatorPacket(version, typeId);
                result.Add(newOperatorPacket);
                var lengthType = TakeNumber(input, 1);
                if(lengthType == 0) {
                    int numberOfBitsInnerPackets = TakeNumber(input, 15);
                    newOperatorPacket.InnerPackets.AddRange(ParseOperator(input.Take(numberOfBitsInnerPackets).ToList()));
                    input.RemoveRange(0, numberOfBitsInnerPackets);
                } else {
                    int numberOfPackets = TakeNumber(input, 11);
                    newOperatorPacket.InnerPackets.AddRange(ParseOperator(input, numberOfPackets));
                }
            }
        }
        return result;
    }

    public string Execute() {
        var input = new FileReader(16).Read().First();
        var binary = input.SelectMany(c => MyConverter.HexToBinary(c).Select(c1 => c1)).ToList();
        if(binary is null) return "Error";
        
        var operatorPackages = ParseOperator(binary);
        var sumOfVersions = operatorPackages.Sum(op => op.SumOfVersions());
        var resultOfEvaluation = operatorPackages.First().Evaluate();

        return $"The sum of versions is {sumOfVersions} and the evaluated value is {resultOfEvaluation}";
    }

}