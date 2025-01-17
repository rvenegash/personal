using OER_CargaDatos.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace OER_CargaDatos
{
    public partial class frmActRealName : Form
    {
        const string PORT_TYPE = "port-type";
        const string REAL_NAME = "real-name";

        public frmActRealName()
        {
            InitializeComponent();
        }

        private void frmActRealName_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            OERHelper.crearServicio();

            var atS = OERHelper.getServicios();
            cbServicios.DisplayMember = "Name";
            cbServicios.ValueMember = "Id";
            cbServicios.DataSource = atS;

            lbAssets.DisplayMember = "Name";
            lbAssets.ValueMember = "Id";

            this.Cursor = Cursors.Default;
        }

        private void cbServicios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbServicios.SelectedItem != null)
            {
                lbPortType.Text = "";

                var customData = "";
                var servicioId = (Asset)cbServicios.SelectedItem;
                var assetL = OERHelper.getAssetsRelated(servicioId.Id, 108, out customData); // Contains =
                if (customData != "")
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(customData);

                    XmlNodeList bdCD = doc.GetElementsByTagName("custom-data");
                    foreach (XmlNode nodeP in bdCD)
                    {
                        XmlElement subsElement = (XmlElement)nodeP;
                        XmlNodeList stepsSE = subsElement.ChildNodes;

                        foreach (XmlNode nodeStep in stepsSE)
                        {
                            if (nodeStep.Name == PORT_TYPE)
                            {
                                lbPortType.Text = nodeStep.InnerText;
                                break;
                            }
                        }
                    }

                }
                lbAssets.Items.Clear();
                foreach (var item in assetL)
                {
                    lbAssets.Items.Add(item);
                }
            }

            progressBar1.Value = 0;
        }

        private void bSubmit_Click(object sender, EventArgs e)
        {
            if (cbServicios.SelectedItem != null)
            {
                this.Cursor = Cursors.WaitCursor;
                var servicioId = (Asset)cbServicios.SelectedItem;

                if (lbAssets.Items.Count > 0 && lbPortType.Text != "")
                {
                    progressBar1.Value = 0;
                    progressBar1.Maximum = lbAssets.Items.Count;
                    foreach (var item in lbAssets.Items)
                    {
                        var asset = (Asset)item;
                        var cd = string.Format("{0}.{1}", lbPortType.Text, asset.Name);

                        OERHelper.updateCustomData(asset.Id, REAL_NAME, cd);

                        progressBar1.Value++;
                    }

                }
                this.Cursor = Cursors.Default;
            }
        }

        private void bSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
