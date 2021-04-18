using System;
using System.Data;
using Dunk.Tools.Data.Extensions;
using NUnit.Framework;

namespace Dunk.Tools.Data.Test.Extensions
{
    [TestFixture]
    public class DataRecordExtensionsTests
    {
        [Test]
        public void ReaderGetValueOrDefaultThrowsIfRecordIsNull()
        {
            IDataRecord record = null;
            Assert.Throws<ArgumentNullException>(() => record.GetValueOrDefault<bool>(0));
        }

        [Test]
        public void ReaderGetValueOrDefaultColumnNameThrowsIfRecordIsNull()
        {
            IDataRecord record = null;
            Assert.Throws<ArgumentNullException>(() => record.GetValueOrDefault<bool>("boolean_Column"));
        }

        [Test]
        public void ReaderGetValueOrDefaultGetsValueForColumnIndex()
        {
            const bool booleanValue = true;
            const string columnName = "boolean_Column";

            DataTable table = CreateTestTable(columnName, typeof(bool), booleanValue);
            DataTableReader reader = new DataTableReader(table);
            reader.Read();
            bool result = reader.GetValueOrDefault<bool>(0);

            Assert.AreEqual(booleanValue, result);
        }

        [Test]
        public void ReaderGetValueOrDefaultGetsValueForColumnName()
        {
            const bool booleanValue = true;
            const string columnName = "boolean_Column";

            DataTable table = CreateTestTable(columnName, typeof(bool), booleanValue);
            DataTableReader reader = new DataTableReader(table);
            reader.Read();
            bool result = reader.GetValueOrDefault<bool>(columnName);

            Assert.AreEqual(booleanValue, result);
        }

        [Test]
        public void ReaderGetValueOrDefaultGetsDefaultIfCellIsDbNullForColumnIndex()
        {
            const string columnName = "boolean_Column";

            DataTable table = CreateTestTable(columnName, typeof(bool), DBNull.Value);
            DataTableReader reader = new DataTableReader(table);
            reader.Read();
            bool result = reader.GetValueOrDefault<bool>(0);

            Assert.AreEqual(default(bool), result);
        }

        [Test]
        public void ReaderGetValueOrDefaultGetsDefaultIfCellIsDbNullForColumnName()
        {
            const string columnName = "boolean_Column";

            DataTable table = CreateTestTable(columnName, typeof(bool), DBNull.Value);
            DataTableReader reader = new DataTableReader(table);
            reader.Read();
            bool result = reader.GetValueOrDefault<bool>(columnName);

            Assert.AreEqual(default(bool), result);
        }

        [Test]
        public void ReaderGetValueOrDefaultThrowsIfCellIsNotExpectedTypeForColumnIndex()
        {
            const bool booleanValue = true;
            const string columnName = "boolean_Column";

            DataTable table = CreateTestTable(columnName, typeof(bool), booleanValue);
            DataTableReader reader = new DataTableReader(table);
            reader.Read();
            Assert.Throws<InvalidCastException>(() => reader.GetValueOrDefault<char>(0));
        }

        [Test]
        public void ReaderGetValueOrDefaultThrowsIfCellIsNotExpectedTypeForColumnName()
        {
            const bool booleanValue = true;
            const string columnName = "boolean_Column";

            DataTable table = CreateTestTable(columnName, typeof(bool), booleanValue);
            DataTableReader reader = new DataTableReader(table);
            reader.Read();
            Assert.Throws<InvalidCastException>(() => reader.GetValueOrDefault<char>(columnName));
        }

        [Test]
        public void ReaderGetNullableValueThrowsIfRecordIsNull()
        {
            IDataRecord record = null;
            Assert.Throws<ArgumentNullException>(() => record.GetNullableValue<bool>(0));
        }

        [Test]
        public void ReaderGetNullableValueColumnThrowsIfRecordIsNull()
        {
            IDataRecord record = null;
            Assert.Throws<ArgumentNullException>(() => record.GetNullableValue<bool>("boolean_Column"));
        }

        [Test]
        public void ReaderGetNullableValueGetsValueForColumnIndex()
        {
            const bool booleanValue = true;
            const string columnName = "boolean_Column";

            DataTable table = CreateTestTable(columnName, typeof(bool), booleanValue);
            DataTableReader reader = new DataTableReader(table);
            reader.Read();
            bool? result = reader.GetNullableValue<bool>(0);

            Assert.AreEqual(booleanValue, result);
        }

        [Test]
        public void ReaderGetNullableValueGetsValueForColumnName()
        {
            const bool booleanValue = true;
            const string columnName = "boolean_Column";

            DataTable table = CreateTestTable(columnName, typeof(bool), booleanValue);
            DataTableReader reader = new DataTableReader(table);
            reader.Read();
            bool? result = reader.GetNullableValue<bool>(columnName);

            Assert.AreEqual(booleanValue, result);
        }

