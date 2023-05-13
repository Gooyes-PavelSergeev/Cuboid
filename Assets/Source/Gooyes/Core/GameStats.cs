using Gooyes.Tools;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace Cubic
{
    public class GameStats : Singleton<GameStats>
    {
        private int _score;
        public int Score { get { return _score; } }
        public int Highscore { get => PlayerPrefs.GetInt("Highscore", _score); }
        public event Action<int> ScoreChanged;
        public event Action<int> HighscoreChanged;

        private void OnEnable()
        {
            Cuboid.SpawnEvent += OnCuboidSpawn;
        }

        private void OnDisable()
        {
            Cuboid.SpawnEvent -= OnCuboidSpawn;
        }

        private void OnCuboidSpawn(Cuboid cuboid)
        {
            _score += cuboid.Number;
            UpdateHighscore();
            ScoreChanged?.Invoke(_score);
        }

        private void UpdateHighscore()
        {
            int highscore = PlayerPrefs.GetInt("Highscore", 0);
            if (_score > highscore)
            {
                HighscoreChanged?.Invoke(_score);
                PlayerPrefs.SetInt("Highscore", _score);
            }
        }

        public GameEndStats GetEndStats(GameEndReason reason)
        {
            if (MainController.Instance.Active)
                throw new Exception("The game is still active");
            GameEndStats stats = new GameEndStats();
            stats.playtime = MainController.Instance.TotalPlaytime;
            stats.reason = reason;
            stats.score = _score;
            return stats;
        }

        public struct GameEndStats
        {
            public GameEndReason reason;
            public float playtime;
            public int score;
        }
    }
}
