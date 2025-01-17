using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace OER_CargaDatos_v2
{
    class OERHelper
    {
        OerProd.FlashlineRegistryClient ws;
        OerProd.AuthToken token;
        public string ModoConexionLista { get; set; }
        public string ModoConexionAsset { get; set; }

        internal async void crearServicioAsync()
        {
            Console.WriteLine("ModoConexion : {0}", this.ModoConexionLista);
            if ((this.ModoConexionLista == "WS" || this.ModoConexionLista == "BD" || this.ModoConexionLista == "FL") && ws == null)
            {
                ws = new OerProd.FlashlineRegistryClient();
                ws.Endpoint.Binding.SendTimeout = new TimeSpan(1, 5, 0);
                ((System.ServiceModel.BasicHttpBinding)ws.Endpoint.Binding).Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                var xToken = ws.authTokenCreateAsync(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);
                xToken.Wait();
                token = xToken.Result;
                Console.WriteLine("Token : {0}", token.token);
            }
        }

        internal async System.Threading.Tasks.Task<OerProd.AssetSummary[]> leeAssetsAsync()
        {
            Console.WriteLine("En leeAssetsAsync, {0}", token);

            switch (this.ModoConexionLista)
            {
                case "WS":
                    if (ws != null)
                    {
                        var criteria = new OerProd.AssetCriteria();
                        criteria.browsableOnlyCriteria = "true";
                        return await ws.assetQuerySummaryAsync(token, criteria);
                    }
                    else
                    {
                        return null;
                    }
                    break;
                case "BD":

                    using (var conn = new OracleConnection(ConfigurationManager.AppSettings["OracleOER"]))
                    {
                        conn.Open();
                        long cant = 0;
                        using (var cmd = new OracleCommand("select count(*) from assets ", conn)) //where ACTIVESTATUS = 0
                        {
                            cmd.CommandType = System.Data.CommandType.Text;
                            cant = Convert.ToInt64(cmd.ExecuteScalar());
                        }
                        var result = new OerProd.AssetSummary[cant + 5];

                        using (var cmd = new OracleCommand("select * from assets", conn)) // where ACTIVESTATUS = 0
                        {
                            cmd.CommandType = System.Data.CommandType.Text;
                            int index = 0;
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    result[index] = new OerProd.AssetSummary()
                                    {
                                        ID = Convert.ToInt64(reader["id"]),
                                        name = Convert.ToString(reader["name"]),
                                        version = Convert.ToString(reader["version"]),
                                        typeID = Convert.ToInt64(reader["assetTypeId"])
                                    };
                                    index++;
                                }
                            }
                        }
                        return result;
                    }

                    break;
                case "FL":
                    long cant2 = 0;
                    var arch = ConfigurationManager.AppSettings["AssetsFL"];
                    var sr1 = new System.IO.StreamReader(arch);
                    while (sr1.Peek() >= 0)
                    {
                        sr1.ReadLine();
                        cant2++;
                    }
                    sr1.Close();
                    var sr = new System.IO.StreamReader(arch);
                    var result1 = new OerProd.AssetSummary[cant2 + 5];
                    var index1 = 0;
                    while (sr.Peek() >= 0)
                    {
                        Console.WriteLine("index1, {0}", index1);
                        var linea = sr.ReadLine().Split('#');
                        if (index1 > 0 && linea[0] != "")
                        {
                            result1[index1] = new OerProd.AssetSummary()
                            {
                                ID = Convert.ToInt64(linea[0]),
                                name = linea[1],
                                version = linea[2],
                                typeID = Convert.ToInt64(linea[3])
                            };
                        }
                        index1++;
                    }

                    return result1;
                    break;
                default:
                    return null;
                    break;
            }
        }

        internal async System.Threading.Tasks.Task<string> leerAssetXML(long id)
        {
            if (ws != null)
            {
                //Console.WriteLine("En leerAssetXML, {0}", token.token);
                return await ws.assetReadXmlAsync(token, id);
            }
            else
            {
                using (var conn = new OracleConnection(ConfigurationManager.AppSettings["OracleOER"]))
                {
                    conn.Open();

                    using (var cmd = new OracleCommand("select xml from ASSETXML where ASSETID = " + id.ToString(), conn))
                    {
                        cmd.CommandType = System.Data.CommandType.Text;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var b = reader.GetOracleBlob(0);
                                var sr = new System.IO.StreamReader(b);
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }

                return null;
            }
        }
    }
}
