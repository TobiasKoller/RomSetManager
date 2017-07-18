using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using RomSetManager.Strings;
using RomSetManager.Views.BestMatch;

namespace RomSetManager.Views.Main
{
    public partial class MainViewModel
    {
        public void NavigateToBestMatchView()
        {
            NavigationService.NavigateToViewModel<BestMatchViewModel>();
        }

        public void Init(Frame frame)
        {
            NavigationService = NavigationServiceProvider.Register(frame, Constants.FRAME_MAIN);
            NavigationService.NavigateToViewModel<BestMatchViewModel>();
        }
    }
}
