using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Model;
using Model.Constants;
using RomSetManager.IoC;
using RomSetManager.Navigation;
using RomSetManager.Strings;
using IServiceProvider = RomSetManager.Services.IServiceProvider;

namespace RomSetManager.Views.Dialogs.BestMatchFilter
{
    public partial class BestMatchFilterDialogViewModel : ViewModelBase
    {
        public bool? DialogResult { get; }
        
        public ObservableCollection<NamePart> NamePartsIncluded { get; set; }
        public ObservableCollection<NamePart> NamePartsExcluded { get; set; }

        public List<NamePart> CurrentSelectedIncludedNameParts { get; set; }
        public List<NamePart> CurrentSelectedExcludedNameParts { get; set; }

        public BestMatchFilterDialogViewModel(string name, SimpleContainerEx container, IEventAggregator eventAggregator, INavigationServiceProvider navigationServiceProvider, 
            IServiceProvider serviceProvider) 
            : base(name, container, eventAggregator, navigationServiceProvider, Constants.FRAME_MAIN, serviceProvider)
        {
            NamePartsIncluded = new ObservableCollection<NamePart>();
            NamePartsExcluded = new ObservableCollection<NamePart>();
            CurrentSelectedIncludedNameParts = new List<NamePart>();
            CurrentSelectedExcludedNameParts = new List<NamePart>();

            Init();
        }

        private void Init()
        {
            var config = ServiceProvider.ConfigurationService.GetConfiguration();
            
            var includedPreferences = config.BestMatch.Preferences.NameParts.Where(n => n.Include == IncludeType.Yes).OrderBy(l => l.Position).ToList();
            var excludedPreferences = config.BestMatch.Preferences.NameParts.Where(n => n.Include == IncludeType.No).OrderBy(l => l.Position).ToList();

            InitNamePartList(NamePartsIncluded, includedPreferences);
            InitNamePartList(NamePartsExcluded, excludedPreferences);


            //NamePartsIncluded.Clear();
            //NamePartsExcluded.Clear();

            //foreach (var preference in includedPreferences)
            //    NamePartsIncluded.Add(preference);

            //foreach (var preference in excludedPreferences)
            //    NamePartsExcluded.Add(preference);
        }

        private void InitNamePartList(ObservableCollection<NamePart> sourceList, List<NamePart> namePartList)
        {
            sourceList.Clear();
            var counter = 1;
            foreach (var preference in namePartList)
            {
                preference.Position = counter;
                sourceList.Add(preference);
                counter++;
            }
        }
    }
}
