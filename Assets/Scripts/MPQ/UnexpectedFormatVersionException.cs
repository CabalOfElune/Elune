using System;

namespace Elune.MPQ {
    [System.Serializable]
    public class UnexpectedFormatVersionException : Exception
    {
        public UnexpectedFormatVersionException() { }
        
        public UnexpectedFormatVersionException(string message) : base(message) { }
        
        public UnexpectedFormatVersionException(string message, System.Exception inner) : base(message, inner) { }

        protected UnexpectedFormatVersionException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}