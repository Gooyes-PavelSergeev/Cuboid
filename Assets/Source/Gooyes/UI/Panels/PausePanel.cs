using Cubic.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Cubic.UI
{
    internal class PausePanel : UIPanel
    {
        [SerializeField] private Button _restart;
        [SerializeField] private Button _audio;
        [SerializeField] private GameObject _audioCheck;

        protected override void SetButtonsOnClick()
        {
            base.SetButtonsOnClick();
            _restart.onClick.AddListener(MainController.Instance.RestartGame);
            _audio.onClick.AddListener(() => {
                AudioPlayer.Instance.Mute(!AudioPlayer.Instance.Muted);
                _audioCheck.SetActive(AudioPlayer.Instance.Muted);
            });
        }

        public override void ShowPanel(bool withAnimation = true)
        {
            base.ShowPanel(withAnimation);
            _audioCheck.SetActive(AudioPlayer.Instance.Muted);
            MainController.Instance.StopGame();
        }

        public override void ClosePanel(bool withAnimation = true)
        {
            base.ClosePanel(withAnimation);
            MainController.Instance.ContinueGame();
        }
    }
}
