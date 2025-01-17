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
    public partial class frmAsociaAAsset : Form
    {
        public frmAsociaAAsset()
        {
            InitializeComponent();
        }

        private void bSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bBuscar_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            var sr = tbNombre.Text.Split('\n', '\r').Distinct();
            foreach (var item in sr)
            {
                if (item != "")
                {
                    var itemL = new AssetType() { Id = 0, Name = item };
                    long assetId = 0;

                    if (long.TryParse(item, out assetId))
                    {
                        var asst = assetName(assetId);
                        itemL.Name = asst;
                        itemL.Id = assetId;
                    }

                    lbAssets.Items.Add(itemL);
                }
            }
            this.Cursor = Cursors.Default;
            tbNombre.Text = "";
        }

        private void bLimpiarAssets_Click(object sender, EventArgs e)
        {
            lbAssets.Items.Clear();

            bSubmit.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cbRelacion.SelectedItem != null)
            {
                if (lbAssets.SelectedItems.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    long assetId = 0;

                    if (long.TryParse(tbAssetId.Text, out assetId))
                    {
                        var tipo = (AssetType)cbRelacion.SelectedItem;

                        var lSel = lbAssets.SelectedItems.OfType<AssetType>().Select(m => m.Id).ToArray();

                        OERHelper.creaAssetsRelated(assetId, tipo.Id, lSel);
                    }
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void bSubmit_Click(object sender, EventArgs e)
        {
            long assetId = 0;

            if (long.TryParse(tbAssetId.Text, out assetId))
            {
                this.Cursor = Cursors.WaitCursor;

                tbAssetName.Text = assetName(assetId);

                this.Cursor = Cursors.Default;
            }
        }

        private void frmAsociaAAsset_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            OERHelper.crearServicio();

            var rtL = OERHelper.getRelationTypes();
            cbRelacion.DisplayMember = "Name";
            cbRelacion.ValueMember = "Id";
            cbRelacion.DataSource = rtL;

            lbAssets.DisplayMember = "Name";
            lbAssets.ValueMember = "Id";

            this.Cursor = Cursors.Default;
        }
        private string assetName(long id)
        {
            var asst = OERHelper.LeeAssetName(id);
            var st = (asst == null) ? "(not found)" : string.Format("{0} ({1})", asst.name, asst.version);

            return st;
        }
    }
}
