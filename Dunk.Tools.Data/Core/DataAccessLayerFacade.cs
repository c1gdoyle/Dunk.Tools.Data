using System;
using System.Data;
using System.Data.Common;
using Dunk.Tools.Data.Base;
using Dunk.Tools.Data.Utilities;

namespace Dunk.Tools.Data.Core
{
    /// <summary>
    /// An implementation of <see cref="IDataAccessLayerFacade"/> for
    /// executing read operations against a SQL database.
    /// </summary>
    public class DataAccessLayerFacade : IDataAccessLayerFacade
    {
        private readonly Func<IDbConnection> _connectionFactory;
        private const int DefaultCommandTimeout = 300;
        private readonly int _retryCount;

        /// <summary>
        /// Initialises a new instance of <see cref="DataAccessLayerFacade"/> with a
        /// specifed function to create a connection to the data-base.
        /// </summary>
        /// <param name="connectionFactory">The function used to generate a connection for the data-base.</param>
        public DataAccessLayerFacade(Func<IDbConnection> connectionFactory)
            : this(connectionFactory, 3)
        {
        }

        /// <summary>
        /// Initialises a new instance of <see cref="DataAccessLayerFacade"/> with a
        /// specifed function to create a connection to the data-base and the number of times to
        /// retry a failed call to the data-base before returning an error.
        /// </summary>
        /// <param name="connectionFactory">The function used to generate a connection for the data-base.</param>
        /// <param name="retryCount">The number of retries allowed. By default this will be 3.</param>
        public DataAccessLayerFacade(Func<IDbConnection> connectionFactory, int retryCount)
        {
            _connectionFactory = connectionFactory;
            _retryCount = retryCount;
        }

        #region IDataAccessLayerFacade Members
        /// <inheritdoc />
        public Func<IDbConnection> ConnectionFactory
        {
            get { return _connectionFactory; }
        }

        /// <inheritdoc />
        public DataTable FillTable(string query, string tableName)
        {
            return FillTable(query, tableName, new object[0], DefaultCommandTimeout, 1);
        }

        /// <inheritdoc />
        public DataTable FillTable(string query, string tableName, DbParameter[] queryParameters)
        {
            return FillTable(query, tableName, queryParameters, DefaultCommandTimeout, 1);
        }

        /// <inheritdoc />
        public DataTable FillTable(string query, string tableName, int commandTimeout)
        {
            return FillTable(query, tableName, new object[0], commandTimeout, 1);
        }

        /// <inheritdoc />
        public DataTable FillTable(string query, string tableName, DbParameter[] queryParameters, int commandTimeout)
        {
            return FillTable(query, tableName, queryParameters, commandTimeout, 1);
        }
        #endregion IDataAccessLayerFacade Members

        private DataTable FillTable(string query, string dataTableName, object[] queryParameters, int commandTimeout, int retryCount)
        {
            using (IDbConnection connection = ConnectionFactory())
            {
                try
                {
                    return FillTable(connection, query, dataTableName, queryParameters, commandTimeout);
                }
                catch (System.Data.Common.DbException dbEx)
                {
                    if (RetriesExceeded(retryCount))
                    {
                        throw new DataAccessLayerFacadeException($"Failed calls to data-base exceeded allowed retrieves {_retryCount}", dbEx);
                    }
                }
            }
            //retry
            return FillTable(query, dataTableName, queryParameters, commandTimeout, ++retryCount);
        }

        private DataTable FillTable(IDbConnection connection, string query, string dataTableName, object[] queryParameters, int commandTimeout)
        {
            OpenConnection(connection);
            using (IDbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = string.Format(query, queryParameters);
                cmd.CommandTimeout = commandTimeout;

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    return PopulateDataTable(reader, dataTableName);
                }
            }
        }

        private DataTable PopulateDataTable(IDataReader reader, string dataTableName)
        {
            int count = 0;
            DataTable table = null;
            while (reader.Read())
            {
                if (count++ == 0)
                {
                    table = CreateTable(reader);
                    table.TableName = dataTableName;
                }
                AddDataRow(reader, table);

            }
            return table;
        }

        private DataTable CreateTable(IDataReader reader)
        {
            var table = new DataTable();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                AddDataColumn(reader, table, i);
            }
            return table;
        }

        private void AddDataColumn(IDataReader reader, DataTable table, int index)
        {
            string columnName = reader.GetName(index);
            Type columnType = reader.GetFieldType(index);
            table.Columns.Add(columnName, columnType);
        }

        private void AddDataRow(IDataReader reader, DataTable table)
        {
            DataRow row = table.NewRow();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[i] = reader.GetValue(i);
            }
            table.Rows.Add(row);
        }

        private void OpenConnection(IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        private bool RetriesExceeded(int retryCount)
        {
            if (retryCount == 3)
            {
                return true;
            }
            return false;
        }
    }
}
