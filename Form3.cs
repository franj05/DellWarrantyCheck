using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DellWarrantyCheck
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            try
            {
                VisitaVinculo("http://www.dell.com");
                linkLabel1.LinkVisited = true;
            }
            catch (Exception ex)
            {

                MessageBox.Show("No se puede abrir el link" + " - " + ex.Message);

            }
            

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitaVinculo("https://www.dell.com/support/home/es-do?app=products");
                linkLabel2.LinkVisited = true;
            }
            catch (Exception ex )
            {

                MessageBox.Show("No se puede abrir el link" + " - " + ex.Message);

            }


        }
        
        private void VisitaVinculo(string visitlen)
        {

            System.Diagnostics.Process.Start(visitlen);

        }
    
    }

    
}
