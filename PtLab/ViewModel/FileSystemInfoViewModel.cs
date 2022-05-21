using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;

namespace PtLab.ViewModel
{
    public class FileSystemInfoViewModel : ViewModelBase
    {
        private FileSystemInfo _fileSystemInfo;
        private DateTime _lastWriteTime;
        private string _caption = String.Empty;
        private string _imagesource = String.Empty;
        private string _extension;
        private long _size;
        private string _statusMessage;

        private Dictionary<string, string> _images = new Dictionary<string, string>();
        
        public ViewModelBase Owner { get; private set; }

        public FileExplorer OwnerExplorer
        {
            get
            {
                var owner = Owner;
                while (owner is DirectoryInfoViewModel ownerDirectory)
                {
                    if (ownerDirectory.Owner is FileExplorer explorer)
                        return explorer;
                    owner = ownerDirectory.Owner;
                }
                return null;
            }
        }

        //public FileSystemInfoViewModel()
        //{

        //}

        public FileSystemInfo Model
        {
            get { return _fileSystemInfo; }
            set
            {
                if (_fileSystemInfo != value)
                {
                    SetProperties(value);
                    NotifyPropertyChanged();
                }
            }
        }

        public string ImageSource
        {
            get { return _imagesource; }
            set
            {
                if (_imagesource != value)
                {
                    _imagesource = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime LastWriteTime
        {
            get { return _lastWriteTime; }
            set
            {
                if (_lastWriteTime != value)
                {
                    _lastWriteTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Caption
        {
            get { return _caption; }
            set
            {
                if (_caption != value)
                {
                    _caption = value;
                }
            }
        }
        public string Extension
        {
            get { return _extension; }
            set
            {
                if (_extension != value)
                {
                    _extension = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public long Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
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

        public FileSystemInfoViewModel(ViewModelBase owner)
        {
            _images.Add("default", "pack://application:,,,/Resource/Icons/default.png");
            _images.Add(".txt", "pack://application:,,,/Resource/Icons/txt.png");
            _images.Add(".cs", "pack://application:,,,/Resource/Icons/csharp.png");
            _images.Add(".pdf", "pack://application:,,,/Resource/Icons/pdf.png");
            _images.Add(".jpg", "pack://application:,,,/Resource/Icons/image.png");
            _images.Add(".png", "pack://application:,,,/Resource/Icons/image.png");
            _images.Add(".xaml", "pack://application:,,,/Resource/Icons/xaml.png");

            Owner = owner;
        }

        protected void SetProperties(FileSystemInfo model)
        {
            _fileSystemInfo = model;
            this.LastWriteTime = model.LastWriteTime;
            this.Caption = model.Name;
            this.Extension = model.Extension;

            if (_images.ContainsKey(model.Extension))
            {
                this.ImageSource = _images[model.Extension];
            }
            else
            {
                this.ImageSource = _images["default"];
            }
            if ((model.Attributes & FileAttributes.Directory) != FileAttributes.Directory)
            {
                this.Size = new FileInfo(model.FullName).Length;
            }
        }

        public virtual void Sort(SortViewModel sorting)
        {

        }

        public void OpenFileCommand()
        {

        }

    }
}
