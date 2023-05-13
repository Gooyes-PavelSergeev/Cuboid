using Gooyes.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cubic
{
    public class MainController : Singleton<MainController>
    {
        public bool Active { get; private set; }
        private List<IPausable> _otherPausables;
        public Observable<bool> IsRunning { get; private set; }
        public event Action<GameStats.GameEndStats> GameEndEvent;
        public event Action RestartEvent;
        public bool DEV_MODE;
        public const float END_MIN_DELAY = 1f;
        public const float START_DELAY = 0.3f;
        public const float RELOAD_DELAY = 0.3f;
        public float TotalPlaytime { get; private set; }

        private void Start()
        {
            _otherPausables = new List<IPausable>();
            IsRunning = new Observable<bool>(false);
            Active = false;
            IsRunning.OnChanged += ChangeTimeScale;
            ChangeTimeScale(true);
            StartCoroutine(StartWithDelay(START_DELAY));
        }

        private void Update()
        {
            if (IsRunning.Value)
            {
                TotalPlaytime += Time.deltaTime;
            }
        }

        private void ChangeTimeScale(bool isRunning)
        {
            float scale = isRunning ? 1f : 0f;
            Time.timeScale = scale;
        }

        public void StartGame()
        {
            IsRunning.Value = true;
            Active = true;
            IPausable[] pausables = FindTargets();
            foreach (IPausable pausable in pausables)
            {
                pausable.OnStartByUser();
            }
        }

        public void StopGame()
        {
            IsRunning.Value = false;
            IPausable[] pausables = FindTargets();
            foreach (IPausable pausable in pausables)
            {
                pausable.OnPause();
            }
        }

        public void ContinueGame()
        {
            IsRunning.Value = true;
            IPausable[] pausables = FindTargets();
            foreach (IPausable pausable in pausables)
            {
                pausable.OnContinue();
            }
        }

        public void RestartGame()
        {
            LevelChanger.Instance.Reload();
            StopAllCoroutines();
            TotalPlaytime = 0f;
            IsRunning.Value = true;
            IPausable[] pausables = FindTargets();
            foreach (IPausable pausable in pausables)
            {
                pausable.OnRestart();
            }
            RestartEvent?.Invoke();
        }

        public void EndGame(GameEndReason reason)
        {
            Debug.Log($"End by {reason}");
            StartCoroutine(WaitAndEnd(reason));
        }

        private IPausable[] FindTargets()
        {
            IEnumerable objects = FindObjectsOfType<MonoBehaviour>().OfType<IPausable>();
            List<IPausable> result = objects.Cast<IPausable>().ToList();
            if (_otherPausables.Count != 0) result.AddRange(_otherPausables);
            return result.ToArray();
        }

        private IEnumerator WaitAndEnd(GameEndReason reason)
        {
            yield return new WaitForSecondsRealtime(END_MIN_DELAY);
            IsRunning.Value = false;
            Active = false;
            IPausable[] pausables = FindTargets();
            foreach (IPausable pausable in pausables)
            {
                pausable.OnPause();
            }
            GameEndEvent?.Invoke(GameStats.Instance.GetEndStats(reason));
        }

        private IEnumerator StartWithDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            StartGame();
        }

        internal void AddPausable(IPausable pausable)
        {
            _otherPausables.Add(pausable);
        }
    }

    public enum GameEndReason
    {
        DeadZoneCuboid
    }
}
