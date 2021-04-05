using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Dunk.Tools.Data.Extensions
{
    /// <summary>
    /// Provides a series of extension methods for converting a <see cref="IEnumerable{T}"/>
    /// instance into an ADO .NET <see cref="DataTable"/>
    /// </summary>
    public static class EnumerableDataTableExtensions
    {
        private static readonly Type[] BasicDataTypes = new[]
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(bool),
            typeof(Guid),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(byte[]),
            typeof(string)
        };

        /// <summary>
        /// Converts a given <see cref="IEnumerable{T}"/> into a  <see cref="DataTable"/>
        /// </summary>
        /// <typeparam name="T">The type of elements in the data sequence.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to create the DataTable from.</param>
        /// <returns>
        /// A <see cref="DataTable"/> that contains data matching the elements in the
        /// input sequence.
        /// </returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> source)
            where T : class
        {
            return ToDataTable(source.ToList());
        }

        /// <summary>
        /// Converts a given <see cref="IEnumerable{T}"/> into a <see cref="DataTable"/> with a specified
        /// predicate to filter the properties of <typeparamref name="T"/> to be included in the table.
        /// </summary>
        /// <typeparam name="T">The type of elements in the data sequence.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to create the DataTable from.</param>
        /// <param name="filter">The predicate to use to filter the properties of <typeparamref name="T"/> to be included in the table.</param>
        /// <returns>
        /// A <see cref="DataTable"/> that contains data matching the elements in the
        /// input sequence.
        /// </returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> source, Func<PropertyInfo, bool> filter)
            where T : class
        {
            return ToDataTable(source.ToList(), filter);
        }


        /// <summary>
        /// Converts a given <see cref="IList{T}"/> into a  <see cref="DataTable"/>
        /// </summary>
        /// <typeparam name="T">The type of elements in the data sequence.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to create the DataTable from.</param>
        /// <returns>
        /// A <see cref="DataTable"/> that contains data matching the elements in the
        /// input sequence.
        /// </returns>
        public static DataTable ToDataTable<T>(this IList<T> source)
            where T : class
        {
            var properties = typeof(T).GetProperties()
                .Where(p => IsBasicType(p.PropertyType));

            return ToDataTable(source, properties);
        }

        /// <summary>
        /// Converts a given <see cref="IList{T}"/> into a <see cref="DataTable"/> with a specified
        /// predicate to filter the properties of <typeparamref name="T"/> to be included in the table.
        /// </summary>
        /// <typeparam name="T">The type of elements in the data sequence.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to create the DataTable from.</param>
        /// <param name="filter">The predicate to use to filter the properties of <typeparamref name="T"/> to be included in the table.</param>
        /// <returns>
        /// A <see cref="DataTable"/> that contains data matching the elements in the
        /// input sequence.
        /// </returns>
        public static DataTable ToDataTable<T>(this IList<T> source, Func<PropertyInfo, bool> filter)
            where T : class
        {
            var properties = typeof(T).GetProperties()
                .Where(filter)
                .Where(p => IsBasicType(p.PropertyType));

            return ToDataTable(source, properties);
        }

        private static bool IsBasicType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type.IsEnum || BasicDataTypes.Contains(type);
        }

        private static DataTable ToDataTable<T>(IList<T> data, IEnumerable<PropertyInfo> properties)
        {
            var table = new DataTable();

            //map the columns
            foreach (var property in properties)
            {
                table.Columns.Add(new DataColumn(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType));
            }

            //map the rows
            foreach (T item in data)
            {
                var row = table.NewRow();

                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
