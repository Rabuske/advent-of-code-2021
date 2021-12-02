class Day02 : IDayCommand {

    public int Part01(List<(string operation, int value)> commands) {
        int depth = 0;
        int horizontal = 0;

        commands.ForEach(command => {
            switch (command.operation)
            {
                case "forward":
                    horizontal += command.value;
                    break;
                case "up":
                    depth -= command.value;
                    break;
                case "down":
                    depth += command.value;
                    break;
            }
        });

        return depth * horizontal;
    }

    public int Part02(List<(string operation, int value)> commands) {
        int depth = 0;
        int horizontal = 0;
        int aim = 0;

        commands.ForEach(command => {
            switch (command.operation)
            {
                case "forward":                    
                    horizontal += command.value;
                    depth += command.value * aim;
                    break;
                case "up":
                    aim -= command.value;
                    break;
                case "down":
                    aim += command.value;
                    break;
            }
        });

        return depth * horizontal;
    }    

    public string Execute() {
        List<(string, int)> commands = new FileReader(02).Read()
            .Select(line => {
                var separatedLine = line.Split(" ");
                return (separatedLine[0], int.Parse(separatedLine[1]));
            }).ToList();
        
        return $"The result of the multiplication of positions of part 1 is {Part01(commands)} \n" +
               $"The result of the multiplication of positions of part 2 is {Part02(commands)}" ;
    }

}