using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;
using OER_CargaDatos.Helpers;
using System.Xml;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OER_CargaDatos.Clases;

namespace OER_CargaDatos
{
    partial class actualizaAssetsOER
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
        enum tipoRelation
        {
            Implements = 119,
            Invoked = 50000,
            Contains = 108,
            ReferencedBy = 118,
            Reemplaces = 50101,
            ExposedBy = 50004
        }
        static OerProd.FlashlineRegistryService ws;
        static OerProd.AuthToken token;
        static List<PairKeyValue> lApps;
        static List<PairKeyValue> lServices;
        static List<PairKeyValue> lOperations;

        public static void MainActualizaOer(bool salir) //
        {
            Console.WriteLine("Creando servicio...");
            ws = new OerProd.FlashlineRegistryService();

            Console.WriteLine("Autenticando");
            token = ws.authTokenCreate(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);
            //token = new OER_CargaDatos.OerProd.AuthToken();
            //token.token = "-74de3462-144e0be446a--72e0";

            OERHelper.crearServicio();

            //CargaDatosAPIs();

            grabaAssetsBD();

            //CargaDatosEntidades();

            //CargaInterfacesServApp();

            //creaApps();
            //creaServicios();

            //creaListaServiciosArch();
            //creaListaOperacionesArch();

            //creaListaApps();
            //creaListaServicios();
            //creaListaOperacionesServ();

            //CargaDatosOperaciones();

            //CargaDatosServApp();

            //CargaDatosOperaciones2();

            //CargaRelacionesEntreAssets2();

            //MarcaDeprecados();

            //CargaFechaUndepoy();

            Console.WriteLine("Terminado...");
            if (!salir)
            {
                Console.ReadLine();
            }
        }

        private static void MarcaDeprecados()
        {
            //bajar todos los assets en estado Unsubmitted
            Console.WriteLine("Leyendo assets");

            var sr = new StreamReader(ConfigurationManager.AppSettings["Path"] + "operaciones - deprecadas.csv");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "operaciones - deprecadas.log");

            long tipoCat = 112;

            //recorrer archivo
            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split(';');

                var sOperacion = linea[0].Trim();

