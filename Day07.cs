class Day07: IDayCommand {

    private long CalculateComplexFuel(int position1, int position2) {
        var distance = Math.Abs(position1 - position2);
        return ((long)Math.Pow(distance,2) + distance) / 2;
    }
    public string Execute() {

        List<int> crabPositions = new FileReader(07).Read().First().Split(",").Select(n => int.Parse(n)).ToList();
        var allPositions = Enumerable.Range(crabPositions.Min(), crabPositions.Max());
        var totalFuelSimpleCost = allPositions.Min(p => crabPositions.Sum(c => Math.Abs(c - p)));
        var totalFuelComplexCost = allPositions.Min(p => crabPositions.Sum(c => CalculateComplexFuel(c, p)));

        return $"Total fuel spent with simple cost calculation is {totalFuelSimpleCost}" + Environment.NewLine +
               $"Total fuel spent with complex cost calculation is {totalFuelComplexCost}";
    }

}