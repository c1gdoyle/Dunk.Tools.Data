using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dunk.Tools.Data.Base;
using Dunk.Tools.Data.Core;
using Moq;
using NUnit.Framework;

namespace Dunk.Tools.Data.Test.Core
{
    [TestFixture]
    public class SqlBulkOperationWriterTests
    {
        [Test]
        public void SqlBulkOperationWriterInitialises()
        {
            var bulkWriter = new SqlBulkOperationWriter(() => new SqlConnection());
            Assert.IsNotNull(bulkWriter);
        }

        [Test]
        public void SqlBulkOperationWriterInsertsWritesToSpecifiedTable()
        {
            string tableName = null;
            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);

            var bulkCopy = new Mock<IBulkCopy>();
            bulkCopy.SetupSet(b => b.DestinationTableName = It.IsAny<string>())
                .Callback<string>(s => tableName = s);

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);
            bulkWriter.BulkInsert(GetStudentsData(), "Students");

            Assert.AreEqual("Students", tableName);
        }

        [Test]
        public void SqlBulkOperationWriterInsertsWritesDataToServer()
        {
            DataTable table = null;

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);

            var bulkCopy = new Mock<IBulkCopy>();
            bulkCopy.Setup(b => b.WriteToServer(It.IsAny<DataTable>()))
                .Callback<DataTable>(dt => table = dt);

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);
            bulkWriter.BulkInsert(GetStudentsData(), "Students");

            Assert.IsNotNull(table);
        }

        [Test]
        public void SqlBulkOperationWriterInsertWritesExpectedNumberOfRows()
        {
            DataTable table = null;

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);

            var bulkCopy = new Mock<IBulkCopy>();
            bulkCopy.Setup(b => b.WriteToServer(It.IsAny<DataTable>()))
                .Callback<DataTable>(dt => table = dt);

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);
            bulkWriter.BulkInsert(GetStudentsData(), "Students");

            Assert.AreEqual(3, table.Rows.Count);
        }

        [Test]
        public void SqlBUlkOperationWriterInsertWritesDataInBatchesIfSpecified()
        {
            int? batchSize = null;

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);

            var bulkCopy = new Mock<IBulkCopy>();
            bulkCopy.SetupSet(b => b.BatchSize = It.IsAny<int?>())
                .Callback<int?>(i => batchSize = i);

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);
            bulkWriter.BulkInsert(GetStudentsData(), "Students", 1000);

            Assert.AreEqual(1000, batchSize.Value);
        }

        [Test]
        public void SqlBulkOperationWriterUpdateQueriesInformationSchema()
        {
            List<string> commands = new List<string>();
            const string expectedSchemaQuery =
                "SELECT * " +
                "FROM information_schema.columns " +
                "WHERE table_name = 'Students'";

            Mock<IDataReader> reader = CreateDataReader();

            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupSet(c => c.CommandText = It.IsAny<string>())
                .Callback<string>(s => commands.Add(s));

            command.Setup(s => s.ExecuteReader())
                .Returns(reader.Object);

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);

            var bulkCopy = new Mock<IBulkCopy>();

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);

            bulkWriter.BulkUpdate(GetStudentsData(), "Students", new[] { "StudentId" });

            Assert.AreEqual(expectedSchemaQuery, commands[0]);
        }

        [Test]
        public void SqlBulkOperationWriterUpdateCreatesTemporaryTable()
        {
            List<string> commands = new List<string>();
            const string expectedTempTableCreateQuery =
                "CREATE TABLE #TempTable " +
                "([StudentId] int NOT NULL,[StudentName] varchar(100) NOT NULL,[DateOfBirth] datetime2 NOT NULL,[TestScore] decimal(10,10) NOT NULL)";

            Mock<IDataReader> reader = CreateDataReader();

            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupSet(c => c.CommandText = It.IsAny<string>())
                .Callback<string>(s => commands.Add(s));

            command.Setup(s => s.ExecuteReader())
                .Returns(reader.Object);

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);

            var bulkCopy = new Mock<IBulkCopy>();

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);

            bulkWriter.BulkUpdate(GetStudentsData(), "Students", new[] { "StudentId" });

            Assert.AreEqual(expectedTempTableCreateQuery, commands[1]);
        }

        [Test]
        public void SqlBulkOperationWriterUpdateWritesDataToTemporaryTable()
        {
            const string tempTableName = "#TempTable";

            List<string> commands = new List<string>();

            string tableName = null;
            DataTable table = null;

            Mock<IDataReader> reader = CreateDataReader();

            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupSet(c => c.CommandText = It.IsAny<string>())
                .Callback<string>(s => commands.Add(s));

            command.Setup(s => s.ExecuteReader())
                .Returns(reader.Object);

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);

            var bulkCopy = new Mock<IBulkCopy>();
            bulkCopy.SetupSet(b => b.DestinationTableName = It.IsAny<string>())
                .Callback<string>(s => tableName = s);
            bulkCopy.Setup(b => b.WriteToServer(It.IsAny<DataTable>()))
                .Callback<DataTable>(dt => table = dt);

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);

            bulkWriter.BulkUpdate(GetStudentsData(), "Students", new[] { "StudentId" });

            Assert.IsNotNull(table);
            Assert.AreEqual(tempTableName, tableName);
        }

        [Test]
        public void SqlBulkOperationWriterUpdateUpdatesDataInTargetTableByJoiningOnTemporaryTable()
        {
            List<string> commands = new List<string>();
            const string expectedUpdateStatement =
                "UPDATE target " +
                "SET target.StudentName=source.StudentName,target.DateOfBirth=source.DateOfBirth,target.TestScore=source.TestScore " +
                "FROM Students target " +
                "INNER JOIN #TempTable source ON target.StudentId = source.StudentId";

            Mock<IDataReader> reader = CreateDataReader();

            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupSet(c => c.CommandText = It.IsAny<string>())
                .Callback<string>(s => commands.Add(s));

            command.Setup(s => s.ExecuteReader())
                .Returns(reader.Object);

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);

            var bulkCopy = new Mock<IBulkCopy>();

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);

            bulkWriter.BulkUpdate(GetStudentsData(), "Students", new[] { "StudentId" });

            Assert.AreEqual(expectedUpdateStatement, commands[2]);
        }

        [Test]
        public void SqlBulkOperationWriterUpsertQueiesInformationSchema()
        {
            List<string> fieldsToMatch = new List<string> { "StudentId" };

            List<string> commands = new List<string>();
            const string expectedSchemaQuery =
                "SELECT * " + "" +
                "FROM information_schema.columns " +
                "WHERE table_name = 'Students'";

            Mock<IDataReader> reader = CreateDataReader();

            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupSet(c => c.CommandText = It.IsAny<string>())
                .Callback<string>(s => commands.Add(s));

            command.Setup(s => s.ExecuteReader())
                .Returns(reader.Object);

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);

            var bulkCopy = new Mock<IBulkCopy>();

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);

            bulkWriter.BulkUpsert(GetStudentsData(), "Students", new[] { "StudentId" }, fieldsToMatch);

            Assert.AreEqual(expectedSchemaQuery, commands[0]);
        }

        [Test]
        public void SqlBulkOperationWriterUpsertCreatesTemporaryTable()
        {
            List<string> fieldsToMatch = new List<string> { "StudentId" };

            List<string> commands = new List<string>();
            const string expectedTempTableCreateQuery =
                "CREATE TABLE #TempTable " +
                "([StudentId] int NOT NULL,[StudentName] varchar(100) NOT NULL,[DateOfBirth] datetime2 NOT NULL,[TestScore] decimal(10,10) NOT NULL)";

            Mock<IDataReader> reader = CreateDataReader();

            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupSet(c => c.CommandText = It.IsAny<string>())
                .Callback<string>(s => commands.Add(s));

            command.Setup(s => s.ExecuteReader())
                .Returns(reader.Object);

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);

            var bulkCopy = new Mock<IBulkCopy>();

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);

            bulkWriter.BulkUpsert(GetStudentsData(), "Students", new[] { "StudentId" }, fieldsToMatch);

            Assert.AreEqual(expectedTempTableCreateQuery, commands[1]);
        }

        [Test]
        public void SqlBulkOperationWriterUpsertWritesDataToTemporaryTable()
        {
            const string tempTableName = "#TempTable";

            List<string> fieldsToMatch = new List<string> { "StudentId" };

            List<string> commands = new List<string>();

            string tableName = null;
            DataTable table = null;

            Mock<IDataReader> reader = CreateDataReader();

            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupSet(c => c.CommandText = It.IsAny<string>())
                .Callback<string>(s => commands.Add(s));

            command.Setup(s => s.ExecuteReader())
                .Returns(reader.Object);

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);

            var bulkCopy = new Mock<IBulkCopy>();
            bulkCopy.SetupSet(b => b.DestinationTableName = It.IsAny<string>())
                .Callback<string>(s => tableName = s);
            bulkCopy.Setup(b => b.WriteToServer(It.IsAny<DataTable>()))
                .Callback<DataTable>(dt => table = dt);

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);

            bulkWriter.BulkUpsert(GetStudentsData(), "Students", new[] { "StudentId" }, fieldsToMatch);

            Assert.IsNotNull(table);
            Assert.AreEqual(tempTableName, tableName);
        }

        [Test]
        public void SqlBulkOperationWriterUpsertMergesUpdatesDataInTargetTableWithTemporaryTable()
        {
            List<string> fieldsToMatch = new List<string> { "StudentId" };

            List<string> commands = new List<string>();
            const string expectedUpsertStatement =
                "MERGE INTO Students AS target " +
                "USING #TempTable AS source " +
                "ON target.StudentId=source.StudentId " +
                "WHEN MATCHED THEN UPDATE SET target.StudentName=source.StudentName,target.DateOfBirth=source.DateOfBirth,target.TestScore=source.TestScore " +
                "WHEN NOT MATCHED THEN INSERT (StudentName,DateOfBirth,TestScore) VALUES (source.StudentName,source.DateOfBirth,source.TestScore);";

            Mock<IDataReader> reader = CreateDataReader();

            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupSet(c => c.CommandText = It.IsAny<string>())
                .Callback<string>(s => commands.Add(s));

            command.Setup(s => s.ExecuteReader())
                .Returns(reader.Object);

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);

            var bulkCopy = new Mock<IBulkCopy>();

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);

            bulkWriter.BulkUpsert(GetStudentsData(), "Students", new[] { "StudentId" }, fieldsToMatch);

            Assert.AreEqual(expectedUpsertStatement, commands[2]);
        }

        [Test]
        public void SqlBulkOperationWriterDeleteQueriesInformationSchema()
        {
            List<string> commands = new List<string>();
            const string expectedSchemaQuery =
                "SELECT * " +
                "FROM information_schema.columns " +
                "WHERE table_name = 'Students'";

            Mock<IDataReader> reader = CreateDataReader();

            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupSet(c => c.CommandText = It.IsAny<string>())
                .Callback<string>(s => commands.Add(s));

            command.Setup(s => s.ExecuteReader())
                .Returns(reader.Object);

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);

            var bulkCopy = new Mock<IBulkCopy>();

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);

            bulkWriter.BulkDelete(GetStudentsData(), "Students", new[] { "StudentId" });

            Assert.AreEqual(expectedSchemaQuery, commands[0]);
        }

        [Test]
        public void SqlBulkOperationWriterDeleteCreatesTemporaryTable()
        {
            List<string> commands = new List<string>();
            const string expectedTempTableCreateQuery = "CREATE TABLE #TempTable " +
                "([StudentId] int NOT NULL,[StudentName] varchar(100) NOT NULL,[DateOfBirth] datetime2 NOT NULL,[TestScore] decimal(10,10) NOT NULL)";

            Mock<IDataReader> reader = CreateDataReader();

            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupSet(c => c.CommandText = It.IsAny<string>())
                .Callback<string>(s => commands.Add(s));

            command.Setup(s => s.ExecuteReader())
                .Returns(reader.Object);

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);

            var bulkCopy = new Mock<IBulkCopy>();

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);

            bulkWriter.BulkDelete(GetStudentsData(), "Students", new[] { "StudentId" });

            Assert.AreEqual(expectedTempTableCreateQuery, commands[1]);
        }

        [Test]
        public void SqlBulkOperationWriterDeleteWritesDataToTemporaryTable()
        {
            const string tempTableName = "#TempTable";

            List<string> commands = new List<string>();

            string tableName = null;
            DataTable table = null;

            Mock<IDataReader> reader = CreateDataReader();

            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupSet(c => c.CommandText = It.IsAny<string>())
                .Callback<string>(s => commands.Add(s));

            command.Setup(s => s.ExecuteReader())
                .Returns(reader.Object);

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);

            var bulkCopy = new Mock<IBulkCopy>();
            bulkCopy.SetupSet(b => b.DestinationTableName = It.IsAny<string>())
                .Callback<string>(s => tableName = s);
            bulkCopy.Setup(b => b.WriteToServer(It.IsAny<DataTable>()))
                .Callback<DataTable>(dt => table = dt);

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);

            bulkWriter.BulkDelete(GetStudentsData(), "Students", new[] { "StudentId" });

            Assert.IsNotNull(table);
            Assert.AreEqual(tempTableName, tableName);
        }

        [Test]
        public void SqlBulkOperationWriterDeleteDeletesDataFromTargetTableByJoiningOnTemporaryTable()
        {
            List<string> commands = new List<string>();
            const string expectedDeleteStatement =
                "DELETE target " + "" +
                "FROM Students target " +
                "INNER JOIN #TempTable source ON target.StudentId = source.StudentId";

            Mock<IDataReader> reader = CreateDataReader();

            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupSet(c => c.CommandText = It.IsAny<string>())
                .Callback<string>(s => commands.Add(s));

            command.Setup(s => s.ExecuteReader())
                .Returns(reader.Object);

            var connection = new Mock<IDbConnection>();
            connection.Setup(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);

            var bulkCopy = new Mock<IBulkCopy>();

            var bulkWriter = new SqlBulkOperationWriter(() => connection.Object, c => bulkCopy.Object);

            bulkWriter.BulkDelete(GetStudentsData(), "Students", new[] { "StudentId" });

            Assert.AreEqual(expectedDeleteStatement, commands[2]);
        }

        private IEnumerable<Student> GetStudentsData()
        {
            return new List<Student>
            {
                new Student {StudentId = 1, StudentName= "Tom", DateOfBirth = DateTime.Now, TestScore = 123.45m },
                new Student {StudentId = 2, StudentName= "Dick", DateOfBirth = DateTime.Now, TestScore = 123.45m },
                new Student {StudentId = 3, StudentName= "Harry", DateOfBirth = DateTime.Now, TestScore = 123.45m },
            };
        }

        private Mock<IDataReader> CreateDataReader()
        {
            var dataReader = new Mock<IDataReader>();

            dataReader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(true)
                .Returns(true)
                .Returns(true)
                .Returns(false);

            dataReader.SetupSequence(r => r["COLUMN_NAME"])
                .Returns("StudentId")
                .Returns("StudentName")
                .Returns("DateOfBirth")
                .Returns("TestScore");

            dataReader.SetupSequence(r => r["DATA_TYPE"])
                .Returns("int")
                .Returns("varchar")
                .Returns("datetime2")
                .Returns("decimal");

            dataReader.SetupSequence(r => r["IS_NULLABLE"])
                .Returns("NO")
                .Returns("NO")
                .Returns("NO")
                .Returns("NO");

            dataReader.SetupSequence(r => r["CHARACTER_MAXIMUM_LENGTH"])
                .Returns(DBNull.Value.ToString())
                .Returns("100")
                .Returns(DBNull.Value.ToString())
                .Returns(DBNull.Value.ToString());

            dataReader.SetupSequence(r => r["NUMERIC_PRECISION"])
                .Returns(DBNull.Value.ToString())
                .Returns("10")
                .Returns(DBNull.Value.ToString())
                .Returns("10");

            dataReader.SetupSequence(r => r["NUMERIC_SCALE"])
                .Returns(DBNull.Value.ToString())
                .Returns("0")
                .Returns(DBNull.Value.ToString())
                .Returns("10");

            return dataReader;
        }

        private class Student
        {
            public int StudentId { get; set; }

            public string StudentName { get; set; }

            public DateTime? DateOfBirth { get; set; }

            public decimal? TestScore { get; set; }
        }
    }
}
