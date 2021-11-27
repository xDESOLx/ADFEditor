using Ookii.Dialogs.Wpf;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADFEditor.Services
{
    public class FileDialogService : IDialogService
    {
        public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            throw new NotImplementedException();
        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            switch (name)
            {
                case "OpenPackageDialog":
                    VistaOpenFileDialog openFileDialog = new VistaOpenFileDialog()
                    {
                        Filter = "ADF files (*.adf)|*.adf"
                    };
                    var openPackageDialogResult = openFileDialog.ShowDialog();
                    callback.Invoke(new DialogResult(openPackageDialogResult.Value ? ButtonResult.OK : ButtonResult.Cancel, new DialogParameters($"path={openFileDialog.FileName}")));
                    break;
                case "SavePackageDialog":
                    VistaSaveFileDialog saveFileDialog = new VistaSaveFileDialog()
                    {
                        Filter = "ADF files (*.adf)|*.adf",
                        DefaultExt = ".adf"
                    };
                    var savePackageDialogResult = saveFileDialog.ShowDialog();
                    callback.Invoke(new DialogResult(savePackageDialogResult.Value ? ButtonResult.OK : ButtonResult.Cancel, new DialogParameters($"path={saveFileDialog.FileName}")));
                    break;
                case "FolderBrowserDialog":
                    VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog();
                    var folderBrowserResult = vistaFolderBrowserDialog.ShowDialog();
                    callback.Invoke(new DialogResult(folderBrowserResult.Value ? ButtonResult.OK : ButtonResult.Cancel, new DialogParameters($"path={vistaFolderBrowserDialog.SelectedPath}")));
                    break;
                case "AddFilesDialog":
                    VistaOpenFileDialog addFilesDialog = new VistaOpenFileDialog()
                    {
                        Filter = "Any files (*.*)|*.*",
                        Multiselect = true
                    };
                    var addFilesDialogResult = addFilesDialog.ShowDialog();
                    callback.Invoke(new DialogResult(addFilesDialogResult.Value ? ButtonResult.OK : ButtonResult.Cancel, new DialogParameters($"filenames={string.Join("\t", addFilesDialog.FileNames)}")));
                    break;
                case "ReplaceFileDialog":
                    VistaOpenFileDialog replaceFileDialog = new VistaOpenFileDialog()
                    {
                        Filter = "Any files (*.*)|*.*"
                    };
                    var replacePackageDialogResult = replaceFileDialog.ShowDialog();
                    callback.Invoke(new DialogResult(replacePackageDialogResult.Value ? ButtonResult.OK : ButtonResult.Cancel, new DialogParameters($"path={replaceFileDialog.FileName}")));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(name));
            }
        }
    }
}
