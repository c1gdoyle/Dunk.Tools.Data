using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dunk.Tools.Data.Base;
using Dunk.Tools.Data.Core;
using Dunk.Tools.Data.Test.Stubs;
using Dunk.Tools.Data.Test.TestUtils;
using Dunk.Tools.Data.Utilities;
using Moq;
using NUnit.Framework;

namespace Dunk.Tools.Data.Test.Core
{
    [TestFixture]
    public class DataAccessLayerFacadeTests
    {
        private const string TableName = "TestTable";

        private const int NumberOfColumns = 3;
        private const int NumberOfRows = 5;

        [Test]
        public void DataAccessLayerFacadeIntialises()
        {
            Mock<IDbConnection> connection = MockDbConnection();
            IDataAccessLayerFacade facade = new DataAccessLayerFacade(() => connection.Object);

            Assert.IsNotNull(facade);
        }

        [Test]
        public void DataAccessLayerFacadeCreatesTable()
        {
            Mock<IDbConnection> connection = MockDbConnection();
            IDataAccessLayerFacade facade = new DataAccessLayerFacade(() => connection.Object);

            var table = facade.FillTable("SELECT * FROM Anywhere", TableName);

            Assert.IsNotNull(table);
        }

        [Test]
        public void DataAccessLayerFacadeCreatesTableWithCorrectName()
        {
            Mock<IDbConnection> connection = MockDbConnection();
            IDataAccessLayerFacade facade = new DataAccessLayerFacade(() => connection.Object);

            var table = facade.FillTable("SELECT * FROM Anywhere", TableName);

            Assert.AreEqual(TableName, table.TableName);
        }

        [Test]
        public void DataAccessLayerFacadeCreatesTableWithExceptedNumberOfColumns()
        {
            Mock<IDbConnection> connection = MockDbConnection();
            IDataAccessLayerFacade facade = new DataAccessLayerFacade(() => connection.Object);

            var table = facade.FillTable("SELECT * FROM Anywhere", TableName);

            Assert.AreEqual(NumberOfColumns, table.Columns.Count);
        }

        [Test]
        public void DataAccessLayerFacadeCreatesTableWithExceptedColumnNames()
        {
            Mock<IDbConnection> connection = MockDbConnection();
            IDataAccessLayerFacade facade = new DataAccessLayerFacade(() => connection.Object);

            var table = facade.FillTable("SELECT * FROM Anywhere", TableName);

            Assert.AreEqual("Id", table.Columns[0].ColumnName);
            Assert.AreEqual("Name", table.Columns[1].ColumnName);
            Assert.AreEqual("Salary", table.Columns[2].ColumnName);
        }

        [Test]
        public void DataAccessLayerFacadeCreatesTableWithExceptedColumnTypes()
        {
            Mock<IDbConnection> connection = MockDbConnection();
            IDataAccessLayerFacade facade = new DataAccessLayerFacade(() => connection.Object);

            var table = facade.FillTable("SELECT * FROM Anywhere", TableName);

            Assert.AreEqual(typeof(int), table.Columns[0].DataType);
            Assert.AreEqual(typeof(string), table.Columns[1].DataType);
            Assert.AreEqual(typeof(double), table.Columns[2].DataType);
        }

        [Test]
        public void DataAccessLayerFacadeCreatesTableWithExpectedNumberOfRows()
        {
            Mock<IDbConnection> connection = MockDbConnection();
            IDataAccessLayerFacade facade = new DataAccessLayerFacade(() => connection.Object);

            var table = facade.FillTable("SELECT * FROM Anywhere", TableName);

            Assert.AreEqual(NumberOfRows, table.Rows.Count);
        }

        [Test]
        public void DataAccessLayerFacadeThrowsIfQueryFails()
        {
            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.CreateCommand())
                .Throws(new DbStubException());

            IDataAccessLayerFacade facade = new DataAccessLayerFacade(() => connection.Object);

            Assert.Throws<DataAccessLayerFacadeException>(() => facade.FillTable("SELECT * FROM Anywhere", TableName));
        }

        [Test]
        public void DataAccessLayerFacadeThrowsSerialisableExceptionIfQueryFails()
        {
            DataAccessLayerFacadeException error = null;
            DataTable table = null;

            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.CreateCommand())
                .Throws(new DbStubException());

            IDataAccessLayerFacade facade = new DataAccessLayerFacade(() => connection.Object);

            try
            {
                table = facade.FillTable("SELECT * FROM Anywhere", TableName);
            }
            catch (DataAccessLayerFacadeException ex)
            {
                error = TestSerialisationHelper.SerialiseAndDeserialiseException(ex);
            }

            Assert.IsNull(table);
            Assert.IsNotNull(error);
        }

        [Test]
        public void DataAccessLayerFacadeThrowsWhenRetryCountIsExceeded()
        {
            int retryCount = 0;

            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.CreateCommand())
                .Callback(() => retryCount++)
                .Throws(new DbStubException());

            IDataAccessLayerFacade facade = new DataAccessLayerFacade(() => connection.Object);

            Assert.Throws<DataAccessLayerFacadeException>(() => facade.FillTable("SELECT * FROM Anywhere", TableName));
            Assert.AreEqual(3, retryCount);
        }

        [Test]
        public void DataAccessLayerFacadeThrowsSerialisableExceptionWhenRetryCountIsExceeded()
        {
            DataAccessLayerFacadeException error = null;
            DataTable table = null;
            int retryCount = 0;

            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.CreateCommand())
                .Callback(() => retryCount++)
                .Throws(new DbStubException());

            IDataAccessLayerFacade facade = new DataAccessLayerFacade(() => connection.Object);

            try
            {
                table = facade.FillTable("SELECT * FROM Anywhere", TableName);
            }
            catch (DataAccessLayerFacadeException ex)
            {
                error = TestSerialisationHelper.SerialiseAndDeserialiseException(ex);
            }

            Assert.IsNull(table);
            Assert.IsNotNull(error);
            Assert.AreEqual(3, retryCount);
        }

        private Mock<IDbConnection> MockDbConnection()
        {
            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataReader> reader = MockDataReader();

            connection.Setup(c => c.CreateCommand()).Returns(command.Object);
            command.Setup(cmd => cmd.ExecuteReader()).Returns(reader.Object);

            return connection;
        }

        private Mock<IDataReader> MockDataReader()
        {
            Mock<IDataReader> reader = new Mock<IDataReader>();
            //current position in the rows to enumerate
            int count = -1;

            reader.Setup(r => r.Read())
                //  return true while we still have a row
                .Returns(() => count < NumberOfRows - 1)
                //  go to the next position
                .Callback(() => count++);

            //fields
            reader.Setup(r => r.FieldCount).Returns(NumberOfColumns);

            reader.Setup(r => r.GetName(0)).Returns("Id");
            reader.Setup(r => r.GetFieldType(0)).Returns(typeof(int));

            reader.Setup(r => r.GetName(1)).Returns("Name");
            reader.Setup(r => r.GetFieldType(1)).Returns(typeof(string));

            reader.Setup(r => r.GetName(2)).Returns("Salary");
            reader.Setup(r => r.GetFieldType(2)).Returns(typeof(double));

            //just markt content as null for now
            reader.Setup(r => r.GetValue(It.IsAny<int>())).Returns(DBNull.Value);

            return reader;
        }
    }
}
