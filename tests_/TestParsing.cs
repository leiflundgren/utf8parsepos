using System;
using System.Text;
using Xunit;

namespace utf8parsepos.tests
{
    public class TestParsing
    {
        [Fact]
        public void TestParseSingleCharacter()
        {
            for (int i = 0; i < 50000; i++)
            {
                char c = (char)i;
                byte[] buf = new byte[4];
                int bytes = Encoding.UTF8.GetBytes(new char[] { c }, 0, 1, buf, 0);

                Assert.Equal(c, Encoding.UTF8.GetString(buf, 0, bytes)[0], "System UTF8 decoder fail");

                (char parsed_c, int used_bytes) = UTF8_Parser.Parse(buf[0], buf[1], buf[2], buf[3]);

                Assert.Equal(c, parsed_c, $"Got wrong char back n={i}");
                Assert.Equal(bytes, used_bytes, $"Unexpected read-length n={i}");
            }
        }

        private static void AreEqual(object expected, object actual, string errorMessage)
        {
            if (expected == actual) return;

            throw new Xunit.Assert.
        }
    }
}
