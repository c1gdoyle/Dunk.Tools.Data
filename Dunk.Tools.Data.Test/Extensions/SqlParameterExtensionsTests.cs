using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Dunk.Tools.Data.Extensions;
using Dunk.Tools.Data.Test.TestUtils;
using NUnit.Framework;

namespace Dunk.Tools.Data.Test.Extensions
{
    [TestFixture]
    public class SqlParameterExtensionsTests
    {
        [Test]
        public void ToSqlParameterWithSelectorReturnsParameterWithExpectedValueAndName()
        {
            const int expectedValue = 1;
            const string parameterName = "ID";

            Student obj = new Student
            {
                Id = 1,
                Name = "Tom",
                Age = 16
            };
            Expression<Func<Student, int>> selector = s => s.Id;

            SqlParameter param = obj.ToSqlParameter(selector);

            Assert.IsNotNull(param);
            Assert.AreEqual(expectedValue, param.Value);
            Assert.AreEqual(parameterName.ToUpper(), param.ParameterName);
        }

        [Test]
        public void ToSqlParameterWithSelectorThrowsIfObjectIsNull()
        {
            Student obj = null;
            Expression<Func<Student, int>> selector = s => s.Id;

            Assert.Throws<ArgumentNullException>(() => obj.ToSqlParameter(selector));
        }

        [Test]
        public void ToSqlParameterWithSelectorThrowsIfSelectorIsNull()
        {
            Student obj = new Student
            {
                Id = 1,
                Name = "Tom",
                Age = 16
            };
            Expression<Func<Student, int>> selector = null;

            Assert.Throws<ArgumentNullException>(() => obj.ToSqlParameter(selector));
        }

        [Test]
        public void ToSqlParameterReturnsParameterWithExpectedValueAndName()
        {
            const string expectedValue = "foo";
            const string parameterName = "param_1";

            SqlParameter param = expectedValue.ToSqlParameter(parameterName);

            Assert.IsNotNull(param);
            Assert.AreEqual(expectedValue, param.Value);
            Assert.AreEqual(parameterName.ToUpper(), param.ParameterName);
        }

        [Test]
        public void ToSqlParameterReturnsParameterWithExpectedValueForNull()
        {
            const string expectedValue = null;
            const string parameterName = "param_1";

            SqlParameter param = expectedValue.ToSqlParameter(parameterName);

            Assert.IsNotNull(param);
            Assert.AreEqual(DBNull.Value, param.Value);
            Assert.AreEqual(parameterName.ToUpper(), param.ParameterName);
        }

        [Test]
        public void ToSqlParameterThrowsIfParameterNameIsNull()
        {
            string value = "foo";
            Assert.Throws<ArgumentException>(() => value.ToSqlParameter(null));
        }

        [Test]
        public void ToSqlParameterThrowsIfParameterNameIsEmpty()
        {
            string value = "foo";
            Assert.Throws<ArgumentException>(() => value.ToSqlParameter(string.Empty));
        }

        [Test]
        public void ToSqlParameterThrowsIfParameterNameIfWhiteSpace()
        {
            string value = "foo";
            Assert.Throws<ArgumentException>(() => value.ToSqlParameter("  "));
        }

        [Test]
        public void ToSqlParametersArrayReturnsExpectedArray()
        {
            Student obj = new Student
            {
                Id = 1,
                Name = "Tom",
                Age = 16
            };
            var expectedArray = new[]
            {
                new SqlParameter(nameof(Student.Id).ToUpper(), obj.Id),
                new SqlParameter(nameof(Student.Name).ToUpper(), obj.Name),
                new SqlParameter(nameof(Student.Age).ToUpper(), obj.Age),
            };

            var array = obj.ToSqlParametersArray();

            Assert.IsNotNull(array);
            Assert.AreEqual(expectedArray.Length, array.Length);
        }

        [Test]
        public void ToSqlParametersArrayThrowsIfObjectIsNull()
        {
            Student obj = null;
            Assert.Throws<ArgumentNullException>(() => obj.ToSqlParametersArray());
        }

        [Test]
        public void ToSqlParametersListReturnsExpectedList()
        {
            Student obj = new Student
            {
                Id = 1,
                Name = "Tom",
                Age = 16
            };
            var expectedList = new List<SqlParameter>
            {
                new SqlParameter(nameof(Student.Id).ToUpper(), obj.Id),
                new SqlParameter(nameof(Student.Name).ToUpper(), obj.Name),
                new SqlParameter(nameof(Student.Age).ToUpper(), obj.Age),
            };

            var list = obj.ToSqlParametersList();

            Assert.IsNotNull(list);
            Assert.AreEqual(expectedList.Count, list.Count);
        }

        [Test]
        public void ToSqlParametersListThrowsIfObjectIsNull()
        {
            Student obj = null;
            Assert.Throws<ArgumentNullException>(() => obj.ToSqlParametersList());
        }

        [Test]
        public void ToEnumerableThrowsIfSqlParameterCollectionIsNull()
        {
            SqlParameterCollection collection = null;
            Assert.Throws<ArgumentNullException>(() => collection.ToEnumerable());
        }

        [Test]
        public void ToEnumerableReturnsIfSqlParameterCollectionIsNotNull()
        {
            var collection = ConstructorHelper.Construct<SqlParameterCollection>();
            var enumerable = collection.ToEnumerable();

            Assert.IsNotNull(enumerable);
        }

        [Test]
        public void ToEnumerableThrowsIfSqlErrorCollectionIsNull()
        {
            SqlErrorCollection collection = null;
            Assert.Throws<ArgumentNullException>(() => collection.ToEnumerable());
        }

        [Test]
        public void ToEnumerableReturnsIfSqlErrorCollectionIsNotNull()
        {
            SqlErrorCollection collection = ConstructorHelper.Construct<SqlErrorCollection>();
            var enumerable = collection.ToEnumerable();

            Assert.IsNotNull(enumerable);
        }

        private class Student
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
