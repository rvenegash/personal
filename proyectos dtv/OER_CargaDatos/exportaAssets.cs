using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace OER_CargaDatos
{
    public class exportaAssets
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
        static List<Operacion> operaciones;
        static List<Servicio> servicios;
        static List<Functionality> funcionalidades;

        public static void MainExportaAssets() //
        {

            Console.WriteLine("Creando servicio...");
            ws = new OerProd.FlashlineRegistryService();

            Console.WriteLine("Autenticando");
            token = ws.authTokenCreate(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);

            //creaListaAppsArch();

            //creaListaOperArch();

            creaListaServArch();

            //exportaUsoAssets();

        }

        private static void creaListaServArch()
        {
            Console.WriteLine("SERVICIOS");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "servicios.csv");

            servicios = new List<Servicio>();
            operaciones = new List<Operacion>();
            funcionalidades = new List<Functionality>();

            var criteriaApp = new OER_CargaDatos.OerProd.AssetCriteria();
            criteriaApp.assetTypeCriteria = (int)tipoAsset.Service;
            var appL = ws.assetQuerySummary(token, criteriaApp);
            foreach (var serv in appL)
            {
                Console.WriteLine("{0}\t{1}", serv.ID, serv.name);
                sw.WriteLine("{0}\t{1}", serv.ID, serv.name);

            }
            Console.WriteLine("FIN !!");
            sw.Close();
        }

        private static void creaListaAppsArch()
        {
            Console.WriteLine("APLICACIONES");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "aplicaciones.csv");

            operaciones = new List<Operacion>();

            var criteriaApp = new OER_CargaDatos.OerProd.AssetCriteria();
            criteriaApp.assetTypeCriteria = (int)tipoAsset.Application;
            var appL = ws.assetQuerySummary(token, criteriaApp);
            foreach (var app in appL)
            {
                //sw.WriteLine("{0}\t{1}", item.ID, item.name);
                //Console.WriteLine("{0}\t{1}", app.ID, app.name);

                var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
                criteria.assetTypeCriteria = (int)tipoAsset.Application;
                criteria.IDCriteria = app.ID;
                var serviceQ = ws.assetQuery(token, criteria);
                foreach (var itemA in serviceQ)
                {
                    foreach (var rel in itemA.relationshipTypes)
                    {
                        if (rel.ID == (int)tipoRelation.Invoked)
                        {
                            foreach (var itemSec in rel.primaryIDs)
                            {
                                var operacion = getOperacion(itemSec, true);

                                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", app.ID, app.name, operacion.ID, operacion.Nombre, operacion.Servicio);
                                sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", app.ID, app.name, operacion.ID, operacion.Nombre, operacion.Servicio);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("FIN !!");
            sw.Close();
        }

        private static void creaListaOperArch()
        {
            Console.WriteLine("OPERACIONES");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "operaciones.csv");

            servicios = new List<Servicio>();
            operaciones = new List<Operacion>();
            funcionalidades = new List<Functionality>();

            var criteriaApp = new OER_CargaDatos.OerProd.AssetCriteria();
            criteriaApp.assetTypeCriteria = (int)tipoAsset.Operation;
            var appL = ws.assetQuerySummary(token, criteriaApp);
            foreach (var oper in appL)
            {
                //sw.WriteLine("{0}\t{1}", item.ID, item.name);
                //Console.WriteLine("{0}\t{1}", app.ID, app.name);

                var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
                criteria.IDCriteria = oper.ID;
                var serviceQ = ws.assetQuery(token, criteria);
                foreach (var itemA in serviceQ)
                {
                    var servicio = new Servicio(0, "", null);
                    var functionality = new Functionality(0, "", null);
                    foreach (var rel in itemA.relationshipTypes)
                    {
                        if (rel.ID == (int)tipoRelation.Contains)
                        {
                            foreach (var itemSec in rel.primaryIDs)
                            {
                                servicio = getServicio(itemSec);

                            }
                        }
                        if (rel.ID == (int)tipoRelation.Implements)
                        {
                            foreach (var itemSec in rel.secondaryIDs)
                            {
                                functionality = getFunctionality(itemSec);
                            }
                        }
                    }
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", oper.ID, oper.name, servicio.ID, servicio.Nombre, functionality.Nombre);
                    sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", oper.ID, oper.name, servicio.ID, servicio.Nombre, functionality.Nombre);
                }
            }
            Console.WriteLine("FIN !!");
            sw.Close();
        }

        private static void exportaUsoAssets()
        {
            Console.WriteLine("OPERACIONES INVOC");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "operacionesInvoc.csv");

            servicios = new List<Servicio>();
            operaciones = new List<Operacion>();

            var criteriaApp = new OER_CargaDatos.OerProd.AssetCriteria();
            criteriaApp.assetTypeCriteria = (int)tipoAsset.Operation;
            var appL = ws.assetQuerySummary(token, criteriaApp);
            foreach (var oper in appL)
            {
                var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
                criteria.IDCriteria = oper.ID;
                var serviceQ = ws.assetQuery(token, criteria);
                foreach (var itemA in serviceQ)
                {
                    var cantInvoc = 0;
                    Servicio servicio = new Servicio(0, "", null);
                    foreach (var rel in itemA.relationshipTypes)
                    {
                        if (rel.ID == (int)tipoRelation.Invoked)
                        {
                            cantInvoc += rel.secondaryIDs.Count();
                        }
                        if (rel.ID == (int)tipoRelation.Contains)
                        {
                            foreach (var itemSec in rel.primaryIDs)
                            {
                                servicio = getServicio(itemSec);
                            }
                        }
                    }
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", oper.ID, oper.name, servicio.ID, servicio.Nombre, cantInvoc);
                    sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", oper.ID, oper.name, servicio.ID, servicio.Nombre, cantInvoc);
                }
            }
            Console.WriteLine("FIN !!");
            sw.Close();
        }

        private static Operacion getOperacion(long operId, bool relations)
        {
            var operL = operaciones.Where(m => m.ID == operId);
            if (operL.Count() == 0)
            {
                var criteriaRel = new OER_CargaDatos.OerProd.AssetCriteria();
                criteriaRel.IDCriteria = operId;
                var appRel = ws.assetQuery(token, criteriaRel);
                var nombre = "";
                var servicio = "";
                foreach (var oper in appRel)
                {
                    nombre = oper.name;

                    if (relations)
                    {
                        foreach (var rel in oper.relationshipTypes)
                        {
                            if (rel.ID == (int)tipoRelation.Contains)
                            {
                                foreach (var itemPri in rel.primaryIDs)
                                {
                                    var criteriaRelC = new OER_CargaDatos.OerProd.AssetCriteria();
                                    criteriaRelC.IDCriteria = itemPri;
                                    var appRelC = ws.assetQuerySummary(token, criteriaRelC);
                                    if (appRelC.Count() > 0)
                                    {
                                        servicio = appRelC.First().name;
                                    }
                                }
                            }
                        }
                    }
                    var operacion = new Operacion(operId, nombre, servicio, oper);
                    operaciones.Add(operacion);

                    return operacion;
                }
                return null;
            }
            else
            {
                return operL.First();
            }
        }

        private static Servicio getServicio(long serId)
        {
            var operL = servicios.Where(m => m.ID == serId);
            if (operL.Count() == 0)
            {
                var criteriaRel = new OER_CargaDatos.OerProd.AssetCriteria();
                criteriaRel.IDCriteria = serId;
                var appRel = ws.assetQuery(token, criteriaRel);
                var nombre = "";
                foreach (var oper in appRel)
                {
                    nombre = oper.name;

                    var servicio = new Servicio(serId, nombre, oper);
                    servicios.Add(servicio);

                    return servicio;
                }
                return null;
            }
            else
            {
                return operL.First();
            }
        }

        private static Functionality getFunctionality(long serId)
        {
            var operL = funcionalidades.Where(m => m.ID == serId);
            if (operL.Count() == 0)
            {
                var criteriaRel = new OER_CargaDatos.OerProd.AssetCriteria();
                criteriaRel.IDCriteria = serId;
                var appRel = ws.assetQuery(token, criteriaRel);
                var nombre = "";
                foreach (var oper in appRel)
                {
                    nombre = oper.name;

                    var servicio = new Functionality(serId, nombre, oper);
                    funcionalidades.Add(servicio);

                    return servicio;
                }
                return null;
            }
            else
            {
                return operL.First();
            }
        }
    }

    class Operacion
    {
        public Operacion(long id, string nombre, string servicio, OerProd.Asset asset)
        {
            this.ID = id;
            this.Nombre = nombre;
            this.Servicio = servicio;
            this.Asset = asset;
        }
        public long ID { get; set; }
        public string Nombre { get; set; }
        public string Servicio { get; set; }
        public OerProd.Asset Asset { get; set; }
    }

    class Servicio
    {
        public Servicio(long id, string nombre, OerProd.Asset asset)
        {
            this.ID = id;
            this.Nombre = nombre;
            this.Asset = asset;
        }
        public long ID { get; set; }
        public string Nombre { get; set; }
        public OerProd.Asset Asset { get; set; }
    }

    class Functionality
    {
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
