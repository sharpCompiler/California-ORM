using System;
using System.Runtime.Serialization;

namespace California_ORM.Sql.Builder
{
    internal class MappingNotFoundException : Exception
    {
        public MappingNotFoundException()
        {
        }

        public MappingNotFoundException(string? message) : base(message)
        {
        }

        public MappingNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MappingNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}