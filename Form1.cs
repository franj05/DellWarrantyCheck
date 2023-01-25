using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Net;
using System.Globalization;

namespace gettoken
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = paisCodigo("DO");


        }

        static string paisCodigo(string codigoCountrys)
        {
            RegionInfo myReg1 = new RegionInfo(codigoCountrys);

            return myReg1.DisplayName;


        }
    }
}
