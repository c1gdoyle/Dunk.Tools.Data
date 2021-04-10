using System.Linq;
using System.Reflection;

namespace Dunk.Tools.Data.Test.TestUtils
{
    internal static class ConstructorHelper
    {
        public static T Construct<T>(params object[] p)
        {
            var ctor = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(c => c.GetParameters().Count() == p.Count());

            return (T)ctor.Invoke(p);
        }
    }
}
