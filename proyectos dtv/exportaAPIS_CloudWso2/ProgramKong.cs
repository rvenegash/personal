
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using exportaAPIS_CloudWso2.Models;
using Microsoft.Extensions.Configuration;

namespace exportaAPIS_CloudWso2
{
    public static class ProgramKong
    {
        public static void CreaApisDesdeJsonWso2EnKong(string[] args) //crea doc en kong, desde carpeta , 
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqcm.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;

            string pathFiles = "/api-cli/open-api/files/";

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);
            //client.DefaultRequestHeaders.Add("Host", "api-manager-qcm.dtvdev.net");

            var files = Directory.GetFiles(rutaDestino, "*.json");
            //var files = Directory.GetFiles(@"/mnt/c/TEMP/apisqcm", "*.json");
            foreach (var item in files)
            {
                var sr = new StreamReader(item);
                var apiText = sr.ReadToEnd();
                sr.Close();

                var apiObj = System.Text.Json.JsonSerializer.Deserialize<Models.Api>(apiText);
                if (apiObj != null)
                {
                    var nombreArchivo = "specs/" + apiObj.name + "_" + apiObj.version + ".json";
                    Console.WriteLine("archivo : " + Path.GetFileName(item) + ", " + apiObj.status);

                    if (apiObj.status == "PROTOTYPED" || apiObj.status == "CREATED" || apiObj.status == "DEPRECATED")
                    {
                        //si existe eliminar
                        var response = client.GetAsync(pathFiles + nombreArchivo);
                        response.Wait();
                        Console.WriteLine(response.Result.StatusCode);

                        if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            //eliminar                        
                            var responseDel = client.DeleteAsync(pathFiles + nombreArchivo);
                            responseDel.Wait();
                            Console.WriteLine("eliminada " + nombreArchivo);
                        }
                    }

                    if (apiObj.status == "PUBLISHED")
                    {
                        var response = client.GetAsync(pathFiles + nombreArchivo);
                        response.Wait();
                        Console.WriteLine(response.Result.StatusCode);

                        bool crear = false;
                        if (response.Result.StatusCode == System.Net.HttpStatusCode.NotFound) //no existe, se crea
                        {
                            crear = true;
                        }
                        else if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            //eliminar                        
                            var responseDel = client.DeleteAsync(pathFiles + nombreArchivo);
                            responseDel.Wait();
                            Console.WriteLine("eliminada " + nombreArchivo);
                            crear = true; //cambiar a false luego
                        }
                        else
                        {
                            Console.WriteLine("api no valida " + nombreArchivo);
                        }

                        if (crear)
                        {
                            var usrRes = response.Result.Content.ReadAsStringAsync();
                            usrRes.Wait();

                            var swagger = apiObj.apiDefinition;
                            JsonNode document = JsonNode.Parse(swagger)!;
                            JsonNode root = document.Root;
                            string esSwagger = (string)root["swagger"]!;
                            string esOpenapi = (string)root["openapi"]!;

                            string host = (string)root["host"]!;
                            string basePath = (string)root["basePath"]!;

                            if (string.IsNullOrEmpty(host))
                            {
                                root["host"] = hostKong;
                            }
                            else
                            {
                                host = host.Replace("gateway.api.cloud.wso2.com:443", hostKong);
                                host = host.Replace("apigateway-qcp.dtvdev.net:443", hostKong);
                                host = host.Replace("apigateway-qcm.dtvdev.net:443", hostKong);
                                host = host.Replace("apigateway-qam.dtvdev.net:443", hostKong);
                                host = host.Replace("apigateway-qam.dtvpan.com:443", hostKong);
                                host = host.Replace("apigateway.dtvpan.com:443", hostKong);
                                root["host"] = host;
                            }
                            if (string.IsNullOrEmpty(basePath))
                            {
                                root["basePath"] = string.Format("/API/{0}/{1}", apiObj.name, apiObj.version);
                            }
                            else
                            {
                                basePath = basePath.Replace("/t/dtvqcp", "");
                                basePath = basePath.Replace("/t/dtvqcm", "");
                                basePath = basePath.Replace("/t/dtvdev", "");
                                basePath = basePath.Replace("/t/dtvqam", "");
                                basePath = basePath.Replace("/t/dtvla1179", "");

                                if (!basePath.EndsWith(apiObj.version))
                                {
                                    basePath = basePath + "/" + apiObj.version;
                                }
                                root["basePath"] = basePath;
                            }
                            var options = new JsonSerializerOptions { WriteIndented = true };
                            swagger = document.ToJsonString(options);

                            var kongFile = new KongFile()
                            {
                                path = nombreArchivo,
                                contents = swagger
                            };
                            var kongFileObj = JsonSerializer.Serialize(kongFile);

                            var content = new StringContent(kongFileObj, Encoding.UTF8, "application/json");

                            var response2 = client.PostAsync(pathFiles, content);
                            response2.Wait();

                            //obtener respuesta
                            if (response2.Result.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Creada " + nombreArchivo);
                                //File.Move(item, item.Replace(".json", ".bak"));
                            }
                            else
                            {
                                //error
                                var mensaje = "";
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Fin");
        }

        public static void ExportaApisKongTxt(string[] args) //exporta apis de kong 
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;

            string pathAdmin = "/api-cli/open-api/";

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);
            //client.DefaultRequestHeaders.Add("Host", "api-manager-qcm.dtvdev.net");

            var archivo = rutaDestino + "\\listado-apis-kong.txt";
            Console.WriteLine(archivo);
            StreamWriter sw = new StreamWriter(archivo, false);

            bool continuar = true;
            string offset = "/routes";
            do
            {
                var response = client.GetAsync(pathAdmin + offset);
                response.Wait();
                Console.WriteLine(response.Result.StatusCode);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();
                    usrRes.Wait();

                    var app = JsonSerializer.Deserialize<Models.KongRoutes>(usrRes.Result);
                    if (app != null)
                    {
                        foreach (var i in app.data)
                        {
                            Console.WriteLine(i.name);

                            //buscar nombre del servicio/url
                            var resServ = client.GetAsync(pathAdmin + "/services/" + i.service.id);
                            resServ.Wait();
                            if (resServ.Result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var strServ = resServ.Result.Content.ReadAsStringAsync();
                                strServ.Wait();

                                var serv = JsonSerializer.Deserialize<Models.KongService>(strServ.Result);
                                if (serv != null)
                                {
                                    i.service = serv;
                                }
                            }

                            sw.WriteLine("{0},{1},{2},{3},{4}", i.id, i.name, i.paths[0], i.service.host, i.service.path);
                        }

                        if (string.IsNullOrEmpty(app.next))
                        {
                            continuar = false;
                        }
                        else
                        {
                            offset = app.next;
                        }
                    }
                    else
                    {
                        continuar = false;
                    }
                }
            } while (continuar);

            sw.Close();

