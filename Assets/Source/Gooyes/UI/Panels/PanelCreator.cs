using UnityEngine;
using Gooyes.Tools;
using System.Collections;
using System;

namespace Cubic.UI
{
    public class PanelCreator : Singleton<PanelCreator>
    {
        private BasePanel[] _panels;
        public event Action<bool, BasePanel> PanelShowEvent;
        private void Start()
        {
            StartCoroutine(LateStart());
        }

        private IEnumerator LateStart()
        {
            yield return new WaitForSeconds(MainController.START_DELAY);

            _panels = FindObjectsOfType<BasePanel>(true);
            foreach (BasePanel panel in _panels)
            {
                panel.Initialize();
            }
        }

        private void OnEnable()
        {
            BasePanel.ShowEventStatic += OnPanelShow;
        }

        private void OnDisable()
        {
            BasePanel.ShowEventStatic -= OnPanelShow;
        }

        private void OnPanelShow(bool showState, BasePanel sender)
        {
            PanelShowEvent?.Invoke(showState, sender);
            if (!showState) return;
            foreach (BasePanel panel in _panels)
            {
                if (panel.IsOpened && panel != sender)
                {
                    panel.ClosePanel();
                }
            }
        }
    }
}
