namespace OpenAA.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 文字コード判別クラス。
    /// 精度はイマイチ。
    /// もう少しマシなのに差し替えたい。
    /// </summary>
    public class EncodingDetector
    {
        private bool _bBOM;
        private bool _bLE;
        private bool _bBE;
        private int _sjis;
        private int _euc;
        private int _utf8;

        public bool HasBOM
        {
            get { return _bBOM; }
        }

        public Encoding GetCode(byte[] bytes)
        {
            if (IsUTF32(bytes) == true)
            {
                if (_bLE == true)
                {// UTF32LE
                    return Encoding.GetEncoding(12000);
                }
                else if (_bBE == true)
                {// UTF32BE
                    return Encoding.GetEncoding(12001);
                }
            }

            else if (IsUTF16(bytes) == true)
            {
                if (_bLE == true)
                {// UTF16LE
                    return Encoding.Unicode;
                }
                else if (_bBE == true)
                {// UTF16BE
                    return Encoding.BigEndianUnicode;
                }
            }

            else if (IsJis(bytes) == true)
            {
                return Encoding.GetEncoding(50220);
            }

            else if (IsAscii(bytes) == true)
            {
                return Encoding.ASCII;
            }

            else
            {
                bool bUTF8 = IsUTF8(bytes);
                bool bShitJis = IsShiftJis(bytes);
                bool bEUC = IsEUC(bytes);

                if (bUTF8 == true || bShitJis == true || bEUC == true)
                {
                    if (_euc > _sjis && _euc > _utf8)
                    {
                        return Encoding.GetEncoding(51932);
                    }
                    else if (_sjis > _euc && _sjis > _utf8)
                    {
                        return Encoding.GetEncoding(932);
                    }
                    else if (_utf8 > _euc && _utf8 > _sjis)
                    {
                        return Encoding.UTF8;
                    }
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        private bool IsUTF32(byte[] bytes)      // Check for UTF-32
        {
            int len = bytes.Length;
            byte b1, b2, b3, b4;
            _bLE = false; _bBE = false;

            if (len >= 4)
            {
                b1 = bytes[0]; b2 = bytes[1]; b3 = bytes[2]; b4 = bytes[3];

                if (b1 == 0xFF && b2 == 0xFE && b3 == 0x00 && b4 == 0x00)
                {
                    _bLE = true;
                    return true;
                }
                else if (b1 == 0x00 && b2 == 0x00 && b3 == 0xFE && b4 == 0xFF)
                {
                    _bBE = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        private bool IsUTF16(byte[] bytes)      // Check for UTF-16
        {
            int len = bytes.Length;
            byte b1, b2;
            _bLE = false; _bBE = false;

            if (len >= 2)
            {
                b1 = bytes[0];
                b2 = bytes[1];

                if (b1 == 0xFF && b2 == 0xFE)
                {
                    _bLE = true;
                    return true;
                }
                else if (b1 == 0xFE && b2 == 0xFF)
                {
                    _bBE = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool IsJis(byte[] bytes)        // Check for JIS (ISO-2022-JP)
        {
            int len = bytes.Length;
            byte b1, b2, b3, b4, b5, b6;

            for (int i = 0; i < len; i++)
            {
                b1 = bytes[i];

                if (b1 > 0x7F)
                {
                    return false;   // Not ISO-2022-JP (0x00～0x7F)
                }
                else if (i < len - 2)
                {
                    b2 = bytes[i + 1]; b3 = bytes[i + 2];

                    if (b1 == 0x1B && b2 == 0x28 && b3 == 0x42)
                    {
                        return true;    // ESC ( B  : JIS ASCII
                    }

                    else if (b1 == 0x1B && b2 == 0x28 && b3 == 0x4A)
                    {
                        return true;    // ESC ( J  : JIS X 0201-1976 Roman Set
                    }

                    else if (b1 == 0x1B && b2 == 0x28 && b3 == 0x49)
                    {
                        return true;    // ESC ( I  : JIS X 0201-1976 kana
                    }

                    else if (b1 == 0x1B && b2 == 0x24 && b3 == 0x40)
                    {
                        return true;    // ESC $ @  : JIS X 0208-1978(old_JIS)
                    }

                    else if (b1 == 0x1B && b2 == 0x24 && b3 == 0x42)
                    {
                        return true;    // ESC $ B  : JIS X 0208-1983(new_JIS)
                    }
                }
                else if (i < len - 3)
                {
                    b2 = bytes[i + 1]; b3 = bytes[i + 2]; b4 = bytes[i + 3];

                    if (b1 == 0x1B && b2 == 0x24 && b3 == 0x28 && b4 == 0x44)
                    {
                        return true;    // ESC $ ( D  : JIS X 0212-1990（JIS_hojo_kanji）
                    }
                }
                else if (i < len - 5)
                {
                    b2 = bytes[i + 1]; b3 = bytes[i + 2];
                    b4 = bytes[i + 3]; b5 = bytes[i + 4]; b6 = bytes[i + 5];

                    if (b1 == 0x1B && b2 == 0x26 && b3 == 0x40 &&
                        b4 == 0x1B && b5 == 0x24 && b6 == 0x42)
                    {
                        return true;    // ESC & @ ESC $ B  : JIS X 0208-1990
                    }
                }
            }

            return false;
        }

        private bool IsAscii(byte[] bytes)      // Check for Ascii
        {
            int len = bytes.Length;

            for (int i = 0; i < len; i++)
            {
                if (bytes[i] <= 0x7F)
                {
                    // ASCII : 0x00～0x7F
                    ;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsShiftJis(byte[] bytes)   // Check for Shift-JIS
        {
            int len = bytes.Length;
            byte b1, b2;
            bool result = true;

            _sjis = 0;

            for (int i = 0; i < len; i++)
            {
                b1 = bytes[i];

                if ((b1 <= 0x7F) || (0xA1 <= b1 && b1 <= 0xDF))
                {
                    // ASCII : 0x00～0x7F
                    // kana  : 0xA1～0xDF
                    ;
                }
                else if (i < len - 1)
                {
                    b2 = bytes[i + 1];

                    if (
                        ((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) &&
                        ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC))
                        )
                    {
                        // kanji first byte  : 0x81～0x9F、0xE0～0xFC
                        //       second byte : 0x40～0x7E、0x80～0xFC
                        i++;
                        _sjis += 2;
                    }
                    else
                    {
                        // SJISは外字の可能性があるのでとりあえず判別を続ける
                        result = false;
                    }
                }
            }

            return result;
        }

        private bool IsEUC(byte[] bytes)        // Check for _euc-jp 
        {
            int len = bytes.Length;
            byte b1, b2, b3;
            bool result = true;

            _euc = 0;

            for (int i = 0; i < len; i++)
            {
                b1 = bytes[i];

                if (b1 <= 0x7F)
                {   //  ASCII : 0x00～0x7F
                    ;
                }
                else if (i < len - 1)
                {
                    b2 = bytes[i + 1];

                    if ((b1 >= 0xA1 && b1 <= 0xFE) && (b2 >= 0xA1 && b1 <= 0xFE))
                    { // kanji - first & second byte : 0xA1～0xFE
                        i++;
                        _euc += 2;
                    }
                    else if ((b1 == 0x8E) && (b2 >= 0xA1 && b1 <= 0xDF))
                    { // kana - first byte : 0x8E, second byte : 0xA1～0xDF
                        i++;
                        _euc += 2;
                    }
                    else if (i < len - 2)
                    {
                        b3 = bytes[i + 2];

                        if ((b1 == 0x8F) &&
                            (b2 >= 0xA1 && b2 <= 0xFE) && (b3 >= 0xA1 && b3 <= 0xFE))
                        { // hojo kanji - first byte : 0x8F, second & third byte : 0xA1～0xFE
                            i += 2;
                            _euc += 3;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        private bool IsUTF8(byte[] bytes)       // Check for UTF-8
        {
            int len = bytes.Length;
            byte b1, b2, b3, b4;
            _utf8 = 0;
            _bBOM = false;

            int i = 0;

            if (3 <= len)
            {
                b1 = bytes[0];
                b2 = bytes[0];
                b3 = bytes[0];
                if (b1 == 0xEF && b2 == 0xBB && b3 == 0xBF)
                { // BOM : 0xEF 0xBB 0xBF
                    _bBOM = true;
                    i += 3;
                    _utf8 += 3;
                }
            }

            for (; i < len; i++)
            {
                b1 = bytes[i];

                if (b1 <= 0x7F)
                { //  ASCII : 0x00～0x7F
                    ;
                }
                else if (i < len - 1)
                {
                    b2 = bytes[i + 1];

                    if ((0xC0 <= b1 && b1 <= 0xDF) &&
                        (0x80 <= b2 && b2 <= 0xBF))
                    { // 2 byte char
                        i += 1;
                        _utf8 += 2;
                    }
                    else if (i < len - 2)
                    {
                        b3 = bytes[i + 2];

                        if ((0xE0 <= b1 && b1 <= 0xEF) &&
                            (0x80 <= b2 && b2 <= 0xBF) &&
                            (0x80 <= b3 && b2 <= 0xBF))
                        { // 3 byte char
                            i += 2;
                            _utf8 += 3;
                        }

                        else if (i < len - 3)
                        {
                            b4 = bytes[i + 3];

                            if ((0xF0 <= b1 && b1 <= 0xF7) &&
                                (0x80 <= b2 && b2 <= 0xBF) &&
                                (0x80 <= b3 && b2 <= 0xBF) &&
                                (0x80 <= b4 && b2 <= 0xBF))
                            { // 4 byte char
                                i += 3;
                                _utf8 += 4;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
