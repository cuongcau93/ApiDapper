using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace Euroland.NetCore.ToolsFramework.Data.Test
{
    public class ReflectionTypeUtilsTest
    {
        [Fact]
        public void CanDetectPrimitiveType()
        {
            Assert.True(TypeUtils.IsPrimitive(typeof(int)));
            Assert.True(TypeUtils.IsPrimitive(typeof(int?)));
            Assert.True(TypeUtils.IsPrimitive(typeof(float)));
            Assert.True(TypeUtils.IsPrimitive(typeof(Nullable<float>)));
            Assert.True(TypeUtils.IsPrimitive(typeof(double)));
            Assert.True(TypeUtils.IsPrimitive(typeof(Nullable<double>)));
            Assert.True(TypeUtils.IsPrimitive(typeof(long)));
            Assert.True(TypeUtils.IsPrimitive(typeof(Nullable<long>)));
            Assert.True(TypeUtils.IsPrimitive(typeof(string)));
        }

        [Fact]
        public void CanDetectIEnumerableType()
        {
            Assert.True(TypeUtils.IsIEnumerable(typeof(string[])));
            Assert.True(TypeUtils.IsIEnumerable(typeof(List<string>)));
            Assert.True(TypeUtils.IsIEnumerable(typeof(IEnumerable<string>)));
        }
      
    }
}
