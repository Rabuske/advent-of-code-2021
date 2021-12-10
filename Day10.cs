class Day10 : IDayCommand {

    private Dictionary<char,(int points, char openSymbol)> _symbols = new Dictionary<char, (int, char)>() {
        { ')' , (3, '(') },
        { ']' , (57, '[') },
        { '}' , (1197, '{') },
        { '>' , (25137, '<') },        
    };

    private Dictionary<char, int> _completionStringPoints = new Dictionary<char, int>() {
        { '(' , 1 },
        { '[' , 2 },
        { '{' , 3 },
        { '<' , 4 },        
    };    

    private int GetScoreForInvalidLines(List<char> line) {
        var openChunks = new Stack<char>();
        foreach (var symbol in line)
        {
            if("([{<".Contains(symbol)) {
                openChunks.Push(symbol);
            } else {
                var closeSymbolInfo = _symbols[symbol];
                if(openChunks.Pop() != closeSymbolInfo.openSymbol) {
                    return closeSymbolInfo.points;
                }
            }                
        }

        return 0;
    }

    private long GetScoreForCompletionString(List<char> line) {
        var openChunks = new Stack<char>();
        foreach (var symbol in line)
        {
            if("([{<".Contains(symbol)) {
                openChunks.Push(symbol);
            } else {
                openChunks.Pop();
            }                
        }        

        return openChunks.Aggregate((long) 0, (result, c) => result * 5 + _completionStringPoints[c]);
    }


    public string Execute() {

        var chunks = new FileReader(10).Read().Select(line => line.ToList()).ToList();

        var sumOfIllegalChunks = chunks.Sum(chunk => GetScoreForInvalidLines(chunk));
        var onlyIncorruptedLines = chunks.Where(chunk => GetScoreForInvalidLines(chunk) == 0);
        var scoreOfAllCompletionString = onlyIncorruptedLines.Select(chunk => GetScoreForCompletionString(chunk)).OrderBy(c => c).ToList();
        var scoreOfCompletionString = scoreOfAllCompletionString[scoreOfAllCompletionString.Count() / 2];

        return $"Sum of illegal chars is {sumOfIllegalChunks} and the score for completion string is {scoreOfCompletionString}";
    }
}