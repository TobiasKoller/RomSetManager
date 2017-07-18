using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using RomSetManager.IoC;
using RomSetManager.Navigation;
using IServiceProvider = RomSetManager.Services.IServiceProvider;

namespace RomSetManager.Views
{
    public abstract class LoadableViewModelBase : ViewModelBase
    {
        private Visibility _loadScreenVisibility;
        public Visibility LoadScreenVisibility
        {
            get => _loadScreenVisibility;
            set { SetPropertyAndNotify(ref _loadScreenVisibility, value, ()=> LoadScreenVisibility);}
        }

        private string _loadScreenText;
        public string LoadScreenText
        {
            get => _loadScreenText;
            set { SetPropertyAndNotify(ref _loadScreenText, value, () => LoadScreenText); }
        }
        private int _loadScreenProgress;
        public int LoadScreenProgress
        {
            get => _loadScreenProgress;
            set
            {
                SetPropertyAndNotify(ref _loadScreenProgress, value, () => LoadScreenProgress);
            }
        }

        protected LoadableViewModelBase(string name, SimpleContainerEx container, IEventAggregator eventAggregator, INavigationServiceProvider navigationServiceProvider, 
            string navigationServiceName, IServiceProvider serviceProvider) 
            : base(name, container, eventAggregator, navigationServiceProvider, navigationServiceName, serviceProvider)
        {
            LoadScreenVisibility = Visibility.Collapsed;
            LoadScreenText = string.Empty;
            LoadScreenProgress = 0;
        }

        public void ShowLoadScreen(string message="")
        {
            LoadScreenVisibility = Visibility.Visible;
            LoadScreenText = message;
            LoadScreenProgress = 0;
        }
        public void HideLoadScreen()
        {
            LoadScreenVisibility = Visibility.Collapsed;
            LoadScreenText = string.Empty;
            LoadScreenProgress = 0;
        }
    }
    
}