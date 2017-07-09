using System.Collections.Generic;
using System.Windows.Controls;
using Caliburn.Micro;

namespace RomSetManager.Navigation
{

    public class NavigationServiceProvider : INavigationServiceProvider
    {
        private Dictionary<string, INavigationService> _services;

        public NavigationServiceProvider()
        {
            _services = new Dictionary<string, INavigationService>();
        }

        public INavigationService GetNavigationService(string key)
        {
            if (!_services.ContainsKey(key))
                throw new KeyNotFoundException($"No NavigationService registered with key [{key}]");

            return _services[key];
        }

        public INavigationService Register(Frame frame, string frameName = null, bool overrideRegistration = false)
        {
            var navigationService = new FrameAdapter(frame);

            if (string.IsNullOrWhiteSpace(frameName))
                frameName = frame.Name;

            return Register(frameName, navigationService, overrideRegistration);
        }

        public INavigationService Register(string key, INavigationService service, bool overrideRegistration = false)
        {
            if (!_services.ContainsKey(key))
            {
                _services.Add(key, service);
            }
            else if (overrideRegistration)
            {
                _services[key] = service;
            }

            return _services[key];
        }
    }
}
