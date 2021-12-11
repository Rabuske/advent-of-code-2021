class Octopus {
    public int EnergyLevel {get; set;}
    public (int x, int y) Postion {get; set;}
    public List<Octopus> AdjacentOctopi {get; init;}

    public Octopus(int energyLevel) {
        EnergyLevel = energyLevel;
        AdjacentOctopi = new List<Octopus>();
    }

    public void Flash(HashSet<Octopus> alreadyFlashed) {
        if(this.EnergyLevel <= 9) return;
        if(alreadyFlashed.Contains(this)) return;
        alreadyFlashed.Add(this);
        this.EnergyLevel = 0;        
        // Increase Energy of Neighbors
        AdjacentOctopi.ForEach(o => o.IncreaseEnergy(alreadyFlashed));
        // Process Neighbors
        AdjacentOctopi.ForEach(o => o.Flash(alreadyFlashed));
    }

    public void IncreaseEnergy(HashSet<Octopus> alreadyFlashed) {
        if(alreadyFlashed.Contains(this)) return;
        EnergyLevel += 1;
    }
}

class Day11: IDayCommand {

    public long SimulateNumberOfSteps(int steps, List<Octopus> octopi) {
        long numberOfFlashes = 0;
        for (int i = 0; i < steps; i++){
            numberOfFlashes += SimulateSingleStep(octopi);
        }

        return numberOfFlashes;
    }

    public long SimulateUntilAllFlash(List<Octopus> octopi) {
        long currentStep = 0;
        long octupiThatFlashed = 0;
        do{
            currentStep += 1;
            octupiThatFlashed = SimulateSingleStep(octopi);
        } while(octopi.Count() != octupiThatFlashed);
        return currentStep;        
    }

    private static long SimulateSingleStep(List<Octopus> octopi)
    {
        var flashedOctipi = new HashSet<Octopus>();
        octopi.ForEach(o => o.IncreaseEnergy(flashedOctipi));
        octopi.ForEach(o => o.Flash(flashedOctipi));
        return flashedOctipi.Count();
    }

    public string Execute() {

        var octopi = new FileReader(11).Read()
            .Select(line => line.Select(item => new Octopus((int)char.GetNumericValue(item))).ToArray())
            .ToArray();

        // Set the adjacent ones
        for (int i = 0; i < octopi.Count(); i++)
        {
            for (int j = 0; j < octopi[i].Count(); j++)
            {
                var currentOctopus = octopi[i][j];
                currentOctopus.Postion = (i, j);
                if(i - 1 >= 0) currentOctopus.AdjacentOctopi.Add(octopi[i-1][j]);
                if(i + 1 < octopi.Count()) currentOctopus.AdjacentOctopi.Add(octopi[i+1][j]);
                if(j - 1 >= 0) currentOctopus.AdjacentOctopi.Add(octopi[i][j-1]);
                if(j + 1 < octopi[i].Count()) currentOctopus.AdjacentOctopi.Add(octopi[i][j+1]);

                if(i - 1 >= 0 && j - 1 >= 0) currentOctopus.AdjacentOctopi.Add(octopi[i-1][j-1]);
                if(i - 1 >= 0 && j + 1 < octopi[i].Count()) currentOctopus.AdjacentOctopi.Add(octopi[i-1][j+1]);
                if(i + 1 < octopi.Count() && j - 1 >= 0) currentOctopus.AdjacentOctopi.Add(octopi[i+1][j-1]);
                if(i + 1 < octopi.Count() && j + 1 < octopi[i].Count()) currentOctopus.AdjacentOctopi.Add(octopi[i+1][j+1]);
            }
        }
        var octupiAsSingleList = octopi.SelectMany(o => o).ToList();
        var resultAfter100Steps = SimulateNumberOfSteps(100, octupiAsSingleList);
        var stepWhenAllFlashed = SimulateUntilAllFlash(octupiAsSingleList) + 100;

        return $"The number of flashes after 100 steps is {resultAfter100Steps} and the step during which all octupi flash is {stepWhenAllFlashed}";
    }
}