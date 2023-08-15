using System;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace Elune.MPQ {
    public class MPQArchive {
        public MPQHeader header;
        public MPQHashTable hashTable;

        private MPQArchive() {}

        public MPQArchive(MPQHeader header, MPQHashTable hashTable) {
            this.header = header;
            this.hashTable = hashTable;
        }

        public byte[] FindFile(Cryptography crypto, string path) {
            if (hashTable == null) {
                throw new NoHashTableException("Cannot find a file without a hash table.");
            }
            
            MPQHashEntry fileHandle;
            try {
                fileHandle = hashTable.FindEntry(crypto, path);
            }
            catch(FileNotFoundException ex) {
                throw ex;
            }

            return Encoding.ASCII.GetBytes("Hello World!"); // TODO: Go even further beyond
        }
    }

    [Serializable]
    public class NoHashTableException : Exception
    {
        public NoHashTableException() { }
        public NoHashTableException(string message) : base(message) { }
        public NoHashTableException(string message, Exception inner) : base(message, inner) { }
        protected NoHashTableException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }

    
}