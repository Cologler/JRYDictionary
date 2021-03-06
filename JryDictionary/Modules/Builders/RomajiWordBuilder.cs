using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Jasily;
using JryDictionary.Models;

namespace JryDictionary.Modules.Builders
{
    [Export(typeof(IWordBuilder))]
    public sealed class RomajiWordBuilder : IWordBuilder, IOrderable
    {
        private static readonly Dictionary<string, string> Cached;

        static RomajiWordBuilder()
        {
            // http://h84473111.myweb.hinet.net/TACR/jp.htm
            Cached = new Dictionary<string, string>
            {
                // 峠 邪 兆 - 賠 咄
                ["あ"] = "a",
                ["か"] = "ka",
                ["さ"] = "sa",
                ["た"] = "ta",
                ["な"] = "na",
                ["は"] = "ha",
                ["ま"] = "ma",
                ["や"] = "ya",
                ["ら"] = "ra",
                ["わ"] = "wa",
                ["い"] = "i",
                ["き"] = "ki",
                ["し"] = "shi",
                ["ち"] = "chi",
                ["に"] = "ni",
                ["ひ"] = "hi",
                ["み"] = "mi",
                ["り"] = "ri",
                ["う"] = "u",
                ["く"] = "ku",
                ["す"] = "su",
                ["つ"] = "tsu",
                ["ぬ"] = "nu",
                ["ふ"] = "fu",
                ["む"] = "mu",
                ["ゆ"] = "yu",
                ["る"] = "ru",
                ["ん"] = "n",
                ["え"] = "e",
                ["け"] = "ke",
                ["せ"] = "se",
                ["て"] = "te",
                ["ね"] = "ne",
                ["へ"] = "he",
                ["め"] = "me",
                ["れ"] = "re",
                ["お"] = "o",
                ["こ"] = "ko",
                ["そ"] = "so",
                ["と"] = "to",
                ["の"] = "no",
                ["ほ"] = "ho",
                ["も"] = "mo",
                ["よ"] = "yo",
                ["ろ"] = "ro",
                ["を"] = "o",

                // 峠 邪 兆 - �� 咄 �� 磯 �� 咄
                ["が"] = "ga",
                ["ざ"] = "za",
                ["だ"] = "da",
                ["ば"] = "ba",
                ["ぱ"] = "pa",
                ["ぎ"] = "gi",
                ["じ"] = "ji",
                ["ぢ"] = "ji",
                ["び"] = "bi",
                ["ぴ"] = "pi",
                ["ぐ"] = "gu",
                ["ず"] = "zu",
                ["づ"] = "zu",
                ["ぶ"] = "bu",
                ["ぷ"] = "pu",
                ["げ"] = "ge",
                ["ぜ"] = "ze",
                ["で"] = "de",
                ["べ"] = "be",
                ["ぺ"] = "pe",
                ["ご"] = "go",
                ["ぞ"] = "zo",
                ["ど"] = "do",
                ["ぼ"] = "bo",
                ["ぽ"] = "po",

                // 峠 邪 兆 - 渣 咄
                ["きゃ"] = "kya",
                ["ぎゃ"] = "gya",
                ["しゃ"] = "sha",
                ["じゃ"] = "ja",
                ["ちゃ"] = "cha",
                ["にゃ"] = "nya",
                ["ひゃ"] = "hya",
                ["びゃ"] = "bya",
                ["ぴゃ"] = "pya",
                ["みゃ"] = "mya",
                ["りゃ"] = "rya",
                ["きゅ"] = "kyu",
                ["ぎゅ"] = "gyu",
                ["しゅ"] = "shu",
                ["じゅ"] = "ju",
                ["ちゅ"] = "chu",
                ["にゅ"] = "nyu",
                ["ひゅ"] = "hyu",
                ["びゅ"] = "byu",
                ["ぴゅ"] = "pyu",
                ["みゅ"] = "myu",
                ["りゅ"] = "ryu",
                ["きょ"] = "kyo",
                ["ぎょ"] = "gyo",
                ["しょ"] = "sho",
                ["じょ"] = "jo",
                ["ちょ"] = "cho",
                ["にょ"] = "nyo",
                ["ひょ"] = "hyo",
                ["びょ"] = "byo",
                ["ぴょ"] = "pyo",
                ["みょ"] = "myo",
                ["りょ"] = "ryo",

                // 頭 邪 兆 - 賠 咄
                ["ア"] = "a",
                ["カ"] = "ka",
                ["サ"] = "sa",
                ["タ"] = "ta",
                ["ナ"] = "na",
                ["ハ"] = "ha",
                ["マ"] = "ma",
                ["ヤ"] = "ya",
                ["ラ"] = "ra",
                ["ワ"] = "wa",
                ["イ"] = "i",
                ["キ"] = "ki",
                ["シ"] = "shi",
                ["チ"] = "chi",
                ["ニ"] = "ni",
                ["ヒ"] = "hi",
                ["ミ"] = "mi",
                ["リ"] = "ri",
                ["ウ"] = "u",
                ["ク"] = "ku",
                ["ス"] = "su",
                ["ツ"] = "tsu",
                ["ヌ"] = "nu",
                ["フ"] = "fu",
                ["ム"] = "mu",
                ["ユ"] = "yu",
                ["ル"] = "ru",
                ["ン"] = "n",
                ["エ"] = "e",
                ["ケ"] = "ke",
                ["セ"] = "se",
                ["テ"] = "te",
                ["ネ"] = "ne",
                ["ヘ"] = "he",
                ["メ"] = "me",
                ["レ"] = "re",
                ["オ"] = "o",
                ["コ"] = "ko",
                ["ソ"] = "so",
                ["ト"] = "to",
                ["ノ"] = "no",
                ["ホ"] = "ho",
                ["モ"] = "mo",
                ["ヨ"] = "yo",
                ["ロ"] = "ro",
                ["ヲ"] = "o",

                // 頭 邪 兆 - �� 咄 �� �� 咄
                ["ガ"] = "ga",
                ["ザ"] = "za",
                ["ダ"] = "da",
                ["バ"] = "ba",
                ["パ"] = "pa",
                ["ギ"] = "gi",
                ["ジ"] = "ji",
                ["ジ"] = "ji",
                ["ビ"] = "bi",
                ["ピ"] = "pi",
                ["グ"] = "gu",
                ["ズ"] = "zu",
                ["ズ"] = "zu",
                ["ブ"] = "bu",
                ["プ"] = "pu",
                ["ゲ"] = "ge",
                ["ゼ"] = "ze",
                ["デ"] = "de",
                ["ベ"] = "be",
                ["ペ"] = "pe",
                ["ゴ"] = "go",
                ["ゾ"] = "zo",
                ["ド"] = "do",
                ["ボ"] = "bo",
                ["ポ"] = "po",

                // 頭 邪 兆 - 渣 咄
                ["キャ"] = "kya",
                ["ギャ"] = "gya",
                ["シャ"] = "sha",
                ["ジャ"] = "ja",
                ["チャ"] = "cha",
                ["ニャ"] = "nya",
                ["ヒャ"] = "hya",
                ["ビャ"] = "bya",
                ["ピャ"] = "pya",
                ["ミャ"] = "mya",
                ["リャ"] = "rya",
                ["キュ"] = "kyu",
                ["ギュ"] = "gyu",
                ["シュ"] = "shu",
                ["ジュ"] = "ju",
                ["チュ"] = "chu",
                ["ニュ"] = "nyu",
                ["ヒュ"] = "hyu",
                ["ビュ"] = "byu",
                ["ピュ"] = "pyu",
                ["ミュ"] = "myu",
                ["チュ"] = "ryu",
                ["キョ"] = "kyo",
                ["ギョ"] = "gyo",
                ["ショ"] = "sho",
                ["ジョ"] = "jo",
                ["チョ"] = "cho",
                ["ニョ"] = "nyo",
                ["ヒョ"] = "hyo",
                ["ビョ"] = "byo",
                ["ピョ"] = "pyo",
                ["ミョ"] = "myo",
                ["リョ"] = "ryo",
            };
            Debug.Assert(Cached.Keys.Select(z => z.Length).Max() == 2);
        }

        #region Implementation of IWordBuilder

        public string Name => "Romaji";

        public IEnumerable<Word> Build(Thing thing, Word word)
        {
            var text = word.Text;
            var sb = new StringBuilder();
            for (var i = 0; i < text.Length; i++)
            {
                if (i + 1 < text.Length)
                {
                    var val = Cached.GetValueOrDefault(text.Substring(i, 2));
                    if (val != null)
                    {
                        sb.Append(val);
                        i++;
                        continue;
                    }
                }

                var val2 = Cached.GetValueOrDefault(text.Substring(i, 1));
                if (val2 != null)
                {
                    sb.Append(val2);
                }
                else
                {
                    sb.Append(text, i, 1);
                }
            }
            var retText = sb.ToString();
            if (retText != text)
            {
                yield return new Word
                {
                    Language = this.Name,
                    Text = retText
                };
            }
        }

        #endregion

        #region Implementation of IOrderable

        public int GetOrderCode() => 11;

        #endregion
    }
}