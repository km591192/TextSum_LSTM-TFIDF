using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTMTimeSeriesDemo
{
    class Realize_print
    {
        List<string> strlist = new List<string>();
        List<string> list = new List<string>();
        List<string> templist = new List<string>();
        List<string> list_word = new List<string>();
        List<string> list_str = new List<string>();
        List<string> list_word_sw = new List<string>();
        List<string> list_highwordcount = new List<string>();
        List<string> list_highwordcountstem = new List<string>();
        List<string> list_highverword = new List<string>();
        List<string> list_highverwordstem = new List<string>();
        List<string> listfromtfidf = new List<string>();
        List<string> listfromtfidf_distinct = new List<string>();
        List<string> listtoneironfromfn = new List<string>();
        List<double> weights = new List<double>();
        String origintext = "";

        TF_IDF tfidf = new TF_IDF();
        WordStemming ws = new WordStemming();
        Ver ver = new Ver();

        static string[] fn = { "textclasspaint.txt", "textclassscience.txt", "textclasssport.txt" };
        static public int kolvo_classov = 3;

        //  public static string classif_fn = "textclassscience.txt"; //ТУТ МЕНЯТЬ ФАЙЛ ДЛЯ ТЕСТИРОВАНИЯ!!!!!
        public static string classif_fn = Select_text.ReadFN();

        double[][] arr_input = new double[fn.Length][];
        double[,] arr_w_2l = new double[30, kolvo_classov];
        double[,] arr_w_1l = new double[25, 30];
        double[] arr_f_1l = new double[30];
        double[] arr_f_2l = new double[kolvo_classov];
        double[][] arr_z = new double[fn.Length][];
        double[] delta = new double[kolvo_classov];
        double[] delta_mid = new double[30];
        double[] delta_1l = new double[25];

        public List<string> printword()
        {
            Clearlistdict();
            classif_fn = Select_text.ReadFN();
            ver.word_list(classif_fn, fn);
            list_word = ver.lw;
            return list_word;
        }

        public List<string> printwordsw()
        {
            Clearlistdict();
            classif_fn = Select_text.ReadFN();
            ver.word_list(classif_fn, fn);
            list_word_sw = ver.lwsw;
            return list_word_sw;
        }

        public List<string> printstr()
        {
            Clearlistdict();
            classif_fn = Select_text.ReadFN();
            ver.word_list(classif_fn, fn);
            list_str = ver.lstr;
            /*for (int i = 0; i < list_str.Count;i++)
                if(list_str[i].Trim() != "")
                list_str[i] = FirstUpper(list_str[i].Trim());*/
            return list_str;
        }

        public  string FirstUpper(string str)
        {
            return str.Substring(0, 1).ToUpper() + (str.Length > 1 ? str.Substring(1) : "");
        }

        public String printorigintext()
        {
            origintext = "";
            classif_fn = Select_text.ReadFN();
            ver.word_list(classif_fn, fn);
            origintext = ver.origintext;
            return origintext;
        }

        public List<string> printword_count()
        {
            Clearlistdict();
            classif_fn = Select_text.ReadFN();
            ver.dictdata(classif_fn, fn);
            foreach (string k in ver.dict2.Keys)
            {
                string tempstr = "";
                ver.dict2.TryGetValue(k, out tempstr);
                list.Add(k + " - " + tempstr);
            }
            list.Add("All word count = " + ver.count_all_world.ToString());
            return list;
        }

        public List<string> printword_ver_TFIDF()
        {
            classif_fn = Select_text.ReadFN();
            strlist = tfidf.TF_IDF_ver(classif_fn, fn, ver.dict2, ver.count_all_world);
            strlist.Sort();
            return strlist;
        }
        public List<string> printword_ver_TFSLF()
        {
            classif_fn = Select_text.ReadFN();
            strlist = tfidf.TF_SLF_ver(classif_fn, fn, ver.dict2, ver.count_all_world);
            strlist.Sort();
            return strlist;
        }

        public List<string> print_highverword()
        {
            strlist.Reverse();
            int count = 0;
            foreach (string f in strlist)
            {
                if ((!list_highverwordstem.Contains(ws.Stem(f))) && count < 25)
                {
                    list_highverwordstem.Add(ws.Stem(f));
                    list_highverword.Add(f);
                    count++;
                }
            }
            return list_highverword;
        }

        public List<string> print_highwordcount()
        {
            classif_fn = Select_text.ReadFN();
            ver.dictdata(classif_fn, fn);
            foreach (string k in ver.dict2.Keys)
            {
                string tempstr = "";
                ver.dict2.TryGetValue(k, out tempstr);
                string[] tempstrarr = tempstr.Split('|');
                templist.Add(tempstr[0] + " - " + k);
            }
            templist.Sort();
            templist.Reverse();
            int count = 0;
            foreach (string f in templist)
            {
                if ((!list_highwordcountstem.Contains(ws.Stem(f))) && count < 25)
                {
                    list_highwordcountstem.Add(ws.Stem(f));
                    list_highwordcount.Add(f);
                    count++;
                }
            }
            return list_highwordcount;
        }

        private void Clearlistdict()
        {
            ver.dict.Clear();
            ver.dict2.Clear();
            list_highverword.Clear();
            list_word.Clear();
            list_highverwordstem.Clear();
            list_highwordcount.Clear();
            list_highwordcountstem.Clear();
            weights.Clear();
        }
        private List<string> x_list_TFIDF()
        {
            Clearlistdict();
            printword_count();
            printword_ver_TFIDF();
            print_highverword();
            return list_highverword;
        }
        private List<string> x_list_TFSLF()
        {
            Clearlistdict();
            printword_count();
            printword_ver_TFSLF();
            print_highverword();
            return list_highverword;
        }



    }
}
