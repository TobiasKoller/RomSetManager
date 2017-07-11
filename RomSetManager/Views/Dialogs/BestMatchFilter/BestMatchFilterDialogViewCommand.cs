using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Model;
using Model.Constants;

namespace RomSetManager.Views.Dialogs.BestMatchFilter
{
    public partial class BestMatchFilterDialogViewModel
    {

        public void SelectedPreferenceChanged(DataGrid sender)
        {
            if (sender.SelectedItems == null)
                return;

            var list = (sender.Name == "PreferencesExcludedGrid") ? CurrentSelectedExcludedNameParts : CurrentSelectedIncludedNameParts;

            list.Clear();
            foreach (var senderSelectedItem in sender.SelectedItems)
            {
                if (sender.SelectedItem is NamePart)
                    list.Add((NamePart)senderSelectedItem);
            }
        }

       
        public void PreferenceIncludedUp()
        {
            RepositioningPreference(-1, 1,NamePartsIncluded);
        }

        public void PreferenceIncludedDown()
        {
            RepositioningPreference(1, NamePartsIncluded.Count, NamePartsIncluded);
        }

        public void ExcludeSelected()
        {
            var copy = CurrentSelectedIncludedNameParts.ToList();

            foreach (var currentSelectedNamePart in copy)
            {
                currentSelectedNamePart.Include = IncludeType.No;
                NamePartsExcluded.Add(currentSelectedNamePart);
                NamePartsIncluded.Remove(currentSelectedNamePart);
            }
        }

        public void IncludeSelected()
        {
            var copy = CurrentSelectedExcludedNameParts.ToList();
            var pos = NamePartsIncluded.Max(n => n.Position);

            foreach (var currentSelectedNamePart in copy)
            {
                pos++;
                currentSelectedNamePart.Position = pos;
                currentSelectedNamePart.Include = IncludeType.Yes;
                NamePartsIncluded.Add(currentSelectedNamePart);
                NamePartsExcluded.Remove(currentSelectedNamePart);
                
            }
        }

        public void Ok()
        {
            //updating languages
            var service = ServiceProvider.ConfigurationService;
            var config = service.GetConfiguration();

            var preferences = new Preferences();
            foreach (var namePart in NamePartsIncluded)
                preferences.NameParts.Add(namePart);
            foreach (var namePart in NamePartsExcluded)
                preferences.NameParts.Add(namePart);

            config.BestMatch.Preferences = preferences;
            service.UpdateConfiguration(config);

            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(true);
        }

        private void RepositioningPreference(int step, int max, ObservableCollection<NamePart> namePartList)
        {
            if (CurrentSelectedIncludedNameParts.Count == 0 || CurrentSelectedIncludedNameParts.Count > 1)
                return;

            var current = CurrentSelectedIncludedNameParts.First();

            if ((step > 0) && (current == null || current.Position >= max))
                return;
            if ((step < 0) && (current == null || current.Position < max))
                return;


            var currentPos = current.Position;
            var prevPos = currentPos + step;

            var prevLanguage = namePartList.FirstOrDefault(l => l.Position == prevPos);
            if (prevLanguage == null)
                return;

            prevLanguage.Position = currentPos;
            current.Position = prevPos;

            InitNamePartList(namePartList, namePartList.OrderBy(l => l.Position).ToList());
        }
    }
}