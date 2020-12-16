using MView.Core.Extension;
using System.Text;

namespace MView.Core.Data
{
    /// <summary>
    /// A class that provides tools related to RPGSAVE files.
    /// </summary>
    public class SaveDataManager
    {
        private string _filePath = string.Empty;
        private Encoding _encoding = Encoding.UTF8;

        /// <summary>
        /// Create a new instance that provides the functions associated with the RPGSAVE file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        public SaveDataManager(string filePath, Encoding encoding)
        {
            _filePath = filePath;
            _encoding = encoding;
        }

        /// <summary>
        /// Package and save the source.
        /// </summary>
        /// <param name="source"></param>
        public void Package(string source)
        {
            string result = source.CompressToBase64();
            FileManager.WriteTextFile(_filePath, result, _encoding);
        }

        /// <summary>
        /// Returns the JSON string by unpackaging the file.
        /// </summary>
        /// <returns></returns>
        public string Unpackage()
        {
            string source = FileManager.ReadTextFile(_filePath, _encoding);
            return source.DecompressFromBase64();
        }
    }
}
