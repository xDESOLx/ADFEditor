using ADFEditor.Models;
using ADFEditor.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ADFEditor.Services
{
    public class PackageService : IPackageService
    {
        private string currentPath;
        public bool IsPathSet
        {
            get
            {
                return !string.IsNullOrEmpty(currentPath);
            }
        }

        public async Task ExtractFilesAsync(string path, List<FileEntry> files)
        {
            foreach (var item in files)
            {
                using (Stream stream = File.Create(Path.Combine(path, item.FileName)))
                {
                    await stream.WriteAsync(item.Content, 0, (int)item.Size);
                }
            }
        }

        public List<FileEntry> NewPackage()
        {
            currentPath = null;
            return new List<FileEntry>();
        }

        public async Task<List<FileEntry>> OpenPackageAsync(string path)
        {
            currentPath = path;
            using (Stream stream = File.OpenRead(currentPath))
            {
                byte[] buffer = new byte[18];
                await stream.ReadAsync(buffer, 0, 18);
                ADFHeader header = ByteArrayToStructure<ADFHeader>(buffer);
                if (new string(header.Magic) != "  YOU LAMER ;)  ")
                {
                    throw new FileFormatException(new Uri(currentPath), "The header of the specified package is invalid.");
                }
                List<ADFFileEntry> indexTable = new List<ADFFileEntry>();
                buffer = new byte[22];
                for (int i = 0; i < header.FileEntryCount; i++)
                {
                    await stream.ReadAsync(buffer, 0, 22);
                    indexTable.Add(ByteArrayToStructure<ADFFileEntry>(buffer));
                }
                List<FileEntry> files = new List<FileEntry>();
                foreach (var item in indexTable)
                {
                    FileEntry file = new FileEntry();
                    file.FileName = item.FileName;
                    file.Size = item.Size;
                    file.Content = new byte[file.Size];
                    stream.Seek(item.Offset, SeekOrigin.Begin);
                    await stream.ReadAsync(file.Content, 0, (int)file.Size);
                    files.Add(file);
                }
                return files;
            }
        }

        public async Task SavePackageAsync(string path, List<FileEntry> files)
        {
            currentPath = path;
            await SavePackageAsync(files);
        }

        public async Task SavePackageAsync(List<FileEntry> files)
        {
            using (Stream stream = File.Create(currentPath))
            {
                ADFHeader header = new ADFHeader()
                {
                    Magic = "  YOU LAMER ;)  ".ToCharArray(),
                    FileEntryCount = (ushort)files.Count
                };
                byte[] buffer = StructureToByteArray<ADFHeader>(header, 18);
                await stream.WriteAsync(buffer, 0, 18);
                uint currentOffset = 18 + header.FileEntryCount * (uint)22;
                for (int i = 0; i < files.Count; i++)
                {
                    ADFFileEntry entry = new ADFFileEntry()
                    {
                        FileName = files[i].FileName,
                        Offset = currentOffset,
                        Size = files[i].Size
                    };
                    buffer = StructureToByteArray<ADFFileEntry>(entry, 22);
                    await stream.WriteAsync(buffer, 0, 22);
                    currentOffset += entry.Size;
                }
                foreach (var item in files)
                {
                    await stream.WriteAsync(item.Content, 0, item.Content.Length);
                }

            }
        }

        private T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            T stuff;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
            return stuff;
        }
        private byte[] StructureToByteArray<T>(T structure, uint size) where T : struct
        {
            byte[] stuff = new byte[size];
            GCHandle handle = GCHandle.Alloc(stuff, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr<T>(structure, handle.AddrOfPinnedObject(), false);
            }
            finally
            {
                handle.Free();
            }
            return stuff;
        }
    }
}
