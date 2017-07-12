using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Model;
using Model.Constants;

namespace RomSetManager.Views.Dialogs.BestMatchFilter
{
    public partial class BestMatchFilterDialogViewModel
    {

        public void GridSelectionChanged(DataGrid sender, BehaviourType type)
        {
            if (sender.SelectedItems == null)
                return;

            List<NamePart> list;
            switch (type)
            {
                case BehaviourType.Favorite:
                    if (FavoriteSelectedIndex == -1)
                        return;
                    list = CurrentSelectedFavoriteItems;
                    break;
                case BehaviourType.DontCare:
                    if (DontCareSelectedIndex == -1)
                        return;
                    list = CurrentSelectedDontCareItems;
                    break;
                case BehaviourType.MustHave:
                    if (MustHavesSelectedIndex == -1)
                        return;
                    list = CurrentSelectedMustHaveItems;
                    break;
                case BehaviourType.NeverUse:
                    if (NeverUseSelectedIndex == -1)
                        return;
                    list = CurrentSelectedNeverUseItems;
                    break;

                default:
                    return;
            }

            if (type != BehaviourType.DontCare)
            {
                CurrentSelectedDontCareItems.Clear();
                DontCareSelectedIndex = -1;
            }
            if (type != BehaviourType.Favorite)
            {
                CurrentSelectedFavoriteItems.Clear();
                FavoriteSelectedIndex = -1;
            }
            if (type != BehaviourType.MustHave)
            {
                CurrentSelectedMustHaveItems.Clear();
                MustHavesSelectedIndex = -1;
            }
            if (type != BehaviourType.NeverUse)
            {
                CurrentSelectedNeverUseItems.Clear();
                NeverUseSelectedIndex = -1;
            }

            //var list = (sender.Name == "PreferencesExcludedGrid") ? CurrentSelectedDontCareItems : CurrentSelectedFavoriteItems;

            list.Clear();
            foreach (var senderSelectedItem in sender.SelectedItems)
            {
                if (sender.SelectedItem is NamePart)
                    list.Add((NamePart)senderSelectedItem);
            }
        }

       
        public void FavoriteItemUp()
        {
            RepositioningPreference(-1, 1,FavoriteItems);
        }

        public void FavoriteItemDown()
        {
            RepositioningPreference(1, FavoriteItems.Count, FavoriteItems);
        }

        private BehaviourType GetSelectionSource()
        {
            if (FavoriteSelectedIndex != -1)
                return BehaviourType.Favorite;
            if (DontCareSelectedIndex != -1)
                return BehaviourType.DontCare;
            if (MustHavesSelectedIndex != -1)
                return BehaviourType.MustHave;
            if (NeverUseSelectedIndex != -1)
                return BehaviourType.NeverUse;

            return BehaviourType.Favorite;
        }

        //private List<NamePart> GetSelection()
        //{
        //    var type = GetSelectionSource();
        //    if (FavoriteSelectedIndex != -1)
        //        return CurrentSelectedFavoriteItems.ToList();
        //    if (DontCareSelectedIndex != -1)
        //        return CurrentSelectedDontCareItems.ToList();
        //    if (MustHavesSelectedIndex != -1)
        //        return CurrentSelectedMustHaveItems.ToList();
        //    if (NeverUseSelectedIndex != -1)
        //        return CurrentSelectedNeverUseItems.ToList();

        //    return new List<NamePart>();
        //}

        private void MoveSelection(BehaviourType destinationType)
        {
            var sourceType = GetSelectionSource();

            ObservableCollection<NamePart> sourceList;
            ObservableCollection<NamePart> destList;
            List<NamePart> selectionList;

            switch (sourceType)
            {
                    case BehaviourType.Favorite:
                        sourceList = FavoriteItems;
                        selectionList = CurrentSelectedFavoriteItems;
                        break;
                    case BehaviourType.DontCare:
                        sourceList = DontCareItems;
                        selectionList = CurrentSelectedDontCareItems;
                    break;
                    case BehaviourType.MustHave:
                        sourceList = MustHavesItems;
                        selectionList = CurrentSelectedMustHaveItems;
                    break;

                    case BehaviourType.NeverUse:
                        sourceList = NeverUseItems;
                        selectionList = CurrentSelectedNeverUseItems;
                    break;
                default:
                    return;
            }

            switch (destinationType)
            {
                case BehaviourType.Favorite:
                    destList = FavoriteItems;
                    break;
                case BehaviourType.DontCare:
                    destList = DontCareItems;
                    break;
                case BehaviourType.MustHave:
                    destList = MustHavesItems;
                    break;

                case BehaviourType.NeverUse:
                    destList = NeverUseItems;
                    break;
                default:
                    return;
            }

            var list = selectionList.ToList();
            foreach (var namePart in list)
            {
                namePart.Behaviour = destinationType;

                if (destinationType == BehaviourType.Favorite)
                {
                    //need to reset position
                    namePart.Position = FavoriteItems.Count+1;
                }

                destList.Add(namePart);
                sourceList.Remove(namePart);
            }
        }

        public void AddSelectedToDontCare()
        {
            MoveSelection(BehaviourType.DontCare);
            

            //var copy = CurrentSelectedFavoriteItems.ToList();

            //foreach (var currentSelectedNamePart in copy)
            //{
            //    currentSelectedNamePart.Behaviour = BehaviourType.DontCare;
            //    DontCareItems.Add(currentSelectedNamePart);
            //    FavoriteItems.Remove(currentSelectedNamePart);
            //}
        }

        public void AddSelectedToFavorite()
        {
            MoveSelection(BehaviourType.Favorite);
            //var copy = CurrentSelectedDontCareItems.ToList();
            //var pos = FavoriteItems.Max(n => n.Position);

            //foreach (var currentSelectedNamePart in copy)
            //{
            //    pos++;
            //    currentSelectedNamePart.Position = pos;
            //    currentSelectedNamePart.Behaviour = BehaviourType.Favorite;
            //    FavoriteItems.Add(currentSelectedNamePart);
            //    DontCareItems.Remove(currentSelectedNamePart);

            //}
        }

        public void AddSelectedToMustHaves()
        {
            MoveSelection(BehaviourType.MustHave);
        }

        public void AddSelectedToNeverUse()
        {
            MoveSelection(BehaviourType.NeverUse);
        }

        public void Ok()
        {
            //updating languages
            var service = ServiceProvider.ConfigurationService;
            var config = service.GetConfiguration();

            var preferences = new Preferences();
            foreach (var namePart in FavoriteItems)
                preferences.NameParts.Add(namePart);
            foreach (var namePart in DontCareItems)
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
            //if (CurrentSelectedFavoriteItems.Count == 0 || CurrentSelectedFavoriteItems.Count > 1)
            //    return;

            if (FavoriteSelectedIndex == -1)
                return;

            var current = CurrentSelectedFavoriteItems.First();

            if ((step > 0) && (current == null || current.Position >= max))
                return;
            if ((step < 0) && (current == null || current.Position < max))
                return;


            var currentPos = current.Position;
            var nextPos = currentPos + step;

            var prevNamePart = namePartList.FirstOrDefault(l => l.Position == nextPos);
            if (prevNamePart == null)
                return;

            prevNamePart.Position = currentPos;
            current.Position = nextPos;


            InitNamePartList(namePartList, namePartList.OrderBy(l => l.Position).ToList());
            
            FavoriteSelectedIndex = nextPos-1;
        }
    }
}