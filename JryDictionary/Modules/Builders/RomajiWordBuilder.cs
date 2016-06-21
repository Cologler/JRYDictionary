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
                // ƽ �� �� - �� ��
                ["��"] = "a",
                ["��"] = "ka",
                ["��"] = "sa",
                ["��"] = "ta",
                ["��"] = "na",
                ["��"] = "ha",
                ["��"] = "ma",
                ["��"] = "ya",
                ["��"] = "ra",
                ["��"] = "wa",
                ["��"] = "i",
                ["��"] = "ki",
                ["��"] = "shi",
                ["��"] = "chi",
                ["��"] = "ni",
                ["��"] = "hi",
                ["��"] = "mi",
                ["��"] = "ri",
                ["��"] = "u",
                ["��"] = "ku",
                ["��"] = "su",
                ["��"] = "tsu",
                ["��"] = "nu",
                ["��"] = "fu",
                ["��"] = "mu",
                ["��"] = "yu",
                ["��"] = "ru",
                ["��"] = "n",
                ["��"] = "e",
                ["��"] = "ke",
                ["��"] = "se",
                ["��"] = "te",
                ["��"] = "ne",
                ["��"] = "he",
                ["��"] = "me",
                ["��"] = "re",
                ["��"] = "o",
                ["��"] = "ko",
                ["��"] = "so",
                ["��"] = "to",
                ["��"] = "no",
                ["��"] = "ho",
                ["��"] = "mo",
                ["��"] = "yo",
                ["��"] = "ro",
                ["��"] = "o",

                // ƽ �� �� - �� �� �� �� �� ��
                ["��"] = "ga",
                ["��"] = "za",
                ["��"] = "da",
                ["��"] = "ba",
                ["��"] = "pa",
                ["��"] = "gi",
                ["��"] = "ji",
                ["��"] = "ji",
                ["��"] = "bi",
                ["��"] = "pi",
                ["��"] = "gu",
                ["��"] = "zu",
                ["��"] = "zu",
                ["��"] = "bu",
                ["��"] = "pu",
                ["��"] = "ge",
                ["��"] = "ze",
                ["��"] = "de",
                ["��"] = "be",
                ["��"] = "pe",
                ["��"] = "go",
                ["��"] = "zo",
                ["��"] = "do",
                ["��"] = "bo",
                ["��"] = "po",

                // ƽ �� �� - �� ��
                ["����"] = "kya",
                ["����"] = "gya",
                ["����"] = "sha",
                ["����"] = "ja",
                ["����"] = "cha",
                ["�ˤ�"] = "nya",
                ["�Ҥ�"] = "hya",
                ["�Ӥ�"] = "bya",
                ["�Ԥ�"] = "pya",
                ["�ߤ�"] = "mya",
                ["���"] = "rya",
                ["����"] = "kyu",
                ["����"] = "gyu",
                ["����"] = "shu",
                ["����"] = "ju",
                ["����"] = "chu",
                ["�ˤ�"] = "nyu",
                ["�Ҥ�"] = "hyu",
                ["�Ӥ�"] = "byu",
                ["�Ԥ�"] = "pyu",
                ["�ߤ�"] = "myu",
                ["���"] = "ryu",
                ["����"] = "kyo",
                ["����"] = "gyo",
                ["����"] = "sho",
                ["����"] = "jo",
                ["����"] = "cho",
                ["�ˤ�"] = "nyo",
                ["�Ҥ�"] = "hyo",
                ["�Ӥ�"] = "byo",
                ["�Ԥ�"] = "pyo",
                ["�ߤ�"] = "myo",
                ["���"] = "ryo",

                // Ƭ �� �� - �� ��
                ["��"] = "a",
                ["��"] = "ka",
                ["��"] = "sa",
                ["��"] = "ta",
                ["��"] = "na",
                ["��"] = "ha",
                ["��"] = "ma",
                ["��"] = "ya",
                ["��"] = "ra",
                ["��"] = "wa",
                ["��"] = "i",
                ["��"] = "ki",
                ["��"] = "shi",
                ["��"] = "chi",
                ["��"] = "ni",
                ["��"] = "hi",
                ["��"] = "mi",
                ["��"] = "ri",
                ["��"] = "u",
                ["��"] = "ku",
                ["��"] = "su",
                ["��"] = "tsu",
                ["��"] = "nu",
                ["��"] = "fu",
                ["��"] = "mu",
                ["��"] = "yu",
                ["��"] = "ru",
                ["��"] = "n",
                ["��"] = "e",
                ["��"] = "ke",
                ["��"] = "se",
                ["��"] = "te",
                ["��"] = "ne",
                ["��"] = "he",
                ["��"] = "me",
                ["��"] = "re",
                ["��"] = "o",
                ["��"] = "ko",
                ["��"] = "so",
                ["��"] = "to",
                ["��"] = "no",
                ["��"] = "ho",
                ["��"] = "mo",
                ["��"] = "yo",
                ["��"] = "ro",
                ["��"] = "o",

                // Ƭ �� �� - �� �� �� �� ��
                ["��"] = "ga",
                ["��"] = "za",
                ["��"] = "da",
                ["��"] = "ba",
                ["��"] = "pa",
                ["��"] = "gi",
                ["��"] = "ji",
                ["��"] = "ji",
                ["��"] = "bi",
                ["��"] = "pi",
                ["��"] = "gu",
                ["��"] = "zu",
                ["��"] = "zu",
                ["��"] = "bu",
                ["��"] = "pu",
                ["��"] = "ge",
                ["��"] = "ze",
                ["��"] = "de",
                ["��"] = "be",
                ["��"] = "pe",
                ["��"] = "go",
                ["��"] = "zo",
                ["��"] = "do",
                ["��"] = "bo",
                ["��"] = "po",

                // Ƭ �� �� - �� ��
                ["����"] = "kya",
                ["����"] = "gya",
                ["����"] = "sha",
                ["����"] = "ja",
                ["����"] = "cha",
                ["�˥�"] = "nya",
                ["�ҥ�"] = "hya",
                ["�ӥ�"] = "bya",
                ["�ԥ�"] = "pya",
                ["�ߥ�"] = "mya",
                ["���"] = "rya",
                ["����"] = "kyu",
                ["����"] = "gyu",
                ["����"] = "shu",
                ["����"] = "ju",
                ["����"] = "chu",
                ["�˥�"] = "nyu",
                ["�ҥ�"] = "hyu",
                ["�ӥ�"] = "byu",
                ["�ԥ�"] = "pyu",
                ["�ߥ�"] = "myu",
                ["����"] = "ryu",
                ["����"] = "kyo",
                ["����"] = "gyo",
                ["����"] = "sho",
                ["����"] = "jo",
                ["����"] = "cho",
                ["�˥�"] = "nyo",
                ["�ҥ�"] = "hyo",
                ["�ӥ�"] = "byo",
                ["�ԥ�"] = "pyo",
                ["�ߥ�"] = "myo",
                ["���"] = "ryo",
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