using System;

namespace HereSay
{
    [Serializable]
    public class InstallationException : Exception
    {
        public InstallationException() { }
        public InstallationException(string message) : base(message) { }
        public InstallationException(string message, Exception inner) : base(message, inner) { }
        protected InstallationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
