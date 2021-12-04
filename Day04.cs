

class BingoBoard {
    private List<(int i, int j, int number)> _drawnNumbers = new List<(int, int, int)>();
    private List<List<int>> _board;

    public BingoBoard(List<string> board) {
        _board = board.Select(line => line.Replace("  " , " ").Trim().Split(" ").Select(number => int.Parse(number)).ToList()).ToList();
    }

    public void AddDrawnNumber(int number) {
        for (int i = 0; i < _board.Count(); i++)
            for (int j = 0; j < _board[i].Count(); j++)
                if(_board[i][j] == number) _drawnNumbers.Add((i,j,number));

    }

    public bool IsBingo() {
        for (int checkingPosition = 0; checkingPosition < 5; checkingPosition++)
        {   
            // Check Lines
            if(_drawnNumbers.Where(position => position.i == checkingPosition).Count() == _board.Count()) return true;
            // Check Columns
            if(_drawnNumbers.Where(position => position.j == checkingPosition).Count() == _board.Count()) return true;
        }
        return false;
    }

    private bool HasNumberBeenDrawn(int number) {
        foreach (var drawnNumber in _drawnNumbers)
        {
            if(drawnNumber.number == number) return true; 
        }
        return false;
    }

    public int CalculateScore() {
        int unmarkedNumbersSum = _board.Select(line => line.Where(number => !HasNumberBeenDrawn(number)).Sum()).Sum();
        int lastDrawnNumber = _drawnNumbers.Last().number;
        return unmarkedNumbersSum * lastDrawnNumber;
    }

    public void ResetBoard() {
        _drawnNumbers = new List<(int, int, int)>();
    }
}

class Day04 : IDayCommand {

    private List<BingoBoard> BuildBoards(List<string> input) {
        List<BingoBoard> boards = new List<BingoBoard>();
        List<string> linesToBuildBoard = new List<string>();
        foreach (var line in input)
        {
            if(line.Length <= 1) {
                boards.Add(new BingoBoard(linesToBuildBoard));
                linesToBuildBoard = new List<string>();
            } else {
                linesToBuildBoard.Add(line);
            }
        }

        if(linesToBuildBoard.Count() > 0) {
            boards.Add(new BingoBoard(linesToBuildBoard));
        }

        return boards;
    }

    private int PlayAndGetFinalScore(List<int> numbersToDrawn, List<BingoBoard> boards) {
        foreach (var number in numbersToDrawn)
        {
            foreach (var board in boards)
            {
                board.AddDrawnNumber(number);
                if(board.IsBingo()) {
                    return board.CalculateScore();
                }
            }
        }
        return -1;
    }

    private int PlayUntilFinalBoardWinsAndGetFinalScore(List<int> numbersToDrawn, List<BingoBoard> boards) {
        var remainingBoards = boards.Select(b => b);
        foreach (var number in numbersToDrawn)
        {
            foreach (var board in remainingBoards)
            {
                board.AddDrawnNumber(number);
                if(board.IsBingo()) {
                    if(remainingBoards.Count() > 1) {
                        remainingBoards = remainingBoards.Where(b => b != board);
                    } else {
                        return board.CalculateScore();
                    }
                }
            }
        }
        return -1;
    }

    public string Execute() {
        var fileContent = new FileReader(04).Read().ToList();
        var test = fileContent[0].Split(" ");
        List<int> numbersToDrawn = fileContent[0].Split(",").Select(n => int.Parse(n)).ToList();
        List<BingoBoard> boards = BuildBoards(fileContent.Skip(2).ToList());

        var part01 = PlayAndGetFinalScore(numbersToDrawn, boards);
        boards.ForEach(board => board.ResetBoard());
        var part02 = PlayUntilFinalBoardWinsAndGetFinalScore(numbersToDrawn, boards);

        return $"The winning board score is {part01}" + Environment.NewLine +
               $"The last winning board score is {part02}";
    }

}