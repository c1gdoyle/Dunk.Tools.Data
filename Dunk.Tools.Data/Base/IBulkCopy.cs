using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Dunk.Tools.Data.Base
{
    /// <summary>
    /// An interface that defines the behaviour of a class that allows you to
    /// efficiently bulk load SQL table with data from another source.
    /// </summary>
    public interface IBulkCopy
    {
        /// <summary>
        /// Gets or sets the number of rows in each batch. At the end of each batch, the rows in
        /// the batch are sent to the server.
        /// </summary>
        int? BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the name of the destination table on
        /// the server.
        /// </summary>
        string DestinationTableName { get; set; }

        /// <summary>
        /// Copies all rows in the supplied <see cref="DataTable"/> to a destination table.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/> whose rows will be copied to the destination table.</param>
        /// <remarks>
        /// The destination table is specified by either the <see cref="DestinationTableName"/> property of this object
        /// or the <see cref="DataTable.TableName"/> property of the supplied <paramref name="table"/>.
        /// </remarks>
        void WriteToServer(DataTable table);

        /// <summary>
        /// Copies all items in the supplied collection to a destination table.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="items">The items that will be copied to the destination table.</param>
        /// <remarks>
        /// The destination table is specified by either the <see cref="DestinationTableName"/> property of this object.
        /// </remarks>
        void WriteToServer<T>(IEnumerable<T> items)
            where T : class;

        /// <summary>
        /// Copies all items in the supplied collection to a destination table 
        /// with a specified predicate to filter the properties of <typeparamref name="T"/>
        /// to be included.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="items">The items that will be copied to the destination table.</param>
        /// <param name="filter">The filter.</param>
        /// <remarks>
        /// The destination table is specified by either the <see cref="DestinationTableName"/> property of this object.
        /// </remarks>
        void WriteToServer<T>(IEnumerable<T> items, Func<PropertyInfo, bool> filter)
            where T : class;
    }
}
