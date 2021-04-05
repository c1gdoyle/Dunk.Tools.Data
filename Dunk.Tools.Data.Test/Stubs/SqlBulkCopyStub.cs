using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Dunk.Tools.Data.Core;

namespace Dunk.Tools.Data.Test.Stubs
{
    public sealed class SqlBulkCopyStub : SqlBulkCopyAdapter
    {
        public SqlBulkCopyStub(DbConnection connection)
            : base(connection)
        {
        }

        public SqlBulkCopyStub(IDbConnection connection)
            : base(connection)
        {
        }

        public DataTable BulkCopyData { get; set; }

        public IEnumerable<SqlBulkCopyColumnMapping> BulkCopyColumnMappings { get; set; }

        public string BulkCopyDestinationTableName { get; set; }

        public override void WriteToServer(DataTable table, IEnumerable<SqlBulkCopyColumnMapping> columnMappings, string destinationTableName)
        {
            BulkCopyData = table;
            BulkCopyColumnMappings = columnMappings;
            BulkCopyDestinationTableName = destinationTableName;
        }
    }
}
