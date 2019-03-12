using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTMTimeSeriesDemo
{
    public class WordVec
    {
        Call_print print = new Call_print();
        List<string> list_word_count = new List<string>();
        List<string> list_word = new List<string>();
        public List<string> list_word_ver = new List<string>();
        public List<string> list_high_word_ver = new List<string>();
        public List<string> list_high_word_count = new List<string>();
        public List<string> list_str = new List<string>();
        public string origintext = "";

        public List<string> list_wv = new List<string>();

        public float[] vec;

        public void wv()
        {
            list_word_count.Clear();
            list_word_ver.Clear();
            list_high_word_ver.Clear();
            list_high_word_count.Clear();
            origintext = "";
            list_str.Clear();
            list_word.Clear();
            list_wv.Clear();

            list_word_count = print.printword_count();
            list_word_ver = print.printword_ver_TFIDF();
            list_high_word_ver = print.print_highverword();
            list_high_word_count = print.print_highwordcount();
            origintext = print.printorigintext();
            list_str = print.printstr();
            list_word = print.printwordsw(); //(printwordsw)-/for without stop words;(printword) - for with stop words

            int i = 0;
            foreach (string el in list_word)
            {
                if (el != "")
                    foreach (string el1 in list_word_ver)
                    {
                        string[] temp = el1.Split('-');
                        if (el == temp[1].Trim())
                        {
                            list_wv.Add(el + " - " + temp[0]);
                            break;
                        }
                        i++;
                        if (i == list_word_ver.Count)
                        {
                            list_wv.Add(el + " - " + 1);
                        }
                    }
                i = 0;
            }

            int g = 0;
            vec = new float[list_wv.Count];
            foreach (string el1 in list_wv)
            {
                string[] temparr = el1.Split('-');
                vec[g] = (float)Convert.ToDouble(temparr[1].Trim());
                g++;
            }


        }
    }
}
