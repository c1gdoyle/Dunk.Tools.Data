using System;
using System.Data;
using System.Linq;
using Dunk.Tools.Data.Extensions;
using NUnit.Framework;

namespace Dunk.Tools.Data.Test.Extensions
{
    [TestFixture]
    public class DataRowExtensionsTests
    {
        [Test]
        public void ToDictionaryThrowsIfRowIsNull()
        {
            DataRow row = null;
            Assert.Throws<ArgumentNullException>(() => row.ToDictionary());
        }

        [Test]
        public void ToDictionaryConvertsColumnsToKeys()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Test_Id", typeof(int)));

            var row = dt.NewRow();
            row["Test_Id"] = 47;

            var dictionary = row.ToDictionary();

            Assert.IsTrue(dictionary.ContainsKey("Test_Id"));
        }

        [Test]
        public void ToDictionaryConvertsCellContentsToValues()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Test_Id", typeof(int)));

            var row = dt.NewRow();
            row["Test_Id"] = 47;

            var dictionary = row.ToDictionary();

            object value = dictionary["Test_Id"];

            Assert.AreEqual(47, value);
        }

        [Test]
        public void GetColumnNameThrowsIfRowIsNull()
        {
            DataRow row = null;
            Assert.Throws<ArgumentNullException>(() => row.GetColumnName(0));
        }

        [Test]
        public void GetColumnNameThrowsIfOriginalIsNegative()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Test_Id", typeof(int)));

            var row = dt.NewRow();
            row["Test_Id"] = 47;

            Assert.Throws<ArgumentOutOfRangeException>(() => row.GetColumnName(-1));
        }

        [Test]
        public void GetColumnNameThrowsIfOrdinalIsGreaterThanArrayLength()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Test_Id", typeof(int)));

            var row = dt.NewRow();
            row["Test_Id"] = 47;

            Assert.Throws<ArgumentOutOfRangeException>(() => row.GetColumnName(1));
        }

        [Test]
        public void GetColumnNameReturnsColumnName()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Test_Id", typeof(int)));

            var row = dt.NewRow();
            row["Test_Id"] = 47;

            string columnName = row.GetColumnName(0);

            Assert.AreEqual("Test_Id", columnName);
        }

        [Test]
        public void ItemArrayToStringThrowsIfRowIsNull()
        {
            DataRow row = null;
            Assert.Throws<ArgumentNullException>(() => row.ItemArrayToString());
        }

        [Test]
        public void ItemArrayToStringProducesCommaSeparatedString()
        {
            const string expectedString = "47,432.134";

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Test_Id", typeof(int)));
            dt.Columns.Add(new DataColumn("Test_Price", typeof(double)));

            var row = dt.NewRow();
            row["Test_Id"] = 47;
            row["Test_Price"] = 432.134;

            string result = row.ItemArrayToString();

            Assert.AreEqual(expectedString, result);
        }

        [Test]
        public void ItemArrayToStringProducesStringSeparatedBySuppliedSeparator()
        {
            const string expectedString = "47;432.134";

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Test_Id", typeof(int)));
            dt.Columns.Add(new DataColumn("Test_Price", typeof(double)));

            var row = dt.NewRow();
            row["Test_Id"] = 47;
            row["Test_Price"] = 432.134;

            string result = row.ItemArrayToString(";");

            Assert.AreEqual(expectedString, result);
        }
    }
}
