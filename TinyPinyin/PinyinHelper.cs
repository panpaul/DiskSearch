using System.Collections.Generic;
using System.Linq;
using TinyPinyin.Data;

namespace TinyPinyin
{
    public static class PinyinHelper
    {
        private static bool IsChinese(char c)
        {
            return PinyinData.MIN_VALUE <= c && c <= PinyinData.MAX_VALUE && GetPinyinCode(c) > 0 ||
                   PinyinData.CHAR_12295 == c;
        }

        public static string GetPinyin(char c)
        {
            if (!IsChinese(c)) return " ";
            return c == PinyinData.CHAR_12295 ? PinyinData.PINYIN_12295 : PinyinData.PINYIN_TABLE[GetPinyinCode(c)];
        }

        public static string GetPinyin(string str, string separator = " ")
        {
            return Engine.ToPinyin(str, null, null, separator);
        }


        public static string GetPinyinInitials(string str)
        {
            var result = GetPinyin(str, "|");
            return string.Join("", result.Split('|').Select(x => x.Substring(0, 1)).ToArray());
        }

        private static int GetPinyinCode(char c)
        {
            var offset = c - PinyinData.MIN_VALUE;
            if (0 <= offset && offset < PinyinData.PINYIN_CODE_1_OFFSET)
                return DecodeIndex(PinyinCode1.PINYIN_CODE_PADDING, PinyinCode1.PINYIN_CODE, offset);
            if (PinyinData.PINYIN_CODE_1_OFFSET <= offset
                && offset < PinyinData.PINYIN_CODE_2_OFFSET)
                return DecodeIndex(PinyinCode2.PINYIN_CODE_PADDING, PinyinCode2.PINYIN_CODE,
                    offset - PinyinData.PINYIN_CODE_1_OFFSET);
            return DecodeIndex(PinyinCode3.PINYIN_CODE_PADDING, PinyinCode3.PINYIN_CODE,
                offset - PinyinData.PINYIN_CODE_2_OFFSET);
        }

        private static short DecodeIndex(IReadOnlyList<byte> paddings, IReadOnlyList<byte> indexes, int offset)
        {
            //CHECKSTYLE:OFF
            var index1 = offset / 8;
            var index2 = offset % 8;
            var realIndex = (short) (indexes[offset] & 0xff);
            //CHECKSTYLE:ON
            if ((paddings[index1] & PinyinData.BIT_MASKS[index2]) != 0)
                realIndex = (short) (realIndex | PinyinData.PADDING_MASK);
            return realIndex;
        }
    }
}