                //validar linea
                long operacionId = 0;
                if (Int64.TryParse(sOperacion, out operacionId))
                {
                    //buscar asset
                    Console.WriteLine("Buscando {0}", operacionId);
                    try
                    {
                        var asset = ws.assetRead(token, operacionId);

                        if ((asset != null))
                        {
                            //buscar asset
                            Console.WriteLine("Agregando a {0}", asset.name);
                            if ((!asset.categorizations.Select(m => m.typeID).Contains(tipoCat)))
                            {
                                //Stage 2 - Activo
                                //Stage 3 - Deprecado
                                ws.assetAddCategorization(token, operacionId, "AssetLifecycleStage", "Stage 2 - Activo");
                                sw.WriteLine("Categoria creada {0}\t{1}", operacionId, asset.name);
                                Console.WriteLine("Categoria creada {0}\t{1}", operacionId, asset.name);
                            }
                            else
                            {
                                sw.WriteLine("Categoria ya existe {0}\t{1}", operacionId, asset.name);
                                Console.WriteLine("Categoria ya existe {0}\t{1}", operacionId, asset.name);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            sr.Close();
            sw.Close();
        }

        private static void CargaRelacionesEntreAssets()
        {
            var sr = new StreamReader(ConfigurationManager.AppSettings["Path"] + "esb_depreca_intarg.txt");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "esb_depreca_intarg.log");
            var swNC = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "esb_depreca_intarg-nocreadas.log");
            var swE = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "esb_depreca_intarg-errores.log");

            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split('\t');
                var id_invoca = linea[0].Trim();
                var id_invocado = linea[1].Trim();

                if (id_invoca != "")
                {
                    try
                    {
                        var lInvoca = Convert.ToInt32(id_invoca);
                        var lInvocado = Convert.ToInt32(id_invocado);
                        Console.WriteLine("Cargando {0}\t{1}", id_invoca, id_invocado);
                        var asset = ws.assetRead(token, lInvoca);

                        var asset2 = ws.assetRead(token, lInvocado);

                        if (asset != null && asset2 != null)
                        {

                            List<long> idRel = new List<long>();
                            idRel.Add(lInvocado);
                            if (OERHelper.creaAssetsRelated(lInvoca, (long)tipoRelation.Reemplaces, idRel.ToArray())) //50000 Invokes
                            {
                                sw.WriteLine("Creada relación {0}\t{1}", id_invoca, id_invocado);
                                Console.WriteLine("Creada relación {0}\t{1}", id_invoca, id_invocado);
                            }
                            else
                            {
                                swE.WriteLine("Error al crear relación {0}\t{1}", id_invoca, id_invocado);
                                Console.WriteLine("Error al crear relación {0}\t{1}", id_invoca, id_invocado);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        swE.WriteLine("{0}\t{1}\t{2}", id_invoca, id_invocado, ex.Message);
                        Console.WriteLine("ERROR {0}\t{1}\t{2}", id_invoca, id_invocado, ex.Message);
                    }
                }

            }
            sr.Close();
            sw.Close();
            swNC.Close();
            swE.Close();
        }

        private static void CargaRelacionesEntreAssets2()
        {
            var sr = new StreamReader(ConfigurationManager.AppSettings["Path"] + "asociar_funcionalidades.csv");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "asociar_funcionalidades.log");
            var swNC = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "asociar_funcionalidades-nocreadas.log");
            var swE = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "asociar_funcionalidades-errores.log");

            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split(';'); //('\t');
                var id_invoca = linea[0].Trim();
                var id_invocado = linea[4].Trim();

                if (id_invoca != "")
                {
                    try
                    {
                        var lInvoca = Convert.ToInt32(id_invoca);
                        var lInvocado = Convert.ToInt32(id_invocado);
                        Console.WriteLine("Cargando {0}\t{1}", id_invoca, id_invocado);
                        var asset = ws.assetRead(token, lInvoca);

                        var asset2 = ws.assetRead(token, lInvocado);

                        if (asset != null && asset2 != null)
                        {

                            List<long> idRel = new List<long>();
                            idRel.Add(lInvoca);
                            if (OERHelper.creaAssetsRelated(lInvocado, (long)tipoRelation.Implements, idRel.ToArray())) //50000 Invokes
                            {
                                sw.WriteLine("Creada relación {0}\t{1}", id_invoca, id_invocado);
                                Console.WriteLine("Creada relación {0}\t{1}", id_invoca, id_invocado);
                            }
                            else
                            {
                                swE.WriteLine("Error al crear relación {0}\t{1}", id_invoca, id_invocado);
                                Console.WriteLine("Error al crear relación {0}\t{1}", id_invoca, id_invocado);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        swE.WriteLine("{0}\t{1}\t{2}", id_invoca, id_invocado, ex.Message);
                        Console.WriteLine("ERROR {0}\t{1}\t{2}", id_invoca, id_invocado, ex.Message);
                    }
                }

            }
            sr.Close();
            sw.Close();
            swNC.Close();
            swE.Close();
        }

        private static void CargaDatosEntidades()
        {
            var sr = new StreamReader(ConfigurationManager.AppSettings["Path"] + "Componentes.csv");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "Componentes.log");
            var swNC = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "Componentes-nocreadas.log");
            var swE = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "Componentes-errores.log");

            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split(',');

                var nombre_componente = linea[0].Trim();
                var tipo_componente = linea[1].Trim();
                var version_componente = linea[2].Trim();

                if (nombre_componente != "")
                {
                    try
                    {
                        Console.WriteLine("Creando {0}\t{1}", nombre_componente, tipo_componente);
                        var asset = ws.assetCreate(token, nombre_componente, version_componente, (int)tipoAsset.Component);

                        sw.WriteLine("{0}\t{1}\t{2}", nombre_componente, tipo_componente, version_componente);
                        Console.WriteLine("{0}\t{1}\t{2}", nombre_componente, tipo_componente, version_componente);

                        //lOperations.Add(new PairKeyValue(asset.ID, tipo_componente));
                    }
                    catch (Exception ex)
                    {
                        swE.WriteLine("{0}\t{1}\t{2}\t{3}", nombre_componente, tipo_componente, version_componente, ex.Message);
                        Console.WriteLine("ERROR {0}\t{1}\t{2}\t{3}", nombre_componente, tipo_componente, version_componente, ex.Message);
                    }
                }
            }
            sr.Close();
            sw.Close();
            swNC.Close();
            swE.Close();
        }

        private static void CargaDatosAPIs()
        {
            var results = JsonConvert.DeserializeObject<ListaAPI>(File.ReadAllText(@"C:\Trabajo\apis.json"));

            foreach (var item in results.Lista)
            {
                var asset = ws.assetCreate(token, item.Api, "1.0", (int)tipoAsset.APIs);

                asset.description = item.Description;
                var assetU = ws.assetUpdate(token, asset);

                Console.WriteLine("Asset creado {0}\t{1}", asset.ID, asset.name);

                //var assetU2 = ws.assetUpdateCustomDataString(token, asset.ID, cdType, cdValue);
                //OERHelper.updateCustomData(asset.ID, "retirement-decommission-date", "2016-04-20T00:00:00");

                ws.assetAddCategorization(token, asset.ID, "AssetLifecycleStage", "Stage 1 - Propuesto");

                //esposed by 53886, API MANAGER
                List<long> idRel = new List<long>();
                idRel.Add(53886);

                if (OERHelper.creaAssetsRelated(asset.ID, (long)tipoRelation.ExposedBy, idRel.ToArray()))
                {
                    Console.WriteLine("Creada relación {0}\t{1}", 53886, asset.ID);
                }
                else
                {
                    Console.WriteLine("Error al crear relación {0}\t{1}", 53886, asset.ID);
                }
            }
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
                var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
                //criteria.assetTypeCriteria = (int)tipoAsset.Operation;
                var serviceQ = ws.assetQuerySummary(token, criteria);
                foreach (var item in serviceQ)
                {
                    //Console.WriteLine("Insertando asset {0}\t{1}", item.ID, item.name);

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

                Console.WriteLine("Leyendo assets para leer sus relaciones");

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from asset";
                    //cmd.CommandText = "select * from asset where id = 50285";
                    cmd.CommandType = CommandType.Text;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = Convert.ToInt64(reader["id"]);
                            var name = Convert.ToString(reader["name"]);
                            var type = Convert.ToInt32(reader["type"]);
                            System.Diagnostics.Debug.WriteLine(String.Format("Buscando asset {0}\t{1}", id, name));

                            var item = ws.assetReadXml(token, id);

                            //Console.WriteLine("Buscando custom-data del asset {0}\t{1}", id, name);
                            if (item != "")
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
                                        if (nodeStep.Name == "service-type")
                                        {
                                            update_asset = true;
                                            valores.Add(new PairKeyValue(0, "attibute1", nodeStep.InnerText));
                                        }
                                        if (type == ((int)tipoAsset.Service) && nodeStep.Name == "documentation")
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

                                                                var i = cmdR.ExecuteNonQuery();
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

                                                                var i = cmdR.ExecuteNonQuery();
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

        private static void creaListaOperacionesServ()
        {
            lOperations = new List<PairKeyValue>();

            Console.WriteLine("OPERACIONES");

            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.assetTypeCriteria = (int)tipoAsset.Operation;
            var serviceQ = ws.assetQuerySummary(token, criteria);
            foreach (var item in serviceQ)
            {
                lOperations.Add(new PairKeyValue(item.ID, item.name));
            }
        }

        private static void CargaDatosOperaciones()
        {
            var sr = new StreamReader(ConfigurationManager.AppSettings["Path"] + "Listado-App-Servicios-Arg-2.csv");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "operaciones.log");
            var swNC = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "operaciones-nocreadas.log");
            var swE = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "operaciones-errores.log");

            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split('\t');

                var servicio = linea[0].Trim();
                var operacion = linea[1].Trim();
                var aplicacion = linea[2].Trim();

                long serviceId = 0;
                var servL = lServices.Where(m => m.Name == servicio);
                if (servL.Count() > 0)
                {
                    serviceId = servL.First().Id;
                }

                long appId = 0;
                var appL = lApps.Where(m => m.Name == aplicacion);
                if (appL.Count() > 0)
                {
                    appId = appL.First().Id;
                }

                if (serviceId > 0)
                {
                    try
                    {
                        long operacionId = 0;
                        var operL = lOperations.Where(m => m.Name == operacion);
                        if (operL.Count() > 0)
                        {
                            operacionId = operL.First().Id;

                            Console.WriteLine("Buscando {0}\t{1}", servicio, operacion);
                            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
                            criteria.assetTypeCriteria = (int)tipoAsset.Operation;
                            criteria.IDCriteria = operacionId;
                            var serviceQ = ws.assetQuery(token, criteria);
                            foreach (var item in serviceQ)
                            {
                                var relContains = false;
                                var relInvoked = false;
                                var update = false;
                                foreach (var rel in item.relationshipTypes)
                                {
                                    relContains = relContains || (rel.ID == (int)tipoRelation.Contains && rel.primaryIDs.Contains(serviceId));
                                    relInvoked = relInvoked || (rel.ID == (int)tipoRelation.Invoked && rel.secondaryIDs.Contains(appId));

                                    if (rel.ID == (int)tipoRelation.Contains && !rel.primaryIDs.Contains(serviceId))
                                    {
                                        var ultimo = rel.primaryIDs.Length;
                                        var paso = new long[rel.primaryIDs.Length + 1];
                                        Array.Copy(rel.primaryIDs, paso, rel.primaryIDs.Length);

                                        paso[ultimo] = serviceId;
                                        rel.primaryIDs = paso;
                                        update = true;
                                    }
                                    if (rel.ID == (int)tipoRelation.Invoked && !rel.secondaryIDs.Contains(appId))
                                    {
                                        var ultimo = rel.secondaryIDs.Length;
                                        var paso = new long[rel.secondaryIDs.Length + 1];
                                        Array.Copy(rel.secondaryIDs, paso, rel.secondaryIDs.Length);

                                        paso[ultimo] = appId;
                                        rel.secondaryIDs = paso;
                                        update = true;
                                    }
                                }
                                if (update)
                                {
                                    Console.WriteLine("Actualizando relaciones {0}\t{1}", servicio, operacion);
                                    var assetU = ws.assetUpdate(token, item);
                                }
                                break;
                            }

                        }
                        else
                        {
                            Console.WriteLine("Creando {0}\t{1}", servicio, operacion);
                            var asset = ws.assetCreate(token, operacion, "1.0", (int)tipoAsset.Operation);

                            asset.relationshipTypes = new OER_CargaDatos.OerProd.RelationshipType[2];
                            asset.relationshipTypes[0] = new OER_CargaDatos.OerProd.RelationshipType();
                            asset.relationshipTypes[0].ID = (int)tipoRelation.Contains;
                            asset.relationshipTypes[0].primaryIDs = new long[1];
                            asset.relationshipTypes[0].primaryIDs[0] = serviceId;

                            asset.relationshipTypes[1] = new OER_CargaDatos.OerProd.RelationshipType();
                            asset.relationshipTypes[1].ID = (int)tipoRelation.Invoked;
                            asset.relationshipTypes[1].secondaryIDs = new long[1];
                            asset.relationshipTypes[1].secondaryIDs[0] = appId;

                            var assetU = ws.assetUpdate(token, asset);

                            var assetU2 = ws.assetUpdateCustomDataString(token, asset.ID, "service-type", "Data Service");

                            sw.WriteLine("{0}\t{1}\t{2}", servicio, operacion, aplicacion);
                            Console.WriteLine("{0}\t{1}\t{2}", servicio, operacion, aplicacion);

                            lOperations.Add(new PairKeyValue(asset.ID, operacion));
                        }
                    }
                    catch (Exception ex)
                    {
                        swE.WriteLine("{0}\t{1}\t{2}\t{3}", servicio, operacion, aplicacion, ex.Message);
                        Console.WriteLine("ERROR {0}\t{1}\t{2}\t{3}", servicio, operacion, aplicacion, ex.Message);
                    }
                }
            }
            sr.Close();
            sw.Close();
            swNC.Close();
            swE.Close();
        }

        private static void CargaDatosOperaciones2()
        {
            var nombre_servicio = "crm-assurance-c";
            var path2 = ConfigurationManager.AppSettings["Path"] + "Referencias\\";

            var sr = new StreamReader(path2 + nombre_servicio + ".csv");
            var sw = new StreamWriter(path2 + nombre_servicio + ".log");
            var swNC = new StreamWriter(path2 + nombre_servicio + "-no-creados.log");
            var swE = new StreamWriter(path2 + nombre_servicio + "-errores.log");

            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split('\t');

                var servicio = linea[0].Trim();
                var operacion = linea[1].Trim();
                var oper_invocada = linea[2].Trim();
                var portType = linea[4].Trim();
                var servicio_invocado = portType.Remove(0, portType.LastIndexOf(':') + 1);

                long serviceOrigenId = 0;
                var servLO = lServices.Where(m => m.Name == servicio);
                if (servLO.Count() > 0)
                {
                    serviceOrigenId = servLO.First().Id;
                }

                long serviceId = 0;
                var servL = lServices.Where(m => m.Key == servicio_invocado);
                if (servL.Count() > 0)
                {
                    serviceId = servL.First().Id;
                }

                long operId = 0;
                var operL = lOperations.Where(m => m.Name == operacion);
                if (operL.Count() > 0)
                {
                    operId = operL.First().Id;
                }
                else
                {
                    //crear operacion
                    var version = "1.0";
                    Console.WriteLine("Creando no existe {0}\t{1}", servicio, operacion);
                    var asset = ws.assetCreate(token, operacion, version, (int)tipoAsset.Operation);
                    operId = asset.ID;

                    asset.relationshipTypes = new OER_CargaDatos.OerProd.RelationshipType[1];
                    asset.relationshipTypes[0] = new OER_CargaDatos.OerProd.RelationshipType();
                    asset.relationshipTypes[0].ID = (int)tipoRelation.Contains;
                    asset.relationshipTypes[0].primaryIDs = new long[1];
                    asset.relationshipTypes[0].primaryIDs[0] = serviceOrigenId;

                    var assetU = ws.assetUpdate(token, asset);

                    var assetU2 = ws.assetUpdateCustomDataString(token, asset.ID, "service-type", "Web Service");

                    sw.WriteLine("{0}\t{1}\t{2}", servicio, operacion, operacion);
                    Console.WriteLine("{0}\t{1}\t{2}", servicio, operacion, operacion);

                    lOperations.Add(new PairKeyValue(asset.ID, operacion));
                }

                if (serviceId > 0 && operId > 0)
                {
                    try
                    {
                        long operacionId = 0;
                        var operLIbs = lOperations.Where(m => m.Name == oper_invocada);
                        if (operLIbs.Count() > 0)
                        {
                            operacionId = operLIbs.First().Id;

                            Console.WriteLine("Buscando {0}\t{1}", servicio_invocado, oper_invocada);
                            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
                            criteria.assetTypeCriteria = (int)tipoAsset.Operation;
                            criteria.IDCriteria = operacionId;
                            var serviceQ = ws.assetQuery(token, criteria);
                            foreach (var item in serviceQ)
                            {
                                //var relContains = false;
                                //var relInvoked = false;
                                var update = false;
                                foreach (var rel in item.relationshipTypes)
                                {
                                    //    relContains = relContains || (rel.ID == (int)tipoRelation.Contains && rel.primaryIDs.Contains(serviceId));
                                    //    relInvoked = relInvoked || (rel.ID == (int)tipoRelation.Invoked && rel.secondaryIDs.Contains(operId));

                                    //if (rel.ID == (int)tipoRelation.Contains && !rel.primaryIDs.Contains(serviceId))
                                    //{
                                    //    var ultimo = rel.primaryIDs.Length;
                                    //    var paso = new long[rel.primaryIDs.Length + 1];
                                    //    Array.Copy(rel.primaryIDs, paso, rel.primaryIDs.Length);

                                    //    paso[ultimo] = serviceId;
                                    //    rel.primaryIDs = paso;
                                    //    update = true;
                                    //}
                                    if (rel.ID == (int)tipoRelation.Invoked && !rel.secondaryIDs.Contains(operId))
                                    {
                                        var ultimo = rel.secondaryIDs.Length;
                                        var paso = new long[rel.secondaryIDs.Length + 1];
                                        Array.Copy(rel.secondaryIDs, paso, rel.secondaryIDs.Length);

                                        paso[ultimo] = operId;
                                        rel.secondaryIDs = paso;
                                        update = true;
                                    }
                                }
                                if (update)
                                {
                                    Console.WriteLine("Actualizando relaciones {0}\t{1}", servicio_invocado, oper_invocada);
                                    var assetU = ws.assetUpdate(token, item);
                                }
                                break;
                            }

                        }
                        else
                        {
                            Console.WriteLine("Creando {0}\t{1}", servicio_invocado, oper_invocada);
                            var version = "6.2";
                            var asset = ws.assetCreate(token, oper_invocada, version, (int)tipoAsset.Operation);

                            asset.relationshipTypes = new OER_CargaDatos.OerProd.RelationshipType[2];
                            asset.relationshipTypes[0] = new OER_CargaDatos.OerProd.RelationshipType();
                            asset.relationshipTypes[0].ID = (int)tipoRelation.Contains;
                            asset.relationshipTypes[0].primaryIDs = new long[1];
                            asset.relationshipTypes[0].primaryIDs[0] = serviceId;

                            asset.relationshipTypes[1] = new OER_CargaDatos.OerProd.RelationshipType();
                            asset.relationshipTypes[1].ID = (int)tipoRelation.Invoked;
                            asset.relationshipTypes[1].secondaryIDs = new long[1];
                            asset.relationshipTypes[1].secondaryIDs[0] = operId;

                            var assetU = ws.assetUpdate(token, asset);

                            var assetU2 = ws.assetUpdateCustomDataString(token, asset.ID, "service-type", "Business Service");

                            sw.WriteLine("{0}\t{1}\t{2}", servicio, operacion, oper_invocada);
                            Console.WriteLine("{0}\t{1}\t{2}", servicio, operacion, oper_invocada);

                            lOperations.Add(new PairKeyValue(asset.ID, oper_invocada));
                        }
                    }
                    catch (Exception ex)
                    {
                        swE.WriteLine("{0}\t{1}\t{2}\t{3}", servicio, operacion, oper_invocada, ex.Message);
                        Console.WriteLine("ERROR {0}\t{1}\t{2}\t{3}", servicio, operacion, oper_invocada, ex.Message);
                    }
                }
                else
                {
                    swNC.WriteLine("{0}\t{1}\t{2}\t{3}", servicio, operacion, oper_invocada, servicio_invocado);
                    Console.WriteLine("NO CREADA {0}\t{1}\t{2}\t{3}", servicio, operacion, oper_invocada, servicio_invocado);
                }
            }
            sr.Close();
            sw.Close();
            swNC.Close();
            swE.Close();
        }

        private static void CargaInterfacesServApp()
        {
            var sr = new StreamReader(ConfigurationManager.AppSettings["Path"] + "servicios_interfaz.txt");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "servicios_interfaz.log");
            var swNC = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "servicios_interfaz-nocreadas.log");
            var swE = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "servicios_interfaz-errores.log");

            OERHelper.crearServicio();

            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split('\t');

                var servicio = linea[0].Trim();
                var interfaz = linea[1].Trim();

                long serviceId = Int32.Parse(servicio);

                try
                {
                    Console.WriteLine("Buscando {0}\t{1}", servicio, interfaz);
                    var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
                    criteria.assetTypeCriteria = (int)tipoAsset.Service;
                    criteria.IDCriteria = serviceId;
                    var serviceQ = ws.assetQuery(token, criteria);
                    foreach (var item in serviceQ)
                    {
                        OERHelper.updateCustomData(item.ID, "port-type", interfaz);

                        Console.WriteLine("Actualizando interfez {0}\t{1}", servicio, interfaz);
                    }
                }
                catch (Exception ex)
                {
                    swE.WriteLine("{0}\t{1}\t{2}", servicio, interfaz, ex.Message);
                    Console.WriteLine("ERROR {0}\t{1}\t{2}", servicio, interfaz, ex.Message);
                }

            }
            sr.Close();
            sw.Close();
            swNC.Close();
            swE.Close();
        }

        private static void CargaFechaUndepoy()
        {
            var sr = new StreamReader(ConfigurationManager.AppSettings["Path"] + "poner fecha retiro-2.txt");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "poner fecha retiro.log");
            var swNC = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "poner fecha retiro-nocreadas.log");
            var swE = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "poner fecha retiro-errores.log");

            OERHelper.crearServicio();

            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split('\t');

                if (linea.Count() == 2)
                {
                    var id_asset = linea[0].Trim();
                    var nombre = linea[1].Trim();

                    long assetId = Int32.Parse(id_asset);

                    try
                    {
                        Console.WriteLine("Buscando {0}\t{1}", id_asset, nombre);
                        var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
                        criteria.IDCriteria = assetId;
                        var serviceQ = ws.assetQuery(token, criteria);
                        foreach (var item in serviceQ)
                        {
                            /*
                             * <retirement-decommission-date>30 Sep 2016 12:00:00 AM ART
                             */
                            //OERHelper.updateCustomData(item.ID, "retirement-decommission-date", "2016-09-30T00:00:00");
                            OERHelper.updateCustomData(item.ID, "retirement-decommission-date", "2016-04-20T00:00:00");

                            Console.WriteLine("Actualizando interfez {0}\t{1}", id_asset, nombre);
                        }
                    }
                    catch (Exception ex)
                    {
                        swE.WriteLine("{0}\t{1}\t{2}", id_asset, nombre, ex.Message);
                        Console.WriteLine("ERROR {0}\t{1}\t{2}", id_asset, nombre, ex.Message);
                    }
                }
            }
            sr.Close();
            sw.Close();
            swNC.Close();
            swE.Close();
        }

        private static void CargaDatosServApp()
        {
            var sr = new StreamReader(ConfigurationManager.AppSettings["Path"] + "App-Servicios-2.csv");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "App-Servicios.log");
            var swNC = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "App-Servicios-nocreadas.log");
            var swE = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "App-Servicios-errores.log");

            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split('\t');

                var servicio = linea[0].Trim();
                var aplicacion = linea[1].Trim();

                long serviceId = 0;
                var servL = lServices.Where(m => m.Name == servicio);
                if (servL.Count() > 0)
                {
                    serviceId = servL.First().Id;
                }

                long appId = 0;
                var appL = lApps.Where(m => m.Name == aplicacion);
                if (appL.Count() > 0)
                {
                    appId = appL.First().Id;
                }

                if (serviceId > 0 && appId > 0)
                {
                    try
                    {
                        Console.WriteLine("Buscando {0}\t{1}", servicio, aplicacion);
                        var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
                        criteria.assetTypeCriteria = (int)tipoAsset.Service;
                        criteria.IDCriteria = serviceId;
                        var serviceQ = ws.assetQuery(token, criteria);
                        foreach (var item in serviceQ)
                        {
                            var update = false;
                            foreach (var rel in item.relationshipTypes)
                            {
                                if (rel.ID == (int)tipoRelation.ReferencedBy && !rel.primaryIDs.Contains(appId))
                                {
                                    var ultimo = rel.primaryIDs.Length;
                                    var paso = new long[rel.primaryIDs.Length + 1];
                                    Array.Copy(rel.primaryIDs, paso, rel.primaryIDs.Length);

                                    paso[ultimo] = appId;
                                    rel.primaryIDs = paso;
                                    update = true;
                                }
                            }
                            if (update)
                            {
                                Console.WriteLine("Actualizando relaciones {0}\t{1}", servicio, aplicacion);
                                var assetU = ws.assetUpdate(token, item);
                            }
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        swE.WriteLine("{0}\t{1}\t{2}", servicio, aplicacion, ex.Message);
                        Console.WriteLine("ERROR {0}\t{1}\t{2}", servicio, aplicacion, ex.Message);
                    }
                }
                else
                {
                    swNC.WriteLine("NO SE ENCONTRO {0}\t{1}", servicio, aplicacion);
                    Console.WriteLine("NO SE ENCONTRO {0}\t{1}", servicio, aplicacion);
                }
            }
            sr.Close();
            sw.Close();
            swNC.Close();
            swE.Close();
        }

        private static void creaListaOperaciones()
        {
            lOperations = new List<PairKeyValue>();
            lOperations.Add(new PairKeyValue(50161, "ActivateAdditionalServiceForPostpaidCustomer"));
            lOperations.Add(new PairKeyValue(50426, "ActivateCustomerProduct"));
            lOperations.Add(new PairKeyValue(50326, "ActivateService"));
            lOperations.Add(new PairKeyValue(50162, "ActivateServiceForPostpaidCustomer"));
            lOperations.Add(new PairKeyValue(50198, "ActivateServiceLink"));
            lOperations.Add(new PairKeyValue(50197, "ActivateServiceLinkSwopRobo"));
            lOperations.Add(new PairKeyValue(50196, "ActivateServiceSwopTDT"));
            lOperations.Add(new PairKeyValue(50195, "ActivateServiceTDT"));
            lOperations.Add(new PairKeyValue(50309, "AddAccounts"));
            lOperations.Add(new PairKeyValue(50347, "AddActivation"));
            lOperations.Add(new PairKeyValue(50189, "AddActivationByProcess"));
            lOperations.Add(new PairKeyValue(50230, "AddAddress"));
            lOperations.Add(new PairKeyValue(50249, "AddCampaign"));
            lOperations.Add(new PairKeyValue(50269, "AddCommandCAMC"));
            lOperations.Add(new PairKeyValue(50131, "AddContact"));
            lOperations.Add(new PairKeyValue(50313, "AddCustomer"));
            lOperations.Add(new PairKeyValue(50285, "AddCustomer - IntArg"));
            lOperations.Add(new PairKeyValue(50318, "AddCustomerAccountProductInvolvement"));
            lOperations.Add(new PairKeyValue(50427, "AddCustomerAccountProductInvolvementWithResource"));
            lOperations.Add(new PairKeyValue(50011, "AddCustomerAgreement"));
            lOperations.Add(new PairKeyValue(50204, "AddCustomerInquiry"));
            lOperations.Add(new PairKeyValue(50019, "AddCustomerProductOffer"));
            lOperations.Add(new PairKeyValue(50299, "AddCustomerRelation"));
            lOperations.Add(new PairKeyValue(50203, "AddCustomerWorkOrder"));
            lOperations.Add(new PairKeyValue(50339, "AddIRDChanges"));
            lOperations.Add(new PairKeyValue(50258, "AddJobCard"));
            lOperations.Add(new PairKeyValue(50250, "AddKeyword"));
            lOperations.Add(new PairKeyValue(50240, "AddOrUpgradeCustomerProductInvolvement"));
            lOperations.Add(new PairKeyValue(50283, "AddPPV"));
            lOperations.Add(new PairKeyValue(50282, "AddPPVAllSmartCard"));
            lOperations.Add(new PairKeyValue(50270, "AddPremium"));
            lOperations.Add(new PairKeyValue(50256, "AddScheduleWithEventReason"));
            lOperations.Add(new PairKeyValue(50277, "AddServiceToWorkOrder"));
            lOperations.Add(new PairKeyValue(50393, "ApplyCustomerSpear"));
            lOperations.Add(new PairKeyValue(50315, "ApproveShippingOrder"));
            lOperations.Add(new PairKeyValue(50329, "AsignadorUnico"));
            lOperations.Add(new PairKeyValue(50017, "BestAddress"));
            lOperations.Add(new PairKeyValue(50453, "CancelAgreementDetails"));
            lOperations.Add(new PairKeyValue(50271, "CancelarProduct"));
            lOperations.Add(new PairKeyValue(50234, "CancelCustomerProductByProductId"));
            lOperations.Add(new PairKeyValue(50241, "CancelCustomerProducts"));
            lOperations.Add(new PairKeyValue(50278, "CancelWorkOrder"));
            lOperations.Add(new PairKeyValue(50147, "CloseShippingOrder"));
            lOperations.Add(new PairKeyValue(50190, "CompareResourceTechnology"));
            lOperations.Add(new PairKeyValue(50226, "CompleteWorkOrder"));
            lOperations.Add(new PairKeyValue(50451, "CreateAgreement"));
            lOperations.Add(new PairKeyValue(50467, "CreateChargeTransactionWithDiscounts"));
            lOperations.Add(new PairKeyValue(50464, "CreateContact"));
            lOperations.Add(new PairKeyValue(50201, "CreateCustomerCharge"));
            lOperations.Add(new PairKeyValue(50013, "CreateCustomerPayment"));
            lOperations.Add(new PairKeyValue(50452, "CreateOffer"));
            lOperations.Add(new PairKeyValue(50469, "CreatePaymentTransactions"));
            lOperations.Add(new PairKeyValue(50369, "CreateRelation"));
            lOperations.Add(new PairKeyValue(50417, "CreateSandbox"));
            lOperations.Add(new PairKeyValue(50312, "CreateShippingOrder"));
            lOperations.Add(new PairKeyValue(50367, "CustomerExists"));
            lOperations.Add(new PairKeyValue(50251, "DeleteAllActiveSchedules"));
            lOperations.Add(new PairKeyValue(50183, "DeleteCampaign"));
            lOperations.Add(new PairKeyValue(50316, "DeleteCustomerCharacteristic"));
            lOperations.Add(new PairKeyValue(50206, "DeleteCustomerProductOffer"));
            lOperations.Add(new PairKeyValue(50281, "DeleteCustomerProductOfferByProductOfferID"));
            lOperations.Add(new PairKeyValue(50135, "DeleteCustomerProducts"));
            lOperations.Add(new PairKeyValue(50252, "DeleteKeywords"));
            lOperations.Add(new PairKeyValue(50454, "DeleteOffer"));
            lOperations.Add(new PairKeyValue(50253, "DeleteScheduleById"));
            lOperations.Add(new PairKeyValue(50245, "Despaquetizar"));
            lOperations.Add(new PairKeyValue(50185, "doActivarPrepago"));
            lOperations.Add(new PairKeyValue(50320, "doActivarWimax"));
            lOperations.Add(new PairKeyValue(50321, "doSwopWiMax"));
            lOperations.Add(new PairKeyValue(50272, "Downgrade"));
            lOperations.Add(new PairKeyValue(50301, "DowngradePremium"));
            lOperations.Add(new PairKeyValue(50163, "DowngradeServiceForPostpaidCustomer"));
            lOperations.Add(new PairKeyValue(50164, "DowngradeServiceForPostpaidCustomerArgentina"));
            lOperations.Add(new PairKeyValue(50264, "EditAcc"));
            lOperations.Add(new PairKeyValue(50121, "EditAccounts"));
            lOperations.Add(new PairKeyValue(50115, "EditarCustomer"));
            lOperations.Add(new PairKeyValue(50259, "EditarJobCard"));
            lOperations.Add(new PairKeyValue(50008, "EditCustomer"));
            lOperations.Add(new PairKeyValue(50298, "EditCustomerAccount"));
            lOperations.Add(new PairKeyValue(50113, "EditCustomerInquiry"));
            lOperations.Add(new PairKeyValue(50179, "EditCustomerProduct"));
            lOperations.Add(new PairKeyValue(50291, "EditCustomerWorkOrder"));
            lOperations.Add(new PairKeyValue(50317, "EditInvoice"));
            lOperations.Add(new PairKeyValue(50284, "EditNid"));
            lOperations.Add(new PairKeyValue(50232, "EditShippingOrder"));
            lOperations.Add(new PairKeyValue(50156, "EsBillingRule"));
            lOperations.Add(new PairKeyValue(50122, "EsImputable"));
            lOperations.Add(new PairKeyValue(50260, "FinalizarJobCard"));
            lOperations.Add(new PairKeyValue(50420, "FinalizeSandbox"));
            lOperations.Add(new PairKeyValue(50443, "FindCommunicationLogEntries"));
            lOperations.Add(new PairKeyValue(50396, "FindCustomerByAddressDetails"));
            lOperations.Add(new PairKeyValue(50395, "FindCustomerByCustomerDetails"));
            lOperations.Add(new PairKeyValue(50327, "GenerarPDF"));
            lOperations.Add(new PairKeyValue(50123, "GetAccount"));
            lOperations.Add(new PairKeyValue(50254, "GetActiveScheduleByCustomer"));
            lOperations.Add(new PairKeyValue(50194, "GetAddActivationConfiguration"));
            lOperations.Add(new PairKeyValue(50231, "GetAddress"));
            lOperations.Add(new PairKeyValue(50430, "GetAgeingForFinancialAccount"));
            lOperations.Add(new PairKeyValue(50447, "GetAgreement"));
            lOperations.Add(new PairKeyValue(50371, "GetAgreementDetail"));
            lOperations.Add(new PairKeyValue(50446, "GetAgreementDetailsForCustomer"));
            lOperations.Add(new PairKeyValue(50439, "GetAgreementDetailStatus"));
            lOperations.Add(new PairKeyValue(50424, "GetAgreementDetailView"));
            lOperations.Add(new PairKeyValue(50418, "GetAgreementsForCustomerId"));
            lOperations.Add(new PairKeyValue(50404, "GetAgreementView"));
            lOperations.Add(new PairKeyValue(50217, "GetAllocationPrepaId"));
            lOperations.Add(new PairKeyValue(50218, "GetAllocationPrepaidByBasicProduct"));
            lOperations.Add(new PairKeyValue(50199, "GetAntenaSenial"));
            lOperations.Add(new PairKeyValue(50319, "GetAvLink"));
            lOperations.Add(new PairKeyValue(50191, "GetBasicProductMigration"));
            lOperations.Add(new PairKeyValue(50219, "GetBonus"));
            lOperations.Add(new PairKeyValue(50306, "GetCampaignActive"));
            lOperations.Add(new PairKeyValue(50304, "GetCampaigns"));
            lOperations.Add(new PairKeyValue(50255, "GetCampSubs"));
            lOperations.Add(new PairKeyValue(50140, "GetCanalByServiceId"));
            lOperations.Add(new PairKeyValue(50295, "GetChannelsForCustomer"));
            lOperations.Add(new PairKeyValue(50235, "GetCommandStatus"));
            lOperations.Add(new PairKeyValue(50436, "GetCommercialProduct"));
            lOperations.Add(new PairKeyValue(50437, "GetCommercialProductCategory"));
            lOperations.Add(new PairKeyValue(50307, "GetConfig"));
            lOperations.Add(new PairKeyValue(50132, "GetContact"));
            lOperations.Add(new PairKeyValue(50466, "GetContact-IBS"));
            lOperations.Add(new PairKeyValue(50267, "GetContactBeforeDays"));
            lOperations.Add(new PairKeyValue(50459, "GetContactView"));
            lOperations.Add(new PairKeyValue(50358, "GetContext"));
            lOperations.Add(new PairKeyValue(50006, "GetCustomer"));
            lOperations.Add(new PairKeyValue(50116, "GetCustomer - IntArg"));
            lOperations.Add(new PairKeyValue(50391, "GetCustomer-IBS"));
            lOperations.Add(new PairKeyValue(50222, "GetCustomerAddresses"));
            lOperations.Add(new PairKeyValue(50346, "GetCustomerAgreements"));
            lOperations.Add(new PairKeyValue(50305, "GetCustomerByDNI"));
            lOperations.Add(new PairKeyValue(50223, "GetCustomerByIdentification"));
            lOperations.Add(new PairKeyValue(50398, "GetCustomerByInternetUserId"));
            lOperations.Add(new PairKeyValue(50397, "GetCustomerBySerialNumber"));
            lOperations.Add(new PairKeyValue(50207, "GetCustomerCharacteristics"));
            lOperations.Add(new PairKeyValue(50141, "GetCustomerDecodersAndScDT"));
            lOperations.Add(new PairKeyValue(50370, "GetCustomerDeviceView"));
            lOperations.Add(new PairKeyValue(50022, "GetCustomerFinancialInfo"));
            lOperations.Add(new PairKeyValue(50221, "GetCustomerFinancialInfoWithWallet"));
            lOperations.Add(new PairKeyValue(50225, "GetCustomerFinancialTransactions"));
            lOperations.Add(new PairKeyValue(50024, "GetCustomerFinancialTransactionsByCriteria"));
            lOperations.Add(new PairKeyValue(50303, "GetCustomerHistoriesByCriteria"));
            lOperations.Add(new PairKeyValue(50148, "GetCustomerHistoriesByCriteriaByEntity"));
            lOperations.Add(new PairKeyValue(50149, "GetCustomerHistoryByCriteriaByEvents"));
            lOperations.Add(new PairKeyValue(50150, "GetCustomerHistoryByCriteriaByReasonsId"));
            lOperations.Add(new PairKeyValue(50205, "GetCustomerIncobrable"));
            lOperations.Add(new PairKeyValue(50242, "GetCustomerInquiries"));
            lOperations.Add(new PairKeyValue(50243, "GetCustomerInquiriesByCriteria"));
            lOperations.Add(new PairKeyValue(50244, "GetCustomerInquiriesByDays"));
            lOperations.Add(new PairKeyValue(50209, "GetCustomerInvoiceById"));
            lOperations.Add(new PairKeyValue(50151, "GetCustomerInvoicingProfile"));
            lOperations.Add(new PairKeyValue(50210, "GetCustomerLastInvoice"));
            lOperations.Add(new PairKeyValue(50112, "GetCustomerOrderedEventsByCriteria"));
            lOperations.Add(new PairKeyValue(50007, "GetCustomerProductByStatus"));
            lOperations.Add(new PairKeyValue(50208, "GetCustomerProductsByCriteria"));
            lOperations.Add(new PairKeyValue(50152, "GetCustomerProductsByCriteriaByCustomer"));
            lOperations.Add(new PairKeyValue(50211, "GetCustomerProductsOffer"));
            lOperations.Add(new PairKeyValue(50300, "GetCustomerRelations"));
            lOperations.Add(new PairKeyValue(50180, "GetCustomerResources"));
            lOperations.Add(new PairKeyValue(50233, "GetCustomerResources - Crm"));
            lOperations.Add(new PairKeyValue(50224, "GetCustomerResourcesByCriteria"));
            lOperations.Add(new PairKeyValue(50290, "GetCustomersByIdentification"));
            lOperations.Add(new PairKeyValue(50289, "GetCustomersByInternetUserId"));
            lOperations.Add(new PairKeyValue(50018, "GetCustomersByNames"));
            lOperations.Add(new PairKeyValue(50010, "GetCustomersByPhoneNumber"));
            lOperations.Add(new PairKeyValue(50181, "GetCustomersBySerialNumber"));
            lOperations.Add(new PairKeyValue(50236, "GetCustomerSchedules"));
            lOperations.Add(new PairKeyValue(50153, "GetCustomerStatus"));
            lOperations.Add(new PairKeyValue(50431, "GetCustomerView"));
            lOperations.Add(new PairKeyValue(50182, "GetCustomerWithDocumento"));
            lOperations.Add(new PairKeyValue(50146, "GetCustomerWorkOrderById"));
            lOperations.Add(new PairKeyValue(50279, "GetCustomerWorkOrderByStatus"));
            lOperations.Add(new PairKeyValue(50227, "GetCustomerWorkOrdersByCriteria"));
            lOperations.Add(new PairKeyValue(50343, "GetDealerCodes"));
            lOperations.Add(new PairKeyValue(50229, "GetDealerInfo"));
            lOperations.Add(new PairKeyValue(50184, "GetDecosByDecoOrScNr"));
            lOperations.Add(new PairKeyValue(50136, "GetDecosWithSC"));
            lOperations.Add(new PairKeyValue(50276, "GetDecosWithScAndCT"));
            lOperations.Add(new PairKeyValue(50265, "GetDeuda"));
            lOperations.Add(new PairKeyValue(50372, "GetDeviceBySerialNumber"));
            lOperations.Add(new PairKeyValue(50441, "GetDevicesPerAgreementDetailByDevice"));
            lOperations.Add(new PairKeyValue(50421, "GetDevicesPerAgreementDetailForCustomer"));
            lOperations.Add(new PairKeyValue(50175, "GetEntitiesForActivateService"));
            lOperations.Add(new PairKeyValue(50273, "GetEstadoComando"));
            lOperations.Add(new PairKeyValue(50137, "GetEstadoPaquete"));
            lOperations.Add(new PairKeyValue(50442, "GetEvent"));
            lOperations.Add(new PairKeyValue(50114, "GetEventReason"));
            lOperations.Add(new PairKeyValue(50400, "GetFinancialAccount"));
            lOperations.Add(new PairKeyValue(50468, "GetFinancialAccountsForCustomer"));
            lOperations.Add(new PairKeyValue(50429, "GetFinancialAccountView"));
            lOperations.Add(new PairKeyValue(50471, "GetFinancialTransaction"));
            lOperations.Add(new PairKeyValue(50434, "GetFinancialTransactionsByEntityTypeAndId"));
            lOperations.Add(new PairKeyValue(50129, "GetFinTranFecha"));
            lOperations.Add(new PairKeyValue(50157, "GetFinTransaction"));
            lOperations.Add(new PairKeyValue(50124, "GetFtByOrder"));
            lOperations.Add(new PairKeyValue(50261, "GetGenJobCardService"));
            lOperations.Add(new PairKeyValue(50268, "GetHistory"));
            lOperations.Add(new PairKeyValue(50133, "GetHistoryEvent"));
            lOperations.Add(new PairKeyValue(50406, "GetHistoryRecord"));
            lOperations.Add(new PairKeyValue(50405, "GetHistoryView"));
            lOperations.Add(new PairKeyValue(50440, "GetHomologationByCanonicalCode"));
            lOperations.Add(new PairKeyValue(50423, "GetHomologationCollectionByCanonicalCategoryCode"));
            lOperations.Add(new PairKeyValue(50125, "GetInvDetail"));
            lOperations.Add(new PairKeyValue(50402, "GetInvoice"));
            lOperations.Add(new PairKeyValue(50266, "GetInvoiceByNro"));
            lOperations.Add(new PairKeyValue(50130, "GetInvoiceLine"));
            lOperations.Add(new PairKeyValue(50126, "GetInvoices"));
            lOperations.Add(new PairKeyValue(50154, "GetJobCardGarantia"));
            lOperations.Add(new PairKeyValue(50117, "GetJobCards"));
            lOperations.Add(new PairKeyValue(50262, "GetJobCardsActive"));
            lOperations.Add(new PairKeyValue(50118, "GetJobCardsByNroJC"));
            lOperations.Add(new PairKeyValue(50263, "GetJobCardsStatus"));
            lOperations.Add(new PairKeyValue(50308, "GetKeywordAllowed"));
            lOperations.Add(new PairKeyValue(50257, "GetKeywords"));
            lOperations.Add(new PairKeyValue(50192, "GetMaxResourcesForCustomer"));
            lOperations.Add(new PairKeyValue(50246, "GetNextInvoiceDate"));
            lOperations.Add(new PairKeyValue(50294, "GetNextOrderableEvent"));
            lOperations.Add(new PairKeyValue(50127, "GetNID"));
            lOperations.Add(new PairKeyValue(50407, "GetOfferView"));
            lOperations.Add(new PairKeyValue(50193, "GetOperationForWOServiceType"));
            lOperations.Add(new PairKeyValue(50119, "GetOrdenesBySDS"));
            lOperations.Add(new PairKeyValue(50433, "GetOrderedEventHistory"));
            lOperations.Add(new PairKeyValue(50432, "GetOrderedEventView"));
            lOperations.Add(new PairKeyValue(50120, "GetOrderLines"));
            lOperations.Add(new PairKeyValue(50188, "GetPhysicalResource"));
            lOperations.Add(new PairKeyValue(50200, "GetPhysicalResourceBYSerialNumber"));
            lOperations.Add(new PairKeyValue(50322, "GetPhysicalResourcesByCriteria"));
            lOperations.Add(new PairKeyValue(50287, "GetPPVHistory"));
            lOperations.Add(new PairKeyValue(50435, "GetPricingByCustomer"));
            lOperations.Add(new PairKeyValue(50314, "GetProductOfferById"));
            lOperations.Add(new PairKeyValue(50142, "GetProducts"));
            lOperations.Add(new PairKeyValue(50138, "GetProductsCustomerDS"));
            lOperations.Add(new PairKeyValue(50143, "GetProductsNro"));
            lOperations.Add(new PairKeyValue(50144, "GetProductsStatus"));
            lOperations.Add(new PairKeyValue(50145, "GetProductsType"));
            lOperations.Add(new PairKeyValue(50444, "GetReason"));
            lOperations.Add(new PairKeyValue(50368, "GetRelations"));
            lOperations.Add(new PairKeyValue(50378, "GetRelationType"));
            lOperations.Add(new PairKeyValue(50438, "GetRelationViews"));
            lOperations.Add(new PairKeyValue(50286, "GetSaldos"));
            lOperations.Add(new PairKeyValue(50297, "GetSaldoTextForCustomer"));
            lOperations.Add(new PairKeyValue(50374, "GetScheduleHeader"));
            lOperations.Add(new PairKeyValue(50373, "GetScheduleView"));
            lOperations.Add(new PairKeyValue(50134, "GetServHistDetail"));
            lOperations.Add(new PairKeyValue(50187, "GetShippingOrder"));
            lOperations.Add(new PairKeyValue(50228, "GetShippingOrdersByCustomerId"));
            lOperations.Add(new PairKeyValue(50340, "GetSwops"));
            lOperations.Add(new PairKeyValue(50186, "GetTelecomTechnician"));
            lOperations.Add(new PairKeyValue(50341, "GetUpgradeDowngrade"));
            lOperations.Add(new PairKeyValue(50425, "GetUpgradeDowngradeForCommercialProduct"));
            lOperations.Add(new PairKeyValue(50463, "GetUserByName"));
            lOperations.Add(new PairKeyValue(50349, "GetVista360"));
            lOperations.Add(new PairKeyValue(50139, "InmediatePremiumCancel"));
            lOperations.Add(new PairKeyValue(50296, "Keyword SALDO"));
            lOperations.Add(new PairKeyValue(50165, "LinkServiceForPostpaidCustomer"));
            lOperations.Add(new PairKeyValue(50335, "LIQNETServices"));
            lOperations.Add(new PairKeyValue(50202, "Listener y distribucion de ShO en estado Ready hacia el Asignador Unico"));
            lOperations.Add(new PairKeyValue(50333, "ListenerPPVPack"));
            lOperations.Add(new PairKeyValue(50212, "LocalPayment"));
            lOperations.Add(new PairKeyValue(50419, "ManageProductCapture"));
            lOperations.Add(new PairKeyValue(50422, "ManageSoftwareForAgreementDetail"));
            lOperations.Add(new PairKeyValue(50344, "MoveResourceToStockHandler"));
            lOperations.Add(new PairKeyValue(50292, "OrderPayPerView"));
            lOperations.Add(new PairKeyValue(50128, "PagoManual"));
            lOperations.Add(new PairKeyValue(50310, "PaquetizacionSuperPack"));
            lOperations.Add(new PairKeyValue(50311, "PaquetizacionTrio"));
            lOperations.Add(new PairKeyValue(50247, "PlanTemporada"));
            lOperations.Add(new PairKeyValue(50248, "ProgramacionFreeAutoDown"));
            lOperations.Add(new PairKeyValue(50237, "ReauthorizeCustomerResources"));
            lOperations.Add(new PairKeyValue(50445, "ReauthorizeDevices"));
            lOperations.Add(new PairKeyValue(50345, "ReceiveReturnedPhysicalResource"));
            lOperations.Add(new PairKeyValue(50456, "ReconnectAgreementDetails"));
            lOperations.Add(new PairKeyValue(50238, "ReconnectCustomerProducts"));
            lOperations.Add(new PairKeyValue(50274, "ReconnectProduct"));
            lOperations.Add(new PairKeyValue(50337, "RemoveWorkOrderService"));
            lOperations.Add(new PairKeyValue(50323, "ReportingExecution"));
            lOperations.Add(new PairKeyValue(50155, "ReturnOrder"));
            lOperations.Add(new PairKeyValue(50470, "ReverseFinancialTransactions"));
            lOperations.Add(new PairKeyValue(50213, "ReverseLocalPayment"));
            lOperations.Add(new PairKeyValue(50220, "ReverseOffer"));
            lOperations.Add(new PairKeyValue(50111, "ReversePayment"));
            lOperations.Add(new PairKeyValue(50375, "ScheduleSendCommandToDevice"));
            lOperations.Add(new PairKeyValue(50449, "ScheduleUpgradeDowngradeAgreementDetail"));
            lOperations.Add(new PairKeyValue(50331, "SDSSolicitud"));
            lOperations.Add(new PairKeyValue(50332, "SDSUsuario"));
            lOperations.Add(new PairKeyValue(50280, "SearchDocumentNumber"));
            lOperations.Add(new PairKeyValue(50239, "SendCommand"));
            lOperations.Add(new PairKeyValue(50376, "SendCommandToDevice"));
            lOperations.Add(new PairKeyValue(50328, "SendMailService"));
            lOperations.Add(new PairKeyValue(50325, "SeriesAndreani"));
            lOperations.Add(new PairKeyValue(50293, "SolveError711"));
            lOperations.Add(new PairKeyValue(50166, "SwapServiceForCustomer"));
            lOperations.Add(new PairKeyValue(50167, "SwapServiceForCustomerArgentina"));
            lOperations.Add(new PairKeyValue(50158, "SwitchCustomerProductsFinanceOptions"));
            lOperations.Add(new PairKeyValue(50176, "TDTServiceForPostpaidCustomer"));
            lOperations.Add(new PairKeyValue(50394, "UpdateAddress"));
            lOperations.Add(new PairKeyValue(50455, "UpdateAgreementDetail"));
            lOperations.Add(new PairKeyValue(50465, "UpdateContact"));
            lOperations.Add(new PairKeyValue(50392, "UpdateCustomer"));
            lOperations.Add(new PairKeyValue(50401, "UpdateFinancialAccount"));
            lOperations.Add(new PairKeyValue(50403, "UpdateInvoice"));
            lOperations.Add(new PairKeyValue(50342, "UpdateResourceStatus"));
            lOperations.Add(new PairKeyValue(50275, "Upgrade"));
            lOperations.Add(new PairKeyValue(50448, "UpgradeDowngradeAgreementDetail"));
            lOperations.Add(new PairKeyValue(50428, "UpgradeDowngradeCustomerProduct"));
            lOperations.Add(new PairKeyValue(50302, "UpgradePremium"));
            lOperations.Add(new PairKeyValue(50159, "UpgradeServiceForPostpaidCustomer"));
            lOperations.Add(new PairKeyValue(50160, "UpgradeServiceForPostpaidCustomerArgentina"));
            lOperations.Add(new PairKeyValue(50168, "ValidateActivateAdditionalServiceEntitiesForPostpaidCustomer"));
            lOperations.Add(new PairKeyValue(50177, "ValidateActivateServiceEntities"));
            lOperations.Add(new PairKeyValue(50169, "ValidateActivateServiceEntitiesForPostpaidCustomer"));
            lOperations.Add(new PairKeyValue(50288, "ValidateCreditCardNumber"));
            lOperations.Add(new PairKeyValue(50214, "ValidateDirect"));
            lOperations.Add(new PairKeyValue(50170, "ValidateDowngradeServiceEntitiesForPostpaidCustomer"));
            lOperations.Add(new PairKeyValue(50171, "ValidateLinkServiceEntitiesForPostpaidCustomer"));
            lOperations.Add(new PairKeyValue(50215, "ValidateReverse"));
            lOperations.Add(new PairKeyValue(50216, "ValidateSerial"));
            lOperations.Add(new PairKeyValue(50172, "ValidateSwapServiceEntitiesForCustomer"));
            lOperations.Add(new PairKeyValue(50178, "ValidateTDTServiceEntitiesForPostpaidCustomer"));
            lOperations.Add(new PairKeyValue(50173, "ValidateUpgradeServiceEntitiesForPostpaidCustomer"));
            lOperations.Add(new PairKeyValue(50174, "ValidateUpgradeServiceEntitiesForPostpaidCustomerArgentina"));
            lOperations.Add(new PairKeyValue(50324, "VTOLServices"));
            lOperations.Add(new PairKeyValue(50334, "VTOLSettings"));
            lOperations.Add(new PairKeyValue(50330, "WsEclypse"));

        }

        private static void creaListaApps()
        {
            lApps = new List<PairKeyValue>();
            lApps.Add(new PairKeyValue(50025, "ABDNET"));
            lApps.Add(new PairKeyValue(50026, "Activaciones"));
            lApps.Add(new PairKeyValue(50027, "Ajuste Masivo de TF"));
            lApps.Add(new PairKeyValue(50028, "Asignador Unico"));
            lApps.Add(new PairKeyValue(50029, "Aspect Cobranza Argentina"));
            lApps.Add(new PairKeyValue(50030, "Blackberry"));
            lApps.Add(new PairKeyValue(50031, "COBRANZAS"));
            lApps.Add(new PairKeyValue(50032, "Cobro Online"));
            lApps.Add(new PairKeyValue(50033, "Consulta Saldo Prepago"));
            lApps.Add(new PairKeyValue(50034, "Control Fraude"));
            lApps.Add(new PairKeyValue(50035, "Control Ingreso Equipos"));
            lApps.Add(new PairKeyValue(50036, "CONTROLREMOTO"));
            lApps.Add(new PairKeyValue(50037, "Corporate Dealer"));
            lApps.Add(new PairKeyValue(50038, "CRM Engage"));
            lApps.Add(new PairKeyValue(50039, "Cuenta No Cliente"));
            lApps.Add(new PairKeyValue(50040, "Cupones de Pago"));
            lApps.Add(new PairKeyValue(50041, "Customer Mail Service"));
            lApps.Add(new PairKeyValue(50042, "Customer PPV"));
            lApps.Add(new PairKeyValue(50043, "DIRECNET"));
            lApps.Add(new PairKeyValue(50044, "DTVLAWS"));
            lApps.Add(new PairKeyValue(50045, "DTVWEB"));
            lApps.Add(new PairKeyValue(50046, "EASYGUIDE"));
            lApps.Add(new PairKeyValue(50047, "ENCUESTAS"));
            lApps.Add(new PairKeyValue(50048, "Envio de Comandos"));
            lApps.Add(new PairKeyValue(50049, "GOGREEN"));
            lApps.Add(new PairKeyValue(50015, "IBS"));
            lApps.Add(new PairKeyValue(50050, "iPad"));
            lApps.Add(new PairKeyValue(50051, "IVR"));
            lApps.Add(new PairKeyValue(50052, "L10L11"));
            lApps.Add(new PairKeyValue(50053, "LIM"));
            lApps.Add(new PairKeyValue(50054, "Liqnet"));
            lApps.Add(new PairKeyValue(50055, "MCU"));
            lApps.Add(new PairKeyValue(50056, "MDU"));
            lApps.Add(new PairKeyValue(50057, "MIDIRECTV"));
            lApps.Add(new PairKeyValue(50058, "NID"));
            lApps.Add(new PairKeyValue(50059, "PPV Pack"));
            lApps.Add(new PairKeyValue(50060, "PROMOCIONES"));
            lApps.Add(new PairKeyValue(50061, "Registracion de Usuarios"));
            lApps.Add(new PairKeyValue(50067, "SDS"));
            lApps.Add(new PairKeyValue(50004, "SDSNET"));
            lApps.Add(new PairKeyValue(50062, "SEMOS"));
            lApps.Add(new PairKeyValue(50014, "SGI"));
            lApps.Add(new PairKeyValue(50063, "Tarjeta de Credito"));
            lApps.Add(new PairKeyValue(50064, "Televentas"));
            lApps.Add(new PairKeyValue(50065, "WIMAX"));
            lApps.Add(new PairKeyValue(50066, "wsSCO"));
        }

        private static void creaListaServicios()
        {
            lServices = new List<PairKeyValue>();
            lServices.Add(new PairKeyValue(50068, "ActivateService-Service", "ActivateService_pt"));
            lServices.Add(new PairKeyValue(50069, "AntenaSenialSA", ""));
            lServices.Add(new PairKeyValue(50070, "AsignadorUnico-Service", ""));
            lServices.Add(new PairKeyValue(50357, "AuthenticationContext", "AuthenticationContext_ptt"));
            lServices.Add(new PairKeyValue(50016, "BestAddress-Service", ""));
            lServices.Add(new PairKeyValue(50012, "BillPaymentsAndReceivablesManagement", ""));
            lServices.Add(new PairKeyValue(50351, "CNetService", ""));
            lServices.Add(new PairKeyValue(50071, "CRM-Assurance - Resource", "ptResourceBinding"));
            lServices.Add(new PairKeyValue(50929, "CRM-Assurance - Product", "ptProduct_pt"));
            lServices.Add(new PairKeyValue(50930, "CRM-Assurance - Common Business", "ptCommonBusiness_pt"));
            lServices.Add(new PairKeyValue(50931, "CRM-Assurance - Vista360", "ptVista360"));
            lServices.Add(new PairKeyValue(50932, "CRM-Assurance - Customer", "ptCustomer_pt"));
            lServices.Add(new PairKeyValue(50352, "CRMService", ""));
            lServices.Add(new PairKeyValue(50005, "CRMSupportAndReadiness", "CRMSupportAndReadiness_pt"));
            lServices.Add(new PairKeyValue(50072, "CustomerInterfaceManagement", "CustomerInterfaceManagement_pt"));
            lServices.Add(new PairKeyValue(50073, "Exactarget-Service", ""));
            lServices.Add(new PairKeyValue(50074, "GenerarPDF-Service", ""));
            lServices.Add(new PairKeyValue(50075, "IBSConfigurationService", ""));
            lServices.Add(new PairKeyValue(50076, "IBSCRMService", ""));
            lServices.Add(new PairKeyValue(50077, "IBSCustomerService", "IBSCustomerServiceSoap"));
            lServices.Add(new PairKeyValue(50078, "IBSEngineeringService", ""));
            lServices.Add(new PairKeyValue(50079, "IBSFinanceService", "IBSFinanceServiceSoap"));
            lServices.Add(new PairKeyValue(50080, "IBSHistoryService", ""));
            lServices.Add(new PairKeyValue(50081, "IBSProductService", ""));
            lServices.Add(new PairKeyValue(50082, "IVRService", ""));
            lServices.Add(new PairKeyValue(50107, "LIM-Service", "LIMConfiguration_pt"));
            lServices.Add(new PairKeyValue(50083, "LIMUtilityService-Service", ""));
            lServices.Add(new PairKeyValue(50084, "LIQNETServices-Service", ""));
            lServices.Add(new PairKeyValue(50085, "ListenerAsignadorUnico", ""));
            lServices.Add(new PairKeyValue(50086, "ListenerPPVPack-Service", ""));
            lServices.Add(new PairKeyValue(50020, "ManageBillingEvents", "ManageBillingEvents_pt"));
            lServices.Add(new PairKeyValue(50087, "ManageWorkforce", "ManageWorkforce_pt"));
            lServices.Add(new PairKeyValue(50088, "Nosis", ""));
            lServices.Add(new PairKeyValue(50089, "OnlinePayment", ""));
            lServices.Add(new PairKeyValue(50353, "OptimusManageWorkforce", "OptimusManageWorkforcePort"));
            lServices.Add(new PairKeyValue(50021, "OrderHandling", "OrderHandling_pt"));
            lServices.Add(new PairKeyValue(50090, "PrepaidBalanceAllocator", ""));
            lServices.Add(new PairKeyValue(50091, "PrepaidBonification", ""));
            lServices.Add(new PairKeyValue(50092, "ReportingExecution-Service", ""));
            lServices.Add(new PairKeyValue(50093, "ResourceProvisioning", "ResourceProvisioning_pt"));
            lServices.Add(new PairKeyValue(50094, "SDSSolicitud-Service", ""));
            lServices.Add(new PairKeyValue(50095, "SDSUsuario-Service", ""));
            lServices.Add(new PairKeyValue(50096, "Selling", ""));
            lServices.Add(new PairKeyValue(50097, "SendMailService-Service", ""));
            lServices.Add(new PairKeyValue(50098, "SeriesAndreani-Service", ""));
            lServices.Add(new PairKeyValue(50099, "ServiceConfigurationAndActivation", "ServiceConfigurationAndActivation_pt"));
            lServices.Add(new PairKeyValue(50100, "ServiceConfigurationRules", ""));
            lServices.Add(new PairKeyValue(50101, "ServiceProblemManagement", ""));
            lServices.Add(new PairKeyValue(50102, "SPRMSupportAndReadiness", ""));
            lServices.Add(new PairKeyValue(50103, "Utility", "utility_pt"));
            lServices.Add(new PairKeyValue(50348, "Vista360-Service", ""));
            lServices.Add(new PairKeyValue(50104, "VTOLServices-Service", ""));
            lServices.Add(new PairKeyValue(50105, "VTOLSettings-Service", ""));
            lServices.Add(new PairKeyValue(50106, "WsEclypse-Service", ""));
            lServices.Add(new PairKeyValue(50354, "WSIBSService", ""));
            lServices.Add(new PairKeyValue(50359, "IBS-ASM-CustomersService", "ICustomersService"));
            lServices.Add(new PairKeyValue(50360, "IBS-ASM-ViewFacadeService", "IViewFacadeService"));
            lServices.Add(new PairKeyValue(50361, "IBS-ASM-HistoryService", "IHistoryService"));
            lServices.Add(new PairKeyValue(50362, "IBS-ASM-AgreementManagementService", "IAgreementManagementService"));
            lServices.Add(new PairKeyValue(50363, "IBS-ASM-DevicesService", "IDevicesService"));
            lServices.Add(new PairKeyValue(50364, "IBS-ASM-ScheduleManagerService", "IScheduleManagerService"));
            lServices.Add(new PairKeyValue(50365, "IBS-ASM-OfferManagementConfigurationService", "IOfferManagementConfigurationService"));
            lServices.Add(new PairKeyValue(50366, "IBS-ASM-SandBoxManagerService", "ISandBoxManagerService"));
            lServices.Add(new PairKeyValue(50399, "IBS-ASM-FinanceService", "IFinanceService"));
            lServices.Add(new PairKeyValue(50408, "IBS-ASM-ProductCatalogConfigurationService", "IProductCatalogConfigurationService"));
            lServices.Add(new PairKeyValue(50409, "IBS-ASM-AgreementManagementConfigurationService", "IAgreementManagementConfigurationService"));
            lServices.Add(new PairKeyValue(50410, "IBS-ASM-ProductCatalogService", "IProductCatalogService"));
            lServices.Add(new PairKeyValue(50411, "IBS-ASM-OrderableEventService", "IOrderableEventService"));
            lServices.Add(new PairKeyValue(50412, "IBS-ASM-PricingService", "IPricingService"));
            lServices.Add(new PairKeyValue(50413, "IBS-ASM-BizFacadeService", "IBizFacadeService"));
            lServices.Add(new PairKeyValue(50414, "IBS-ASM-SystemEventsConfigurationService", "ISystemEventsConfigurationService"));
            lServices.Add(new PairKeyValue(50415, "IBS-ASM-CommunicationLogService", "ICommunicationLogService"));
            lServices.Add(new PairKeyValue(50416, "IBS-ASM-WorkforceService", "IWorkforceService"));
            lServices.Add(new PairKeyValue(50450, "IBS-ASM-OfferManagementService", "IOfferManagementService"));
            lServices.Add(new PairKeyValue(50461, "IBS-ASM-UsersConfigurationService", "IUsersConfigurationService"));
            lServices.Add(new PairKeyValue(50462, "IBS-ASM-ContactsService", "IContactsService"));
            lServices.Add(new PairKeyValue(50472, "IBS-ASM-LogisticsConfigurationService", "ILogisticsConfigurationService"));
            lServices.Add(new PairKeyValue(50473, "IBS-ASM-DistributorsService", "IDistributorsService"));
            lServices.Add(new PairKeyValue(50474, "IBS-ASM-LogisticsService", "ILogisticsService"));
            lServices.Add(new PairKeyValue(50486, "IBS-ASM-AuthenticationService", "IAuthenticationService"));
            lServices.Add(new PairKeyValue(50488, "IBS-ASM-OrderManagementService", "IOrderManagementService"));
            lServices.Add(new PairKeyValue(50506, "RESB-ResourceRules", "ResourceRules_pt"));
            lServices.Add(new PairKeyValue(50516, "RetentionAndLoyalty", ""));
            lServices.Add(new PairKeyValue(50518, "RESB-WorkForceRules", "WorkForceRules_pt"));
            lServices.Add(new PairKeyValue(50519, "IBS-ASM-WorkforceConfigurationService", "IWorkforceConfigurationService"));
            lServices.Add(new PairKeyValue(50520, "IBS-ASM-OrderableEventConfigurationService", "IOrderableEventConfigurationService"));
            lServices.Add(new PairKeyValue(50521, "IBS-ASM-PriceAdjustmentConfigurationService", "IPriceAdjustmentConfigurationService"));
            lServices.Add(new PairKeyValue(50557, "RESB-ServiceConfigurationRules", "ServiceConfigurationRules_pt"));
            lServices.Add(new PairKeyValue(50579, "IBS-ASM-DateFormulasConfigurationService", "IDateFormulasConfigurationService"));
            lServices.Add(new PairKeyValue(50580, "IBS-ASM-InvoiceRunConfigurationService", "IInvoiceRunConfigurationService"));
            lServices.Add(new PairKeyValue(50581, "IBS-ASM-DateFormulasService", "IDateFormulasService"));
            lServices.Add(new PairKeyValue(50582, "IBS-ASM-FinanceConfigurationService", "IFinanceConfigurationService"));
            lServices.Add(new PairKeyValue(50596, "Selling-AddCustomerProductsWithCustomerOffers", "AddCustomerProductsWithCustomerOffers_Pt"));
            lServices.Add(new PairKeyValue(50707, "RESB-SolveError711Rules", "SolveError711Rules_pt"));
            lServices.Add(new PairKeyValue(50711, "TVAppsInterface", "TVAppsService"));
            lServices.Add(new PairKeyValue(50717, "ITESBInterface", ""));
            lServices.Add(new PairKeyValue(50744, "IBS-ASM-MarketSegmentsConfigurationService", "IMarketSegmentsConfigurationService"));
            lServices.Add(new PairKeyValue(50787, "Optimus-ServicioFrontera", "servicioFronteraPortType"));
            lServices.Add(new PairKeyValue(50801, "EventListenerRESB-Service", ""));
            lServices.Add(new PairKeyValue(50806, "ActivateService-Service-CL", "ActivateService_pt-CL"));
            lServices.Add(new PairKeyValue(50807, "ActivateService-Service-AR", "ActivateService_pt-AR"));
            lServices.Add(new PairKeyValue(50808, "ActivateService-Service-CO", "ActivateService_pt-CO"));
            lServices.Add(new PairKeyValue(50809, "ActivateService-Service-EC", "ActivateService_pt-EC"));
            lServices.Add(new PairKeyValue(50810, "ActivateService-Service-PE", "ActivateService_pt-PE"));
            lServices.Add(new PairKeyValue(50811, "ActivateService-Service-VE", "ActivateService_pt-VE"));
            lServices.Add(new PairKeyValue(50744, "IBS-ASM-CustomersConfigurationService", "ICustomersConfigurationService"));
        }

        private static void creaApps()
        {
            var apps = new List<string>();
            #region Arma lista
            apps.Add("ABDNET");
            apps.Add("Activaciones");
            apps.Add("Ajuste Masivo de TF");
            apps.Add("Asignador Unico");
            apps.Add("Aspect Cobranza Argentina");
            apps.Add("Blackberry");
            apps.Add("COBRANZAS");
            apps.Add("Cobro Online");
            apps.Add("Consulta Saldo Prepago");
            apps.Add("Control Fraude");
            apps.Add("Control Ingreso Equipos");
            apps.Add("CONTROLREMOTO");
            apps.Add("Corporate Dealer");
            apps.Add("CRM Engage");
            apps.Add("Cuenta No Cliente");
            apps.Add("Cupones de Pago");
            apps.Add("Customer Mail Service");
            apps.Add("Customer PPV");
            apps.Add("DIRECNET");
            apps.Add("DTVLAWS");
            apps.Add("DTVWEB");
            apps.Add("EASYGUIDE");
            apps.Add("ENCUESTAS");
            apps.Add("Envio de Comandos");
            apps.Add("GOGREEN");
            apps.Add("iPad");
            apps.Add("IVR");
            apps.Add("L10L11");
            apps.Add("LIM");
            apps.Add("Liqnet");
            apps.Add("MCU");
            apps.Add("MDU");
            apps.Add("MIDIRECTV");
            apps.Add("NID");
            apps.Add("PPV Pack");
            apps.Add("PROMOCIONES");
            apps.Add("Registracion de Usuarios");
            apps.Add("SDS");
            //apps.Add("SDSNET");
            apps.Add("SEMOS");
            //apps.Add("SGI");
            apps.Add("Tarjeta de Crédito");
            apps.Add("Televentas");
            apps.Add("WIMAX");
            apps.Add("wsSCO");
            #endregion

            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "apps.log");
            var swE = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "appserror.log");
            sw.WriteLine("Id\tApp");
            foreach (var item in apps)
            {
                try
                {
                    var asset = ws.assetCreate(token, item, "1.0", (int)tipoAsset.Application);
                    sw.WriteLine("{0}\t{1}", asset.ID, item);
                    Console.WriteLine("{0}\t{1}", asset.ID, item);
                }
                catch (Exception ex)
                {
                    swE.WriteLine("{0}\t{1}", item, ex.Message);
                    Console.WriteLine("ERROR: {0}\t{1}", item, ex.Message);
                }
            }
            sw.Close();
            swE.Close();
        }

        private static void creaServicios()
        {
            var servs = new List<string>();

            #region Arma lista
            servs.Add("ActivateService");
            servs.Add("AntenaSenialSA");
            servs.Add("AsignadorUnico");
            servs.Add("CRM-Assurance - Resource");
            servs.Add("CustomerInterfaceManagement");
            servs.Add("Exactarget");
            servs.Add("GenerarPDF");
            servs.Add("IBSConfigurationService");
            servs.Add("IBSCRMService");
            servs.Add("IBSCustomerService");
            servs.Add("IBSEngineeringService");
            servs.Add("IBSFinanceService");
            servs.Add("IBSHistoryService");
            servs.Add("IBSProductService");
            servs.Add("IVRService");
            servs.Add("LIM");
            servs.Add("LIMUtilityService");
            servs.Add("LIQNETServices");
            servs.Add("ListenerAsignadorUnico");
            servs.Add("ListenerPPVPack");
            servs.Add("ManageWorkforce");
            servs.Add("Nosis");
            servs.Add("OnlinePayment");
            servs.Add("PrepaidBalanceAllocator");
            servs.Add("PrepaidBonification");
            servs.Add("ReportingExecution");
            servs.Add("ResourceProvisioning");
            servs.Add("SDSSolicitud");
            servs.Add("SDSUsuario");
            servs.Add("Selling");
            servs.Add("SendMailService");
            servs.Add("SeriesAndreani");
            servs.Add("ServiceConfigurationAndActivation");
            servs.Add("ServiceConfigurationRules");
            servs.Add("ServiceProblemManagement");
            servs.Add("SPRMSupportAndReadiness");
            servs.Add("Utility");
            servs.Add("VTOLServices");
            servs.Add("VTOLSettings");
            servs.Add("WsEclypse");
            #endregion

            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "servs.log");
            var swE = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "servserror.log");
            sw.WriteLine("Id\tService");
            foreach (var item in servs)
            {
                try
                {
                    var asset = ws.assetCreate(token, item, "1.0", (int)tipoAsset.Service);
                    sw.WriteLine("{0}\t{1}", asset.ID, item);
                    Console.WriteLine("{0}\t{1}", asset.ID, item);
                }
                catch (Exception ex)
                {
                    swE.WriteLine("{0}\t{1}", item, ex.Message);
                    Console.WriteLine("ERROR: {0}\t{1}", item, ex.Message);
                }
            }
            sw.Close();
            swE.Close();
        }

        private static void creaListaAppsArch()
        {
            lApps = new List<PairKeyValue>();

            Console.WriteLine("APLICACIONES");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "aplicaciones.csv");

            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.assetTypeCriteria = (int)tipoAsset.Application;
            var serviceQ = ws.assetQuerySummary(token, criteria);
            foreach (var item in serviceQ)
            {
                lApps.Add(new PairKeyValue(item.ID, item.name));
                sw.WriteLine("{0}\t{1}", item.ID, item.name);
                Console.WriteLine("{0}\t{1}", item.ID, item.name);
            }
            sw.Close();
        }

        private static void creaListaServiciosArch()
        {
            lServices = new List<PairKeyValue>();

            Console.WriteLine("SERVICIOS");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "servicios.csv", false);
            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.assetTypeCriteria = (int)tipoAsset.Service;
            var serviceQ = ws.assetQuerySummary(token, criteria);
            foreach (var item in serviceQ)
            {
                lServices.Add(new PairKeyValue(item.ID, item.name));
                sw.WriteLine("lServices.Add(new PairKeyValue({0},\"{1}\", \"\"));", item.ID, item.name);
                Console.WriteLine("{0}\t{1}", item.ID, item.name);
            }
            sw.Close();
        }

        private static void creaListaOperacionesArch()
        {
            lOperations = new List<PairKeyValue>();

            Console.WriteLine("OPERACIONES");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "operaciones.csv", false);
            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.assetTypeCriteria = (int)tipoAsset.Operation;
            var serviceQ = ws.assetQuerySummary(token, criteria);
            foreach (var item in serviceQ)
            {
                sw.WriteLine("lOperations.Add(new PairKeyValue({0},\"{1}\"));", item.ID, item.name);
                Console.WriteLine("{0}\t{1}", item.ID, item.name);
            }
            sw.Close();
        }

    }

    class PairKeyValue
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }

        public PairKeyValue(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public PairKeyValue(long id, string name, string key)
        {
            this.Id = id;
            this.Name = name;
            this.Key = key;
        }
    }
}
