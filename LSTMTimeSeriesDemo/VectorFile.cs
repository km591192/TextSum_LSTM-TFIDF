
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LSTMTimeSeriesDemo
{
    class VectorFile
    {
        public string[] arr_datatosum;
        public string datatosum;

        public float [] xdata()
        {
            string[] xdataarr;
            float[] xdataarrfloat;

            if (!File.Exists("model.txt"))
                throw new FileNotFoundException();
            string data = File.ReadAllText("model.txt", Encoding.Default);

            string fn = Select_text.ReadFN();
            if (!File.Exists(fn))
                throw new FileNotFoundException();
             datatosum = File.ReadAllText(fn, Encoding.Default);

            string[] arr_data = data.Split('\n');
            arr_datatosum = datatosum.Split('.','!','?');

            string[] arr_datatosum_word = datatosum.Split(' ');
            string[] arr_data_wordsvec = data.Split(' ');
            string[] arr_data_words = new string[arr_data_wordsvec.Length / 101+ 1];
            int j = 0;
            for (int i = 0; i < arr_data_wordsvec.Length; i+=101)
            {
                arr_data_words[j] = arr_data_wordsvec[i];
                j++;
            }

            j = 0;
            int k = 0, flag = 0, kk = 0;
            xdataarr = new string[arr_datatosum_word.Length * 100];

            for (int i = 0; i < arr_datatosum_word.Length; i++)
                for (int jj = 0; jj < arr_data_words.Length; jj++)
            {
                string[] arr_data_item = arr_data_words[jj].Split('\n');
                if (arr_datatosum_word[i] == arr_data_item[arr_data_item.Length - 1])
                {
                    for (int u = jj * 101 + 1; u < jj * 101 + 101; u++) //j
                    {
                            if (u < arr_data_wordsvec.Length)
                            {
                                if (arr_data_wordsvec[u].Trim() != "")
                                    xdataarr[k] = arr_data_wordsvec[u].Trim();
                                k++;
                                flag = 1;
                            }
                    }
                    j = 0;
                }
                j++;
                if (jj == arr_data_words.Length - 1) //j
                {
                    j = 0;
                        if (flag == 0)
                        {
                            kk = k;
                            for (int u = kk; u < kk + 100; u++)
                            {
                                xdataarr[u] = null;
                                k++;
                            }
                        }
                   flag = 0;
                }
            }

            xdataarrfloat = converttofloat(xdataarr);
            return xdataarrfloat;
        }

        private float[] converttofloat(string[] arr)
        {
            float[] floatarr = new float[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                if(arr[i] != null)
                    floatarr[i] = float.Parse(arr[i], CultureInfo.InvariantCulture);

                if (arr[i] == null)
                    floatarr[i] = 0;
            }            
            return floatarr;
        }
    }
}