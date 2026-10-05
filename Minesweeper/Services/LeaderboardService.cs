using Minesweeper.Models;

namespace Minesweeper.Services;

public static class LeaderboardService
{

    public static void Initialize()
    {
    }

    public static List<ScoreEntry> GetTop(string difficultyName, int count = 10)
    {
        return new List<ScoreEntry>();
    }

    public static int? AddScore(string playerName, string difficultyName, int seconds)
    {
        return null;
    }
}
