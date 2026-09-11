using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishReadingApp.Entity
{
    public class PoetryItem
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Dynasty { get; set; } = "";
        public string Content { get; set; } = "";
        public string Translation { get; set; } = "";

        /// <summary>
        /// 诗名拼音（带声调符号，如 "jìng yè sī"）。转换失败时返回原中文。
        /// </summary>
        public string TitlePinyin => GetPinyinWithToneSymbol(Title);

        /// <summary>
        /// 作者拼音（带声调符号，如 "lǐ bái"）。转换失败时返回原中文。
        /// </summary>
        public string AuthorPinyin => GetPinyinWithToneSymbol(Author);

        /// <summary>
        /// 朝代拼音（带声调符号，如 "táng dài"）。转换失败时返回原中文。
        /// </summary>
        public string DynastyPinyin => GetPinyinWithToneSymbol(Dynasty);

        public List<PinyinLine> GetPinyinLines()
        {
            var result = new List<PinyinLine>();
            var lines = Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    result.Add(new PinyinLine
                    {
                        Chinese = trimmedLine,
                        Pinyin = GetPinyinWithToneSymbol(trimmedLine)
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// 使用 ChinesePinyinConverter 获取拼音并转换为声调符号
        /// </summary>
        public string GetPinyinWithToneSymbol(string chinese)
        {
            try
            {
                // 使用 ChinesePinyinConverter 获取带数字声调的拼音
                var pinyinList = ChinesePinyinConverter.PinyinConverter.ConvertToPinyin(chinese, withTone: true);

                if (pinyinList == null || !pinyinList.Any())
                    return chinese;

                // 多音字覆盖：修正拼音库在特定语境下的误读
                // （如"瀑布"的"瀑"库给 bào，正确应为 pù）
                var syllables = pinyinList.ToList();
                PolyphonicPinyin.Apply(chinese, syllables);

                // 将列表转为字符串，如 "chun1 mian2 bu2 jiao4 xiao3"
                string pinyinWithNumber = string.Join(" ", syllables);

                // 将数字声调转换为声调符号
                return ConvertNumberToToneMark(pinyinWithNumber);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"拼音转换失败: {ex.Message}");
                return chinese;
            }
        }

        /// <summary>
        /// 将数字声调转换为声调符号
        /// 例如： "chun1" -> "chūn"
        /// </summary>
        private string ConvertNumberToToneMark(string pinyinWithNumber)
        {
            // 声调符号映射
            var toneMarks = new Dictionary<string, string>
            {
                // a
                {"a1", "ā"}, {"a2", "á"}, {"a3", "ǎ"}, {"a4", "à"},
                // o
                {"o1", "ō"}, {"o2", "ó"}, {"o3", "ǒ"}, {"o4", "ò"},
                // e
                {"e1", "ē"}, {"e2", "é"}, {"e3", "ě"}, {"e4", "è"},
                // i
                {"i1", "ī"}, {"i2", "í"}, {"i3", "ǐ"}, {"i4", "ì"},
                // u
                {"u1", "ū"}, {"u2", "ú"}, {"u3", "ǔ"}, {"u4", "ù"},
                // ü
                {"v1", "ǖ"}, {"v2", "ǘ"}, {"v3", "ǚ"}, {"v4", "ǜ"},
            };

            var words = pinyinWithNumber.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();

            foreach (var word in words)
            {
                if (string.IsNullOrEmpty(word)) continue;

                // 找到数字的位置（声调标记在最后）
                int digitIndex = -1;
                for (int i = word.Length - 1; i >= 0; i--)
                {
                    if (char.IsDigit(word[i]))
                    {
                        digitIndex = i;
                        break;
                    }
                }

                if (digitIndex > 0)
                {
                    string purePinyin = word.Substring(0, digitIndex);
                    string toneNumber = word.Substring(digitIndex);
                    string toneKey = GetToneKey(purePinyin, toneNumber);

                    if (toneMarks.TryGetValue(toneKey, out string? toneMark))
                    {
                        result.Add(ApplyToneMark(purePinyin, toneMark));
                    }
                    else
                    {
                        result.Add(word);
                    }
                }
                else
                {
                    result.Add(word);
                }
            }

            return string.Join(" ", result);
        }

        /// <summary>
        /// 获取声调标记的键值
        /// </summary>
        private string GetToneKey(string pinyin, string toneNumber)
        {
            // 找到应该标声调的元音
            if (pinyin.Contains('a'))
                return "a" + toneNumber;
            if (pinyin.Contains('o'))
                return "o" + toneNumber;
            if (pinyin.Contains('e'))
                return "e" + toneNumber;
            if (pinyin.Contains("iu"))
                return "u" + toneNumber;
            if (pinyin.Contains("ui"))
                return "i" + toneNumber;
            if (pinyin.Contains('i'))
                return "i" + toneNumber;
            if (pinyin.Contains('u'))
                return "u" + toneNumber;
            if (pinyin.Contains('v'))
                return "v" + toneNumber;

            return "a" + toneNumber; // 默认
        }

        /// <summary>
        /// 将声调符号应用到拼音的正确位置
        /// </summary>
        private string ApplyToneMark(string pinyin, string toneMark)
        {
            // 规则：有 a 标在 a 上
            if (pinyin.Contains('a'))
                return pinyin.Replace("a", toneMark);

            // 有 o 标在 o 上
            if (pinyin.Contains('o'))
                return pinyin.Replace("o", toneMark);

            // 有 e 标在 e 上
            if (pinyin.Contains('e'))
                return pinyin.Replace("e", toneMark);

            // iu 标在 u 上
            if (pinyin.Contains("iu"))
                return pinyin.Replace("u", toneMark);

            // ui 标在 i 上
            if (pinyin.Contains("ui"))
                return pinyin.Replace("i", toneMark);

            // 有 i 标在 i 上
            if (pinyin.Contains('i'))
                return pinyin.Replace("i", toneMark);

            // 有 u 标在 u 上
            if (pinyin.Contains('u'))
                return pinyin.Replace("u", toneMark);

            // 有 v (ü) 标在 v 上
            if (pinyin.Contains('v'))
                return pinyin.Replace("v", toneMark);

            return pinyin + toneMark;
        }
    }

    public class PinyinLine
    {
        public string Chinese { get; set; } = "";
        public string Pinyin { get; set; } = "";
    }
}