class DeterministicDie {
    public int NumberOfRolls {get; private set;} = 0;
    private int _currentRoll {get; set;} = 1;

    public int Roll() {
        var result = _currentRoll;
        NumberOfRolls += 1;
        _currentRoll += 1;
        if(_currentRoll > 100) _currentRoll = 1;
        return result;
    }
}

class Player {
    public int Id {get; init;}
    public int TrackPosition {get; private set;}
    public int Score {get; private set;}

    public Player(int id, int startingPosition, int score = 0){
        Id = id;        
        TrackPosition = startingPosition;
        Score = score;
    }

    public void Play(List<int> rolls) {
        MoveOnTrack(rolls.Sum());
        Score += TrackPosition + 1;
    }

    private void MoveOnTrack(int positions){
        TrackPosition += positions;
        TrackPosition %= 10;
    }
}

class Universe {

    public Player Player1 {get; init;}
    public Player Player2 {get; init;}
    public Player? WinningPlayer {get; private set;}

    public static Dictionary<int, int> rolls = new Dictionary<int, int>{
        {3, 1},
        {4, 3},
        {5, 6},
        {6, 7},
        {7, 6},
        {8, 3},
        {9, 1}
    };

    public (int p1Position, int p1Score, int p2Position, int p2Score) Tally => (
        Player1.TrackPosition, Player1.Score, Player2.TrackPosition, Player2.Score
    );

    public decimal NumberOfUniverses {get; private set;}

    public Universe(Player player1, Player player2, decimal numberOfUniverses = 1) {
        Player1 = player1;
        Player2 = player2;
        NumberOfUniverses = numberOfUniverses;
    }
    
    public List<Universe> Play(int playerId) {
        var result = new List<Universe>();      

        foreach (var roll in rolls)
        {
            var p1 = new Player(Player1.Id, Player1.TrackPosition, Player1.Score);
            var p2 = new Player(Player2.Id, Player2.TrackPosition, Player2.Score);
            if(playerId == p1.Id) {
                p1.Play(new List<int>(){roll.Key});
            } else {
                p2.Play(new List<int>(){roll.Key});
            }
            var newUniverse = new Universe(p1, p2, NumberOfUniverses * roll.Value);
            newUniverse.WinningPlayer = p2.Score >= 21? p2 : newUniverse.WinningPlayer;
            newUniverse.WinningPlayer = p1.Score >= 21? p1 : newUniverse.WinningPlayer;
            result.Add(newUniverse);
        }
        return result;
    }

}


class Day21 : IDayCommand {    

    public List<Player> ReadAndParsePlayers() {
        return new FileReader(21).Read().Select(line => {
            var numbers = line.Replace("Player ", "").Replace("starting position", "").Split(":");
            return new Player(int.Parse(numbers[0].Trim()), int.Parse(numbers[1].Trim()) -1);
        }).ToList();
    }

    public long Part01() {
        var players = ReadAndParsePlayers();

        var die = new DeterministicDie();
        Player? winningPlayer = null;

        while(winningPlayer is null) {
            foreach (var p in players)
            {
                p.Play(new List<int>(){die.Roll(), die.Roll(), die.Roll()});
                if(p.Score >= 1000) {
                  winningPlayer = p;  
                  break;
                }                 
            }
        }

        var scoreLosingPlayers = players.Where(p => p != winningPlayer).Sum(p => p.Score);
        return scoreLosingPlayers * die.NumberOfRolls;
    }

    public decimal Part02() {
        var players = ReadAndParsePlayers();

        var unfinishedUniverses = new List<Universe>() { new Universe(players[0], players[1])};
        var finishedUniverses = new List<Universe>();
        
        var playerIdIndex = 1;
        while(unfinishedUniverses.Count() > 0) {
            var processedUniverses = unfinishedUniverses.SelectMany(u => u.Play(playerIdIndex)).ToList();
            playerIdIndex += 1;
            playerIdIndex = playerIdIndex > 2? 1 : playerIdIndex;
            finishedUniverses.AddRange(processedUniverses.Where(u => u.WinningPlayer is not null));
            unfinishedUniverses = processedUniverses.Except(finishedUniverses).ToList();

            // Combine Universes that have the same tally
            var uniqueUniverses = processedUniverses
                .Except(finishedUniverses)
                .GroupBy(u => u.Tally)
                .ToDictionary(g => g.Key, g => g.Sum(u => u.NumberOfUniverses));
            
            unfinishedUniverses = uniqueUniverses.Select(u => new Universe(
                new Player(1, u.Key.p1Position, u.Key.p1Score),
                new Player(2, u.Key.p2Position, u.Key.p2Score),
                u.Value
            )).ToList();
        }

        var player1Wins = finishedUniverses.Where(u => u.WinningPlayer?.Id == 1).Sum(u => u.NumberOfUniverses);
        var player2Wins = finishedUniverses.Where(u => u.WinningPlayer?.Id == 2).Sum(u => u.NumberOfUniverses);

        return Math.Max(player1Wins, player2Wins);
    }

    public string Execute() {

        return $"The score of the losing player multiplied by the number of rolls is {Part01()}" + Environment.NewLine +
               $"When playing with quantum dice, the player with most wins won in {Part02()} universes";
    }
}