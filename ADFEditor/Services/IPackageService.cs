using ADFEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADFEditor.Services
{
    public interface IPackageService
    {
        bool IsPathSet { get; }
        List<FileEntry> NewPackage();
        Task<List<FileEntry>> OpenPackageAsync(string path);
        Task SavePackageAsync(string path, List<FileEntry> files);
        Task SavePackageAsync(List<FileEntry> files);
        Task ExtractFilesAsync(string path, List<FileEntry> files);
    }
}
