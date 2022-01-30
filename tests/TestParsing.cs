using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace utf8parsepos.tests
{
    [TestClass]
    public class TestParsing
    {

        [TestMethod]

        public void TestParseSingleCharacter()
        {
            for (int i = 0; i < 50000; i++)
            {
                char c = (char)i;
                byte[] buf = new byte[4];
                int bytes = Encoding.UTF8.GetBytes(new char[] { c }, 0, 1, buf, 0);

                Assert.AreEqual(c, Encoding.UTF8.GetString(buf, 0, bytes)[0], "System UTF8 decoder fail");

                (int parsed_c, int used_bytes) = UTF8_Parser.Parse(buf[0], buf[1], buf[2], buf[3]);

                Assert.AreNotEqual(-1, parsed_c, $"Failed to parse n={i}");
                Assert.AreEqual(c, (char)parsed_c, $"Got wrong char back n={i}");
                Assert.AreEqual(bytes, used_bytes, $"Unexpected read-length n={i}");
            }
        }

        [TestMethod]
        public void TestParseSimple()
        {
            string expected = "hello world";

            byte[] bytes = Encoding.UTF8.GetBytes(expected);
            char[] characters = new char[Encoding.UTF8.GetMaxByteCount(bytes.Length)];
            int[] positions = new int[Encoding.UTF8.GetMaxByteCount(bytes.Length)];

            int count = UTF8_Parser.Parse(bytes, 0, bytes.Length, characters, 0, positions, 0, characters.Length);
            string actual = new string(characters, 0, count);

            Assert.AreEqual(expected, actual);
            AssertSequenceEqual(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, positions, count);
        }

        [TestMethod]
        public void TestParseMultiBytes()
        {
            string expected = "räksmörgås";

            byte[] bytes = Encoding.UTF8.GetBytes(expected);
            char[] characters = new char[Encoding.UTF8.GetMaxByteCount(bytes.Length)];
            int[] positions = new int[Encoding.UTF8.GetMaxByteCount(bytes.Length)];

            int count = UTF8_Parser.Parse(bytes, 0, bytes.Length, characters, 0, positions, 0, characters.Length);
            string actual = new string(characters, 0, count);

            Assert.AreEqual(expected, actual);
            AssertSequenceEqual(new int[] { 0, 1, 3, 4, 5, 6, 8, 9, 10, 12 }, positions, count);
        }



        private static void AssertSequenceEqual<T>(ICollection<T> expected, ICollection<T> actual, string message = "")
        {
            AssertSequenceEqual<T>(expected, actual, expected.Count, message);
        }
        private static void AssertSequenceEqual<T>(ICollection<T> expected, ICollection<T> actual, int count, string message = "")
        {
            int expectedCount = Math.Min(count, expected.Count);
            int actualCount = Math.Min(count, actual.Count);
            Assert.AreEqual(expectedCount, actualCount, $"Extected length {expectedCount}, actual length {actualCount} " + message);
            IEnumerator<T> expEnum = expected.GetEnumerator(), actEnum = actual.GetEnumerator();
            for(int i=0; i< count; ++i)
            {
                Assert.AreEqual(expEnum.MoveNext(), actEnum.MoveNext(), message);
                Assert.AreEqual(expEnum.Current, actEnum.Current, $"At position {i} " + message);
            }
        }
    }
}
