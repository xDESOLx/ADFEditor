using ADFEditor.Models;
using ADFEditor.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Syncfusion.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ADFEditor.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Untitled - Arsenal ADF Package Editor";
        private readonly IDialogService _dialogService;
        private readonly IPackageService _packageService;

        public DelegateCommand NewPackageCommand { get; private set; }
        public DelegateCommand OpenPackageCommand { get; private set; }
        public DelegateCommand SavePackageCommand { get; private set; }
        public DelegateCommand SavePackageAsCommand { get; private set; }
        public DelegateCommand AddFilesCommand { get; private set; }
        public DelegateCommand ExtractFilesCommand { get; private set; }
        public DelegateCommand ReplaceFileCommand { get; private set; }
        public DelegateCommand RemoveFilesCommand { get; private set; }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private ObservableCollection<FileEntry> files;
        public ObservableCollection<FileEntry> Files
        {
            get { return files; }
            set { SetProperty(ref files, value); }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        private ObservableCollection<object> selectedFiles;
        public ObservableCollection<object> SelectedFiles
        {
            get { return selectedFiles; }
            set { SetProperty(ref selectedFiles, value); }
        }

        public MainWindowViewModel(IDialogService dialogService, IPackageService packageService)
        {
            _dialogService = dialogService;
            _packageService = packageService;
            

            Files = new ObservableCollection<FileEntry>();
            Files.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            {

            };
            SelectedFiles = new ObservableCollection<object>();
            SelectedFiles.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            {
                ExtractFilesCommand.RaiseCanExecuteChanged();
                ReplaceFileCommand.RaiseCanExecuteChanged();
                RemoveFilesCommand.RaiseCanExecuteChanged();
            };

            NewPackageCommand = new DelegateCommand(NewPackage);
            OpenPackageCommand = new DelegateCommand(() => _dialogService.ShowDialog("OpenPackageDialog", null, OpenPackage));
            SavePackageCommand = new DelegateCommand(SavePackage);
            SavePackageAsCommand = new DelegateCommand(() => _dialogService.ShowDialog("SavePackageDialog", null, SavePackageAs));
            AddFilesCommand = new DelegateCommand(() => _dialogService.ShowDialog("AddFilesDialog", null, AddFiles));
            ExtractFilesCommand = new DelegateCommand(() => _dialogService.ShowDialog("FolderBrowserDialog", null, ExtractFiles), () => SelectedFiles.Count != 0);
            ReplaceFileCommand = new DelegateCommand(() => _dialogService.ShowDialog("ReplaceFileDialog", null, ReplaceFile), () => SelectedFiles.Count == 1);
            RemoveFilesCommand = new DelegateCommand(RemoveFiles, () => SelectedFiles.Count != 0);
        }

        private void NewPackage()
        {
            Files = new ObservableCollection<FileEntry>(_packageService.NewPackage());
            Title = "Untitled - Arsenal ADF Package Editor";
        }

        private async void OpenPackage(IDialogResult result)
        {
            if (result.Result == ButtonResult.OK)
            {
                IsBusy = true;
                Files = new ObservableCollection<FileEntry>(await _packageService.OpenPackageAsync(result.Parameters.GetValue<string>("path")));
                Title = Path.GetFileName(result.Parameters.GetValue<string>("path")) + " - Arsenal ADF Package Editor";
                IsBusy = false;
            }
        }
        private async void SavePackageAs(IDialogResult result)
        {
            if (result.Result == ButtonResult.OK)
            {
                IsBusy = true;
                await _packageService.SavePackageAsync(result.Parameters.GetValue<string>("path"), Files.ToList());
                Title = Path.GetFileName(result.Parameters.GetValue<string>("path")) + " - Arsenal ADF Package Editor";
                IsBusy = false;
            }
        }
        private async void SavePackage()
        {
            if (_packageService.IsPathSet)
            {
                IsBusy = true;
                await _packageService.SavePackageAsync(Files.ToList());
                IsBusy = false;
                
            }
            else
            {
                SavePackageAsCommand.Execute();
            }
        }

        private async void AddFiles(IDialogResult result)
        {
            if (result.Result == ButtonResult.OK)
            {
                IsBusy = true;
                string[] filenames = result.Parameters.GetValue<string>("filenames").Split('\t');
                List<FileEntry> entries = new List<FileEntry>();
                foreach (var item in filenames)
                {
                    using (Stream stream = File.OpenRead(item))
                    {
                        FileEntry entry = new FileEntry()
                        {
                            FileName = Path.GetFileName(item).ToUpper(),
                            Content = new byte[stream.Length],
                            Size = (uint)stream.Length
                        };
                        await stream.ReadAsync(entry.Content, 0, (int)stream.Length);
                        entries.Add(entry);
                    }
                }
                Files.AddRange(entries);
                IsBusy = false;
            }
        }
        private async void ExtractFiles(IDialogResult result)
        {
            if (result.Result == ButtonResult.OK)
            {
                IsBusy = true;
                await _packageService.ExtractFilesAsync(result.Parameters.GetValue<string>("path"), SelectedFiles.Cast<FileEntry>().ToList());
                IsBusy = false;
            }
        }
        private async void ReplaceFile(IDialogResult result)
        {
            if (result.Result == ButtonResult.OK)
            {
                IsBusy = true;
                using (Stream stream = File.OpenRead(result.Parameters.GetValue<string>("path")))
                {
                    FileEntry entry = (FileEntry)SelectedFiles.Single();
                    entry.Size = (uint)stream.Length;
                    entry.Content = new byte[entry.Size];
                    await stream.ReadAsync(entry.Content, 0, (int)entry.Size);
                    entry.FileName = Path.GetFileName(result.Parameters.GetValue<string>("path")).ToUpper();
                }
                IsBusy = false;
            }
        }
        private void RemoveFiles()
        {
            List<FileEntry> filesToRemove = SelectedFiles.Cast<FileEntry>().ToList();
            List<FileEntry> newFiles = new List<FileEntry>(Files);
            newFiles.RemoveAll(x => filesToRemove.Contains(x));
            Files = new ObservableCollection<FileEntry>(newFiles);
        }
    }
}
