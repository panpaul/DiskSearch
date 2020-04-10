using System.Collections.Generic;

namespace TinyPinyin
{
    public interface IPinyinDict
    {
        string[] ToPinyin(string word);

        List<string> Words();
    }
}