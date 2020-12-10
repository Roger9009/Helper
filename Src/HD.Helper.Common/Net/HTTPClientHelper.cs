using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Net;
using HD.Helper.Common;
using System.Threading.Tasks;

namespace HD.Common.Net
{
    public class HTTPClientHelper
    {

        private static readonly HttpClient HttpClient;
        static HTTPClientHelper()
        {
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };
            HttpClient = new HttpClient(handler);
        }


        #region GET

        /// <summary>
        /// get请求，可以对请求头进行多项设置
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetResponseByGet(string url)
        {
            return GetResponseByGet(url, null);
        }
        /// <summary>
        /// get请求，可以对请求头进行多项设置
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public static string GetResponseByGet(string url, List<KeyValuePair<string, string>> paramArray)
        {
            string result = "";

            var httpclient = HTTPClientHelper.HttpClient;

            if (paramArray != null)
                url = url + "?" + BuildParam(paramArray);

            var response = httpclient.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Stream myResponseStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                result = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
            }

            return result;
        }


        public static string GetResponseBySimpleGet(string url)
        {
            return GetResponseBySimpleGet(url, null);
        }
        public static string GetResponseBySimpleGet(string url, List<KeyValuePair<string, string>> paramArray)
        {

            var httpclient = HTTPClientHelper.HttpClient;

            if (paramArray != null)
                url = url + "?" + BuildParam(paramArray);

            var result = httpclient.GetStringAsync(url).Result;
            return result;
        }


        #endregion

        #region POST
        /// <summary>
        /// post请求，可以对请求头进行多项设置
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public static string HttpPostRequestAsync(string Url, List<KeyValuePair<string, string>> paramArray, string ContentType = "application/json")
        {
            return HttpPostRequestAsync(Url, paramArray, null, ContentType);
        }

        public static string HttpPostRequestAsync(string Url, object objBody, string ContentType = "application/json")
        {
            return HttpPostRequestAsync(Url, null, objBody, ContentType);
        }

        public static string HttpPostRequestAsync(string Url, List<KeyValuePair<string, string>> paramArray, object objBody, string ContentType = "application/x-www-form-urlencoded")
        {
            string result = "";

            var postData = BuildParam(paramArray);
            string strBody = string.Empty;

            if (paramArray != null)
                Url = string.Concat(new string[] { Url, "?", postData });

            if (objBody != null)
                strBody = JsonHelper.ObjectToJson(objBody);

            try
            {
                using (HttpClient http = new HttpClient())
                {
                    //http.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (compatible; Baiduspider/2.0; +http://www.baidu.com/search/spider.html)");
                    //http.DefaultRequestHeaders.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

                    HttpResponseMessage message = null;

                    using (StringContent content = new StringContent(strBody, Encoding.UTF8, ContentType))
                    {
                        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                        var task = http.PostAsync(Url, content);
                        message = task.Result;
                    }

                    if (message != null && message.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        using (message)
                        {
                            result = message.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }



        #endregion

        #region DownLoad

        public static async Task DownloadImags(string url, List<KeyValuePair<string, string>> paramArray, string newpath)
        {
            if (paramArray != null)
                url = url + "?" + BuildParam(paramArray);

            try
            {
                using (var client = new HttpClient())
                {
                    System.IO.FileStream fs;
                    byte[] urlContents = await client.GetByteArrayAsync(url);
                    fs = new System.IO.FileStream(newpath, System.IO.FileMode.CreateNew);
                    fs.Write(urlContents, 0, urlContents.Length);

                    fs.Dispose();
                    fs.Close();
                }

            }
            catch
            {
                throw;
            }

        }


        #endregion
        private static string Encode(string content, Encoding encode = null)
        {
            if (encode == null) return content;

            return System.Web.HttpUtility.UrlEncode(content, Encoding.UTF8);

        }

        private static string BuildParam(List<KeyValuePair<string, string>> paramArray, Encoding encode = null)
        {
            string url = "";

            if (encode == null) encode = Encoding.UTF8;

            if (paramArray != null && paramArray.Count > 0)
            {
                var parms = "";
                foreach (var item in paramArray)
                {
                    parms += string.Format("{0}={1}&", Encode(item.Key, encode), Encode(item.Value, encode));
                }
                if (parms != "")
                {
                    parms = parms.TrimEnd('&');
                }
                url += parms;

            }
            return url;
        }

    }

}
