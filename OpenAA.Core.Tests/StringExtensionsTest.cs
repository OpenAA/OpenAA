namespace OpenAA.Tests
{
    using NUnit.Framework;
    using System;
    using System.Net;
    using System.Net.Http;
    using OpenAA.Extensions.String;

    [TestFixture()]
    public class StringExtensionsTest
    {
        [Test()]
        public void IsAsciiAlphabetAndNumeric()
        {
            Assert.IsTrue("abcdef".IsAsciiAlphabetAndNumeric());
            Assert.IsTrue("ABCDEF".IsAsciiAlphabetAndNumeric());
            Assert.IsTrue("abc1ef".IsAsciiAlphabetAndNumeric());
            Assert.IsFalse("abc-ef".IsAsciiAlphabetAndNumeric());
            Assert.IsFalse("abc\nef".IsAsciiAlphabetAndNumeric());
            Assert.IsFalse("abc ef".IsAsciiAlphabetAndNumeric());
            Assert.IsFalse("　".IsAsciiAlphabetAndNumeric());
            Assert.IsFalse("あ".IsAsciiAlphabetAndNumeric());
            Assert.IsFalse("ア".IsAsciiAlphabetAndNumeric());
            Assert.IsFalse("漢".IsAsciiAlphabetAndNumeric());
            Assert.IsFalse("ａ".IsAsciiAlphabetAndNumeric());
            Assert.IsFalse("Ａ".IsAsciiAlphabetAndNumeric());
        }
    }
}

