using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MView.Utilities
{
    /// <summary>
    /// Internals are mostly from here: http://www.codeproject.com/Articles/2532/Obtaining-and-managing-file-and-folder-icons-using
    /// Caches all results.
    /// </summary>
    internal static class IconManager
    {
        private static readonly Dictionary<string, ImageSource> _smallIconCache = new Dictionary<string, ImageSource>();
        private static readonly Dictionary<string, ImageSource> _largeIconCache = new Dictionary<string, ImageSource>();

        /// <summary>
        /// Get an icon for a given filename
        /// </summary>
        /// <param name="fileName">any filename</param>
        /// <param name="large">16x16 or 32x32 icon</param>
        /// <returns>null if path is null, otherwise - an icon</returns>
        internal static ImageSource? FindIconForFilename(string fileName, bool large)
        {
            DirectoryInfo di = new DirectoryInfo(fileName);
            string extension = di.Extension;

            if (extension == null)
                return null;
            
            if (extension == string.Empty && di.Root.FullName == di.FullName)
            {
                extension = "ROOT";
            }
            
            if (extension == string.Empty && di.Attributes.HasFlag(FileAttributes.Directory))
            {
                extension = "FOLDER";
            }
            else
            {
                if (di.Attributes.HasFlag(FileAttributes.Directory) && extension != "ROOT")
                {
                    extension = "FOLDER";
                }
            }
            
            var cache = large ? _largeIconCache : _smallIconCache;
            ImageSource icon;
            
            if (cache.TryGetValue(extension, out icon))
                return icon;

            if (di.Attributes.HasFlag(FileAttributes.Directory) && extension != "ROOT")
            {
                icon = IconReader.GetFolderIcon(fileName, large ? IconReader.IconSize.Large : IconReader.IconSize.Small, IconReader.FolderType.Closed).ToImageSource();
            }
            else
            {
                icon = IconReader.GetFileIcon(fileName, large ? IconReader.IconSize.Large : IconReader.IconSize.Small, false).ToImageSource();
            }
            
            if (extension != "ROOT") cache.Add(extension, icon);
            
            return icon;
        }

        /// <summary>
        /// http://stackoverflow.com/a/6580799/1943849
        /// </summary>
        static ImageSource ToImageSource(this System.Drawing.Icon icon)
        {
            var imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }

        /// <summary>
        /// Provides static methods to read system icons for both folders and files.
        /// </summary>
        /// <example>
        /// <code>IconReader.GetFileIcon("c:\\general.xls");</code>
        /// </example>
        static class IconReader
        {
            /// <summary>
            /// Options to specify the size of icons to return.
            /// </summary>
            internal enum IconSize
            {
                /// <summary>
                /// Specify large icon - 32 pixels by 32 pixels.
                /// </summary>
                Large = 0,
                /// <summary>
                /// Specify small icon - 16 pixels by 16 pixels.
                /// </summary>
                Small = 1
            }

            /// <summary>
            /// Returns an icon for a given file - indicated by the name parameter.
            /// </summary>
            /// <param name="name">Pathname for file.</param>
            /// <param name="size">Large or small</param>
            /// <param name="linkOverlay">Whether to include the link icon</param>
            /// <returns>System.Drawing.Icon</returns>
            internal static System.Drawing.Icon GetFileIcon(string name, IconSize size, bool linkOverlay)
            {
                var shfi = new Shell32.SHFILEINFO();
                var flags = Shell32.SHGFI_ICON;

                if (linkOverlay) flags += Shell32.SHGFI_LINKOVERLAY;

                /* Check the size specified for return. */
                if (IconSize.Small == size)
                    flags += Shell32.SHGFI_SMALLICON;
                else
                    flags += Shell32.SHGFI_SMALLICON;

                Shell32.SHGetFileInfo(name,
                    Shell32.FILE_ATTRIBUTE_NORMAL,
                    ref shfi,
                    (uint)Marshal.SizeOf(shfi),
                    flags);
                // Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly
                var icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
                User32.DestroyIcon(shfi.hIcon);     // Cleanup
                return icon;
            }

            /// <summary>
            /// Options to specify whether folders should be in the open or closed state.
            /// </summary>
            internal enum FolderType
            {
                /// <summary>
                /// Specify open folder.
                /// </summary>
                Open = 0,
                /// <summary>
                /// Specify closed folder.
                /// </summary>
                Closed = 1
            }

            /// <summary>
            /// Used to access system folder icons.
            /// </summary>
            /// <param name="size">Specify large or small icons.</param>
            /// <param name="folderType">Specify open or closed FolderType.</param>
            /// <returns>System.Drawing.Icon</returns>
            internal static System.Drawing.Icon GetFolderIcon(string name, IconSize size, FolderType folderType)
            {
                // Need to add size check, although errors generated at present!
                uint flags = Shell32.SHGFI_ICON | Shell32.SHGFI_SYSICONINDEX;

                if (FolderType.Open == folderType)
                {
                    flags += Shell32.SHGFI_OPENICON;
                }

                if (IconSize.Small == size)
                {
                    flags += Shell32.SHGFI_SMALLICON;
                }
                else
                {
                    flags += Shell32.SHGFI_LARGEICON;
                }

                // Get the folder icon
                Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
                Shell32.SHGetFileInfo(name,
                    Shell32.FILE_ATTRIBUTE_DIRECTORY,
                    ref shfi,
                    (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                    flags);

                System.Drawing.Icon.FromHandle(shfi.hIcon); // Load the icon from an HICON handle

                // Now clone the icon, so that it can be successfully stored in an ImageList
                System.Drawing.Icon icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();

                User32.DestroyIcon(shfi.hIcon);     // Cleanup
                return icon;
            }
        }
    }

    /// <summary>
    /// Wraps necessary Shell32.dll structures and functions required to retrieve Icon Handles using SHGetFileInfo. Code
    /// courtesy of MSDN Cold Rooster Consulting case study.
    /// </summary>
    internal class Shell32
    {

        internal const int MAX_PATH = 256;
        [StructLayout(LayoutKind.Sequential)]
        internal struct SHITEMID
        {
            internal ushort cb;
            [MarshalAs(UnmanagedType.LPArray)]
            internal byte[] abID;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ITEMIDLIST
        {
            internal SHITEMID mkid;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct BROWSEINFO
        {
            public IntPtr hwndOwner;
            public IntPtr pidlRoot;
            public IntPtr pszDisplayName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszTitle;
            public uint ulFlags;
            public IntPtr lpfn;
            public int lParam;
            public IntPtr iImage;
        }

        // Browsing for directory.
        internal const uint BIF_RETURNONLYFSDIRS = 0x0001;
        internal const uint BIF_DONTGOBELOWDOMAIN = 0x0002;
        internal const uint BIF_STATUSTEXT = 0x0004;
        internal const uint BIF_RETURNFSANCESTORS = 0x0008;
        internal const uint BIF_EDITBOX = 0x0010;
        internal const uint BIF_VALIDATE = 0x0020;
        internal const uint BIF_NEWDIALOGSTYLE = 0x0040;
        internal const uint BIF_USENEWUI = (BIF_NEWDIALOGSTYLE | BIF_EDITBOX);
        internal const uint BIF_BROWSEINCLUDEURLS = 0x0080;
        internal const uint BIF_BROWSEFORCOMPUTER = 0x1000;
        internal const uint BIF_BROWSEFORPRINTER = 0x2000;
        internal const uint BIF_BROWSEINCLUDEFILES = 0x4000;
        internal const uint BIF_SHAREABLE = 0x8000;

        [StructLayout(LayoutKind.Sequential)]
        internal struct SHFILEINFO
        {
            internal const int NAMESIZE = 80;
            internal IntPtr hIcon;
            internal int iIcon;
            internal uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAMESIZE)]
            internal string szTypeName;
        };

        internal const uint SHGFI_ICON = 0x000000100;     // get icon
        internal const uint SHGFI_DISPLAYNAME = 0x000000200;     // get display name
        internal const uint SHGFI_TYPENAME = 0x000000400;     // get type name
        internal const uint SHGFI_ATTRIBUTES = 0x000000800;     // get attributes
        internal const uint SHGFI_ICONLOCATION = 0x000001000;     // get icon location
        internal const uint SHGFI_EXETYPE = 0x000002000;     // return exe type
        internal const uint SHGFI_SYSICONINDEX = 0x000004000;     // get system icon index
        internal const uint SHGFI_LINKOVERLAY = 0x000008000;     // put a link overlay on icon
        internal const uint SHGFI_SELECTED = 0x000010000;     // show icon in selected state
        internal const uint SHGFI_ATTR_SPECIFIED = 0x000020000;     // get only specified attributes
        internal const uint SHGFI_LARGEICON = 0x000000000;     // get large icon
        internal const uint SHGFI_SMALLICON = 0x000000001;     // get small icon
        internal const uint SHGFI_OPENICON = 0x000000002;     // get open icon
        internal const uint SHGFI_SHELLICONSIZE = 0x000000004;     // get shell size icon
        internal const uint SHGFI_PIDL = 0x000000008;     // pszPath is a pidl
        internal const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;     // use passed dwFileAttribute
        internal const uint SHGFI_ADDOVERLAYS = 0x000000020;     // apply the appropriate overlays
        internal const uint SHGFI_OVERLAYINDEX = 0x000000040;     // Get the index of the overlay

        internal const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        internal const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        [DllImport("Shell32.dll", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr SHGetFileInfo(
                      string pszPath,
                      uint dwFileAttributes,
                      ref SHFILEINFO psfi,
                      uint cbFileInfo,
                      uint uFlags
                      );
    }

    /// <summary>
    /// Wraps necessary functions imported from User32.dll. Code courtesy of MSDN Cold Rooster Consulting example.
    /// </summary>
    static class User32
    {
        /// <summary>
        /// Provides access to function required to delete handle. This method is used internally
        /// and is not required to be called separately.
        /// </summary>
        /// <param name="hIcon">Pointer to icon handle.</param>
        /// <returns>N/A</returns>
        [DllImport("User32.dll")]
        internal static extern int DestroyIcon(IntPtr hIcon);
    }
}
