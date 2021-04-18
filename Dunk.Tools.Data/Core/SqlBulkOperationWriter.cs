using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dunk.Tools.Data.Base;
using Dunk.Tools.Data.Extensions;

namespace Dunk.Tools.Data.Core
{
    /// <summary>
    /// An implementation of <see cref="IBulkOperationWriter"/> that performs bulk 
    /// WRITE operations against a configured SQL data-base
    /// </summary>
    public class SqlBulkOperationWriter : IBulkOperationWriter
    {
        private readonly Func<IDbConnection> _connectionFactory;
        private readonly Func<IDbConnection, IBulkCopy> _bulkCopyFactory;

        /// <summary>
        /// Intialises a new instance of <see cref="SqlBulkOperationWriter"/> with a specified 
        /// function for generating a connection to the data-base.
        /// </summary>
        /// <param name="connectionFactory">The function used to generate a connection to the data-base.</param>
        public SqlBulkOperationWriter(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _bulkCopyFactory = c => new Core.SqlBulkCopyAdapter(c as System.Data.SqlClient.SqlConnection);
        }

        internal SqlBulkOperationWriter(Func<IDbConnection> connectionFactory, Func<IDbConnection, IBulkCopy> bulkCopyFactory)
        {
            _connectionFactory = connectionFactory;
            _bulkCopyFactory = bulkCopyFactory;
        }

        #region IBulkOperationWriter Members
        /// <inheritdoc />
        public void BulkInsert<T>(IEnumerable<T> data, string tableName, int? batchSize = null)
            where T : class
        {
            using (var connection = _connectionFactory())
            {
                connection.Open();

                var bulkCopy = _bulkCopyFactory(connection);
                WriteToDatabase(bulkCopy, data, tableName, batchSize);
            }
        }

        /// <inheritdoc />
        public void BulkUpdate<T>(IEnumerable<T> data, string tableName, string[] primaryKeys, ICollection<string> fieldsToUpdate = null, int? batchSize = null)
            where T : class
        {
            using (var connection = _connectionFactory())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    var updateClauses = GetUpdateClausesFromInformationSchema(command, tableName, primaryKeys, fieldsToUpdate);

                    //create temporary table
                    command.CommandText = $"CREATE TABLE #TempTable ({string.Join(",", updateClauses.CreateTableClauses)})";
                    command.ExecuteNonQuery();

                    //write data to temporary table
                    var bulkCopy = _bulkCopyFactory(connection);
                    WriteToDatabase(bulkCopy, data, "#TempTable", batchSize);

                    StringBuilder sbPrimaryKeys = new StringBuilder();
                    sbPrimaryKeys.Append(string.Join(" AND ", primaryKeys
                        .Select(pk => $"target.{pk} = source.{pk}")));

                    //update target table using data in temporary table
                    command.CommandText = $"UPDATE target SET {string.Join(",", updateClauses.UpdateValuesClauses)} FROM {tableName} target INNER JOIN #TempTable source ON {sbPrimaryKeys.ToString()}";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <inheritdoc />
        public void BulkUpsert<T>(IEnumerable<T> data, string tableName, string[] primaryKeys, ICollection<string> fieldsToMatch, int? batchSize = null)
            where T : class
        {
            if(fieldsToMatch == null)
            {
                throw new ArgumentNullException(nameof(fieldsToMatch));
            }
            using (var connection = _connectionFactory())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    var upsertClauses = GetUpsertClausesFromInformationSchema(command, tableName, primaryKeys, fieldsToMatch);

                    //create temporary table
                    command.CommandText = $"CREATE TABLE #TempTable ({string.Join(",", upsertClauses.CreateTableClauses)})";
                    command.ExecuteNonQuery();

                    //write data to temporary table
                    var bulkCopy = _bulkCopyFactory(connection);
                    WriteToDatabase(bulkCopy, data, "#TempTable", batchSize);

                    //merge data in temporary table with target table
                    command.CommandText = $"MERGE INTO {tableName} AS target USING #TempTable AS source ON {string.Join(" AND ", upsertClauses.WhereClauses)} WHEN MATCHED THEN UPDATE SET {string.Join(",", upsertClauses.UpdateValuesClauses)} WHEN NOT MATCHED THEN INSERT ({string.Join(",", upsertClauses.InsertClauses)}) VALUES ({string.Join(",", upsertClauses.InsertValuesClauses)});";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <inheritdoc />
        public void BulkDelete<T>(IEnumerable<T> data, string tableName, string[] primaryKeys, int? batchSize = null)
            where T : class
        {
            using (var connection = _connectionFactory())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    var deleteClauses = GetDeleteClausesFromInformationSchema(command, tableName);

                    //create temporary table
                    command.CommandText = $"CREATE TABLE #TempTable ({string.Join(",", deleteClauses.CreateTableClauses)})";
                    command.ExecuteNonQuery();

                    //write data to temporary table
                    var bulkCopy = _bulkCopyFactory(connection);
                    WriteToDatabase(bulkCopy, data, "#TempTable", batchSize);

                    StringBuilder sbPrimaryKeys = new StringBuilder();
                    sbPrimaryKeys.Append(string.Join(" AND ", primaryKeys
                        .Select(pk => $"target.{pk} = source.{pk}")));

                    //delete data in target table by joining on temporary table
                    command.CommandText = $"DELETE target FROM {tableName} target INNER JOIN #TempTable source ON {sbPrimaryKeys.ToString()}";
                    command.ExecuteNonQuery();
                }
            }
        }
        #endregion IBulkOperationWriter Members

