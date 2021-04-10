using System;
using System.Data.Common;
using System.Runtime.Serialization;

namespace Dunk.Tools.Data.Test.Stubs
{
    [Serializable]
    public class DbStubException : DbException
    {
        public DbStubException()
            : base()
        {
        }

        protected DbStubException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
