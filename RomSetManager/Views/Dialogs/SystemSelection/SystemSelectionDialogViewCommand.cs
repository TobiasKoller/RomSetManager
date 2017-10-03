using System.Windows.Controls;

namespace RomSetManager.Views.Dialogs.SystemSelection
{
    public partial class SystemSelectionDialogViewModel
    {
        public void SelectAll(CheckBox sender, object dataContext)
        {
            var selected = sender.IsChecked;

            foreach(var system in Systems)
            {
                system.IsSelected = selected==true;
            }
        }

        public void SelectionChanged(CheckBox sender, Model.System dataContext)
        {
            dataContext.IsSelected = sender.IsChecked == true;
        }

        public void Ok()
        {
            TryClose(true);
        }        
    }
}