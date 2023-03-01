using MView.Extensions;
using NAudio.MediaFoundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Utilities
{
    /// <summary>
    /// Provides cryptography functions for RPG Maker MV/MZ.
    /// </summary>
    internal static class CryptographyProvider
    {
        #region ::Constants::

        /// <summary>
        /// A list of file extensions that can be handled by MView.
        /// </summary>
        internal static readonly string[] EXTENSIONS = new string[] { ".png", ".ogg", ".m4a", ".wav", ".rpgmvp", ".rpgmvo", ".rpgmvm", ".rpgmvw", ".png_", ".ogg_", ".m4a_", ".wav_" };

        /// <summary>
        /// A list of extension pairs consisting of Key(Unencrypted) and Value(Encrypted).
        /// </summary>
        internal static readonly Dictionary<string, string> EXTENSION_PAIR_MV = new Dictionary<string, string>() { { ".png", ".rpgmvp" }, { ".ogg", "rpgmvo" }, { ".m4a", ".rpgmvm" }, { ".wav", ".rpgmvw" } };

        /// <summary>
        /// A list of extension pairs consisting of Key(Unencrypted) and Value(Encrypted).
        /// </summary>
        internal static readonly Dictionary<string, string> EXTENSION_PAIR_MZ = new Dictionary<string, string>() { { ".png", ".png_" }, { ".ogg", ".ogg_" }, { ".m4a", ".m4a_" }, { ".wav", ".wav_" } };

        /// <summary>
        /// The RMMV/RMMZ encrypted resource file header.
        /// </summary>
        private static readonly byte[] SIGNATURE = new byte[] { 0x52, 0x50, 0x47, 0x4D, 0x56, 0x00, 0x00, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// The PNG(Portable Network Graphics) file header.
        /// </summary>
        private static readonly byte[] GENERAL_HEADER_PNG = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52 };

        /// <summary>
        /// The OGG(OGG Vorbis) file header.
        /// </summary>
        private static readonly byte[] GENERAL_HEADER_OGG = new byte[] { 0x4F, 0x67, 0x67, 0x53, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// The M4A(MPEG-4 Part.14 audio) file header.
        /// </summary>
        private static readonly byte[] GENERAL_HEADER_M4A = new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x41, 0x20, 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// The WAV(Waveform audio format) file header.
        /// </summary>
        private static readonly byte[] GENERAL_HEADER_WAV = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x24, 0x3C, 0x00, 0x00, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20 };

        #endregion

        /// <summary>
        /// Verify that a file is a valid RPG Maker MV/MZ Encrypted Resource File.
        /// </summary>
        /// <param name="filePath">A absolute or relative path for the source file.</param>
        /// <returns>A verification result.</returns>
        internal static async Task<bool> VerifySignatureAsync(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Note: RMMV/RMMZ Encrypted Resource File signature -> RPGMV...........(16 Byte)
                byte[] targetBytes = new byte[16];
                stream.Position = 0;
                await stream.ReadAsync(targetBytes, 0, 16);

                return Enumerable.SequenceEqual(targetBytes, SIGNATURE);
            }
        }

        // Note: Structure ~ RMMV/RMMZ Signature(16 Byte) -> Original Header that encrypted by XOR operation(16 Byte) -> File Contents(n Byte)
        // Note: Procedure ~ Encrypt the source file header with XOR operation(16 Byte) -> Create a new file -> Insert the RMMV/RMMZ Signature(16 Byte) on the top -> Insert the encrypted source File

        /// <summary>
        /// Encrypt a file for RPG Maker MV/MZ.
        /// </summary>
        /// <param name="key">A encryption key to be used.</param>
        /// <param name="filePath">A absolute or relative path for the source file.</param>
        /// <param name="outputPath">A absolute or relative path where the encrypted file will be saved. If the value is null, it is saved to the source path.</param>
        /// <returns>A task that represents encrypt operation.</returns>
        /// <exception cref="ArgumentException">The key must be 32 characters long.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="FileFormatException">The file is already encrypted.</exception>
        internal static async Task EncryptAsync(string key, string filePath, string? outputPath = null)
        {
            // Check the length of the key.
            if (key.Length != 32)
            {
                throw new ArgumentException("The key must be 32 characters long.");
            }

            // Check the file exists.
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file does not exist.");
            }

            // Check if it is already encrypted.
            if (await VerifySignatureAsync(filePath))
            {
                throw new FileFormatException("The file is already encrypted.");
            }

            // Create key bytes.
            byte[] keyBytes = new byte[16];

            int index = 0;

            foreach (string keyString in key.SplitInParts(2))
            {
                keyBytes[index++] = keyString.HexToByte();
            }

            // IF: outputPath == null -> output to the source file.
            if (string.IsNullOrEmpty(outputPath))
            {
                using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                using (MemoryStream tempStream = new MemoryStream())
                {
                    // Copy the source stream.
                    tempStream.Position = 0;
                    tempStream.SetLength(sourceStream.Length);

                    sourceStream.Position = 0;
                    await sourceStream.CopyToAsync(tempStream, 4096);

                    // Encrypt with key bytes.
                    tempStream.Position = 0;

                    byte[] bytesToEncrypt = new byte[16];
                    await tempStream.ReadAsync(bytesToEncrypt, 0, 16);

                    for (int i = 0; i < 16; i++)
                    {
                        bytesToEncrypt[i] ^= keyBytes[i];
                    }

                    tempStream.Position = 0;
                    await tempStream.WriteAsync(bytesToEncrypt, 0, 16);

                    // Expand the source stream (+ 16 Byte).
                    sourceStream.SetLength(sourceStream.Length + 16);

                    // Write to the source stream.
                    sourceStream.Position = 0;
                    tempStream.Position = 0;

                    await sourceStream.WriteAsync(SIGNATURE, 0, 16);
                    await tempStream.CopyToAsync(sourceStream, 4096);

                    // Flush.
                    await sourceStream.FlushAsync();
                }
            }
            else
            {
                if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
                }

                using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    // Set the output stream length.
                    outputStream.SetLength(sourceStream.Length + 16);

                    // Copy bytes.
                    sourceStream.Position = 0;
                    outputStream.Position = 0;

                    await outputStream.WriteAsync(SIGNATURE, 0, 16);
                    await sourceStream.CopyToAsync(outputStream, 4096);

                    // Encrypt with key bytes.
                    outputStream.Position = 16;

                    byte[] bytesToEncrypt = new byte[16];
                    await outputStream.ReadAsync(bytesToEncrypt, 0, 16);

                    for (int i = 0; i < 16; i++)
                    {
                        bytesToEncrypt[i] ^= keyBytes[i];
                    }

                    outputStream.Position = 16;
                    await outputStream.WriteAsync(bytesToEncrypt, 0, 16);
                    await outputStream.FlushAsync();
                }
            }
        }

        // Note: Structure ~ RMMV/RMMZ Signature(16 Byte) -> Original Header that encrypted by XOR operation(16 Byte) -> File Contents(n Byte)
        // Note: Procedure ~ Remove the first 16 Byte -> Perform XOR operation to the first 16 Byte with key bytes.

        /// <summary>
        /// Decrypt a RPG Maker MV/MZ encrypted resource file.
        /// </summary>
        /// <param name="key">A encryption key to be used.</param>
        /// <param name="filePath">A absolute or relative path for the source file.</param>
        /// <param name="outputPath">A absolute or relative path where the decrypted file will be saved. If the value is null, it is saved to the source path.</param>
        /// <returns>A task that represents decrypt operation.</returns>
        /// <exception cref="ArgumentException">The key must be 32 characters long.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="FileFormatException">The file is already decrypted.</exception>
        internal static async Task DecryptAsync(string key, string filePath, string? outputPath = null)
        {
            // Check the length of the key.
            if (key.Length != 32)
            {
                throw new InvalidDataException("The key must be 32 characters long.");
            }

            // Check the file exists.
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file does not exist.");
            }

            // Check if it is already decrypted.
            if (!await VerifySignatureAsync(filePath))
            {
                throw new FileFormatException("The file is already decrypted.");
            }

            // Create key bytes.
            byte[] keyBytes = new byte[16];

            int index = 0;

            foreach (string keyString in key.SplitInParts(2))
            {
                keyBytes[index++] = keyString.HexToByte();
            }

            // IF: outputPath == null -> output to the source file.
            if (string.IsNullOrEmpty(outputPath))
            {
                using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                using (MemoryStream tempStream = new MemoryStream())
                {
                    // Copy the source stream.
                    tempStream.Position = 0;
                    tempStream.SetLength(sourceStream.Length - 16);

                    sourceStream.Position = 16;
                    await sourceStream.CopyToAsync(tempStream, 4096);

                    // Decrypt with key bytes.
                    tempStream.Position = 0;

                    byte[] bytesToDecrypt = new byte[16];
                    await tempStream.ReadAsync(bytesToDecrypt, 0, 16);

                    for (int i = 0; i < 16; i++)
                    {
                        bytesToDecrypt[i] ^= keyBytes[i];
                    }

                    tempStream.Position = 0;
                    await tempStream.WriteAsync(bytesToDecrypt, 0, 16);

                    // Expand the source stream (- 16 Byte).
                    sourceStream.SetLength(sourceStream.Length - 16);

                    // Write to the source stream.
                    sourceStream.Position = 0;
                    tempStream.Position = 0;

                    await tempStream.CopyToAsync(sourceStream, 4096);

                    // Flush.
                    await sourceStream.FlushAsync();
                }
            }
            else
            {
                if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
                }

                using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    // Set the output stream length.
                    outputStream.SetLength(sourceStream.Length - 16);

                    // Copy bytes.
                    sourceStream.Position = 16;
                    outputStream.Position = 0;

                    await sourceStream.CopyToAsync(outputStream, 4096);

                    // Decrypt with key bytes.
                    outputStream.Position = 0;

                    byte[] bytesToDecrypt = new byte[16];
                    await outputStream.ReadAsync(bytesToDecrypt, 0, 16);

                    for (int i = 0; i < 16; i++)
                    {
                        bytesToDecrypt[i] ^= keyBytes[i];
                    }

                    outputStream.Position = 0;
                    await outputStream.WriteAsync(bytesToDecrypt, 0, 16);
                    await outputStream.FlushAsync();
                }
            }
        }

        /// <summary>
        /// Estimate the encryption key from the RPG Maker MV/MZ encrypted resource file.
        /// </summary>
        /// <param name="filePath">A absolute or relative path for the source file.</param>
        /// <returns>A estimated encryption key.</returns>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="FileFormatException">The file is not encrypted. Unable to estimate the encryption key.</exception>
        /// <exception cref="NotSupportedException">An incompatible file is inputted.</exception>
        internal static async Task<string> EstimateKeyAsync(string filePath)
        {
            // Check the file exists.
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file does not exist.");
            }

            // Check if it is encrypted.
            if (!await VerifySignatureAsync(filePath))
            {
                throw new FileFormatException("The file is not encrypted. Unable to estimate the encryption key.");
            }

            // Select the general header.
            string extension = Path.GetExtension(filePath).ToLower();

            byte[] header;

            switch (extension)
            {
                case ".rpgmvp":
                case ".png_":
                    header = GENERAL_HEADER_PNG;
                    break;
                case ".rpgmvm":
                case ".m4a_":
                    header = GENERAL_HEADER_M4A;
                    break;
                case ".rpgmvw":
                case ".wav_":
                    header = GENERAL_HEADER_WAV;
                    break;
                default:
                    throw new NotSupportedException("An incompatible file is inputted.");
            }

            // Estimate the encryption key.
            using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Copy encrypted bytes.
                byte[] targetBytes = new byte[16];

                sourceStream.Position = 16;
                await sourceStream.ReadAsync(targetBytes, 0, 16);

                // Calculate an original key.
                byte[] keyBytes = new byte[16];

                for (int i = 0; i < 16; i++)
                {
                    keyBytes[i] = (byte)(targetBytes[i] ^ header[i]);
                }

                return keyBytes.ByteArrayToString();
            }
        }

        // Note: Structure ~ RMMV/RMMZ Signature(16 Byte) -> Original Header that encrypted by XOR operation(16 Byte) -> File Contents(n Byte)
        // Note: Procedure ~ Remove the first 32 Byte -> Insert a general header(16 Byte) at the beginning of the file

        /// <summary>
        /// Restore the encyrpted file(PNG, M4A, WAV).
        /// </summary>
        /// <param name="filePath">A absolute or relative path for the source file.</param>
        /// <returns>A memory stream containing the restored file.</returns>
        /// <exception cref="NotSupportedException">An incompatible file is inputted.</exception>
        private static async Task<Stream> RestoreInternalAsync(string filePath)
        {
            MemoryStream outputStream = new MemoryStream();

            using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Copy the file.
                outputStream.SetLength(sourceStream.Length - 16);

                sourceStream.Position = 16;
                outputStream.Position = 0;
                await sourceStream.CopyToAsync(outputStream, 4096);

                // Select the general header.
                string extension = Path.GetExtension(filePath).ToLower();

                byte[] generalHeader;

                switch (extension)
                {
                    case ".rpgmvp":
                    case ".png_":
                        generalHeader = GENERAL_HEADER_PNG;
                        break;
                    case ".rpgmvm":
                    case ".m4a_":
                        generalHeader = GENERAL_HEADER_M4A;
                        break;
                    case ".rpgmvw":
                    case ".wav_":
                        generalHeader = GENERAL_HEADER_WAV;
                        break;
                    default:
                        throw new NotSupportedException("An incompatible file is inputted.");
                }

                // Fill with the general header.
                outputStream.Position = 0;
                await outputStream.WriteAsync(generalHeader, 0, 16);
                await outputStream.FlushAsync();
            }

            return outputStream;
        }

        // Note: Structure ~ RMMV/RMMZ Signature(16 Byte) -> Original Header that encrypted by XOR operation(16 Byte) -> File Contents(n Byte)
        // Note: OGG Structure ~ Signature(4 Byte)[OggS] -> Version(1 Byte)[0x00] -> Flags(1 Byte)[0x02, Beginning Of Stream] -> GranulePosition(8 Byte)[00000000]
        // -> SerialNumber(4 Byte)[Random] -> Checksum(4 Byte) -> TotalSegments(1 Byte)
        // Note: Procedure ~ Remove the first 32 Byte -> Insert a general header(14 Byte) at the beginning of the file -> Fill in the next 2-byte with random values.

        /// <summary>
        /// Restore the encyrpted OGG file.
        /// </summary>
        /// <param name="filePath">A absolute or relative path for the source file.</param>
        /// <returns>A memory stream containing the restored file.</returns>
        /// <exception cref="FileFormatException">Failed to parse the OGG header.</exception>
        private static async Task<Stream> RestoreOggInternalAsync(string filePath)
        {
            MemoryStream outputStream = new MemoryStream();

            using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int offset = 0;

                // Get the total segments value.
                offset += 42;
                sourceStream.Position = offset;

                int totalSegments = 1;
                totalSegments = sourceStream.ReadByte();

                // Get the OGG signature bytes.
                offset += 30 + totalSegments * 2;
                sourceStream.Position = offset;

                byte[] oggSignatureBytes = new byte[4];
                await sourceStream.ReadAsync(oggSignatureBytes, 0, 4);

                // Check the OGG signature.
                if (Encoding.ASCII.GetString(oggSignatureBytes) != "OggS")
                {
                    throw new FileFormatException("Failed to parse the OGG header.");
                }

                // Get the serial number.
                offset += 14;
                sourceStream.Position = offset;

                byte[] serialNumber = new byte[4]; // Note: The type of serial number is UInt32.
                await sourceStream.ReadAsync(serialNumber, 0, 4);

                if (BitConverter.IsLittleEndian)
                {
                    if (serialNumber[0] < serialNumber[3])
                    {
                        Array.Reverse(serialNumber); // Convert to LE.
                    }
                }
                else
                {
                    if (serialNumber[0] > serialNumber[3])
                    {
                        Array.Reverse(serialNumber); // Convert to LE.
                    }
                }

                // Compose the header.
                byte[] header = new byte[18];
                Buffer.BlockCopy(GENERAL_HEADER_OGG, 0, header, 0, 14);
                Buffer.BlockCopy(serialNumber, 0, header, 14, 4);

                // Copy the file.
                outputStream.SetLength(sourceStream.Length - 16);

                outputStream.Position = 0;
                sourceStream.Position = 16;
                await sourceStream.CopyToAsync(outputStream, 4096);

                // Overwrite the header.
                outputStream.Position = 0;
                await outputStream.WriteAsync(header, 0, 18);
                await outputStream.FlushAsync();
            }

            return outputStream;
        }


        internal static async Task RestoreAsync(string filePath, string? outputPath = null)
        {
            // Check the file exists.
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file does not exist.");
            }

            // Check if it is already decrypted.
            if (!await VerifySignatureAsync(filePath))
            {
                throw new FileFormatException("The file is already decrypted.");
            }

            Stream sourceStream = Stream.Null;

            try
            {
                // Select a stream provider.
                string extension = Path.GetExtension(filePath).ToLower();

                switch (extension)
                {
                    case ".rpgmvp":
                    case ".png_":
                    case ".rpgmvm":
                    case ".m4a_":
                    case ".rpgmvw":
                    case ".wav_":
                        sourceStream = await RestoreInternalAsync(filePath);
                        break;
                    case ".rpgmvo":
                    case ".ogg_":
                        sourceStream = await RestoreOggInternalAsync(filePath);
                        break;
                    default:
                        throw new NotSupportedException("An incompatible file is inputted.");
                }

                // IF: outputPath == null -> output to the source file.
                if (string.IsNullOrEmpty(outputPath))
                {
                    using (FileStream outputStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        sourceStream.Position = 0;
                        outputStream.Position = 0;
                        outputStream.SetLength(sourceStream.Length);
                        await sourceStream.CopyToAsync(outputStream, 4096);
                        await outputStream.FlushAsync();
                    }
                }
                else
                {
                    using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        sourceStream.Position = 0;
                        outputStream.Position = 0;
                        outputStream.SetLength(sourceStream.Length);
                        await sourceStream.CopyToAsync(outputStream, 4096);
                        await outputStream.FlushAsync();
                    }
                }
            }
            finally
            {
                sourceStream.Close();
            }
        }

        internal static async Task<Stream> RestoreAndGetAsync(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".rpgmvp":
                case ".png_":
                case ".rpgmvm":
                case ".m4a_":
                case ".rpgmvw":
                case ".wav_":
                    return await RestoreInternalAsync(filePath);
                    break;
                case ".rpgmvo":
                case ".ogg_":
                    return await RestoreOggInternalAsync(filePath);
                    break;
                default:
                    throw new NotSupportedException("An incompatible file is inputted.");
            }
        }
    }
}
