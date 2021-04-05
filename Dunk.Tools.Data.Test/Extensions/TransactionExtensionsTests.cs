using System;
using System.Data.Common;
using Dunk.Tools.Data.Extensions;
using Moq;
using NUnit.Framework;

namespace Dunk.Tools.Data.Test.Extensions
{
    [TestFixture]
    public class TransactionExtensionsTests
    {
        [Test]
        public void TryCommitTransactionThrowsIfTransactionIsNull()
        {
            DbTransaction transaction = null;
            Assert.Throws<ArgumentNullException>(() => transaction.TryCommitTransaction());
        }

        [Test]
        public void TryCommitTransactionThrowsIfErrorHandlerIsNull()
        {
            Mock<DbTransaction> transaction = new Mock<DbTransaction>();
            Action<Exception> errorHandler = null;

            Assert.Throws<ArgumentNullException>(() => transaction.Object.TryCommitTransaction(errorHandler));
        }

        [Test]
        public void TryCommitTransactionReturnsTrueIfCommitSucceeds()
        {
            Mock<DbTransaction> transaction = new Mock<DbTransaction>();
            Assert.IsTrue(transaction.Object.TryCommitTransaction());
        }

        [Test]
        public void TryCommitTransactionReturnsFalseIfCommitFails()
        {
            Mock<DbTransaction> transaction = new Mock<DbTransaction>();
            transaction.Setup(t => t.Commit()).Throws(new Exception("Error committing transaction"));

            Assert.IsFalse(transaction.Object.TryCommitTransaction());
        }

        [Test]
        public void TryCommitTransactionInvokesErrorHandlerIfCommitFails()
        {
            Exception error = null;

            Mock<DbTransaction> transaction = new Mock<DbTransaction>();
            transaction.Setup(t => t.Commit()).Throws(new Exception("Error committing transaction"));

            Action<Exception> errorHandler = e => error = e;

            transaction.Object.TryCommitTransaction(errorHandler);

            Assert.IsNotNull(error);
        }

        [Test]
        public void TryRollbackTransactionThrowsIfTransactionIsNull()
        {
            DbTransaction transaction = null;
            Assert.Throws<ArgumentNullException>(() => transaction.TryRollbackTransaction());
        }

        [Test]
        public void TryRollbackTransactionThrowsIfErrorHandlerIsNull()
        {
            Mock<DbTransaction> transaction = new Mock<DbTransaction>();
            Action<Exception> errorHandler = null;

            Assert.Throws<ArgumentNullException>(() => transaction.Object.TryRollbackTransaction(errorHandler));
        }

        [Test]
        public void TryRollbackTransactionReturnsTrueIfCommitSucceeds()
        {
            Mock<DbTransaction> transaction = new Mock<DbTransaction>();
            Assert.IsTrue(transaction.Object.TryRollbackTransaction());
        }

        [Test]
        public void TryRollbackTransactionReturnsFalseIfCommitFails()
        {
            Mock<DbTransaction> transaction = new Mock<DbTransaction>();
            transaction.Setup(t => t.Rollback()).Throws(new Exception("Error rolling back transaction"));

            Assert.IsFalse(transaction.Object.TryRollbackTransaction());
        }

        [Test]
        public void TryRollbackTransactionInvokesErrorHandlerIfCommitFails()
        {
            Exception error = null;

            Mock<DbTransaction> transaction = new Mock<DbTransaction>();
            transaction.Setup(t => t.Rollback()).Throws(new Exception("Error rolling back transaction"));

            Action<Exception> errorHandler = e => error = e;

            transaction.Object.TryRollbackTransaction(errorHandler);

            Assert.IsNotNull(error);
        }
    }
}
