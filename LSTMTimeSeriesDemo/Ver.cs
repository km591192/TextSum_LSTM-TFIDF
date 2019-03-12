using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LSTMTimeSeriesDemo
{
    class Ver
    {
        public SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
        public int count_all_world = 0, countwordintext = 0;
        public SortedDictionary<string, string> dict2 = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> dict2_SLF = new SortedDictionary<string, string>();

        public string origintext = "";
        public List<string> lw = new List<string>();
        public List<string> lwsw = new List<string>();
        public List<string> lstr = new List<string>();

        WordStemming ws = new WordStemming();

        public void dictdata(string fn, string[] fnarr)
        {
            count_all_world = 0;
            countwordintext = 0;
            dict2.Clear();
            veroyt(fn, fnarr);
        }

        public void word_list(string filename, string[] fnarr)
        {
            string text = "";
            for (int i = 0; i < fnarr.Length; i++)
            {
                using (StreamReader fs = new StreamReader(fnarr[i], Encoding.Default))
                {
                    if (filename == fnarr[i])
                    {
                        while (true)
                        {
                            string temp = fs.ReadLine();
                            if (temp == null) break;
                            text += temp + " ";
                        }
                        origintext = text;
                        text = text.ToLower();

                        string[] textarr = ws.TextArr(text);
                        lw = ws.regTextarr;
                        lwsw = ws.regTextarr_sw;
                        lstr = ws.textMass1;
                        lstr = origintext.Split('.').ToList(); //UpperLetter
                    }

                }
            }
        }

        public void veroyt(string filename, string[] fnarr)
        {
            int count = 0, countword = 0, countwordfile = 0;
            string text = "";
            string stemtext = "";
            string[] temparr = new string[fnarr.Length];
            int k = 0;

            if (filename != fnarr[0] && fnarr.Length > 1)
            {

                for (int i = 0; i < fnarr.Length; i++)
                {
                    if (fnarr[i] != filename)
                    {
                        temparr[k + 1] = fnarr[i];
                        k++;
                    }
                    if (fnarr[i] == filename)
                    {
                        temparr[0] = filename;
                    }
                }
                fnarr = temparr;
            }

            for (int i = 0; i < fnarr.Length; i++)
            {
                using (StreamReader fs = new StreamReader(fnarr[i], Encoding.Default))
                {
                    while (true)
                    {
                        string temp = fs.ReadLine();
                        if (temp == null) break;
                        text += temp + " ";
                    }
                }
                text = text.ToLower();

                string[] textarr = ws.TextArr(text);
                lw = ws.regTextarr;

                stemtext = ws.stem_text(textarr);
                string[] textMass = stemtext.Split(' ');
                for (int u = 0; u < textMass.Length; u++)
                {
                    if (textMass[u].Length > 1 && textMass[u] != "")
                    {
                        countword = ws.count_word(textMass[u], stemtext);
                        countwordintext++;
                        string fnstr = fnarr[i];
                        if (fnstr == filename)
                        {
                            countwordfile = countword;
                            count_all_world++;
                            count++;
                            if (!dict2.Keys.Contains(textarr[u].Trim()))
                            {
                                if (filename.Contains("textclasspaint"))
                                {
                                    dict2.Add(textarr[u], countwordfile.ToString() + "|" + count.ToString() + "|" + 1 + "|" + 0 + "|" + 0);
                                }

                                if (filename.Contains("textclassscience"))
                                {
                                    dict2.Add(textarr[u], countwordfile.ToString() + "|" + count.ToString() + "|" + 0 + "|" + 1 + "|" + 0);
                                }

                                if (filename.Contains("textclasssport"))
                                {
                                    dict2.Add(textarr[u], countwordfile.ToString() + "|" + count.ToString() + "|" + 0 + "|" + 0 + "|" + 1);
                                }
                            }
                            count = 0;
                        }
                    }
                }



                string[] textMass1 = textMass.Distinct().ToArray();
                string[] textarr1 = textarr.Distinct().ToArray();


                if (fnarr[i] != filename)
                {
                    for (int u = 0; u < textMass1.Length; u++)
                    {
                        if (textMass1[u] != "")
                        {
                            string strdict2 = "";
                            dict2.TryGetValue(textarr1[u], out strdict2);
                            if (strdict2 != null)
                            {
                                string[] strdict2arr = strdict2.Split('|');
                                int countdict2 = Convert.ToInt16(strdict2arr[1]);
                                countdict2++;
                                if (fnarr[i].Contains("textclasspaint"))
                                {
                                    if (strdict2arr.Length < 2)
                                        dict2[textarr1[u]] = strdict2arr[0] + "|" + countdict2 + "|" + 1 + "|" + 0 + "|" + 0;

                                    if (strdict2arr.Length > 2)
                                        dict2[textarr1[u]] = strdict2arr[0] + "|" + countdict2 + "|" + (Convert.ToDouble(strdict2arr[2]) + 1).ToString() + "|" + strdict2arr[3] + "|" + strdict2arr[4];

                                }

                                if (fnarr[i].Contains("textclassscience"))
                                {
                                    if (strdict2arr.Length < 2)
                                        dict2[textarr1[u]] = strdict2arr[0] + "|" + countdict2 + "|" + 0 + "|" + 1 + "|" + 0;

                                    if (strdict2arr.Length > 2)
                                        dict2[textarr1[u]] = strdict2arr[0] + "|" + countdict2 + "|" + strdict2arr[2] + "|" + (Convert.ToDouble(strdict2arr[3]) + 1).ToString() + "|" + strdict2arr[4];
                                }

                                if (fnarr[i].Contains("textclasssport"))
                                {
                                    if (strdict2arr.Length < 2)
                                        dict2[textarr1[u]] = strdict2arr[0] + "|" + countdict2 + "|" + 0 + "|" + 0 + "|" + 1;

                                    if (strdict2arr.Length > 2)
                                        dict2[textarr1[u]] = strdict2arr[0] + "|" + countdict2 + "|" + strdict2arr[2] + "|" + strdict2arr[3] + "|" + (Convert.ToDouble(strdict2arr[4]) + 1).ToString();
                                }
                                
                                string strdict = "";
                                dict.TryGetValue(textarr1[u], out strdict);
                                if (strdict == null)
                                {
                                    dict.Add(textarr1[u], strdict2arr[0] + "|" + fnarr[i] + "-" + ws.count_word(ws.Stem(textarr1[u]), stemtext) + ":" + countdict2);
                                }
                                if (strdict != null)
                                {
                                    dict[textarr1[u]] += "/" + fnarr[i] + "-" + ws.count_word(ws.Stem(textarr1[u]), stemtext) + ":" + countdict2;
                                }
                            }
                        }
                    }

                    string strcountallword = "";
                    dict.TryGetValue(fnarr[i].Trim(), out strcountallword);
                    if (!dict.Keys.Contains(fnarr[i].Trim()))
                        dict.Add(fnarr[i], countwordintext.ToString());
                    dict.TryGetValue(fnarr[i].Trim(), out strcountallword);
                    if (dict.Keys.Contains(fnarr[i].Trim()) && !(Convert.ToInt16(strcountallword) == countwordintext))
                        dict.Add(i.ToString() + fnarr[i], countwordintext.ToString());
                    countwordintext = 0;
                }
                count = 0;
                text = "";

            }
        }

        public int kolvodoksoslovom(string[] fn, string word)
        {
            int count = 0, countword = 0;
            string text = "";
            string stemtext = "";
            for (int i = 0; i < fn.Length; i++)
            {
                using (StreamReader fs = new StreamReader(fn[i], Encoding.Default))
                {
                    while (true)
                    {
                        string temp = fs.ReadLine();
                        if (temp == null) break;
                        text += temp + " ";
                    }
                }
                text = text.ToLower();

                string[] strarr = ws.TextArr(text);
                stemtext = ws.stem_text(strarr);

                string stem_slov = ws.Stem(word);
                if (stem_slov.Length > 1 && stem_slov != "")
                {
                    countword = ws.count_word(stem_slov, stemtext);
                }
                if (countword > 0)
                {
                    countword = 0;
                    count++;
                }
                text = "";
            }
            return count;

        }



        private string kolvoslovorazvfile(string slovo, string fn)
        {
            try
            {
                int count = 0;
                string[] strarr = ws.TextArr(fn);
                string str = ws.stem_text(strarr);
                string answer = "";

                string stem_slov = ws.Stem(slovo);
                if (stem_slov.Length > 1 && stem_slov != "")
                {
                    count = ws.count_word(stem_slov, str);
                    answer = slovo + " - " + count.ToString();
                    count_all_world++;
                    countwordintext++;
                }
                return answer;
            }
            catch (Exception ex) { return ""; }

        }

        private string print_word_ver(string fn, string[] fnarr)
        {
            string text = "", answer = "";

            using (StreamReader fs = new StreamReader(fn, Encoding.Default))
            {
                while (true)
                {
                    string temp = fs.ReadLine();
                    if (temp == null) break;
                    text += temp + " ";
                }
            }
            text = text.ToLower();


            string pattern = @"\b[а-яa-z]+\b";
            Regex reg = new Regex(pattern);
            MatchCollection mc = reg.Matches(text);
            string regtext = "";
            foreach (Match m in mc)
            {
                regtext += m.Value + " ";
            }
            string[] textMass = regtext.Split(' ');

            for (int i = 0; i < textMass.Length; i++)
            {
                if (textMass[i] != "")
                    answer += kolvoslovorazvfile(textMass[i].Trim(), text) + "|" + kolvodoksoslovom(fnarr, textMass[i].Trim()) + "\n";
            }

            return answer;
        }




    }
}
