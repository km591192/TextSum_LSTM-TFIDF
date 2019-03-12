using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LSTMTimeSeriesDemo
{
    public class WordStemming
    {

        const string c_vower = "аеиоуыэюя";
        const string c_perfectiveground = "((ив|ивши|ившись|ыв|ывши|ывшись)|((?<=[ая])(в|вши|вшись)))$";
        const string c_reflexive = "(с[яь])$";
        const string c_adjective = "(ее|ие|ые|ое|ими|ыми|ей|ий|ый|ой|ем|им|ым|ом|его|ого|еых|ую|юю|ая|яя|ою|ею)$";
        const string c_participle = "((ивш|ывш|ующ)|((?<=[ая])(ем|нн|вш|ющ|щ)))$";
        const string c_verb = "((ила|ыла|ена|ейте|уйте|ите|или|ыли|ей|уй|ил|ыл|им|ым|ены|ить|ыть|ишь|ую|ю)|((?<=[ая])(ла|на|ете|йте|ли|й|л|ем|н|ло|но|ет|ют|ны|ть|ешь|нно)))$";
        const string c_noun = "(а|ев|ов|ие|ье|е|иями|ями|ами|еи|ии|и|ией|ей|ой|ий|й|и|иям|ям|ием|ем|ам|ом|о|у|ах|иях|ях|ы|ь|ию|ью|ю|ия|ья|я)$";
        const string c_rvre = "^(.*?[аеиоуыэюя])(.*)$";
        const string c_derivational = "[^аеиоуыэюя][аеиоуыэюя]+[^аеиоуыэюя]+[аеиоуыэюя].*(?<=о)сть?$";

        // public WordStemming(){}

        public List<string> regTextarr = new List<string>();
        public List<string> regTextarr_sw = new List<string>();
        public List<string> textMass1 = new List<string>();

        public int count_word(string word, string str)
        {
            int count = 0;

            string[] strarr = str.Trim().Split(' ');
            for (int i = 0; i < strarr.Length; i++)
            {
                if (word.Trim() == strarr[i].Trim())
                    count++;
            }
            return count;

        }
        public string[] TextArr(string fn)
        {
            regTextarr.Clear();
            textMass1 = fn.Split('\n', '.', '?', '!').ToList();
            string pattern = @"\b[а-яa-z]+\b";
            Regex reg = new Regex(pattern);
            MatchCollection mc = reg.Matches(fn);
            string regtext = "";
            foreach (Match m in mc)
            {
                regtext += m.Value + " ";
            }
            string[] textMass = regtext.Split(' ');

            regTextarr = textMass.ToList();

            string temp = String.Empty;
            int count = 0;
            foreach (var word in textMass)
            {
                foreach (var sword in StopWord.stopWordsList)
                {
                    if (word.Trim() != sword.Trim())
                        count++;

                    if (count == StopWord.stopWordsList.Count())
                    {
                        temp += word + " ";
                        count = 0;
                    }
                }
                count = 0;
            }
            textMass = temp.Split(' ');

            regTextarr_sw = textMass.ToList();

            return textMass;
        }

        public string stem_text(string[] textMass)
        {
            string answer = "";
            for (int i = 0; i < textMass.Length; i++)
            {
                if (textMass[i] != "")
                    answer += Stem(textMass[i].Trim()) + " ";
            }

            return answer;
        }

        bool RegexReplace(ref string Original, string Regx, string Value)
        {
            string original = Original;
            Regex reg = new Regex(Regx);
            Original = reg.Replace(Original, Value);
            return (Original != original);
        }

        Match RegexMatch(string Original, string Regx)
        {
            Regex reg = new Regex(Regx);
            return reg.Match(Original);
        }

        MatchCollection RegexMatches(string Original, string Regx)
        {
            Regex reg = new Regex(Regx, RegexOptions.Multiline);
            return reg.Matches(Original);
        }

        public string Parse(string Query)
        {
            Regex reg = new Regex(@"[ ,\.\?!=\&\*\+]");
            string[] words = reg.Split(Query);
            ArrayList swords = new ArrayList();

            for (int i = 0; i < words.Length; i++)
                if (!string.IsNullOrEmpty(words[i].Trim()))
                    swords.Add(Stem(words[i].Trim()));

            string result = string.Join("%", (String[])(swords.ToArray(typeof(string))));

            return string.Format("%{0}%", result);
        }


        public string Stem(string Word)
        {
            string word = Word.ToLower().Trim().Replace("ё", "е");
            string value = string.Empty;
            do
            {
                MatchCollection matches = RegexMatches(word, c_rvre);
                if (matches.Count < 1)
                {
                    Match matchEnglishOrDigits = RegexMatch(word, "[a-z0-9]");
                    if (matchEnglishOrDigits.Success)
                        value = word;

                    break;
                }

                string rv = matches[0].Value;

                if (!RegexReplace(ref rv, c_perfectiveground, string.Empty))
                {
                    RegexReplace(ref rv, c_reflexive, string.Empty);

                    if (RegexReplace(ref rv, c_adjective, string.Empty))
                        RegexReplace(ref rv, c_participle, string.Empty);
                    else
                        if (!RegexReplace(ref rv, c_verb, string.Empty))
                        RegexReplace(ref rv, c_noun, string.Empty);
                }

                RegexReplace(ref rv, "и$", string.Empty);

                Match match = RegexMatch(rv, c_derivational);
                if (match.Success)
                    RegexReplace(ref rv, "ость?$", string.Empty);

                if (!RegexReplace(ref rv, "ь$", string.Empty))
                {
                    RegexReplace(ref rv, "ейше?", string.Empty);
                    RegexReplace(ref rv, "нн$", "н");
                }

                value = rv;

            } while (false);

            return value;
        }

    }

}
