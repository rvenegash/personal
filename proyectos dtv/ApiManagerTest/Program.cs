using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;

namespace ApiManagerTest
{
    class Program
    {
        static void Main(string[] args)
        {
   

            HttpClient clientToken = new HttpClient();

            //obtener valores de configuración
            string sUrlAM_UrlToken = ConfigurationManager.AppSettings["APIM_Token_Url"];
            string sAM_ConsumerKey = ConfigurationManager.AppSettings["APIM_ConsumerKey"];
            string sAM_ConsumerSecret = ConfigurationManager.AppSettings["APIM_ConsumerSecret"];
            string AM_AccessToken = "";
            string AM_RefreshToken = "";

            //armar header authorization
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", sAM_ConsumerKey, sAM_ConsumerSecret));

            //invocar para pedir nuevo token
            clientToken.BaseAddress = new Uri(sUrlAM_UrlToken);
            clientToken.DefaultRequestHeaders.Clear();
            clientToken.DefaultRequestHeaders.Add("Authorization", "Basic " + System.Convert.ToBase64String(plainTextBytes));

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("validity_period", "3600")
              });

            var responseMessage = clientToken.PostAsync("/token", formContent).Result;

            if (responseMessage.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<APIM_Token>(responseMessage.Content.ReadAsStringAsync().Result);
                AM_AccessToken = data.access_token;
                AM_RefreshToken = data.refresh_token;
            }

        }
    }
}
