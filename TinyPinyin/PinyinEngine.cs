using System.Text;

namespace TinyPinyin
{
    public static class PinyinEngine
    {
        public static string ToPinyin(string inputStr, string separator)
        {
            if (string.IsNullOrEmpty(inputStr)) return inputStr;

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