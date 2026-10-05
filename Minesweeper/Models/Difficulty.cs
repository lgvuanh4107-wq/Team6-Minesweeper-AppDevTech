namespace Minesweeper.Models;

public record Difficulty(string Name, int Width, int Height, int Mines)
{
    public static readonly Difficulty Easy = new("Dễ", 9, 9, 10);
    public static readonly Difficulty Medium = new("Vừa", 16, 16, 40);
    public static readonly Difficulty Hard = new("Khó", 30, 16, 99);

    public static readonly Difficulty[] All = { Easy, Medium, Hard };
}
