using Microsoft.EntityFrameworkCore;

namespace Minesweeper.Models;

public class AppDbContext : DbContext
{
    public DbSet<ScoreEntry> Scores => Set<ScoreEntry>();

   
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
       string path = System.IO.Path.Combine(AppContext.BaseDirectory, "minesweeper.db");
        options.UseSqlite($"Data Source={path}");
    }
}
