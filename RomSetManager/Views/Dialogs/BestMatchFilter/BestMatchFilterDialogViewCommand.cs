using System.Linq;
using System.Windows.Controls;
using Model;

namespace RomSetManager.Views.Dialogs.BestMatchFilter
{
    public partial class BestMatchFilterDialogViewModel
    {

        public void SelectedLanguageChanged(DataGrid sender)
        {
            if (sender.SelectedItem == null)
                return;

            CurrentSelectedLanguage = (Language)sender.SelectedItem;
        }

        public void LanguageUp()
        {
            RePositionLangugage(-1, 1);
        }

        public void LanguageDown()
        {
            RePositionLangugage(1, Languages.Count);
        }

        private void RePositionLangugage(int step, int max)
        {
            if ((step > 0) && (CurrentSelectedLanguage == null || CurrentSelectedLanguage.Position >= max))
                return;
            if ((step < 0) && (CurrentSelectedLanguage == null || CurrentSelectedLanguage.Position < max))
                return;


            var currentPos = CurrentSelectedLanguage.Position;
            var prevPos = currentPos + step;

            var prevLanguage = Languages.FirstOrDefault(l => l.Position == prevPos);
            if (prevLanguage == null)
                return;

            prevLanguage.Position = currentPos;
            CurrentSelectedLanguage.Position = prevPos;

            SetLanguageList(Languages.OrderBy(l => l.Position).ToList());
        }
    }
}