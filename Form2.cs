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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net;
using RestSharp;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Dynamic;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;
using DellWarrantyCheck;

namespace gettoken
{
    public partial class Form2 : Form
    {
        private string token;
        DateTime tiempoVen;
        DateTime tiempoCom;
        bool tNulo = false;
        IRestResponse response;
        IRestResponse response2;
        bool options = false;
        public Form2()
        {
            InitializeComponent();
        }

        public class JSON
        {
            public long? Id { get; set; }
            public string ServiceTag { get; set; }
            public long? OrderBuid { get; set; }
            public DateTimeOffset? ShipDate { get; set; }
            public string ProductCode { get; set; }
            public long? LocalChannel { get; set; }
            public string ProductId { get; set; }
            public string ProductLineDescription { get; set; }
            public string ProductFamily { get; set; }
            public string SystemDescription { get; set; }
            public string ProductLobDescription { get; set; }
            public string CountryCode { get; set; }
            public bool? Duplicated { get; set; }
            public bool? Invalid { get; set; }
            public Entitlement[] Entitlements { get; set; }
        }

        public class Entitlement
        {
            public string ItemNumber { get; set; }
            public DateTimeOffset? StartDate { get; set; }
            public DateTimeOffset? EndDate { get; set; }
            public string EntitlementType { get; set; }
            public string ServiceLevelCode { get; set; }
            public string ServiceLevelDescription { get; set; }
            public long? ServiceLevelGroup { get; set; }
        }
        
       
        private void button1_Click(object sender, EventArgs e)
        {

            String Consulta = "";
            var client = new RestClient("https://apigtwb2c.us.dell.com/PROD/sbil/eapi/v5/asset-entitlements");
            //var client = new RestClient("https://apigtwb2c.us.dell.com/PROD/sbil/eapi/v5/asset-entitlement-components");
            var request = new RestRequest(Method.GET);

            pictureBox1.Image = DellWarrantyCheck.Properties.Resources.sem_ama;
            label1.Text = "Realizando PETICION al servidor";

            if (DG1.RowCount > 0)

            {

                request.AddHeader("cache-control", "no-cache");
                //request.AddHeader("Accept", "application/xml; charset=UTF-8");         
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                //request.AddHeader("Content-Type", "application/xml; charset=UTF-8");
                request.AddHeader("Authorization", "Bearer " + token);


                for (int b = 0; b < DG1.RowCount; b++)
                {

                    if (Consulta == "")
                    {
                        Consulta = DG1.Rows[b].Cells[0].Value.ToString();

                    }
                    else
                    {
                        Consulta = Consulta + "," + DG1.Rows[b].Cells[0].Value.ToString();
                    }
                }
                request.AddParameter("servicetags", Consulta);
                response2 = client.Execute(request);

                options = false;


                dynamic json = JsonConvert.DeserializeObject(response2.Content);
                System.IO.File.WriteAllText(Application.StartupPath + "/datos.json", json.ToString(), Encoding.UTF8);

                int numRows = DG1.RowCount;
                DG1.Rows.Clear();


                String levelWarranry = "";
                //String countrySelect = "";
                int d = 0;

                for (int a = 0; a < numRows; a++)

                {
                    //PARA ANALIZAR LA FECHA Y NIVELES DE GARANTIA
                    //PONER AQUI CONTROL CUANDO EL TAG ES INCORRECTO ENTITLEMENT ARRAY VIENE VACIO

                     if (json[a].productLineDescription != null)
                    {
                        if (json[a].entitlements[0].endDate != null)
                        {
                            tiempoVen = json[a].entitlements[0].endDate;
                        }
                        else 
                        { 
                            tNulo = true;                    
                        }
                      
                        foreach (var item in json[a].entitlements)
                        {
                            if (json[a].entitlements[d].serviceLevelCode == "KK") { levelWarranry = levelWarranry + "KYHD / "; }
                            if (json[a].entitlements[d].serviceLevelCode == "CC") { levelWarranry = levelWarranry + "COMPLETE CARE / "; }
                            if (json[a].entitlements[d].serviceLevelCode == "TS") { levelWarranry = levelWarranry + "PROSUPPORT / "; }
                            if (json[a].entitlements[d].serviceLevelCode == "PZ") { levelWarranry = levelWarranry + "PREMIUM SUPPPORT / "; }
                            if (json[a].entitlements[d].serviceLevelCode == "PY") { levelWarranry = levelWarranry + "PROSUPPORT PLUS / "; }
                            if (json[a].entitlements[d].serviceLevelCode == "XB") { levelWarranry = levelWarranry + "EXTENDED BATTERY / "; }
                            if (json[a].entitlements[d].serviceLevelCode == "ND") { levelWarranry = levelWarranry + "NBD / "; }
                            if (json[a].entitlements[d].serviceLevelCode == "NBD") { levelWarranry = levelWarranry + "NBD / "; }
                            if (json[a].entitlements[d].serviceLevelCode == "PHONESUPP") { levelWarranry = levelWarranry + "LIMITED PHONE SUPP / "; }


                            if (tNulo == false) { tiempoCom = json[a].entitlements[d].endDate; }
                            if (tiempoVen > tiempoCom)
                            {


                                d++;
                                //listBox1.Items.Add(json.entitlements[i].serviceLevelDescription);

                            }
                            else
                            {

                                if (!tNulo) { tiempoVen = json[a].entitlements[d].endDate; }
                                d++;
                            }               

                        }
                        
                        
                        if (!tNulo)
                        {
                            DG1.Rows.Insert(a, json[a].serviceTag, json[a].productLineDescription, tiempoVen.ToShortDateString(), paisCodigo(json[a].countryCode), levelWarranry);
                            levelWarranry = "";

                        }
                        else
                        {
                            DG1.Rows.Insert(a, json[a].serviceTag, json[a].productLineDescription, "NO FECHA", paisCodigo(json[a].countryCode), levelWarranry);
                            levelWarranry = "";

                        }

                        if (tiempoVen < DateTime.Now)
                        {
                            DG1.Rows[a].DefaultCellStyle.BackColor = Color.FromArgb(235, 160, 157);
                        }
                        else
                        {
                            DG1.Rows[a].DefaultCellStyle.BackColor = Color.LightGreen;
                        }
                        if (tNulo == true)
                        {

                            DG1.Rows[a].DefaultCellStyle.BackColor = Color.LightYellow;

                        }


                        d = 0;
                        tNulo = false;

                    }
                    else 
                    {
                        DG1.Rows.Insert(a, json[a].serviceTag, "NULO", "NULO", "NULO", "NULO");
                        levelWarranry = "";
                        d = 0;

                    }

                    //tiempoFila = DateTime.Parse(DG1.Rows[a].Cells[3].ToString());
                
                   // if (tiempoVen < DateTime.Now)
                    //{
                      //  DG1.Rows[a].DefaultCellStyle.BackColor = Color.FromArgb(235, 160, 157);
                        
                    //}
                    //else
                    //{
                    //  DG1.Rows[a].DefaultCellStyle.BackColor = Color.LightGreen;
                    //}
                    //if (tNulo == true)
                    //{

                      // DG1.Rows[a].DefaultCellStyle.BackColor = Color.LightYellow;

                    //}

                }
            }
        }



