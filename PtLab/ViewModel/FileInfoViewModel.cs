using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using PtLab.View;

namespace PtLab.ViewModel
{

    public class FileInfoViewModel : FileSystemInfoViewModel
    {
        public ICommand OpenFileCommand { get; private set; }

        public FileInfoViewModel(ViewModelBase owner) : base(owner)
        {
            OpenFileCommand = new RelayCommand(OnOpenFileCommand, CanExecuteOnOpenFileCommand);
        }

        private bool CanExecuteOnOpenFileCommand(object obj)
        {
            return OwnerExplorer.OpenFileCommand.CanExecute(obj);
        }

        private void OnOpenFileCommand(object obj)
        {
            OwnerExplorer.OpenFileCommand.Execute(obj);
        }

    }
}
