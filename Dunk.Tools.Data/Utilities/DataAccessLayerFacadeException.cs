using System;
using System.Data.Common;

namespace Dunk.Tools.Data.Utilities
{
    /// <summary>
    /// An exception that is thrown when a call to the data-base returns a warning or
    /// error.
    /// </summary>
    [Serializable]
    public class DataAccessLayerFacadeException : Exception
    {
        /// <summary>
        /// Initialises a new instance of <see cref="DataAccessLayerFacadeException"/> with a specified message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public DataAccessLayerFacadeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of <see cref="DataAccessLayerFacadeException"/> with a specified message
        /// and a reference to the inner exception that was the cause of this exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The exception thrown by the datasource that is the cause of this exception.</param>
        public DataAccessLayerFacadeException(string message, DbException innerException)
            : base(message, innerException)
        {
        }
    }
}
