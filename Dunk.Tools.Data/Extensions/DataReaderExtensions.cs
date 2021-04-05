using System;
using System.Collections.Generic;
using System.Data;

namespace Dunk.Tools.Data.Extensions
{
    /// <summary>
    /// Provides a series of extension methods for a <see cref="IDataReader"/> instance.
    /// </summary>
    public static class DataReaderExtensions
    {
        /// <summary>
        /// Converts a <see cref="IDataReader"/> into an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="source">The reader to convert.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> that supports iterating over the reader.
        /// </returns>
        /// <remarks>
        /// Note due to the nature of a <see cref="IDataReader"/> the enumerable only supports
        /// reading once.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> cannot be null.</exception>
        public static IEnumerable<T> AsEnumerable<T>(this T source)
            where T : IDataReader
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return BuildEnumerable(source.Read, () => source);
        }

        private static IEnumerable<T> BuildEnumerable<T>(Func<bool> moveNext, Func<T> current)
        {
            var enumeratorWrapper = new EnumeratorWrapper<T>(moveNext, current);
            foreach (var s in enumeratorWrapper)
            {
                yield return s;
            }
        }

        /// <summary>
        /// A helper class that converts a data-source into an
        /// enumerable like collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// This differs from a <see cref="System.Collections.Generic.IEnumerable{T}"/> in that
        /// the underlying data-source does not support resetting the enumerator to the 
        /// initial position.
        /// </remarks>
        internal class EnumeratorWrapper<T>
        {
            private readonly Func<bool> _moveNext;
            private readonly Func<T> _current;

            public EnumeratorWrapper(Func<bool> moveNext, Func<T> current)
            {
                if (moveNext == null)
                {
                    throw new ArgumentNullException($"{nameof(moveNext)}");
                }

                if (current == null)
                {
                    throw new ArgumentNullException($"{nameof(current)}");
                }
                _moveNext = moveNext;
                _current = current;
            }

            /// <summary>
            /// Returns an <see cref="EnumeratorWrapper{T}"/> that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="EnumeratorWrapper{T}"/> that can be used to iterate through the
            /// collection.
            /// </returns>
            public EnumeratorWrapper<T> GetEnumerator()
            {
                return this;
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// <c>true</c> if the enumerator was successfully advanced to the next; <c>false</c> if
            /// the enumerator has passsed the end of the collection.
            /// </returns>
            public bool MoveNext()
            {
                return _moveNext();
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public T Current
            {
                get { return _current(); }
            }
        }
    }
}
