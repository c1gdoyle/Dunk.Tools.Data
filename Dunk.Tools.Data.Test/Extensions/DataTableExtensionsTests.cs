using System;
using System.Data;
using Dunk.Tools.Data.Extensions;
using NUnit.Framework;

namespace Dunk.Tools.Data.Test.Extensions
{
    [TestFixture]
    public class DataTableExtensionsTests
    {
        [Test]
        public void RemoveColumnByNameThrowsIfTableIsNull()
        {
            DataTable table = null;
            Assert.Throws<ArgumentNullException>(() => table.RemoveColumn("Column_Name"));
        }

        [Test]
        public void RemoveColumnByNameThrowsIfColumnNameIsNullOrEmpty()
        {
            DataTable table = new DataTable();
            Assert.Throws<ArgumentNullException>(() => table.RemoveColumn(""));
        }

        [Test]
        public void RemoveColumnByNameThrowsIfTableDoesNotContainColumn()
        {
            DataTable table = new DataTable();
            Assert.Throws<ArgumentException>(() => table.RemoveColumn("Column_Name"));
        }

        [Test]
        public void RemoveColumnByNameRemovesColumn()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Test_Column1", typeof(string)));

            table.RemoveColumn("Test_Column1");

            Assert.IsFalse(table.Columns.Contains("Test_Column1"));
        }

        [Test]
        public void RenameColumnThrowsIfTableIsNull()
        {
            DataTable table = null;
            Assert.Throws<ArgumentNullException>(() => table.RenameColumn("Test_OldColumn", "Test_NewColumn"));
        }

        [Test]
        public void RenameColumnThrowsIfOldColumnNameIsNull()
        {
            DataTable table = new DataTable();
            Assert.Throws<ArgumentNullException>(() => table.RenameColumn(null, "Test_NewColumn"));
        }

        [Test]
        public void RenameColumnThrowsIfNewColumnNameIsNull()
        {
            DataTable table = new DataTable();
            Assert.Throws<ArgumentNullException>(() => table.RenameColumn("Test_OldColumn", null));
        }

        [Test]
        public void RenameColumnThrowsIfOldNameAndNewNameAreTheSame()
        {
            DataTable table = new DataTable();
            Assert.Throws<ArgumentException>(() => table.RenameColumn("Test_OldColumn", "Test_OldColumn"));
        }

        [Test]
        public void RenameColumnThrowsIfTableDoesNotContainOldColumnName()
        {
            DataTable table = new DataTable();
            Assert.Throws<ArgumentException>(() => table.RenameColumn("Test_OldColumn", "Test_NewColumn"));
        }

        [Test]
        public void RenameColumnRenamesColumn()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Test_OldColumn", typeof(string)));
            table.RenameColumn("Test_OldColumn", "Test_NewColumn");

            Assert.IsFalse(table.Columns.Contains("Test_OldColumn"));
            Assert.IsTrue(table.Columns.Contains("Test_NewColumn"));
        }
    }
}
