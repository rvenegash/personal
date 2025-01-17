using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
//using Microsoft.Office.Interop;

namespace OER_CargaDatos
{
    class subeProcesoseTOM
    {
        enum tipoAsset
        {
            Service = 154,
            Endpoint = 169,
            Application = 158,
            Interface = 187,
            Operation = 50004,
            Country = 50006,
            DataCenter = 50005,
            MessageQueue = 50007,
            BusinessEntity = 50009,
            Functionality = 50102,
            BusinessProcess = 193
        }
        enum tipoRelation
        {
            Implements = 119,
            Invoked = 50000,
            Contains = 108,
            ReferencedBy = 118
        }
        enum estadoRegistro
        {
            Unsubmitted = 10,
            Submitted = 51,
            Accepted = 52,
            Registered = 13
        }
        static OerProd.FlashlineRegistryService ws;
        static OerProd.AuthToken token;


        static void Main1(string[] args)
        {
            //Console.WriteLine("Creando servicio...");
            //ws = new OerProd.FlashlineRegistryService();

            //Console.WriteLine("Autenticando");
            //token = ws.authTokenCreate(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);

            ////bajar todos los assets en estado Unsubmitted
            //Console.WriteLine("Leyendo assets");

            //var arch = "c:\\trabajo\\FrameworxProcesses13.5_Jan14.xlsx";
            //var sw = new StreamWriter("c:\\trabajo\\FrameworxProcesses13.5_Jan14.log");

            //object misValue = System.Reflection.Missing.Value;

            //var xlApp = new Microsoft.Office.Interop.Excel.Application();
            //var xlWorkBook = xlApp.Workbooks.Open(arch, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            //var xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);

            ////recorrer archivo
            //var valor1 = xlWorkSheet.get_Range("A1", "A1").Value2.ToString();


            //for (int i = 2; i < 1450; i++)
            //{
            //    var sI = i.ToString();
            //    var nombre = xlWorkSheet.get_Range("A" + sI, "A" + sI).Value2.ToString();

            //    if (nombre.Equals(""))
            //        break;

            //    var proc_id = xlWorkSheet.get_Range("B" + sI, "B" + sI).Value2.ToString();
            //    var category = xlWorkSheet.get_Range("D" + sI, "D" + sI).Value2.ToString();
            //    var desc = xlWorkSheet.get_Range("F" + sI, "F" + sI).Value2.ToString();

            //    var nombre_final = string.Format("{0} (L{1})", nombre, category.Split(' ')[0].Replace("(", "").Replace(")", ""));

            //    var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            //    criteria.assetTypeCriteria = (int)tipoAsset.BusinessProcess;
            //    criteria.nameCriteria = nombre_final;
            //    var serviceQ = ws.assetQuerySummary(token, criteria);
            //    long asset_id = 0;
            //    foreach (var item in serviceQ)
            //    {
            //        asset_id = item.ID;
            //        break;
            //    }

            //    if (asset_id == 0)
            //    {
            //        Console.WriteLine("Creando {0}", nombre_final);
            //        var version = "13.5";
            //        var asset = ws.assetCreate(token, nombre_final, version, (int)tipoAsset.BusinessProcess);
            //        asset_id = asset.ID;

            //        sw.WriteLine("Creado {0} - {1}", asset.ID, nombre_final);
            //    }

            //    if (asset_id != 0)
            //    {
            //        var assetR = ws.assetRead(token, asset_id);
            //        if (assetR.description != desc)
            //        {
            //            assetR.description = desc;

            //            ws.assetUpdate(token, assetR);

            //            sw.WriteLine("Actualizando desc {0} - {1}", asset_id, nombre_final);
            //            Console.WriteLine("Actualizando desc {0} - {1}", asset_id, nombre_final);
            //        }
            //        else
            //        {
            //            Console.WriteLine("NO Actualiza desc {0} - {1}", asset_id, nombre_final);

            //        }
            //    }
            //}

            //sw.Close();

            //xlWorkBook.Close(true, misValue, misValue);
            //xlApp.Quit();

            //releaseObject(xlWorkSheet);
            //releaseObject(xlWorkBook);
            //releaseObject(xlApp);

            //fin
            Console.WriteLine("Terminado...");
            Console.ReadLine();
        }

        // cambia de estado todos los assets de eTOM
        static void Main2(string[] args)
        {
            Console.WriteLine("Creando servicio...");
            ws = new OerProd.FlashlineRegistryService();

            Console.WriteLine("Autenticando");
            token = ws.authTokenCreate(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);

            //bajar todos los assets BusinessProcess
            Console.WriteLine("Leyendo assets BusinessProcess ");

            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.assetTypeCriteria = (int)tipoAsset.BusinessProcess;
            criteria.versionCriteria = "13.5";
            var serviceQ = ws.assetQuerySummary(token, criteria);
            long asset_id = 0;
            foreach (var item in serviceQ)
            {
                var lAssetCriteria = new OER_CargaDatos.OerProd.AssetCriteria();
                lAssetCriteria.IDCriteria = item.ID;
                var lKeyValuePair = ws.assetEvaluate(token, lAssetCriteria, "Registration Status");
                if (lKeyValuePair.value.Equals("10")) //Unsubmitted
                {
                    Console.WriteLine("Submit : {0}, {1}", item.ID, item.name);
                    ws.assetSubmit(token, item.ID);
                }
                if (lKeyValuePair.value.Equals("51")) //Submitted
                {
                    Console.WriteLine("Accept : {0}, {1}", item.ID, item.name);
                    ws.assetAccept(token, item.ID);
                }
                if (lKeyValuePair.value.Equals("52")) //Accepted
                {
                    Console.WriteLine("Register : {0}, {1}", item.ID, item.name);
                    ws.assetRegister(token, item.ID);
                }
            }


            //fin
            Console.WriteLine("Terminado...");
            Console.ReadLine();
        }

        private static void releaseObject(object obj)
        {
            try
            {
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                //MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
