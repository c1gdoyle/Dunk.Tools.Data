using System;
using System.Data.Common;

namespace Dunk.Tools.Data.Test.Stubs
{
    [Serializable]
    public class DbExceptionStub : DbException
    {
        public DbExceptionStub()
            : base()
        {
        }
    }
}
