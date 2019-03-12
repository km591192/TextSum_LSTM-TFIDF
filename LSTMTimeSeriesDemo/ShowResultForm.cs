using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSTMTimeSeriesDemo
{
    public partial class ShowResultForm : Form
    {
        public ShowResultForm()
        {
            InitializeComponent();
        }

        private string recievedstring = "";
        string[,] arrdata;

        private void ShowResultForm_Load(object sender, EventArgs e)
        {
            richTextBox3.Text = recievedstring;
            richTextBox1.Text = "";

            for (int i = 0; i < arrdata.Length/2; i++)
            {
               richTextBox1.Text += arrdata[i, 0] + "\n" + arrdata[i, 1] + "\n\n";               
            }
        }

        public void sendData(string s)
        {
            recievedstring = s;
        }

        public void sendData_arr(string [,] s)
        {
            arrdata = s;
        }

       
    }
}
