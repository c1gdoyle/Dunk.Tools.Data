using System;
using System.Data;

namespace Dunk.Tools.Data.Extensions
{
    /// <summary>
    /// Provides a series of extension methods for a <see cref="DataTable"/> instance.
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// Remove a given column from the data-table.
        /// </summary>
        /// <param name="table">The data-table.</param>
        /// <param name="columnName">The name of the column to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="table"/> or <paramref name="columnName"/> was null.</exception>
        /// <exception cref="ArgumentException"><paramref name="table"/> does not contain <paramref name="columnName"/>.s</exception> 
        public static void RemoveColumn(this DataTable table, string columnName)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table),
                    $"Unable to remove column. {nameof(table)} parameter cannot be null.");
            }

            if (string.IsNullOrEmpty(columnName))
            {
                throw new ArgumentNullException(nameof(columnName),
                    $"Unable to remove column. {nameof(columnName)} parameter cannot be null.");
            }

            int index = table.Columns.IndexOf(columnName);
            if (index == -1)
            {
                throw new ArgumentException($"Unable to remove column. Table does not contain column of name {columnName}",
                    nameof(columnName));
            }
            table.Columns.RemoveAt(index);
            table.AcceptChanges();
        }

        /// <summary>
        /// Renames a specified column in the data-table to a specified new name.
        /// </summary>
        /// <param name="table">The data-table.</param>
        /// <param name="oldName">The old name of the column.</param>
        /// <param name="newName">The new name of the column.</param>
        /// <exception cref="ArgumentNullException"><paramref name="table"/> or <paramref name="oldName"/> or <paramref name="newName"/>was null.</exception>
        /// <exception cref="ArgumentException"><paramref name="oldName"/> and <paramref name="newName"/> are the same, or <paramref name="table"/> does not contain <paramref name="oldName"/>.s</exception> 
        public static void RenameColumn(this DataTable table, string oldName, string newName)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table),
                    $"Unable to rename column. {nameof(table)} parameter cannot be null.");
            }

            if (string.IsNullOrEmpty(oldName))
            {
                throw new ArgumentNullException(nameof(oldName),
                    $"Unable to rename column. {nameof(oldName)} parameter cannot be null.");
            }

            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentNullException(nameof(newName),
                    $"Unable to rename column. {nameof(newName)} parameter cannot be null.");
            }

            if (oldName == newName)
            {
                throw new ArgumentException($"Unable to rename column. {nameof(oldName)} and {nameof(newName)} must be different");
            }

            int index = table.Columns.IndexOf(oldName);
            if (index == -1)
            {
                throw new ArgumentException($"Unable to remove column. Table does not contain column of name {oldName}",
                    nameof(oldName));
            }
            table.Columns[index].ColumnName = newName;
            table.AcceptChanges();
        }
    }
}
