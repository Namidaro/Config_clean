﻿using System;
using NModbus4.Utility;
using Xunit;

namespace NModbus4.UnitTests.Utility
{
    public class DiscriminatedUnionFixture
    {
        [Fact]
        public void DiscriminatedUnion_CreateA()
        {
            var du = DiscriminatedUnion<string, string>.CreateA("foo");
            Assert.Equal(DiscriminatedUnionOption.A, du.Option);
            Assert.Equal((string) "foo", (string) du.A);
        }

        [Fact]
        public void DiscriminatedUnion_CreateB()
        {
            var du = DiscriminatedUnion<string, string>.CreateB("foo");
            Assert.Equal(DiscriminatedUnionOption.B, du.Option);
            Assert.Equal((string) "foo", (string) du.B);
        }

        [Fact]
        public void DiscriminatedUnion_AllowNulls()
        {
            var du = DiscriminatedUnion<object, object>.CreateB(null);
            Assert.Equal(DiscriminatedUnionOption.B, du.Option);
            Assert.Equal(null, du.B);
        }

        [Fact]
        public void AccessInvalidOption_A()
        {
            var du = DiscriminatedUnion<string, string>.CreateB("foo");
            Assert.Throws<InvalidOperationException>(() => du.A.ToString());
        }

        [Fact]
        public void AccessInvalidOption_B()
        {
            var du = DiscriminatedUnion<string, string>.CreateA("foo");
            Assert.Throws<InvalidOperationException>(() => du.B.ToString());
        }

        [Fact]
        public void DiscriminatedUnion_ToString()
        {
            var du = DiscriminatedUnion<string, string>.CreateA("foo");
            Assert.Equal((string) du.ToString(), "foo");
        }
    }
}