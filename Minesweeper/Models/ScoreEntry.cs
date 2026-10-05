namespace Minesweeper.Models;

public class ScoreEntry
{
    public int Id { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int Seconds { get; set; }
    public DateTime PlayedAt { get; set; }
}
