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

        public static void LoadMPQ(string filePath) {
            // Guard! Don't accept nonexistent files.
            if(!File.Exists(filePath)) {
                throw new FileNotFoundException(filePath);
            }

            FileStream dataStream = new(filePath, FileMode.Open);

            int? headerPointer = FindHeaderPointer(dataStream);

            if(headerPointer == null) {
                dataStream.Close();
                return;
            }
            
            // Read header contents
            MPQHeader header = ReadHeaderData(dataStream, headerPointer);

            // At this point, we can start reading the contents of the MPQ.

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
                // 0x00 Signature (4 bytes)
                // 0x04 HeaderSize (4 bytes) 
            uint archiveSize = BitConverter.ToUInt32(headerData, 0x08); // 4 bytes
            ushort formatVersion = BitConverter.ToUInt16(headerData, 0x0C); // 2 bytes
            ushort blockSize = BitConverter.ToUInt16(headerData, 0x0E); // 2 bytes
            uint hashTableAddress = BitConverter.ToUInt32(headerData, 0x10); // 4 bytes
            uint blockTableAddress = BitConverter.ToUInt32(headerData, 0x14); // 4 bytes
            uint hashTableSize = BitConverter.ToUInt32(headerData, 0x18); // 4 bytes
            uint blockTableSize = BitConverter.ToUInt32(headerData, 0x1C); // 4 bytes

            // TODO: Handle errors if the FormatVersion is not 0
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
            
            throw new UnexpectedFormatVersionException($"MPQ Format 0x{formatVersion:x2} not supported");
        }
    }
}