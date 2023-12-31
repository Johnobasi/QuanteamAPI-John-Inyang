﻿using System.Runtime.Serialization;

namespace QuanteamAPI.ExceptionMiddleware
{
    [Serializable]
    public class UserFriendlyException : Exception
    {
        public string? Details { get; private set; }

        public UserFriendlyException()
        {

        }

        protected UserFriendlyException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

        public UserFriendlyException(string message)
            : base(message)
        {

        }

        public UserFriendlyException(string message, string details)
            : base(message)
        {
            Details = details;
        }

        public UserFriendlyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public UserFriendlyException(string message, string details, Exception innerException)
            : base(message, innerException)
        {
            Details = details;
        }
    }
}
