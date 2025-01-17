using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OER_CargaDatos
{
    class principal
    {

        public static void Main(string[] args)
        {
            if (args.Count() == 0)
            {
                var frm = new frmMenu();
                frm.ShowDialog();
            }
            else
            {
                OER_CargaDatos.actualizaAssetsOER.MainActualizaOer(true);
                //OER_CargaDatos.actualizaDatosROI.MainRoi2(null);
            }
        }
    }
}
