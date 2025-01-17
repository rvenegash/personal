using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;

namespace revisaProyectosSCA_V2
{
    public class generaReportes
    {
        public void GeneraReportes()
        {
            var mysqldb = ConfigurationManager.AppSettings["MySQL160"];
            MySqlConnection conn = new MySqlConnection(mysqldb);
            conn.Open();

            //crear archivos
            Console.WriteLine("Exportando Asset a Crear");
            var nombre_arch = string.Format("Asset-A-Crear.txt");
            StreamWriter arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            using (var cmd = new MySqlCommand("set net_write_timeout=99999; set net_read_timeout=99999", conn))
            {
                cmd.ExecuteNonQuery();
            }

            //ejecutar sql
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select * from operations o where o.ASSET_ID is null ";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                arch.WriteLine("Nombre\tServicio\tBpel\tCarpeta\tPortType\tRealName");
                while (reader.Read())
                {
                    arch.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", reader["name"].ToString(), reader["service_name"].ToString(), reader["bpel"].ToString(), reader["folder_name"].ToString(), reader["port_type"].ToString(), reader["real_name"].ToString());
                }
                reader.Close();
                reader = null;
            }
            //cerrar archivo
            arch.Close();


            Console.WriteLine("Exportando relacion en OER, no en GIT");
            nombre_arch = string.Format("En-OER-no-en-GIT.txt");
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            //ejecutar sql
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select a2.id, a2.name, at.NAME as type_name, a2.real_name real_name_a2, a.id as id1, a.name as name1, " +
                    "   a.REAL_NAME as REAL_NAME1, at2.NAME as type_name2, ar.asset_id, ar.related_asset_id " +
                    "from asset_relation ar inner join asset a on ar.RELATED_ASSET_ID = a.id " +
                    "inner join asset a2 on ar.ASSET_ID = a2.id " +
                    "inner join asset_type at on a2.type = at.id  " +
                    "inner join asset_type at2 on  a.type = at2.id " +
                    "where  ar.RELATION_TYPE in ( 50000, 50001 ) and ar.RELATED_PS = 'P' and at.id in (145, 50004) and a2.name not like '%BUSDTV%' and " +
                    "a.id not in ( " +
                    "   select opr.INVOKED_ASSET_ID from operations_rel opr inner join operations o on opr.real_name = o.real_name where o.ASSET_ID = ar.ASSET_id ) ";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();

                arch.WriteLine("Id\tNombre\tRealName\tType\tId2\tName2\tRealName2\tType2\trelated_asset_id");

