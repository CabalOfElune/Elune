using System;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

namespace Elune.MPQ {

public class MPQHashTable {
    private readonly MPQHashEntry[] entries;

    public MPQHashTable(MPQHashEntry[] hashEntries) {
        var size = hashEntries.Length;
        if(size == 0) {
            throw new ArgumentException("Array size must not be 0", "hashEntries");
        }
        // Only allow power-of-2 sizes
        if((size & (size-1)) != 0) {
            throw new ArgumentException("Array size is not a power of 2", "hashEntries");
        }

        entries = hashEntries;
    }

    public MPQHashTable(int size) {
        entries = new MPQHashEntry[size];
        for(int i = 0; i < entries.Length; i++) {
            MPQHashEntry entry = new(
                0xFFFF_FFFF, 
                0xFFFF_FFFF, 
                0xFFFF, 0xFFFF, 
                MPQHashEntry.BLOCK_INDEX_EMPTY);
            entries[i] = entry;
        }
    }

    public MPQHashEntry Get(int index) {
        return entries[index];
    }

    public int InsertEntry(Cryptography crypto, string name, ushort locale, ushort platform, uint blockIndex) {
        uint hash_index = crypto.HashString(name, Cryptography.HashType.TableIndex);
        uint hash_name1 = crypto.HashString(name, Cryptography.HashType.NameA);
        uint hash_name2 = crypto.HashString(name, Cryptography.HashType.NameB);
        MPQHashEntry newEntry = new(hash_name1, hash_name2, locale, platform, blockIndex);

        int startIndex = (int)(hash_index % (uint)entries.Length);
        for(int i = startIndex; ; ) {
            MPQHashEntry testEntry = entries[i];

            if(testEntry.BlockIndex == MPQHashEntry.BLOCK_INDEX_EMPTY) {
                entries[i] = newEntry;
                return i;
            }

            i++;
            i %= entries.Length;
            if(i == startIndex) {
                throw new IndexWraparoundException("Search for entry led to checking every hash!", name);
            }
        }
    }

    public MPQHashEntry FindEntry(Cryptography crypto, string name) {
        // Hash the name
        uint hash_index = crypto.HashString(name, Cryptography.HashType.TableIndex);
        uint hash_name1 = crypto.HashString(name, Cryptography.HashType.NameA);
        uint hash_name2 = crypto.HashString(name, Cryptography.HashType.NameB);

        int startIndex = (int)(hash_index % entries.Length);
        Debug.Log($"Starting at: {startIndex:X4} / {entries.Length:X4}");
        for(int i = startIndex; ; ) {
            MPQHashEntry testEntry = entries[i];

            if(testEntry.Name1 == hash_name1 && testEntry.Name2 == hash_name2) {
                return testEntry;
            }
            if(testEntry.BlockIndex == MPQHashEntry.BLOCK_INDEX_EMPTY) {
                throw new FileNotFoundException("Could not find hashentry for path", name);
            }

            i++;
            // Safety! Wrap around the list, but don't loop forever.
            i %= entries.Length;
            if(i == startIndex) {
                throw new IndexWraparoundException("Search for entry led to checking every hash!", name);
            }
        }
    }
    
    [Serializable]
    public class IndexWraparoundException : SystemException
    {
        public readonly string name;

        public IndexWraparoundException(string name) {
            this.name = name;
        }
        public IndexWraparoundException(string message, string name) : base(message) {
            this.name = name;
        }
        public IndexWraparoundException(string message, Exception inner, string name) : base(message, inner) {
            this.name = name;
        }
        protected IndexWraparoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}

} // Namespace