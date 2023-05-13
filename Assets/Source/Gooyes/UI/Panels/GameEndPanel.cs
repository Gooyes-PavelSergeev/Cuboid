using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cubic.UI
{
    public class GameEndPanel : EventPanel
    {
        [Header("Buttons")]
        [SerializeField] private Button _restartButton;
        [SerializeField] private GameObject _pause;
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private TextMeshProUGUI _highscore;
        [SerializeField] private GameObject _HUDscore;
        [SerializeField] private GameObject _HUDhighscore;

        protected override void Subscribe()
        {
            MainController.Instance.GameEndEvent += OnGameEnd;
            //_restartButton.onClick.AddListener(MainController.Instance.RestartGame);
            _restartButton.onClick.AddListener(MainController.Instance.RestartGame);
            _restartButton.onClick.AddListener(() => ClosePanel());
        }

        public override void ShowPanel(bool withAnimation = true)
        {
            base.ShowPanel(withAnimation);
            _HUDscore.SetActive(false);
            _HUDhighscore.SetActive(false);
            _pause.SetActive(false);
        }

        public override void ClosePanel(bool withAnimation = true)
        {
            base.ClosePanel(withAnimation);
            _HUDscore.SetActive(true);
            _HUDhighscore.SetActive(true);
            _pause.SetActive(true);
        }

        private void OnGameEnd(GameStats.GameEndStats stats)
        {
            ShowPanel();
            _score.text = stats.score.ToString();
            _highscore.text = PlayerPrefs.GetInt("Highscore", stats.score).ToString();
        }
    }
}
