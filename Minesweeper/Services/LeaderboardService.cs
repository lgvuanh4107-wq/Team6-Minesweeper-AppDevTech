using Minesweeper.Models;

namespace Minesweeper.Services;

public static class LeaderboardService
{
   
    public static void Initialize()
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();

        if (db.Scores.Any())
            return;

        db.Scores.AddRange(
            Sample("Minh Anh", "Dễ", 9, 12),
            Sample("Quốc Bảo", "Dễ", 14, 10),
            Sample("Thu Hà", "Dễ", 19, 9),
            Sample("Gia Huy", "Dễ", 24, 7),
            Sample("Ngọc Lan", "Dễ", 31, 4),
            Sample("Đức Anh", "Dễ", 38, 2),

            Sample("Hoàng Nam", "Vừa", 72, 11),
            Sample("Phương Thảo", "Vừa", 95, 8),
            Sample("Tuấn Kiệt", "Vừa", 118, 6),
            Sample("Bảo Châu", "Vừa", 143, 5),
            Sample("Khánh Linh", "Vừa", 171, 3),
            Sample("Thanh Tùng", "Vừa", 196, 1),

            Sample("Việt Hùng", "Khó", 214, 13),
            Sample("Mai Phương", "Khó", 275, 9),
            Sample("Duy Khang", "Khó", 338, 7),
            Sample("Thảo Vy", "Khó", 412, 5),
            Sample("Trung Hiếu", "Khó", 505, 3),
            Sample("Hải Yến", "Khó", 587, 2));

        db.SaveChanges();
    }

    
    public static List<ScoreEntry> GetTop(string difficultyName, int count = 10)
    {
        using var db = new AppDbContext();
        return db.Scores
            .Where(s => s.Difficulty == difficultyName)
            .OrderBy(s => s.Seconds)
            .ThenBy(s => s.Id)
            .Take(count)
            .ToList();
    }

    public static int? AddScore(string playerName, string difficultyName, int seconds)
    {
        using var db = new AppDbContext();
        db.Scores.Add(new ScoreEntry
        {
            PlayerName = playerName,
            Difficulty = difficultyName,
            Seconds = seconds,
            PlayedAt = DateTime.Now
        });
        db.SaveChanges();

        int rank = db.Scores.Count(s => s.Difficulty == difficultyName && s.Seconds <= seconds);
        return rank <= 10 ? rank : null;
    }

    private static ScoreEntry Sample(string name, string difficulty, int seconds, int daysAgo)
    {
        return new ScoreEntry
        {
            PlayerName = name,
            Difficulty = difficulty,
            Seconds = seconds,
            PlayedAt = DateTime.Now.AddDays(-daysAgo)
        };
    }
}
