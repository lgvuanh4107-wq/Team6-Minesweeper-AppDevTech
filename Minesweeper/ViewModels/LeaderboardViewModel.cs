using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Minesweeper.Models;
using Minesweeper.Services;

namespace Minesweeper.ViewModels;

public class LeaderboardViewModel : INotifyPropertyChanged
{
    private string _currentDifficulty = "Dễ";

    public string CurrentDifficulty
    {
        get => _currentDifficulty;
        set
        {
            _currentDifficulty = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ScoreEntry> TopScores { get; set; } = new ObservableCollection<ScoreEntry>();

    // Khởi tạo ViewModel với độ khó ban đầu
    public LeaderboardViewModel(string initialDifficulty)
    {
        CurrentDifficulty = initialDifficulty;
        LoadScores(initialDifficulty);
    }

    // Tải danh sách điểm cao từ service
    public void LoadScores(string difficulty)
    {
        CurrentDifficulty = difficulty;
        TopScores.Clear();
        var scores = LeaderboardService.GetTop(difficulty, 10);
        foreach (var score in scores)
        {
            TopScores.Add(score);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    // Phát sự kiện khi thuộc tính thay đổi
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
