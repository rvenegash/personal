using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using exportaAPIS_CloudWso2.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;

namespace exportaAPIS_CloudWso2
{
    public static class ProgramWso2
    {
        public static void Mainpublicar(string[] args) //
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqap.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;

            //pedir token
            string token = "3bc51182-53af-32b5-af57-237c8afc78af"; // pedirToken(config, "&scope=apim:api_view");
            if (token.Length != 0)
            {
                Console.WriteLine("Token api_view : " + token);

                Models.ApiResult apis = exportarAPIs(config, token);

                var httpClientHandler = new HttpClientHandler() { };
                var client = new HttpClient(httpClientHandler);
                client.BaseAddress = new Uri(apiUrl);

                var tokenPub = "3bc51182-53af-32b5-af57-237c8afc78af"; //publicar
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenPub);
                client.DefaultRequestHeaders.Add("X-WSO2-Tenant", "dtvqap");

                foreach (var item in apis.list)
                {
                    Console.WriteLine("id : {0}, name: {1}, version: {2}", item.id, item.name, item.version);

                    if (item.status == "CREATED")
                    {//cambiar de estado
                        var content = new StringContent("", Encoding.UTF8, "application/json");
                        var response2 = client.PostAsync(apiUrlPublisher + "/apis/change-lifecycle?apiId=" + item.id + "&action=Publish", content);
                        response2.Wait();

                        //obtener respuesta
                        if (response2.Result.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Creada");
                        }
                        else
                        {
                            //error
                            var mensaje = "";
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Token NO obtenido");
            }

            Console.WriteLine("Fin");
        }


        static void MainExp2exportaJson(string[] args) // de prod a archivos json, 
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqcp.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqcm.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");

            //pedir token
            string token = pedirToken(config, "&scope=apim:api_view");
            if (token.Length != 0)
            {
                Console.WriteLine("Token api_view : " + token);

                Models.ApiResult apis = exportarAPIs(config, token);

                //var tokenApp = "2a34bfae-2f8a-31e0-be70-40f62fcbeb1b"; //dtvqap
                //Console.WriteLine("Token api_create : " + tokenApp);

                //var ruta = @"C:\Users\rvenegas\OneDrive - VRIO DIRECTV\Requerimientos Directv\2023 09 14 apis prod\";
                //var ruta = @"C:\Users\rvenegas\OneDrive - VRIO DIRECTV\Requerimientos Directv\2023 09 14 apis qam\";
                //var ruta = @"C:\Users\rvenegas\OneDrive - VRIO DIRECTV\Requerimientos Directv\2023 09 14 apis qcp\";
                var ruta = @"C:\Users\rvenegas\OneDrive - VRIO DIRECTV\Requerimientos Directv\2023 09 14 apis qcm\";
                exportaAPIaOtroTenant(config, apis, token, ruta);
            }
            else
            {
                Console.WriteLine("Token NO obtenido");
            }

            Console.WriteLine("Fin");
        }


        private static void exportaAPIaOtroTenant(IConfigurationSection config, ApiResult apis, string token, string ruta)
        {
            Console.WriteLine("---===  exportaAPIaOtroTenant ===---");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;

            var httpClientHandler = new HttpClientHandler() { };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var item in apis.list)
            {
                Console.WriteLine("id : {0}, name: {1}, version: {2}", item.id, item.name, item.version);
                if (File.Exists(ruta + item.id + ".bak"))
                {
                    Console.WriteLine("ya fue migrada");
                    continue;
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(apiUrlPublisher + "/apis/" + item.id);
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();

                    usrRes.Wait();

                    var api = System.Text.Json.JsonSerializer.Deserialize<Models.Api>(usrRes.Result);
                    if (api != null)
                    {
                        Console.WriteLine("copiando api");
                        if (false)
                        {
                            //api.visibility = "PRIVATE";
                            //api.context = api.context.Replace("/t/dtvla1179", "");
                            api.provider = "";
                            api.id = "";
                            api.thumbnailUri = "";
                            //api.visibleRoles = new string[0];
                            //api.accessControlRoles = new string[0];
                            api.apiLevelPolicy = null;
                            if (api.tiers.Length == 0)
                            {
                                api.tiers = new string[1];
                                api.tiers[0] = "Gold";
                            }
                            /*
                            if (api.endpointConfig != null)
                            {
                                api.endpointConfig = api.endpointConfig.Replace("suspendDuration\":\"0\"", "suspendDuration\":0");
                                api.endpointConfig = api.endpointConfig.Replace("suspendMaxDuration\":\"0\"", "suspendMaxDuration\":0");
                                api.endpointConfig = api.endpointConfig.Replace("actionDuration\":\"30000\"", "actionDuration\":0");
                                var endp = System.Text.Json.JsonSerializer.Deserialize<Models.EndpointConfig>(api.endpointConfig);
                                if (endp.production_endpoints != null)
                                {
                                    endp.production_endpoints.url = endp.production_endpoints.url.Replace("https://apiresources.dtvpan.com", "https://soaresourcestest.dtvdev.net/QAM");
                                }
                                endp.sandbox_endpoints = endp.production_endpoints;
                                api.endpointConfig = System.Text.Json.JsonSerializer.Serialize(endp);
                            }
                            */
                            //api.name += "_RV";
                        }
                        var sw = new StreamWriter(ruta + item.name + "#" + item.version + "#" + item.id + ".json", false, Encoding.UTF8);
                        System.Text.Json.JsonSerializerOptions op = new System.Text.Json.JsonSerializerOptions() { WriteIndented = true };
                        sw.Write(System.Text.Json.JsonSerializer.Serialize(api, options: op));
                        sw.Close();
                    }
                }
                else
                {
                    Console.WriteLine("api/am/publisher/apis StatusCode : " + response.Result.StatusCode);
                    //return null;
                }
            }
        }


        static void MaineliminaAPIs(string[] args) //ELIMINA TODO,
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");

