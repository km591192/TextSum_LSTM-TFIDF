using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSTMTimeSeriesDemo
{
    public partial class RelativeUtility : Form
    {
        public RelativeUtility()
        {
            InitializeComponent();
        }

        private string [] recievedarr;
        string[] arrdata;
        string[] arrdata_rtb1;
        string[] arrdata_rtb3;

        private void RelativeUtility_Load(object sender, EventArgs e)
        {
            richTextBox3.Text = "";
            richTextBox1.Text = "";

            for (int i = 0; i < arrdata.Length; i++)
            {
                if(arrdata[i].Trim() != "")
                richTextBox1.Text += arrdata[i] + "\n -~- N -~-\n\n" ;
            }

            for (int i = 0; i < recievedarr.Length; i++)
            {
                richTextBox3.Text += recievedarr[i] + "\n -~- N -~-\n\n";
            }
        }

        public void sendData(string s)
        {
            recievedarr = s.Split('.');
        }

        public void sendData_arr(string[] s)
        {
            arrdata = s;
        }

        private string[] read_rtb_data(RichTextBox rtb)
        {
            string[] arr_data;
            arr_data = Regex.Split(rtb.Text.ToString(), @"-~-").ToArray();
            return arr_data;
        }

        private string measure(string[] arr)
        {
            string measure_str = "";
            int measure_int = 0;
            int count = 0;
            for (int i = 1; i < arr.Length; i+=2)
            {
                measure_int += Convert.ToInt32(arr[i]);
                count++;
            }
            measure_str = ((double) measure_int/count).ToString();
            return measure_str;
        }

        private string measure_ort(string[] arr, string[] arrot)
        {
            string measure_str = "";
            int measure_int = 0;
            int count = 0;
            for (int i = 0; i < arrot.Length; i += 2)
            {
                for (int j = 0; j < arr.Length; j += 2)
                    if (arr[j].Trim() == arrot[i].Trim())
                     {
                        if (arr[j].Trim() != String.Empty && arrot[i].Trim() != String.Empty)
                        {
                            measure_int += Convert.ToInt32(arrot[i + 1]);
                            count++;
                        }
                    }                
            }
            measure_str = ((double)measure_int / count).ToString();
            return measure_str;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            arrdata_rtb1 = read_rtb_data(richTextBox1);
            textBox1.Text = "Answer \n" + measure(arrdata_rtb1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            arrdata_rtb1 = read_rtb_data(richTextBox1);
            arrdata_rtb3 = read_rtb_data(richTextBox3);
            textBox1.Text = "Answer \n" + measure_ort(arrdata_rtb1, arrdata_rtb3);
        }
    }
}
