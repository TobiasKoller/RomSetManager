using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using Configuration;
using Model;
using RomSetManager.IoC;
using RomSetManager.Navigation;
using RomSetManager.Services;
using RomSetManager.Strings;
using RomSetManager.Worker;
using IServiceProvider = RomSetManager.Services.IServiceProvider;

namespace RomSetManager.Views.BestMatch
{
    public partial class BestMatchViewModel : ViewModelBase
    {
        public ObservableCollection<NamePart> FilterLanguages { get; set; }
        public ObservableCollection<RomFile> RomFiles { get; set; }
        private ConfigurationService _configurationService;
        private ObservableCollection<ICollectionView> GroupedRomFiles { get; set; }
        private RomFileWorker _romFileWorker;

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
            set{SetPropertyAndNotify(ref _destinationDirectory, value,()=>DestinationDirectory);}
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

            _romFileWorker = new RomFileWorker(config);
            SourceDirectory = config.BestMatch.RomSourceDirectory;
            DestinationDirectory = config.BestMatch.RomDestinationDirectory;
        }
    }
}
