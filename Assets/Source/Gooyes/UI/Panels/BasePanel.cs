using System;
using System.Collections;
using UnityEngine;

namespace Cubic.UI
{
    public abstract class BasePanel : MonoBehaviour
    {
        [SerializeField] private UIAnimator _animator;
        [SerializeField] protected bool _disableOnStart = true;
        public event Action<bool> PanelShowEvent;
        public static event Action<bool, BasePanel> ShowEventStatic;
        [NonSerialized] public bool inAnimation;
        public Coroutine lastAnimation;
        public bool IsOpened { get; protected set; }

        public abstract void Initialize();

        public virtual void ShowPanel(bool withAnimation = true)
        {
            IsOpened = true;
            if (withAnimation) lastAnimation = _animator.ShowPanel(this, null);
            else gameObject.SetActive(true);
            InvokeShowEvent(true);
        }

        public virtual void ClosePanel(bool withAnimation = true)
        {
            IsOpened = false;
            if (withAnimation) lastAnimation = _animator.ClosePanel(this, null);
            else gameObject.SetActive(false);
            InvokeShowEvent(false);
        }

        protected void InvokeShowEvent(bool showState)
        {
            PanelShowEvent?.Invoke(showState);
            ShowEventStatic?.Invoke(showState, this);
        }
    }
}
