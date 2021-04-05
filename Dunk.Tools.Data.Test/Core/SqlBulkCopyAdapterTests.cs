using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dunk.Tools.Data.Core;
using Dunk.Tools.Data.Extensions;
using Dunk.Tools.Data.Test.Stubs;
using Moq;
using NUnit.Framework;

namespace Dunk.Tools.Data.Test.Core
{
    [TestFixture]
    public class SqlBulkCopyAdapterTests
    {
        [Test]
        public void BulkCopyAdapterThrowsIfConnectionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlBulkCopyAdapter(null as IDbConnection));
        }

        [Test]
        public void BulkCopyAdapterThrowsIfConnectionIsNotOpen()
        {
            SqlConnection sqlConnection = new SqlConnection();

            Assert.Throws<ArgumentException>(() => new SqlBulkCopyAdapter(sqlConnection));
        }

        [Test]
        public void BulkCopyAdapterInitialises()
        {
            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);

            var bulkCopy = new SqlBulkCopyAdapter(connection.Object);

            Assert.IsNotNull(bulkCopy);
        }

        [Test]
        public void BulkCopyAdapterCreatesDataTableFromItems()
        {
            var items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);

            var bulkCopy = new SqlBulkCopyStub(connection.Object)
            {
                DestinationTableName = "TestDateItems"
            };

            bulkCopy.WriteToServer(items, p => !p.GetGetMethod().IsVirtual || p.GetGetMethod().IsFinal);

            Assert.IsNotNull(bulkCopy.BulkCopyData);
        }

        [Test]
        public void BulkCopyCreatesSqlBulkCopyMappingsFromDataTable()
        {
            var items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);

            var bulkCopy = new SqlBulkCopyStub(connection.Object)
            {
                DestinationTableName = "TestDateItems"
            };

            bulkCopy.WriteToServer(items, p => !p.GetGetMethod().IsVirtual || p.GetGetMethod().IsFinal);

            var columnMappings = bulkCopy.BulkCopyColumnMappings;

            Assert.IsNotNull(columnMappings);
            Assert.AreEqual(2, columnMappings.Count());
        }

        [Test]
        public void BulkCopyCreatesSqlBulkCopyMappingsWithExpectedSourceColumnNames()
        {
            const string sourceColumn1 = "Id";
            const string sourceColumn2 = "Date";

            var items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);

            var bulkCopy = new SqlBulkCopyStub(connection.Object)
            {
                DestinationTableName = "TestDateItems"
            };

            bulkCopy.WriteToServer(items, p => !p.GetGetMethod().IsVirtual || p.GetGetMethod().IsFinal);

            var columnMappings = bulkCopy.BulkCopyColumnMappings.ToArray();

            Assert.AreEqual(sourceColumn1, columnMappings[0].SourceColumn);
            Assert.AreEqual(sourceColumn2, columnMappings[1].SourceColumn);
        }

        [Test]
        public void BulkCopyCreatesSqlBulkCopyMappingsWithExpectedDestinationColumnNames()
        {
            const string destinationColumn1 = "Id";
            const string destinationColumn2 = "Date";

            var items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);

            var bulkCopy = new SqlBulkCopyStub(connection.Object)
            {
                DestinationTableName = "TestDateItems"
            };

            bulkCopy.WriteToServer(items, p => !p.GetGetMethod().IsVirtual || p.GetGetMethod().IsFinal);

            var columnMappings = bulkCopy.BulkCopyColumnMappings.ToArray();

            Assert.AreEqual(destinationColumn1, columnMappings[0].DestinationColumn);
            Assert.AreEqual(destinationColumn2, columnMappings[1].DestinationColumn);
        }

        [Test]
        public void BulkCopyWritesToDestinationTable()
        {
            const string testDestinationTableName = "TestDataItems";

            var items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);

            var bulkCopy = new SqlBulkCopyStub(connection.Object)
            {
                DestinationTableName = testDestinationTableName
            };

            bulkCopy.WriteToServer(items, p => !p.GetGetMethod().IsVirtual || p.GetGetMethod().IsFinal);

            var destinationTableName = bulkCopy.DestinationTableName;

            Assert.AreEqual(testDestinationTableName, destinationTableName);
        }

        [Test]
        public void BulkCopyGetsDestinationTableNameFromSpecifiedDataTableAsFallback()
        {
            const string testDestinationTableName = "TestDataItems";

            var items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);

            var bulkCopy = new SqlBulkCopyStub(connection.Object);

            DataTable table = items.ToDataTable(p => !p.GetGetMethod().IsVirtual || p.GetGetMethod().IsFinal);
            table.TableName = testDestinationTableName;

            bulkCopy.WriteToServer(table);

            var destinationTableName = bulkCopy.BulkCopyDestinationTableName;

            Assert.AreEqual(testDestinationTableName, destinationTableName);
        }

        [Test]
        public void BulkCopyThrowsIfDestinationTableNameIsNotAvailableOnBulkCopyOrDataTable()
        {
            var items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);

            var bulkCopy = new SqlBulkCopyStub(connection.Object);

            DataTable table = items.ToDataTable(p => !p.GetGetMethod().IsVirtual || p.GetGetMethod().IsFinal);

            Assert.Throws<ArgumentException>(() => bulkCopy.WriteToServer(table));

        }

        private class TestDataItem
        {
            public int Id { get; set; }

            public DateTime Date { get; set; }

            public virtual ICollection<string> ItemTags { get; set; }
        }
    }
}
