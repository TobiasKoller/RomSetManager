using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using Model;
using RomSetManager.IoC;
using RomSetManager.Navigation;
using RomSetManager.Strings;
using IServiceProvider = RomSetManager.Services.IServiceProvider;

namespace RomSetManager.Views.Dialogs.BestMatchFilter
{
    public partial class BestMatchFilterDialogViewModel : ViewModelBase
    {
        public bool? DialogResult { get; }

        public ObservableCollection<Language> Languages { get; set; }
       
        public Language CurrentSelectedLanguage { get; set; }

        public BestMatchFilterDialogViewModel(string name, SimpleContainerEx container, IEventAggregator eventAggregator, INavigationServiceProvider navigationServiceProvider, 
            IServiceProvider serviceProvider) 
            : base(name, container, eventAggregator, navigationServiceProvider, Constants.FRAME_MAIN, serviceProvider)
        {
            Languages = new ObservableCollection<Language>();

            Init();
        }

        private void Init()
        {
            var config = ServiceProvider.ConfigurationService.GetConfiguration();

            SetLanguageList(config.BestMatch.LanguageList);
        }

        private void SetLanguageList(List<Language> languages)
        {
            Languages.Clear();

            foreach (var language in languages)
            {
                Languages.Add(language);
            }
        }

    }
}
