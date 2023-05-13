namespace Cubic
{
    internal interface IPausable
    {
        void OnStartByUser();
        void OnPause();
        void OnContinue();
        void OnRestart();
    }
}
