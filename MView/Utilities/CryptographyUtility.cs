using MView.Extensions;
using System;
using System.IO;
using System.Linq;

namespace MView.Utilities
{
    /// <summary>
    /// Provides tools related to encrypted resource files of RPG Maker MV.
    /// </summary>
    public static class CryptographyUtility
    {
        #region ::Consts::

        /// <summary>
        /// RPG MV encrypted resource file extensions.
        /// </summary>
        public static readonly string[] EncryptedExtensions = new string[] { ".rpgmvo", ".rpgmvm", ".rpgmvw", ".rpgmvp" };

        /// <summary>
        /// RPG MV decrypted resource file extensions.
        /// </summary>
        public static readonly string[] DecryptedExtensions = new string[] { ".ogg", ".m4a", ".wav", ".png" };

        /// <summary>
        /// RPG MV encrypted resource file header.
        /// </summary>
        private static readonly string[] HEADER_MV = new string[] { "52", "50", "47", "4D", "56", "00", "00", "00", "00", "03", "01", "00", "00", "00", "00", "00" };

        /// <summary>
        /// MPEG-4 Part.14 audio file header.
        /// </summary>
        private static readonly string[] HEADER_M4A = new string[] { "00", "00", "00", "20", "66", "74", "79", "70", "4D", "34", "41", "20", "00", "00", "00", "00" };

        /// <summary>
        /// Waveform audio format file header.
        /// </summary>
        private static readonly string[] HEADER_WAV = new string[] { "52", "49", "46", "46", "24", "3C", "00", "00", "57", "41", "56", "45", "66", "6D", "74", "20" };

        /// <summary>
        /// Portable Network Graphics file header.
        /// </summary>
        private static readonly string[] HEADER_PNG = new string[] { "89", "50", "4E", "47", "0D", "0A", "1A", "0A", "00", "00", "00", "0D", "49", "48", "44", "52" };

        #endregion

        /// <summary>
        /// Verify that the file is a valid RPG Maker MV encrypted resource file.
        /// </summary>
        /// <param name="filePath">Path to the file to validate.</param>
        /// <returns>Validity of the file</returns>
        public static bool VerifyFakeHeader(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("The file you are trying to decrypt header does not exist.");
                }

                // Loads encrypted file and skips RPG Maker MV encryption header area.
                byte[] file = File.ReadAllBytes(filePath);

                for (int index = 0; index < HEADER_MV.Length; index++)
                {
                    if (file[index] != HEADER_MV[index].HexToByte())
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Encrypted File Structure : RPG Maker MV Header(16Byte) -> Original Header that encrypted by XOR operation(16Byte) -> File Contents
        // Encryption Procedure : Encrypt original file header with XOR operation(16Byte) -> Create new file -> Insert RPG Maker MV Header(16Byte) on the top -> Insert Encrypted original File

        /// <summary>
        /// Encrypts the header of the RPG Maker MV resource file.
        /// </summary>
        /// <param name="filePath">The path to the file to encrypt the header.</param>
        /// <param name="savePath">The path where the completed file will be saved.</param>
        /// <param name="key">The key to encrypt the file. Requires hex 128-bit value (MD5).</param>
        public static void EncryptHeader(string filePath, string savePath, string key)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("The file you are trying to encrypt header does not exist.");
                }

                // Checks extension.
                string extension = Path.GetExtension(filePath).ToLower();

                if (!DecryptedExtensions.Contains(extension))
                {
                    throw new NotSupportedException("Incompatible file format used.");
                }

                // Loads encrypted file and skips RPG Maker MV encryption header area.
                byte[] file = File.ReadAllBytes(filePath);

                // Writes a fake header as byte array.
                byte[] header = new byte[16];

                for (int index = 0; index < header.Length; index++)
                {
                    header[index] = HEADER_MV[index].HexToByte();
                }

                // Use the header and key to perform an XOR operation.
                string[] keys = key.SplitInParts(2).ToArray();

                for (int index = 0; index < keys.Length; index++)
                {
                    file[index] = (byte)(file[index] ^ keys[index].HexToByte());
                }

                // Save encrypted file.
                using (FileStream fs = new FileStream(savePath, FileMode.Create))
                {
                    fs.Write(header, 0, header.Length);
                    fs.Write(file, 0, file.Length);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Encrypted File Structure : RPG Maker MV Header(16Byte) -> Original Header that encrypted by XOR operation(16Byte) -> File Contents
        // Decryption Procedure : Remove 16Byte on the top -> Perform XOR operation with each byte of the key and the top 16 bytes

        /// <summary>
        /// Decrypts the header of the encrypted RPG Maker MV resource file.
        /// </summary>
        /// <param name="filePath">The path to the file to decrypt the header.</param>
        /// <param name="savePath">The path where the completed file will be saved.</param>
        /// <param name="key">The key to decrypt the file. Requires hex 128-bit value (MD5).</param>
        public static void DecryptHeader(string filePath, string savePath, string key)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("The file you are trying to decrypt header does not exist.");
                }

                // Checks extension.
                string extension = Path.GetExtension(filePath).ToLower();

                if (!EncryptedExtensions.Contains(extension))
                {
                    throw new NotSupportedException("Incompatible file format used.");
                }

                // Loads encrypted file and skips RPG Maker MV encryption header area.
                byte[] source = File.ReadAllBytes(filePath);
                byte[] file = source.Skip(16).ToArray();

                // Use the header and key to perform an XOR operation.
                string[] keys = key.SplitInParts(2).ToArray();

                for (int index = 0; index < keys.Length; index++)
                {
                    file[index] = (byte)(file[index] ^ keys[index].HexToByte());
                }

                // Save decrypted file.
                using (FileStream fs = new FileStream(savePath, FileMode.Create))
                {
                    fs.Write(file, 0, file.Length);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Encrypted File Structure : RPG Maker MV Header(16Byte) -> Original Header that encrypted by XOR operation(16Byte) -> File Contents
        // Restore Procedure : Remove 32Byte on the top -> Insert a 16-byte header from the original file at the top

        /// <summary>
        /// Recover the header of the files: *.rpgmvm, *.rpgmvw, *.rpgmvp.
        /// </summary>
        /// <param name="filePath">The path to the file to restore the header.</param>
        /// <param name="savePath">The path where the completed file will be saved.</param>
        public static void RestoreHeader(string filePath, string savePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("The file you are trying to decrypt header does not exist.");
                }

                // Checks extension.
                string extension = Path.GetExtension(filePath).ToLower();

                if (!EncryptedExtensions.Contains(extension) && extension == ".rpgmvo")
                {
                    throw new NotSupportedException("Incompatible file format used.");
                }

                // Finds and sets header.
                string[] headerHexArray = null;

                if (extension == ".rpgmvm")
                {
                    headerHexArray = HEADER_M4A;
                }
                else if (extension == ".rpgmvw")
                {
                    headerHexArray = HEADER_WAV;
                }
                else if (extension == ".rpgmvp")
                {
                    headerHexArray = HEADER_PNG;
                }

                // Writes a header as byte array.
                byte[] header = new byte[16];

                for (int index = 0; index < header.Length; index++)
                {
                    header[index] = headerHexArray[index].HexToByte();
                }

                // Loads encrypted file and skips RPG Maker MV encryption header area.
                byte[] file = File.ReadAllBytes(filePath);
                byte[] contents = file.Skip(32).ToArray();

                // Save decrypted file.
                using (FileStream fs = new FileStream(savePath, FileMode.Create))
                {
                    fs.Write(header, 0, header.Length);
                    fs.Write(contents, 0, contents.Length);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
