using System.ComponentModel;

namespace Model
{
    public class System : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Name { get; set; }
        public bool KeepCompressed { get; set; }
        public bool IsSelected {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

    }
}