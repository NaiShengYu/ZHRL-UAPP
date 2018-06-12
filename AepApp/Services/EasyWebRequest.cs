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
            req.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + App.token);//给请求添加权限
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

        public static async Task<HTTPResponse> SendHTTPRequestAsync(string url, string param, string method = "GET", string token = null, string contenttype="json")
        {
            HttpWebResponse res = null;
            string result = null;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                if (token != null)
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

                    req.Method = "POST";

                    if (contenttype=="json")
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
                        //requestStream.Write(bs, 0, bs.Length);
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
                result = ex.Message;
                return new HTTPResponse { Results = result, StatusCode = HttpStatusCode.ExpectationFailed };
            }
            Console.WriteLine("ex:" + result);

            return new HTTPResponse { Results = result, StatusCode = res.StatusCode };
        }


        public static async Task<HTTPResponse> HTTPRequestDownloadAsync(string url,string fileN,string token = null){

            HttpWebResponse res = null;
            string result = null;
            try
            {
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
                FileStream fileStream = new FileStream(filename,FileMode.Create, FileAccess.Write);
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
            Console.WriteLine("ex:" + result);

            return new HTTPResponse { Results = result, StatusCode = HttpStatusCode.ExpectationFailed };
  

        }
       
        public static async void UploadImage(MediaFile mediaFile)
        {
            //variable
            var url = App.EmergencyModule.url + "/api/File/Upload";
            var file = mediaFile.Path;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;
                //read file into upfilebytes array
                var upfilebytes = File.ReadAllBytes(file);

                //create new HttpClient and MultipartFormDataContent and add our file, and StudentId
                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();
                ByteArrayContent baContent = new ByteArrayContent(upfilebytes);
                StringContent studentIdContent = new StringContent("2123");
                content.Add(baContent, "File", "filename.ext");
                content.Add(studentIdContent, "StudentId");


                //upload MultipartFormDataContent content async and store response in response var
                var response =
                  await client.PostAsync(url, content);

                //read response result as a string async into json var
                var responsestr = response.Content.ReadAsStringAsync().Result;

                //debug
                Console.WriteLine(responsestr);

            }
            catch (Exception e)
            {
                //debug
                Console.WriteLine("Exception Caught: " + e.ToString());

                return;
            }
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
     
    }
}
