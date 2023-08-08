namespace Elune.MPQ {

    /// <summary>
    /// MPQ headers based on the format documented in http://www.zezula.net/en/mpq/mpqformat.html
    /// </summary>
    public class MPQHeader {
        const string ID_MPQ = "MPQ\x1A";
        // int id = ID_MPQ;

        /// <summary>
        /// Size of the MPQ Header in bytes
        /// </summary>
        int headerSize { get; }

        /// <summary>
        /// Size of the MPQ archive in bytes.
        /// </summary>
        int archiveSize { get; }

        enum MPQFormatVersion : ushort {
            // The version numbers are misaligned. 
            // Deal with it, this is how the documentation refers to it.
            FORMAT_1 = 0x0, // Vanilla
            FORMAT_2 = 0x1, // BC+
            FORMAT_3 = 0x2, // Cataclysm Beta
            FORMAT_4 = 0x3, // Everything after
        }
        
        /// Edition of the MPQ file format.       
        MPQFormatVersion formatVersion { get; }

        /// <summary>
        /// Power-of-2 exponent specifying then number of 512-byte sectors in 
        /// each logical sector of the archive.
        /// </summary>
        ushort blockSize { get; }

        /// <summary>
        /// Offset to the beginning of the hash table, relative to the 
        /// beginning of the archive.
        /// </summary>
        int hashTableAddress;

        /// <summary>
        /// Offset to the beginning of the block table, relative to the 
        /// beginning of the archive.
        /// </summary>
        int blockTablePos;

        /// <summary>
        /// Number of entries in the hash table
        /// </summary>
        int hashTableSize;

        /// <summary>
        /// Number of entries in the block table
        /// </summary>
        int blockTableSize;

    }
}