using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Xml;

namespace winUtils
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void bBase_Click(object sender, EventArgs e)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(tbBase.Text);
            tbDebase.Text = System.Convert.ToBase64String(plainTextBytes);
        }

        private void tDebase_Click(object sender, EventArgs e)
        {
            byte[] plainTextBytes = System.Convert.FromBase64String(tbDebase.Text);
            tbBase.Text = UTF8Encoding.UTF8.GetString(plainTextBytes);
        }

        private void bFormatJson_Click(object sender, EventArgs e)
        {

            try
            {
            using var jDoc = JsonDocument.Parse(tbJson.Text);
            tbJson.Text = JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });

            }
            catch (Exception)
            {

            }
        }
    }
}