        [Test]
        public void ReaderGetNullableValueGetsNullIfCellIsDbNullForColumnIndex()
        {
            const string columnName = "boolean_Column";

            DataTable table = CreateTestTable(columnName, typeof(bool), DBNull.Value);
            DataTableReader reader = new DataTableReader(table);
            reader.Read();
            bool? result = reader.GetNullableValue<bool>(0);

            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void ReaderGetNullableValueGetsNullIfCellIsDbNullForColumnName()
        {
            const string columnName = "boolean_Column";

            DataTable table = CreateTestTable(columnName, typeof(bool), DBNull.Value);
            DataTableReader reader = new DataTableReader(table);
            reader.Read();
            bool? result = reader.GetNullableValue<bool>(columnName);

            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void ReaderGetNullableValueThrowsIfCellNotExpectedTypeForColumnIndex()
        {
            const bool booleanValue = true;
            const string columnName = "boolean_Column";

            DataTable table = CreateTestTable(columnName, typeof(bool), booleanValue);
            DataTableReader reader = new DataTableReader(table);
            reader.Read();
            Assert.Throws<InvalidCastException>(() => reader.GetNullableValue<char>(0));
        }

        [Test]
        public void ReaderGetNullableValueThrowsIfCellNotExpectedTypeForColumnName()
        {
            const bool booleanValue = true;
            const string columnName = "boolean_Column";

            DataTable table = CreateTestTable(columnName, typeof(bool), booleanValue);
            DataTableReader reader = new DataTableReader(table);
            reader.Read();
            Assert.Throws<InvalidCastException>(() => reader.GetNullableValue<char>(columnName));
        }

        [Test]
        public void ReaderHasColumnThrowsIfRecordIsNull()
        {
            IDataRecord record = null;
            Assert.Throws<ArgumentNullException>(() => record.HasColumn(0));
        }

        [Test]
        public void ReaderHasColumnColumnNameThrowsIfRecordIsNull()
        {
            IDataRecord record = null;
            Assert.Throws<ArgumentNullException>(() => record.HasColumn("boolean_Column"));
        }

        [Test]
        public void RecordHasColumnReturnsTrueIfColumnIndexIsFound()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            Assert.IsTrue(reader.HasColumn(0));
        }

        [Test]
        public void RecordHasColumnReturnsFalseIfColumnIndexIsLessThanZero()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            Assert.IsFalse(reader.HasColumn(-1));
        }

        [Test]
        public void RecordHasColumnReturnsFalseIfColumnIndexIsGreaterThanFieldCount()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            Assert.IsFalse(reader.HasColumn(1));
        }

        [Test]
        public void RecordHasColumnReturnsTrueIfColumnNameIsFound()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            Assert.IsTrue(reader.HasColumn(columnName));
        }

        [Test]
        public void RecordHasColumnReturnsFalseIfColumnNameIsNotFound()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            Assert.IsFalse(reader.HasColumn("Aardvark"));
        }

        [Test]
        public void RecordTryGetColumnNameReturnsTrueIfColumnIndexExists()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            string actualColumnName;
            Assert.IsTrue(reader.TryGetColumnName(0, out actualColumnName));
        }

        [Test]
        public void RecordTryGetColumnNameReturnsColumnNameWithValueIfColumnIndexExists()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            string actualColumnName;
            reader.TryGetColumnName(0, out actualColumnName);

            Assert.IsNotNull(actualColumnName);
            Assert.AreEqual(columnName, actualColumnName);
        }

        [Test]
        public void RecordTryGetColumnNameReturnsFalseIfColumnIndexDoesNotExist()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            string actualColumnName;
            Assert.IsFalse(reader.TryGetColumnName(-1, out actualColumnName));
        }

        [Test]
        public void RecordTryGetColumnNameReturnsColumnNameWithNullValueIfColumnIndexDoesNotExist()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            string actualColumnName;
            reader.TryGetColumnName(-1, out actualColumnName);

            Assert.IsNull(actualColumnName);
        }

        [Test]
        public void RecordTryGetColumnIndexReturnsTrueIfColumnNameExists()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            int actualColumnIndex;
            Assert.IsTrue(reader.TryGetColumnIndex(columnName, out actualColumnIndex));
        }

        [Test]
        public void RecordTryGetColumnIndexReturnsColumnIndexWithValueIfColumnNameExists()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            int actualColumnIndex;
            reader.TryGetColumnIndex(columnName, out actualColumnIndex);

            Assert.AreEqual(0, actualColumnIndex);
        }

        [Test]
        public void RecordTryGetColumnIndexReturnsFalseIfColumnNameDoesNotExist()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            int actualColumnIndex;
            Assert.IsFalse(reader.TryGetColumnIndex("Aardvark", out actualColumnIndex));
        }

        [Test]
        public void RecordTryGetColumnIndexReturnsColumnIndexWithMinusValueIfColumnNameDoesNotExist()
        {
            string columnName = "Test_Column";

            DataTable table = CreateTestTable(columnName, typeof(int), default(int));
            DataTableReader reader = new DataTableReader(table);

            reader.Read();

            int actualColumnIndex;
            reader.TryGetColumnIndex("Aardvark", out actualColumnIndex);

            Assert.AreEqual(-1, actualColumnIndex);
        }


        private DataTable CreateTestTable(string columnName, Type columnType, object columnValue)
        {
            DataTable table = new DataTable("Test Table");
            var column = new DataColumn(columnName, columnType);
            table.Columns.Add(column);
            DataRow row = table.NewRow();
            row[columnName] = columnValue;
            table.Rows.Add(row);

            return table;
        }
    }
}
