using System.Collections.Generic;

namespace Dunk.Tools.Data.Base
{
    /// <summary>
    /// Defines the behaviour of a writer class responsible for
    /// performing bulk WRITE opeartions against a configured data-base.
    /// </summary>
    public interface IBulkOperationWriter
    {
        /// <summary>
        /// Performs a bulk INSERT operation against the data-base using a specified
        /// collection of entities and table name.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="data">The data to be inserted.</param>
        /// <param name="tableName">The name of the data-base table to insert to.</param>
        /// <param name="batchSize">The batch size to use for the inserts.</param>
        void BulkInsert<T>(IEnumerable<T> data, string tableName, int? batchSize = null)
            where T : class;

        /// <summary>
        /// Performs a bulk UPDATE operation against the data-base using a speciifed
        /// collection of entities, table name and primary-keys/
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="data">The data to be inserted.</param>
        /// <param name="tableName">The name of the data-base table to insert to.</param>
        /// <param name="primaryKeys">The data-base tables primary keys to update.</param>
        /// <param name="fieldsToUpdate">The fields to update.</param>
        /// <param name="batchSize">The batch size to use for the inserts.</param>
        void BulkUpdate<T>(IEnumerable<T> data, string tableName, string[] primaryKeys, ICollection<string> fieldsToUpdate = null, int? batchSize = null)
            where T : class;

        /// <summary>
        /// Performs a bulk UPSERT operation against the database using a specified
        /// collection of entities, table name, primary keys and the fields to match on.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="data">The data to be inserted.</param>
        /// <param name="tableName">The name of the data-base table to insert to.</param>
        /// <param name="primaryKeys">The data-base tables primary keys to update.</param>
        /// <param name="fieldsToMatch">
        /// The fields in the database to match on.
        /// If the fields in the supplied data match existing entries in the database we UPDATE
        /// using the data entity; otherwise we INSERT a new entity.
        /// </param>
        /// <param name="batchSize">The batch size to use for the inserts.</param>
        void BulkUpsert<T>(IEnumerable<T> data, string tableName, string[] primaryKeys, ICollection<string> fieldsToMatch, int? batchSize = null)
            where T : class;

        /// <summary>
        /// Performs a bulk DELETE operation against the underyling data-base using a specified
        /// collection of entities.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="data">The data to be inserted.</param>
        /// <param name="tableName">The name of the data-base table to insert to.</param>
        /// <param name="primaryKeys">The data-base tables primary keys to update.</param>
        /// <param name="batchSize">The batch size to use for the inserts.</param>
        void BulkDelete<T>(IEnumerable<T> data, string tableName, string[] primaryKeys, int? batchSize = null)
            where T : class;
    }
}
