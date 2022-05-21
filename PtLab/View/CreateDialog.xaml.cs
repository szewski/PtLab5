using System;
using System.Windows;
using System.Collections.Generic;

namespace PtLab
{
    /// <summary>
    /// Interaction logic for CreateDialog.xaml
    /// </summary>
    public partial class CreateDialog : Window
    {
        public bool Cancel;
        public bool AttributeReadOnly;
        public bool AttributeArchive;
        public bool AttributeHidden;
        public bool AttributeSystem;

        public CreateDialog()
        {
            InitializeComponent();
        }

        private void OnOkButton_Click(object sender, RoutedEventArgs e)
        {
            Cancel = false;

            if (cboReadOnly.IsChecked is not null) AttributeReadOnly = (bool)cboReadOnly.IsChecked;
            if (cboArchive.IsChecked is not null) AttributeArchive = (bool)cboArchive.IsChecked;
            if (cboHidden.IsChecked is not null) AttributeHidden = (bool)cboHidden.IsChecked;
            if (cboSystem.IsChecked is not null) AttributeSystem = (bool)cboSystem.IsChecked;

            Close();
        }

        private void OnCancelButton_Click_1(object sender, RoutedEventArgs e)
        {
            Cancel = true;
            Close();
        }
    }
}
