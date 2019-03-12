using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTMTimeSeriesDemo
{
    class TF_IDF
    {
        public List<string> TF_IDF_ver(string fn, string[] fnarr, SortedDictionary<string, string> dict2, double count_all_world)
        {
            List<string> liststr = new List<string>();
            List<double> double_verdata = new List<double>();
            foreach (string k in dict2.Keys)
            {
                string tempstr = "";
                dict2.TryGetValue(k, out tempstr);
                string[] tempstrsplit = tempstr.Trim().Split('|');
                double kolvovhozhdslovavdok = Convert.ToDouble(tempstrsplit[0]);
                double kolvoslovvdoc = count_all_world;
                double kolvodok = fnarr.Length;
                double kolvodoksoslovom = Convert.ToDouble(tempstrsplit[1]);
                double TF = kolvovhozhdslovavdok / kolvoslovvdoc;
                double IDF = Math.Log(kolvodok / kolvodoksoslovom);
                double TF_IDF = TF * IDF;

                liststr.Add(TF_IDF.ToString() + " - " + k);
                double_verdata.Add(TF_IDF);
            }

            //УБРАТЬ НОРМАЛИЗАЦИЮ МОЖНО ТУТ. ЗАКОММЕНТИРОВАТЬ ГДЕ "//..."
            double_verdata = Normalize(double_verdata.ToArray()).ToList(); //...

            for (int i = 0; i < liststr.Count; i++) //...
            { //...
                string[] liststrarr = liststr[i].Split('-'); //...
                liststr[i] = double_verdata[i] + " - " + liststrarr[1]; //...
            } //...

            return liststr;
        }

        public List<string> TF_SLF_ver(string fn, string[] fnarr, SortedDictionary<string, string> dict2, double count_all_world)
        {
            List<string> liststr = new List<string>();
            List<double> double_verdata = new List<double>();

            foreach (string k in dict2.Keys)
            {
                string tempstr = "";
                dict2.TryGetValue(k, out tempstr);
                string[] tempstrsplit = tempstr.Trim().Split('|');
                double kolvovhozhdslovavdok = Convert.ToDouble(tempstrsplit[0]);
                double kolvoslovvdoc = count_all_world;
                double kolvocategorii = Realize_print.kolvo_classov;
                double kolvodokvclasse = fn.Length / Realize_print.kolvo_classov;

                double Rt = Convert.ToDouble(tempstrsplit[2]) / kolvodokvclasse + Convert.ToDouble(tempstrsplit[3]) / kolvodokvclasse + Convert.ToDouble(tempstrsplit[4]) / kolvodokvclasse;

                double TF = kolvovhozhdslovavdok / kolvoslovvdoc;
                double SLF = Math.Log(kolvocategorii / Rt);
                double TF_SLF = TF * SLF;

                liststr.Add(TF_SLF.ToString() + " - " + k);
                double_verdata.Add(TF_SLF);
            }

            //УБРАТЬ НОРМАЛИЗАЦИЮ МОЖНО ТУТ. ЗАКОММЕНТИРОВАТЬ ГДЕ "//..."
            double_verdata = Normalize(double_verdata.ToArray()).ToList(); //...

            for (int i = 0; i < liststr.Count; i++) //...
            { //...
                string[] liststrarr = liststr[i].Split('-'); //...
                liststr[i] = double_verdata[i] + " - " + liststrarr[1]; //...
            } //...

            return liststr;
        }


        // Normalizes a TF*IDF vector using L2-Norm.
        // Xi = Xi / Sqrt(X0^2 + X1^2 + .. + Xn^2)
        //http://www.primaryobjects.com/2013/09/13/tf-idf-in-c-net-for-machine-learning-term-frequency-inverse-document-frequency/

        private double[] Normalize(double[] vector)
        {
            List<double> result = new List<double>();

            double sumSquared = 0;
            foreach (var value in vector)
            {
                sumSquared += value * value;
            }

            double SqrtSumSquared = Math.Sqrt(sumSquared);

            foreach (var value in vector)
            {
                // L2-norm: Xi = Xi / Sqrt(X0^2 + X1^2 + .. + Xn^2)
                result.Add((value / SqrtSumSquared)); // without *10!!!!!!!!!!!!
            }

            return result.ToArray();
        }

    }
}
