using System.IO;
using System.Text;

namespace MView.Utilities
{
    /// <summary>
    /// Provides functions related to file I/O.
    /// </summary>
    public static class FileUtility
    {
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
