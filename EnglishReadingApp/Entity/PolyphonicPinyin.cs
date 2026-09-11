using System;
using System.Collections.Generic;
using System.Linq;

namespace EnglishReadingApp.Entity
{
    /// <summary>
    /// 多音字拼音覆盖表。
    /// 用于修正 ChinesePinyinConverter 在特定语境下的误读（如"瀑布"的"瀑"被转为 bào，正确应为 pù）。
    /// </summary>
    /// <remarks>
    /// 匹配顺序：先按词组（长词优先），未命中的字再按单字兜底。
    /// 值为数字声调拼音（如 "pu4"），由 <see cref="PoetryItem"/> 统一转换为声调符号（pù）。
    /// 新增词条时优先加词组，避免单字全局覆盖造成"湖泊"被读成 hú bó 这类误伤。
    /// </remarks>
    public static class PolyphonicPinyin
    {
        /// <summary>
        /// 词组 → 拼音（空格分隔）。静态初始化时已按词长降序，保证长词优先匹配。
        /// </summary>
        private static readonly (string Phrase, string Pinyin)[] PhraseOverrides = new (string Phrase, string Pinyin)[]
        {
            ("瀑布", "pu4 bu4"),     // 库误作 bào；bào 仅用于水名"瀑河"
            ("不觉", "bu4 jue2"),    // "不觉晓"：觉读 jué 非 jiào；"不"在阳平前读本调 bù
            ("曲项", "qu1 xiang4"),  // "曲项向天歌"：曲读 qū（弯曲）非 qǔ（歌曲）
            ("相思", "xiang1 si1"),  // "起相思"：相读 xiāng 非 xiàng
        }
        .OrderByDescending(p => p.Phrase.Length)
        .ToArray();

        /// <summary>
        /// 单字 → 拼音。仅对未被词组覆盖的位置生效，作为兜底。
        /// </summary>
        private static readonly Dictionary<string, string> CharOverrides = new()
        {
            { "瀑", "pu4" },
        };

        /// <summary>
        /// 就地对拼音音节列表应用多音字覆盖。
        /// </summary>
        /// <param name="chinese">汉字串，与 syllables 按位置一一对应</param>
        /// <param name="syllables">数字声调拼音列表（如 ["wang4","lu2"]），会被原地修改</param>
        public static void Apply(string chinese, IList<string> syllables)
        {
            if (string.IsNullOrEmpty(chinese) || syllables == null || syllables.Count == 0)
                return;

            var covered = new bool[syllables.Count];

            // 1) 词组优先：长词先匹配，避免短词抢占（如"瀑布"优先于单字"瀑"）
            foreach (var (phrase, pinyin) in PhraseOverrides)
            {
                var parts = pinyin.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int from = 0;

                while (true)
                {
                    int idx = chinese.IndexOf(phrase, from, StringComparison.Ordinal);
                    if (idx < 0 || idx >= syllables.Count) break;

                    for (int i = 0; i < parts.Length && idx + i < syllables.Count; i++)
                    {
                        syllables[idx + i] = parts[i];
                        covered[idx + i] = true;
                    }

                    from = idx + phrase.Length;
                }
            }

            // 2) 单字兜底：只处理未被词组覆盖的位置
            var chars = chinese.ToCharArray();
            for (int i = 0; i < syllables.Count && i < chars.Length; i++)
            {
                if (covered[i]) continue;

                if (CharOverrides.TryGetValue(chars[i].ToString(), out var replacement))
                    syllables[i] = replacement;
            }
        }
    }
}