                //grabar resultado en archivo
                while (reader.Read())
                {
                    arch.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}", reader["id"].ToString(), reader["name"].ToString(),
                        reader["real_name_a2"].ToString(), reader["type_name"].ToString(), reader["id1"].ToString(),
                        reader["name1"].ToString(), reader["REAL_NAME1"].ToString(), reader["type_name"].ToString(),
                        reader["type_name2"].ToString());
                }
                reader.Close();
                reader = null;
            }
            //cerrar archivo
            arch.Close();



            Console.WriteLine("Exportando aplicaciones sin referencias a los servicios de las operaciones que invocan");
            nombre_arch = string.Format("APP-Sin-Referencias-A-Servicios.txt");
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            //ejecutar sql
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select a.id, a.name, ad.id id_oper, ad.name oper_name, ad2.id id_service, ad2.name service_name " +
                    "from asset a " +
                    "inner join asset_relation ar on ar.ASSET_ID = a.id " +
                    "inner join asset ad on ad.id = ar.RELATED_ASSET_ID " +
                    "inner join asset_relation ar2 on ar2.ASSET_ID = ad.id " +
                    "inner join asset ad2 on ad2.id = ar2.RELATED_ASSET_ID " +
                    "where a.type in (158) and " +
                    "ar.RELATION_TYPE = 50000 and ar.RELATED_PS = 'P' and " +
                    "ar2.RELATION_TYPE = 108  and ar2.RELATED_PS = 'P' and not exists " +
                    "(select 1 " +
                    "from asset_relation ar3 " +
                    "where  ar3.ASSET_ID = a.id and ar3.RELATED_ASSET_ID = ad2.id and " +
                    "  ar3.RELATION_TYPE = 118 and ar3.RELATED_PS = 'S')" +
                    "order by 1;";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                arch.WriteLine("AssetId\tNombre\tIdOper\tOperName\tIdService\tServiceInvoked");
                while (reader.Read())
                {
                    arch.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", reader["id"].ToString(), reader["name"].ToString(),
                        reader["id_oper"].ToString(), reader["oper_name"].ToString(),
                        reader["id_service"].ToString(), reader["service_name"].ToString());
                }
                reader.Close();
                reader = null;
            }
            //cerrar archivo
            arch.Close();


            Console.WriteLine("Exportando aplicaciones con referencias a servicios y que no invoca a sus operaciones");
            nombre_arch = string.Format("APP-Con-Referencias-Sobrantes.txt");
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            //ejecutar sql
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select a.id, a.name, ase.id id_service, ase.name service_name " +
                    "from asset a " +
                    "inner join asset_relation ar3 on ar3.ASSET_ID = a.id " +
                    "inner join asset ase on ase.id = ar3.RELATED_ASSET_ID " +
                    "where a.type in (158) and  ar3.RELATION_TYPE = 118 and ar3.RELATED_PS = 'S' " +
                    "and not exists " +
                    "(select 1 " +
                    "from asset_relation ar " +
                    "inner join asset_relation ar2 on ar2.ASSET_ID = ar.RELATED_ASSET_ID " +
                    "where ar.ASSET_ID = a.id and ar.RELATION_TYPE = 50000 and ar.RELATED_PS = 'P' and ar2.RELATION_TYPE = 108 and ar2.RELATED_PS = 'P' and  " +
                    " ar2.RELATED_ASSET_ID = ase.id)" +
                    "order by 1;";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                arch.WriteLine("AssetId\tNombre\tIdService\tServiceInvoked");
                while (reader.Read())
                {
                    arch.WriteLine("{0}\t{1}\t{2}\t{3}", reader["id"].ToString(), reader["name"].ToString(),
                        reader["id_service"].ToString(), reader["service_name"].ToString());
                }
                reader.Close();
                reader = null;
            }
            //cerrar archivo
            arch.Close();

            Console.WriteLine("Exportando relacion en GIT, no en OER");
            nombre_arch = string.Format("En-GIT-no-en-OER.txt");
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            //ejecutar sql
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select o.ASSET_ID, opr.*,  o.REAL_NAME " +
                    "from operations_rel opr inner join operations o on opr.operation_id = o.operation_id " +
                    "where opr.INVOKED_ASSET_ID not in ( " +
                    "select ar.RELATED_ASSET_ID from asset_relation ar  " +
                    "where ar.ASSET_id =  o.ASSET_ID and ar.RELATION_TYPE in ( 50000, 50001 ) ) or opr.INVOKED_ASSET_ID is null " +
                    "order by 2,1";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                arch.WriteLine("AssetId\tNombre\tServiceName\tRealName\tOperationInvoked\tServiceInvoked\tInvokedAssetId\tInvokedPortType");
                while (reader.Read())
                {
                    arch.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", reader["asset_id"].ToString(), reader["operation_name"].ToString(),
                        reader["service_name"].ToString(), reader["REAL_NAME"].ToString(),
                        reader["operation_invoked"].ToString(), reader["service_invoked"].ToString(), reader["INVOKED_ASSET_ID"].ToString(),
                        reader["invoked_port_type"].ToString(), reader["port_type"].ToString());
                }
                reader.Close();
                reader = null;
            }
            //cerrar archivo
            arch.Close();

            Console.WriteLine("Exportando relacion en GIT, no en OER de colas");
            nombre_arch = string.Format("En-GIT-no-en-OER-COLAS.txt");
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            //ejecutar sql
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT o.asset_id, od.*  " +
                    "from operations_dequeue od inner join operations o on o.operation_id = od.operation_id and not exists " +
                    "(select 1 from asset_relation ar " +
                    "inner join asset a on a.id = ar.ASSET_ID  " +
                    "where a.id = o.asset_id and ar.RELATION_TYPE in ( 50100) and ar.related_ps = 'P')  " +
                    "order by 2,1";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                arch.WriteLine("AssetId\tNombre\tServiceName\tRealName\tQueueInvoked\tQueueType\tInvokedAssetId");
                while (reader.Read())
                {
                    arch.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", reader["asset_id"].ToString(), reader["operation_name"].ToString(),
                        reader["service_name"].ToString(), reader["REAL_NAME"].ToString(),
                        reader["queue_invoked"].ToString(), reader["queue_type"].ToString(), reader["INVOKED_ASSET_ID"].ToString()
                        );
                }
                reader.Close();
                reader = null;
            }
            //cerrar archivo
            arch.Close();
            conn.Clone();
        }
    }
}
