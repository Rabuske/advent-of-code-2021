class Day06 : IDayCommand {
    
    private void AddFishToDictionary(int day, long numberOfFish, Dictionary<int, long> dictionary) {
        if(dictionary.ContainsKey(day)) {
            dictionary[day] = dictionary[day] + numberOfFish;
        } else {
            dictionary.Add(day, numberOfFish);
        }
    }

    private Dictionary<int, long> Reproduce(Dictionary<int, long> timers, int days) {
        var returnTimer = timers.Select(t => t).ToDictionary(g => g.Key, g => g.Value);
        for (int i = 0; i < days; i++)
        {
            Dictionary<int, long> newTimers = new Dictionary<int, long>();
            returnTimer.Keys.ToList().ForEach(day => {
                if(day == 0) {
                    AddFishToDictionary(8, returnTimer[day], newTimers); // Reproduce the fish
                    AddFishToDictionary(6, returnTimer[day], newTimers); // Reset the timer
                } else {
                    AddFishToDictionary(day - 1, returnTimer[day], newTimers);
                }
            });
            returnTimer = newTimers;
        }
        return returnTimer;
    }
    public string Execute() {
        List<int> lanternfish = new FileReader(06).Read().First().Split(",").Select(n => int.Parse(n)).ToList();

        Dictionary<int, long> timers = lanternfish.GroupBy(n => n).ToDictionary(group => group.Key, group => (long) group.ToList().Count());
        
        var timerAfter80 = Reproduce(timers, 80);
        long after80DaysCount = timerAfter80.Select(t => t.Value).Sum();

        var timerAfter256 = Reproduce(timers, 256);
        long after256DaysCount = timerAfter256.Select(t => t.Value).Sum();

        return $"Number of resulting lanternfish after 80 days is {after80DaysCount}" + Environment.NewLine +
               $"Number of resulting lanternfish after 256 days is {after256DaysCount}";
    }
}