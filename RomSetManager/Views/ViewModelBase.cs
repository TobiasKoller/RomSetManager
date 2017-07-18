using System;
using System.Linq.Expressions;
using System.Windows.Controls;
using Caliburn.Micro;
using RomSetManager.IoC;
using RomSetManager.Messages;
using RomSetManager.Navigation;

namespace RomSetManager.Views
{
    public abstract class ViewModelBase : Screen, IHandle<ResumeStateMessage>//, IHandle<SuspendStateMessage>
    {
        protected SimpleContainerEx Container;
        private bool _resume;
        protected IEventAggregator EventAggregator;
        protected INavigationServiceProvider NavigationServiceProvider;
        protected INavigationService NavigationService;
        protected Services.IServiceProvider ServiceProvider;
        public string Name { get; set; }

        protected ViewModelBase(string name, SimpleContainerEx container, IEventAggregator eventAggregator, INavigationServiceProvider navigationServiceProvider,
            string navigationServiceName, Services.IServiceProvider serviceProvider)
        {
            Name = name;
            Container = container;
            EventAggregator = eventAggregator;
            NavigationServiceProvider = navigationServiceProvider;
            ServiceProvider = serviceProvider;

            if (!string.IsNullOrEmpty(navigationServiceName))
                InitNavigationService(navigationServiceName);
        }

        protected void InitNavigationService(string navigationServiceName)
        {
            NavigationService = NavigationServiceProvider.GetNavigationService(navigationServiceName);
        }

        protected override void OnActivate()
        {
            EventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            EventAggregator.Unsubscribe(this);
        }

        //public void Handle(SuspendStateMessage message)
        //{
        //    //NavigationService.SuspendState();

        //}

        public void Handle(ResumeStateMessage message)
        {
            _resume = true;
        }

        public void RegisterFrame(Frame frame)
        {
            var provider = Container.GetInstance<INavigationServiceProvider>();
            NavigationService = provider.Register(frame);

            //if (_resume)
            //    NavigationService.ResumeState();
        }

        public void SetPropertyAndNotify<TProperty>(ref TProperty storage, TProperty value, Expression<Func<TProperty>> property)
        {
            if (Equals(storage, value))
                return;

            storage = value;
            NotifyOfPropertyChange(property.GetMemberInfo().Name);
        }
    }
}
