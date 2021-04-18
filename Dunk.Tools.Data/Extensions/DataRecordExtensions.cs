using System;
using System.Data;

namespace Dunk.Tools.Data.Extensions
{
    /// <summary>
    /// Provides a series of extension methods for a <see cref="IDataRecord"/>
    /// </summary>
    public static class DataRecordExtensions
    {
        /// <summary>
        /// Gets the value from a given column using a specified column index.
        /// </summary>
        /// <typeparam name="T">The type we are retrieving from the column.</typeparam>
        /// <param name="record">The data-record.</param>
        /// <param name="columnIndex">The column index we are reading from.</param>
        /// <returns>
        /// The value retrieved from the data-record or the default value of <typeparamref name="T"/> if
        /// the cell was <see cref="DBNull.Value"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="record"/> was null.</exception>
        /// <exception cref="InvalidCastException">The value in the data-record was not <see cref="DBNull"/> but could not be case to an instance of <typeparamref name="T"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="IDataRecord.FieldCount"/>.</exception>
        public static T GetValueOrDefault<T>(this IDataRecord record, int columnIndex)
        {
            if(record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (!record.IsDBNull(columnIndex))
            {
                return record.GetDataValue<T>(columnIndex);
            }
            return default(T);
        }

        /// <summary>
        /// Gets the value from a given column using a specified column name.
        /// </summary>
        /// <typeparam name="T">The type we are retrieving from the column.</typeparam>
        /// <param name="record">The data-record.</param>
        /// <param name="columnName">The column name we are reading from.</param>
        /// <returns>
        /// The value retrieved from the data-record or the default value of <typeparamref name="T"/> if
        /// the cell was <see cref="DBNull.Value"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="record"/> was null.</exception>
        /// <exception cref="InvalidCastException">The value in the data-record was not <see cref="DBNull"/> but could not be case to an instance of <typeparamref name="T"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="IDataRecord.FieldCount"/>.</exception>
        public static T GetValueOrDefault<T>(this IDataRecord record, string columnName)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            int columnIndex = record.GetOrdinal(columnName);
            if (!record.IsDBNull(columnIndex))
            {
                return record.GetValueOrDefault<T>(columnIndex);
            }
            return default(T);
        }

        /// <summary>
        /// Gets the value from a given column using a specified column index.
        /// </summary>
        /// <typeparam name="T">The type we are retrieving from the column.</typeparam>
        /// <param name="record">The data-record.</param>
        /// <param name="columnIndex">The column index we are reading from.</param>
        /// <returns>
        /// A <see cref="Nullable{T}"/> representing the value in the data-record. If the value in the cell
        /// was not <see cref="DBNull.Value"/> will contain the value; otherwise will contain no value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="record"/> was null.</exception>
        /// <exception cref="InvalidCastException">The value in the data-record was not <see cref="DBNull"/> but could not be case to an instance of <typeparamref name="T"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="IDataRecord.FieldCount"/>.</exception>
        public static T? GetNullableValue<T>(this IDataRecord record, int columnIndex)
            where T : struct
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (!record.IsDBNull(columnIndex))
            {
                return record.GetDataValue<T>(columnIndex);
            }
            return null;
        }


