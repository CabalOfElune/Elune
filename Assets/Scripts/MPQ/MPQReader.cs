using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Collections;

namespace Elune.MPQ {
    /// <summary>
    /// Reads MPQ Files
    /// </summary>
    public class MPQReader {
        const string ID_MPQ_USERDATA = "MPQ\x1B";
        const int HASH_ENTRY_SIZE = 0x10;
        const int HEADER_SEARCH_INTERVAL = 0x200;
        public const string DEFAULT_HASHTABLE_KEYSTRING = "(hash table)";
        public const string DEFAULT_BLOCKTABLE_KEYSTRING = "(block table)";

        /// <summary>
        /// TODO: Document this when it is being used correctly
        /// </summary>
        /// <param name="filePath"></param>
        public static MPQArchive LoadMPQ(string filePath) {
            FileStream dataStream; 
            try {
                dataStream = new(filePath, FileMode.Open);
            }
            catch (FileNotFoundException ex) { 
                // TODO: Catch other kinds of other exceptions!
                throw ex;
            }

            int headerPointer;
            try {
                headerPointer = FindHeaderPointer(dataStream);  
            } 
            catch (SignatureNotFoundException ex) {
                dataStream.Close();
                throw ex;
            }
            
            // Read header contents
            MPQHeader header = ReadHeaderData(dataStream, headerPointer);

            Cryptography crypto = new();

            MPQHashTable hashTable = null;
            if(header.HashTableSize > 0) {
                hashTable = ReadHashTable(dataStream, crypto, header.HashTableAddress, header.HashTableSize);
            }
            
            dataStream.Close();

            MPQArchive archive = new MPQArchive(header, hashTable);
            return archive;
        }

        /// <summary>
        /// Locates the address of the header within the given file.
        /// </summary>
        /// <param name="dataStream"></param>
        /// <returns></returns>
        /// <exception cref="SignatureNotFoundException"></exception>
        public static int FindHeaderPointer(Stream dataStream) {
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

        private static MPQHeader ReadHeaderData(Stream dataStream, int headerAddress)
        {
            // TODO: Support later header versions and sizes
            dataStream.Seek(headerAddress, SeekOrigin.Begin);
            byte[] headerData = new byte[MPQHeader.HEADER_SIZE_V1];
            dataStream.Read(headerData, 0, MPQHeader.HEADER_SIZE_V1);
            // SKIP 0x00 Signature (4 bytes)
            // SKIP 0x04 HeaderSize (4 bytes) 
            uint archiveSize =      BitConverter.ToUInt32(headerData, 0x08); // 4 bytes
            ushort formatVersion =  BitConverter.ToUInt16(headerData, 0x0C); // 2 bytes
            ushort blockSize =      BitConverter.ToUInt16(headerData, 0x0E); // 2 bytes
            uint hashTableAddress = BitConverter.ToUInt32(headerData, 0x10); // 4 bytes
            uint blockTableAddress =BitConverter.ToUInt32(headerData, 0x14); // 4 bytes
            uint hashTableSize =    BitConverter.ToUInt32(headerData, 0x18); // 4 bytes
            uint blockTableSize =   BitConverter.ToUInt32(headerData, 0x1C); // 4 bytes

            return formatVersion switch {
                0 => MPQHeader.CreateVersion1(
                    archiveSize, 
                    blockSize, 
                    hashTableAddress, 
                    blockTableAddress, 
                    hashTableSize, 
                    blockTableSize
                ),
                _ => throw new UnexpectedFormatVersionException($"MPQ format version not supported", formatVersion)
            };
        }

        /// <summary>
        /// Reads the contents of an encrypted block table to a new byte array, and unencrypts it. 
        /// </summary>
        /// <param name="dataStream">A seekable datastream where the table is stored</param>
        /// <param name="startAddress">Address of the table within the datastream</param>
        /// <param name="tableSize">Size of the requested table, in bytes</param>
        /// <param name="key">Encryption key</param>
        /// <returns>An array of bytes </returns>
        public static MPQHashTable ReadHashTable(
            Stream encryptedStream,
            Cryptography crypto,
            uint tableAddress, 
            uint entryCount
        ) { 
            // Seek to offset
            encryptedStream.Seek(tableAddress, SeekOrigin.Begin);

            // Decrypt
            byte[] tableData = new byte[entryCount*HASH_ENTRY_SIZE];
            uint key = crypto.HashString(DEFAULT_HASHTABLE_KEYSTRING, Cryptography.HashType.FileKey);

            encryptedStream.Read(tableData);   
            crypto.DecryptBlock(tableData, key);
            
            // Read entries 
            MemoryStream dataStream = new(tableData); 

            MPQHashEntry[] entries = new MPQHashEntry[entryCount];
            byte[] buffer = new byte[HASH_ENTRY_SIZE];
            for (int i = 0; i < entryCount; i++) {
                dataStream.Read(buffer);
                uint name1 =        BitConverter.ToUInt32(buffer, 0x00); // 4 bytes
                uint name2 =        BitConverter.ToUInt32(buffer, 0x04); // 4 bytes
                ushort locale =     BitConverter.ToUInt16(buffer, 0x08); // 2 bytes
                ushort platform =   BitConverter.ToUInt16(buffer, 0x0A); // 2 bytes
                uint blockindex =   BitConverter.ToUInt32(buffer, 0x0C); // 4 bytes
                MPQHashEntry entry = new(name1, name2, locale, platform, blockindex);
                entries[i] = entry;
            }

            return new MPQHashTable(entries);
        }
    }

}
