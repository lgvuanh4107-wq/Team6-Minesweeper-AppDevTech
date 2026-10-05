using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace Minesweeper.Services;

public enum Sound
{
    Click,
    Flag,
    Explode,
    Win
}

public static class SoundManager
{
    public static bool IsMuted { get; private set; } = false;
    private static readonly Dictionary<Sound, SoundPlayer> _players = new();

    static SoundManager()
    {
        LoadSound(Sound.Click, "click.wav");
        LoadSound(Sound.Flag, "flag.wav");
        LoadSound(Sound.Explode, "explode.wav");
        LoadSound(Sound.Win, "win.wav");
    }

    // Tải âm thanh vào bộ nhớ từ file nếu file tồn tại
    private static void LoadSound(Sound s, string fileName)
    {
        try
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Sounds", fileName);
            if (File.Exists(path))
            {
                var player = new SoundPlayer(path);
                player.Load();
                _players[s] = player;
            }
        }
        catch
        {
            // Bỏ qua lỗi nếu file thiếu hoặc hỏng
        }
    }

    // Bật hoặc tắt âm thanh
    public static void ToggleMute()
    {
        IsMuted = !IsMuted;
    }

    // Phát âm thanh tương ứng nếu không bị tắt tiếng
    public static void Play(Sound s)
    {
        if (IsMuted) return;
        
        if (_players.TryGetValue(s, out var player))
        {
            player.Play();
        }
    }
}
