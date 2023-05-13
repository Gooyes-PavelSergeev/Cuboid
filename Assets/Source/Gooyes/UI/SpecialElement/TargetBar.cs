using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Cubic.UI
{
    internal class TargetBar : ProgressBar
    {
        [SerializeField, Range(0f, 1f)] private float _targetValue;
        [SerializeField] private Image _targetZone;
        [SerializeField] private RectTransform _arrows;
        [SerializeField] private RectTransform _targetBorder;
        [SerializeField] private RectTransform _targetParent;
        [SerializeField] private RectTransform _referenceBorder;
        public float TargetValue { get { return _targetValue; } set { SetTargetValue(value); } }

        private void SetTargetValue(float value)
        {
            value = Mathf.Clamp01(value);
            _targetValue = value;
            float min = _progressBarMinWidth;
            float max = _progressBarMaxWidth;
            float newWidth = max * (1 - value) + min * (1 + value);
            _targetZone.rectTransform.sizeDelta = new Vector2(newWidth, _targetZone.rectTransform.sizeDelta.y);
            float newPosX = (_progressBarMaxWidth - newWidth) / 2;
            _targetZone.transform.localPosition = new Vector3(newPosX, 0, 0);
            _arrows.localPosition = new Vector3((value - 0.5f) * (max - min), 0, 0);
            if (Application.isPlaying)
            {
                StartCoroutine(SetTarget());
            }
            else
            {
                _targetBorder.position = _referenceBorder.position;
                _targetParent.sizeDelta = _progressBarFG.rectTransform.sizeDelta;
            }
        }

        private IEnumerator SetTarget()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            _targetBorder.position = _referenceBorder.position;
            _targetParent.sizeDelta = _progressBarFG.rectTransform.sizeDelta;
        }

        protected override void OnValidate()
        {
            if (Application.isPlaying) return;
            base.OnValidate();
            SetTargetValue(_targetValue);
        }
    }
}
