using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http.Headers;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //validar parametros, deben ser 2 url
            if (args.Length < 3)
            {
                Console.WriteLine("Debe enviar ambiente, pais y cliente");
                return;
            }
            var ambiente = args[0];
            if (!ambiente.Equals("QAM") && !ambiente.Equals("PRD"))
            {
                Console.WriteLine("Ambientes validos QAM y PRD");
                return;
            }
            var pais = args[1];
            var cliente = args[2];

            //https://customerinfo-fdv-360view-qam.dtvpan.com/api/GetCustomer360View?token=12354&i_customer={0}&i_country={1}&i_sync_on=1&format=json
            //http://rgqamk2v.dtvpan.com:3213/api/GetCustomer360View?token=1234&i_customer={0}&i_requestId=123456&i_systemId=K2V&i_country={1}&i_sync_on=1&format=json

            var urlFDV = "";
            var urlK2V = "";
            if (ambiente== "QAM")
            {
                urlFDV = String.Format("https://customerinfo-fdv-360view-qam.dtvpan.com/api/GetCustomer360View?token=12354&i_requestId=123456&i_systemId=K2V&i_customer={0}&i_country={1}&i_sync_on=1&format=json", cliente, pais);
                urlK2V = String.Format("http://rgqamk2v.dtvpan.com:3213/api/GetCustomer360View?token=1234&i_customer={0}&i_requestId=123456&i_systemId=K2V&i_country={1}&i_sync_on=1&format=json", cliente, pais); ;
            }
            if (ambiente == "PRD")
            {
                urlFDV = String.Format("https://customerinfo-fdv-360view.dtvpan.com/api/GetCustomer360View?token=12354&i_customer={0}&i_country={1}&i_sync_on=1&format=json", cliente, pais);
                urlK2V = String.Format("http://172.29.167.10:3213/api/GetCustomer360View?token=a0e046b6-71d1-2ae8-b9a4-df1af29c6f50&i_customer={0}&i_requestId=123456&i_systemId=K2V&i_country={1}&i_sync_on=1&format=json", cliente, pais); ;
            }
            Console.WriteLine("URL FDV:" + urlFDV);

            Console.WriteLine("URL K2V:" + urlK2V);

            string original = "";
            string duplicate = "";

            //invocar ambas soluciones, validar ambas retornan http 200
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                using (var response = httpClient.GetStringAsync(urlFDV))
                {
                    response.Wait();
                    original = response.Result;
                }
                using (var response = httpClient.GetStringAsync(urlK2V))
                {
                    response.Wait();
                    duplicate = response.Result;
                }
            }

            //obtener jsons

            //comparar, informar diferencias
            var jdp = new JsonDiffPatch();
            var left = JToken.Parse(original);
            var right = JToken.Parse(duplicate);

            JToken patch = jdp.Diff(left, right);

            Console.WriteLine(patch.ToString());
        }
    }
}
