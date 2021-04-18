using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Dunk.Tools.Data.Extensions
{
    /// <summary>
    /// Provides extension methods for converting object properties into 
    /// a <see cref="SqlParameter"/>.
    /// </summary>
    public static class SqlParameterExtensions
    {
        /// <summary>
        /// Converts a property on an object to a <see cref="SqlParameter"/> using a specified selector.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <typeparam name="TProp">The type of the property to select.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="propertySelector">The property selector.</param>
        /// <returns>
        /// A <see cref="SqlParameter"/>.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="obj"/> or <paramref name="propertySelector"/> was null or empty.</exception>
        public static SqlParameter ToSqlParameter<T, TProp>(this T obj, Expression<Func<T, TProp>> propertySelector)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj),
                    $"Unable to convert to SqlParameter, {nameof(obj)} cannot be null");
            }
            if (propertySelector == null)
            {
                throw new ArgumentNullException(nameof(propertySelector),
                    $"Unable to convert to SqlParameter, {nameof(propertySelector)} cannot be null");
            }
            string parameterName = ((MemberExpression)propertySelector.Body).Member.Name;
            TProp value = propertySelector.Compile()(obj);
            return ToSqlParameter(value, parameterName);
        }

        /// <summary>
        /// Converts a value into a <see cref="SqlParameter"/> with a specified parameter-name.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">The parameter-name.</param>
        /// <returns>
        /// A <see cref="SqlParameter"/>.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="parameterName"/> was null or empty.</exception>
        public static SqlParameter ToSqlParameter<T>(this T value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName) ||
                 parameterName.Length == 0)
            {
                throw new ArgumentException($"Unable to convert to SqlParameter, {nameof(parameterName)} cannot be null or empty",
                    nameof(parameterName));
            }
            return value != null ?
                new SqlParameter(parameterName.ToUpperInvariant(), value) :
                new SqlParameter(parameterName.ToUpperInvariant(), DBNull.Value);
        }

        /// <summary>
        /// Converts a specified object properties into an array of <see cref="SqlParameter"/>s.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="obj">The object to convert.</param>
        /// <returns>
        /// An array of <see cref="SqlParameter"/>s based on the <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> was be null.</exception>
        public static SqlParameter[] ToSqlParametersArray<T>(this T obj)
            => ToSqlParameters(obj).ToArray();

        /// <summary>
        /// Converts a specified object properties into a list of <see cref="SqlParameter"/>s.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="obj">The object to convert.</param>
        /// <returns>
        /// A list of <see cref="SqlParameter"/>s based on the <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> was be null.</exception>
        public static IList<SqlParameter> ToSqlParametersList<T>(this T obj)
            => ToSqlParameters(obj).ToList();

        /// <summary>
        /// Converts a specified object properties into a enumerable of <see cref="SqlParameter"/>s.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="obj">The object to convert.</param>
        /// <returns>
        /// An enumerable of <see cref="SqlParameter"/>s based on the <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> was be null.</exception>
        public static IEnumerable<SqlParameter> ToSqlParameters<T>(this T obj)
        {
            CheckToSqlParametersArguments(obj);
            foreach (var prop in typeof(T).GetProperties())
            {
                string parameterName = prop.Name;
                object value = prop.GetValue(obj);

                yield return value.ToSqlParameter(parameterName);
            }
        }

        /// <summary>
        /// Converts a <see cref="SqlParameterCollection"/> to an enumerable.
        /// </summary>
        /// <param name="collection">The Sql-Parameter collection.</param>
        /// <returns>
        /// An enumerable containing the items from the <paramref name="collection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> was null.</exception>
        public static IEnumerable<SqlParameter> ToEnumerable(this SqlParameterCollection collection)
        {
            if(collection == null)
            {
                throw new ArgumentNullException(nameof(collection),
                    $"Unable to convert to Sql-Parameter enumerable, {nameof(collection)} cannot be null");
            }
            return collection.Cast<SqlParameter>();
        }

        /// <summary>
        /// Converts a <see cref="SqlErrorCollection"/> to an enumerable.
        /// </summary>
        /// <param name="collection">The Sql-Error collection.</param>
        /// <returns>
        /// An enumerable containing the items from the <paramref name="collection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> was null.</exception>
        public static IEnumerable<SqlError> ToEnumerable(this SqlErrorCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection),
                    $"Unable to convert to Sql-Error enumerable, {nameof(collection)} cannot be null");
            }
            return collection.Cast<SqlError>();
        }

        private static void CheckToSqlParametersArguments<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj),
                    $"Unable to convert to SqlParameters, {nameof(obj)} cannot be null");
            }
        }
    }
}
