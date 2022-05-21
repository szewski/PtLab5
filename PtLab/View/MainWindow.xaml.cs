using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Globalization;
using System.ComponentModel;
using System.Resources;

using PtLab.Resource;
using PtLab.ViewModel;

namespace PtLab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ResourceManager resources = new ResourceManager("PtLab.Resource.Strings", typeof(Strings).Assembly);
        ContextMenu contextmenu = new ContextMenu();
        MenuItem menucreate = new MenuItem();
        MenuItem menudelete = new MenuItem();
        MenuItem menuopen = new MenuItem();
        private FileExplorer _fileExplorer;

        public MainWindow()
        {
            InitializeComponent();
            _fileExplorer = new FileExplorer();
            DataContext = _fileExplorer;
            _fileExplorer.PropertyChanged += _fileExplorer_PropertyChanged;
            treeViewControl.SelectedItemChanged += OnSelectionChange;

            // Create context menu
            menuopen.Header = resources.GetString("Open");
            menuopen.Click += OnMenuItemOpenClick;
            menuopen.IsEnabled = false;

            menucreate.Header = resources.GetString("Create");
            menucreate.Click += OnMenuItemCreateClick;

            menudelete.Header = resources.GetString("Delete");
            menudelete.Click += OnMenuItemDeleteClick;

            contextmenu.Items.Add(menuopen);
            contextmenu.Items.Add(menucreate);
            contextmenu.Items.Add(menudelete);

            _fileExplorer.OnOpenFileRequest += OnOpenFileRequest;
        }

        // Toolbox
        private void OnMenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Context menu
        private void OnOpenFileRequest(object? sender, FileInfoViewModel e)
        {
            var content = _fileExplorer.GetFileContent(e);
            if (content is string text)
            {
                textBoxControl.Text = text;
            }
        }
        private void OnMenuItemOpenClick(object sender, RoutedEventArgs args)
        {
            if (sender is not MenuItem menuItem) return;
            if (treeViewControl.SelectedItem == null) return;

            string nodePath = ((FileSystemInfoViewModel)treeViewControl.SelectedItem).Model.FullName;

            if (!File.Exists(nodePath)) return;

            using (var textReader = File.OpenText(nodePath))
            {
                string text = textReader.ReadToEnd();
                textBoxControl.Text = text;
            }
        }
        
        private void OnMenuItemCreateClick(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem) return;
            if (treeViewControl.SelectedItem == null) return;

            string nodePath = ((FileSystemInfoViewModel)treeViewControl.SelectedItem).Model.FullName;
            string folderPath = Path.GetDirectoryName(nodePath);

            CreateDialog dialog = new CreateDialog();
            dialog.ShowDialog();

            if (dialog.Cancel) return;

            try
            {
                if (dialog.rbDirectory.IsChecked == true)
                {
                    CreateDirectory(folderPath, dialog);
                }
                else if (dialog.rbFile.IsChecked == true)
                {
                    CreateFile(folderPath, dialog);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void OnMenuItemDeleteClick(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem) return;
            if (treeViewControl.SelectedItem == null) return;

            string nodePath = ((FileSystemInfoViewModel)treeViewControl.SelectedItem).Model.FullName;

            FileAttributes attr = File.GetAttributes(nodePath);

            if (attr.HasFlag(FileAttributes.Directory))
                try
                {
                    Directory.Delete(nodePath, true);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return;
                }
            else
                try
                {
                    File.Delete(nodePath);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return;
                }
        }

        private void OnSelectionChange(object sender, RoutedEventArgs e)
        {
            if (sender is not System.Windows.Controls.TreeView treeViewItem) return;
            if (treeViewControl.SelectedItem == null) return;

            string nodePath = ((FileSystemInfoViewModel)treeViewControl.SelectedItem).Model.FullName;

            // Update context menu
            menuopen.IsEnabled = Path.GetExtension(nodePath) == ".txt";

            // Update status bar
            txtStatusBar.Text = "";

            FileAttributes fileAttribute = File.GetAttributes(nodePath);
            string[] rash = { "-", "-", "-", "-"};

            if (fileAttribute.HasFlag(FileAttributes.ReadOnly)) rash[0] = "r";
            if (fileAttribute.HasFlag(FileAttributes.Archive)) rash[1] = "a";
            if (fileAttribute.HasFlag(FileAttributes.System)) rash[2] = "s";
            if (fileAttribute.HasFlag(FileAttributes.Hidden)) rash[3] = "h";

            txtStatusBar.Text = string.Join("", rash);
            e.Handled = true;
        }

        private void CreateDirectory(string directoryPath, CreateDialog dialog)
        {
            string directoryName = dialog.txtName.Text;
            string newDirectoryPath = directoryPath + Path.DirectorySeparatorChar + directoryName;

            Directory.CreateDirectory(newDirectoryPath);
            SetFileAttributes(newDirectoryPath, dialog);
        }
        private void CreateFile(string directoryPath, CreateDialog dialog)
        {
            string fileName = dialog.txtName.Text;
            string newFilePath = directoryPath + Path.DirectorySeparatorChar + fileName;

            if (File.Exists(newFilePath)) return;
            File.Create(newFilePath);
            SetFileAttributes(newFilePath, dialog);
        }
        private void SetFileAttributes(string filePath, CreateDialog dialog)
        {
            if (dialog.AttributeReadOnly)
            {
                File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.ReadOnly);
            }
            if (dialog.AttributeHidden)
            {
                File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);
            }
            if (dialog.AttributeArchive)
            {
                File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Archive);
            }
            if (dialog.AttributeSystem)
            {
                File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.System);
            }
        }

        private void _fileExplorer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FileExplorer.Lang))
                CultureResources.ChangeCulture(CultureInfo.CurrentUICulture);
        }

    }
}
