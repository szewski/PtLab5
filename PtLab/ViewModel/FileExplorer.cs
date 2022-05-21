using System;
using System.Windows.Input;
using System.Windows.Forms;
using System.Globalization;
using PtLab.View;
using PtLab.Resource;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PtLab.ViewModel
{
    public class FileExplorer : ViewModelBase
    {
        public static readonly string[] TextFilesExtensions = new string[] { ".txt", ".ini", ".log" };

        public ICommand OpenRootFolderCommand { get; private set; }
        public ICommand SortRootCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }

        public event EventHandler<FileInfoViewModel> OnOpenFileRequest;

        private DirectoryInfoViewModel _root;
        private SortViewModel _sort;
        private string _statusMessage;

        public FileExplorer()
        {
            NotifyPropertyChanged(nameof(Lang));

            _root = new DirectoryInfoViewModel(this);
            _root.PropertyChanged += Root_PropertyChanged;
            NotifyPropertyChanged(nameof(Root));
            
            _sort = new SortViewModel();
            _sort.SortBy = Enum.SortBy.Alphabetically;
            _sort.SortOrder = Enum.SortOrder.Ascending;
            NotifyPropertyChanged(nameof(Sort));
            Sort.PropertyChanged += Sort_PropertyChangedAsync;

            OpenRootFolderCommand = new RelayCommand(OpenRootFolderExecuteAsync);
            SortRootCommand = new RelayCommand(SortRootExecute, CanExecuteSort);
            OpenFileCommand = new RelayCommand(OpenFileExecute, CanExecuteOpenFile);
        }

        public object GetFileContent(FileInfoViewModel viewModel)
        {
            var extension = viewModel.Extension?.ToLower();
            if (TextFilesExtensions.Contains(extension))
            {
                return GetTextFileContent(viewModel);
            }
            return null;
        }

        private string GetTextFileContent(FileInfoViewModel viewModel)
        {
            string result = "";

            using (var textReader = File.OpenText(viewModel.Model.FullName))
            {
                result = textReader.ReadToEnd();
            }

            return result;
        }

        private bool CanExecuteOpenFile(object parameter)
        {
            if (parameter is FileInfoViewModel viewModel)
            {
                var extension = viewModel.Extension?.ToLower();
                return TextFilesExtensions.Contains(extension);
            }
            return false;
        }

        private void OpenFileExecute(object obj)
        {
            if (obj is not FileInfoViewModel) return;
            FileInfoViewModel viewModel = (FileInfoViewModel)obj;
            OnOpenFileRequest.Invoke(this, viewModel);
        }

        private async void Sort_PropertyChangedAsync(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                Root.Sort(Sort);
            });
        }

        public SortViewModel Sort
        {
            get { return _sort; }
            set 
            { 
                if (value != null) _sort = value; 
                NotifyPropertyChanged(); 
            }
        }

        private void SortRootExecute(object parameter)
        {
            var dlg = new SortDialog(Sort);
            dlg.ShowDialog();
        }

        private bool CanExecuteSort(object parameter)
        {
            return _root.IsInitialized;
        }

        private async void OpenRootFolderExecuteAsync(object parameter)
        {
            var dlg = new FolderBrowserDialog() { Description = Strings.Open_dir_description, UseDescriptionForTitle = true };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await Task.Factory.StartNew(() =>
                {
                    StatusMessage = Strings.Loading;
                    string path = dlg.SelectedPath;
                    bool result = Root.Open(path);
                    if (result) StatusMessage = Strings.Ready;
                });
            }
        }

        private void OpenRootFolderExecute(object parameter)
        {
            var dlg = new FolderBrowserDialog() { Description = Strings.Open_dir_description };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dlg.SelectedPath;
                OpenRoot(path);
            }
        }

        public string Lang
        {
            get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName; }
            set
            {
                if (value != null)
                    if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != value)
                    {
                        CultureInfo.CurrentUICulture = new CultureInfo(value);
                        NotifyPropertyChanged();
                    }
            }
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (value != null && value != _statusMessage)
                {
                    _statusMessage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void Root_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "StatusMessage" && sender is FileSystemInfoViewModel viewModel)
                this.StatusMessage = viewModel.StatusMessage;
        }

        public DirectoryInfoViewModel Root
        {
            get { return _root; }
            set
            {
                if (value != null) _root = value;
            }
        }

        public void OpenRoot(string path)
        {
            Root.Open(path);
        }
    }
}
