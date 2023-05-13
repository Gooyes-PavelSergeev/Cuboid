using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cubic.UI
{
    public abstract class UIPanel : BasePanel
    {
        [SerializeField] protected Button _targetButton;

        [SerializeField] protected Button _closeButton;

        public override void Initialize()
        {
            if (_disableOnStart) ClosePanel(false);
            SetButtonsOnClick();
        }

        protected virtual void SetButtonsOnClick()
        {
            if (_targetButton == null || _closeButton == null)
                throw new Exception($"Set up UI button script for {gameObject.name}");
            _targetButton.onClick.AddListener(() => ShowPanel());
            _closeButton.onClick.AddListener(() => ClosePanel());
        }
    }
}