            //pedir token
            string token = "6deb97c8-ab4f-3864-9351-a5ba79de24b7"; // pedirToken(config, "&scope=apim:api_view");
            if (token.Length != 0)
            {
                Console.WriteLine("Token api_view : " + token);

                Models.ApiResult apis = exportarAPIs(config, token);

                var tokenApp = "003c7559-56a7-35d6-b37f-0e7b668e5336"; //dtvqam
                                                                       //Console.WriteLine("Token api_create : " + tokenApp);

                eliminaAPIs(config, apis, tokenApp);
            }
            else
            {
                Console.WriteLine("Token NO obtenido");
            }

            Console.WriteLine("Fin");
        }

        private static void eliminaAPIs(IConfigurationSection config, ApiResult apis, string token)
        {
            Console.WriteLine("---===  eliminaAPIs ===---");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;

            var httpClientHandler = new HttpClientHandler() { };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            foreach (var item in apis.list)
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.DeleteAsync(apiUrlPublisher + "/apis/" + item.id);
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    Console.WriteLine(string.Format("API borrada : {0}, {1}", item.name, item.version));

                }
                else
                {
                    Console.WriteLine(string.Format("API   NO   borrada : {0}, {1}", item.name, item.version));
                }
            }

            Console.WriteLine("Fin");
        }

        public static void ExportaAMySql(string[] args) //inserta en mysql
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string dbCS = configRoot.GetConnectionString("MySQL");

            //borrar datos de tablas 
            deleteTables(dbCS);

            //pedir token
            string token = pedirToken(config, "&scope=apim:api_view");
            if (token.Length != 0)
            {
                Console.WriteLine("Token api_view : " + token);

                Models.ApiResult apis = exportarAPIs(config, token);

                //insertar apis en tabla
                insertAPIS(dbCS, apis);

                exportarDocumentacionAPIs(config, apis, token, dbCS);

                exportarOtrosAPIs(config, apis, token, dbCS);

                var tokenSub = pedirToken(config, "&scope=apim:subscription_view");
                Console.WriteLine("Token subscription_view : " + tokenSub);
                var tokenApp = pedirToken(config, "&scope=apim:api_create");
                Console.WriteLine("Token api_create : " + tokenApp);
                exportarSuscripcionesAPIs(config, token, tokenSub, tokenApp, apis, dbCS);
            }
            else
            {
                Console.WriteLine("Token NO obtenido");
            }

            Console.WriteLine("Fin");
        }

        static void MainExportaOracle(string[] args) // exportar a oracle db
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string dbCS = configRoot.GetConnectionString("OracleDb");

            //borrar datos de tablas 
            deleteTablesOracle(dbCS);

            //pedir token
            string token = pedirToken(config, "&scope=apim:api_view");
            if (token.Length != 0)
            {
                Console.WriteLine("Token api_view : " + token);

                Models.ApiResult apis = exportarAPIs(config, token);

                //exportar apis en cs
                exportCsvAPISOracle(dbCS, apis);

                exportarDocumentacionAPIsOracle(config, apis, token, dbCS);

                exportarOtrosAPIsOracle(config, apis, token, dbCS);

                var tokenSub = pedirToken(config, "&scope=apim:subscription_view");
                Console.WriteLine("Token subscription_view : " + tokenSub);
                var tokenApp = pedirToken(config, "&scope=apim:api_create");
                Console.WriteLine("Token api_create : " + tokenApp);
                exportarSuscripcionesAPIsOracle(config, token, tokenSub, tokenApp, apis, dbCS);
            }
            else
            {
                Console.WriteLine("Token NO obtenido");
            }

            Console.WriteLine("Fin");
        }


        public static void Main_exportarcsv(string[] args) //
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("API-url").Value;
            string file = config.GetSection("apis.csv").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;

            //pedir token
            string token = pedirToken(config, "&scope=apim:api_view");
            if (token.Length != 0)
            {
                Console.WriteLine("Token api_view : " + token);

                var httpClientHandler = new HttpClientHandler() { UseDefaultCredentials = true, ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };

                var client = new HttpClient(httpClientHandler);
                client.BaseAddress = new Uri(apiUrl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Models.ApiResult apis = exportarAPIs(config, token);
                using (var sw = new StreamWriter(file))
                {
                    var linea = "id, name, endpointConfig, context, version, provider, status, visibility";
                    sw.WriteLine(linea);

                    //exportar apis en cs
                    foreach (var item in apis.list)
                    {
                        Console.WriteLine("{0} {1}", item.name, item.version);

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        var response = client.GetAsync(apiUrlPublisher + "/apis/" + item.id + "");
                        response.Wait();

                        string endpointUrlP = "";
                        string endpointUrlS = "";

                        //obtener respuesta
                        if (response.Result.IsSuccessStatusCode)
                        {
                            var usrRes = response.Result.Content.ReadAsStringAsync();
                            usrRes.Wait();

                            var api = JsonSerializer.Deserialize<Models.Api>(usrRes.Result);
                            if (!string.IsNullOrEmpty(api.endpointConfig))
                            {
                                JsonNode document = JsonNode.Parse(api.endpointConfig)!;
                                try
                                {
                                    JsonNode root = document.Root;
                                    endpointUrlP = (string)(root["production_endpoints"]["url"]);
                                    endpointUrlS = (string)(root["sandbox_endpoints"]["url"]);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("api.endpointConfig : " + api.endpointConfig);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("api/am/publisher/apis StatusCode : " + response.Result.StatusCode);
                            //return null;
                        }
                        sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", item.id, item.name, item.context, item.version, item.provider, item.status, item.visibility, endpointUrlP, endpointUrlS);
                    }
                    sw.Close();
                }

                //exportarDocumentacionAPIsOracle(config, apis, token, dbCS);
                //exportarOtrosAPIsOracle(config, apis, token, dbCS);

                //var tokenSub = pedirToken(config, "&scope=apim:subscription_view");
                //Console.WriteLine("Token subscription_view : " + tokenSub);
                //var tokenApp = pedirToken(config, "&scope=apim:api_create");
                //Console.WriteLine("Token api_create : " + tokenApp);
                //exportarSuscripcionesAPIsOracle(config, token, tokenSub, tokenApp, apis, dbCS);
            }
            else
            {
                Console.WriteLine("Token NO obtenido");
            }

            Console.WriteLine("Fin");
        }

        static void MaincambiaVisibility(string[] args) //
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqcm.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");

            //pedir token
            string token = "78e598b5-dc1e-3a30-8773-83f88e8c2977";// pedirToken(config, " & scope=apim:api_view");
            if (token.Length != 0)
            {
                Console.WriteLine("Token api_view : " + token);

                Models.ApiResult apis = exportarAPIs(config, token);

                //var tokenSub = pedirToken(config, "&scope=apim:subscription_view");
                //Console.WriteLine("Token subscription_view : " + tokenSub);
                var tokenApp = "3aceac0e-0d22-36d2-aaec-a8cc0adbace0"; // pedirToken(config, " & scope=apim:api_create");
                Console.WriteLine("Token api_create : " + tokenApp);

                cambiaVisibilityAPI(config, apis, token, tokenApp);
            }
            else
            {
                Console.WriteLine("Token NO obtenido");
            }

            Console.WriteLine("Fin");
        }


        static void Maincambiaendpoint(string[] args) //
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - devqcr.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");

            //pedir token
            string token = "bbe37f4b-4255-36f2-8126-b3be3b558ded";// pedirToken(config, " & scope=apim:api_view");
            if (token.Length != 0)
            {
                Console.WriteLine("Token api_view : " + token);

                Models.ApiResult apis = exportarAPIs(config, token);

                //var tokenSub = pedirToken(config, "&scope=apim:subscription_view");
                //Console.WriteLine("Token subscription_view : " + tokenSub);
                var tokenApp = "97cb52b0-87e3-3848-8236-f053980f5138"; // pedirToken(config, " & scope=apim:api_create");
                Console.WriteLine("Token api_create : " + tokenApp);

                cambiaEndpointAPI(config, apis, token, tokenApp);
            }
            else
            {
                Console.WriteLine("Token NO obtenido");
            }

            Console.WriteLine("Fin");
        }

        private static void cambiaEndpointAPI(IConfigurationSection config, ApiResult apis, string token, string tokenApp)
        {
            Console.WriteLine("---===  cambiaVisibilityAPI ===---");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;

            var httpClientHandler = new HttpClientHandler() { };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var item in apis.list)
            {
                Console.WriteLine("id : {0}, name: {1}, version: {2}", item.id, item.name, item.version);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(apiUrlPublisher + "/apis/" + item.id);
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();

                    usrRes.Wait();

                    var api = System.Text.Json.JsonSerializer.Deserialize<Models.Api>(usrRes.Result);
                    if (api.endpointConfig != null)
                    {
                        Console.WriteLine("cambiando endpoint");

                        api.endpointConfig = null; ; // "{\"production_endpoints\":{\"url\":\"https://soaresourcestest.dtvdev.net/QAP/DTVLA-Services/API/PartyManagement\",\"config\":null,\"template_not_supported\":false},\"sandbox_endpoints\":{\"url\":\"https://soaresourcestest.dtvdev.net/QCP/DTVLA-Services/API/PartyManagement\",\"config\":null,\"template_not_supported\":false},\"endpoint_type\":\"http\"}";
                        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(api), Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenApp);
                        var response2 = client.PutAsync(apiUrlPublisher + "/apis/" + item.id, content);
                        response2.Wait();

                        //obtener respuesta
                        if (response2.Result.IsSuccessStatusCode)
                        {
                        }
                        else
                        {
                            //error
                            var mensaje = "";
                        }
                    }
                }
                else
                {
                    Console.WriteLine("api/am/publisher/apis StatusCode : " + response.Result.StatusCode);
                    //return null;
                }
            }
        }





        private static void cambiaVisibilityAPI(IConfigurationSection config, ApiResult apis, string token, string tokenApp)
        {
            Console.WriteLine("---===  cambiaVisibilityAPI ===---");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;

            var httpClientHandler = new HttpClientHandler() { };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var item in apis.list)
            {
                Console.WriteLine("id : {0}, name: {1}, version: {2}", item.id, item.name, item.version);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(apiUrlPublisher + "/apis/" + item.id);
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();

                    usrRes.Wait();

                    var api = System.Text.Json.JsonSerializer.Deserialize<Models.Api>(usrRes.Result);
                    if (api.visibility != "PRIVATE")
                    {
                        Console.WriteLine("cambiando a privada");
                        //var newapi = new Models.Api();
                        //newapi.id = item.id;
                        //newapi.visibility = "PRIVATE";
                        api.visibility = "PRIVATE";
                        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(api), Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenApp);
                        var response2 = client.PutAsync(apiUrlPublisher + "/apis/" + item.id, content);
                        response2.Wait();

                        //obtener respuesta
                        if (response2.Result.IsSuccessStatusCode)
                        {
                        }
                        else
                        {
                            //error
                            var mensaje = "";
                        }
                    }
                }
                else
                {
                    Console.WriteLine("api/am/publisher/apis StatusCode : " + response.Result.StatusCode);
                    //return null;
                }
            }
        }

        private static void exportarOtrosAPIs(IConfigurationSection config, ApiResult apis, string token, string dbCS)
        {
            Console.WriteLine("---===  exportarOtrosAPIs ===---");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;
            string archivoSwagger = config.GetSection("ArchTempSwagger").Value;

            var httpClientHandler = new HttpClientHandler() { UseDefaultCredentials = true, ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var item in apis.list)
            {
                Console.WriteLine("id : {0}, name: {1}, version: {2}", item.id, item.name, item.version);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(apiUrlPublisher + "/apis/" + item.id + "");
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();

                    usrRes.Wait();

                    try
                    {
                        var api = System.Text.Json.JsonSerializer.Deserialize<Models.Api>(usrRes.Result);
                        if (!string.IsNullOrEmpty(api.apiDefinition))
                        {
                            /*  var parser = new RamlParser();
                              var sw = new StreamWriter(archivoSwagger, false);
                              sw.Write(api.apiDefinition);
                              sw.Close();

                              var model = parser.Load(SpecificationType.OASJSON, archivoSwagger);
                            */

                        }
                        updateOtrosAPIS(dbCS, api, item.id);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }


                    //Console.WriteLine("updateAPIS OK ");
                }
                else
                {
                    Console.WriteLine("api/am/publisher/apis StatusCode : " + response.Result.StatusCode);
                    //return null;
                }
            }
        }

        private static void updateOtrosAPIS(string cs, Models.Api doc, string apiId)
        {
            if (doc != null)
            {
                using (var conn = new MySqlConnection(cs))
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;

                        cmd.CommandText = "UPDATE API SET visibility = @visibility WHERE id = @id";
                        cmd.CommandType = System.Data.CommandType.Text;

                        cmd.Parameters.Add(new MySqlParameter("id", apiId));
                        cmd.Parameters.Add(new MySqlParameter("visibility", doc.visibility));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void exportarOtrosAPIsOracle(IConfigurationSection config, ApiResult apis, string token, string dbCS)
        {
            Console.WriteLine("---===  exportarOtrosAPIs ===---");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;
            string archivoSwagger = config.GetSection("ArchTempSwagger").Value;

            var httpClientHandler = new HttpClientHandler() { UseDefaultCredentials = true, ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var item in apis.list)
            {
                Console.WriteLine("id : {0}, name: {1}, version: {2}", item.id, item.name, item.version);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(apiUrlPublisher + "/apis/" + item.id + "");
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();

                    usrRes.Wait();

                    try
                    {
                        var api = System.Text.Json.JsonSerializer.Deserialize<Models.Api>(usrRes.Result);
                        if (!string.IsNullOrEmpty(api.apiDefinition))
                        {
                            /*  var parser = new RamlParser();
                              var sw = new StreamWriter(archivoSwagger, false);
                              sw.Write(api.apiDefinition);
                              sw.Close();

                              var model = parser.Load(SpecificationType.OASJSON, archivoSwagger);
                            */

                        }
                        updateOtrosAPISOracle(dbCS, api, item.id);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }


                    //Console.WriteLine("updateAPIS OK ");
                }
                else
                {
                    Console.WriteLine("api/am/publisher/apis StatusCode : " + response.Result.StatusCode);
                    //return null;
                }
            }
        }

        private static void updateOtrosAPISOracle(string cs, Models.Api doc, string apiId)
        {
            if (doc != null)
            {
                using (var conn = new OracleConnection(cs))
                {
                    conn.Open();

                    using (var cmd = new OracleCommand())
                    {
                        cmd.Connection = conn;

                        cmd.CommandText = "UPDATE RV_API SET visibility = @visibility WHERE id = @id";
                        cmd.CommandType = System.Data.CommandType.Text;

                        cmd.Parameters.Add(new OracleParameter("id", apiId));
                        cmd.Parameters.Add(new OracleParameter("visibility", doc.visibility));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }


        private static void exportarDocumentacionAPIs(IConfigurationSection config, ApiResult apis, string token, string dbCS)
        {
            Console.WriteLine("---===  exportarDocumentacionAPIs ===---");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;

            var httpClientHandler = new HttpClientHandler() { UseDefaultCredentials = true, ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var item in apis.list)
            {
                Console.WriteLine("id : {0}, name: {1}, version: {2}", item.id, item.name, item.version);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(apiUrlPublisher + "/apis/" + item.id + "/documents");
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();

                    usrRes.Wait();

                    var api = System.Text.Json.JsonSerializer.Deserialize<Models.DocResult>(usrRes.Result);

                    updateDocAPIS(dbCS, api, item.id);

                    //Console.WriteLine("updateAPIS OK ");
                }
                else
                {
                    Console.WriteLine("api/am/publisher/apis StatusCode : " + response.Result.StatusCode);
                    //return null;
                }
            }
        }

        private static void updateDocAPIS(string cs, Models.DocResult doc, string apiId)
        {
            if (doc.list.Length > 0)
            {
                var docs = "";
                foreach (var item in doc.list)
                {
                    docs += "(" + item.name + "-" + item.type + ")";
                }
                using (var conn = new MySqlConnection(cs))
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;

                        cmd.CommandText = "UPDATE API SET docUrl = @docUrl WHERE id = @id";
                        cmd.CommandType = System.Data.CommandType.Text;

                        cmd.Parameters.Add(new MySqlParameter("id", apiId));
                        cmd.Parameters.Add(new MySqlParameter("docUrl", docs));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void exportarDocumentacionAPIsOracle(IConfigurationSection config, ApiResult apis, string token, string dbCS)
        {
            Console.WriteLine("---===  exportarDocumentacionAPIs ===---");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;

            var httpClientHandler = new HttpClientHandler() { UseDefaultCredentials = true, ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var item in apis.list)
            {
                Console.WriteLine("id : {0}, name: {1}, version: {2}", item.id, item.name, item.version);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(apiUrlPublisher + "/apis/" + item.id + "/documents");
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();

                    usrRes.Wait();

                    var api = System.Text.Json.JsonSerializer.Deserialize<Models.DocResult>(usrRes.Result);

                    updateDocAPISOracle(dbCS, api, item.id);

                    //Console.WriteLine("updateAPIS OK ");
                }
                else
                {
                    Console.WriteLine("api/am/publisher/apis StatusCode : " + response.Result.StatusCode);
                    //return null;
                }
            }
        }

        private static void updateDocAPISOracle(string cs, Models.DocResult doc, string apiId)
        {
            if (doc.list.Length > 0)
            {
                var docs = "";
                foreach (var item in doc.list)
                {
                    docs += "(" + item.name + "-" + item.type + ")";
                }
                using (var conn = new OracleConnection(cs))
                {
                    conn.Open();

                    using (var cmd = new OracleCommand())
                    {
                        cmd.Connection = conn;

                        cmd.CommandText = "UPDATE RV_API SET docUrl = @docUrl WHERE id = @id";
                        cmd.CommandType = System.Data.CommandType.Text;

                        cmd.Parameters.Add(new OracleParameter("id", apiId));
                        cmd.Parameters.Add(new OracleParameter("docUrl", docs));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }


        public static void MainCrearDesdeJson(string[] args) //crea desde archivos json, 
        {
            var token = "0f34870b-9827-3d95-b2ba-26f39cb0a80c"; //creacion qcp

            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("X-WSO2-Tenant", "dtvqam");

            var files = Directory.GetFiles(@"C:\Users\rvenegas\Box Sync\Requerimientos Directv\2020 11 25 copiar apis prod qam wso2\apis_prod_2", "*.json");
            foreach (var item in files)
            {
                var sr = new StreamReader(item);
                var api = sr.ReadToEnd();
                sr.Close();

                Console.WriteLine("archivo : " + Path.GetFileName(item));
                Console.WriteLine("api : " + api.Substring(0, 30));
                var content = new StringContent(api, Encoding.UTF8, "application/json");

                var response2 = client.PostAsync(apiUrlPublisher + "/apis", content);
                response2.Wait();

                //obtener respuesta
                if (response2.Result.IsSuccessStatusCode)
                {
                    File.Move(item, item.Replace(".json", ".bak"));
                    Console.WriteLine("Creada");
                }
                else
                {
                    //error
                    var mensaje = "";
                }


            }
            Console.WriteLine("Fin");
        }

        public static void Main_ExportaArchivo(string[] args) //
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: false, reloadOnChange: false).Build();
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: false, reloadOnChange: false).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");

            System.IO.StreamWriter swA = new System.IO.StreamWriter(config.GetSection("CarpetaInforme").Value + "apis-qam.csv");
            System.IO.StreamWriter swS = new System.IO.StreamWriter(config.GetSection("CarpetaInforme").Value + "subscriptions-qam.csv");

            //pedir token
            string token = pedirToken(config, "&scope=apim:api_view");
            if (token.Length != 0)
            {
                Console.WriteLine("Token api_view : " + token);

                Models.ApiResult apis = exportarAPIs(config, token);

                foreach (var item in apis.list)
                {
                    //ID	NAME	DESC	CONTEXT	VERSION	PROVIDER	STATUS	URL PROD	URL SANDBOX	DOCS
                    swA.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", item.id, item.name, item.description, item.context, item.version, item.provider, item.status, item.endpointConfig, item.isDefaultVersion, item.visibility);
                }
                //suscripciones
                var tokenSub = pedirToken(config, "&scope=apim:subscription_view");
                var tokenApp = pedirToken(config, "&scope=apim:api_create");
                Console.WriteLine("Token api_create : " + tokenApp);
                exportarSuscripcionesAPIsArchivo(apis, config, token, tokenSub, tokenApp, swS);

            }
            else
            {
                Console.WriteLine("Token NO obtenido");
            }
            swA.Close();
            swS.Close();
            Console.WriteLine("Fin");
        }

        private static void exportarSuscripcionesAPIs(IConfigurationSection config, string token, string tokenSub, string tokenApp, Models.ApiResult apis, string cs)
        {
            Console.WriteLine("---===  exportarSuscripcionesAPIs ===---");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;
            int limit = Convert.ToInt32(config.GetSection("API-Limit").Value);

            var httpClientHandler = new HttpClientHandler() { UseDefaultCredentials = true, ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            foreach (var item in apis.list)
            {
                Console.WriteLine("id : {0}, name: {1}, version: {2}", item.id, item.name, item.version);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(apiUrlPublisher + "/apis/" + item.id);
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();

                    usrRes.Wait();

                    var api = System.Text.Json.JsonSerializer.Deserialize<Models.Api>(usrRes.Result);

                    updateAPIS(cs, api);

                    //Console.WriteLine("updateAPIS OK ");
                }
                else
                {
                    Console.WriteLine("api/am/publisher/apis StatusCode : " + response.Result.StatusCode);
                    //return null;
                }

                //GET https://gateway.api.cloud.wso2.com/api/am/publisher/subscriptions?apiId=890a4f4d-09eb-4877-a323-57f6ce2ed79b
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenSub);
                response = client.GetAsync(apiUrlPublisher + "/subscriptions?apiId=" + item.id);
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();

                    usrRes.Wait();

                    var api = System.Text.Json.JsonSerializer.Deserialize<Models.SubscriptionResult>(usrRes.Result);

                    foreach (var itemSub in api.list)
                    {
                        //GET https://gateway.api.cloud.wso2.com/api/am/publisher/applications/896658a0-b4ee-4535-bbfa-806c894a4015
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenApp);
                        response = client.GetAsync(apiUrlPublisher + "/applications/" + itemSub.applicationId);
                        response.Wait();

                        //obtener respuesta
                        if (response.Result.IsSuccessStatusCode)
                        {
                            usrRes = response.Result.Content.ReadAsStringAsync();

                            usrRes.Wait();

                            var app = System.Text.Json.JsonSerializer.Deserialize<Models.Application>(usrRes.Result);

                            itemSub.applicationName = app.name;
                            itemSub.subscriber = app.subscriber;
                        }
                        else
                        {
                            Console.WriteLine("api/am/publisher/applications StatusCode : " + response.Result.StatusCode);
                            //return null;
                        }
                    }

                    insertSubs(cs, item.id, api);

                    //Console.WriteLine("usrRes.Result : " + usrRes.Result);
                }
                else
                {
                    Console.WriteLine("api/am/publisher/subscriptions StatusCode : " + response.Result.StatusCode);
                    //return null;
                }
            }
        }

        private static void deleteTablesOracle(string cs)
        {
            Console.WriteLine("OracleConnection : " + cs);
            using (var conn = new OracleConnection(cs))
            {
                conn.Open();

                using (var cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from RV_API";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from RV_API_Subscriptions";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Tablas borradas");
        }

        private static void updateAPIS(string cs, Models.Api api)
        {
            using (var conn = new MySqlConnection(cs))
            {
                conn.Open();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "UPDATE API SET endpointConfig = @endpointConfig WHERE id = @id";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.Parameters.Add(new MySqlParameter("id", api.id));
                    cmd.Parameters.Add(new MySqlParameter("endpointConfig", api.endpointConfig));

                    cmd.ExecuteNonQuery();
                }
                if (api.endpointConfig != null && api.endpointConfig != "")
                {
                    try
                    {
                        if (api.endpointConfig.Contains("\"actionDuration\":30000"))
                        { api.endpointConfig = api.endpointConfig.Replace("\"actionDuration\":30000", "\"actionDuration\":\"30000\""); }

                        Models.EndpointConfig ep = System.Text.Json.JsonSerializer.Deserialize<Models.EndpointConfig>(api.endpointConfig); ;

                        //if (ep.production_endpoints.url != "" || ep.sandbox_endpoints.url != "")
                        {
                            using (var cmd = new MySqlCommand())
                            {
                                cmd.Connection = conn;

                                cmd.CommandText = "UPDATE API SET production_endpoints = @production_endpoints, sandbox_endpoints = @sandbox_endpoints WHERE id = @id";
                                cmd.CommandType = System.Data.CommandType.Text;

                                cmd.Parameters.Add(new MySqlParameter("id", api.id));
                                cmd.Parameters.Add(new MySqlParameter("production_endpoints", ep.production_endpoints == null ? "" : ep.production_endpoints.url));
                                cmd.Parameters.Add(new MySqlParameter("sandbox_endpoints", ep.sandbox_endpoints == null ? "" : ep.sandbox_endpoints.url));

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        var m = e.Message;
                    }
                }
            }
        }

        private static void insertSubs(string cs, string apiId, Models.SubscriptionResult subs)
        {
            Console.WriteLine("insertSubs");

            using (var conn = new MySqlConnection(cs))
            {
                conn.Open();

                foreach (var item in subs.list)
                {
                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO API_Subscriptions (api_id, tier, applicationId, applicationName, status, subscriber) VALUES (@api_id, @tier, @applicationId, @applicationName, @status, @subscriber)";
                        cmd.CommandType = System.Data.CommandType.Text;

                        cmd.Parameters.Add(new MySqlParameter("api_id", apiId));
                        cmd.Parameters.Add(new MySqlParameter("tier", item.tier));
                        cmd.Parameters.Add(new MySqlParameter("applicationId", item.applicationId));
                        cmd.Parameters.Add(new MySqlParameter("applicationName", item.applicationName));
                        cmd.Parameters.Add(new MySqlParameter("status", item.status));
                        cmd.Parameters.Add(new MySqlParameter("subscriber", item.subscriber));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            //Console.WriteLine("Fin insertSubs");
        }

        private static string pedirToken(IConfigurationSection config, string scope)
        {
            Console.WriteLine("---===  pedirToken ===---");

            string apiUrl = config.GetSection("API-url-token").Value;
            string apiTokenKey = config.GetSection("API-token-key").Value;
            string apiTokenUrl = config.GetSection("API-token-url").Value;

            Console.WriteLine("apiUrl : " + apiUrl);
            Console.WriteLine("apiTokenKey : " + apiTokenKey);
            Console.WriteLine("apiTokenUrl : " + apiTokenUrl);

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) =>
            {
                return true;
            };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiTokenKey);

            var response = client.PostAsync("/token", new StringContent(apiTokenUrl + scope, Encoding.UTF8, "application/x-www-form-urlencoded"));
            response.Wait();

            //obtener respuesta
            if (response.Result.IsSuccessStatusCode)
            {
                var usrRes = response.Result.Content.ReadAsStringAsync();

                usrRes.Wait();

                //Console.WriteLine("usrRes.Result : " + usrRes.Result);

                return System.Text.Json.JsonSerializer.Deserialize<Models.Token>(usrRes.Result).access_token;
            }
            else
            {
                Console.WriteLine("response.Result.StatusCode : " + response.Result.StatusCode);
                return "";
            }
        }

        private static Models.ApiResult exportarAPIs(IConfigurationSection config, string token)
        {
            Console.WriteLine("---===  exportarAPIs ===---");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;
            int limit = Convert.ToInt32(config.GetSection("API-Limit").Value);
            string tenant = config.GetSection("tenant").Value;

            var httpClientHandler = new HttpClientHandler() { UseDefaultCredentials = true, ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = client.GetAsync(apiUrlPublisher + "/apis?limit=" + limit.ToString() + "&tenant=" + tenant);
            response.Wait();

            //obtener respuesta
            if (response.Result.IsSuccessStatusCode)
            {
                var usrRes = response.Result.Content.ReadAsStringAsync();

                usrRes.Wait();

                //Console.WriteLine("usrRes.Result : " + usrRes.Result);

                Console.WriteLine("usrRes.Result.length : " + usrRes.Result.Length);
                var api = System.Text.Json.JsonSerializer.Deserialize<Models.ApiResult>(usrRes.Result);

                return api;
            }
            else
            {
                Console.WriteLine("response.Result.StatusCode : " + response.Result.StatusCode);
                Console.WriteLine("apiUrlPublisher : " + apiUrlPublisher);
                return null;
            }

        }

        private static void exportarSuscripcionesAPIsArchivo(ApiResult apis, IConfigurationSection config, string token, string tokenSub, string tokenApp, StreamWriter swS)
        {
            Console.WriteLine("---===  exportarSuscripcionesAPIs ===---");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;
            int limit = Convert.ToInt32(config.GetSection("API-Limit").Value);

            var httpClientHandler = new HttpClientHandler() { UseDefaultCredentials = true, ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var item in apis.list)
            {
                Console.WriteLine("id : {0}, name: {1}", item.id, item.name);

                //GET https://gateway.api.cloud.wso2.com/api/am/publisher/subscriptions?apiId=890a4f4d-09eb-4877-a323-57f6ce2ed79b
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenSub);
                var response = client.GetAsync(apiUrlPublisher + "/subscriptions?apiId=" + item.id);
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();

                    usrRes.Wait();

                    var api = System.Text.Json.JsonSerializer.Deserialize<Models.SubscriptionResult>(usrRes.Result);

                    foreach (var itemSub in api.list)
                    {
                        //GET https://gateway.api.cloud.wso2.com/api/am/publisher/applications/896658a0-b4ee-4535-bbfa-806c894a4015
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenApp);
                        response = client.GetAsync(apiUrlPublisher + "/applications/" + itemSub.applicationId);
                        response.Wait();

                        //obtener respuesta
                        if (response.Result.IsSuccessStatusCode)
                        {
                            usrRes = response.Result.Content.ReadAsStringAsync();

                            usrRes.Wait();

                            var app = System.Text.Json.JsonSerializer.Deserialize<Models.Application>(usrRes.Result);

                            itemSub.applicationName = app.name;
                            itemSub.subscriber = app.subscriber;
                        }
                        else
                        {
                            Console.WriteLine("api/am/publisher/applications StatusCode : " + response.Result.StatusCode);
                            //return null;
                        }
                    }
                    //API ID	TIER	APP ID	APP NAME	STATUS 	SUBSCRIBER
                    foreach (var sub in api.list)
                    {
                        swS.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", item.id, sub.tier, sub.applicationId, sub.applicationName, sub.status, sub.subscriber);
                    }
                }
                else
                {
                    Console.WriteLine("api/am/publisher/subscriptions StatusCode : " + response.Result.StatusCode);
                    //return null;
                }
            }
        }

        private static void deleteTables(string cs)
        {
            Console.WriteLine("MySqlConnection : " + cs);
            using (var conn = new MySqlConnection(cs))
            {
                conn.Open();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from API";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from API_Subscriptions";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Tablas borradas");
        }

        private static void insertAPIS(string cs, Models.ApiResult apis)
        {
            Console.WriteLine("insertAPIS");

            using (var conn = new MySqlConnection(cs))
            {
                conn.Open();

                foreach (var item in apis.list)
                {
                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO API (id, name, description, context, version, provider, status, visibility) VALUES (@id, @name, @description, @context, @version, @provider, @status, @visibility)";
                        cmd.CommandType = System.Data.CommandType.Text;

                        cmd.Parameters.Add(new MySqlParameter("id", item.id));
                        cmd.Parameters.Add(new MySqlParameter("name", item.name));
                        cmd.Parameters.Add(new MySqlParameter("description", item.description));
                        cmd.Parameters.Add(new MySqlParameter("context", item.context));
                        cmd.Parameters.Add(new MySqlParameter("version", item.version));
                        cmd.Parameters.Add(new MySqlParameter("provider", item.provider));
                        cmd.Parameters.Add(new MySqlParameter("status", item.status));
                        cmd.Parameters.Add(new MySqlParameter("visibility", item.visibility));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            Console.WriteLine("Fin insertAPIS (" + apis.list.Length + ")");
        }

        private static void exportCsvAPISOracle(string cs, Models.ApiResult apis)
        {
            Console.WriteLine("insertAPIS");

            using (var conn = new OracleConnection(cs))
            {
                conn.Open();

                foreach (var item in apis.list)
                {
                    using (var cmd = new OracleCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO RV_API (id, name, description, context, version, provider, status, visibility) VALUES (@id, @name, @description, @context, @version, @provider, @status, @visibility)";
                        cmd.CommandType = System.Data.CommandType.Text;

                        cmd.Parameters.Add(new MySqlParameter("id", item.id));
                        cmd.Parameters.Add(new MySqlParameter("name", item.name));
                        cmd.Parameters.Add(new MySqlParameter("description", item.description));
                        cmd.Parameters.Add(new MySqlParameter("context", item.context));
                        cmd.Parameters.Add(new MySqlParameter("version", item.version));
                        cmd.Parameters.Add(new MySqlParameter("provider", item.provider));
                        cmd.Parameters.Add(new MySqlParameter("status", item.status));
                        cmd.Parameters.Add(new MySqlParameter("visibility", item.visibility));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            Console.WriteLine("Fin insertAPIS (" + apis.list.Length + ")");
        }

        private static void exportarSuscripcionesAPIsOracle(IConfigurationSection config, string token, string tokenSub, string tokenApp, Models.ApiResult apis, string cs)
        {
            Console.WriteLine("---===  exportarSuscripcionesAPIs ===---");
            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;
            int limit = Convert.ToInt32(config.GetSection("API-Limit").Value);

            var httpClientHandler = new HttpClientHandler() { UseDefaultCredentials = true, ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };

            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            foreach (var item in apis.list)
            {
                Console.WriteLine("id : {0}, name: {1}, version: {2}", item.id, item.name, item.version);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(apiUrlPublisher + "/apis/" + item.id);
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();

                    usrRes.Wait();

                    var api = System.Text.Json.JsonSerializer.Deserialize<Models.Api>(usrRes.Result);

                    updateAPISOracle(cs, api);

                    //Console.WriteLine("updateAPIS OK ");
                }
                else
                {
                    Console.WriteLine("api/am/publisher/apis StatusCode : " + response.Result.StatusCode);
                    //return null;
                }

                //GET https://gateway.api.cloud.wso2.com/api/am/publisher/subscriptions?apiId=890a4f4d-09eb-4877-a323-57f6ce2ed79b
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenSub);
                response = client.GetAsync(apiUrlPublisher + "/subscriptions?apiId=" + item.id);
                response.Wait();

                //obtener respuesta
                if (response.Result.IsSuccessStatusCode)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();

                    usrRes.Wait();

                    var api = System.Text.Json.JsonSerializer.Deserialize<Models.SubscriptionResult>(usrRes.Result);

                    foreach (var itemSub in api.list)
                    {
                        //GET https://gateway.api.cloud.wso2.com/api/am/publisher/applications/896658a0-b4ee-4535-bbfa-806c894a4015
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenApp);
                        response = client.GetAsync(apiUrlPublisher + "/applications/" + itemSub.applicationId);
                        response.Wait();

                        //obtener respuesta
                        if (response.Result.IsSuccessStatusCode)
                        {
                            usrRes = response.Result.Content.ReadAsStringAsync();

                            usrRes.Wait();

                            var app = System.Text.Json.JsonSerializer.Deserialize<Models.Application>(usrRes.Result);

                            itemSub.applicationName = app.name;
                            itemSub.subscriber = app.subscriber;
                        }
                        else
                        {
                            Console.WriteLine("api/am/publisher/applications StatusCode : " + response.Result.StatusCode);
                            //return null;
                        }
                    }

                    insertSubsOracle(cs, item.id, api);

                    //Console.WriteLine("usrRes.Result : " + usrRes.Result);
                }
                else
                {
                    Console.WriteLine("api/am/publisher/subscriptions StatusCode : " + response.Result.StatusCode);
                    //return null;
                }
            }
        }

        private static void insertSubsOracle(string cs, string apiId, Models.SubscriptionResult subs)
        {
            Console.WriteLine("insertSubs");

            using (var conn = new OracleConnection(cs))
            {
                conn.Open();

                foreach (var item in subs.list)
                {
                    using (var cmd = new OracleCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO API_Subscriptions (api_id, tier, applicationId, applicationName, status, subscriber) VALUES (@api_id, @tier, @applicationId, @applicationName, @status, @subscriber)";
                        cmd.CommandType = System.Data.CommandType.Text;

                        cmd.Parameters.Add(new OracleParameter("api_id", apiId));
                        cmd.Parameters.Add(new OracleParameter("tier", item.tier));
                        cmd.Parameters.Add(new OracleParameter("applicationId", item.applicationId));
                        cmd.Parameters.Add(new OracleParameter("applicationName", item.applicationName));
                        cmd.Parameters.Add(new OracleParameter("status", item.status));
                        cmd.Parameters.Add(new OracleParameter("subscriber", item.subscriber));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            //Console.WriteLine("Fin insertSubs");
        }

        private static void updateAPISOracle(string cs, Models.Api api)
        {
            using (var conn = new OracleConnection(cs))
            {
                conn.Open();

                using (var cmd = new OracleCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "UPDATE RV_API SET endpointConfig = @endpointConfig WHERE id = @id";
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.Parameters.Add(new OracleParameter("id", api.id));
                    cmd.Parameters.Add(new OracleParameter("endpointConfig", api.endpointConfig));

                    cmd.ExecuteNonQuery();
                }
                if (api.endpointConfig != null && api.endpointConfig != "")
                {
                    try
                    {
                        if (api.endpointConfig.Contains("\"actionDuration\":30000"))
                        { api.endpointConfig = api.endpointConfig.Replace("\"actionDuration\":30000", "\"actionDuration\":\"30000\""); }

                        Models.EndpointConfig ep = System.Text.Json.JsonSerializer.Deserialize<Models.EndpointConfig>(api.endpointConfig); ;

                        //if (ep.production_endpoints.url != "" || ep.sandbox_endpoints.url != "")
                        {
                            using (var cmd = new OracleCommand())
                            {
                                cmd.Connection = conn;

                                cmd.CommandText = "UPDATE RV_API SET production_endpoints = @production_endpoints, sandbox_endpoints = @sandbox_endpoints WHERE id = @id";
                                cmd.CommandType = System.Data.CommandType.Text;

                                cmd.Parameters.Add(new OracleParameter("id", api.id));
                                cmd.Parameters.Add(new OracleParameter("production_endpoints", ep.production_endpoints == null ? "" : ep.production_endpoints.url));
                                cmd.Parameters.Add(new OracleParameter("sandbox_endpoints", ep.sandbox_endpoints == null ? "" : ep.sandbox_endpoints.url));

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        var m = e.Message;
                    }
                }
            }
        }

        public static void Main_DeprecarAPIS(string[] args) //inserta en mysql
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            ///appsettings - cloud - dtvqcm
            ///appsettings - prod

            IConfigurationSection config = configRoot.GetSection("MySettings");

            string apiUrl = config.GetSection("API-url").Value;
            string apiUrlPublisher = config.GetSection("API-Url-publisher").Value;

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);
            var clientDel = new HttpClient(httpClientHandler);
            clientDel.BaseAddress = new Uri(apiUrl);

            //pedir token
            string token = pedirToken(config, "&scope=apim:api_view");
            if (token.Length != 0)
            {
                Console.WriteLine("Token api_view : " + token);

                Models.ApiResult apis = exportarAPIs(config, token);

                string tokenPub = pedirToken(config, "&scope=apim:api_publish");
                Console.WriteLine("Token api_publish : " + tokenPub);

                string tokenCreate = pedirToken(config, "&scope=apim:api_create");
                Console.WriteLine("Token api_create : " + tokenCreate);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenPub);

                clientDel.DefaultRequestHeaders.Accept.Clear();
                clientDel.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientDel.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenCreate);

                foreach (var api in apis.list)
                {
                    Console.WriteLine("api: " + api.name + ", status: " + api.status);
                    var newStatus = "";
                    bool deleteApi = false;
                    if (api.status.Equals("PUBLISHED"))
                    {
                        newStatus = "Deprecate";
                    }
                    if (api.status.Equals("DEPRECATED"))
                    {
                        newStatus = "Retire";
                    }
                    if (api.status.Equals("RETIRED") || api.status.Equals("CREATED") || api.status.Equals("PROTOTYPED"))
                    {
                        deleteApi = true;
                    }
                    if (!newStatus.Equals(""))
                    {
                        //cambiar estado
                        //https://localhost:9443/api/am/publisher/v0.14/apis/change-lifecycle?apiId=890a4f4d-09eb-4877-a323-57f6ce2ed79b&action=Publish

                        var content = new StringContent("", Encoding.UTF8, "application/json");
                        var response2 = client.PostAsync(apiUrlPublisher + "/apis/change-lifecycle?apiId=" + api.id + "&action=" + newStatus, content);
                        response2.Wait();

                        //obtener respuesta
                        if (response2.Result.IsSuccessStatusCode)
                        {
                            Console.WriteLine("    * cambiada");
                        }
                        else
                        {
                            //error
                            Console.WriteLine("ERROR, NO cambiada");
                        }
                    }
                    if (deleteApi)
                    {
                        var response2 = clientDel.DeleteAsync(apiUrlPublisher + "/apis/" + api.id);
                        response2.Wait();
                        
                        //obtener respuesta
                        if (response2.Result.IsSuccessStatusCode)
                        {
                            Console.WriteLine("    * eliminada");
                        }
                        else
                        {
                            //error
                            Console.WriteLine("ERROR, NO eliminada");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Token NO obtenido");
            }

            Console.WriteLine("Fin");
        }
    }
}