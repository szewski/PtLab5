using System;
using PtLab.Enum;

namespace PtLab.ViewModel
{
    public class SortViewModel : ViewModelBase
    {
        private SortBy _sortType;
        private SortOrder _sortOrder;

        public SortBy SortBy
        {
            get { return _sortType; }
            set { 
                _sortType = value;
                NotifyPropertyChanged();
            }
        }

        public SortOrder SortOrder
        {
            get { return _sortOrder; }
            set
            {
                _sortOrder = value;
                NotifyPropertyChanged();
            }
        }
    }
}
