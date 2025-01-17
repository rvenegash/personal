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
    public partial class frmAprobaciones : Form
    {
        public frmAprobaciones()
        {
            InitializeComponent();
        }

        private void frmAprobaciones_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            OERHelper.crearServicio();

            var atL = OERHelper.getAssetsType();
            cbTipoAssets.DisplayMember = "Name";
            cbTipoAssets.ValueMember = "Id";
            cbTipoAssets.DataSource = atL;

            var aeL = OERHelper.getAssetsRegistrationStatus();
            cboEstAssets.DisplayMember = "Name";
            cboEstAssets.ValueMember = "Id";
            cboEstAssets.DataSource = aeL;

            this.Cursor = Cursors.Default;
        }

        private void bBuscar_Click(object sender, EventArgs e)
        {
            bSubmit.Enabled = false;
            bAccept.Enabled = false;
            bRegister.Enabled = false;

            if (cbTipoAssets.SelectedItem != null && cboEstAssets.SelectedItem != null)
            {
                this.Cursor = Cursors.WaitCursor;

                var tipo = (AssetType)cbTipoAssets.SelectedItem;
                var regSt = (AssetRegistrationStatus)cboEstAssets.SelectedItem;

                var atL = OERHelper.searchAssets(tipo.Id, regSt.Id);
                lbAssets.DisplayMember = "NameFull";
                lbAssets.ValueMember = "Id";
                lbAssets.DataSource = atL;

                bSubmit.Enabled = (regSt.Id == 10);
                bAccept.Enabled = (regSt.Id == 51);
                bRegister.Enabled = (regSt.Id == 52);

                this.Cursor = Cursors.Default;
            }
        }

        private void bSubmit_Click(object sender, EventArgs e)
        {
            if (lbAssets.SelectedItems.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                progressBar1.Value = 0;
                progressBar1.Maximum = lbAssets.SelectedItems.Count;
                foreach (var item in lbAssets.SelectedItems)
                {
                    OERHelper.submitAsset(((Asset)item).Id);

                    progressBar1.Value++;
                }
                bSubmit.Enabled = false;
                this.Cursor = Cursors.Default;
            }
        }

        private void bAccept_Click(object sender, EventArgs e)
        {
            if (lbAssets.SelectedItems.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                progressBar1.Value = 0;
                progressBar1.Maximum = lbAssets.SelectedItems.Count;
                foreach (var item in lbAssets.SelectedItems)
                {
                    OERHelper.acceptAsset(((Asset)item).Id);

                    progressBar1.Value++;
                }
                bAccept.Enabled = false;
                this.Cursor = Cursors.Default;
            }
        }

        private void bRegister_Click(object sender, EventArgs e)
        {
            if (lbAssets.SelectedItems.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                progressBar1.Value = 0;
                progressBar1.Maximum = lbAssets.SelectedItems.Count;
                foreach (var item in lbAssets.SelectedItems)
                {
                    OERHelper.registerAsset(((Asset)item).Id);

                    progressBar1.Value++;
                }
                bRegister.Enabled = false;
                this.Cursor = Cursors.Default;
            }
        }

        private void bSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
