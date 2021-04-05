using System;
using System.Data;
using System.Data.Common;

namespace Dunk.Tools.Data.Base
{
    /// <summary>
    /// Descrivers the behaviour of a facade class that supports
    /// read operations from a configured data-base.
    /// </summary>
    public interface IDataAccessLayerFacade
    {
        /// <summary>
        /// Gets the function for generating a <see cref="IDbConnection"/> instance
        /// to access the data-base.
        /// </summary>
        Func<IDbConnection> ConnectionFactory { get; }

        /// <summary>
        /// Executes a query against the configured data-base, creates a <see cref="DataTable"/> of the
        /// given name and fills the table with the results of the query.
        /// </summary>
        /// <param name="query">The SQL query to execute against the database.</param>
        /// <param name="tableName">The name of the table to be created.</param>
        /// <returns>
        /// The newly created table.
        /// </returns>
        DataTable FillTable(string query, string tableName);

        /// <summary>
        /// Executes a query against the configured data-base using a specified time-out, creates a <see cref="DataTable"/> of the
        /// given name and fills the table with the results of the query.
        /// </summary>
        /// <param name="query">The SQL query to execute against the database.</param>
        /// <param name="tableName">The name of the table to be created.</param>
        /// <param name="commandTimeout">The time-out for the SQL query to execute.</param>
        /// <returns>
        /// The newly created table.
        /// </returns>
        DataTable FillTable(string query, string tableName, int commandTimeout);

        /// <summary>
        /// Executes a query against the configured data-base using specified query parameters, creates a <see cref="DataTable"/> of the
        /// given name and fills the table with the results of the query.
        /// </summary>
        /// <param name="query">The SQL query to execute against the database.</param>
        /// <param name="tableName">The name of the table to be created.</param>
        /// <param name="queryParameters">The parameters to be used in the SQL query.</param>
        /// <returns>
        /// The newly created table.
        /// </returns>
        DataTable FillTable(string query, string tableName, DbParameter[] queryParameters);

        /// <summary>
        /// Executes a query against the configured data-base using specified query parameters and time-out, creates a <see cref="DataTable"/> of the
        /// given name and fills the table with the results of the query.
        /// </summary>
        /// <param name="query">The SQL query to execute against the database.</param>
        /// <param name="tableName">The name of the table to be created.</param>
        /// <param name="queryParameters">The parameters to be used in the SQL query.</param>
        /// <param name="commandTimeout">The time-out for the SQL query to execute.</param>
        /// <returns>
        /// The newly created table.
        /// </returns>
        DataTable FillTable(string query, string tableName, DbParameter[] queryParameters, int commandTimeout);
    }
}
