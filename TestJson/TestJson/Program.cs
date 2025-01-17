using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TestJson
{
    class Program
    {
        static void Main(string[] args)
        {


            var url = "http://172.22.126.182:8280/services/Customer360viewAR/view360/49801136";
            string urlParameters = "";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var dataObjects = response.Content.ReadAsStringAsync().Result;

                var settings = new JsonSerializerSettings();
                settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                var obj = JsonConvert.DeserializeObject<responseObj>(dataObjects, settings);



                //foreach (var d in dataObjects)
                //{
                //    Console.WriteLine("{0}", d.Name);
                //}
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            //Make any other calls using HttpClient here.

            //Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();

            //var content = client.DownloadString(url);

            //var parseado = JObject.Parse(content);

            // Create the Json serializer and parse the response
            //DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Clases.BlogInfo.BlogInfoMain));
            //using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(content)))
            //{
            //    var blogInfo = (Clases.BlogInfo.BlogInfoMain)serializer.ReadObject(ms);
            //    if (blogInfo.meta.status == 200)
            //    {
            //        twVista.Nodes.Clear();
            //        var root = twVista.Nodes.Add(blogInfo.response.blog.name);
            //        root.Nodes.Add("title : " + blogInfo.response.blog.title);
            //        root.Nodes.Add("url : " + blogInfo.response.blog.url);
            //        root.Nodes.Add("description : " + blogInfo.response.blog.description);
            //        root.Nodes.Add("likes : " + blogInfo.response.blog.likes.ToString());
            //        root.Nodes.Add("posts : " + blogInfo.response.blog.posts.ToString());

            //        twVista.ExpandAll();
            //    }
            //}


        }
    }
}
