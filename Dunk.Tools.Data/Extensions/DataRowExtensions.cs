using System;
using System.Collections.Generic;
using System.Data;

namespace Dunk.Tools.Data.Extensions
{
    /// <summary>
    /// Provides a series of extension methods for a <see cref="DataRow"/> instance.
    /// </summary>
    public static class DataRowExtensions
    {
        /// <summary>
        /// Converts the data row into a dictionary.
        /// </summary>
        /// <param name="row">The dat row to convert.</param>
        /// <returns>
        /// A <see cref="Dictionary{TKey, TValue}"/> containing the contents of the data row.
        /// The keys match the column names of the row and the values match the corresponding cell-value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="row"/> was null.</exception>
        public static Dictionary<string, object> ToDictionary(this DataRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row),
                    $"Unable to convert data-row to dictionary. {nameof(row)} parameter cannot be null.");
            }

            var dictionary = new Dictionary<string, object>();

            for (int i = 0; i < row.ItemArray.Length; i++)
            {
                string columName = row.GetColumnName(i);
                object cellValue = row[i];
                dictionary.Add(columName, cellValue);
            }
            return dictionary;
        }

        /// <summary>
        /// Gets the column name from the data row for a given ordinal.
        /// </summary>
        /// <param name="row">The data row.</param>
        /// <param name="ordinal">The ordinal index.</param>
        /// <returns>
        /// The name of the column at the ordinal index.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="row"/> was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="ordinal"/> was negative or greater than number of columns.</exception>
        public static string GetColumnName(this DataRow row, int ordinal)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row),
                    $"Unable to get column name. {nameof(row)} parameter cannot be null.");
            }
            if (ordinal < 0 || ordinal >= row.ItemArray.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(ordinal), 
                    $"Unable to get column name. {nameof(ordinal)} must be non-negative and less than number of columns");
            }

            return row.Table.Columns[ordinal].ColumnName;
        }

        /// <summary>
        /// Produces a comma-separated string representation of the contents of the data-row.
        /// </summary>
        /// <param name="row">The data-row.</param>
        /// <returns>
        /// A string representation of the contents of the data-row.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="row"/> was null.</exception>
        public static string ItemArrayToString(this DataRow row)
        {
            return ItemArrayToString(row, ",");
        }

        /// <summary>
        /// Produces a comma-separated string representation of the contents of the data-row with specified
        /// separator.
        /// </summary>
        /// <param name="row">The data-row.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>
        /// A string representation of the contents of the data-row.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="row"/> was null.</exception>
        public static string ItemArrayToString(this DataRow row, string separator)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row),
                    $"Unable to get string representation of item array. {nameof(row)} parameter cannot be null.");
            }

            return string.Join(separator, row.ItemArray);

        }
    }
}
