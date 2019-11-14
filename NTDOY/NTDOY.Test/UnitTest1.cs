using System;
using Xunit;

namespace NTDOY.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            int x = 1;
            int y = 1;
            Assert.Equal(2,x + y );
        }
        [Theory]
        [InlineData(-1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, -3)]
        public void test2(int val1, int val2)
        {
            Assert.True(val1 + val2 > -1);

        }
    }
}
