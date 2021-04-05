using System;
using System.Data.Common;

namespace Dunk.Tools.Data.Extensions
{
    /// <summary>
    /// Provides a series of extension methods for a <see cref="DbTransaction"/>
    /// </summary>
    public static class TransactionExtensions
    {
        /// <summary>
        /// Attempts to commit a transaction to the database.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns>
        /// <c>true</c> if the transaction is successfully committed; otherwise returns <c>false</c>.
        /// </returns>
        public static bool TryCommitTransaction(this DbTransaction transaction)
        {
            return TryCommitTransaction(transaction, e => { });
        }

        /// <summary>
        /// Attempts to commit a transaction to the database with a specified error handler.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="errorHandler">The error-handler delegate that will be invoked if any exception occurs committing the transaction.</param>
        /// <returns>
        /// <c>true</c> if the transaction is successfully committed; otherwise returns <c>false</c>.
        /// </returns>
        public static bool TryCommitTransaction(this DbTransaction transaction, Action<Exception> errorHandler)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), 
                    $"Unable to commit transaction. {nameof(transaction)} parameter cannot be null");
            }
            if (errorHandler == null)
            {
                throw new ArgumentNullException(nameof(errorHandler), 
                    $"Unable to commit transaction. {nameof(errorHandler)} parameter cannot be null");
            }

            try
            {
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                errorHandler(ex);
                return false;
            }
        }

        /// <summary>
        /// Attempts to rollback a transaction from the database.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns>
        /// <c>true</c> if the transaction is successfully rolled back; otherwise returns <c>false</c>.
        /// </returns>
        public static bool TryRollbackTransaction(this DbTransaction transaction)
        {
            return TryRollbackTransaction(transaction, e => { });
        }

        /// <summary>
        /// Attempts to rollback a transaction from the database with a specified error handler.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="errorHandler">The error-handler delegate that will be invoked if any exception occurs rolling back the transaction.</param>
        /// <returns>
        /// <c>true</c> if the transaction is successfully rolled back; otherwise returns <c>false</c>.
        /// </returns>
        public static bool TryRollbackTransaction(this DbTransaction transaction, Action<Exception> errorHandler)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), 
                    $"Unable to rollback transaction. {nameof(transaction)} parameter cannot be null");
            }
            if (errorHandler == null)
            {
                throw new ArgumentNullException(nameof(errorHandler), 
                    $"Unable to rollback transaction. {nameof(errorHandler)} parameter cannot be null");
            }

            try
            {
                transaction.Rollback();
                return true;
            }
            catch (Exception ex)
            {
                errorHandler(ex);
                return false;
            }
        }
    }
}
