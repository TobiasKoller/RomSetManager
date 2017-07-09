using System;
using System.Windows.Controls;
using Caliburn.Micro;

namespace RomSetManager.IoC
{
    public class SimpleContainerEx : SimpleContainer
    {
        /// <summary>
        /// Registers the Caliburn.Micro navigation service with the container.
        /// </summary>
        /// <param name="rootFrame">The application root frame.</param>
        /// <param name="key">Key to store the frame.</param>
        /// <param name="treatViewAsLoaded">if set to <c>true</c> [treat view as loaded].</param>
        /// <param name="cacheViewModels">if set to <c>true</c> then navigation service cache view models for resuse.</param>
        public INavigationService RegisterNavigationService(Frame frame, string key, bool treatViewAsLoaded = false, bool cacheViewModels = false)
        {
            if (HasHandler(typeof(INavigationService), key))
                return this.GetInstance<INavigationService>(key);

            if (frame == null)
                throw new ArgumentNullException("Frame [" + key + "] is null");

            var frameAdapter = new FrameAdapter(frame, treatViewAsLoaded);

            RegisterInstance(typeof(INavigationService), key, frameAdapter);

            return frameAdapter;
        }

        
    }
}
