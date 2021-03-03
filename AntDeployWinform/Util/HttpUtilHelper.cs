using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AntDeployWinform.Util
{
    public class Artifacts
    {
        public DateTime push_time { get; set; }
        public int project_id { get; set; }
        public List<ImageTags> tags { get; set; } = new List<ImageTags>();
    }

    public class ImageTags
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime push_time { get; set; }
    }

    public static class HttpUtilHelper
    {
        public static string GetProjectImagesTag(string projectName, string imageName,string httpUrl)
        {
            var url =
                $"https://{httpUrl}/api/v2.0/projects/{projectName}/repositories/{imageName}/artifacts?page=1&page_size=100&with_tag=true&with_label=false&with_scan_overview=false&with_signature=false&with_immutable_status=false";

            var one = HttpGetMethod(HttpMethod.Get, url);


            var result = JsonConvert.DeserializeObject<List<Artifacts>>(one)
                .OrderByDescending(o => o.push_time)
                .FirstOrDefault()?.tags.OrderByDescending(item => item.push_time).FirstOrDefault();

            return result?.name;
        }

        private static string HttpGetMethod(HttpMethod method, string url)
        {
            //List<KeyValuePair<string, string>> formData = null;

            var requestHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate, chain, sslPolicyErrors) => true
            };

            var httpClient = new HttpClient(requestHandler);

            //if (formData != null)
            //{
            //    HttpContent content = new FormUrlEncodedContent(formData);
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            //    content.Headers.ContentType.CharSet = "UTF-8";

            //    for (var i = 0; i < formData.Count; i++) content.Headers.Add(formData[i].Key, formData[i].Value);
            //}

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            //for (var i = 0; i < formData?.Count; i++) request.Headers.Add(formData[i].Key, formData[i].Value);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("X-Is-Resource-Name", "false");

            var res = httpClient.SendAsync(request).GetAwaiter().GetResult();

            var one = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return one;
        }
    }
}
