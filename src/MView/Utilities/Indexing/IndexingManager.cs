using MView.Utilities.Text;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Utilities.Indexing
{
    internal static class IndexingManager
    {
        internal static bool IsDirectory(string path)
        {
            FileAttributes attrributes = File.GetAttributes(path);

            return (attrributes & FileAttributes.Directory) == FileAttributes.Directory ? true : false;
        }

        internal static bool IsExists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        internal static IndexedItem? GetFile(FileInfo file, string rootDirectory, List<string>? extensions = null)
        {
            if (extensions == null)
            {
                extensions = new List<string>();
            }
            else
            {
                if (!extensions.Any(e => e.Equals(file.Extension, StringComparison.OrdinalIgnoreCase)))
                {
                    return null;
                }
            }

            IndexedItem item = new IndexedItem();
            item.IsSelected = true;
            item.FileName = file.Name;
            item.FullPath = file.FullName;
            item.RootDirectory = rootDirectory;
            item.ParentDirectory = Path.GetDirectoryName(file.FullName)!;
            item.Size = file.Length;
            item.SizeString = UnitConverter.GetFileSizeString(file.Length);

            return item;
        }

        internal static List<IndexedItem> GetFiles(DirectoryInfo directory, string rootDirectory, List<string>? extensions = null)
        {
            List<IndexedItem> items = new List<IndexedItem>();

            // Index files in current directory.
            try
            {
                foreach (FileInfo file in directory.EnumerateFiles())
                {
                    IndexedItem? item = GetFile(file, rootDirectory, extensions);

                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An unexpected exception has occured.");
            }

            // Re-index files in sub-directory.
            try
            {
                foreach (DirectoryInfo subDirectory in directory.EnumerateDirectories())
                {
                    List<IndexedItem> subItems = GetFiles(subDirectory, rootDirectory, extensions);
                    items.AddRange(subItems);
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An unexpected exception has occured.");
            }

            return items;
        }
    }
}
