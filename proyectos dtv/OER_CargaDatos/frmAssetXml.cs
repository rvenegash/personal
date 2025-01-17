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
    public partial class frmAssetXml : Form
    {
        public frmAssetXml()
        {
            InitializeComponent();
        }

        private void bSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bSubmit_Click(object sender, EventArgs e)
        {
            long assetId = 0;

            if (long.TryParse(tbAssetId.Text, out assetId))
            {
                this.Cursor = Cursors.WaitCursor;

                richTextBox1.Text = OERHelper.LeeAssetFullXML(assetId);

                HighlightRTF(richTextBox1);

                this.Cursor = Cursors.Default;
            }
        }

        private void frmAssetXml_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            OERHelper.crearServicio();

            this.Cursor = Cursors.Default;
        }

        public static void HighlightRTF(RichTextBox rtb)
        {
            int k = 0;

            string str = rtb.Text;

            int st, en;
            int lasten = -1;
            while (k < str.Length)
            {
                st = str.IndexOf('<', k);

                if (st < 0)
                    break;

                if (lasten > 0)
                {
                    rtb.Select(lasten + 1, st - lasten - 1);
                    rtb.SelectionColor = HighlightColors.HC_INNERTEXT;
                }

                en = str.IndexOf('>', st + 1);
                if (en < 0)
                    break;

                k = en + 1;
                lasten = en;

                if (str[st + 1] == '!')
                {
                    rtb.Select(st + 1, en - st - 1);
                    rtb.SelectionColor = HighlightColors.HC_COMMENT;
                    continue;

                }
                String nodeText = str.Substring(st + 1, en - st - 1);


                bool inString = false;

                int lastSt = -1;
                int state = 0;
                /* 0 = before node name
                 * 1 = in node name
                   2 = after node name
                   3 = in attribute
                   4 = in string
                   */
                int startNodeName = 0, startAtt = 0;
                for (int i = 0; i < nodeText.Length; ++i)
                {
                    if (nodeText[i] == '"')
                        inString = !inString;

                    if (inString && nodeText[i] == '"')
                        lastSt = i;
                    else
                        if (nodeText[i] == '"')
                        {
                            rtb.Select(lastSt + st + 2, i - lastSt - 1);
                            rtb.SelectionColor = HighlightColors.HC_STRING;
                        }

                    switch (state)
                    {
                        case 0:
                            if (!Char.IsWhiteSpace(nodeText, i))
                            {
                                startNodeName = i;
                                state = 1;
                            }
                            break;
                        case 1:
                            if (Char.IsWhiteSpace(nodeText, i))
                            {
                                rtb.Select(startNodeName + st, i - startNodeName + 1);
                                rtb.SelectionColor = HighlightColors.HC_NODE;
                                state = 2;
                            }
                            break;
                        case 2:
                            if (!Char.IsWhiteSpace(nodeText, i))
                            {
                                startAtt = i;
                                state = 3;
                            }
                            break;

                        case 3:
                            if (Char.IsWhiteSpace(nodeText, i) || nodeText[i] == '=')
                            {
                                rtb.Select(startAtt + st, i - startAtt + 1);
                                rtb.SelectionColor = HighlightColors.HC_ATTRIBUTE;
                                state = 4;
                            }
                            break;
                        case 4:
                            if (nodeText[i] == '"' && !inString)
                                state = 2;
                            break;
                    }

                }
                if (state == 1)
                {
                    rtb.Select(st + 1, nodeText.Length);
                    rtb.SelectionColor = HighlightColors.HC_NODE;
                }
            }
        }

        private void bFontMas_Click(object sender, EventArgs e)
        {
            var f = new Font("Verdana", richTextBox1.Font.Size + 1);
            richTextBox1.Font = f;

            HighlightRTF(richTextBox1);
        }

        private void btnFontMenos_Click(object sender, EventArgs e)
        {
            var f = new Font("Verdana", richTextBox1.Font.Size - 1);
            richTextBox1.Font = f;

            HighlightRTF(richTextBox1);
        }

        private void tbAssetId_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }


    public class HighlightColors
    {
        public static Color HC_NODE = Color.Firebrick;
        public static Color HC_STRING = Color.Blue;
        public static Color HC_ATTRIBUTE = Color.Red;
        public static Color HC_COMMENT = Color.GreenYellow;
        public static Color HC_INNERTEXT = Color.Black;
    }
}
