class Day01 : IDayCommand {
    private int GetNumberOfMeasurementsLargerThanPrevious(List<int> measurements, int windowSize) {        
        int increaseCount = 0;

        int previousWindow = measurements.Take(windowSize).Sum();

        for (int i = windowSize; i < measurements.Count(); i++)
        {
            int currentWindow = previousWindow - measurements[i - windowSize] + measurements[i];            
            if(currentWindow > previousWindow) increaseCount++;
            previousWindow = currentWindow;
        }

        return increaseCount;
    }
  
    public string Execute() {
        List<int> measurements = new FileReader(01).Read().Select(line => int.Parse(line)).ToList();

        return $"Number of measurement large than the previous one (single measurement): {GetNumberOfMeasurementsLargerThanPrevious(measurements, 1)}" 
                + Environment.NewLine 
                + $"Number of measurement large than the previous one (window measurement): {GetNumberOfMeasurementsLargerThanPrevious(measurements, 3)}";
    }

}