using OER_CargaDatos.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OER_CargaDatos
{
    public partial class frmAsocia : Form
    {
        public frmAsocia()
        {
            InitializeComponent();
        }

        private void cbTipoAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbServicios.SelectedItem != null)
            {
                this.Cursor = Cursors.WaitCursor;

                var customData = "";
                var servicioId = (Asset)cbServicios.SelectedItem;
                var assetL = OERHelper.getAssetsRelated(servicioId.Id, 108, out customData); // Contains =
                lbAssets.Items.Clear();
                foreach (var item in assetL)
                {
                    lbAssets.Items.Add(item);
                }

                this.Cursor = Cursors.Default;
            }

            progressBar1.Value = 0;
        }
        
        private void frmAsocia_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            OERHelper.crearServicio();

            var atL = OERHelper.getAssetsType();
            cbTipoAssets.DisplayMember = "Name";
            cbTipoAssets.ValueMember = "Id";
            cbTipoAssets.DataSource = atL;

            var atS = OERHelper.getServicios();
            cbServicios.DisplayMember = "Name";
            cbServicios.ValueMember = "Id";
            cbServicios.DataSource = atS;

            lbAssets.DisplayMember = "Name";
            lbAssets.ValueMember = "Id";
            
            this.Cursor = Cursors.Default;
        }

        private void bSubmit_Click(object sender, EventArgs e)
        {
            if (lbAssets.SelectedItems.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                progressBar1.Value = 0;
                progressBar1.Maximum = lbAssets.Items.Count + 1;

                var servicioId = (cbAplicacion.SelectedItem) == null ? 0 : ((Asset)cbAplicacion.SelectedItem).Id;

                List<long> idRel = new List<long>();
                foreach (var item in lbAssets.SelectedItems)
                {
                    var rel = (Asset)item;
                    idRel.Add(rel.Id);
                    progressBar1.Value++;
                }
                if (idRel.Count() > 0)
                {
                    if (!OERHelper.creaAssetsRelated(servicioId, 50000, idRel.ToArray())) //50000 Invokes
                    {
                        MessageBox.Show("Error al crear relación");
                    }
                    progressBar1.Value++;
                }

               // bSubmit.Enabled = false;
                this.Cursor = Cursors.Default;
            }
        }

        private void bSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbTipoAssets_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            var tipo = (AssetType)cbTipoAssets.SelectedItem;

            var atL = OERHelper.searchAssets(tipo.Id, 0);
            cbAplicacion.DisplayMember = "Name";
            cbAplicacion.ValueMember = "Id";
            cbAplicacion.DataSource = atL;

            this.Cursor = Cursors.Default;
        }
    }
}
