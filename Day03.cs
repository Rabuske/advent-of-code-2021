interface IRatingFilter {
    public bool ShouldKeep((int zeroes, int ones) count, char digit);
}

class OxygenGeneratorRatingComparator : IRatingFilter {
    public bool ShouldKeep((int zeroes, int ones) count, char digit){
        char digitToKeep = count.ones >= count.zeroes? '1' : '0';
        return digit == digitToKeep;
    }
}

class CO2ScrubberRatingComparator : IRatingFilter {
    public bool ShouldKeep((int zeroes, int ones) count, char digit){
        char digitToKeep = count.ones < count.zeroes? '1' : '0';
        return digit == digitToKeep;
    }
}

class Day03 : IDayCommand {

    public List<(int, int)> GenerateCountOfDigits(List<char[]> numbers) {
        var resultingNumber = new List<(int zeroesCount, int onesCount)>();            

        numbers.ForEach(number => {
            for (int i = 0; i < number.Count(); i++)
            {   
                // Insert the initial record in case it does not exist
                if(resultingNumber.Count <= i) {
                    resultingNumber.Add((0,0));
                }
                var digit = number[i];
                if(digit == '0') {
                    resultingNumber[i] = (resultingNumber[i].zeroesCount + 1, resultingNumber[i].onesCount);
                } else {
                    resultingNumber[i] = (resultingNumber[i].zeroesCount, resultingNumber[i].onesCount + 1);
                }
            }
        });
        
        return resultingNumber;
    }

    public (int gammaRate, int epsilonRate) Part01(List<char[]> numbers) {
        List<(int zeroes, int ones)> digitsCount = GenerateCountOfDigits(numbers);

        int gammaRate   = Convert.ToInt32(string.Join("", digitsCount.Select(pair => pair.ones > pair.zeroes ? "1" : "0")) ?? "", 2);
        int epsilonRate = Convert.ToInt32(string.Join("", digitsCount.Select(pair => pair.ones < pair.zeroes ? "1" : "0")) ?? "", 2);

        return (gammaRate, epsilonRate);
    }

    public int Part02(List<char[]> numbers, IRatingFilter filter) {
        List<char[]> newNumbers = numbers;
        int currentIndex = 0;

        while(newNumbers.Count > 1 && currentIndex < numbers[0].Length)
        {
            // Not the most efficient way to do it, since it will calculate everything again... but performance is not an issue so far
            List<(int zeroes, int ones)> digitsCount = GenerateCountOfDigits(newNumbers);
            newNumbers = newNumbers.Where(number => filter.ShouldKeep(digitsCount[currentIndex], number[currentIndex])).ToList();
            currentIndex++;
        }

        return Convert.ToInt32(string.Join("", newNumbers[0]), 2);
    }

    public string Execute() {
        var numbersAsString = new FileReader(03).Read();
        List<char[]> numbersAsBinaryArray = numbersAsString.Select(numbersAsString => numbersAsString.ToArray()).ToList();
        
        (int gammaRate, int epsilonRate) = Part01(numbersAsBinaryArray);
        int oxygenGeneratorRating = Part02(numbersAsBinaryArray, new OxygenGeneratorRatingComparator());
        int C02ScrubberRating = Part02(numbersAsBinaryArray, new CO2ScrubberRatingComparator());

        return $"Part 01: Gamma Rate is {gammaRate} Epsilon Rate is {epsilonRate}, so the total is {gammaRate * epsilonRate}" + Environment.NewLine +
               $"Part 02: Oxygen Generator Rating is {oxygenGeneratorRating} C02 Scrubber Rating is {C02ScrubberRating}, so the total is {oxygenGeneratorRating * C02ScrubberRating}";
    }

}