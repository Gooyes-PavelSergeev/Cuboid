using Cubic.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Cubic.UI
{
    [RequireComponent(typeof(Button))]
    internal class SoundButton : MonoBehaviour
    {
        [SerializeField] private SoundType _sound;
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            StartCoroutine(LateStart());
        }

        private IEnumerator LateStart()
        {
            yield return new WaitForSeconds(MainController.START_DELAY);
            _button.onClick.AddListener(() => AudioPlayer.Instance.Play(_sound));
        }
    }
}
