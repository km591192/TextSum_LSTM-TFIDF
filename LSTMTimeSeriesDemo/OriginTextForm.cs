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
    public partial class OriginTextForm : Form
    {
        public OriginTextForm()
        {
            InitializeComponent();
        }

        private string recievedstring = "";

        private void OriginTextForm_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = recievedstring;
        }

        public void sendData(string s)
        {
            recievedstring = s;
        }

    }
}
