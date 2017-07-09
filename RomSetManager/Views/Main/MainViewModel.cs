using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RomSetManager.IoC;
using RomSetManager.Navigation;
using RomSetManager.Strings;
using IServiceProvider = RomSetManager.Services.IServiceProvider;

namespace RomSetManager.Views.Main
{
    public partial class MainViewModel : ViewModelBase
    {
        public MainViewModel(string name, SimpleContainerEx container, IEventAggregator eventAggregator, INavigationServiceProvider navigationServiceProvider, IServiceProvider serviceProvider) :
            base(name, container, eventAggregator, navigationServiceProvider, null, serviceProvider)
        {
        }
    }
}
