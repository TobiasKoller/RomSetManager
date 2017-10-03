using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RomSetManager.IoC;
using RomSetManager.Navigation;
using RomSetManager.Services;
using Model;
using System.IO;

namespace RomSetManager.Views.Dialogs.SystemSelection
{
    public partial class SystemSelectionDialogViewModel : ViewModelBase
    {
        public ObservableCollection<Model.System> Systems { get; set; }

        public SystemSelectionDialogViewModel(string name, SimpleContainerEx container, IEventAggregator eventAggregator, 
            INavigationServiceProvider navigationServiceProvider, string navigationServiceName, Services.IServiceProvider serviceProvider) : 
            base(name, container, eventAggregator, navigationServiceProvider, navigationServiceName, serviceProvider)
        {
            Systems = new ObservableCollection<Model.System>();
            
        }

        public void SetSystems(List<Model.System> systems)
        {
           
            foreach(var system in systems)
            {
                Systems.Add(system);
            }
        }

        public void SetSystemSelection(List<string> systems)
        {
            //SelectedSystems.Clear();

            //foreach (var system in systems)
            //{
            //    SelectedSystems.Add(system);
            //}
        }

        public List<string> GetSystemSelection()
        {
            return Systems.Where(s => s.IsSelected).Select(x => x.Name).ToList();
            //return SelectedSystems.ToList();
        }
    }
}
