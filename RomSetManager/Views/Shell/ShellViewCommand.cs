using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using RomSetManager.Strings;
using RomSetManager.Views.BestMatch;
using RomSetManager.Views.Main;

namespace RomSetManager.Views.Shell
{
    public partial class ShellViewModel
    {
        public void Init(Frame frame)
        {
            var service = NavigationServiceProvider.Register(frame, Constants.FRAME_ROOT);
            service.NavigateToViewModel<MainViewModel>();
        }

    }
}