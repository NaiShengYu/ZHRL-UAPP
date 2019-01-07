using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AepApp;
using Plugin.Media.Abstractions;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Specialized;

#if __MOBILE__
using Newtonsoft.Json;
#endif

namespace CloudWTO.Services
{
    public class HTTPResponse
    {
        public string Results { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    public class EasyWebRequest
    {
        delegate void ErrCall(Exception ex);
        private ErrCall callBack;
        public static string sendGetHttpWebRequest(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //req.Host = "example.com";
            req.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + App.FrameworkToken);//给请求添加权限
            req.ContentType = "application/json";
            req.Method = "GET";
            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)req.GetResponse();

            }
            catch (WebException ex)
            {
                res = (HttpWebResponse)ex.Response;

            }
            StreamReader sr = new StreamReader(res.GetResponseStream());
            string result = sr.ReadToEnd();
            Console.WriteLine("ex:" + result);

            return result;
        }

        public static async Task<HTTPResponse> SendHTTPRequestAsync(string url, string param, string method = "GET", string token = null, string contenttype = "json")
        {
            HttpWebResponse res = null;
            string result = null;
            try
            {
                Console.WriteLine("请求URL：" + url);
                Console.WriteLine("请求token：" + "Bearer " + token);
                Console.WriteLine("请求参数：" + param);
                ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    req.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);//给请求添加权限
                }
                req.ContentType = "application/json";
                if (method.Equals("GET"))
                {
                    req.Method = "GET";
                }
                else
                {
                    byte[] bs = Encoding.UTF8.GetBytes(param);

                    req.Method = method;

                    if (contenttype == "json")
                    {
                        req.ContentType = "application/json";
                    }
                    else
                    {
                        req.ContentType = "application/x-www-form-urlencoded";
                    }

                    req.ContentLength = bs.Length;
                    try
                    {
                        Stream requestStream = await req.GetRequestStreamAsync();
                        await requestStream.WriteAsync(bs, 0, bs.Length);
                        requestStream.Close();
                    }
                    catch (WebException ex)
                    {
                        int a = 0;
                    }
                }
                WebResponse wr = await req.GetResponseAsync();
                res = wr as HttpWebResponse;
                StreamReader sr = new StreamReader(res.GetResponseStream());
                result = await sr.ReadToEndAsync();
                sr.Close();
            }
            catch (Exception ex)
            {
                result = ex == null ? "" : ex.Message;
                Console.WriteLine("错误信息：====" + ex +"错误的URL"+url);
                return new HTTPResponse { Results = result, StatusCode = HttpStatusCode.ExpectationFailed };
            }
            Console.WriteLine(url + "===ex:" + result);

            return new HTTPResponse { Results = result, StatusCode = res.StatusCode };
        }

        public static async Task<HTTPResponse> HTTPRequestDownloadAsync(string url, string fileN, string token = null) {

            HttpWebResponse res = null;
            string result = null;
            try
            {
                Console.WriteLine("请求URL：" + url);
                Console.WriteLine("请求token：" + "Bearer " + token);
                Console.WriteLine("请求参数：" + fileN);

                ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                if (token != null)
                {
                    req.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);//给请求添加权限
                }
                req.ContentType = "application/json";
                req.Method = "GET";
                WebResponse wr = await req.GetResponseAsync();
                res = wr as HttpWebResponse;
                //存储路径
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                //存储文件名
                string filename = Path.Combine(path, fileN);
                //创建一个文件流，路径为flieName
                FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                //获取到数据库的数据流
                Stream stream = res.GetResponseStream();
                //将数据库的数据流拷贝到本地文件流中
                stream.CopyTo(fileStream);
                fileStream.Close();
                //查看文件流内容长度
                FileInfo file = new FileInfo(filename);
                Console.WriteLine("fileLength===" + file.Length);

            }
            catch (WebException ex)
            {
                result = ex.Message;
                return new HTTPResponse { Results = result, StatusCode = HttpStatusCode.ExpectationFailed };
            }
            Console.WriteLine(url + "===ex:" + result);

            return new HTTPResponse { Results = result, StatusCode = res.StatusCode };


        }

        public static async Task<HTTPResponse> upload(string filePath, string Suffix, string baseUrl, string requestUri, NameValueCollection nameValue = null)
        {
            HttpWebResponse res = null;
            string result = null;
            try
            {
                var upfilebytes = File.ReadAllBytes(filePath);
                var imageStream = new MemoryStream(upfilebytes);
                //var imageStream = new ByteArrayContent(upfilebytes);
                Console.WriteLine("fileLength===" + upfilebytes.Length);

                var multi = new MultipartFormDataContent();
                //这句话很关键第一个“files”是接口参数名，第二个文件后缀名(.jpg,.png)
                multi.Add(new StreamContent(imageStream), "files", Suffix);
                if(nameValue != null)
                {
                    foreach (string key in nameValue.Keys)
                    {
                        multi.Add(new StringContent(nameValue[key]), key);
                    }
                }

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.EmergencyToken);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage resurlt1 = await client.PostAsync(requestUri, multi);
                result = await resurlt1.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                result = ex.Message;
                return new HTTPResponse { Results = result, StatusCode = HttpStatusCode.ExpectationFailed };
            }
            Console.WriteLine("ex:" + result);
            return new HTTPResponse { Results = result, StatusCode = HttpStatusCode.OK };

        }


        public static string sendGetHttpWebRequestWithNoToken(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            HttpWebResponse res;
            try
            {
                Console.WriteLine("ex:666666");

                res = (HttpWebResponse)req.GetResponse();
                Console.WriteLine("ex:0000000");

            }
            catch (WebException ex)
            {
                Console.WriteLine("ex:" + ex.Message);

                res = (HttpWebResponse)ex.Response;
                Console.WriteLine("ex:555555555");

            }
            Console.WriteLine("ex:1111111");

            StreamReader sr = new StreamReader(res.GetResponseStream());
            Console.WriteLine("ex:22222222");

            string result = sr.ReadToEnd();
            Console.WriteLine("ex:333333333");

            Console.WriteLine("ex:" + result);

            return result;
        }

        public static string sendPOSTHttpWebWithTokenRequest(string url, string param)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;
                byte[] bs = Encoding.GetEncoding("UTF-8").GetBytes(param);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.ContentType = "application/json";
                if(!string.IsNullOrWhiteSpace(App.token))
                    req.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + App.token);//给请求添加权限
                req.ContentLength = bs.Length;
                req.Method = "POST";
                //req.ContentType = "application/x-www-form-urlencoded";               
                Stream requestStream = req.GetRequestStream();
                requestStream.Write(bs, 0, bs.Length);
                requestStream.Close();
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(res.GetResponseStream());
                string result = sr.ReadToEnd();

                sr.Close();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex:" + ex.Message);


                return ex.Message;

            }
        }



        static bool MyCertHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
        {
            Console.WriteLine("hi");
            // Ignore errors
            return true;
        }

        public abstract class Callback<T>{
            public abstract void OnSuccess(T data);
            public abstract void OnFailed();
        }

    }
}
