using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using PtLab.Enum;
using PtLab.Resource;
using PtLab.View;

namespace PtLab.ViewModel
{
    public class DirectoryInfoViewModel : FileSystemInfoViewModel
    {
        private string _path = String.Empty;
        private FileSystemWatcher _watcher;
        public DispatchedObservableCollection<FileSystemInfoViewModel> Items { get; private set; }
            = new DispatchedObservableCollection<FileSystemInfoViewModel>();

        public Exception Exception { get; private set; }

        public bool IsInitialized
        {
            get
            {
                if (_path == null) return false;
                if (_path.Length == 0) return false;
                return true;
            }
        }
        public void OnFileSystemChanged(object sender, FileSystemEventArgs e)
        {
            //Handling for multithread execution
            Application.Current.Dispatcher.Invoke(() => OnFileSystemChanged(e));
        }

        public DirectoryInfoViewModel(ViewModelBase owner) : base(owner)
        {
            Items.CollectionChanged += Items_CollectionChanged;
        }

        public bool Open(string path)
        {
            this._path = path;
            bool result = false;
            
            Items.Clear();
            ReadCatalogs();
            ReadFiles();
            result = true;

            if (result)
            {
                InitlizeWatcher();
            }

            return result;
        }

        public void Sort(SortViewModel sortViewModel)
        {
            bool isEmpty = !IsInitialized;

            if (isEmpty) return;

            List<Task> tasks = new List<Task>();

            foreach (var item in Items)
            {
                Task? task = null;

                if (item is DirectoryInfoViewModel directoryItem)
                {
                    task = Task.Factory.StartNew(() =>
                    {
                        directoryItem?.Sort(sortViewModel);
                    });

                    if (item?.Model?.FullName != null)
                    {
                        StatusMessage = Strings.Sorting_dir + " " + item.Model.Name;
                    }
                }

                if (task != null) {
                    tasks.Add(task);
                };
            }

            Task.WaitAll(tasks.ToArray());

            StatusMessage = Strings.Ready;

            var orderableItems = Items.OrderBy(OrderByType);

            if (sortViewModel.SortOrder == SortOrder.Ascending)
            {
                if (sortViewModel.SortBy == SortBy.Alphabetically)
                {
                    orderableItems = orderableItems.ThenBy(item => item.Caption);
                }

                if (sortViewModel.SortBy == SortBy.LastModificationDate)
                {
                    orderableItems = orderableItems.ThenBy(item => item.LastWriteTime);
                }

                if (sortViewModel.SortBy == SortBy.Extension)
                {
                    orderableItems = orderableItems.ThenBy(item => item.Extension);
                }

                if (sortViewModel.SortBy == SortBy.Size)
                {
                    orderableItems = orderableItems.ThenBy(item => item.Size);
                }
            }

            if (sortViewModel.SortOrder == SortOrder.Descending)
            {
                if (sortViewModel.SortBy == SortBy.Alphabetically)
                {
                    orderableItems = orderableItems.ThenByDescending(item => item.Caption);
                }

                if (sortViewModel.SortBy == SortBy.LastModificationDate)
                {
                    orderableItems = orderableItems.ThenByDescending(item => item.LastWriteTime);
                }

                if (sortViewModel.SortBy == SortBy.Extension)
                {
                    orderableItems = orderableItems.ThenByDescending(item => item.Extension);
                }

                if (sortViewModel.SortBy == SortBy.Size)
                {
                    orderableItems = orderableItems.ThenByDescending(item => item.Size);
                }
            }

            var itemList = orderableItems.ToList();

            foreach (var item in itemList)
            {
                var newIndex = itemList.IndexOf(item);
                var oldIndex = Items.IndexOf(item);
                if (newIndex > -1)
                {
                    Items.Move(oldIndex, newIndex);
                }
            }
        }

        private int OrderByType(ViewModelBase item)
        {
            if (item is DirectoryInfoViewModel)
            {
                return 0;
            }

            return 1;
        }

        private void ReadCatalogs()
        {
            try
            {
                foreach (var dirName in Directory.GetDirectories(_path))
                {
                    var dirInfo = new DirectoryInfo(dirName);
                    var itemViewModel = new DirectoryInfoViewModel(this);
                    itemViewModel.Model = dirInfo;
                    Items.Add(itemViewModel);

                    // Recurrecny load
                    itemViewModel.Open(dirName);
                }
            }
            catch (Exception ex)
            {
                // do nothing
            }
            
        }

        private void ReadFiles()
        {
            foreach (var fileName in Directory.GetFiles(_path))
            {
                var fileInfo = new FileInfo(fileName);
                var itemViewModel = new FileInfoViewModel(this);

                itemViewModel.Model = fileInfo;
                Items.Add(itemViewModel);
            }
        }

        private void InitlizeWatcher()
        {
            _watcher = new FileSystemWatcher(_path);
            _watcher.Created += OnFileSystemChanged;
            _watcher.Renamed += OnFileSystemChanged;
            _watcher.Deleted += OnFileSystemChanged;
            _watcher.Changed += OnFileSystemChanged;

            _watcher.Error += WatcherError;
            _watcher.EnableRaisingEvents = true;
        }

        private void WatcherError(object sender, ErrorEventArgs e)
        {
            return;
        }

        private void OnFileSystemChanged(FileSystemEventArgs e)
        {
            Open(_path);
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in args.NewItems.Cast<FileSystemInfoViewModel>())
                    {
                        item.PropertyChanged += Item_PropertyChanged;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in args.NewItems.Cast<FileSystemInfoViewModel>())
                    {
                        item.PropertyChanged -= Item_PropertyChanged;
                    }
                    break;
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "StatusMessage" && sender is FileSystemInfoViewModel viewModel)
                this.StatusMessage = viewModel.StatusMessage;
        }
    }
}