        public void pegar_portapapeles(DataGridView datagrid)
        {
            DataObject o = (DataObject)Clipboard.GetDataObject();
            if (o.GetDataPresent(DataFormats.Text))
            {
                if (datagrid.RowCount > 0)
                    datagrid.Rows.Clear();

                //if (datagrid.ColumnCount > 0)
                //datagrid.Columns.Clear();

                //bool columnsAdded = false;
                int myRowIndex = datagrid.Rows.Count;
                string[] pastedRows = Regex.Split(o.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray()), "\r\n");
                foreach (string pastedRow in pastedRows)
                {
                    string[] pastedRowCells = pastedRow.Split(new char[] { '\t' });

                    datagrid.Rows.Add();


                    for (int i = 0; i < pastedRowCells.Length; i++)
                    {
                        datagrid.Rows[myRowIndex].Cells[0].Value = pastedRowCells[0];
                        myRowIndex++;
                    }

                }

            }
        }

        private void DG1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                pegar_portapapeles(DG1);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength == 7) 
            {
                DG1.Rows.Add();
                DG1.Rows[DG1.Rows.Count - 1].Cells[0].Value = textBox1.Text;
                textBox1.Text = "";
                textBox1.Focus();

            }
            else if (textBox1.TextLength < 7)
            {
                MessageBox.Show("SERVICE TAG INCORRECTO", "Dell Warranty Check", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (textBox1.TextLength == 0)  
            { 
                MessageBox.Show("Introducir el SERVICE TAG", "Dell Warranty Check", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {


            pegar_portapapeles(DG1);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            DG1.Rows.Clear();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dynamic resp3;
            string path_2 = Application.StartupPath + "/key.json";
            using (StreamReader jsonStream = File.OpenText(path_2))
            {
                var json2 = jsonStream.ReadToEnd();
                resp3 = JObject.Parse(json2);
            }
            String id = resp3.id;
            String secret = resp3.secret;
            DateTime Tiempo = DateTime.Now;
            dynamic resp2;


            string path = Application.StartupPath + "/token.json";
            using (StreamReader jsonStream = File.OpenText(path))
            {
                var json = jsonStream.ReadToEnd();
                resp2 = JObject.Parse(json);
            }



            if ((resp2.expires_in == null))

            {
                var client = new RestClient("https://apigtwb2c.us.dell.com/auth/oauth/v2/token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials&scope=all&client_id=" + id + "&client_secret=" + secret, ParameterType.RequestBody);
                response = client.Execute(request);

                
                dynamic resp = JObject.Parse(response.Content);
                token = resp.access_token;
                DateTime SaveTiempo = DateTime.Now;
                resp.expires_in = SaveTiempo;
                string json1 = JsonConvert.SerializeObject(resp);
                System.IO.File.WriteAllText(Application.StartupPath + "/token.json", json1, Encoding.UTF8);
                options = true;
                timer1.Start();

            }

            else

            {
                DateTime SaveTiempo1 = resp2.expires_in;
                DateTime HoraActual = DateTime.Now;
                TimeSpan DiferenciaSeg = (HoraActual - SaveTiempo1);

                if (DiferenciaSeg >= new TimeSpan(0, 59, 0))
                {

                    var client = new RestClient("https://apigtwb2c.us.dell.com/auth/oauth/v2/token");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials&scope=all&client_id=" + id + "&client_secret=" + secret, ParameterType.RequestBody);
                    response = client.Execute(request);


                    dynamic resp = JObject.Parse(response.Content);
                    token = resp.access_token;
                    DateTime SaveTiempo = DateTime.Now;
                    resp.expires_in = SaveTiempo;
                    string json1 = JsonConvert.SerializeObject(resp);
                    System.IO.File.WriteAllText(Application.StartupPath + "/token.json", json1, Encoding.UTF8);
                    label1.Text = response.ResponseStatus.ToString();
                    options = true;
                    timer1.Start();

                }
                else
                {
                    token = resp2.access_token;
                    pictureBox1.Image = DellWarrantyCheck.Properties.Resources.sem_verde;
                    label1.Text = "Autenticacion exitosa AL SERVIDOR DE DELL";
                    label1.ForeColor = Color.Green;
                }
         
            }

            
        }


        static string paisCodigo(dynamic codigoCountrys)
        {

            if (codigoCountrys != null)
            {
                string codigoCountry = codigoCountrys.ToString();

                RegionInfo myReg1 = new RegionInfo(codigoCountry);

                return myReg1.DisplayName;

            }
            else { return "NULO"; }
        }

        private void button2_Click(object sender, EventArgs e)
        {


            
            if (textBox2.TextLength >0)
            {
                //int myRowIndex = datagrid.Rows.Count;
                string[] divisionLetras = Regex.Split(textBox2.Text.ToString(), "\n");
                foreach (string divisionLetra in divisionLetras)
                {
                    string[] pastedRowCells = divisionLetra.Split(",");

                    


                    for (int i = 0; i < pastedRowCells.Length; i++)
                    {
                        DG1.Rows.Insert(i, pastedRowCells[i]);
                        
                        
                    }

                }

            }



        }

        private void semaforo(bool opcion)
        {


            if (opcion)
            {
                if (response.ResponseStatus == ResponseStatus.Completed)
                {

                    pictureBox1.Image = DellWarrantyCheck.Properties.Resources.sem_verde;
                    label1.Text = "Peticion exitosa";
                    label1.ForeColor = Color.Green;
                }
                else if (response.ResponseStatus == ResponseStatus.Error)
                {

                    label1.Text = "Problema de INTERNET/AUTENTICACION/ERROR";
                    pictureBox1.Image = DellWarrantyCheck.Properties.Resources.sem_rojo;


                }
            }
            else
            {
                if (response2.ResponseStatus == ResponseStatus.Completed)
                {

                    pictureBox1.Image = DellWarrantyCheck.Properties.Resources.sem_verde;
                    label1.Text = "Peticion exitosa";
                    label1.ForeColor = Color.Green;
                }
                else if (response2.ResponseStatus == ResponseStatus.Error)
                {

                    label1.Text = "Problema de INTERNET/AUTENTICACION/ERROR";
                    pictureBox1.Image = DellWarrantyCheck.Properties.Resources.sem_rojo;


                }


            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            semaforo(options);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form3 fmr = new Form3();
            fmr.ShowDialog();
        }
    }
    

}
