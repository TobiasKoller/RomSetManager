using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Configuration;
using Model;
using RomSetManager.IoC;
using RomSetManager.Navigation;
using RomSetManager.Services;
using RomSetManager.Strings;
using IServiceProvider = RomSetManager.Services.IServiceProvider;

namespace RomSetManager.Views.BestMatch
{
    public partial class BestMatchViewModel : ViewModelBase
    {
        public ObservableCollection<NamePart> FilterLanguages { get; set; }
        public ObservableCollection<RomFile> RomFiles { get; set; }
        private ConfigurationService _configurationService;

        private string _sourceDirectory, _destinationDirectory="";

        public string SourceDirectory{get => _sourceDirectory;
            set
            {
                _sourceDirectory = value;
                NotifyOfPropertyChange(()=>SourceDirectory);
            }
        }
        public string DestinationDirectory
        {
            get => _destinationDirectory;
            set
            {
                _destinationDirectory = value;
                NotifyOfPropertyChange(() => DestinationDirectory);
            }
        }

        public BestMatchViewModel(string name, SimpleContainerEx container, IEventAggregator eventAggregator, INavigationServiceProvider navigationServiceProvider, 
            IServiceProvider serviceProvider) : 
            base(name, container, eventAggregator, navigationServiceProvider, Constants.FRAME_MAIN, serviceProvider)
        {
            FilterLanguages = new ObservableCollection<NamePart>();
            RomFiles = new ObservableCollection<RomFile>();

            _configurationService = ServiceProvider.ConfigurationService;
            Init();
        }

        private void Init()
        {
            var service = ServiceProvider.ConfigurationService;
            var config = service.GetConfiguration();

            SourceDirectory = config.BestMatch.RomSourceDirectory;
            DestinationDirectory = config.BestMatch.RomDestinationDirectory;
        }
    }
}
