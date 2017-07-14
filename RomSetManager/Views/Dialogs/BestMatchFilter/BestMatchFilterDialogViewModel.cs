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
        
        public ObservableCollection<NamePart> FavoriteItems { get; set; }
        public ObservableCollection<NamePart> DontCareItems { get; set; }
        public ObservableCollection<NamePart> MustHavesItems { get; set; }
        public ObservableCollection<NamePart> NeverUseItems { get; set; }

        public List<NamePart> CurrentSelectedFavoriteItems { get; set; }
        public List<NamePart> CurrentSelectedDontCareItems { get; set; }
        public List<NamePart> CurrentSelectedMustHaveItems { get; set; }
        public List<NamePart> CurrentSelectedNeverUseItems { get; set; }



        private int _favoriteSelectedIndex = -1;
        private int _dontCareSelectedIndex = -1;
        private int _mustHavesSelectedIndex = -1;
        private int _neverUseSelectedIndex = -1;
        public int FavoriteSelectedIndex
        {
            get => _favoriteSelectedIndex;
            set
            {
                SetPropertyAndNotify(ref _favoriteSelectedIndex, value, ()=>FavoriteSelectedIndex);
                NotifyOfPropertyChange(() => CanAddSelectedToFavorite); //to enable/disable move-button
            }
        }
        public int DontCareSelectedIndex
        {
            get => _dontCareSelectedIndex;
            set {
                SetPropertyAndNotify(ref _dontCareSelectedIndex, value, () => DontCareSelectedIndex);
                NotifyOfPropertyChange(()=>CanAddSelectedToDontCare); //to enable/disable move-button
            }
        }

        public int MustHavesSelectedIndex
        {
            get => _mustHavesSelectedIndex;
            set
            {
                SetPropertyAndNotify(ref _mustHavesSelectedIndex, value, () => MustHavesSelectedIndex);
                NotifyOfPropertyChange(() => CanAddSelectedToMustHaves); //to enable/disable move-button }
            }
        }

        public int NeverUseSelectedIndex
        {
            get => _neverUseSelectedIndex;
            set
            {
                SetPropertyAndNotify(ref _neverUseSelectedIndex, value, () => NeverUseSelectedIndex);
                NotifyOfPropertyChange(() => CanAddSelectedToNeverUse); //to enable/disable move-button
            }
        }

        private bool _ignoreMustHaveForOneRom;
        public bool IgnoreMustHaveForOneRom
        {
            get => _ignoreMustHaveForOneRom;
            set{SetPropertyAndNotify(ref _ignoreMustHaveForOneRom, value, ()=>IgnoreMustHaveForOneRom);}
        }

        private bool _ignoreNeverUseForOneRom;
        public bool IgnoreNeverUseForOneRom
        {
            get => _ignoreNeverUseForOneRom;
            set{ SetPropertyAndNotify(ref _ignoreNeverUseForOneRom,value,()=>IgnoreNeverUseForOneRom);}
        }


        public BestMatchFilterDialogViewModel(string name, SimpleContainerEx container, IEventAggregator eventAggregator, INavigationServiceProvider navigationServiceProvider, 
            IServiceProvider serviceProvider) 
            : base(name, container, eventAggregator, navigationServiceProvider, Constants.FRAME_MAIN, serviceProvider)
        {
            FavoriteItems = new ObservableCollection<NamePart>();
            DontCareItems = new ObservableCollection<NamePart>();
            MustHavesItems = new ObservableCollection<NamePart>();
            NeverUseItems = new ObservableCollection<NamePart>();
            CurrentSelectedFavoriteItems = new List<NamePart>();
            CurrentSelectedDontCareItems = new List<NamePart>();
            CurrentSelectedMustHaveItems = new List<NamePart>();
            CurrentSelectedNeverUseItems = new List<NamePart>();

            Init();
        }

        private void Init()
        {
            var config = ServiceProvider.ConfigurationService.GetConfiguration();

            var preferences = config.BestMatch.Preferences;
            var nameParts = preferences.NameParts;

            var favoriteItems = nameParts.Where(n => n.Behaviour == BehaviourType.Favorite).OrderBy(l => l.Position).ToList();
            var dontCareItems = nameParts.Where(n => n.Behaviour == BehaviourType.DontCare).OrderBy(l => l.Position).ToList();
            var mustHavesItems = nameParts.Where(n => n.Behaviour == BehaviourType.MustHave).OrderBy(l => l.Position).ToList();
            var neverUseItems = nameParts.Where(n => n.Behaviour == BehaviourType.NeverUse).OrderBy(l => l.Position).ToList();

            IgnoreMustHaveForOneRom = preferences.IgnoreMustHaveForOneRom;
            IgnoreNeverUseForOneRom = preferences.IgnoreNeverUseForOneRom;

            InitNamePartList(FavoriteItems, favoriteItems);
            InitNamePartList(DontCareItems, dontCareItems);
            InitNamePartList(MustHavesItems, mustHavesItems);
            InitNamePartList(NeverUseItems, neverUseItems);
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
