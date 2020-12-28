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
        /// <param name="dicHead"></param>
        /// <returns></returns>
        public static string GetResponseByGet(string url, Dictionary<string, string> dicHead = null)
        {
            string result = "";

            using (HttpClient http = new HttpClient())
            {
                if (dicHead != null)
                {
                    if (dicHead != null)
                    {
                        foreach (var item in dicHead)
                        {
                            http.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }
                }

                HttpResponseMessage message = null;
                var task = http.GetAsync(url);
                message = task.Result;

                if (message != null && message.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (message)
                    {
                        result = message.Content.ReadAsStringAsync().Result;
                    }
                }
                else
                {
                    result = url + " 调试不通。" + message.StatusCode.ToString();
                }
            }

            return result;
        }



        public static string GetResponseBySimpleGet(string url, Dictionary<string, string> dicHead = null)
        {
            string result = "";
            var httpclient = HTTPClientHelper.HttpClient;

            try
            {


                using (HttpClient http = new HttpClient())
                {
                    if (dicHead != null)
                    {
                        if (dicHead != null)
                        {
                            foreach (var item in dicHead)
                            {
                                http.DefaultRequestHeaders.Add(item.Key, item.Value);
                            }
                        }
                    }

                    HttpResponseMessage message = null;
                    result = http.GetStringAsync(url).Result;

                }
            }
            catch (Exception ex)
            {
                result = url + " " + ex.Message;
            }


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
        public static string PostRequestAsync(string url, string ContentType = "application/json")
        {
            return PostRequestAsync(url, null, null, ContentType);
        }

        public static string PostRequestAsync(string url, string strBody, string ContentType = "application/json")
        {
            return PostRequestAsync(url, strBody, null, ContentType);
        }

        public static string PostRequestAsync(string url, string strBody, Dictionary<string, string> dicHead = null, string ContentType = "application/json")
        {
            string result = "";

            try
            {
                using (HttpClient http = new HttpClient())
                {

                    if (dicHead != null)
                    {
                        foreach (var item in dicHead)
                        {
                            http.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }

                    HttpResponseMessage message = null;

                    using (StringContent content = new StringContent(strBody, Encoding.UTF8, ContentType))
                    {
                        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                        var task = http.PostAsync(url, content);
                        message = task.Result;
                    }

                    if (message != null && message.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        using (message)
                        {
                            result = message.Content.ReadAsStringAsync().Result;
                        }
                    }
                    else
                    {
                        result = url + " 调试不通。" + message.StatusCode.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }



        #endregion

        #region DownLoad

        public static async Task DownloadImags(string url, string newpath)
        {


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


        public static string BuildParam(Dictionary<string, string> paramArray, Encoding encode = null)
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
