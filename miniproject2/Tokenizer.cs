using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace miniproject2
{
    /// <summary>
    /// Taken from
    /// http://sentiment.christopherpotts.net/code-data/happyfuntokenizing.py
    /// </summary>
    public static class Tokenizer
    {
        #region Regex Strings
        private static string EmoticonString =
            @"(?:" +
            @"[<>]?" +
            @"[:;=8]" +
            @"[-o*']?" +
            @"[\)\]\(\[dDpP/:\}\{@\|\\]" +
            @"|" +
            @"[\)\]\(\[dDpP/:\}\{@\|\\]" +
            @"[-o*']?" +
            @"[:;=8]" +
            @"[<>]?" +
            @")";

        private static string PhoneNumbers =
            @"(?:" +
                @"(?:" +
                    @"\+?[01]" +
                    @"[\-\s.]*" +
                @")?" +
                @"(?:" +
                    @"[\(]?" +
                    @"\d{3}" +
                    @"[\-\s.\)]*" +
                @")?" +
                @"\d{3}" +
                @"[\-\s.]*" +
                @"\d{4}" +
            @")";

        private static string HTMLTags = @"<[^>]>";

        private static string TwitterUserName = @"(?:@[\w_]+)";

        private static string TwitterHashTags = @"(?:#+[\w_]+[\w'_\-]*[\w_]+)";

        private static string RemainingWordTypes =
            @"(?:[a-z][a-z'\-_]+[a-z])" +
            @"|(?:[+\-]?\d+[,/.:-]\d+[+\-]?)" +
            @"|(?:[\w_]+)" +
            @"|(?:\.(?:\s*\.){1,})" +
            @"|(?:\S)";
        #endregion

        private static Regex EmoRegex { get; set; }
        private static Regex RegexTokenizer { get; set; }

        static Tokenizer()
        {
            EmoRegex = new Regex(EmoticonString, RegexOptions.Compiled);
            string[] RegexStrings ={
                                          PhoneNumbers,
                                          EmoticonString,
                                          HTMLTags,
                                          TwitterUserName,
                                          TwitterHashTags,
                                          RemainingWordTypes
                                      };

            string regString = string.Join("|", RegexStrings);
            RegexTokenizer = new Regex(regString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }


        private static Regex PunctuationRegex = new Regex("^[.:;!?]$", RegexOptions.Compiled);
        private static Regex NegationRegex = new Regex("(?:^(?:never|no|nothing|nowhere|noone|none|not|havent|hasnt|hadnt|cant|couldnt|shouldnt|wont|wouldnt|dont|doesnt|didnt|isnt|arent|aint)$)|n't", RegexOptions.Compiled);

        public static IEnumerable<string> Tokenize(string s)
        {
            var matchList = RegexTokenizer.Matches(s);
            var matches = matchList.Cast<Match>()
                .Select(m => m.Value);
            var lowered = matches
                .Select(m => EmoRegex.IsMatch(m) ? m : m.ToLower()).ToArray();

            bool neg = false;
            for (int i = 0; i < lowered.Length; i++)
            {
                if (neg)
                {
                    if (PunctuationRegex.IsMatch(lowered[i]))
                    {
                        neg = false;
                    }
                    else
                    {
                        lowered[i] = lowered[i] + "_NEG";
                    }
                }
                else
                {
                    if (NegationRegex.IsMatch(lowered[i]))
                    {
                        neg = true;
                    }
                }
            }

            foreach (var token in lowered)
            {
                // punct - neg=false
                // neg - neg=true
            }

            return lowered;
        }
    }
}
