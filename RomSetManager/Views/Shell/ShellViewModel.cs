using Caliburn.Micro;
using RomSetManager.IoC;
using RomSetManager.Navigation;
using RomSetManager.Services;

namespace RomSetManager.Views.Shell
{
    public partial class ShellViewModel : ViewModelBase
    {
        public ShellViewModel(string name, SimpleContainerEx container, IEventAggregator eventAggregator, INavigationServiceProvider navigationServiceProvider, string navigationServiceName, IServiceProvider serviceProvider) : 
            base(name, container, eventAggregator, navigationServiceProvider, navigationServiceName, serviceProvider)
        {
        }
    }
}
