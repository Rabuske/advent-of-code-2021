class Day11: IDayCommand {
    public void Flash(HashSet<Node<int>> alreadyFlashed, Node<int> node) {
        if(node.Value <= 9) return;
        if(alreadyFlashed.Contains(node)) return;
        alreadyFlashed.Add(node);
        node.Value = 0;        
        // Increase Energy of Neighbors
        node.AdjacentNodes.Keys.ToList().ForEach(n => IncreaseEnergy(alreadyFlashed, n));
        // Process Neighbors
        node.AdjacentNodes.Keys.ToList().ForEach(n => Flash(alreadyFlashed, n));
    }

    public void IncreaseEnergy(HashSet<Node<int>> alreadyFlashed, Node<int> node) {
        if(alreadyFlashed.Contains(node)) return;
        node.Value += 1;
    }

    public long SimulateNumberOfSteps(int steps, Map<int> octopi) {
        long numberOfFlashes = 0;
        for (int i = 0; i < steps; i++){
            numberOfFlashes += SimulateSingleStep(octopi);
        }

        return numberOfFlashes;
    }

    public long SimulateUntilAllFlash(Map<int> octopi) {
        long currentStep = 0;
        long octupiThatFlashed = 0;
        do{
            currentStep += 1;
            octupiThatFlashed = SimulateSingleStep(octopi);
        } while(octopi.Nodes.Count() != octupiThatFlashed);
        return currentStep;        
    }

    private long SimulateSingleStep(Map<int> octopi)
    {
        var flashedOctipi = new HashSet<Node<int>>();
        octopi.Nodes.ForEach(o => IncreaseEnergy(flashedOctipi, o));
        octopi.Nodes.ForEach(o => Flash(flashedOctipi, o));
        return flashedOctipi.Count();
    }

    public string Execute() {

        var grid = new FileReader(11).Read()
            .Select(line => line.Select(item => new Node<int>((int)char.GetNumericValue(item))).ToArray())
            .ToArray();

        var map = new Map<int>(grid, considerDiagonals:true);
        var resultAfter100Steps = SimulateNumberOfSteps(100, map);
        var stepWhenAllFlashed = SimulateUntilAllFlash(map) + 100;

        return $"The number of flashes after 100 steps is {resultAfter100Steps} and the step during which all octupi flash is {stepWhenAllFlashed}";
    }
}