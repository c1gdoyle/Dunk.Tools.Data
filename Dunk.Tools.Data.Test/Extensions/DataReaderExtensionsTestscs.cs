using System;
using System.Data;
using System.Linq;
using Dunk.Tools.Data.Extensions;
using NUnit.Framework;

namespace Dunk.Tools.Data.Test.Extensions
{
    [TestFixture]
    public class DataReaderExtensionsTestscs
    {
        private const string TestColumn1Name = "Column_1";
        private const string TestColumn2Name = "Column_2";
        private const string TestColumn3Name = "Column_3";
        private const string TestColumn4Name = "Column_4";

        private const int RowCount = 10;

        [Test]
        public void DataReaderAsEnumerableThrowsIfReaderIsNull()
        {
            IDataReader reader = null;
            Assert.Throws<ArgumentNullException>(() => reader.AsEnumerable());
        }

        [Test]
        public void DataReaderAsEnumerableReturnsEnumerable()
        {
            IDataReader reader = CreateReader();

            var enumerable = reader.AsEnumerable();

            Assert.IsNotNull(enumerable);
        }

        [Test]
        public void DataReaderAsEnumerableReturnsExpectedNumberOfItems()
        {
            IDataReader reader = CreateReader();

            var enumerable = reader.AsEnumerable()
                .ToArray();

            Assert.AreEqual(RowCount, enumerable.Length);

        }

        private DataTableReader CreateReader()
        {
            DataTable table = new DataTable("Test Table");

            var columns = new DataColumn[]
            {
                new DataColumn {ColumnName = TestColumn1Name, DataType = typeof(string) },
                new DataColumn {ColumnName = TestColumn2Name, DataType = typeof(string) },
                new DataColumn {ColumnName = TestColumn3Name, DataType = typeof(string) },
                new DataColumn {ColumnName = TestColumn4Name, DataType = typeof(string) },
            };
            table.Columns.AddRange(columns);

            for (int i = 0; i < RowCount; i++)
            {
                DataRow row = table.NewRow();

                row[TestColumn1Name] = TestColumn1Name + i;
                row[TestColumn2Name] = TestColumn2Name + i;
                row[TestColumn3Name] = TestColumn3Name + i;
                row[TestColumn4Name] = TestColumn4Name + i;

                table.Rows.Add(row);
            }
            return new DataTableReader(table);
        }
    }
}
