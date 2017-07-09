using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using RomSetManager.IoC;
using RomSetManager.Navigation;
using RomSetManager.Services;
using RomSetManager.Strings;
using RomSetManager.Views.Dialogs;
using RomSetManager.Views.Shell;
using IServiceProvider = RomSetManager.Services.IServiceProvider;

namespace RomSetManager.Views
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainerEx container;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            container = new SimpleContainerEx(); //new WinRTContainer();
            container.RegisterInstance(typeof(SimpleContainerEx), null, container);
            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();
            //container.PerRequest<ShellViewModel>();
            SetViewModelConvension(container);
            
            var provider = new NavigationServiceProvider();
            container.RegisterInstance(typeof(INavigationServiceProvider), null, provider);
            
            var serviceProvider = new ServiceProvider();
            container.RegisterInstance(typeof(IServiceProvider), null, serviceProvider);
        }

        private void SetViewModelConvension(SimpleContainerEx container)
        {
            //ViewLocator.ConfigureTypeMappings()

            //ViewLocator.AddNamespaceMapping("MyProject.ViewModels.Customers", "MyClient1.Views");

            var types = GetType().Assembly.GetTypes();

            foreach (var type in types.Where(t => t.FullName.EndsWith("View")))
            {
                var fullName = type.FullName;
                var viewModelName = string.Format("{0}Model", fullName);

                var viewModelType = types.FirstOrDefault(t => t.FullName == viewModelName);
                if (viewModelType == null)
                    throw new TypeLoadException("Couldn't find ViewModel named any like " + string.Join(",", viewModelName));

                container.RegisterPerRequest(viewModelType, null, viewModelType); //RegisterViewModel

                //ViewLocator.AddNamespaceMapping(viewModelType.Namespace.ToString(), type.Namespace.ToString());
            }
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
    }

}
