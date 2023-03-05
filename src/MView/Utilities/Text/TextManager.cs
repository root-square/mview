using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Utilities.Text
{
    /// <summary>
    /// Provides functions related to text I/O.
    /// </summary>
    public class TextManager
    {
        /// <summary>
        /// Writes a text file in the specified path.
        /// </summary>
        /// <param name="filePath">Path where the text file will be saved.</param>
        /// <param name="text">Text to be writed.</param>
        /// <param name="encoding">The text encoding to use.</param>
        public static void WriteTextFile(string filePath, string text, Encoding encoding)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            }

            using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                using (StreamWriter writer = new StreamWriter(stream, encoding))
                {
                    writer.Write(text);
                }
            }
        }

        /// <summary>
        /// Writes a text file in the specified path.
        /// </summary>
        /// <param name="filePath">Path where the text file will be saved.</param>
        /// <param name="text">Text to be writed.</param>
        /// <param name="encoding">The text encoding to use.</param>
        public static async Task WriteTextFileAsync(string filePath, string text, Encoding encoding)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            }

            using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                using (StreamWriter writer = new StreamWriter(stream, encoding))
                {
                    await writer.WriteAsync(text);
                }
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
            string text = string.Empty;

            using (StreamReader reader = new StreamReader(filePath, encoding))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }

        /// <summary>
        /// Reads a text file in the specified path.
        /// </summary>
        /// <param name="filePath">Path where the text file will be read.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>Text</returns>
        public static async Task<string> ReadTextFileAsync(string filePath, Encoding encoding)
        {
            string text = string.Empty;

            using (StreamReader reader = new StreamReader(filePath, encoding))
            {
                text = await reader.ReadToEndAsync();
            }

            return text;
        }
    }
}
