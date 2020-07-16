using System.Threading;
using System.Windows;

namespace DiskSearch.GUI
{
    public partial class App : Application
    {
        private Mutex _instanceMutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            _instanceMutex = new Mutex(true, "DiskSearch.GUI", out var createdNew);
            if (!createdNew)
            {
                _instanceMutex = null;
                Current.Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _instanceMutex?.ReleaseMutex();
            base.OnExit(e);
        }
    }
}