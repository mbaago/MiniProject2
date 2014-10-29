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

        public static IEnumerable<string> Tokenize(string s)
        {
            var matchList = RegexTokenizer.Matches(s);
            var matches = matchList.Cast<Match>()
                .Select(m => m.Value);
            var lowered = matches
                .Select(m => EmoRegex.IsMatch(m) ? m : m.ToLower());

            return lowered;
        }
    }
}
