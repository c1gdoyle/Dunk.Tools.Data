using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Dunk.Tools.Data.Extensions;
using NUnit.Framework;

namespace Dunk.Tools.Data.Test.Extensions
{
    [TestFixture]
    public class EnumerableDataTableExtensionsTests
    {
        [Test]
        public void EnumerableToDataTableReturnsTable()
        {
            IEnumerable<TestDataItem> items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            DataTable table = items.ToDataTable();

            Assert.IsNotNull(table);
        }

        [Test]
        public void EnumerableToDataTableMapsColumnsToDataTable()
        {
            IEnumerable<TestDataItem> items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            DataTable table = items.ToDataTable();

            Assert.IsTrue(table.Columns.Contains("Id"));
            Assert.IsTrue(table.Columns.Contains("Date"));
        }

        [Test]
        public void EnumerableToDataTableMapsColumnTypesToDataTable()
        {
            IEnumerable<TestDataItem> items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            DataTable table = items.ToDataTable();

            Assert.AreEqual(typeof(int), table.Columns["Id"].DataType);
            Assert.AreEqual(typeof(DateTime), table.Columns["Date"].DataType);
        }

        [Test]
        public void EnumerableToDataTableOnlyMapsColumnsForBasicTypesByDefault()
        {
            IEnumerable<TestDataItem> items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            DataTable table = items.ToDataTable();

            Assert.IsFalse(table.Columns.Contains("ItemTags"));
        }

        [Test]
        public void EnumerableToDataTableCreatesDataRowsForEachItem()
        {
            IEnumerable<TestDataItem> items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            DataTable table = items.ToDataTable();

            Assert.AreEqual(3, table.Rows.Count);
        }

        [Test]
        public void EnumerableToDataTableMapsItemDataToTableRows()
        {
            IEnumerable<TestDataItem> items = new List<TestDataItem>
            {
                new TestDataItem {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItem {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItem {Id=3, Date = new DateTime(2017,03,08) }
            };

            DataTable table = items.ToDataTable();

            DataRow row = table.Rows[0];

            Assert.AreEqual(1, (int)row["Id"]);
            Assert.AreEqual(new DateTime(2017, 03, 06), (DateTime)row["Date"]);
        }

        [Test]
        public void EnumerableToDataTableMapsUnderlyingTypeForNullableType()
        {
            IEnumerable<TestDateItemsWithNullableProps> items = new List<TestDateItemsWithNullableProps>
            {
                new TestDateItemsWithNullableProps {Id=1, Date = new DateTime(2017,03,06), Price = 11.1 },
                new TestDateItemsWithNullableProps {Id=2, Date = new DateTime(2017,03,07), Price = 2.22 },
                new TestDateItemsWithNullableProps {Id=3, Date = new DateTime(2017,03,08), Price = null }
            };

            DataTable table = items.ToDataTable();

            Assert.AreEqual(typeof(double), table.Columns["Price"].DataType);
        }

        [Test]
        public void EnumerableToDataTableMapsDbNullForNullData()
        {
            IEnumerable<TestDateItemsWithNullableProps> items = new List<TestDateItemsWithNullableProps>
            {
                new TestDateItemsWithNullableProps {Id=1, Date = new DateTime(2017,03,06), Price = 11.1 },
                new TestDateItemsWithNullableProps {Id=2, Date = new DateTime(2017,03,07), Price = 2.22 },
                new TestDateItemsWithNullableProps {Id=3, Date = new DateTime(2017,03,08), Price = null }
            };

            DataTable table = items.ToDataTable();

            DataRow row = table.Rows[2];

            Assert.AreEqual(DBNull.Value, row["Price"]);
        }

        [Test]
        public void EnumerableToDataTableMapsColumnsUsingPredicate()
        {
            IEnumerable<TestDataItemWithVirtualProps> items = new List<TestDataItemWithVirtualProps>
            {
                new TestDataItemWithVirtualProps {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItemWithVirtualProps {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItemWithVirtualProps {Id=3, Date = new DateTime(2017,03,08) }
            };

            Func<PropertyInfo, bool> filter = p => !p.GetGetMethod().IsVirtual || p.GetGetMethod().IsFinal;

            DataTable table = items.ToDataTable(filter);

            Assert.IsTrue(!table.Columns.Contains("ItemsTags"));
            Assert.IsTrue(!table.Columns.Contains("Price"));
        }

        [Test]
        public void EnumerableToDataTableMapsColumnsUsingPredicateForBasicTypesOnly()
        {
            IEnumerable<TestDataItemWithVirtualProps> items = new List<TestDataItemWithVirtualProps>
            {
                new TestDataItemWithVirtualProps {Id=1, Date = new DateTime(2017,03,06) },
                new TestDataItemWithVirtualProps {Id=2, Date = new DateTime(2017,03,07) },
                new TestDataItemWithVirtualProps {Id=3, Date = new DateTime(2017,03,08) }
            };

            Func<PropertyInfo, bool> filter = p => !p.GetGetMethod().IsVirtual || p.GetGetMethod().IsFinal;

            DataTable table = items.ToDataTable(filter);

            Assert.IsTrue(!table.Columns.Contains("HistoricDates"));
        }

        private class TestDataItem
        {
            public int Id { get; set; }

            public DateTime Date { get; set; }

            public ICollection<string> ItemTags { get; set; }
        }

        private class TestDataItemWithVirtualProps
        {
            public int Id { get; set; }

            public DateTime Date { get; set; }

            public virtual double Price { get; set; }

            public virtual ICollection<string> ItemTags { get; set; }

            public ICollection<DateTime> HistoricDates { get; set; }
        }

        public class TestDateItemsWithNullableProps
        {
            public int Id { get; set; }

            public DateTime Date { get; set; }

            public double? Price { get; set; }
        }
    }
}
