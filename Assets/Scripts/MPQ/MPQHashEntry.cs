namespace Elune.MPQ {
    public class MPQHashEntry {
        /// <summary>
        /// The hash of the full file name (part A)
        /// </summary>
        public uint Name1 { get; private set; }

        /// <summary>
        /// The hash of the full file name (part B)
        /// </summary>
        public uint Name2 { get; private set; }

        /// <summary>
        /// The language of the file. Uses values from the Windows LANGID data-type. 
        /// 0 represents the default language or a language-neutral file.
        /// </summary>
        public ushort Locale { get; private set; }

        /// <summary>
        /// Supposedly represents a platform id, but only 0 has ever been observed.
        /// </summary>
        public ushort Platform { get; private set; }

        /// <summary>
        /// If the hash table entry is valid, this is index in the block table for this file.
        /// Otherwise, it's either BLOCK_INDEX_EMPTY or BLOCK_INDEX_DELETED.
        /// </summary>
        public uint BlockIndex { get; private set; }

        /// <summary>
        /// Indicates that the associated hash table entry is empty, and always has been.
        /// Searches for this file should terminate.
        /// </summary>
        public const uint BLOCK_INDEX_EMPTY = 0xFFFF_FFFF;

        /// <summary>
        /// Indicates that the associated hash table entry is empty, but was once valid.
        /// Searches for this file should continue.
        /// </summary>
        public const uint BLOCK_INDEX_DELETED = 0xFFFF_FFFE;

        private MPQHashEntry() {}

        /// <summary>
        /// Construct a hash entry representation with the given data.
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <param name="locale"></param>
        /// <param name="platform"></param>
        /// <param name="blockindex"></param>
        /// <returns></returns>
        public MPQHashEntry(
            uint name1,
            uint name2,
            ushort locale,
            ushort platform,
            uint blockindex
        ) {
            Name1 = name1;
            Name2 = name2;
            Locale = locale;
            Platform = platform;
            BlockIndex = blockindex;
        }
    }
}