using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utf8parsepos
{
    /// <summary>
    /// Links
    /// https://ianthehenry.com/posts/decoding-utf-8/
    /// 
    /// https://datatracker.ietf.org/doc/html/rfc3629#section-4 
/*
   Syntax of UTF-8 Byte Sequences

    For the convenience of implementors using ABNF, a definition of UTF-8
   in ABNF syntax is given here.

   A UTF-8 string is a sequence of octets representing a sequence of UCS
   characters.  An octet sequence is valid UTF-8 only if it matches the
   following syntax, which is derived from the rules for encoding UTF-8
   and is expressed in the ABNF of[RFC2234].

   UTF8-octets = *(UTF8 - char)
   UTF8-char   = UTF8 - 1 / UTF8 - 2 / UTF8 - 3 / UTF8 - 4
   UTF8-1      = 0x00-7F
   UTF8-2      = 0xC2-DF UTF8-tail
   UTF8-3      = 0xE0 0xA0-BF UTF8-tail / 0xE1-EC 2(UTF8-tail ) /
                 0xED 0x80-9F UTF8-tail / 0xEE-EF 2(UTF8-tail )
   UTF8-4      = 0xF0 0x90-BF 2(UTF8-tail ) / 0xF1-F3 3(UTF8-tail ) /
                 0xF4 0x80-8F 2(UTF8-tail )
   UTF8-tail   = 0x80-BF
*/
    /// </summary>


    public static class UTF8_Parser
    {
       

        public static (char, int) Parse(byte b0, byte b1, byte b2, byte b3)
        {
            // UTF8-1      = 0x00-7F
            if (b0 <= 0x7f)
                return ((char)b0, 1);
            // UTF8-2      = 0xC2-DF UTF8-tail
            if (0xC2 <= b0 && b0 <= 0xDF)
                return (Parse2Bytes(b0, b1), 2);
            //UTF8-3  = 
            if (b0 == 0xE0 && 0xA0 <= b1 && b1 <= 0xBF     // 0xE0 0xA0-BF UTF8-tail 
                || 0xE1 <= b0 && b0 <= 0xEC                // 0xE1-EC 2(UTF8-tail )
                || b0 == 0xED && 0x80 <= b1 && b1 <= 0x9F   // 0xED 0x80-9F UTF8-tail
                || b0 == 0xEE || b0 == 0xEF                 //  0xEE-EF 2(UTF8-tail )
                )
                return (Parse3Bytes(b0, b1, b2), 3);
            //UTF8-4  = 0xF0 0x90-BF 2(UTF8-tail ) / 0xF1-F3 3(UTF8-tail ) / 0xF4 0x80-8F 2(UTF8-tail ) 
            if (b0 == 0xF0 && 0x90 <= b1 && b1 <= 0xBF  // 0xF0 0x90-BF 2(UTF8-tail )
            || 0xF1 <= b1 && b1 <= 0xF3 // 0xF1 0xF3 3(UTF8 -tail )
            || b0 == 0xF4 && 0x80 <= b1 && b1 <= 0x8F) // 0xF4 0x80 0x8F 2(UTF8 -tail ) )
                return (Parse4Bytes(b0, b1, b2, b3), 4);

            return ('\0', 1); // invalid, filler.
        }


        private const byte bitfilter_2bytes_0 = 0x1f;
        private const byte bitfilter_2bytes_1 = 0x3f;
        private const int bitshift_2bytes_0 = 3;

        private const byte bitfilter_3bytes_0 = 0x0f;
        private const byte bitfilter_3bytes_1 = 0x3f;
        private const byte bitfilter_3bytes_2 = 0x3f;
        private const int bitshift_3bytes_0 = 4;
        private const int bitshift_3bytes_1 = 2;

        private const byte bitfilter_4bytes_0 = 0x7;
        private const byte bitfilter_4bytes_1 = 0x3f;
        private const byte bitfilter_4bytes_2 = 0x3f;
        private const byte bitfilter_4bytes_3 = 0x3f;
        private const int bitshift_4bytes_0 = 5;
        private const int bitshift_4bytes_1 = 2;
        private const int bitshift_4bytes_2 = 2;


        /// <summary>
        /// pattern  	110xxxxx 10xxxxxx
        /// </summary>
        private static char Parse2Bytes(uint b0, uint b1)
        {
            uint bb0 = b0 & bitfilter_2bytes_0;
            uint bb1 = b1 & bitfilter_2bytes_1;

            uint bs0 = bb0 << 6;
            uint bs1 = bb1;

            uint n = bs0 | bs1;
            return (char)n;
        }

        private static char Parse3Bytes(uint b0, uint b1, uint b2)
        {
            uint bb0 = b0 & bitfilter_3bytes_0;
            uint bb1 = b1 & bitfilter_3bytes_1;
            uint bb2 = b2 & bitfilter_3bytes_2;

            uint bs0 = bb0 << 6 + 6;
            uint bs1 = bb1 << 6;
            uint bs2 = bb2;

            uint n = bs0 | bs1 | bs2;
            return (char)n;
        }

        private static char Parse4Bytes(uint b0, uint b1, uint b2, uint b3)
        {
            uint bb0 = b0 & bitfilter_4bytes_0;
            uint bb1 = b1 & bitfilter_4bytes_1;
            uint bb2 = b2 & bitfilter_4bytes_2;
            uint bb3 = b2 & bitfilter_4bytes_3;

            uint bs3 = bb3;
            uint bs2 = bb2 << 6;
            uint bs1 = bb1 << 6 + 6;
            uint bs0 = bb0 << 6 + 6 + 3;


            uint n = bs0 | bs1 | bs2 | bs3;
            return n < 65535 ? (char)n : Encoding.UTF32.GetString(BitConverter.GetBytes(n))[0];
        }

        //public static int Parse(byte[] in_data, int in_offset, int in_count, byte[] out_data, int out_offset)
        //{
        //    byte b0 = in_count > 0 ? in_data[in_offset] : (byte)0;
        //    byte b1 = in_count > 1 ? in_data[in_offset+1] : 0;
        //    byte b2 = in_count > 2 ? in_data[in_offset+2] : 0;
        //    byte b3 = in_count > 3 ? in_data[in_offset+3] : 0;


        //    for(int in_pos = in_offset, out_pos = out_offset; in_pos < in_count-6; in_pos++)
        //    {

        //    }

        //}


    }
}
