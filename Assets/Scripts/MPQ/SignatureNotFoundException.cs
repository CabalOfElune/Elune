using System;
using System.Runtime.Serialization;

namespace Elune.MPQ {
    [Serializable]
    public class SignatureNotFoundException : Exception
    {
        public SignatureNotFoundException()
        {
        }

        public SignatureNotFoundException(string message) : base(message)
        {
        }

        public SignatureNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SignatureNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}