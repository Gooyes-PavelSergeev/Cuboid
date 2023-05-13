namespace Cubic.UI
{
    public abstract class EventPanel : BasePanel
    {
        public override void Initialize()
        {
            if (_disableOnStart) ClosePanel(false);
            Subscribe();
        }
        protected abstract void Subscribe();
    }
}
