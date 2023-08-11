using System;
using System.Runtime.Serialization;

namespace Elune.MPQ {
    [Serializable]
    public class MPQReadException : Exception
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