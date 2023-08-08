using System;

using System.Runtime.Serialization;

namespace Elune.MPQ {
    [Serializable]
    public class UnexpectedFormatVersionException : Exception
    {
        public UnexpectedFormatVersionException() { }
        
        public UnexpectedFormatVersionException(string message) : base(message) { }
        
        public UnexpectedFormatVersionException(string message, Exception inner) : base(message, inner) { }

        protected UnexpectedFormatVersionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}