        /// <summary>
        /// Gets the value from a given column using a specified column name.
        /// </summary>
        /// <typeparam name="T">The type we are retrieving from the column.</typeparam>
        /// <param name="record">The data-record.</param>
        /// <param name="columnName">The column name we are reading from.</param>
        /// <returns>
        /// A <see cref="Nullable{T}"/> representing the value in the data-record. If the value in the cell
        /// was not <see cref="DBNull.Value"/> will contain the value; otherwise will contain no value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="record"/> was null.</exception>
        /// <exception cref="InvalidCastException">The value in the data-record was not <see cref="DBNull"/> but could not be case to an instance of <typeparamref name="T"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="IDataRecord.FieldCount"/>.</exception>
        public static T? GetNullableValue<T>(this IDataRecord record, string columnName)
            where T : struct
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            int columnIndex = record.GetOrdinal(columnName);
            if (!record.IsDBNull(columnIndex))
            {
                return record.GetDataValue<T>(columnIndex);
            }
            return null;
        }

        /// <summary>
        /// Checks whether or not the data-record contains a given column index.
        /// </summary>
        /// <param name="record">The data-record.</param>
        /// <param name="columnIndex">The column index we are checking for.</param>
        /// <returns>True if the given columnIndex was not negative and less than the record's fieldCount. Otherwise returns false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="record"/> was null.</exception>
        public static bool HasColumn(this IDataRecord record, int columnIndex)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            int fieldCount = record.FieldCount;

            return columnIndex < fieldCount && columnIndex >= 0;
        }

        /// <summary>
        /// Checks whether or not the data-record contains a given column name.
        /// </summary>
        /// <param name="record">The data-record.</param>
        /// <param name="columnName">The name of the column we are checking for.</param>
        /// <returns>True if we were able to match the given columnName to a field in the record. Otherwise returns false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="record"/> was null.</exception>
        public static bool HasColumn(this IDataRecord record, string columnName)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            for (int i = 0; i < record.FieldCount; i++)
            {
                if (record.GetName(i) == columnName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to get the column index of a specified column name for the data-record.
        /// </summary>
        /// <param name="record">The data-record.</param>
        /// <param name="columnIndex">The index of the column we are looking for.</param>
        /// <param name="columnName">The column name. When this method returns columnName will be equal
        /// to the name of the specified column index or will be null if the record does not contain the column.</param>
        /// <returns>True if the record contains the specified column. Otherwise returns false.</returns>
        public static bool TryGetColumnName(this IDataRecord record, int columnIndex, out string columnName)
        {
            columnName = null;

            if (record.HasColumn(columnIndex))
            {
                columnName = record.GetName(columnIndex);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to get the column index of a specified column name for the data-record.
        /// </summary>
        /// <param name="record">The data-record.</param>
        /// <param name="columnName">The name of the column we are looking for.</param>
        /// <param name="columnIndex">The column index. When this method returns columnIndex will be equal
        /// to the index of the specified column name or will be -1 if the record does not contain the column.</param>
        /// <returns>True if the record contains the specified column. Otherwise returns false.</returns>
        public static bool TryGetColumnIndex(this IDataRecord record, string columnName, out int columnIndex)
        {
            columnIndex = -1;

            if (record.HasColumn(columnName))
            {
                columnIndex = record.GetOrdinal(columnName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of T. 
        /// </summary>
        /// <typeparam name="T">The type we are retrieving from the column.</typeparam>
        /// <param name="record">The data-record.</param>
        /// <param name="columnIndex">The column index we are reading from.</param>
        /// <returns>The value retrieved from the DB cell.</returns>
        /// <exception cref="InvalidCastException">The value in the data-record could not be cast to an instance of T.</exception>
        private static T GetDataValue<T>(this IDataRecord record, int columnIndex)
        {
            //If the column index is not found this will throw. Hence all methods calling this should have already verified the columnIndex is valid.
            object value = record[columnIndex];
            try
            {
                return (T)value;
            }
            catch (InvalidCastException icEx)
            {
                throw BuildMoreDetailedException<T>(record.GetName(columnIndex), value, icEx);
            }
        }

        /// <summary>
        /// Creates a more detailed error message from a <see cref="InvalidCastException"/> thrown while attempting
        /// to read from a data record.
        /// </summary>
        /// <typeparam name="T">The type we were attempting to cast the value to.</typeparam>
        /// <param name="columnName">The name of the column we were attempting to read from.</param>
        /// <param name="value">The value retrieved from the data record.</param>
        /// <param name="ex">The original exception.</param>
        /// <returns>An exception detailing the name of column we were attempting to read from and the type mismatch.</returns>
        /// <remarks>
        /// The default <see cref="InvalidCastException"/> thrown by the .NET DataReaders is not very
        /// informative. When they fail to cast an object to the specified type they will just give a 
        /// generic message "Specified cast is not valid". So to make it a bit clearer for developer and support
        /// we create our own exception detailing the column name and the type mismatch that caused the error. This
        /// can then be re-thrown and caught for logging purposes.
        /// 
        /// See http://stackoverflow.com/questions/2817125/c-sharp-find-out-what-column-caused-the-sql-exception
        /// </remarks>
        private static InvalidCastException BuildMoreDetailedException<T>(string columnName, object value, InvalidCastException ex)
        {
            string exceptionMessage = String.Format(System.Globalization.CultureInfo.InvariantCulture,
                "An error occured while attempting to read column {0}. Excepted column type was {1} but actual column type was {2}",
                columnName, typeof(T).Name, value.GetType().Name);

            return new InvalidCastException(exceptionMessage, ex);
        }
    }
}
