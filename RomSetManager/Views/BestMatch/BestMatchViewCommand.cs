using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using Caliburn.Micro;
using Microsoft.Win32;
using RomSetManager.Views.Dialogs;
using RomSetManager.Views.Dialogs.BestMatchFilter;

namespace RomSetManager.Views.BestMatch
{
    public partial class BestMatchViewModel
    {
        public void EditFilter()
        {
            var dialogViewModel = Container.GetInstance<BestMatchFilterDialogViewModel>();
            
           // bool? success = _dialogService.ShowDialog(this, dialogViewModel);


            IWindowManager manager = new WindowManager();
            manager.ShowDialog(dialogViewModel, null, null);
        }

        public void SelectSourceDirectory()
        {
            var fd = new FolderBrowserDialog();

            if (!string.IsNullOrEmpty(SourceDirectory))
                fd.SelectedPath = SourceDirectory;

            var result = fd.ShowDialog();
            if (result != DialogResult.OK)
                return;

            SourceDirectory = fd.SelectedPath;

        }

        public void SelectDestinationDirectory()
        {
            var fd = new FolderBrowserDialog();
            if (!string.IsNullOrEmpty(DestinationDirectory))
                fd.SelectedPath = DestinationDirectory;

            var result = fd.ShowDialog();
            if (result != DialogResult.OK)
                return;

            DestinationDirectory = fd.SelectedPath;
        }
    }
}