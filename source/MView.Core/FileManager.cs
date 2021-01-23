using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MView.Core
{
    /// <summary>
    /// Provides functions related to file I/O.
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// Index all files that exist within the directory.
        /// </summary>
        /// <param name="directory">Directory to index.</param>
        /// <param name="extensions">The extensions of the files to index. If the value is null, index all files.</param>
        /// <returns>Indexed file list</returns>
        public static List<string> GetFiles(string directory, List<string> extensions = null)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException("Directory cannot be null or empty.");
            }

            if (Directory.GetDirectoryRoot(directory).ToLower() == directory.ToLower())
            {
                throw new InvalidOperationException("The list of files in the root directory cannot be imported.");
            }

            try
            {
                List<string> files = new List<string>();
                DirectoryInfo dir = new DirectoryInfo(directory);

                // Indexing files in current directory.
                foreach (FileInfo file in dir.GetFiles())
                {
                    if (file.IsReadOnly)
                    {
                        continue;
                    }

                    if (extensions == null)
                    {
                        files.Add(file.FullName);
                    }
                    else
                    {
                        if (extensions.Contains(file.Extension.ToLower()))
                        {
                            files.Add(file.FullName);
                        }
                    }
                }

                // Re-indexing files in sub-directory.
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                {
                    files.AddRange(GetFiles(subdir.FullName, extensions));
                }

                return files;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Writes a text file in the specified path.
        /// </summary>
        /// <param name="filePath">Path where the text file will be saved.</param>
        /// <param name="text">Text to be writed.</param>
        /// <param name="encoding">The text encoding to use.</param>
        public static void WriteTextFile(string filePath, string text, Encoding encoding)
        {
            using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                StreamWriter objSaveFile = new StreamWriter(stream, encoding);
                objSaveFile.Write(text);
                objSaveFile.Close();
                objSaveFile.Dispose();
            }
        }

        /// <summary>
        /// Reads a text file in the specified path.
        /// </summary>
        /// <param name="filePath">Path where the text file will be read.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>Text</returns>
        public static string ReadTextFile(string filePath, Encoding encoding)
        {
            string temp = string.Empty;
            using (StreamReader objReadFile = new StreamReader(filePath, encoding))
            {
                temp = objReadFile.ReadToEnd();
                objReadFile.Close();
                objReadFile.Dispose();
            }
            return temp;
        }
    }
}
