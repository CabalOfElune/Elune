using System;

using System.Runtime.Serialization;

namespace Elune.MPQ {
    [Serializable]
    public class UnexpectedFormatVersionException : Exception
    {
        public readonly int formatVersion;

        public UnexpectedFormatVersionException(int formatVersion) {
            this.formatVersion = formatVersion;
        }
        
        public UnexpectedFormatVersionException(string message, int formatVersion) : base(message) {
            this.formatVersion = formatVersion;
        }
        
        public UnexpectedFormatVersionException(string message, int formatVersion, Exception inner) : base(message, inner) {
            this.formatVersion = formatVersion;
        }

        protected UnexpectedFormatVersionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}