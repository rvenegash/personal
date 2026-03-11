
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using exportaAPIS_CloudWso2.Models;
using Microsoft.Extensions.Configuration;

namespace exportaAPIS_CloudWso2
{
    public static class ProgramQradar
    {
        public static async void ConsultaConsumoApi(string[] args)
        {
            var routes = getRouteNames();
            string archivoExport = @"C:\Users\rvenegas\OneDrive - VRIO DIRECTV\Requerimientos Directv\2023 09 14 apis prod\consumo-apis-qradar-9.txt";
            string apiUrl = "https://siemqr.dtvpan.com";
            string token = "cnZlbmVnYXM6MmN2ZTA1OC0qMj1EeUE=";

            StreamWriter sw = new StreamWriter(archivoExport);

            var httpClientHandler = new HttpClientHandler() { };
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(apiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");
            //client.DefaultRequestHeaders.Add("Range", "items=0-100");

            foreach (var route in routes)
            {
                Console.WriteLine($"Route : {route}");

                var content = new StringContent("{}", Encoding.UTF8, "application/json");

                var response = client.PostAsync("/api/ariel/searches?query_expression=" + getQradarQuery(route), null);
                response.Wait();
                if (response.Result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var resultPost = await response.Result.Content.ReadAsAsync<QRadarPostResult>();
                    string searchId = resultPost.search_id ?? "";
                    bool buscar = true;
                    bool hayConsumo = false;
                    int cant = 0;
                    Console.WriteLine($"        {searchId}");
                    Thread.Sleep(1000);

                    while (buscar)
                    {
                        try
                        {
                            cant++;
                            var responseGet = client.GetAsync($"/api/ariel/searches/{searchId}");
                            responseGet.Wait();
                            if (responseGet.Result.IsSuccessStatusCode)
                            {
                                var resultGet = responseGet.Result.Content.ReadAsStringAsync();
                                var objResGet = JsonSerializer.Deserialize<QRadarPostResult>(resultGet.Result);

                                Console.Write($"...{objResGet.status}({cant})");
                                if (objResGet.status == "COMPLETED" || objResGet.status == "EXECUTE")
                                {
                                    var responseResult = client.GetAsync($"/api/ariel/searches/{searchId}/results");
                                    responseResult.Wait();
                                    if (responseResult.Result.IsSuccessStatusCode)
                                    {
                                        Console.Write($"...R({responseResult.Result.IsSuccessStatusCode})");
                                        var resultRes = responseResult.Result.Content.ReadAsAsync<QRadarResults>();
                                        resultRes.Wait();
                                        int result = 0;
                                        foreach (var eventitem in resultRes.Result.events)
                                        {
                                            result++;
                                            sw.WriteLine($"{route}\t{eventitem.Username}\t{eventitem.Sender_Country}\t{eventitem.Metadata_System_ID}\t{eventitem.Count}");
                                            Console.WriteLine($"{route}\t{eventitem.Username}\t{eventitem.Sender_Country}\t{eventitem.Metadata_System_ID}\t{eventitem.Count}");
                                            hayConsumo = true;
                                            buscar = false;
                                            if (result>10)
                                            {
                                                break;
                                            }
                                        }

                                        if (objResGet.status == "EXECUTE" && hayConsumo)
                                        {
                                            Console.WriteLine($"        CANCELANDO...");
                                            var responseCancel = client.DeleteAsync($"/api/ariel/searches/{searchId}");
                                            responseCancel.Wait();
                                        }
                                        if (objResGet.status == "COMPLETED")
                                        {
                                            buscar = false;
                                        }
                                    }
                                }
                            }
                            Thread.Sleep(5000);
                        }
                        catch (Exception ex)
                        {
                            buscar = false;
                            sw.WriteLine($"{route}\tERROR\t\t\t");
                            Console.WriteLine($"        ERROR {ex.Message}");
                        }
                    }
                    if (!hayConsumo)
                    {
                        sw.WriteLine($"{route}\tNO HAY CONSUMO\t\t\t");
                        Console.WriteLine($"        NO HAY CONSUMO");
                    }
                }
            }

            sw.Close();
            Console.WriteLine("Fin");
        }

        private static string getQradarQuery(string route)
        {
            string url = $"SELECT \"Route Name\" AS 'Route_Name', \"userName\" AS 'Username', \"Sender Country\" AS 'Sender_Country', \"Metadata System ID\" AS 'Metadata_System_ID', \"Response Code\" AS 'Response_Code', \"date\" AS 'date' from events where logSourceId='39665' and \"Route Name\"='{route}' limit 5 last 10 days";

            return System.Web.HttpUtility.UrlEncode(url);
        }

        private static List<string> getRouteNames()
        {
            var routes = new List<string>();
            //routes.Add("ServiceQualification-4.0");
            //routes.Add("ServiceProblemManagement-5.0-Solv721-Optimus-CL");
            //routes.Add("PrepayBalanceManagement-4.0");
            //routes.Add("PaymentMethod-4.0");
            //routes.Add("ResourceInventoryManagement-6.0-PR-By-Crit-Optimus-UY");
            //routes.Add("AccountManagement-2.0");
            //routes.Add("CustomerManagement-15.0-Cust-By-Crit-Optimus-R2");
            //routes.Add("ResourceInventoryManagement-6.0-Search-LPR-Optimus-R2");
            //routes.Add("ResourceInventoryManagement-6.0-PR-By-Cust-Optimus-AR");
            //routes.Add("Provisioning-2.0-Avail-Ports-R2");
            //routes.Add("WSSDSNETMobile-1.0");
            //routes.Add("PrepayBalanceManagement-1.0");
            //routes.Add("ProductOrdering-4.0");
            //routes.Add("WorkforceManagement-3.0");
            //routes.Add("ServiceQualification-3.0");
            //routes.Add("ResourceInventoryManagement-6.0-PR-By-Crit-Optimus-PE");
            //routes.Add("IdentityManagement-5.0");
            //routes.Add("ProductOrdering-10.0");
            //routes.Add("BillingManagement-4.0");
            //routes.Add("ResourceInventoryManagement-5.0-Search-LPR-Optimus-PE");
            //routes.Add("CustomerManagement-17.0-Cust-By-Crit-Optimus-CL");
            //routes.Add("CustomerManagement-12.0");
            //routes.Add("PaymentManagement-6.0");
            //routes.Add("ServiceQualityManagement-2.0-Check-SL-Optimus-R2");
            //routes.Add("ActivationConfiguration-6.0");
            //routes.Add("Customer360View-4.0");
            //routes.Add("ServiceProblemManagement-5.0-Solv711-Optimus-PE");
            //routes.Add("Provisioning-2.0-Cancel-Interv-R2");
            //routes.Add("AgreementManagement-2.0");
            //routes.Add("WSSDSNETMobile-4.0");
            //routes.Add("ProductOrdering-18.0");
            //routes.Add("ServiceProblemManagement-5.0-Solv721-Optimus-R2");
            //routes.Add("CustomerBillManagement-4.0");
            //routes.Add("CustomerManagement-4.0");
            //routes.Add("ServiceQualityManagement-2.0");
            //routes.Add("ResourceInventoryManagement-6.0-Search-LPR-Optimus-PE");
            //routes.Add("ResourceInventoryManagement-6.0-Upd-PR-Optimus-CO");
            //routes.Add("CustomerManagement-10.0");
            //routes.Add("PredictiveTransaction-2.0");
            //routes.Add("ClaimManagement-2.0");
            //routes.Add("WorkforceManagement-3.0-Get-WO-By-Cust-Optimus-CL");
            //routes.Add("PaymentManagement-14.0");
            //routes.Add("ServiceProblemManagement-3.0-Solv711-Optimus-PE");
            //routes.Add("ResourceInventoryManagement-6.0-PR-By-Cust-Optimus-UY");
            //routes.Add("Provisioning-2.0-Query-Neighbor-CL");
            //routes.Add("ActivationConfiguration-6.0-Service-CL");
            //routes.Add("ProductInventoryManagementGetProducts-2.0");
            //routes.Add("Customers-1.0");
            //routes.Add("ResourceInventoryManagement-6.0-PR-By-Cust-Optimus-EC");
            //routes.Add("CustomerManagement-7.0");
            //routes.Add("ResourceInventoryManagement-6.0-Upd-PR-Optimus-UY");
            //routes.Add("ServiceProblemManagement-3.0-Solv721-Optimus-R2");
            //routes.Add("Promotion-3.0");
            //routes.Add("ProductOrdering-17.0");
            //routes.Add("ServiceProblemManagement-5.0-Solv711-Optimus-R2");
            //routes.Add("ProductInventoryManagementGetProducts-3.0");
            //routes.Add("ResourceInventoryManagement-6.0-PR-By-Crit-Optimus-CL");
            //routes.Add("ServiceQualityManagement-2.0-Avail-4-CSL-Optimus-CL");
            //routes.Add("ResourceInventoryManagement-6.0-Search-Optimus-R2");
            //routes.Add("ServiceProblemManagement-4.0-Solv721-Optimus-PE");
            //routes.Add("ServiceProblemManagement-2.0");
            //routes.Add("ResourceOrdering-1.0");
            //routes.Add("PrepayBalanceManagement-5.0");
            //routes.Add("ServiceQualification-7.0");
            //routes.Add("ServiceQualityManagement-2.0-Check-SL-Optimus-CL");
            //routes.Add("ContactMedium-2.0");
            //routes.Add("ServiceProblemManagement-3.0-Solv764-Optimus-CO");
            //routes.Add("ResourceOrdering-5.0-Update-Ports-Res-Optimus-CO");
            //routes.Add("ProductInventoryManagement-5.0");
            //routes.Add("SurveyManagement-1.0");
            //routes.Add("Provisioning-1.0-Cancel-Interv-R2");
            //routes.Add("ActivationConfiguration-6.0-Service-R2");
            //routes.Add("TestJenkins-2.0");
            //routes.Add("ResourceOrdering-5.0");
            //routes.Add("PaymentMethod-7.0");
            //routes.Add("ServiceProblemManagement-3.0-Solv721-Optimus-PE");
            //routes.Add("Provisioning-1.0-Finalize-Interv-R2");
            //routes.Add("CustomerOTT-1.0");
            //routes.Add("CustomerBillManagement-2.0");
            //routes.Add("ResourceInventoryManagement-5.0-Search-LPR-Optimus-UY");
            //routes.Add("AuthorizationManagement-1.0-Merlin-R2");
            //routes.Add("ResourceOrdering-2.0");
            //routes.Add("ProductOrdering-6.0");
            //routes.Add("PaymentManagement-13.0");
            //routes.Add("NotificationManagement-1.0");
            //routes.Add("ServiceQualification-5.0");
            //routes.Add("Provisioning-1.0-Finalize-Interv-CL");
            //routes.Add("CustomerManagement-17.0-Cust-By-Crit-Optimus-R2");
            //routes.Add("ResourceInventoryManagement-6.0-Upd-PR-Optimus-R2");
            //routes.Add("WorkforceManagement-3.0-Get-WO-By-Cust-Optimus-PE");
            //routes.Add("ResourceIdentification-1.0");
            //routes.Add("WorkforceManagement-3.0-Create-WO-Optimus-AR");
            //routes.Add("QuoteManagement-1.0");
            //routes.Add("ClaimManagement-1.0");
            //routes.Add("ProductOrdering-13.0");
            //routes.Add("CustomerManagement-13.0-Cust-By-Crit-Optimus-R2");
            //routes.Add("DocumentManagement-7.0-Signed-Optimus-CL");
            //routes.Add("IdentityManagement-4.0");
            //routes.Add("Provisioning-2.0-Finalize-Interv-R2");
            //routes.Add("Appointment-2.0");
            //routes.Add("CustomerBillManagement-5.0");
            //routes.Add("BillingManagement-2.0");
            //routes.Add("ServiceProblemManagement-3.0-Solv721-Optimus-CL");
            //routes.Add("CustomerStatusAndProductCategory-1.0");
            //routes.Add("ResourceOrdering-3.0");
            //routes.Add("IdentityManagement-1.0");
            //routes.Add("CustomerBillManagement-3.0");
            //routes.Add("PaymentManagement-8.0");
            //routes.Add("CustomerManagement-17.0-Cust-By-Crit-Optimus-EC");
            //routes.Add("ProductInventoryManagement-4.0");
            //routes.Add("WorkforceManagement-2.0");
            //routes.Add("ServiceProblemManagement-5.0");
            //routes.Add("PartyInteractionManagement-1.0");
            //routes.Add("DocumentManagement-7.0-Forward-Optimus-CL");
            //routes.Add("ElegibleProductOffering-6.0");
            //routes.Add("WorkforceManagement-3.0-Get-WO-By-Cust-Optimus-CO");
            //routes.Add("ProductOrdering-14.0");
            //routes.Add("DocumentManagement-4.0");
            //routes.Add("PaymentMethod-3.0");
            //routes.Add("MMSS-ConfigSecrets-1.0");
            //routes.Add("CustomerInteraction-3.0");
            //routes.Add("ServiceProblemManagement-3.0-Solv721-Optimus-CO");
            //routes.Add("ResourceInventoryManagement-6.0-Search-Optimus-UY");
            //routes.Add("ProductInventoryManagement-3.0");
            //routes.Add("Appointment-1.0");
            //routes.Add("GoBoxDevice-1.0");
            //routes.Add("ValidateBackOffice-1.0");
            //routes.Add("ProductInventoryManagement-7.0");
            //routes.Add("ServiceProblemManagement-4.0-Solv711-Optimus-CL");
            //routes.Add("TestMicrogatewayAnalitics-1.0");
            //routes.Add("ProductOfferingQualification-2.0");
            //routes.Add("Customer360View-5.0");
            //routes.Add("AccountManagement-1.0");
            //routes.Add("Location-4.0");
            //routes.Add("PaymentManagement-5.0");
            //routes.Add("ServiceProblemManagement-5.0-Solv711-Optimus-CL");
            //routes.Add("IdentityManagement-3.0");
            //routes.Add("WorkforceManagement-3.0-Compl-WO-Optimus-PE");
            //routes.Add("MMSS-FBM-ConfigSecrets-1.0");
            //routes.Add("ResourceInventoryManagement-6.0-PR-By-Cust-Optimus-CO");
            //routes.Add("WorkforceManagement-3.0-Compl-WO-Optimus-CL");
            //routes.Add("PaymentManagement-12.0");
            //routes.Add("WorkforceManagement-3.0-Get-WO-By-Cust-Optimus-AR");
            //routes.Add("CustomerInteraction-2.0");
            //routes.Add("PaymentManagement-10.0");
            //routes.Add("EnvironmentStatus-1.0");
            //routes.Add("ResourceInventoryManagement-6.0-PR-By-Crit-Optimus-CO");
            //routes.Add("ResourceOrdering-6.0-Update-Ports-Res-Optimus-R2");
            //routes.Add("ServiceQualityManagement-2.0-Check-SL-Optimus-PE");
            //routes.Add("AgreementManagement-3.0-TermsAndCond-Optimus-CL");
            //routes.Add("ActivationConfiguration-6.0-Service-EC");
            //routes.Add("DocumentManagement-2.0");
            //routes.Add("Location-5.0");
            //routes.Add("CustomerInteraction-1.0");
            //routes.Add("PaymentMethod-2.0");
            //routes.Add("ActivationConfiguration-2.0");
            //routes.Add("PaymentMethod-6.0");
            //routes.Add("CustomerManagement-10.0-Cust-By-Crit-Optimus-PE");
            //routes.Add("LoyaltyManagement-1.0");
            //routes.Add("ServiceProblemManagement-3.0-Solv721-Optimus-UY");
            //routes.Add("ResourceInventoryManagement-6.0-Upd-PR-Optimus-CL");
            //routes.Add("MMSS-FBM-Route-Configuration-Mng");
            //routes.Add("ProductInventoryManagement-6.0");
            //routes.Add("WSSDSNETMobile-2.0");
            //routes.Add("OnBoardingManagement-1.0");
            //routes.Add("Promotion-1.0");
            //routes.Add("ResourceOrdering-5.0-Create-Ports-Res-Optimus-R2");
            //routes.Add("AuthorizationManagement-1.0-Merlin-R1");
            //routes.Add("ResourceInventoryManagement-5.0-Search-LPR-Optimus-EC");
            //routes.Add("DocumentManagement-6.0");
            //routes.Add("Provisioning-1.0-Upd-Ports-On-Serv-R2");
            //routes.Add("PaymentManagement-9.0");
            //routes.Add("ResourceInventoryManagement-4.0");
            //routes.Add("FraudManagement-1.0");
            //routes.Add("ServiceProblemManagement-3.0-Solv721-Optimus-EC");
            //routes.Add("eSALES-Customer-1.0");
            //routes.Add("ResourceInventoryManagement-5.0");
            //routes.Add("Address-3.0");
            //routes.Add("ServiceQualityManagement-2.0-Check-SL-Optimus-CO");
            //routes.Add("AgreementManagement-1.0");
            //routes.Add("PartyInteractions-1.0");
            //routes.Add("Location-5.0-Geo-Reverse-R2");
            //routes.Add("WorkforceManagement-1.0");
            //routes.Add("ProductOrdering-7.0");
            //routes.Add("ActivationConfiguration-1.0");
            //routes.Add("ProductOrdering-8.0");
            //routes.Add("PaymentManagement-11.0");
            //routes.Add("ActivationConfiguration-3.0");
            //routes.Add("WorkforceManagement-3.0-Compl-WO-Optimus-CO");
            //routes.Add("CustomerManagement-10.0-Cust-By-Crit-Optimus-R2");
            //routes.Add("IdentityManagement-2.0");
            //routes.Add("ProductOrdering-5.0");
            //routes.Add("ResourceInventoryManagement-6.0-PR-By-Cust-Optimus-PE");
            //routes.Add("ResourceInventoryManagement-6.0-Search-Optimus-PE");
            //routes.Add("CustomerManagement-12.0-Cust-By-Crit-Optimus-CL");
            //routes.Add("Provisioning-2.0");
            //routes.Add("ServiceProblemManagement-4.0-Solv721-Optimus-CL");
            //routes.Add("WorkforceManagement-3.0-Create-WO-Optimus-CL");
            //routes.Add("ProductOrdering-16.0");
            //routes.Add("Provisioning-2.0-Avail-Ports-CL");
            //routes.Add("CustomerManagement-13.0-Cust-By-Crit-Optimus-CL");
            //routes.Add("ResourceInventoryManagement-6.0-Upd-PR-Optimus-EC");
            //routes.Add("WorkforceManagement-3.0-Create-WO-Optimus-UY");
            //routes.Add("ElegibleProductOffering-5.0");
            //routes.Add("AuthorizationManagement-1.0-Merlin-R3");
            //routes.Add("Provisioning-2.0-Cancel-Interv-CL");
            //routes.Add("Appointment-3.0");
            //routes.Add("CustomerManagement-5.0");
            //routes.Add("PrepayBalanceManagement-2.0");
            //routes.Add("ProductOrdering-9.0");
            //routes.Add("CustomerManagement-10.0-Cust-By-Crit-Optimus-CL");
            //routes.Add("PrepayBalanceManagement-3.0");
            //routes.Add("Location-5.0-Geo-Reverse-R1");
            //routes.Add("Provisioning-1.0-Cancel-Interv-CL");
            //routes.Add("ResourceInventoryManagement-5.0-Search-LPR-Optimus-CL");
            //routes.Add("WorkforceManagement-3.0-Compl-WO-Optimus-UY");
            //routes.Add("ServiceProblemManagement-3.0-Solv711-Optimus-EC");
            //routes.Add("ResourceInventoryManagement-6.0-PR-By-Cust-Optimus-CL");
            //routes.Add("ServiceProblemManagement-4.0-Solv711-Optimus-PE");
            //routes.Add("PaymentMethod-5.0");
            //routes.Add("ResourceInventoryManagement-5.0-Search-Optimus-UY");
            //routes.Add("ResourceInventoryManagement-3.0");
            //routes.Add("ResourceInventoryManagement-5.0-Search-Optimus-R2");
            //routes.Add("WSSDSNETMobile-3.0");
            //routes.Add("ResourceInventoryManagement-1.0");
            //routes.Add("Communication-1.0");
            //routes.Add("ResourceInventoryManagement-6.0-PR-By-Crit-Optimus-EC");
            //routes.Add("AccountManagement-4.0");
            //routes.Add("WorkforceManagement-3.0-Compl-WO-Optimus-EC");
            //routes.Add("CustomerManagement-13.0");
            //routes.Add("BillingManagement-5.0");
            //routes.Add("WorkforceManagement-3.0-Create-WO-Optimus-EC");
            //routes.Add("PaymentEngine-1.0_deprecated");
            //routes.Add("ElegibleProductOffering-7.0");
            //routes.Add("BillingManagement-6.0");
            //routes.Add("AgreementManagement-3.0");
            //routes.Add("COBRO-ONLINE-1.0");
            //routes.Add("UsageConsumptionManagement-1.0");
            //routes.Add("SalesManagement-1.0");
            //routes.Add("PrepayBalanceManagement-6.0");
            //routes.Add("ServiceProblemManagement-3.0-Solv711-Optimus-CO");
            //routes.Add("Location-6.0");
            //routes.Add("ResourceOrdering-5.0-Update-Ports-Res-Optimus-PE");
            //routes.Add("CustomerManagement-17.0");
            //routes.Add("ResourceProvisioning-1.0");
            //routes.Add("ResourceInventoryManagement-6.0-PR-By-Crit-Optimus-R2");
            //routes.Add("BillingManagement-3.0");
            //routes.Add("ServiceProblemManagement-3.0");
            //routes.Add("WorkforceManagement-3.0-Compl-WO-Optimus-AR");
            //routes.Add("ResourceInventoryManagement-5.0-Search-LPR-Optimus-CO");
            //routes.Add("ActivationConfiguration-4.0");
            //routes.Add("Promotion-2.0");
            //routes.Add("PaymentManagement-4.0");
            //routes.Add("ResourceInventoryManagement-6.0-Search-Optimus-CL");
            //routes.Add("ResourceInventoryManagement-5.0-Search-LPR-Optimus-R2");
            //routes.Add("PaymentMethod-1.0");
            //routes.Add("SalesLead-2.0");
            //routes.Add("CustomerManagement-15.0-Cust-By-Crit-Optimus-CL");
            //routes.Add("ActivationConfiguration-6.0-Service-UY");
            //routes.Add("MMSS-FBM-Route-Business-Mng");
            //routes.Add("SAS-1.0");
            //routes.Add("ActivationConfiguration-6.0-Service-PE");
            //routes.Add("DisneyPlus-1.0");
            //routes.Add("ServiceQualityManagement-1.0");
            //routes.Add("ResourceInventoryManagement-5.0-Search-Optimus-CL");
            //routes.Add("CustomerManagement-11.0-Cust-By-Crit-Optimus-R2");
            //routes.Add("CustomerManagement-9.0");
            //routes.Add("AccountManagement-5.0");
            //routes.Add("OCC_SAS-1.0");
            //routes.Add("ResourceInventoryManagement-5.0-Search-Optimus-PE");
            //routes.Add("CustomerManagement-15.0");
            //routes.Add("ResourceInventoryManagement-5.0-Search-Optimus-CO");
            //routes.Add("WorkforceManagement-3.0-Get-WO-By-Cust-Optimus-UY");
            //routes.Add("ServiceQualification-6.0");
            //routes.Add("CustomerManagement-17.0-Cust-By-Crit-Optimus-PE");
            //routes.Add("ResourceInventoryManagement-5.0-Search-Optimus-EC");
            //routes.Add("ServiceProblemManagement-4.0");
            //routes.Add("ProductInventoryManagement-2.0");
            //routes.Add("Provisioning-1.0-Upd-Ports-On-Serv-CO");
            //routes.Add("ActivationConfiguration-5.0");
            //routes.Add("token-dtv-fixed-1.0");
            //routes.Add("WorkforceManagement-3.0-Get-WO-By-Cust-Optimus-EC");
            //routes.Add("CustomerManagement-8.0");
            //routes.Add("ProductOrdering-11.0");
            //routes.Add("WorkforceManagement-3.0-Create-WO-Optimus-PE");
            //routes.Add("PaymentManagement-7.0");
            //routes.Add("ServiceProblemManagement-3.0-Solv711-Optimus-CL");
            //routes.Add("ResourceOrdering-4.0");
            //routes.Add("CustomerManagement-6.0");
            //routes.Add("ServiceQualityManagement-2.0-Avail-4-CSL-Optimus-R2");
            //routes.Add("ServiceProblemManagement-4.0-Solv721-Optimus-R2");
            //routes.Add("ServiceProblemManagement-5.0-Solv721-Optimus-PE");
            //routes.Add("EventManagement-1.0");
            //routes.Add("ServiceProblemManagement-3.0-Solv711-Optimus-R2");
            //routes.Add("DocumentManagement-3.0");
            //routes.Add("ResourceOrdering-5.0-Create-Ports-Res-Optimus-CO");
            //routes.Add("ResourceInventoryManagement-6.0-Search-LPR-Optimus-CL");
            //routes.Add("WorkforceManagement-3.0-Create-WO-Optimus-CO");
            //routes.Add("OnBoardingManagement-2.0");
            //routes.Add("CustomerManagement-17.0-Cust-By-Crit-Optimus-UY");
            //routes.Add("ServiceProblemManagement-4.0-Solv711-Optimus-R2");
            //routes.Add("ContactMedium-V1.0");
            //routes.Add("ResourceInventoryManagement-6.0-Upd-PR-Optimus-PE");
            //routes.Add("Provisioning-2.0-Finalize-Interv-CL");
            //routes.Add("ServiceProblemManagement-3.0-Solv711-Optimus-UY");
            //routes.Add("Azimut_SelfService-1.0");
            //routes.Add("AccountManagement-3.0");
            //routes.Add("ProductCatalog-1.0");
            //routes.Add("ActivationConfiguration-6.0-Service-CO");
            //routes.Add("ProductOrdering-12.0");
            //routes.Add("eSALES-Customer-2.0");
            //routes.Add("SalesLead-1.0");
            //routes.Add("ResourceInventoryManagement-6.0");
            //routes.Add("Provisioning-1.0");
            //routes.Add("DocumentManagement-5.0");
            //routes.Add("CustomerManagement-17.0-Cust-By-Crit-Optimus-CO");
            //routes.Add("ResourceOrdering-5.0-Update-Ports-Res-Optimus-CL");
            //routes.Add("ServiceProblemManagement-1.0");
            //routes.Add("BillingManagement-1.0");
            //routes.Add("QuoteManagement-2.0");
            //routes.Add("ShipmentTracking-1.0");
            //routes.Add("Communication-2.0");
            //routes.Add("CustomerManagement-12.0-Cust-By-Crit-Optimus-R2");
            //routes.Add("DocumentManagement-7.0");
            //routes.Add("PartyManagement-1.0");
            //routes.Add("ResourceOrdering-5.0-Update-Ports-Res-Optimus-R2");

            //routes.Add("AccountManagement-2.0");
            //routes.Add("ActivationConfiguration-2.0");
            //routes.Add("ActivationConfiguration-4.0");
            //routes.Add("Address-3.0");
            //routes.Add("AgreementManagement-1.0");
            //routes.Add("AgreementManagement-3.0");
            //routes.Add("Azimut_SelfService-1.0");
            //routes.Add("BillingManagement-1.0");
            //routes.Add("BillingManagement-3.0");
            //routes.Add("ContactMedium-2.0");
            //routes.Add("CustomerBillManagement-3.0");
            //routes.Add("CustomerManagement-10.0-Cust-By-Crit-Optimus-CL");
            //routes.Add("CustomerManagement-10.0-Cust-By-Crit-Optimus-PE");
            //routes.Add("CustomerManagement-10.0-Cust-By-Crit-Optimus-R2");
            //routes.Add("CustomerManagement-11.0-Cust-By-Crit-Optimus-R2");
            //routes.Add("CustomerManagement-12.0-Cust-By-Crit-Optimus-CL");
            //routes.Add("CustomerManagement-12.0-Cust-By-Crit-Optimus-R2");
            //routes.Add("CustomerManagement-13.0-Cust-By-Crit-Optimus-CL");
            //routes.Add("CustomerManagement-13.0-Cust-By-Crit-Optimus-R2");
            //routes.Add("CustomerManagement-15.0-Cust-By-Crit-Optimus-CL");
            routes.Add("CustomerManagement-15.0-Cust-By-Crit-Optimus-R2");
            routes.Add("CustomerManagement-17.0-Cust-By-Crit-Optimus-CL");
            routes.Add("CustomerManagement-17.0-Cust-By-Crit-Optimus-CO");
            routes.Add("CustomerManagement-17.0-Cust-By-Crit-Optimus-EC");
            routes.Add("CustomerManagement-17.0-Cust-By-Crit-Optimus-PE");
            routes.Add("CustomerManagement-17.0-Cust-By-Crit-Optimus-R2");
            routes.Add("CustomerManagement-17.0-Cust-By-Crit-Optimus-UY");
            routes.Add("CustomerManagement-5.0");
            routes.Add("CustomerManagement-6.0");
            routes.Add("CustomerManagement-7.0");
            routes.Add("Customers-1.0");
            routes.Add("CustomerStatusAndProductCategory-1.0");
            routes.Add("DocumentManagement-2.0");
            routes.Add("DocumentManagement-3.0");
            routes.Add("DocumentManagement-7.0");
            routes.Add("ElegibleProductOffering-5.0");
            routes.Add("ElegibleProductOffering-6.0");
            routes.Add("eSALES-Customer-1.0");
            routes.Add("eSALES-Customer-2.0");
            routes.Add("IdentityManagement-2.0");
            routes.Add("IdentityManagement-3.0");
            routes.Add("Location-4.0");
            routes.Add("Location-6.0");
            routes.Add("LoyaltyManagement-1.0");
            routes.Add("MMSS-ConfigSecrets-1.0");
            routes.Add("MMSS-FBM-ConfigSecrets-1.0");
            routes.Add("NotificationManagement-1.0");
            routes.Add("OnBoardingManagement-1.0");
            routes.Add("OnBoardingManagement-2.0");
            routes.Add("PartyManagement-1.0");
            routes.Add("PaymentEngine-1.0_deprecated");
            routes.Add("PaymentManagement-5.0");
            routes.Add("PaymentManagement-7.0");
            routes.Add("PaymentMethod-2.0");
            routes.Add("ProductInventoryManagement-2.0");
            routes.Add("ProductInventoryManagement-3.0");
            routes.Add("ProductInventoryManagementGetProducts-3.0");
            routes.Add("ProductOfferingQualification-2.0");
            routes.Add("ProductOrdering-13.0");
            routes.Add("ProductOrdering-16.0");
            routes.Add("ProductOrdering-5.0");
            routes.Add("ProductOrdering-6.0");
            routes.Add("ProductOrdering-7.0");
            routes.Add("ProductOrdering-8.0");
            routes.Add("ProductOrdering-9.0");
            routes.Add("Promotion-1.0");
            routes.Add("Promotion-3.0");
            routes.Add("Provisioning-1.0-Cancel-Interv-R2");
            routes.Add("Provisioning-1.0-Finalize-Interv-R2");
            routes.Add("Provisioning-1.0-Upd-Ports-On-Serv-CO");
            routes.Add("Provisioning-2.0-Avail-Ports-R2");
            routes.Add("Provisioning-2.0-Cancel-Interv-CL");
            routes.Add("Provisioning-2.0-Cancel-Interv-R2");
            routes.Add("Provisioning-2.0-Finalize-Interv-CL");
            routes.Add("Provisioning-2.0-Finalize-Interv-R2");
            routes.Add("QuoteManagement-1.0");
            routes.Add("ResourceInventoryManagement-1.0");
            routes.Add("ResourceInventoryManagement-5.0-Search-LPR-Optimus-CL");
            routes.Add("ResourceInventoryManagement-5.0-Search-LPR-Optimus-CO");
            routes.Add("ResourceInventoryManagement-5.0-Search-LPR-Optimus-EC");
            routes.Add("ResourceInventoryManagement-5.0-Search-LPR-Optimus-PE");
            routes.Add("ResourceInventoryManagement-5.0-Search-LPR-Optimus-UY");
            routes.Add("ResourceInventoryManagement-5.0-Search-Optimus-CL");
            routes.Add("ResourceInventoryManagement-5.0-Search-Optimus-CO");
            routes.Add("ResourceInventoryManagement-5.0-Search-Optimus-EC");
            routes.Add("ResourceInventoryManagement-5.0-Search-Optimus-PE");
            routes.Add("ResourceInventoryManagement-5.0-Search-Optimus-R2");
            routes.Add("ResourceInventoryManagement-5.0-Search-Optimus-UY");
            routes.Add("ResourceInventoryManagement-6.0-PR-By-Cust-Optimus-AR");
            routes.Add("ResourceInventoryManagement-6.0-PR-By-Cust-Optimus-CO");
            routes.Add("ResourceInventoryManagement-6.0-PR-By-Cust-Optimus-PE");
            routes.Add("ResourceInventoryManagement-6.0-PR-By-Cust-Optimus-UY");
            routes.Add("ResourceInventoryManagement-6.0-Search-LPR-Optimus-CL");
            routes.Add("ResourceInventoryManagement-6.0-Search-LPR-Optimus-PE");
            routes.Add("ResourceInventoryManagement-6.0-Search-LPR-Optimus-R2");
            routes.Add("ResourceInventoryManagement-6.0-Search-Optimus-CL");
            routes.Add("ResourceInventoryManagement-6.0-Search-Optimus-PE");
            routes.Add("ResourceInventoryManagement-6.0-Search-Optimus-UY");
            routes.Add("ResourceInventoryManagement-6.0-Upd-PR-Optimus-CL");
            routes.Add("ResourceInventoryManagement-6.0-Upd-PR-Optimus-CO");
            routes.Add("ResourceInventoryManagement-6.0-Upd-PR-Optimus-EC");
            routes.Add("ResourceInventoryManagement-6.0-Upd-PR-Optimus-UY");
            routes.Add("ResourceOrdering-2.0");
            routes.Add("ResourceOrdering-5.0-Create-Ports-Res-Optimus-CO");
            routes.Add("ResourceOrdering-5.0-Create-Ports-Res-Optimus-R2");
            routes.Add("ResourceOrdering-5.0-Update-Ports-Res-Optimus-CO");
            routes.Add("ResourceOrdering-6.0-Update-Ports-Res-Optimus-R2");
            routes.Add("SalesLead-1.0");
            routes.Add("SalesManagement-1.0");
            routes.Add("ServiceProblemManagement-4.0-Solv711-Optimus-CL");
            routes.Add("ServiceProblemManagement-4.0-Solv711-Optimus-PE");
            routes.Add("ServiceProblemManagement-4.0-Solv711-Optimus-R2");
            routes.Add("ServiceProblemManagement-4.0-Solv721-Optimus-CL");
            routes.Add("ServiceProblemManagement-4.0-Solv721-Optimus-PE");
            routes.Add("ServiceProblemManagement-4.0-Solv721-Optimus-R2");
            routes.Add("ServiceProblemManagement-5.0-Solv711-Optimus-CL");
            routes.Add("ServiceProblemManagement-5.0-Solv711-Optimus-PE");
            routes.Add("ServiceProblemManagement-5.0-Solv711-Optimus-R2");
            routes.Add("ServiceProblemManagement-5.0-Solv721-Optimus-CL");
            routes.Add("ServiceProblemManagement-5.0-Solv721-Optimus-PE");
            routes.Add("ServiceProblemManagement-5.0-Solv721-Optimus-R2");
            routes.Add("ServiceQualification-3.0");
            routes.Add("ServiceQualification-4.0");
            routes.Add("ServiceQualification-6.0");
            routes.Add("ServiceQualification-7.0");
            routes.Add("ServiceQualityManagement-1.0");
            routes.Add("ServiceQualityManagement-2.0");
            routes.Add("ServiceQualityManagement-2.0-Avail-4-CSL-Optimus-R2");
            routes.Add("ServiceQualityManagement-2.0-Check-SL-Optimus-CO");
            routes.Add("TestJenkins-2.0");
            routes.Add("TestMicrogatewayAnalitics-1.0");
            routes.Add("UsageConsumptionManagement-1.0");
            routes.Add("ValidateBackOffice-1.0");
            routes.Add("WorkforceManagement-3.0-Compl-WO-Optimus-AR");
            routes.Add("WorkforceManagement-3.0-Compl-WO-Optimus-CO");
            routes.Add("WorkforceManagement-3.0-Compl-WO-Optimus-PE");
            routes.Add("WorkforceManagement-3.0-Compl-WO-Optimus-UY");
            routes.Add("WorkforceManagement-3.0-Create-WO-Optimus-AR");
            routes.Add("WorkforceManagement-3.0-Get-WO-By-Cust-Optimus-AR");
            routes.Add("WorkforceManagement-3.0-Get-WO-By-Cust-Optimus-CO");
            routes.Add("WorkforceManagement-3.0-Get-WO-By-Cust-Optimus-PE");
            routes.Add("WorkforceManagement-3.0-Get-WO-By-Cust-Optimus-UY");
            routes.Add("WSSDSNETMobile-1.0");
            routes.Add("WSSDSNETMobile-3.0");


            return routes;
        }
    }
}