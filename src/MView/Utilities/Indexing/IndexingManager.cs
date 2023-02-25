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
    public static class IndexingManager
    {
        /// <summary>
        /// Index a file.
        /// </summary>
        /// <param name="extensions">The extensions of the files to index. If the value is null, index all files.</param>
        /// <returns>Indexed file item</returns>
        public static IndexedItem? GetFile(FileInfo file, string? rootDirectory, List<string>? extensions)
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
            item.RootDirectory = rootDirectory ?? "NULL";
            item.ParentDirectory = Path.GetDirectoryName(file.FullName) ?? "NULL";
            item.Size = file.Length;
            item.SizeString = UnitConverter.GetFileSizeString(file.Length);

            return item;
        }

        /// <summary>
        /// Index all files that exist within the directory.
        /// </summary>
        /// <param name="directory">Directory to index.</param>
        /// <param name="extensions">The extensions of the files to index. If the value is null, index all files.</param>
        /// <returns>Indexed file list</returns>
        public static List<IndexedItem> GetFiles(DirectoryInfo directory, string? rootDirectory, List<string>? extensions)
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
