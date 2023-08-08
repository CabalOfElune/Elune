namespace Elune.MPQ {
    // TODO: Support for headers of higher version numbers

    /// <summary>
    /// MPQ headers based on the format documented in http://www.zezula.net/en/mpq/mpqformat.html
    /// </summary>
    public class MPQHeader {
        public const string ID_MPQ = "MPQ\x1A";
        // int id = ID_MPQ;

        public const int HEADER_SIZE_V1 = 0x20; // Used until WoW:BC
        public const int HEADER_SIZE_V2 = 0x2C;
        public const int HEADER_SIZE_V3 = 0x2C;
        public const int HEADER_SIZE_V4 = 0x2C;
        /// <summary>
        /// Size of the MPQ Header in bytes
        /// </summary>
        public uint HeaderSize { 
            get {
                return this.FormatVersion switch
                {
                    MPQFormatVersion.FORMAT_1 => HEADER_SIZE_V1,
                    MPQFormatVersion.FORMAT_2 => HEADER_SIZE_V2,
                    MPQFormatVersion.FORMAT_3 => HEADER_SIZE_V3,
                    MPQFormatVersion.FORMAT_4 => HEADER_SIZE_V4,
                    _ => 0x0,
                };
            } 
        }

        /// <summary>
        /// Size of the MPQ archive in bytes.
        /// </summary>
        public uint ArchiveSize { get; private set; }
        
        public enum MPQFormatVersion : ushort {
            // The version numbers are misaligned. 
            // Deal with it, this is how the documentation refers to it.
            FORMAT_1 = 0x0, // Vanilla
            FORMAT_2 = 0x1, // BC+
            FORMAT_3 = 0x2, // Cataclysm Beta
            FORMAT_4 = 0x3, // Everything after
        }
        
        /// Edition of the MPQ file format.       
        public MPQFormatVersion FormatVersion { get; private set; }

        /// <summary>
        /// Power-of-2 exponent specifying then number of 512-byte sectors in 
        /// each logical sector of the archive.
        /// </summary>
        public ushort BlockSize { get; private set; }

        /// <summary>
        /// Offset to the beginning of the hash table, relative to the 
        /// beginning of the archive.
        /// </summary>
        public uint HashTableAddress { get; private set; }

        /// <summary>
        /// Offset to the beginning of the block table, relative to the 
        /// beginning of the archive.
        /// </summary>
        public uint BlockTableAddress { get; private set; }

        /// <summary>
        /// Number of entries in the hash table
        /// </summary>
        public uint HashTableSize { get; private set; }

        /// <summary>
        /// Number of entries in the block table
        /// </summary>
        public uint BlockTableSize { get; private set; }

        private MPQHeader() {}

        /// <summary>
        /// Creates an MPQ header representation for version 1 with the given traits.
        /// </summary>
        /// <param name="archiveSize"></param>
        /// <param name="blockSize"></param>
        /// <param name="hashTableAddress"></param>
        /// <param name="blockTableAddress"></param>
        /// <param name="hashTableSize"></param>
        /// <param name="blockTableSize"></param>
        /// <returns></returns>
        public static MPQHeader CreateVersion1(
                uint archiveSize, 
                ushort blockSize,
                uint hashTableAddress,
                uint blockTableAddress,
                uint hashTableSize,
                uint blockTableSize
            ) {
            MPQHeader header = new() {
                ArchiveSize = archiveSize,
                FormatVersion = MPQFormatVersion.FORMAT_1, 
                BlockSize = blockSize, 
                HashTableAddress = hashTableAddress,
                BlockTableAddress = blockTableAddress,
                HashTableSize = hashTableSize,
                BlockTableSize = blockTableSize,
            };

            return header;
        }
    }
}