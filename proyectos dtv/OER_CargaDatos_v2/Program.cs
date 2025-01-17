using MySql.Data.MySqlClient;
using OER_CargaDatos_v2.Clases;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Xml;

namespace OER_CargaDatos_v2
{
    class Program
    {
        enum tipoAsset
        {
            Component = 145,
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
            APIs = 50202
        }
        /* enum tipoRelation
         {
             Implements = 119,
             Invoked = 50000,
             Contains = 108,
             ReferencedBy = 118,
             Reemplaces = 50101,
             ExposedBy = 50004
         }
         */
        static OERHelper oer;

        static void Main(string[] args)
        {
            Console.WriteLine("Creando servicio...");

            oer = new OERHelper();
            oer.ModoConexionLista = ConfigurationManager.AppSettings["ModoConexionLista"];
            oer.ModoConexionAsset = ConfigurationManager.AppSettings["ModoConexionAsset"];
            oer.crearServicioAsync();

            grabaAssetsBD();

            Console.WriteLine("Fin...");
        }

        private static void grabaAssetsBD()
        {
            var mysqldb = ConfigurationManager.AppSettings["MySQL160"];

            using (var conn = new MySqlConnection(mysqldb))
            {
                conn.Open();

                using (var cmd = new MySqlCommand("set net_write_timeout=99999; set net_read_timeout=99999", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Borrando tabla asset_relation");
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from asset_relation";
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Borrando tabla asset");
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from asset";
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Borrando tabla asset_categorization");
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from asset_categorization";
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Leyendo assets");

                //oer.ModoConexion = "BD";
                System.Threading.Tasks.Task<OerProd.AssetSummary[]> serviceQ = oer.leeAssetsAsync();
                serviceQ.Wait();

                foreach (var item in serviceQ.Result)
                {
                    Console.Write(".");
                    //Console.WriteLine("Insertando asset {0}\t{1}", item.ID, item.name);
                    if (item != null)
                    {
                        using (var cmd = new MySqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "insert into asset (id, name , type, version) values (@id, @name, @type, @version)";
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.Add(new MySqlParameter("id", item.ID));
                            cmd.Parameters.Add(new MySqlParameter("name", item.name));
                            cmd.Parameters.Add(new MySqlParameter("type", item.typeID));
                            cmd.Parameters.Add(new MySqlParameter("version", item.version));

                            var i = cmd.ExecuteNonQuery();
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Leyendo assets para leer sus relaciones");

                //oer.ModoConexionLista = "WS";
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from asset";
                    //cmd.CommandText = "select * from asset where id = 54119";
                    cmd.CommandType = CommandType.Text;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.Write(".");

                            var id = Convert.ToInt64(reader["id"]);
                            var name = Convert.ToString(reader["name"]);
                            var type = Convert.ToInt32(reader["type"]);
                            System.Diagnostics.Debug.WriteLine(String.Format("Buscando asset {0}\t{1}", id, name));

                            var itemXML = oer.leerAssetXML(id);
                            itemXML.Wait();
                            var item = itemXML.Result;

                            //Console.WriteLine("Buscando custom-data del asset {0}\t{1}", id, name);
                            if (item != "" && item != null)
                            {
                                var doc = new XmlDocument();
                                doc.LoadXml(item);

                                var update_asset = false;
                                var update_realname = false;
                                List<PairKeyValue> valores = new List<PairKeyValue>();

                                XmlNodeList bdCD = doc.GetElementsByTagName("custom-data");
                                foreach (XmlNode nodeP in bdCD)
                                {
                                    XmlElement subsElement = (XmlElement)nodeP;
                                    XmlNodeList stepsSE = subsElement.ChildNodes;

                                    foreach (XmlNode nodeStep in stepsSE)
                                    {
                                        if (!update_realname && (nodeStep.Name == "real-name" || nodeStep.Name == "port-type") && nodeStep.InnerText != "")
                                        {
                                            update_asset = true;
                                            update_realname = true;
                                            valores.Add(new PairKeyValue(0, "real_name", nodeStep.InnerText));
                                        }
                                        if (!update_realname && (nodeStep.Name == "technical-name") && nodeStep.InnerText != "")
                                        {
                                            update_asset = true;
                                            update_realname = true;
                                            valores.Add(new PairKeyValue(0, "real_name", nodeStep.InnerText));
                                        }
                                        if (nodeStep.Name == "real-name2")
                                        {
                                            update_asset = true;
                                            valores.Add(new PairKeyValue(0, "real_name2", nodeStep.InnerText));
                                        }
                                        if (nodeStep.Name == "service-type")
                                        {
                                            update_asset = true;
                                            valores.Add(new PairKeyValue(0, "attibute1", nodeStep.InnerText));
                                        }
                                        if ((type == ((int)tipoAsset.Service) || type == ((int)tipoAsset.APIs)) && nodeStep.Name == "documentation")
                                        {
                                            XmlElement subsElementDoc = (XmlElement)nodeStep;

                                            XmlNodeList stepsDoc = subsElementDoc.ChildNodes;

                                            foreach (XmlNode nodeStepDoc in stepsDoc)
                                            {
                                                var docName = nodeStepDoc.SelectSingleNode("document-name").InnerText;
                                                var docUrl = nodeStepDoc.SelectSingleNode("document-url").InnerText;
                                                if ((docName == "Service Definition" || docName == "URL PROD" || docName == "URL QA") && docUrl != "")
                                                {
                                                    var column_name = (docName == "Service Definition") ? "url_documentation" : (docName == "URL PROD") ? "url_serv_prod" : (docName == "URL QA") ? "url_serv_qa" : "";

                                                    update_asset = true;
                                                    valores.Add(new PairKeyValue(0, column_name, docUrl));

                                                }
                                            }
                                        }
                                    }
                                }

                                if (update_asset && valores.Count() > 0)
                                {
                                    using (var conn2 = new MySqlConnection(mysqldb))
                                    {
                                        conn2.Open();
                                        using (var cmdR = new MySqlCommand())
                                        {
                                            var strUpd = "";
                                            foreach (var itemV in valores)
                                            {
                                                strUpd += string.Format(", {0} = @{0}", itemV.Name);
                                            }
                                            cmdR.Connection = conn2;
                                            cmdR.CommandText = "update asset set " + strUpd.Substring(2) + " where id = @id";
                                            cmdR.CommandType = CommandType.Text;

                                            cmdR.Parameters.Add(new MySqlParameter("id", id));
                                            foreach (var itemV in valores)
                                            {
                                                cmdR.Parameters.Add(new MySqlParameter(itemV.Name, itemV.Key));
                                            }

                                            var i = cmdR.ExecuteNonQuery();
                                        }
                                    }
                                }

                                XmlNodeList bdCat = doc.GetElementsByTagName("categorization-types");
                                foreach (XmlNode nodeP in bdCat)
                                {
                                    XmlElement subsElement = (XmlElement)nodeP;
                                    XmlNodeList stepsSE = subsElement.ChildNodes;

                                    foreach (XmlNode nodeStep in stepsSE)
                                    {
                                        if (nodeStep.Name == "AssetLifecycleStage")
                                        {
                                            XmlElement subsElement1 = (XmlElement)nodeStep;
                                            XmlNodeList stepsSE1 = subsElement1.ChildNodes;

                                            foreach (XmlNode nodeStep1 in stepsSE1)
                                            {
                                                if (nodeStep1.Name == "categorization")
                                                {
                                                    long assetRelId = long.Parse(nodeStep1.Attributes["id"].Value);

                                                    using (var conn2 = new MySqlConnection(mysqldb))
                                                    {
                                                        conn2.Open();
                                                        using (var cmdR = new MySqlCommand())
                                                        {
                                                            cmdR.Connection = conn2;
                                                            cmdR.CommandText = "insert into asset_categorization (asset_id, categorization_id) values (@id, @catid)";
                                                            cmdR.CommandType = CommandType.Text;

                                                            cmdR.Parameters.Add(new MySqlParameter("id", id));
                                                            cmdR.Parameters.Add(new MySqlParameter("catid", assetRelId));

                                                            var i = cmdR.ExecuteNonQuery();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                XmlNodeList bdRel = doc.GetElementsByTagName("relationships");
                                foreach (XmlNode nodeP in bdRel)
                                {
                                    XmlElement subsElement = (XmlElement)nodeP;
                                    XmlNodeList stepsRel = subsElement.ChildNodes;

                                    foreach (XmlNode nodeStepRel in stepsRel)
                                    {
                                        long relTypeId = long.Parse(nodeStepRel.Attributes["id"].Value);

                                        XmlNodeList stepsSEP = nodeStepRel.ChildNodes;
                                        foreach (XmlNode nodeP1 in stepsSEP)
                                        {
                                            XmlElement subsElement1 = (XmlElement)nodeP1;

                                            if (nodeP1.Name == "primary")
                                            {
                                                XmlNodeList stepsRelP = nodeP1.ChildNodes;
                                                foreach (XmlNode nodeRel in stepsRelP)
                                                {
                                                    long assetRelId = long.Parse(nodeRel.Attributes["id"].Value);

                                                    XmlElement subsElementRel = (XmlElement)nodeP1;

                                                    if (assetRelId > 0)
                                                    {
                                                        using (var conn2 = new MySqlConnection(mysqldb))
                                                        {
                                                            conn2.Open();
                                                            using (var cmdR = new MySqlCommand())
                                                            {
                                                                cmdR.Connection = conn2;
                                                                cmdR.CommandText = "insert into asset_relation (asset_id, relation_type, related_asset_id, related_ps) values (@id, @rel_type, @rel_ass_id, 'P')";
                                                                cmdR.CommandType = CommandType.Text;

                                                                cmdR.Parameters.Add(new MySqlParameter("id", id));
                                                                cmdR.Parameters.Add(new MySqlParameter("rel_type", relTypeId));
                                                                cmdR.Parameters.Add(new MySqlParameter("rel_ass_id", assetRelId));


                                                                try
                                                                {
                                                                    var i = cmdR.ExecuteNonQuery();
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    Console.WriteLine("Error : {0}", ex.Message);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (nodeP1.Name == "secondary")
                                            {
                                                XmlNodeList stepsRelP = nodeP1.ChildNodes;
                                                foreach (XmlNode nodeRel in stepsRelP)
                                                {
                                                    long assetRelId = long.Parse(nodeRel.Attributes["id"].Value);

                                                    XmlElement subsElementRel = (XmlElement)nodeP1;

                                                    if (assetRelId > 0)
                                                    {
                                                        using (var conn2 = new MySqlConnection(mysqldb))
                                                        {
                                                            conn2.Open();
                                                            using (var cmdR = new MySqlCommand())
                                                            {
                                                                cmdR.Connection = conn2;
                                                                cmdR.CommandText = "insert into asset_relation (asset_id, relation_type, related_asset_id, related_ps) values (@id, @rel_type, @rel_ass_id, 'S')";
                                                                cmdR.CommandType = CommandType.Text;

                                                                cmdR.Parameters.Add(new MySqlParameter("id", id));
                                                                cmdR.Parameters.Add(new MySqlParameter("rel_type", relTypeId));
                                                                cmdR.Parameters.Add(new MySqlParameter("rel_ass_id", assetRelId));

                                                                try
                                                                {
                                                                    var i = cmdR.ExecuteNonQuery();
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    Console.WriteLine("Error : {0}", ex.Message);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("Terminado !!");
            }
        }

    }
}
