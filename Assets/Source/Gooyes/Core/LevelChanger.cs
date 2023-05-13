using Gooyes.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cubic
{
    public class LevelChanger : Singleton<LevelChanger>
    {
        private Dictionary<int, string> _levelMap;
        [SerializeField] private int _levelThisObjectOn = 1;
        public int? CurrentLevel { get; private set; } = null;

        private void Start()
        {
            if (_levelMap.ContainsKey(_levelThisObjectOn)) CurrentLevel = _levelThisObjectOn;
            else CurrentLevel = null;
        }

        public void NextLevel(bool goToFirstIfNull = false)
        {
            if (CurrentLevel == null)
            {
                if (goToFirstIfNull) GoToLevel(1);
                return;
            }
            GoToLevel(CurrentLevel.Value + 1);
        }

        public void GoToLevel(int level)
        {
            if (_levelMap == null) return;
            if (_levelMap.TryGetValue(level, out string sceneName))
            {
                bool withDelay = CurrentLevel != null;
                CurrentLevel = level;
                if (withDelay) StartCoroutine(LoadWithDelay(() => SceneManager.LoadScene(sceneName)));
                else SceneManager.LoadScene(sceneName);
            }
            else
            {
                ToMainMenu();
            }
        }

        public void Reload()
        {
            StartCoroutine(LoadWithDelay(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name)));
            /*if (CurrentLevel == null)
            {
                ToMainMenu();
                return;
            }
            GoToLevel(CurrentLevel.Value);*/
        }

        private IEnumerator LoadWithDelay(Action load)
        {
            yield return new WaitForSecondsRealtime(MainController.RELOAD_DELAY);
            load.Invoke();
        }

        public void ToMainMenu()
        {
            CurrentLevel = null;
            SceneManager.LoadScene("StartScene");
        }

#if UNITY_EDITOR
        [SerializeField] private SceneAsset[] _levels;

        private void OnValidate()
        {
            CurrentLevel = null;
            if (_levels == null || _levels.Length == 0) return;
            _levelMap = new Dictionary<int, string>();
            for (int i = 0; i < _levels.Length; i++)
            {
                if (_levelMap.ContainsKey(i + 1)) throw new Exception("There are duplicates in levels");
                _levelMap[i + 1] = _levels[i].name;
            }
        }
#endif
    }
}
