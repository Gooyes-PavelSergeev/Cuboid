using System.Collections;
using TMPro;
using UnityEngine;

namespace Cubic.UI
{
    public class OnTopInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private TextMeshProUGUI _highScore;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(MainController.START_DELAY);
            GameStats.Instance.ScoreChanged += OnScoreChanged;
            GameStats.Instance.HighscoreChanged += OnHighscoreChanged;
            _score.text = GameStats.Instance.Score.ToString();
            _highScore.text = GameStats.Instance.Highscore.ToString();
        }

        private void OnHighscoreChanged(int highscore)
        {
            _highScore.text = highscore.ToString();
        }

        private void OnScoreChanged(int score)
        {
            _score.text = score.ToString();
        }
    }
}
