using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using PtLab.ViewModel;

namespace PtLab.View
{
    /// <summary>
    /// Interaction logic for SortDialog.xaml
    /// </summary>
    public partial class SortDialog : Window
    {
        private SortViewModel _sort;
        public SortViewModel Sort { get { return _sort; } }

        public SortDialog(SortViewModel sort)
        {
            InitializeComponent();
            this._sort = sort;
            DataContext = this._sort;
        }


    }
}
