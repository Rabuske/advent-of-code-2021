// See https://aka.ms/new-console-template for more information

bool couldParse = false;
IDayCommand command = new NullDay();
do {
    Console.Write("Enter the day you want to execute: ");
    int day = 0;
    couldParse = int.TryParse(Console.ReadLine(), out day);
    if(couldParse) {
        command = new DayCommandFactory().GetCommand(day);
    }
} while (!couldParse);

Console.WriteLine(command.Execute());
