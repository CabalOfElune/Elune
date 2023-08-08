using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Runtime.Serialization;

namespace Elune.MPQ {
    /// <summary>
    /// Reads MPQ Files
    /// </summary>
    public class MPQReader {
        const string ID_MPQ_USERDATA = "MPQ\x1B";
        const int HEADER_SEARCH_INTERVAL = 0x200;

        /// <summary>
        /// TODO: Document this when it is being used correctly
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="MPQReadException"></exception>
        public static void LoadMPQ(string filePath) {
            FileStream dataStream; 
            try {
                dataStream = new(filePath, FileMode.Open);
            }
            catch (Exception ex) {
                // None of these specific exceptions need to be handled in a specific way at this stage.
                throw new MPQReadException("Error reading file", filePath, ex);
            }

            int headerPointer;
            try {
                headerPointer = FindHeaderPointer(dataStream);  
            } 
            catch (SignatureNotFoundException ex) {
                dataStream.Close();
                throw new MPQReadException("Failed to find header pointer", filePath, ex);
            }
            
            // Read header contents
            MPQHeader header = ReadHeaderData(dataStream, headerPointer);
            
            // TODO: At this point, we can start reading the contents of the MPQ.

            dataStream.Close();
        }

        public static int FindHeaderPointer(FileStream dataStream) {
            int searchIndex = 0;
            while(dataStream.Length >= searchIndex*HEADER_SEARCH_INTERVAL + MPQHeader.HEADER_SIZE_V1) {
                var headerPointer = searchIndex*HEADER_SEARCH_INTERVAL;
                // Try a position
                byte[] bytes = new byte[MPQHeader.ID_MPQ.Length];
                dataStream.Seek(headerPointer, SeekOrigin.Begin);
                dataStream.Read(bytes);
                var section = Encoding.UTF8.GetString(bytes);

                if(section.Equals(MPQHeader.ID_MPQ)) {
                    // Found it!
                    return headerPointer;
                }
                searchIndex++;
            }
            throw new SignatureNotFoundException("Could not MPQ header signature in file");
        }

        private static MPQHeader ReadHeaderData(FileStream dataStream, int? headerPointer)
        {
            dataStream.Seek(headerPointer.Value, SeekOrigin.Begin);
            byte[] headerData = new byte[MPQHeader.HEADER_SIZE_V1];
            dataStream.Read(headerData, 0, MPQHeader.HEADER_SIZE_V1);
            // SKIP 0x00 Signature (4 bytes)
            // SKIP 0x04 HeaderSize (4 bytes) 
            uint archiveSize = BitConverter.ToUInt32(headerData, 0x08); // 4 bytes
            ushort formatVersion = BitConverter.ToUInt16(headerData, 0x0C); // 2 bytes
            ushort blockSize = BitConverter.ToUInt16(headerData, 0x0E); // 2 bytes
            uint hashTableAddress = BitConverter.ToUInt32(headerData, 0x10); // 4 bytes
            uint blockTableAddress = BitConverter.ToUInt32(headerData, 0x14); // 4 bytes
            uint hashTableSize = BitConverter.ToUInt32(headerData, 0x18); // 4 bytes
            uint blockTableSize = BitConverter.ToUInt32(headerData, 0x1C); // 4 bytes

            MPQHeader header;
            if(formatVersion == 0) {
                header = MPQHeader.CreateVersion1(
                    archiveSize, 
                    blockSize, 
                    hashTableAddress, 
                    blockTableAddress, 
                    hashTableSize, 
                    blockTableSize
                );

                return header;
            }
            
            throw new UnexpectedFormatVersionException($"MPQ Format not supported", formatVersion);
        }

        [Serializable]
        private class MPQReadException : Exception
        {
            public readonly string filePath;

            public MPQReadException(string filePath)
            {
                this.filePath = filePath;
            }

            public MPQReadException(string message, string filePath) : base(message)
            {
                this.filePath = filePath;
            }

            public MPQReadException(string message, string filePath, Exception innerException) : base(message, innerException)
            {
                this.filePath = filePath;
            }

            protected MPQReadException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }

}
