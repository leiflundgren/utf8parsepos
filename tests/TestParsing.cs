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

                (char parsed_c, int used_bytes) = new Parser().Parse(buf[0], buf[1], buf[2], buf[3]);

                Assert.AreNotEqual(-1, parsed_c, $"Failed to parse n={i}");
                Assert.AreEqual(c, parsed_c, $"Got wrong char back n={i}");
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

            int count = new Parser().Parse(bytes, 0, bytes.Length, characters, 0, positions, 0, characters.Length);
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

            int count = new Parser().Parse(bytes, 0, bytes.Length, characters, 0, positions, 0, characters.Length);
            string actual = new string(characters, 0, count);

            Assert.AreEqual(expected, actual);
            AssertSequenceEqual(new int[] { 0, 1, 3, 4, 5, 6, 8, 9, 10, 12 }, positions, count);
        }

        [TestMethod]
        public void TestSearchIdea()
        {
            bool SequenceEquals(char[] x, int x_offset, char[]y, int y_offset, int cnt)
            {
                for ( int i=0, x_pos = x_offset, y_pos = y_offset; i<cnt; ++i, ++x_pos, ++y_pos )
                    if ( x[x_pos] != y[y_pos])
                            return false;
                return true;
            }

            int SearchArray(char[] searchFor, char[] searchIn, int in_offset, int in_count)
            {
                for ( int i = 0, in_pos = in_offset, max = in_count-searchFor.Length; i < max; ++i, ++in_pos)
                {
                    if (!SequenceEquals(searchFor, 0, searchIn, in_pos, searchFor.Length))
                        continue;
                    return in_pos;
                }
                return -1;
            }

            string text_about_raksmorgas =
@"På en räksmörgås/Räksmörgås Från en föregångare i Newport, Rhode Island togs konceptet till Gothia Towers i Göteborg. Precis som på den amerikanska östkusten var det storleken som räknades och namnet blev därför självklart – King size.
1984 är inte bara namnet på George Orwells klassiska framtidsroman. Det är också året när Gothia Towers räksmörgås för första gången såg världens ljus.
Receptet togs fram redan 1977 av Staffan Enander. Under 1970-talet hade han under en rad USA-besök upptäckt en amerikansk storslagenhet som han gillade. Hamburgarna på tallriken var stora och skyskraporna i New York ännu större. Målet blev att ta med sig den storheten till Sverige i form av en maträtt som dessutom skulle vara folklig, ha anknytning till västkusten och till rätt pris. Räkcocktailen hade tagit folkhemmet i anspråk och nu ville Staffan Enander trumfa med en huvudrätt. Resultatet blev en macka med 200 gram handskalade räkor, majonnäs, ägg och en lättare sallad.
Det stora genombrottet kom 2002, året efter Gothia Towers andra torn stod färdigbyggt och Heaven 23 och Incontro invigdes. Sedan dess har Räksmörgåsen för många blivit synonym med Göteborgsbesöket och omskriven i allt från Dagens Industri till Amelia.
I maj 2009 passerades drömgränsen på en miljon sålda räksmörgåsar, 12 januari 2016 såldes den 2 miljonte räksmörgåsen och idag säljs över 14 mackor i timman, året runt. Varje år serveras 35 ton ishavsfiskade räkor på mackor av rågbröd smaksatt med dill.
raksmorgas";

            byte[] bytes = Encoding.UTF8.GetBytes(text_about_raksmorgas);
            char[] characters = new char[Encoding.UTF8.GetMaxByteCount(bytes.Length)];
            int[] positions = new int[Encoding.UTF8.GetMaxByteCount(bytes.Length)];

            int count = new Parser().Parse(bytes, 0, bytes.Length, characters, 0, positions, 0, characters.Length);

            int pos_newport = SearchArray("Newport".ToCharArray(), characters, 0, count);
            int byte_pos_newport = positions[pos_newport];

            Assert.AreEqual(50, pos_newport);
            Assert.AreEqual(60, byte_pos_newport);
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
