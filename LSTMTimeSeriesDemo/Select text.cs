using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSTMTimeSeriesDemo
{
    public partial class Select_text : Form
    {
        public Select_text()
        {
            InitializeComponent();
        }

        string filePath = "";
        public static string filename = ReadFN();

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filePath = ofd.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                Rename(filePath, "textclasspaint.txt");
                filename = "textclasspaint.txt";
                WriteFN(filename);
            }
            if (radioButton2.Checked == true)
            {
                Rename(filePath, "textclassscience.txt");
                filename = "textclassscience.txt";
                WriteFN(filename);
            }
            if (radioButton3.Checked == true)
            {
                Rename(filePath, "textclasssport.txt");
                filename = "textclasssport.txt";
                WriteFN(filename);
            }

            this.Close();
        }

        private void Rename(string pathToFirstFile, string pathToSecondFile)
        {
            if (!File.Exists(pathToFirstFile))
                throw new FileNotFoundException();

            if (!File.Exists(pathToFirstFile))
                throw new FileNotFoundException();

            // var data = File.ReadAllBytes(pathToFirstFile);
            //File.WriteAllBytes(pathToSecondFile, data);

            string data = File.ReadAllText(pathToFirstFile, Encoding.Default);
            File.WriteAllText(pathToSecondFile, data, Encoding.Default);
        }

        public string replace_text_normalize(string data)
        {
            string text = "";
            string[] arr = data.Split('.', '?', '!');
            arr = arr.Where(n => !string.IsNullOrEmpty(n)).ToArray();
            for (int i = 0; i < arr.Length - 1; i++)
            {
                if (arr[i].Split(' ').Contains("рис") || arr[i].Contains("рис.") || arr[i].Contains("Рис.") || arr[i].Contains("Рис"))
                {
                    arr[i] = arr[i + 1] = "";
                    i++;
                }

                if (arr[i].Contains("формул"))
                {
                    arr[i] = "";
                }

                if (arr[i].Contains("разделе"))
                {
                    arr[i] = "";
                }

                if (arr[i].Contains("Рисунок"))
                {
                    arr[i] = "";
                }

                if (arr[i].Contains("ТАБЛИЦА"))
                {
                    arr[i] = "";
                    /* for deleting title of table
                    arr[i] = arr[i + 1] = "";
                    i++;
                    */
                }

                if (arr[i].Split(' ').Contains("табл") || arr[i].Contains("табл."))
                {
                    arr[i] = "";
                }


                if (arr[i].Trim() != "")
                {
                    string temp = delete_some_text(arr[i]);
                    string temp2 = "";
                   if ((arr[i].Trim().Length - temp.Trim().Length) > 2)
                       arr[i] = "";
                    if (arr[i] != " " || arr[i] != "")
                        temp2 = delete_some_text(arr[i]) ;
                    if(temp2 != "")
                        text+= delete_some_text1(temp2) + ".";
                }
            }
            return text;
        }

        

    private string delete_some_text(string str)
        {
            string ret_str = "";
            string pattern = @"\(([0-9]*)\)";
            if (str.Contains("где"))
                ret_str = Regex.Replace(str, pattern, String.Empty);
            else
                ret_str = str;

            return ret_str;
        }

        private string delete_some_text1(string str)
        {
            string pattern = @"\[([^\[\]]+)\]";
            return Regex.Replace(str, pattern, "");
        }

        private void WriteFN(string _filename)
        {
            string fn = "File_name.txt";
            if (!File.Exists(fn))
                throw new FileNotFoundException();
            
            File.WriteAllText(fn, _filename);
         }

        public static string ReadFN()
        {
            string fn = "File_name.txt";
            if (!File.Exists(fn))
                throw new FileNotFoundException();
            
            string data = File.ReadAllText(fn);
            return data.Trim();
        }

       
    }
}
