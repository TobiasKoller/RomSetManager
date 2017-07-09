using System.Windows.Controls;
using Caliburn.Micro;

namespace RomSetManager.Navigation
{
    public interface INavigationServiceProvider
    {
        INavigationService GetNavigationService(string key);
        INavigationService Register(string key, INavigationService service, bool overrideRegistration = false);
        INavigationService Register(Frame frame, string frameName = null, bool overrideRegistration = false);
    }
}
