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
                // Æ½ ¼Ù Ãû - Çå Òô
                ["¤¢"] = "a",
                ["¤«"] = "ka",
                ["¤µ"] = "sa",
                ["¤¿"] = "ta",
                ["¤Ê"] = "na",
                ["¤Ï"] = "ha",
                ["¤Þ"] = "ma",
                ["¤ä"] = "ya",
                ["¤é"] = "ra",
                ["¤ï"] = "wa",
                ["¤¤"] = "i",
                ["¤­"] = "ki",
                ["¤·"] = "shi",
                ["¤Á"] = "chi",
                ["¤Ë"] = "ni",
                ["¤Ò"] = "hi",
                ["¤ß"] = "mi",
                ["¤ê"] = "ri",
                ["¤¦"] = "u",
                ["¤¯"] = "ku",
                ["¤¹"] = "su",
                ["¤Ä"] = "tsu",
                ["¤Ì"] = "nu",
                ["¤Õ"] = "fu",
                ["¤à"] = "mu",
                ["¤æ"] = "yu",
                ["¤ë"] = "ru",
                ["¤ó"] = "n",
                ["¤¨"] = "e",
                ["¤±"] = "ke",
                ["¤»"] = "se",
                ["¤Æ"] = "te",
                ["¤Í"] = "ne",
                ["¤Ø"] = "he",
                ["¤á"] = "me",
                ["¤ì"] = "re",
                ["¤ª"] = "o",
                ["¤³"] = "ko",
                ["¤½"] = "so",
                ["¤È"] = "to",
                ["¤Î"] = "no",
                ["¤Û"] = "ho",
                ["¤â"] = "mo",
                ["¤è"] = "yo",
                ["¤í"] = "ro",
                ["¤ò"] = "o",

                // Æ½ ¼Ù Ãû - á Òô £¦ °ë á Òô
                ["¤¬"] = "ga",
                ["¤¶"] = "za",
                ["¤À"] = "da",
                ["¤Ð"] = "ba",
                ["¤Ñ"] = "pa",
                ["¤®"] = "gi",
                ["¤¸"] = "ji",
                ["¤Â"] = "ji",
                ["¤Ó"] = "bi",
                ["¤Ô"] = "pi",
                ["¤°"] = "gu",
                ["¤º"] = "zu",
                ["¤Å"] = "zu",
                ["¤Ö"] = "bu",
                ["¤×"] = "pu",
                ["¤²"] = "ge",
                ["¤¼"] = "ze",
                ["¤Ç"] = "de",
                ["¤Ù"] = "be",
                ["¤Ú"] = "pe",
                ["¤´"] = "go",
                ["¤¾"] = "zo",
                ["¤É"] = "do",
                ["¤Ü"] = "bo",
                ["¤Ý"] = "po",

                // Æ½ ¼Ù Ãû - ÞÖ Òô
                ["¤­¤ã"] = "kya",
                ["¤®¤ã"] = "gya",
                ["¤·¤ã"] = "sha",
                ["¤¸¤ã"] = "ja",
                ["¤Á¤ã"] = "cha",
                ["¤Ë¤ã"] = "nya",
                ["¤Ò¤ã"] = "hya",
                ["¤Ó¤ã"] = "bya",
                ["¤Ô¤ã"] = "pya",
                ["¤ß¤ã"] = "mya",
                ["¤ê¤ã"] = "rya",
                ["¤­¤å"] = "kyu",
                ["¤®¤å"] = "gyu",
                ["¤·¤å"] = "shu",
                ["¤¸¤å"] = "ju",
                ["¤Á¤å"] = "chu",
                ["¤Ë¤å"] = "nyu",
                ["¤Ò¤å"] = "hyu",
                ["¤Ó¤å"] = "byu",
                ["¤Ô¤å"] = "pyu",
                ["¤ß¤å"] = "myu",
                ["¤ê¤å"] = "ryu",
                ["¤­¤ç"] = "kyo",
                ["¤®¤ç"] = "gyo",
                ["¤·¤ç"] = "sho",
                ["¤¸¤ç"] = "jo",
                ["¤Á¤ç"] = "cho",
                ["¤Ë¤ç"] = "nyo",
                ["¤Ò¤ç"] = "hyo",
                ["¤Ó¤ç"] = "byo",
                ["¤Ô¤ç"] = "pyo",
                ["¤ß¤ç"] = "myo",
                ["¤ê¤ç"] = "ryo",

                // Æ¬ ¼Ù Ãû - Çå Òô
                ["¥¢"] = "a",
                ["¥«"] = "ka",
                ["¥µ"] = "sa",
                ["¥¿"] = "ta",
                ["¥Ê"] = "na",
                ["¥Ï"] = "ha",
                ["¥Þ"] = "ma",
                ["¥ä"] = "ya",
                ["¥é"] = "ra",
                ["¥ï"] = "wa",
                ["¥¤"] = "i",
                ["¥­"] = "ki",
                ["¥·"] = "shi",
                ["¥Á"] = "chi",
                ["¥Ë"] = "ni",
                ["¥Ò"] = "hi",
                ["¥ß"] = "mi",
                ["¥ê"] = "ri",
                ["¥¦"] = "u",
                ["¥¯"] = "ku",
                ["¥¹"] = "su",
                ["¥Ä"] = "tsu",
                ["¥Ì"] = "nu",
                ["¥Õ"] = "fu",
                ["¥à"] = "mu",
                ["¥æ"] = "yu",
                ["¥ë"] = "ru",
                ["¥ó"] = "n",
                ["¥¨"] = "e",
                ["¥±"] = "ke",
                ["¥»"] = "se",
                ["¥Æ"] = "te",
                ["¥Í"] = "ne",
                ["¥Ø"] = "he",
                ["¥á"] = "me",
                ["¥ì"] = "re",
                ["¥ª"] = "o",
                ["¥³"] = "ko",
                ["¥½"] = "so",
                ["¥È"] = "to",
                ["¥Î"] = "no",
                ["¥Û"] = "ho",
                ["¥â"] = "mo",
                ["¥è"] = "yo",
                ["¥í"] = "ro",
                ["¥ò"] = "o",

                // Æ¬ ¼Ù Ãû - á Òô £¦ á Òô
                ["¥¬"] = "ga",
                ["¥¶"] = "za",
                ["¥À"] = "da",
                ["¥Ð"] = "ba",
                ["¥Ñ"] = "pa",
                ["¥®"] = "gi",
                ["¥¸"] = "ji",
                ["¥¸"] = "ji",
                ["¥Ó"] = "bi",
                ["¥Ô"] = "pi",
                ["¥°"] = "gu",
                ["¥º"] = "zu",
                ["¥º"] = "zu",
                ["¥Ö"] = "bu",
                ["¥×"] = "pu",
                ["¥²"] = "ge",
                ["¥¼"] = "ze",
                ["¥Ç"] = "de",
                ["¥Ù"] = "be",
                ["¥Ú"] = "pe",
                ["¥´"] = "go",
                ["¥¾"] = "zo",
                ["¥É"] = "do",
                ["¥Ü"] = "bo",
                ["¥Ý"] = "po",

                // Æ¬ ¼Ù Ãû - ÞÖ Òô
                ["¥­¥ã"] = "kya",
                ["¥®¥ã"] = "gya",
                ["¥·¥ã"] = "sha",
                ["¥¸¥ã"] = "ja",
                ["¥Á¥ã"] = "cha",
                ["¥Ë¥ã"] = "nya",
                ["¥Ò¥ã"] = "hya",
                ["¥Ó¥ã"] = "bya",
                ["¥Ô¥ã"] = "pya",
                ["¥ß¥ã"] = "mya",
                ["¥ê¥ã"] = "rya",
                ["¥­¥å"] = "kyu",
                ["¥®¥å"] = "gyu",
                ["¥·¥å"] = "shu",
                ["¥¸¥å"] = "ju",
                ["¥Á¥å"] = "chu",
                ["¥Ë¥å"] = "nyu",
                ["¥Ò¥å"] = "hyu",
                ["¥Ó¥å"] = "byu",
                ["¥Ô¥å"] = "pyu",
                ["¥ß¥å"] = "myu",
                ["¥Á¥å"] = "ryu",
                ["¥­¥ç"] = "kyo",
                ["¥®¥ç"] = "gyo",
                ["¥·¥ç"] = "sho",
                ["¥¸¥ç"] = "jo",
                ["¥Á¥ç"] = "cho",
                ["¥Ë¥ç"] = "nyo",
                ["¥Ò¥ç"] = "hyo",
                ["¥Ó¥ç"] = "byo",
                ["¥Ô¥ç"] = "pyo",
                ["¥ß¥ç"] = "myo",
                ["¥ê¥ç"] = "ryo",
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