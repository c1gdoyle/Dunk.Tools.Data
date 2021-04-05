using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Dunk.Tools.Data.Base
{
    /// <summary>
    /// An interface that defines the behaviour of a class that allows you to
    /// efficiently bulk load a SQL table with data from another source to a
    /// SqlServer.
    /// </summary>
    public interface ISqlBulkCopy : IBulkCopy
    {
        /// <summary>
        /// Copies all rows in the supplied <see cref="DataTable"/> to a destination table
        /// using specified column mappings.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/> whose rows will be copied to the destination table.</param>
        /// <param name="columnMappings">The column mappings to use to copy the data to the destination table.</param>
        void WriteToServer(DataTable table, IEnumerable<SqlBulkCopyColumnMapping> columnMappings);

        /// <summary>
        /// Copies all rows in the supplied <see cref="DataTable"/> to a destination table
        /// using specified column mappings and a destination table name.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/> whose rows will be copied to the destination table.</param>
        /// <param name="columnMappings">The column mappings to use to copy the data to the destination table.</param>
        /// <param name="destinationTableName">The name of the destination table to write the data to.</param>
        void WriteToServer(DataTable table, IEnumerable<SqlBulkCopyColumnMapping> columnMappings, string destinationTableName);
    }
}
