using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace OER_CargaDatos
{
    class assetsFuncionality
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
            Functionality = 50102
        }
        enum tipoRelation
        {
            Implements = 119,
            Invoked = 50000,
            Contains = 108,
            ReferencedBy = 118
        }
        static OerProd.FlashlineRegistryService ws;
        static OerProd.AuthToken token;
        static List<Functionality> lFunctionalities;

        static void MainFunc(string[] args)
        {
            Console.WriteLine("Creando servicio...");
            ws = new OerProd.FlashlineRegistryService();

            Console.WriteLine("Autenticando");
            token = ws.authTokenCreate(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);

            Console.WriteLine("Leyendo functionality");
            creaListaFunctionalities();

            //asociaAssetFunctionalities();
        }

        private static void asociaAssetFunctionalities()
        {
            var path = ConfigurationManager.AppSettings["Path"] + "functionality\\";

            var sr = new StreamReader(path + "assets-functionalities.csv");
            var sw = new StreamWriter(path + "assets-functionalities.log");

            var lineas = 0;
            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split('\t');

                var id_asset = linea[0];
                var asset_name = linea[1];
                var func_name = linea[5];
                var id_func = linea[6];
                var id_func2 = linea[7];

                if (linea.Count() != 8)
                {
                    Console.WriteLine("La linea no tiene 8 elementos: {0}", linea[0]);
                    sw.WriteLine("La linea no tiene 8 elementos: {0}", linea[0]);
                }
                else
                {
                    int id_a = 0;
                    int id_f = 0;
                    if (!int.TryParse(id_asset, out id_a))
                    {
                        Console.WriteLine("id_asset: {0}, vacio", linea[1]);
                        sw.WriteLine("id_asset: {0}, vacio", linea[1]);
                    }
                    if (!int.TryParse(id_func, out id_f) && id_func2.Equals(""))
                    {
                        Console.WriteLine("id_func: {0}, vacio", linea[1]);
                        sw.WriteLine("id_func: {0}, vacio", linea[1]);
                    }
                    else
                    {
                        var list_id = new List<long>();
                        var id_funcs = id_func2.Split(',');

                        if (id_f != 0)
                        {
                            list_id.Add(id_f);
                        }
                        else
                        {
                            foreach (var item in id_funcs)
                            {
                                list_id.Add(Convert.ToInt64(item));
                            }
                        }

                        var update = false;
                        var asset = ws.assetRead(token, id_a);

                        foreach (var rel in asset.relationshipTypes)
                        {
                            if (rel.ID == (int)tipoRelation.Implements)
                            {
                                foreach (var id_funcion in list_id)
                                {
                                    if (!rel.secondaryIDs.Contains(id_funcion))
                                    {
                                        var ultimo = rel.secondaryIDs.Length;
                                        var paso = new long[rel.secondaryIDs.Length + 1];
                                        Array.Copy(rel.secondaryIDs, paso, rel.secondaryIDs.Length);

                                        paso[ultimo] = id_funcion;
                                        rel.secondaryIDs = paso;

                                        update = true;
                                    }
                                }
                            }
                        }
                        if (update)
                        {
                            var assU = ws.assetUpdate(token, asset);

                            Console.WriteLine("Id:{0} Name:{1} Func:{2} Func Id:{3} Id 2:{4}", id_asset, asset_name, func_name, id_func, id_func2);
                            sw.WriteLine("Id:{0} Name:{1} Func:{2} Func Id:{3} Id 2:{4}", id_asset, asset_name, func_name, id_func, id_func2);
                        }
                        else
                        {
                            Console.WriteLine("No pasó por update Id:{0} Name:{1} Func:{2} Func Id:{3} Id 2:{4}", id_asset, asset_name, func_name, id_func, id_func2);
                            sw.WriteLine("No pasó por update Id:{0} Name:{1} Func:{2} Func Id:{3} Id 2:{4}", id_asset, asset_name, func_name, id_func, id_func2);
                        }
                    }
                }
                lineas++;
            }

            sr.Close();
            sw.Close();
        }

        private static void creaListaFunctionalities()
        {
            lFunctionalities = new List<Functionality>();

            Console.WriteLine("FUNCTIONALITIES");

            var path = ConfigurationManager.AppSettings["Path"] + "functionality\\";
            var sw = new StreamWriter(path + "functionalities.csv");

            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.assetTypeCriteria = (int)tipoAsset.Functionality;
            criteria.browsableOnlyCriteria = "Y";
            var serviceQ = ws.assetQuerySummary(token, criteria);
            foreach (var item in serviceQ)
            {
                lFunctionalities.Add(new Functionality(item.ID, item.name));
                sw.WriteLine("{0}\t{1}", item.ID, item.name);
            }
            /*
            var nuevo_nombre = linea[0].Trim().Replace("�", "ó");

            if (!nuevo_nombre.Equals(""))
            {
                nuevo_nombre += " - Functionality";

                if (lFunctionalities.Where(m => m.Nombre == nuevo_nombre).Count() == 0)
                {
                    Console.WriteLine("Creando {0}", nuevo_nombre);
                    var version = "1.0";
                    var asset = ws.assetCreate(token, nuevo_nombre, version, (int)tipoAsset.Functionality);

                    lFunctionalities.Add(new Functionality(asset.ID, nuevo_nombre, asset));

                    sw.WriteLine("Creado {0} - {1}", asset.ID, nuevo_nombre);
                }
                else
                {
                    Console.WriteLine("Saltando {0}, ya existe", nuevo_nombre);
                    sw.WriteLine("Saltando {0}, ya existe", nuevo_nombre);
                }
            }*/
            sw.Close();
        }

        private static void crearFunctionalities()
        {
            var path = ConfigurationManager.AppSettings["Path"] + "functionality\\";

            var sr = new StreamReader(path + "func.txt");
            var sw = new StreamWriter(path + "func.log");

            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split('\t');

                var nuevo_nombre = linea[0].Trim().Replace("�", "ó");

                if (!nuevo_nombre.Equals(""))
                {
                    nuevo_nombre += " - Functionality";

                    if (lFunctionalities.Where(m => m.Nombre == nuevo_nombre).Count() == 0)
                    {
                        Console.WriteLine("Creando {0}", nuevo_nombre);
                        var version = "1.0";
                        var asset = ws.assetCreate(token, nuevo_nombre, version, (int)tipoAsset.Functionality);

                        lFunctionalities.Add(new Functionality(asset.ID, nuevo_nombre, asset));

                        sw.WriteLine("Creado {0} - {1}", asset.ID, nuevo_nombre);
                    }
                    else
                    {
                        Console.WriteLine("Saltando {0}, ya existe", nuevo_nombre);
                        sw.WriteLine("Saltando {0}, ya existe", nuevo_nombre);
                    }
                }
            }

            sr.Close();
            sw.Close();
        }

        class Functionality
        {
            public Functionality(long id, string nombre)
            {
                this.ID = id;
                this.Nombre = nombre;
            }
            public Functionality(long id, string nombre, OerProd.Asset asset)
            {
                this.ID = id;
                this.Nombre = nombre;
                this.Asset = asset;
            }
            public long ID { get; set; }
            public string Nombre { get; set; }
            public OerProd.Asset Asset { get; set; }
        }
    }
}
