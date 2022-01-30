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
       
        /// <summary>
        /// Parses the bytes in an array to characters
        /// Aborts when all input is parsed or a parse-error happens.
        /// </summary>
        /// <param name="input">in byte array</param>
        /// <param name="in_offset">offset</param>
        /// <param name="in_count"></param>
        /// <param name="out_data">array to fill with parsed characters</param>
        /// <param name="out_offset"></param>
        /// <param name="char_positions">array to fill with byte-position of the characters. Should have same length as out_data</param>
        /// <param name="char_pos_offset"></param>
        /// <param name="out_length"></param>
        /// <returns>number of successfullly parsed characters</returns>
        public static int Parse(byte[] input, int in_offset, int in_count, char[] out_data, int out_offset, int[] char_positions, int char_pos_offset, int out_length)
        {
            const byte EOS_filler = (byte)0xff;
            if (in_count <= 0)
                return 0;
             
            // these bytes is our sliding window into input.
            // That will cause us to use an offset of 3 in lots of places.
            byte b0=0, b1=0, b2=0, b3=0;

            b0 = input[0];
            if (in_count > 1)
                b1 = input[1];
            if (in_count > 2)
                b2 = input[2];
            if (in_count > 3)
                b3 = input[3];

            int chr_int;
            int byte_cnt;

            int in_pos;
            int char_pos; // position of char in input-array
            int out_pos; // where to write output
            int i, max, in_max; // i goes 0--max, but input is -3
            for ( i = 0, in_pos = in_offset+3, out_pos = out_offset, char_pos = in_offset, in_max = in_offset + in_count, max = in_max + 3; ; ++i, ++out_pos)
            {
                if (in_pos >= max)
                    return i; // we have consumed all bytes, now only EOF-filler in byte-window

                (chr_int, byte_cnt) = Parse(b0, b1, b2, b3);
                if (chr_int < 0)
                    return i;

                char chr = (char)chr_int;
                out_data[out_pos] = chr;
                char_positions[out_pos] = char_pos;

                char_pos += byte_cnt;
                ++in_pos;

                // rolled out for-loop
                switch (byte_cnt)
                {
                    case 1:
                        b0 = b1;
                        b1 = b2;
                        b2 = b3;
                        b3 = in_pos < in_max ? input[in_pos] : EOS_filler;
                        break;
                    case 2:
                        b0 = b2;
                        b1 = b3;
                        if (in_pos + 1 < in_max)
                        {
                            b2 = input[in_pos];
                            b3 = input[++in_pos];
                        }
                        else
                        {
                            b2 = EOS_filler;
                            b3 = EOS_filler;
                        }
                        break;
                    case 3:
                        b0 = b3;
                        if (in_pos + 2 < in_max)
                        {
                            b1 = input[in_pos];
                            b2 = input[++in_pos];
                            b3 = input[++in_pos];
                        }
                        else
                        {
                            b1 = EOS_filler;
                            b2 = EOS_filler;
                            b3 = EOS_filler;
                        }
                        break;
                    case 4:
                        if (in_pos + 3 < in_max)
                        {
                            b0 = input[in_pos];
                            b1 = input[++in_pos];
                            b2 = input[++in_pos];
                            b3 = input[++in_pos];
                        }
                        else
                        {
                            b0 = EOS_filler;
                            b1 = EOS_filler;
                            b2 = EOS_filler;
                            b3 = EOS_filler;
                        }
                        break;
                }
            }

        }

        public static (int, int) Parse(byte b0, byte b1, byte b2, byte b3)
        {
            switch (b0)
            {
                // UTF8-1      = 0x00-7F
                #region 0x00--7F
                case 0x00:
                case 0x01:
                case 0x02:
                case 0x03:
                case 0x04:
                case 0x05:
                case 0x06:
                case 0x07:
                case 0x08:
                case 0x09:
                case 0x0A:
                case 0x0B:
                case 0x0C:
                case 0x0D:
                case 0x0E:
                case 0x0F:
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                case 0x14:
                case 0x15:
                case 0x16:
                case 0x17:
                case 0x18:
                case 0x19:
                case 0x1A:
                case 0x1B:
                case 0x1C:
                case 0x1D:
                case 0x1E:
                case 0x1F:
                case 0x20:
                case 0x21:
                case 0x22:
                case 0x23:
                case 0x24:
                case 0x25:
                case 0x26:
                case 0x27:
                case 0x28:
                case 0x29:
                case 0x2A:
                case 0x2B:
                case 0x2C:
                case 0x2D:
                case 0x2E:
                case 0x2F:
                case 0x30:
                case 0x31:
                case 0x32:
                case 0x33:
                case 0x34:
                case 0x35:
                case 0x36:
                case 0x37:
                case 0x38:
                case 0x39:
                case 0x3A:
                case 0x3B:
                case 0x3C:
                case 0x3D:
                case 0x3E:
                case 0x3F:
                case 0x40:
                case 0x41:
                case 0x42:
                case 0x43:
                case 0x44:
                case 0x45:
                case 0x46:
                case 0x47:
                case 0x48:
                case 0x49:
                case 0x4A:
                case 0x4B:
                case 0x4C:
                case 0x4D:
                case 0x4E:
                case 0x4F:
                case 0x50:
                case 0x51:
                case 0x52:
                case 0x53:
                case 0x54:
                case 0x55:
                case 0x56:
                case 0x57:
                case 0x58:
                case 0x59:
                case 0x5A:
                case 0x5B:
                case 0x5C:
                case 0x5D:
                case 0x5E:
                case 0x5F:
                case 0x60:
                case 0x61:
                case 0x62:
                case 0x63:
                case 0x64:
                case 0x65:
                case 0x66:
                case 0x67:
                case 0x68:
                case 0x69:
                case 0x6A:
                case 0x6B:
                case 0x6C:
                case 0x6D:
                case 0x6E:
                case 0x6F:
                case 0x70:
                case 0x71:
                case 0x72:
                case 0x73:
                case 0x74:
                case 0x75:
                case 0x76:
                case 0x77:
                case 0x78:
                case 0x79:
                case 0x7A:
                case 0x7B:
                case 0x7C:
                case 0x7D:
                case 0x7E:
                case 0x7F:
                    #endregion
                    return ((char)b0, 1);


                // UTF8-2      = 0xC2-DF UTF8-tail
                #region 0xC2--DF
                case 0xC2:
                case 0xC3:
                case 0xC4:
                case 0xC5:
                case 0xC6:
                case 0xC7:
                case 0xC8:
                case 0xC9:
                case 0xCA:
                case 0xCB:
                case 0xCC:
                case 0xCD:
                case 0xCE:
                case 0xCF:
                case 0xD0:
                case 0xD1:
                case 0xD2:
                case 0xD3:
                case 0xD4:
                case 0xD5:
                case 0xD6:
                case 0xD7:
                case 0xD8:
                case 0xD9:
                case 0xDA:
                case 0xDB:
                case 0xDC:
                case 0xDD:
                case 0xDE:
                case 0xDF:
                    #endregion
                    return (Parse2Bytes(b0, b1), 2);

                //UTF8-3  = 
                case 0xE0:
                    // 0xE0 0xA0-BF UTF8-tail 
                    return (0xA0 <= b1 && b1 <= 0xBF) ? (Parse3Bytes(b0, b1, b2), 3) : (-1, 1);
                
                // 0xE1-EC 2(UTF8-tail )
                case 0xE1:
                case 0xE2:
                case 0xE3:
                case 0xE4:
                case 0xE5:
                case 0xE6:
                case 0xE7:
                case 0xE8:
                case 0xE9:
                case 0xEA:
                case 0xEB:
                case 0xEC:
                    return (Parse3Bytes(b0, b1, b2), 3);
                case 0xED:
                    // 0xED 0x80-9F UTF8-tail
                    return (0x80 <= b1 && b1 <= 0x9F) ? (Parse3Bytes(b0, b1, b2), 3) : (-1, 1);
                //  0xEE-EF 2(UTF8-tail )
                case 0xEE:
                case 0xEF:
                    return (Parse3Bytes(b0, b1, b2), 3);

                //UTF8-4 

                // 0xF0 0x90-BF 2(UTF8-tail )
                case 0xF0:
                    return  (0x90 <= b1 && b1 <= 0xBF) ? (Parse4Bytes(b0, b1, b2, b3), 4) : (-1, 1);
                // 0xF1 0xF3 3(UTF8 -tail )
                case 0xF1:
                case 0xF2:
                case 0xF3:
                    return (Parse4Bytes(b0, b1, b2, b3), 4);
                // 0xF4 0x80 0x8F 2(UTF8 -tail ) )
                case 0xF4:
                    return  (0x80 <= b1 && b1 <= 0x8F) ? (Parse4Bytes(b0, b1, b2, b3), 4) : (-1, 1);
                default:
                    return (-1, 1); // invalid
            }


        }

#if DEBUG
        /// <summary>
        /// Unreachable code, saved for reference
        /// </summary>
        /// <param name="b0"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="b3"></param>
        /// <returns></returns>
        private static (int, int) Parse_method_using_ifs(byte b0, byte b1, byte b2, byte b3)
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

            return (-1, 1); // invalid, filler.
        }
#endif

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
