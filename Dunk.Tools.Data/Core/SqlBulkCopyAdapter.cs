using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Dunk.Tools.Data.Base;
using Dunk.Tools.Data.Extensions;

namespace Dunk.Tools.Data.Core
{
    /// <summary>
    /// An implementation of <see cref="ISqlBulkCopy"/> that serves as a
    /// wrapper over the <see cref="SqlBulkCopy"/>.
    /// </summary>
    public class SqlBulkCopyAdapter : ISqlBulkCopy
    {
        private readonly IDbConnection _connection;

        /// <summary>
        /// Initialises a new instance of <see cref="SqlBulkCopyAdapter"/> with a specified
        /// Sql Connection
        /// </summary>
        /// <param name="connection">The already open connection instance that will be used to perform the bulk copy operation.</param>
        public SqlBulkCopyAdapter(SqlConnection connection)
            : this((DbConnection)connection)
        {
        }

        internal SqlBulkCopyAdapter(IDbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(
                    nameof(connection),
                    $"Unable to initialise {typeof(SqlBulkCopyAdapter)}, {nameof(connection)} parameter cannot be null");
            }
            if (connection.State != ConnectionState.Open)
            {
                throw new ArgumentException(
                    $"Unable to initialise {typeof(SqlBulkCopyAdapter)}, {nameof(connection)} parameter State must be Open.",
                    nameof(connection));
            }
            _connection = connection;
        }

        #region IBulkCopy Members
        /// <inheritdoc />
        public int? BatchSize { get; set; }

        /// <inheritdoc />
        public string DestinationTableName { get; set; }

        /// <inheritdoc />
        public virtual void WriteToServer(DataTable table)
        {
            var columnMappings = GetColumnMappings(table);
            WriteToServer(table, columnMappings);
        }

        /// <inheritdoc />
        public virtual void WriteToServer<T>(IEnumerable<T> items)
            where T : class
        {
            WriteToServer(items, p => true);
        }

        /// <inheritdoc />
        public virtual void WriteToServer<T>(IEnumerable<T> items, Func<PropertyInfo, bool> filter)
            where T : class
        {
            WriteToServer(items.ToDataTable(filter));
        }
        #endregion IBulkCopy Members

        #region ISqlBulkCopy Members

        /// <inheritdoc />
        public virtual void WriteToServer(DataTable table, IEnumerable<SqlBulkCopyColumnMapping> columnMappings)
        {
            string destinationTableName = GetDestinationTableName(table);
            WriteToServer(table, columnMappings, destinationTableName);
        }

        /// <inheritdoc />
        public virtual void WriteToServer(DataTable table, IEnumerable<SqlBulkCopyColumnMapping> columnMappings, string destinationTableName)
        {
            WriteToServerInternal(table, columnMappings, destinationTableName);
        }
        #endregion ISqlBulkCopy Members

        private void WriteToServerInternal(DataTable dataTable, IEnumerable<SqlBulkCopyColumnMapping> columnMappings, string destinationTableName)
        {
            using (var bulkCopy = new SqlBulkCopy(_connection as SqlConnection))
            {
                bulkCopy.DestinationTableName = destinationTableName;
                SetupBulkCopyBatchSize(bulkCopy, BatchSize);
                SetupBulkCopyColumnMappings(bulkCopy, columnMappings);

                bulkCopy.WriteToServer(dataTable);
            }
        }
        private string GetDestinationTableName(DataTable dataTable)
        {
            string destinationTableName =
                (!string.IsNullOrEmpty(DestinationTableName) ? DestinationTableName : null) // check for Destination table value
                ?? (!string.IsNullOrWhiteSpace(dataTable.TableName) ? dataTable.TableName : null); // if null or empty try the dataTable.TableName

            if (string.IsNullOrWhiteSpace(destinationTableName))
            {
                throw new ArgumentException($"{nameof(destinationTableName)} cannot be null or empty");
            }
            return destinationTableName;
        }

        private IEnumerable<SqlBulkCopyColumnMapping> GetColumnMappings(DataTable dataTable)
        {
            return dataTable.Columns.Cast<DataColumn>()
                .Select(c => new SqlBulkCopyColumnMapping(c.ColumnName, c.ColumnName));
        }

        private static void SetupBulkCopyBatchSize(SqlBulkCopy bulkCopy, int? batchSize)
        {
            if (batchSize.HasValue)
            {
                bulkCopy.BatchSize = batchSize.GetValueOrDefault();
            }
        }

        private static void SetupBulkCopyColumnMappings(SqlBulkCopy bulkCopy, IEnumerable<SqlBulkCopyColumnMapping> columnMappings)
        {
            foreach (var columnMapping in columnMappings)
            {
                bulkCopy.ColumnMappings.Add(columnMapping);
            }
        }
    }
}
