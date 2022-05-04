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
        /// Returns a file item.
        /// </summary>
        /// <param name="file">The file info.</param>
        /// <param name="checkExtension">Whether to check file exteion.</param>
        /// <returns>The directory item.</returns>
        public static IndexedItem? GetFileItem(FileInfo file, bool checkExtension = false)
        {
            // Check a extension of a file. 
            if (checkExtension)
            {
                if (!Settings.KnownExtensions.Any(p => p.Equals(file.Extension, StringComparison.OrdinalIgnoreCase)))
                {
                    return null;
                }
            }

            // Index
            IndexedItem item = new IndexedItem();

            item.Type = IndexedItemType.File;
            item.Name = file.Name;
            item.FullName = file.FullName;
            item.Size = file.Length;
            item.IsSelected = true;
            item.SubItems = new List<IndexedItem>();

            return item;
        }

        /// <summary>
        /// Returns a folder item.
        /// </summary>
        /// <param name="directory">The directory info to get information.</param>
        /// <returns>The directory item.</returns>
        public static IndexedItem? GetFolderItem(DirectoryInfo directory, bool checkExtension = false)
        {
            IndexedItem item = new IndexedItem();

            item.Type = IndexedItemType.Folder;
            item.Name = directory.Name;
            item.FullName = directory.FullName;
            item.IsSelected = true;

            try
            {
                var subDirectories = directory.EnumerateDirectories();

                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    IndexedItem? dirItem = GetFolderItem(subDirectory, checkExtension);

                    if (dirItem?.SubItems.Count > 0)
                    {
                        dirItem.Parent = item;
                        item.SubItems.Add(dirItem);
                    }
                }
            }
            catch { }

            try
            {
                var files = directory.EnumerateFiles();

                foreach (FileInfo file in files)
                {
                    IndexedItem? fileItem = GetFileItem(file, checkExtension);

                    if (fileItem != null)
                    {
                        fileItem.Parent = item;
                        item.SubItems.Add(fileItem);
                    }
                }
            }
            catch { }

            if (item.SubItems.Count == 0)
            {
                return null;
            }

            return item;
        }

    }
}