        private static UpdateClauses GetUpdateClausesFromInformationSchema(IDbCommand command, string tableName, string[] primaryKeys, ICollection<string> fieldsToUpdate = null)
        {
            var updateClauses = new UpdateClauses();
            HashSet<string> pks = new HashSet<string>(primaryKeys);

            command.CommandText = $"SELECT * FROM information_schema.columns WHERE table_name = '{tableName}'";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader["COLUMN_NAME"].ToString();

                    string createTableClause = GetCreateTableClause(reader, name);

                    updateClauses.CreateTableClauses.Add(createTableClause);

                    if (pks.Contains(name))
                    {
                        //don't try to update a primary key column
                        continue;
                    }

                    if (fieldsToUpdate != null && fieldsToUpdate.Count != 0)
                    {
                        //only update the fields specified
                        if (fieldsToUpdate.Contains(name))
                        {
                            updateClauses.UpdateValuesClauses.Add($"target.{name}=source.{name}");
                        }
                        continue;
                    }
                    updateClauses.UpdateValuesClauses.Add($"target.{name}=source.{name}");
                }
            }

            return updateClauses;
        }

        private static UpsertClauses GetUpsertClausesFromInformationSchema(IDbCommand command, string tableName, string[] primaryKeys, ICollection<string> fieldsToMatch)
        {
            var upsertClauses = new UpsertClauses();
            HashSet<string> pks = new HashSet<string>(primaryKeys);

            command.CommandText = $"SELECT * FROM information_schema.columns WHERE table_name = '{tableName}'";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader["COLUMN_NAME"].ToString();

                    string createTableClause = GetCreateTableClause(reader, name);

                    upsertClauses.CreateTableClauses.Add(createTableClause);

                    if (fieldsToMatch.Contains(name))
                    {
                        upsertClauses.WhereClauses.Add($"target.{name}=source.{name}");
                    }

                    if (pks.Contains(name))
                    {
                        //don't try to update a primary key column
                        continue;
                    }

                    //don't try to update a field to match on
                    if (!fieldsToMatch.Contains(name))
                    {
                        upsertClauses.UpdateValuesClauses.Add($"target.{name}=source.{name}");
                    }
                    upsertClauses.InsertClauses.Add(name);

                    upsertClauses.InsertValuesClauses.Add($"source.{name}");
                }
            }
            return upsertClauses;
        }

        private static DeleteClauses GetDeleteClausesFromInformationSchema(IDbCommand command, string tableName)
        {
            var deleteClauses = new DeleteClauses();

            command.CommandText = $"SELECT * FROM information_schema.columns WHERE table_name = '{tableName}'";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader["COLUMN_NAME"].ToString();

                    string createTableClause = GetCreateTableClause(reader, name);

                    deleteClauses.CreateTableClauses.Add(createTableClause);
                }
            }
            return deleteClauses;
        }

        private static string GetCreateTableClause(IDataReader reader, string columName)
        {
            var dataType = reader["DATA_TYPE"].ToString();

            var isNullable = reader["IS_NULLABLE"].ToString() == "YES";

            var maxLength = reader["CHARACTER_MAXIMUM_LENGTH"].ToString();

            var numericPrecision = reader["NUMERIC_PRECISION"].ToString();

            var numericScale = reader["NUMERIC_SCALE"].ToString();

            var maxLengthNumber = maxLength == "-1" ? "max" : maxLength;

            return $"[{columName}] {dataType}{(string.IsNullOrEmpty(maxLength) ? "" : "(" + (maxLengthNumber) + ")")}{(string.IsNullOrEmpty(numericPrecision) || dataType != "decimal" ? "" : "(" + numericPrecision + "," + numericScale + ")")} {(isNullable ? "" : "NOT ")}NULL";
        }

        private static void WriteToDatabase<T>(IBulkCopy bulkCopy, IEnumerable<T> data, string tableName, int? batchSize)
            where T : class
        {
            if (batchSize.HasValue)
            {
                bulkCopy.BatchSize = batchSize;
            }

            bulkCopy.DestinationTableName = tableName;
            using (var dt = data.ToDataTable(p => p.GetGetMethod().IsVirtual || p.GetGetMethod().IsFinal))
            {
                bulkCopy.WriteToServer(dt);
            }
        }

        private class UpsertClauses
        {
            public UpsertClauses()
            {
                CreateTableClauses = new List<string>();
                WhereClauses = new List<string>();
                UpdateValuesClauses = new List<string>();
                InsertClauses = new List<string>();
                InsertValuesClauses = new List<string>();
            }

            public IList<string> CreateTableClauses { get; private set; }

            public IList<string> WhereClauses { get; private set; }

            public IList<string> UpdateValuesClauses { get; private set; }

            public IList<string> InsertClauses { get; private set; }

            public IList<string> InsertValuesClauses { get; private set; }
        }

        private class UpdateClauses
        {
            public UpdateClauses()
            {
                CreateTableClauses = new List<string>();
                UpdateValuesClauses = new List<string>();
            }

            public IList<string> CreateTableClauses { get; private set; }

            public IList<string> UpdateValuesClauses { get; private set; }
        }

        private class DeleteClauses
        {
            public DeleteClauses()
            {
                CreateTableClauses = new List<string>();
            }

            public IList<string> CreateTableClauses { get; private set; }
        }
    }
}
