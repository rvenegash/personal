using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace revisaProyectosSCA_V2
{
    public static class dataHelper
    {
        static MySqlConnection conn;

        public static void buildConn()
        {
            var mysqldb = ConfigurationManager.AppSettings["MySQL160"];
            conn = new MySqlConnection(mysqldb);
            conn.Open();
        }

        public static void grabarService(string service_name, string port_type, string ruta)
        {
            port_type = fixServiceName(service_name, port_type, ruta);

            try
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "insert into services (name, fecha, port_type) values (@service_name, now(), @port_type) ";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.Parameters.Add(new MySqlParameter("service_name", service_name));
                    cmd.Parameters.Add(new MySqlParameter("port_type", port_type));

                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }
        }

        public static void borraTodoBD()
        {
            try
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from operations_dequeue";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from operations_rel";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from operations";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from services";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }
        }

        public static void grabaRelacion(long operation_id, string operationName, string serviceName, string portType, string operationInvoked, string serviceIvoked, string portTypeInv, string folderName, string bpelName)
        {
            try
            {
                if (operationName == null)
                {
                    return;
                }
                portType = fixServiceName(serviceName, portType, folderName);

                var cont = false;
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select count(*) from operations_rel where operation_name = @operation and service_name = @service and " +
                    "operation_invoked = @operation_invoked and service_invoked = @service_invoked and invoked_port_type = @invoked_port_type and real_name = @real_name and " +
                    "operation_id = @operation_id";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.Parameters.Add(new MySqlParameter("operation", operationName));
                    cmd.Parameters.Add(new MySqlParameter("service", serviceName));
                    cmd.Parameters.Add(new MySqlParameter("operation_invoked", operationInvoked));
                    cmd.Parameters.Add(new MySqlParameter("service_invoked", serviceIvoked));
                    portTypeInv = portTypeInv.Contains(":") ? portTypeInv.Remove(0, portTypeInv.IndexOf(":") + 1) : portTypeInv;
                    cmd.Parameters.Add(new MySqlParameter("invoked_port_type", fixPortType(operationName, serviceName, "", folderName, portType)));
                    portType = portType.Contains(":") ? portType.Remove(0, portType.IndexOf(":") + 1) : portType;
                    cmd.Parameters.Add(new MySqlParameter("port_type", portType));
                    cmd.Parameters.Add(new MySqlParameter("real_name", fixPortType(operationName, serviceName, bpelName, folderName, portType)));
                    cmd.Parameters.Add(new MySqlParameter("operation_id", operation_id));

                    var respS = cmd.ExecuteScalar();
                    cont = (Convert.ToInt32(respS) == 0);
                }
                if (cont)
                {
                    portTypeInv = fixServiceNameInv(serviceIvoked, portTypeInv);

                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "insert into operations_rel (operation_name, service_name, operation_invoked, service_invoked, fecha, invoked_port_type, port_type, real_name, operation_id) values " +
                        "(@operation, @service, @operation_invoked, @service_invoked, now(), @invoked_port_type, @port_type, @real_name, @operation_id ) ";
                        cmd.CommandType = System.Data.CommandType.Text;

                        cmd.Parameters.Add(new MySqlParameter("operation", operationName));
                        cmd.Parameters.Add(new MySqlParameter("service", serviceName));
                        portType = portType.Contains(":") ? portType.Remove(0, portType.IndexOf(":") + 1) : portType;
                        cmd.Parameters.Add(new MySqlParameter("port_type", portType));
                        cmd.Parameters.Add(new MySqlParameter("operation_invoked", operationInvoked));
                        cmd.Parameters.Add(new MySqlParameter("service_invoked", serviceIvoked));
                        portTypeInv = portTypeInv.Contains(":") ? portTypeInv.Remove(0, portTypeInv.IndexOf(":") + 1) : portTypeInv;
                        cmd.Parameters.Add(new MySqlParameter("invoked_port_type", portTypeInv));
                        cmd.Parameters.Add(new MySqlParameter("real_name", fixPortType(operationName, serviceName, bpelName, folderName, portType)));
                        cmd.Parameters.Add(new MySqlParameter("operation_id", operation_id));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static long grabaOperacion(string operationName, string serviceName, string bpel, string folderName, string portType, long operation_id, string service_name)
        {
            long id_ret = 0;
            using (var cmd = new MySqlCommand())
            {
                portType = fixServiceName(service_name, portType, folderName);
                cmd.Connection = conn;
                cmd.CommandText = "insert into operations (name, service_name, bpel, folder_name, port_type, fecha, real_name) values (@operation, @service, @bpel, @folder, @port_type, now(), @real_name);    SELECT  last_insert_id() ";
                cmd.CommandType = System.Data.CommandType.Text;

                //cmd.Parameters.Add(new OracleParameter("operation", fixOperationName(operationName, service)));
                cmd.Parameters.Add(new MySqlParameter("operation", operationName));
                cmd.Parameters.Add(new MySqlParameter("service", serviceName));
                cmd.Parameters.Add(new MySqlParameter("bpel", bpel));
                cmd.Parameters.Add(new MySqlParameter("folder", folderName));
                portType = portType.Contains(":") ? portType.Remove(0, portType.IndexOf(":") + 1) : portType;
                cmd.Parameters.Add(new MySqlParameter("port_type", portType));
                cmd.Parameters.Add(new MySqlParameter("real_name", fixPortType(operationName, serviceName, bpel, folderName, portType)));

                try
                {
                    id_ret = Convert.ToInt64(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    var m = ex.Message;
                    id_ret = 0;
                }
            }

            if (operation_id == 0 && id_ret != 0)
            {
                //actualizar relaciones creadas antes del bpel
                using (var cmd = new MySqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "update operations_rel set operation_id = @operation_id where operation_id = 0 ";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.Parameters.Add(new MySqlParameter("operation_id", id_ret));

                    cmd.ExecuteNonQuery();
                }
            }
            return id_ret;
        }

        public static void grabaRelacionQueue(long operation_id, string operationName, string serviceName, string portType, string operationInvoked, string serviceIvoked, string portTypeInv, string folderName, string bpelName)
        {
            try
            {
                if (operationName == null)
                {
                    return;
                }
                portType = fixServiceName(serviceName, portType, folderName);

                var cont = false;
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select count(*) from operations_dequeue where operation_name = @operation and service_name = @service and " +
                    "queue_invoked = @queue_invoked and service_invoked = @service_invoked and queue_type = @queue_type and real_name = @real_name and " +
                    "operation_id = @operation_id";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.Parameters.Add(new MySqlParameter("operation", operationName));
                    cmd.Parameters.Add(new MySqlParameter("service", serviceName));
                    cmd.Parameters.Add(new MySqlParameter("queue_invoked", operationInvoked));
                    cmd.Parameters.Add(new MySqlParameter("service_invoked", serviceIvoked));
                    portTypeInv = portTypeInv.Contains(":") ? portTypeInv.Remove(0, portTypeInv.IndexOf(":") + 1) : portTypeInv;
                    cmd.Parameters.Add(new MySqlParameter("queue_type", fixPortType(operationName, serviceName, "", folderName, portType)));
                    portType = portType.Contains(":") ? portType.Remove(0, portType.IndexOf(":") + 1) : portType;
                    cmd.Parameters.Add(new MySqlParameter("port_type", portType));
                    cmd.Parameters.Add(new MySqlParameter("real_name", fixPortType(operationName, serviceName, bpelName, folderName, portType)));
                    cmd.Parameters.Add(new MySqlParameter("operation_id", operation_id));

                    var respS = cmd.ExecuteScalar();
                    cont = (Convert.ToInt32(respS) == 0);
                }
                if (cont)
                {
                    portTypeInv = fixServiceNameInv(serviceIvoked, portTypeInv);

                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "insert into operations_dequeue (operation_name, service_name, queue_invoked, service_invoked, fecha, queue_type, port_type, real_name, operation_id) values " +
                        "(@operation, @service, @queue_invoked, @service_invoked, now(), @queue_type, @port_type, @real_name, @operation_id ) ";
                        cmd.CommandType = System.Data.CommandType.Text;

                        cmd.Parameters.Add(new MySqlParameter("operation", operationName));
                        cmd.Parameters.Add(new MySqlParameter("service", serviceName));
                        portType = portType.Contains(":") ? portType.Remove(0, portType.IndexOf(":") + 1) : portType;
                        cmd.Parameters.Add(new MySqlParameter("port_type", portType));
                        cmd.Parameters.Add(new MySqlParameter("queue_invoked", operationInvoked));
                        cmd.Parameters.Add(new MySqlParameter("service_invoked", serviceIvoked));
                        portTypeInv = portTypeInv.Contains(":") ? portTypeInv.Remove(0, portTypeInv.IndexOf(":") + 1) : portTypeInv;
                        cmd.Parameters.Add(new MySqlParameter("queue_type", portTypeInv));
                        cmd.Parameters.Add(new MySqlParameter("real_name", fixPortType(operationName, serviceName, bpelName, folderName, portType)));
                        cmd.Parameters.Add(new MySqlParameter("operation_id", operation_id));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string fixPortType(string operationName, string serviceName, string bpel, string folderName, string portType)
        {
            var realName = portType + "." + operationName;
            if (portType == "Consume_Message_ptt" || portType == "PublishWOEventCS" || portType == "PublishEventCS" || portType == "IIntegrationMailbox" ||
                (folderName.EndsWith( "Vista360-Services") && bpel == "GetVista360.bpel") ||
                (folderName.EndsWith("BBVODEventFromPEGS") && bpel == "IOrderableEventCatalogConfigurationService") ||
                (folderName.EndsWith("LeadershipInMovies")) ||
                operationName.ToLower().Contains("consume") || operationName.ToLower().Contains("publish") ||
                operationName.ToLower().Equals("receive") || portType.ToLower().Contains("async") || portType == "EstablishCustomerRelationship_pt"
                )
            {
                realName = bpel + "#" + portType + "." + operationName;
            }
            return realName;

        }

        public static string fixServiceName(string service_name, string port_type, string ruta)
        {
            if (ruta.Contains("ActivateServiceAR") || ruta.Contains("ActivateServiceCH") ||
                ruta.Contains("ActivateServiceCO") || ruta.Contains("ActivateServiceEC") ||
                ruta.Contains("ActivateServicePE") || ruta.Contains("ActivateServiceVE") ||
                ruta.Contains("ActiveServiceUY"))
            {

                return "ActivateService_pt-" + ruta.Substring(ruta.Length - 2);
            }
            else if (ruta.Contains("crm-assurance") && service_name.Contains("360"))
            {
                return "ptVista360-crm";
            }
            else if (ruta.Contains("IntArgFinance") && service_name.Contains("MedIBSFinanceService_ep"))
            {
                return "IBSFinanceServiceSoap";
            }
            else if (service_name.StartsWith("EDA"))
            {
                return service_name;
            }
            else
            {
                return port_type;
            }
        }

        public static string fixServiceNameInv(string service_name, string port_type)
        {
            if (service_name.Contains("ActivateServiceAR") || service_name.Contains("ActivateServiceCH") ||
                service_name.Contains("ActivateServiceCO") || service_name.Contains("ActivateServiceEC") ||
                service_name.Contains("ActivateServicePE") || service_name.Contains("ActivateServiceVE") ||
                service_name.Contains("ActivateServiceUY") || service_name.Contains("ActivateServiceCL")
                )
            {

                return "ActivateService_pt-" + service_name.Substring(service_name.Length - 2);
            }
            else if (port_type == "IDecisionService")
            {
                return "IDecisionService#" + service_name;
            }
            else
            {
                return port_type;
            }
        }
    }
}
