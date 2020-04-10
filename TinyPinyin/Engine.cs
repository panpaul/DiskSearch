using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyPinyin
{
    public static class Engine
    {
        public static string[] PinyinFromDict(string wordInDict, List<IPinyinDict> pinyinDictSet)
        {
            if (pinyinDictSet == null) throw new ArgumentException("No pinyin dict contains word: " + wordInDict);
            foreach (var dict in pinyinDictSet.Where(dict => dict?.Words() != null && dict.Words().Contains(wordInDict))
            )
                return dict.ToPinyin(wordInDict);
            throw new ArgumentException("No pinyin dict contains word: " + wordInDict);
        }

        public static string ToPinyin(string inputStr, string trie, List<IPinyinDict> pinyinDictList, string separator)
        {
            if (string.IsNullOrEmpty(inputStr)) return inputStr;

            if (trie != null) return null;
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < inputStr.Length; i++)
            {
                var str = PinyinHelper.GetPinyin(inputStr[i]);
                //if (str == "") continue;
                stringBuilder.Append(str);
                if (i != inputStr.Length - 1) stringBuilder.Append(separator);
            }

            return stringBuilder.ToString();
        }
    }
}