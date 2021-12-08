record Signal {
    public List<string> Patterns;
    public List<string> Digits;
    public Signal(string input) {
        var patternsAndDigits = input.Split("|");
        Patterns = patternsAndDigits[0].Trim().Split(" ").ToList();
        Digits = patternsAndDigits[1].Trim().Split(" ").ToList();
    }
}

class Day08: IDayCommand {

    private string[] _defaultDisplaySegments = new string[] {
        "abcefg", 
        "cf", 
        "acdeg", 
        "acdfg", 
        "bcdf", 
        "abdfg", 
        "abdefg", 
        "acf", 
        "abcdefg", 
        "abcdfg"
    };

    private bool DoesDigitLengthMach(string digitCode, int displayDigit){
        return digitCode.Length == _defaultDisplaySegments[displayDigit].Length;
    }

    private bool DoesDigitLengthMach(string digitCode, int[] displayDigits){
        return displayDigits.Any(d => DoesDigitLengthMach(digitCode, d));
    }    

    private Dictionary<string, int> DecodePatterns(List<string> patterns){
        var result = new Dictionary<string, int>();
        var wires = new Dictionary<char, char>();

        // 1.a Find digit 1 (only one with 2 segments lit)
        string? digit1 = patterns.Find(p => p.Length == 2);
        if(digit1 == null) return result;

        // 1.b Find digit 7 (only one with 3 segments lit)
        string? digit7 = patterns.Find(p => p.Length == 3);
        if(digit7 == null) return result;

        // 1.c The difference between them maps to the segment a
        wires.Add('a', digit7.Except(digit1).First());

        // 2.a Find digit 6 (composed by 6 segments - one of the unlit ones must be part of digit 1)
        string? digit6 = patterns.Find(p => p.Length == 6 && p.Intersect(digit1).Count() == 1);
        if(digit6 == null) return result;

        // 2.b The missing digit between 1 and 6 is c while the common digit is f
        wires.Add('c', digit1.Except(digit6).First());
        wires.Add('f', digit1.Intersect(digit6).First());

        // 3.a Find digit 4 (only one with 4 segments lit)
        string? digit4 = patterns.Find(p => p.Length == 4);
        if(digit4 == null) return result;

        // 3.b Find digit 3 (only one with a c f lit and length 5)
        string? digit3 = patterns.Find(p => p.Length == 5 && p.Contains(wires['a']) && p.Contains(wires['c']) && p.Contains(wires['f']));
        if(digit3 == null) return result;

        // 3.c determine segment d by comparing digits 3 and 4
        var segmentD = digit3.Intersect(digit4).Where(s => !wires.Values.Contains(s)).First();
        wires.Add('d', segmentD);

        // 3.d The only segment that has not been determined that is in digit 3 and not in 4 is the segment g
        var segmentG = digit3.Except(digit4).Where(s => !wires.Values.Contains(s)).First();
        wires.Add('g', segmentG);

        // 3.d The only segment that has not been determined that is in digit 4 and not in 3 is the segment b
        var segmentB = digit4.Except(digit3).Where(s => !wires.Values.Contains(s)).First();
        wires.Add('b', segmentB);

        // 4 Finally, the only unmapped digit is segment e
        wires.Add('e', "abcdefg".Except(wires.Values).First());

        // Now, just generate the new segments with the mapped value
        for (int i = 0; i < _defaultDisplaySegments.Length; i++)
        {
            var decoded = string.Concat(_defaultDisplaySegments[i].Select(s => wires[s]).OrderBy(s => s));
            result.Add(decoded, i);
        }

        return result;
    }

    public string Execute() {
        var signals = new FileReader(08).Read().Select(line => new Signal(line)).ToList();

        // Part 01
        var digits1478count = signals.Sum(signal => {
            return signal.Digits.Where(d => DoesDigitLengthMach(d, new int[]{1,4,7,8})).Count();
        });

        // Part 02
        var sumOfDecodedDisplays = signals.Sum(signal => {
            var decoder = DecodePatterns(signal.Patterns);
            var numberAsString = string.Concat(
                signal.Digits.Select(digit => decoder[string.Concat(digit.OrderBy(c => c))].ToString())
            );
            return long.Parse(numberAsString);
        });

        return $"Number of 1,4,7 and 8 digits {digits1478count}" + Environment.NewLine + 
               $"Sum of decoded digits is {sumOfDecodedDisplays}";
    }
}