            Console.WriteLine("Fin");
        }

          public static void ExportaACLSRutasKongTxt(string[] args) //exporta apis de kong 
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqcm.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;

            string pathAdmin = "/api-cli/open-api/";

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);
            //client.DefaultRequestHeaders.Add("Host", "api-manager-qcm.dtvdev.net");

            var archivo = rutaDestino + "\\listado_ruta_acls.txt";
            Console.WriteLine(archivo);
            StreamWriter sw = new StreamWriter(archivo, false);

            bool continuar = true;
            string offset = "/routes";
            do
            {
                var response = client.GetAsync(pathAdmin + offset);
                response.Wait();
                Console.WriteLine(response.Result.StatusCode);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();
                    usrRes.Wait();

                    var app = JsonSerializer.Deserialize<Models.KongRoutes>(usrRes.Result);
                    if (app != null)
                    {
                        foreach (var i in app.data)
                        {
                            Console.WriteLine(i.name);

                            //buscar nombre del servicio/url
                            var resServ = client.GetAsync(pathAdmin + "/routes/" + i.id + "/plugins");
                            resServ.Wait();
                            if (resServ.Result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var strServ = resServ.Result.Content.ReadAsStringAsync();
                                strServ.Wait();

                                JsonNode document = JsonNode.Parse(strServ.Result)!;
                                JsonNode root = document.Root;
                                JsonArray appData = (JsonArray)root["data"];
                                string appNext = (string)root["next"];
                                
                                foreach (JsonObject iAcl in appData)
                                {
                                    string piId = (string)iAcl["id"]!;
                                    string piName = (string)iAcl["name"]!;
                                    
                                    if (piName=="acl")
                                    {
                                        string allow = (string)iAcl["config"]["allow"][0]!;

                                        Console.WriteLine("allow : " + allow);

                                        sw.WriteLine("{0},{1},{2},{3}", i.id, i.name, i.paths[0], allow);
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(app.next))
                        {
                            continuar = false;
                        }
                        else
                        {
                            offset = app.next;
                        }
                    }
                    else
                    {
                        continuar = false;
                    }
                }
            } while (continuar);

            sw.Close();

            Console.WriteLine("Fin");
        }

        public static void DeshabilitarPluginsACL(string[] args)
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;

            string pathAdmin = "/api-cli/open-api/";

            var httpClientHandler = new HttpClientHandler() { };
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);
            client.DefaultRequestHeaders.Add("Host", hostKong);

            var archivo = rutaDestino + "\\listado-pi.txt";
            Console.WriteLine(archivo);
            StreamWriter sw = new StreamWriter(archivo, false);

            bool continuar = true;
            string offset = "/plugins";
            do
            {
                var response = client.GetAsync(pathAdmin + offset + "?enabled=true");
                response.Wait();
                Console.WriteLine(response.Result.StatusCode);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();
                    usrRes.Wait();

                    JsonNode document = JsonNode.Parse(usrRes.Result)!;
                    JsonNode root = document.Root;
                    JsonArray appData = (JsonArray)root["data"];
                    string appNext = (string)root["next"];

                    foreach (JsonObject i in appData)
                    {
                        string apiId = (string)i["id"]!;
                        string apiName = (string)i["name"]!;
                        string apiInstance = (string)i["instance_name"]!;

                        Console.WriteLine("{0},{1}", apiName, apiInstance);
                        if (apiName == "acl")
                        {
                            //deshabilitar plugin
                            i["enabled"] = false;
                            var kongFileObj = JsonSerializer.Serialize(i);

                            var content = new StringContent(kongFileObj, Encoding.UTF8, "application/json");

                            var response2 = client.PutAsync(pathAdmin + "plugins/" + apiId, content);
                            response2.Wait();

                            //obtener respuesta
                            if (response2.Result.IsSuccessStatusCode)
                            {
                                Console.WriteLine("actualizada " + apiInstance);
                            }

                            sw.WriteLine("{0},{1},{2}", apiId, apiInstance, ""); //, i.config.allow[0]
                        }
                    }

                    Console.WriteLine("app.next " + appNext);
                    if (string.IsNullOrEmpty(appNext))
                    {
                        continuar = false;
                    }
                    else
                    {
                        offset = appNext;
                    }
                }
            } while (continuar);

            sw.Close();

            Console.WriteLine("Fin");
        }

        public static void CreaRolesDesdeArchivo(string[] args)
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;
            string archivoRoles = config.GetSection("archivoRoles").Value;

            string pathFiles = "/api-cli/open-api/developers/roles/";

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);


            StreamReader sr = new StreamReader(archivoRoles);
            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine();
                if (!string.IsNullOrEmpty(linea))
                {
                    var response = client.GetAsync(pathFiles + linea);
                    response.Wait();
                    //Console.WriteLine(response.Result.StatusCode);

                    if (response.Result.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        //crear
                        var kongFile = new KongRole()
                        {
                            name = linea
                        };
                        var kongFileObj = JsonSerializer.Serialize(kongFile);

                        var content = new StringContent(kongFileObj, Encoding.UTF8, "application/json");

                        var responsePost = client.PostAsync(pathFiles, content);
                        responsePost.Wait();
                        if (responsePost.Result.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            Console.WriteLine("creada " + linea);
                        }
                        else
                        {
                            Console.WriteLine("error " + linea);

                        }
                    }
                    else
                    {
                        Console.WriteLine("existe " + linea);

                    }
                }
            }

            Console.WriteLine("Fin");
        }

        public static void CreaRolesParaDevDesdeArchivo(string[] args)
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;
            string archivoRolesDev = config.GetSection("archivoRolesDev").Value;

            string pathFiles = "/api-cli/open-api/developers/";

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);

            StreamReader sr = new StreamReader(archivoRolesDev);
            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine();
                if (!string.IsNullOrEmpty(linea))
                {
                    var arrLin = linea.Split("\t");
                    var developer = arrLin[0];
                    var roles = arrLin[2];

                    var response = client.GetAsync(pathFiles + developer);
                    response.Wait();
                    //Console.WriteLine(response.Result.StatusCode);

                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //crear
                        var kongFileObj = "{\"roles\": [" + roles + "] }";

                        var content = new StringContent(kongFileObj, Encoding.UTF8, "application/json");

                        var responsePost = client.PatchAsync(pathFiles + developer, content);
                        responsePost.Wait();
                        if (responsePost.Result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine("actualizada " + linea);
                        }
                        else
                        {
                            Console.WriteLine("error " + linea);
                        }
                    }
                    else
                    {
                        Console.WriteLine("NO existe " + linea);
                    }
                }
            }

            Console.WriteLine("Fin");
        }


        public static void ExportaRolesApisDesdeJsonWso2(string[] args)
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string archivoRolesExp = config.GetSection("archivoRolesExp").Value;

            var files = Directory.GetFiles(rutaDestino, "*.json");

            StreamWriter sw = new StreamWriter(archivoRolesExp, false);

            foreach (var item in files)
            {
                var sr = new StreamReader(item);
                var apiText = sr.ReadToEnd();
                sr.Close();

                var apiObj = JsonSerializer.Deserialize<Models.Api>(apiText);
                if (apiObj != null)
                {
                    Console.WriteLine("archivo : " + Path.GetFileName(item) + ", " + apiObj.status);

                    if (apiObj.accessControlRoles != null)
                    {
                        foreach (string s in apiObj.visibleRoles)
                        {
                            sw.WriteLine(s);
                        }
                    }
                }
            }
            sw.Close();
            Console.WriteLine("Fin");
        }

        public static void CambiaRolesFilesSpec(string[] args)
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;

            string pathAdmin = "/api-cli/";

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);

            bool continuar = true;
            string offset = "/open-api/files";
            do
            {
                var response = client.GetAsync(pathAdmin + offset);
                response.Wait();
                Console.WriteLine(response.Result.StatusCode);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();
                    usrRes.Wait();

                    var app = JsonSerializer.Deserialize<Models.KongFiles>(usrRes.Result);
                    if (app != null)
                    {
                        foreach (var i in app.data)
                        {
                            if (i.path.StartsWith("specs/"))
                            {
                                Console.WriteLine(i.path + " " + i.contents.Length);

                                var swagger = i.contents;
                                JsonNode document = JsonNode.Parse(swagger)!;
                                JsonNode root = document.Root;
                                string esSwagger = (string)root["swagger"]!;
                                string esOpenapi = (string)root["openapi"]!;

                                string apiName = (string)root["info"]["title"]!;
                                apiName += "Role";
                                var headmatter = root["x-headmatter"]!;
                                string basePath = (string)root["basePath"]!;

                                Console.WriteLine(apiName);

                                var actualizar = false;
                                if (false) //reemplaza y agrega headmatter
                                {
                                    apiName = apiName.Replace("CustomerBillRole", "CustomerBillManagementRole");
                                    apiName = apiName.Replace("PaymentManagmentRole", "PaymentManagementRole");
                                    apiName = apiName.Replace("Resource Identification ManagementRole", "ResourceInventoryManagementRole");
                                    apiName = apiName.Replace("API Qualification ManagementRole", "QualificationManagementRole");

                                    //"x-headmatter" : {   "readable_by" : ["role_name"]   }
                                    if (headmatter is null)
                                    {
                                        var readable = new JsonObject { ["readable_by"] = new JsonArray(apiName) };
                                        root["x-headmatter"] = readable;
                                        actualizar = true;
                                    }
                                }
                                else //deja headmatter en null
                                {
                                    ((JsonObject)root).Remove("x-headmatter");
                                    actualizar = true;
                                }
                                if (actualizar)
                                {
                                    var options = new JsonSerializerOptions { WriteIndented = true };
                                    swagger = document.ToJsonString(options);

                                    var kongFile = new KongFile()
                                    {
                                        path = i.path,
                                        contents = swagger
                                    };
                                    var kongFileObj = JsonSerializer.Serialize(kongFile);

                                    var content = new StringContent(kongFileObj, Encoding.UTF8, "application/json");

                                    var pathFiles = pathAdmin + "/open-api/files/" + i.path;
                                    var response2 = client.PutAsync(pathFiles, content);
                                    response2.Wait();

                                    if (response2.Result.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine("actualizada " + i.path);
                                    }
                                    else
                                    {
                                        Console.WriteLine("ERROR AL actualizar " + i.path + " " + response2.Result.StatusCode);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("no se actualizada " + i.path);
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(app.next))
                        {
                            continuar = false;
                        }
                        else
                        {
                            offset = app.next;
                        }
                    }
                    else
                    {
                        continuar = false;
                    }
                }
            } while (continuar);

            Console.WriteLine("Fin");
        }


        public static void ExportaFilesSpec(string[] args)
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestinoSwagger").Value;
            string hostKong = config.GetSection("hostKong").Value;

            string pathAdmin = "/api-cli/";

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);

            bool continuar = true;
            string offset = "/open-api/files";
            do
            {
                var response = client.GetAsync(pathAdmin + offset);
                response.Wait();
                Console.WriteLine(response.Result.StatusCode);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();
                    usrRes.Wait();

                    var app = JsonSerializer.Deserialize<Models.KongFiles>(usrRes.Result);
                    if (app != null)
                    {
                        foreach (var i in app.data)
                        {
                            if (i.path.StartsWith("specs/"))
                            {
                                var swagger = i.contents;

                                StreamWriter sw = new StreamWriter(rutaDestino + "\\" + i.path.Replace("specs/", ""));
                                sw.Write(swagger);
                                sw.Close();

                                Console.WriteLine(i.path + " " + i.contents.Length);
                            }
                        }

                        if (string.IsNullOrEmpty(app.next))
                        {
                            continuar = false;
                        }
                        else
                        {
                            offset = app.next;
                        }
                    }
                    else
                    {
                        continuar = false;
                    }
                }
            } while (continuar);

            Console.WriteLine("Fin");
        }

        public static void EditaServicesTimeout(string[] args)
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;

            string pathAdmin = "/api-cli/open-api/";

            var httpClientHandler = new HttpClientHandler() { };
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);
            client.DefaultRequestHeaders.Add("Host", hostKong);

            bool continuar = true;
            string offset = "services";
            do
            {
                var response = client.GetAsync(pathAdmin + offset); //+ "?enabled=true"
                response.Wait();
                Console.WriteLine(response.Result.StatusCode);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();
                    usrRes.Wait();

                    JsonNode document = JsonNode.Parse(usrRes.Result)!;
                    JsonNode root = document.Root;
                    JsonArray appData = (JsonArray)root["data"];
                    string appNext = (string)root["next"];

                    foreach (JsonObject i in appData)
                    {
                        string apiId = (string)i["id"]!;
                        string apiName = (string)i["name"]!;
                        int timeout = (int)i["read_timeout"]!;
                        int retries = (int)i["retries"]!;

                        Console.WriteLine("{0},{1},{2}", apiName, retries, timeout);

                        //deshabilitar plugin
                        int timeoutTarget = 120000;
                        if (timeout != timeoutTarget && true)
                        {
                            i["connect_timeout"] = 30000;
                            i["read_timeout"] = timeoutTarget;
                            i["write_timeout"] = timeoutTarget;
                            i["retries"] = 0;
                            var kongFileObj = JsonSerializer.Serialize(i);

                            var content = new StringContent(kongFileObj, Encoding.UTF8, "application/json");

                            var response2 = client.PutAsync(pathAdmin + "services" + "/" + apiId, content);
                            response2.Wait();

                            //obtener respuesta
                            if (response2.Result.IsSuccessStatusCode)
                            {
                                Console.WriteLine("    actualizada " + apiName);
                            }
                        }
                    }

                    Console.WriteLine("app.next " + appNext);
                    if (string.IsNullOrEmpty(appNext))
                    {
                        continuar = false;
                    }
                    else
                    {
                        offset = appNext;
                    }
                }
            } while (continuar);

            Console.WriteLine("Fin");
        }



        public static void MigrarCuentasPortalAConsumer(string[] args)
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;

            string pathAdmin = "/api-cli/open-api/";

            //logica:
            //obtener developers, GET https://api-manager-qam.dtvpan.com/api-cli/open-api/developers
            //por cada developer, obtener sus apps, GET https://api-manager-qam.dtvpan.com/api-cli/open-api/developers/06e9da4c-0097-4cd5-8b39-4e7f3970497b/applications
            //por cada app, obtener sus credenciales, GET https://api-manager-qam.dtvpan.com/api-cli/open-api/developers/06e9da4c-0097-4cd5-8b39-4e7f3970497b/applications/1979ba54-abfb-433f-a7d2-8aabcb477bd4/credentials/oauth2
            //chequear si existe el consumer GET https://api-manager-qam.dtvpan.com/api-cli/open-api/consumers?custom_id=S_EC_QAM_ARESLAAR@dtvpan.com
            //si no existe, crear POST https://api-manager-qam.dtvpan.com/api-cli/open-api/consumers
            //chequear si no existe la credencial, GET https://api-manager-qam.dtvpan.com/api-cli/open-api/consumers/9e01cd2c-cdca-4641-8c04-fd54a0852b97/oauth2
            //borrar credencial en app, DELETE GET https://api-manager-qam.dtvpan.com/api-cli/open-api/developers/06e9da4c-0097-4cd5-8b39-4e7f3970497b/applications/1979ba54-abfb-433f-a7d2-8aabcb477bd4/credentials/oauth2
            //crearle la credencial POST https://api-manager-qam.dtvpan.com/api-cli/open-api/consumers/9e01cd2c-cdca-4641-8c04-fd54a0852b97/oauth2
            //

            var httpClientHandler = new HttpClientHandler() { };
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);
            client.DefaultRequestHeaders.Add("Host", hostKong);

            var archivo = rutaDestino + "\\cred-developers.txt";
            Console.WriteLine(archivo);
            StreamWriter sw = new StreamWriter(archivo, false);

            bool continuar = true;
            string offset = "/developers";
            do
            {
                var response = client.GetAsync(pathAdmin + offset);
                response.Wait();
                Console.WriteLine(response.Result.StatusCode);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();
                    usrRes.Wait();

                    JsonNode document = JsonNode.Parse(usrRes.Result)!;
                    JsonNode root = document.Root;
                    JsonArray appData = (JsonArray)root["data"];
                    string appNext = (string)root["next"];

                    foreach (JsonObject i in appData)
                    {
                        string devId = (string)i["id"]!;
                        string devName = (string)i["email"]!;

                        if (devName.StartsWith("local_"))
                        {
                            Console.WriteLine("{0},{1}", devName, devId);

                            //GET https://api-manager-qam.dtvpan.com/api-cli/open-api/consumers?username=S_EC_QAM_ARESLAAR@dtvpan.com
                            var newDevName = devName.Replace("local_", "");
                            var responseCon = client.GetAsync(pathAdmin + "consumers?username=" + newDevName);
                            responseCon.Wait();

                            bool continuarCons = true;
                            string consumerId = "";
                            if (responseCon.Result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var appResCon = responseCon.Result.Content.ReadAsStringAsync();
                                appResCon.Wait();

                                JsonNode documentCon = JsonNode.Parse(appResCon.Result)!;
                                JsonNode rootCon = documentCon.Root;
                                JsonArray appDataCon = (JsonArray)rootCon["data"];
                                string appNextCon = (string)rootCon["next"];
                                bool crearConsumer = true;
                                foreach (JsonObject iCon in appDataCon)
                                {
                                    consumerId = (string)iCon["id"]!;

                                    Console.WriteLine("EXISTE, NO CREAR: {0}", newDevName);
                                    crearConsumer = false;
                                }
                                if (crearConsumer)
                                {
                                    Console.WriteLine("NO EXISTE, CREAR: {0}", newDevName);
                                    //POST https://api-manager-qam.dtvpan.com/api-cli/open-api/consumers
                                    //crear
                                    var kongConsumer = new KongConsumerReq()
                                    {
                                        custom_id = newDevName
                                    };
                                    var kongConsumerObj = JsonSerializer.Serialize(kongConsumer);

                                    var content = new StringContent(kongConsumerObj, Encoding.UTF8, "application/json");

                                    var responsePost = client.PostAsync(pathAdmin + "consumers", content);
                                    responsePost.Wait();
                                    if (responsePost.Result.StatusCode == System.Net.HttpStatusCode.Created)
                                    {
                                        var usrPostCons = responsePost.Result.Content.ReadAsStringAsync();
                                        usrPostCons.Wait();
                                        consumerId = System.Text.Json.JsonSerializer.Deserialize<Models.KongConsumer>(usrPostCons.Result).id;

                                        Console.WriteLine("creada {0},{1}", consumerId, newDevName);
                                    }
                                    else
                                    {
                                        Console.WriteLine("error " + newDevName);
                                        continuarCons = false;
                                    }
                                }
                                Console.WriteLine("   consumerId {0}", consumerId);
                            }
                            else
                            {
                                //Console.WriteLine("EXISTE, NO CREAR: {0}", newDevName);
                            }

                            //GET https://api-manager-qam.dtvpan.com/api-cli/open-api/developers/06e9da4c-0097-4cd5-8b39-4e7f3970497b/applications
                            var responseDev = client.GetAsync(pathAdmin + offset + "/" + devId + "/applications");
                            responseDev.Wait();

                            if (responseDev.Result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var appRes = responseDev.Result.Content.ReadAsStringAsync();
                                appRes.Wait();

                                JsonNode documentApp = JsonNode.Parse(appRes.Result)!;
                                JsonNode rootApp = documentApp.Root;
                                JsonArray appDataApp = (JsonArray)rootApp["data"];
                                string appNextApp = (string)rootApp["next"];

                                foreach (JsonObject iApp in appDataApp)
                                {
                                    string appId = (string)iApp["id"]!;
                                    string appName = (string)iApp["name"]!;

                                    Console.WriteLine("  {0},{1}", appId, appName);

                                    //GET https://api-manager-qam.dtvpan.com/api-cli/open-api/developers/06e9da4c-0097-4cd5-8b39-4e7f3970497b/applications/1979ba54-abfb-433f-a7d2-8aabcb477bd4/credentials/oauth2
                                    var responseCred = client.GetAsync(pathAdmin + offset + "/" + devId + "/applications/" + appId + "/credentials/oauth2");
                                    responseCred.Wait();

                                    if (responseCred.Result.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        var appResCred = responseCred.Result.Content.ReadAsStringAsync();
                                        appResCred.Wait();

                                        JsonNode documentAppCred = JsonNode.Parse(appResCred.Result)!;
                                        JsonNode rootAppCred = documentAppCred.Root;
                                        JsonArray appDataAppCred = (JsonArray)rootAppCred["data"];
                                        string appNextAppCred = (string)rootAppCred["next"];

                                        foreach (JsonObject iAppCred in appDataAppCred)
                                        {
                                            string customerCredId = (string)iAppCred["id"]!;
                                            string customerId = (string)iAppCred["client_id"]!;
                                            string customerSecret = (string)iAppCred["client_secret"]!;

                                            Console.WriteLine("    {0},{1}", customerId, customerSecret);
                                            if (!string.IsNullOrEmpty(customerId)) //tiene consumer id
                                            {
                                                //continuo con las apps
                                                if (continuarCons)
                                                {
                                                    //GET https://api-manager-qam.dtvpan.com/api-cli/open-api/consumers/9e01cd2c-cdca-4641-8c04-fd54a0852b97/oauth2
                                                    var responseCredCons = client.GetAsync(pathAdmin + "consumers/" + consumerId + "/oauth2");
                                                    responseCredCons.Wait();

                                                    if (responseCredCons.Result.StatusCode == System.Net.HttpStatusCode.OK)
                                                    {
                                                        var appResCredCons = responseCredCons.Result.Content.ReadAsStringAsync();
                                                        appResCredCons.Wait();

                                                        JsonNode documentAppCredCons = JsonNode.Parse(appResCredCons.Result)!;
                                                        JsonNode rootAppCredCons = documentAppCredCons.Root;
                                                        JsonArray appDataAppCredCons = (JsonArray)rootAppCredCons["data"];
                                                        string appNextAppCredCons = (string)rootAppCredCons["next"];

                                                        if (appDataAppCredCons != null)
                                                        {
                                                            bool crearApp = true;
                                                            foreach (JsonObject iAppCredCons in appDataAppCredCons)
                                                            {
                                                                string newAppName = (string)iAppCredCons["name"]!; ;
                                                                if (newAppName == appName)
                                                                {
                                                                    crearApp = false;
                                                                }
                                                            }

                                                            if (crearApp)
                                                            {
                                                                sw.WriteLine("{0},{1},{2},{3},{4},{5}", devId, devName, appId, appName, customerId, customerSecret);

                                                                //crear credenciales
                                                                Console.WriteLine("creando credenciales para " + appName);
                                                                //borrar credencial en app, DELETE GET https://api-manager-qam.dtvpan.com/api-cli/open-api/developers/06e9da4c-0097-4cd5-8b39-4e7f3970497b/applications/1979ba54-abfb-433f-a7d2-8aabcb477bd4/credentials/oauth2
                                                                var responseDel = client.DeleteAsync(pathAdmin + offset + "/" + devId + "/applications/" + appId + "/credentials/oauth2/" + customerCredId);
                                                                responseDel.Wait();
                                                                if (responseDel.Result.StatusCode == System.Net.HttpStatusCode.Created)
                                                                {
                                                                }


                                                                //crearle la credencial POST https://api-manager-qam.dtvpan.com/api-cli/open-api/consumers/9e01cd2c-cdca-4641-8c04-fd54a0852b97/oauth2
                                                                var kongConsumer = new KongConsumerCredential()
                                                                {
                                                                    name = appName,
                                                                    client_id = customerId,
                                                                    client_secret = customerSecret,
                                                                    client_type = "confidential"
                                                                };
                                                                var kongConsumerObj = JsonSerializer.Serialize(kongConsumer);

                                                                var content = new StringContent(kongConsumerObj, Encoding.UTF8, "application/json");

                                                                var responsePost = client.PostAsync(pathAdmin + "consumers/" + consumerId + "/oauth2", content);
                                                                responsePost.Wait();
                                                                if (responsePost.Result.StatusCode == System.Net.HttpStatusCode.Created)
                                                                {
                                                                    var usrPostCons = responsePost.Result.Content.ReadAsStringAsync();
                                                                    usrPostCons.Wait();
                                                                    var newconsumerId = System.Text.Json.JsonSerializer.Deserialize<Models.KongConsumer>(usrPostCons.Result).id;

                                                                    Console.WriteLine("creada {0},{1}", appName, newconsumerId);
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("error " + appName);
                                                                    continuarCons = false;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }

                                Console.WriteLine("-----------------------------------------------------------");
                            }
                        }
                    }

                    Console.WriteLine("app.next " + appNext);
                    if (string.IsNullOrEmpty(appNext))
                    {
                        continuar = false;
                    }
                    else
                    {
                        offset = appNext;
                    }
                }
            } while (continuar);

            sw.Close();

            Console.WriteLine("Fin");
        }

        public static void CreaRolesParaConsumersDesdeArchivo(string[] args)
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;
            string archivoRolesDev = config.GetSection("archivoRolesCons").Value;

            string pathFiles = "/api-cli/open-api/consumers/";

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("host", hostKong);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);

            StreamReader sr = new StreamReader(archivoRolesDev);
            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine();
                if (!string.IsNullOrEmpty(linea))
                {
                    var arrLin = linea.Split("\t");
                    var consumerName = arrLin[1].ToUpper();
                    var roles = arrLin[0].Trim();

                    var response = client.GetAsync(pathFiles + "?username=" + consumerName);
                    //var response = client.GetAsync(pathFiles + consumerName); //por id
                    response.Wait();
                    Console.WriteLine(response.Result.StatusCode);
                    string consumerId = "";
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var appResCon = response.Result.Content.ReadAsStringAsync();
                        appResCon.Wait();

                        JsonNode documentCon = JsonNode.Parse(appResCon.Result)!;
                        JsonNode rootCon = documentCon.Root;
                        JsonArray appDataCon = (JsonArray)rootCon["data"];
                        string appNextCon = (string)rootCon["next"];

                        foreach (JsonObject iCon in appDataCon)
                        {
                            consumerId = (string)iCon["id"]!;
                        }

                        if (!string.IsNullOrEmpty(consumerId))
                        {
                            Console.WriteLine("{0} {1} {2}", consumerName, consumerId, roles);
                            //crear
                            var kongFileObj = "{\"group\": \"" + roles + "\" }";

                            var content = new StringContent(kongFileObj, Encoding.UTF8, "application/json");

                            var responsePost = client.PostAsync(pathFiles + consumerId + "/acls", content);
                            responsePost.Wait();
                            if (responsePost.Result.IsSuccessStatusCode)
                            {
                                Console.WriteLine("creada " + linea);
                            }
                            else
                            {
                                Console.WriteLine("error " + linea);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("NO existe " + linea);
                    }
                }
            }

            Console.WriteLine("Fin");
        }


        public static void CreaConsumersDesdeArchivo(string[] args)
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;
            string archivoRolesDev = config.GetSection("archivoConsumers").Value;

            string pathFiles = "/api-cli/open-api/consumers/";

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("host", hostKong);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);

            StreamReader sr = new StreamReader(archivoRolesDev);
            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine();
                if (!string.IsNullOrEmpty(linea))
                {
                    var consumerName = linea.Trim().ToUpper();

                    var response = client.GetAsync(pathFiles + "?username=" + consumerName);
                    response.Wait();
                    string consumerId = "";
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine(consumerName);

                        var appResCon = response.Result.Content.ReadAsStringAsync();
                        appResCon.Wait();

                        JsonNode documentCon = JsonNode.Parse(appResCon.Result)!;
                        JsonNode rootCon = documentCon.Root;
                        JsonArray appDataCon = (JsonArray)rootCon["data"];
                        string appNextCon = (string)rootCon["next"];

                        foreach (JsonObject iCon in appDataCon)
                        {
                            consumerId = (string)iCon["id"]!;
                        }
                        if (string.IsNullOrEmpty(consumerId))
                        {
                            //crear
                            var kongFileObj = "{\"username\": \"" + consumerName + "\" }";

                            var content = new StringContent(kongFileObj, Encoding.UTF8, "application/json");

                            var responsePost = client.PostAsync(pathFiles, content);
                            responsePost.Wait();
                            if (responsePost.Result.IsSuccessStatusCode)
                            {
                                Console.WriteLine("creada " + linea);
                            }
                            else
                            {
                                Console.WriteLine("error " + linea);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("NO existe " + linea);
                    }
                }
            }

            Console.WriteLine("Fin");
        }

        public static void ExportaConsumersRolesAArchivo(string[] args)
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;
            string archivoExport = rutaDestino + "/consumers_roles.csv";

            string pathAdmin = "/api-cli/open-api/consumers/";

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("host", hostKong);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);

            StreamWriter sw = new StreamWriter(archivoExport);

            var response = client.GetAsync(pathAdmin);
            response.Wait();
            Console.WriteLine(response.Result.StatusCode);
            string consumerId = "";
            string consumerName = "";
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var appResCon = response.Result.Content.ReadAsStringAsync();
                appResCon.Wait();

                JsonNode documentCon = JsonNode.Parse(appResCon.Result)!;
                JsonNode rootCon = documentCon.Root;
                JsonArray appDataCon = (JsonArray)rootCon["data"];
                string appNextCon = (string)rootCon["next"];

                foreach (JsonObject iCon in appDataCon)
                {
                    consumerId = (string)iCon["id"]!;
                    consumerName = (string)iCon["username"]!;
                    Console.WriteLine("{0}\t{1}", consumerId, consumerName);

                    var responseCredCons = client.GetAsync(pathAdmin + consumerId + "/acls");
                    responseCredCons.Wait();

                    if (responseCredCons.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var appResCredCons = responseCredCons.Result.Content.ReadAsStringAsync();
                        appResCredCons.Wait();

                        JsonNode documentAppCredCons = JsonNode.Parse(appResCredCons.Result)!;
                        JsonNode rootAppCredCons = documentAppCredCons.Root;
                        JsonArray appDataAppCredCons = (JsonArray)rootAppCredCons["data"];
                        string appNextAppCredCons = (string)rootAppCredCons["next"];

                        if (appDataAppCredCons != null)
                        {
                            foreach (JsonObject iAppCredCons in appDataAppCredCons)
                            {
                                string rolName = (string)iAppCredCons["group"]!; ;

                                Console.WriteLine("{0}\t{1}\t#{2}#", consumerId, consumerName, rolName);
                                sw.WriteLine("{0}\t{1}\t{2}", consumerId, consumerName, rolName);
                            }

                        }
                    }
                }
            }
            sw.Close();

            Console.WriteLine("Fin");
        }

        public static void ExportaAppEnConsumersAArchivo(string[] args)
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;
            string archivoExport = rutaDestino + "/consumers_apps.csv";

            string pathAdmin = "/api-cli/open-api/consumers/";

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("host", hostKong);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);

            StreamWriter sw = new StreamWriter(archivoExport);

            var response = client.GetAsync(pathAdmin);
            response.Wait();
            Console.WriteLine(response.Result.StatusCode);
            string consumerId = "";
            string consumerName = "";
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var appResCon = response.Result.Content.ReadAsStringAsync();
                appResCon.Wait();

                JsonNode documentCon = JsonNode.Parse(appResCon.Result)!;
                JsonNode rootCon = documentCon.Root;
                JsonArray appDataCon = (JsonArray)rootCon["data"];
                string appNextCon = (string)rootCon["next"];

                foreach (JsonObject iCon in appDataCon)
                {
                    consumerId = (string)iCon["id"]!;
                    consumerName = (string)iCon["username"]!;
                    Console.WriteLine("{0}\t{1}", consumerId, consumerName);

                    var responseCredCons = client.GetAsync(pathAdmin + consumerId + "/oauth2");
                    responseCredCons.Wait();

                    if (responseCredCons.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var appResCredCons = responseCredCons.Result.Content.ReadAsStringAsync();
                        appResCredCons.Wait();

                        JsonNode documentAppCredCons = JsonNode.Parse(appResCredCons.Result)!;
                        JsonNode rootAppCredCons = documentAppCredCons.Root;
                        JsonArray appDataAppCredCons = (JsonArray)rootAppCredCons["data"];
                        string appNextAppCredCons = (string)rootAppCredCons["next"];

                        if (appDataAppCredCons != null)
                        {
                            foreach (JsonObject iAppCredCons in appDataAppCredCons)
                            {
                                string appName = (string)iAppCredCons["name"]!;
                                string clientId = (string)iAppCredCons["client_id"]!;
                                string clientSecret = (string)iAppCredCons["client_secret"]!;

                                Console.WriteLine("{0}\t{1}\t{2}", consumerId, consumerName, appName);
                                sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", consumerId, consumerName, appName, clientId, clientSecret);
                            }

                        }
                    }
                }

                Console.WriteLine("next : {0}", appNextCon);
            }
            sw.Close();

            Console.WriteLine("Fin");
        }

        public static void MoverConsumersCustomIdAName(string[] args)
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqcm.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;

            string pathAdmin = "/api-cli/open-api/";

            var httpClientHandler = new HttpClientHandler() { };
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);
            client.DefaultRequestHeaders.Add("Host", hostKong);

            bool continuar = true;
            string offset = "consumers";
            do
            {
                var response = client.GetAsync(pathAdmin + offset);
                response.Wait();
                Console.WriteLine(response.Result.StatusCode);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();
                    usrRes.Wait();

                    JsonNode document = JsonNode.Parse(usrRes.Result)!;
                    JsonNode root = document.Root;
                    JsonArray appData = (JsonArray)root["data"];
                    string appNext = (string)root["next"];

                    foreach (JsonObject i in appData)
                    {
                        string consId = (string)i["id"]!;
                        string username = (string)i["username"]!;
                        string custom_id = (string)i["custom_id"]!;

                        Console.WriteLine("{0},{1}", custom_id, username);

                        if (string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(custom_id))
                        {
                            i["username"] = custom_id;
                            i.Remove("custom_id");
                            i.Remove("username_lower");
                            i.Remove("type");

                            var kongFileObj = JsonSerializer.Serialize(i);

                            var content = new StringContent(kongFileObj, Encoding.UTF8, "application/json");

                            var response2 = client.PutAsync(pathAdmin + offset + "/" + consId, content);
                            response2.Wait();

                            //obtener respuesta
                            if (response2.Result.IsSuccessStatusCode)
                            {
                                Console.WriteLine("actualizada " + custom_id);
                            }
                        }
                    }

                    Console.WriteLine("app.next " + appNext);
                    if (string.IsNullOrEmpty(appNext))
                    {
                        continuar = false;
                    }
                    else
                    {
                        offset = appNext;
                    }
                }
            } while (continuar);

            Console.WriteLine("Fin");
        }

        public static void CreaAppEnConsumersDesdeArchivo(string[] args)
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;
            string archivoAppCons = config.GetSection("archivoAppConsumers").Value;

            string pathAdmin = "/api-cli/open-api/consumers/";
            string archReporte = rutaDestino + "\\credenciales_consumers.csv";
            var sw = new StreamWriter(archReporte);

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("host", hostKong);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);

            StreamReader sr = new StreamReader(archivoAppCons);
            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine();
                if (!string.IsNullOrEmpty(linea))
                {
                    var arrLin = linea.Split("\t");
                    var consumerName = arrLin[0].ToUpper();
                    var appName = arrLin[1].Trim();

                    var response = client.GetAsync(pathAdmin + "?username=" + consumerName);
                    response.Wait();
                    Console.WriteLine("{0} {1} {2}", consumerName, appName, response.Result.StatusCode);
                    string consumerId = "";
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var appResCon = response.Result.Content.ReadAsStringAsync();
                        appResCon.Wait();

                        JsonNode documentCon = JsonNode.Parse(appResCon.Result)!;
                        JsonNode rootCon = documentCon.Root;
                        JsonArray appDataCon = (JsonArray)rootCon["data"];
                        string appNextCon = (string)rootCon["next"];

                        foreach (JsonObject iCon in appDataCon)
                        {
                            consumerId = (string)iCon["id"]!;
                        }

                        if (!string.IsNullOrEmpty(consumerId))
                        {
                            //buscar apps
                            //GET https://api-manager-qam.dtvpan.com/api-cli/open-api/consumers/9e01cd2c-cdca-4641-8c04-fd54a0852b97/oauth2
                            var responseCredCons = client.GetAsync(pathAdmin + "/" + consumerId + "/oauth2");
                            responseCredCons.Wait();
                            //Console.WriteLine("    {0}", responseCredCons.Result.StatusCode);

                            if (responseCredCons.Result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var appResCredCons = responseCredCons.Result.Content.ReadAsStringAsync();
                                appResCredCons.Wait();

                                JsonNode documentAppCredCons = JsonNode.Parse(appResCredCons.Result)!;
                                JsonNode rootAppCredCons = documentAppCredCons.Root;
                                JsonArray appDataAppCredCons = (JsonArray)rootAppCredCons["data"];
                                string appNextAppCredCons = (string)rootAppCredCons["next"];

                                string clientId = "";
                                string clientSecret = "";

                                if (appDataAppCredCons != null)
                                {
                                    bool crearApp = true;
                                    foreach (JsonObject iAppCredCons in appDataAppCredCons)
                                    {
                                        string newAppName = (string)iAppCredCons["name"]!;
                                        if (newAppName.ToUpper() == appName.ToUpper())
                                        {
                                            crearApp = false;
                                            clientId = (string)iAppCredCons["client_id"]!;
                                            clientSecret = (string)iAppCredCons["client_secret"]!;
                                            break;
                                        }
                                    }

                                    if (crearApp)
                                    {
                                        //crear credenciales
                                        Console.WriteLine("creando credenciales para " + appName);

                                        //crearle la credencial POST https://api-manager-qam.dtvpan.com/api-cli/open-api/consumers/9e01cd2c-cdca-4641-8c04-fd54a0852b97/oauth2
                                        var kongConsumer = new KongConsumerCredentialCrear()
                                        {
                                            name = appName,
                                            client_type = "confidential"
                                        };
                                        var kongConsumerObj = JsonSerializer.Serialize(kongConsumer);

                                        var content = new StringContent(kongConsumerObj, Encoding.UTF8, "application/json");

                                        var responsePost = client.PostAsync(pathAdmin + "/" + consumerId + "/oauth2", content);
                                        responsePost.Wait();
                                        if (responsePost.Result.StatusCode == System.Net.HttpStatusCode.Created)
                                        {
                                            var usrPostCons = responsePost.Result.Content.ReadAsStringAsync();
                                            usrPostCons.Wait();

                                            JsonNode documentApp = JsonNode.Parse(usrPostCons.Result)!;
                                            JsonNode rootAppCred = documentApp.Root;
                                            clientId = (string)rootAppCred["client_id"];
                                            clientSecret = (string)rootAppCred["client_secret"];

                                            Console.WriteLine("creada {0},{1}", appName, clientId);
                                            sw.WriteLine("{0},{1},{2},{3},{4},{5}", consumerId, consumerName, appName, clientId, clientSecret, "C");
                                        }
                                        else
                                        {
                                            Console.WriteLine("error " + appName);
                                        }
                                    }
                                    else
                                    {
                                        //ya existe la app
                                        Console.WriteLine("ya existe {0} {1}", appName, clientId);

                                        sw.WriteLine("{0},{1},{2},{3},{4},{5}", consumerId, consumerName, appName, clientId, clientSecret, "E");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("NO existe " + linea);
                    }
                }
            }
            sw.Close();

            Console.WriteLine("Fin");
        }

        public static void EjecutaTestCargaTokens2(string[] args)
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings-kong-ld.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrlToken = config.GetSection("API-url-token-kong").Value;
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;
            string archivoClientes = config.GetSection("archivoConsumersBasic").Value;
            string archivoReporte = rutaDestino + "\\reporte_tokens.csv";

            var httpClientHandler = new HttpClientHandler() { };
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            var lConsumers = new List<ConsumerTokens>();

            StreamReader sr = new StreamReader(archivoClientes);
            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine();
                if (!string.IsNullOrEmpty(linea))
                {
                    var arrLin = linea.Split(",");
                    var consumerName = arrLin[0].ToUpper();
                    var consumerBasic = arrLin[1].Trim();

                    var obj = new ConsumerTokens()
                    {
                        consumer = consumerBasic,
                        consumerName = consumerName,
                        tokens = new List<string>()
                    };
                    lConsumers.Add(obj);
                    Console.WriteLine("{0}", linea);
                }
            }

            StreamWriter sw = new StreamWriter(archivoReporte);

            var token = "";
            int i = 0;
            do
            {
                foreach (var item in lConsumers)
                {
                    var parameters = new Dictionary<string, string>();
                    //pedir token
                    if (item.consumer.Contains(":"))
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("host", hostKong);
                        client.DefaultRequestHeaders.Add("Connection", "close");

                        var arrLinB = item.consumer.Split(":");
                        var clientIdC = arrLinB[0].Trim();
                        var clientSecretC = arrLinB[1].Trim();

                        parameters.Add("grant_type", "client_credentials");
                        parameters.Add("client_id", clientIdC);
                        parameters.Add("client_secret", clientSecretC);
                    }
                    else
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Basic " + item.consumer);
                        client.DefaultRequestHeaders.Add("host", hostKong);
                        client.DefaultRequestHeaders.Add("Connection", "close");

                        parameters.Add("grant_type", "client_credentials");

                    }
                    var content = new FormUrlEncodedContent(parameters);

                    var responsePost = client.PostAsync(apiUrlToken, content);
                    responsePost.Wait();
                    if (responsePost.Result.IsSuccessStatusCode)
                    {
                        var usrPostCons = responsePost.Result.Content.ReadAsStringAsync();
                        usrPostCons.Wait();
                        var cacheStatus = responsePost.Result.Headers.GetValues("X-Cache-Status").First();

                        JsonNode documentApp = JsonNode.Parse(usrPostCons.Result)!;
                        JsonNode rootAppCred = documentApp.Root;
                        token = (string)rootAppCred["access_token"];

                        var status = "";
                        //buscar si el token está repetido
                        var t = item.tokens.FirstOrDefault(m => m == token);
                        if (t is null)
                        {
                            status = "OK";
                            item.tokens.Add(token);
                        }
                        else
                        {
                            status = "OK-Estaba";
                        }
                        string oc = "";
                        foreach (var oCons in lConsumers.Where(m => m.consumer != item.consumer))
                        {
                            var t2 = oCons.tokens.FirstOrDefault(m => m == token);
                            if (t2 is not null)
                            {
                                //ya está en otro consumer
                                status = "ERROR, está en " + oCons.consumer;
                                break;
                            }
                            oc += ".";
                        }

                        Console.WriteLine(" {0},{1, -20},{2},{3},{4},{5},{6}", DateTime.Now, item.consumerName, item.tokens.Count, token, cacheStatus, status, oc);

                        sw.WriteLine("{0},{1},{2},{3},{4},{5}{6}", DateTime.Now, item.consumerName, item.tokens.Count, token, cacheStatus, status, oc);
                    }
                    else
                    {
                        Console.WriteLine("error " + item.consumer);
                    }
                }
                i++;
            }
            while (i < 1000000);

            sw.Close();

            Console.WriteLine("Fin");
        }


        public static void RenombraConsumersRoles(string[] args)
        {
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;

            string pathAdmin = "/api-cli/open-api/consumers/";

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("host", hostKong);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);

            var response = client.GetAsync(pathAdmin + "?username=C_RG_PRD_ESB_KONG@DTVPAN.COM");
            response.Wait();
            Console.WriteLine(response.Result.StatusCode);
            string consumerId = "";
            string consumerName = "";
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var appResCon = response.Result.Content.ReadAsStringAsync();
                appResCon.Wait();

                JsonNode documentCon = JsonNode.Parse(appResCon.Result)!;
                JsonNode rootCon = documentCon.Root;
                JsonArray appDataCon = (JsonArray)rootCon["data"];
                string appNextCon = (string)rootCon["next"];

                foreach (JsonObject iCon in appDataCon)
                {
                    consumerId = (string)iCon["id"]!;
                    consumerName = (string)iCon["username"]!;
                    Console.WriteLine("{0}\t{1}", consumerId, consumerName);

                    var responseCredCons = client.GetAsync(pathAdmin + consumerId + "/acls");
                    responseCredCons.Wait();

                    if (responseCredCons.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var appResCredCons = responseCredCons.Result.Content.ReadAsStringAsync();
                        appResCredCons.Wait();

                        JsonNode documentAppCredCons = JsonNode.Parse(appResCredCons.Result)!;
                        JsonNode rootAppCredCons = documentAppCredCons.Root;
                        JsonArray appDataAppCredCons = (JsonArray)rootAppCredCons["data"];
                        string appNextAppCredCons = (string)rootAppCredCons["next"];

                        if (appDataAppCredCons != null)
                        {
                            foreach (JsonObject iRole in appDataAppCredCons)
                            {
                                string rolId = (string)iRole["id"]!;
                                string rolName = (string)iRole["group"]!;

                                Console.WriteLine("{0},{1}", rolId, rolName);
                                if (!rolName.EndsWith("Role"))
                                {
                                    //deshabilitar plugin
                                    iRole["group"] = rolName + "Role";
                                    var kongRolObj = JsonSerializer.Serialize(iRole);

                                    var content = new StringContent(kongRolObj, Encoding.UTF8, "application/json");

                                    var response2 = client.PutAsync(pathAdmin + consumerId + "/acls/" + rolId, content);
                                    response2.Wait();

                                    //obtener respuesta
                                    if (response2.Result.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine("rol actualizado " + rolName);
                                    }
                                }
                            }

                        }
                    }
                }
            }

            Console.WriteLine("Fin");
        }

          public static void PluginsACL_Allow_Faltante(string[] args)
        {
            //IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - prod.json", optional: true, reloadOnChange: true).Build();
            IConfiguration configRoot = new ConfigurationBuilder().AddJsonFile("appsettings - cloud - dtvqam.json", optional: true, reloadOnChange: true).Build();

            IConfigurationSection config = configRoot.GetSection("MySettings");
            string apiUrl = config.GetSection("urlKongAdmin").Value;
            string authKong = config.GetSection("authKongAdmin").Value;
            string rutaDestino = config.GetSection("rutaDestino").Value;
            string hostKong = config.GetSection("hostKong").Value;

            string pathAdmin = "/api-cli/open-api/";

            var httpClientHandler = new HttpClientHandler() { };
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Kong-Admin-Token", authKong);
            client.DefaultRequestHeaders.Add("Host", hostKong);

            var archivo = rutaDestino + "\\listado-pic.txt";
            Console.WriteLine(archivo);
            StreamWriter sw = new StreamWriter(archivo, false);

            bool continuar = true;
            string offset = "/plugins";
            do
            {
                var response = client.GetAsync(pathAdmin + offset ); //+ "?enabled=true"
                response.Wait();
                Console.WriteLine(response.Result.StatusCode);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var usrRes = response.Result.Content.ReadAsStringAsync();
                    usrRes.Wait();

                    JsonNode document = JsonNode.Parse(usrRes.Result)!;
                    JsonNode root = document.Root;
                    JsonArray appData = (JsonArray)root["data"];
                    string appNext = (string)root["next"];

                    foreach (JsonObject i in appData)
                    {
                        string apiId = (string)i["id"]!;
                        string apiName = (string)i["name"]!;
                        string apiInstance = (string)i["instance_name"]!;

                        if (apiName == "acl" )
                        {
                            //Console.WriteLine("{0},{1}", apiName, apiInstance);
                            JsonNode configNode = (JsonNode)i["config"]!;
                            JsonArray apiConfigAllow = (JsonArray)configNode["allow"];
                             if (apiConfigAllow ==null)
                            {
                                string allowName = apiInstance.Substring(0, apiInstance.IndexOf("-"))+ "Role";
                                //agregar allow fatlante
                                ((JsonNode)i["config"])["allow"] = new JsonArray(allowName);
                                var kongFileObj = JsonSerializer.Serialize(i);

                                var content = new StringContent(kongFileObj, Encoding.UTF8, "application/json");
                            
                                var response2 = client.PutAsync(pathAdmin + "plugins/" + apiId, content);
                                response2.Wait();

                                //obtener respuesta
                                if (response2.Result.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("actualizada " + apiInstance);
                                }
    
                                Console.WriteLine("CREAR ALLOW {0},{1},{2}", apiId, apiInstance, allowName); //, i.config.allow[0]
                            }
                            else
                            {
                                foreach (string ic in apiConfigAllow)
                                {
                                    Console.WriteLine("ALLOW creado {0},{1},{2}", apiId, apiInstance, ic);
                                }
                            }
                        }
                    }

                    Console.WriteLine("app.next " + appNext);
                    if (string.IsNullOrEmpty(appNext))
                    {
                        continuar = false;
                    }
                    else
                    {
                        offset = appNext;
                    }
                }
            } while (continuar);

            sw.Close();

            Console.WriteLine("Fin");
        }
    }
}