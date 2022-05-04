using MView.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Utilities
{
    /// <summary>
    /// Provides cryptography tools for encrypted resource files of RPG Maker MV/MZ.
    /// </summary>
    public static class CryptographyProvider
    {
        #region ::Constants::

        /// <summary>
        /// RPG MV/MZ decrypted resource file extensions.
        /// </summary>
        public static readonly string[] EXTENSIONS_DECRYPTED = new string[] { ".png", ".ogg", ".m4a", ".wav", ".png", ".ogg", ".m4a", ".wav" };

        /// <summary>
        /// RPG MV/MZ encrypted resource file extensions.
        /// </summary>
        public static readonly string[] EXTENSIONS_ENCRYPTED = new string[] { ".rpgmvp", ".rpgmvo", ".rpgmvm", ".rpgmvw", ".png_", ".ogg_", ".m4a_", ".wav_" };

        /// <summary>
        /// The RPG MV/MZ encrypted resource file header.
        /// </summary>
        private static readonly byte[] HEADER_MV = new byte[] { 0x52, 0x50, 0x47, 0x4D, 0x56, 0x00, 0x00, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// The PNG(Portable Network Graphics) file header.
        /// </summary>
        private static readonly byte[] HEADER_PNG = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52 };

        /// <summary>
        /// The OGG(OGG Vorbis) file header.
        /// </summary>
        private static readonly byte[] HEADER_OGG = new byte[] { 0x4F, 0x67, 0x67, 0x53, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// The M4A(MPEG-4 Part.14 audio) file header.
        /// </summary>
        private static readonly byte[] HEADER_M4A = new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x41, 0x20, 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// The WAV(Waveform audio format) file header.
        /// </summary>
        private static readonly byte[] HEADER_WAV = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x24, 0x3C, 0x00, 0x00, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20 };

        #endregion

        /// <summary>
        /// Verify that there is a fake header.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <returns>Validity of the file</returns>
        public static bool VerifyFakeHeader(Stream stream)
        {
            // Get the RMMV signature area.
            byte[] targetBytes = new byte[16];
            stream.Position = 0;
            stream.Read(targetBytes, 0, 16);

            // Compare bytes.
            for (int index = 0; index < 16; index++)
            {
                if (targetBytes[index] != HEADER_MV[index])
                {
                    return false;
                }
            }

            return true;
        }

        // Encrypted File Structure : RPG Maker MV Header(16Byte) -> Original Header that encrypted by XOR operation(16Byte) -> File Contents
        // Encryption Procedure : Encrypt original file header with XOR operation(16Byte) -> Create new file -> Insert RPG Maker MV Header(16Byte) on the top -> Insert Encrypted original File

        /// <summary>
        /// Encrypt a header of a RPG Maker MV/MZ resource file.
        /// </summary>
        /// <param name="filePath">The path to the file to encrypt.</param>
        /// <param name="outputPath">The path where the file is saved.</param>
        /// <param name="key">The key to encrypt the file. It requires MD5 hash.</param>
        public static void Encrypt(string filePath, string outputPath, string key)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file does not exist.");
            }

            if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            }

            // Check the length of the key.
            if (key.Length != 32)
            {
                throw new InvalidDataException("The key must be 32 characters long.");
            }

            using (FileStream originalStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite))
            {
                // Copy the file and inject the RMMV header.
                outputStream.SetLength(originalStream.Length + HEADER_MV.Length);

                outputStream.Position = 0;
                outputStream.Write(HEADER_MV, 0, HEADER_MV.Length);

                originalStream.Position = 0;
                originalStream.CopyTo(outputStream, 4096);

                // Make key bytes.
                byte[] keyBytes = new byte[16];

                int index = 0;

                foreach (string keyString in key.SplitInParts(2))
                {
                    keyBytes[index++] = keyString.HexToByte();
                }

                // Encrypt 16 bytes.
                byte[] targetBytes = new byte[16];
                outputStream.Position = HEADER_MV.Length;
                outputStream.Read(targetBytes, 0, 16);

                outputStream.Position = HEADER_MV.Length;

                for (int i = 0; i < keyBytes.Length; i++)
                {
                    outputStream.WriteByte((byte)(targetBytes[i] ^ keyBytes[i]));
                }
            }
        }

        // Encrypted File Structure : RPG Maker MV Header(16Byte) -> Original Header that encrypted by XOR operation(16Byte) -> File Contents
        // Decryption Procedure : Remove 16Byte on the top -> Perform XOR operation with each byte of the key and the top 16 bytes

        /// <summary>
        /// Decrypt a header of a encrypted RPG Maker MV/MZ resource file.
        /// </summary>
        /// <param name="filePath">The path to the file to decrypt.</param>
        /// <param name="outputPath">The path where the file is saved.</param>
        /// <param name="key">The key to encrypt the file. It requires MD5 hash.</param>
        public static void Decrypt(string filePath, string outputPath, string key)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file does not exist.");
            }

            if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            }

            // Check the length of the key.
            if (key.Length != 32)
            {
                throw new InvalidDataException("The key must be 32 characters long.");
            }

            using (FileStream originalStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite))
            {
                // Copy the file.
                outputStream.SetLength(originalStream.Length - HEADER_MV.Length);

                originalStream.Position = HEADER_MV.Length;
                outputStream.Position = 0;
                originalStream.CopyTo(outputStream, 4096);

                // Make key bytes.
                byte[] keyBytes = new byte[16];

                int index = 0;

                foreach (string keyString in key.SplitInParts(2))
                {
                    keyBytes[index++] = keyString.HexToByte();
                }

                // Encrypt 16 bytes.
                byte[] targetBytes = new byte[16];
                outputStream.Position = 0;
                outputStream.Read(targetBytes, 0, 16);

                outputStream.Position = 0; // Back to the zero.

                for (int i = 0; i < keyBytes.Length; i++)
                {
                    outputStream.WriteByte((byte)(targetBytes[i] ^ keyBytes[i]));
                }
            }
        }

        public static string Estimate(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file does not exist.");
            }

            using (FileStream originalStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // Copy encrypted bytes.
                byte[] targetBytes = new byte[16];

                originalStream.Position = 16;
                originalStream.Read(targetBytes, 0, 16);

                // Select the general header.
                string extension = Path.GetExtension(filePath).ToLower();

                byte[] header;

                if (extension == ".rpgmvp" || extension == ".png_")
                {
                    header = HEADER_PNG;
                }
                else if (extension == ".rpgmvm" || extension == ".m4a_")
                {
                    header = HEADER_M4A;
                }
                else if (extension == ".rpgmvw" || extension == ".wav_")
                {
                    header = HEADER_WAV;
                }
                else
                {
                    throw new NotSupportedException("Incompatible file format is used.");
                }

                // Calculate a original key.
                byte[] keyBytes = new byte[16];

                for (int i = 0; i < 16; i++)
                {
                    keyBytes[i] = (byte)(targetBytes[i] ^ header[i]);
                }

                string key = keyBytes.ByteArrayToString();

                return key;
            }
        }

        /// <summary>
        /// Restore a header of a encrypted RPG Maker MV/MZ resource file.
        /// </summary>
        /// <param name="filePath">The path to the file to restore.</param>
        /// <param name="outputPath">The path where the file is saved.</param>
        /// <exception cref="NotSupportedException"></exception>
        public static void Restore(string filePath, string outputPath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            Stream stream = Stream.Null;

            try
            {
                if (extension == ".rpgmvp" || extension == ".rpgmvm" || extension == ".rpgmvw" || extension == ".png" || extension == ".m4a" || extension == ".wav")
                {
                    stream = RestoreInternal(filePath);
                }
                else if (extension == ".rpgmvo" || extension == ".ogg_")
                {
                    stream = RestoreOggInternal(filePath);
                }
                else
                {
                    throw new NotSupportedException("Incompatible file format is used.");
                }

                using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    outputStream.SetLength(stream.Length);
                    stream.CopyTo(outputStream);
                }
            }
            finally
            {
                stream.Dispose();
            }
        }

        /// <summary>
        /// Restore a header of a encrypted RPG Maker MV/MZ resource file.
        /// </summary>
        /// <param name="filePath">The path to the file to restore.</param>
        /// <exception cref="NotSupportedException"></exception>
        public static Stream GetRestoredFileStream(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            Stream stream = Stream.Null;

            try
            {
                if (extension == ".rpgmvp" || extension == ".rpgmvm" || extension == ".rpgmvw" || extension == ".png" || extension == ".m4a" || extension == ".wav")
                {
                    stream = RestoreInternal(filePath);
                }
                else if (extension == ".rpgmvo" || extension == ".ogg_")
                {
                    stream = RestoreOggInternal(filePath);
                }
                else
                {
                    throw new NotSupportedException("Incompatible file format is used.");
                }

                return stream;
            }
            finally
            {
                stream.Dispose();
            }
        }

        // Encrypted File Structure : RPG Maker MV Header(16Byte) -> Original Header that encrypted by XOR operation(16Byte) -> File Contents
        // Restore Procedure : Remove 32Byte on the top -> Insert a 16-byte header from the original file at the top

        /// <summary>
        /// Restore a header of a file: *.rpgmvm, *.rpgmvw, *.rpgmvp.
        /// </summary>
        /// <param name="filePath">The path to the file to restore.</param>
        private static Stream RestoreInternal(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file does not exist.");
            }

            MemoryStream outputStream = new MemoryStream();

            using (FileStream originalStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // Copy the file.
                outputStream.SetLength(originalStream.Length - HEADER_MV.Length);

                originalStream.Position = HEADER_MV.Length;
                outputStream.Position = 0;
                originalStream.CopyTo(outputStream, 4096);

                // Select the general header.
                string extension = Path.GetExtension(filePath).ToLower();

                byte[] header;

                if (extension == ".rpgmvp" || extension == ".png_")
                {
                    header = HEADER_PNG;
                }
                else if (extension == ".rpgmvm" || extension == ".m4a_")
                {
                    header = HEADER_M4A;
                }
                else if (extension == ".rpgmvw" || extension == ".wav_")
                {
                    header = HEADER_WAV;
                }
                else
                {
                    throw new NotSupportedException("Incompatible file format is used.");
                }

                // Fill with the general header.
                outputStream.Position = 0;
                outputStream.Write(header, 0, header.Length);

                return outputStream;
            }
        }

        // Encrypted File Structure : RPG Maker MV Header(16Byte) -> Original Header that encrypted by XOR operation(16Byte) -> File Contents

        // Ogg File Structure : Signature(4Byte)[OggS] -> Version(1Byte)[0x00] -> Flags(1Byte)[0x02, Beginning Of Stream] -> GranulePosition(8Byte)[00000000]
        // -> SerialNumber(4Byte)[Random] -> Checksum(4Byte) -> TotalSegments(1Byte)

        // Restore Procedure : Remove 32Byte on the top -> Insert a 14Byte header from the original file at the top -> Rest 2Bytes are filled randomly

        /// <summary>
        /// Restore a header of a file: *.rpgmvo.
        /// </summary>
        /// <param name="filePath">The path to the file to restore.</param>
        private static Stream RestoreOggInternal(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file does not exist.");
            }

            MemoryStream outputStream = new MemoryStream();

            using (FileStream originalStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // Check the extension.
                string extension = Path.GetExtension(filePath).ToLower();

                if (extension != ".rpgmvo" && extension != ".ogg_")
                {
                    throw new NotSupportedException("Incompatible file format is used.");
                }

                // A tuple for OGG datas.
                var headerData = (Signature: string.Empty, SerialNumber: 0, TotalSegments: 1);

                int offset = 0;
                offset += 16; // Skip the RMMV header.

                // OGG Header.
                offset += 4;
                offset += 22;

                originalStream.Position = offset;
                headerData.TotalSegments = originalStream.ReadByte();

                offset += 2;
                offset += headerData.TotalSegments;

                // OGG Vorbis Header.
                offset += 27;
                offset += 1;
                offset += headerData.TotalSegments;

                bool isLittleEndian = BitConverter.IsLittleEndian;

                // OGG Data Header.
                byte[] signatureBytes = new byte[4];
                originalStream.Position = offset;
                originalStream.Read(signatureBytes, 0, 4);

                headerData.Signature = Encoding.ASCII.GetString(signatureBytes);

                if (headerData.Signature == "OggS")
                {
                    offset += 4;
                }
                else
                {
                    throw new InvalidDataException("Failed to parse the OGG Header.");
                }

                // Serial Number.
                offset += 2;
                offset += 8;

                byte[] serialNumber = new byte[4]; // UInt32
                originalStream.Position = offset;
                originalStream.Read(serialNumber, 0, 4);

                if (isLittleEndian)
                {
                    Array.Reverse(serialNumber); // LE
                }
                else
                {
                    Array.Sort(serialNumber); // BE
                }

                // Compose the header.
                byte[] header = new byte[HEADER_OGG.Length + serialNumber.Length];
                Array.Copy(HEADER_OGG, 0, header, 0, HEADER_OGG.Length);
                Array.Copy(serialNumber, 0, header, HEADER_OGG.Length, serialNumber.Length);

                // Copy the file.
                outputStream.SetLength(originalStream.Length - HEADER_MV.Length);

                originalStream.Position = HEADER_MV.Length;
                outputStream.Position = 0;
                originalStream.CopyTo(outputStream, 4096);

                // Fix the header.
                outputStream.Position = 0;
                outputStream.Write(header, 0, header.Length);

                return outputStream;
            }
        }
    }
}
