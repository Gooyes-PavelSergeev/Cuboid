using Gooyes.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubic.UI
{
    [CreateAssetMenu(fileName = "Panel animator")]
    internal class UIAnimator : ScriptableObject
    {
        [SerializeField] private AnimationCurve _xAxisScaling_IN;
        [SerializeField] private AnimationCurve _yAxisScaling_IN;
        [SerializeField] private AnimationCurve _xAxisScaling_OUT;
        [SerializeField] private AnimationCurve _yAxisScaling_OUT;
        [SerializeField] private bool _xIsPriority_IN;
        [SerializeField] private bool _xIsPriority_OUT;
        private Dictionary<RectTransform, Coroutine> _coroutinesMap;

        private void Awake()
        {
            _coroutinesMap = new Dictionary<RectTransform, Coroutine>();
        }

        public Coroutine ShowPanel(BasePanel panel, Action callback)
        {
            if (panel.inAnimation) Coroutines.StopCoroutine(panel.lastAnimation);
            return Coroutines.StartCoroutine(SmoothDampPanel(panel, true, callback));
        }

        public Coroutine ClosePanel(BasePanel panel, Action callback)
        {
            if (panel.inAnimation) Coroutines.StopCoroutine(panel.lastAnimation);
            return Coroutines.StartCoroutine(SmoothDampPanel(panel, false, callback));
        }

        public void PulseElement(RectTransform element, float time, float power, Action callback =  null)
        {
            if (element == null || element.gameObject == null) Debug.LogWarning("Element is null");
            if (_coroutinesMap.TryGetValue(element, out Coroutine coroutine))
            {
                if (coroutine != null)
                {
                    Coroutines.StopCoroutine(coroutine);
                    coroutine = null;
                }
            }
            _coroutinesMap[element] = Coroutines.StartCoroutine(PulseCoroutine(element, time, power, callback));
        }

        private IEnumerator SmoothDampPanel(BasePanel panel, bool inTrans, Action callback)
        {
            if (inTrans) panel.gameObject.SetActive(true);
            panel.inAnimation = true;
            bool xPriority = inTrans ? _xIsPriority_IN : _xIsPriority_OUT;
            AnimationCurve xCurve = inTrans ? _xAxisScaling_IN : _xAxisScaling_OUT;
            AnimationCurve yCurve = inTrans ? _yAxisScaling_IN : _yAxisScaling_OUT;
            float timer = 0f;
            float maxTime = xPriority ? xCurve.keys[^1].time : yCurve.keys[^1].time;
            while (timer < maxTime)
            {
                float x = xCurve.Evaluate(timer);
                float y = yCurve.Evaluate(timer);
                panel.transform.localScale = new Vector3(x, y, 1);
                yield return null;
                timer += Time.unscaledDeltaTime;
            }
            panel.transform.localScale = inTrans ? Vector3.one : Vector3.zero;
            panel.inAnimation = false;
            if (!inTrans) panel.gameObject.SetActive(false);
            if (callback != null) callback.Invoke();
        }

        private IEnumerator PulseCoroutine(RectTransform element, float time, float power, Action callback)
        {
            float timer = 0;
            while (timer < time)
            {
                float scale = 4 * timer * (power - 1) * (time - timer) / (time * time) + 1;
                if (element != null && element.gameObject != null) element.localScale = Vector3.one * scale;
                yield return null;
                timer += Time.unscaledDeltaTime;
            }
            if (element != null && element.gameObject != null) element.localScale = Vector3.one;
            if (callback != null) callback.Invoke();
        }
    }
}
