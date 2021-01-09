using MView.Core.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MView.Core.Cryptography
{
    /// <summary>
    /// Provides tools related to encrypted resource files of RPG Maker MV/MZ.
    /// </summary>
    public static class CryptographyProvider
    {
        #region ::Consts::

        /// <summary>
        /// RPG MV encrypted resource file extensions.
        /// </summary>
        public static readonly string[] EncryptedExtensions = new string[] { ".rpgmvo", ".rpgmvm", ".rpgmvw", ".rpgmvp", ".ogg_", ".m4a_", ".wav_", ".png_" };

        /// <summary>
        /// RPG MV decrypted resource file extensions.
        /// </summary>
        public static readonly string[] DecryptedExtensions = new string[] { ".ogg", ".m4a", ".wav", ".png", ".ogg", ".m4a", ".wav", ".png" };

        /// <summary>
        /// RPG MV encrypted resource file header.
        /// </summary>
        private static readonly string[] HEADER_MV = new string[] { "52", "50", "47", "4D", "56", "00", "00", "00", "00", "03", "01", "00", "00", "00", "00", "00" };

        /// <summary>
        /// OGG Vorbis file header.
        /// </summary>
        private static readonly string[] HEADER_OGG = new string[] { "4F", "67", "67", "53", "00", "02", "00", "00", "00", "00", "00", "00", "00", "00" };

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
            catch (Exception)
            {
                throw;
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

                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
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
            catch (Exception)
            {
                throw;
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

                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
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
            catch (Exception)
            {
                throw;
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

                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                }

                // Checks extension.
                string extension = Path.GetExtension(filePath).ToLower();

                if (!EncryptedExtensions.Contains(extension) && (extension == ".rpgmvo" || extension == ".ogg_"))
                {
                    throw new NotSupportedException("Incompatible file format used.");
                }

                // Finds and sets header.
                string[] headerHexArray = null;

                if (extension == ".rpgmvm" || extension == ".m4a_")
                {
                    headerHexArray = HEADER_M4A;
                }
                else if (extension == ".rpgmvw" || extension == ".wav_")
                {
                    headerHexArray = HEADER_WAV;
                }
                else if (extension == ".rpgmvp" || extension == ".png_")
                {
                    headerHexArray = HEADER_PNG;
                }
                else
                {
                    throw new NotSupportedException("Not supported file extension.");
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
            catch (Exception)
            {
                throw;
            }
        }

        // Encrypted File Structure : RPG Maker MV Header(16Byte) -> Original Header that encrypted by XOR operation(16Byte) -> File Contents

        // Ogg File Structure : Signature(4Byte)[OggS] -> Version(1Byte)[0x00] -> Flags(1Byte)[0x02, Beginning Of Stream] -> GranulePosition(8Byte)[00000000]
        // -> SerialNumber(4Byte)[Random] -> Checksum(4Byte) -> TotalSegments(1Byte)

        // Restore Procedure : Remove 32Byte on the top -> Insert a 14Byte header from the original file at the top -> Rest 2Bytes are filled randomly

        /// <summary>
        /// Recover the header of the files: *.rpgmvo.
        /// </summary>
        /// <param name="filePath">The path to the file to restore the header.</param>
        /// <param name="savePath">The path where the completed file will be saved.</param>
        public static void RestoreOggHeader(string filePath, string savePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("The file you are trying to decrypt header does not exist.");
                }

                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                }

                // Checks extension.
                string extension = Path.GetExtension(filePath).ToLower();

                if (extension != ".rpgmvo" && extension != ".ogg_")
                {
                    throw new NotSupportedException("Incompatible file format used.");
                }

                byte[] file = File.ReadAllBytes(filePath);

                int offset = 0;
                offset += 16; // Skip RPG MV Header.

                var header = (Signature: "", SerialNumber: 0, TotalSegments: 1);

                // OGG Header.
                offset += 4;
                offset += 22;
                header.TotalSegments = file.ReadByte(offset);

                offset += 2;
                offset += header.TotalSegments;

                // OGG Vorbis Header.
                offset += 27;
                offset += 1;
                offset += header.TotalSegments;

                bool isLittleEndian = BitConverter.IsLittleEndian;

                // OGG Data Header.
                byte[] signatureBytes = file.ReadByte(offset, 4);
                header.Signature = Encoding.ASCII.GetString(signatureBytes);

                if (header.Signature == "OggS")
                {
                    offset += 4;
                }
                else
                {
                    throw new InvalidDataException("Failed to parse the OGG Header.");
                }

                // Serial number.
                offset += 2;
                offset += 8;

                byte[] serialNumber = isLittleEndian ? file.ReadUInt32LE(offset) : file.ReadUInt32BE(offset);

                // Sort serial number to LE or BE.
                if (isLittleEndian)
                {
                    Array.Reverse(serialNumber);
                }
                else
                {
                    Array.Sort(serialNumber);
                }

                // Convert string array to byte array.
                byte[] oggSignature = new byte[14];

                for (int index = 0; index < oggSignature.Length; index++)
                {
                    oggSignature[index] = HEADER_OGG[index].HexToByte();
                }

                // Concat header.
                byte[] oggHeader = new byte[oggSignature.Length + serialNumber.Length];
                Array.Copy(oggSignature, 0, oggHeader, 0, oggSignature.Length);
                Array.Copy(serialNumber, 0, oggHeader, oggSignature.Length, serialNumber.Length);

                // Loads encrypted file and skips RPG Maker MV encryption header area.
                int contentsOffset = serialNumber.Length - 2;

                byte[] contents = file.Skip(32 + contentsOffset).ToArray();

                // Save decrypted file.
                using (FileStream fs = new FileStream(savePath, FileMode.Create))
                {
                    fs.Write(oggHeader, 0, oggHeader.Length);
                    fs.Write(contents, 0, contents.Length);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Recover the header of the files: *.rpgmvm, *.rpgmvw, *.rpgmvp. And returns a restored file.
        /// </summary>
        /// <param name="filePath">The path to the file to restore the header.</param>
        public static byte[] GetRestoredFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("The file you are trying to decrypt header does not exist.");
                }

                // Checks extension.
                string extension = Path.GetExtension(filePath).ToLower();

                if (!EncryptedExtensions.Contains(extension) && (extension == ".rpgmvo" || extension == ".ogg_"))
                {
                    throw new NotSupportedException("Incompatible file format used.");
                }

                // Finds and sets header.
                string[] headerHexArray = null;

                if (extension == ".rpgmvm" || extension == ".m4a_")
                {
                    headerHexArray = HEADER_M4A;
                }
                else if (extension == ".rpgmvw" || extension == ".wav_")
                {
                    headerHexArray = HEADER_WAV;
                }
                else if (extension == ".rpgmvp" || extension == ".png_")
                {
                    headerHexArray = HEADER_PNG;
                }
                else
                {
                    throw new NotSupportedException("Not supported file extension.");
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

                // Copy and return decrypted file.
                byte[] result = new byte[header.Length + contents.Length];
                Array.Copy(header, 0, result, 0, header.Length);
                Array.Copy(contents, 0, result, header.Length, contents.Length);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}