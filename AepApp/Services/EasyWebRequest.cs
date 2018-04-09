﻿using System;

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

#if __MOBILE__
using Newtonsoft.Json;
#endif

namespace CloudWTO.Services
{
    public class EasyWebRequest
    {
        delegate void ErrCall(Exception ex);
        private ErrCall callBack;
        public static string sendGetHttpWebRequest(string url)
        {

            ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //req.Host = "example.com";
            req.Headers.Add(HttpRequestHeader.Authorization, App.token);//给请求添加权限
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
                Console.WriteLine("ex:"+ex.Message);

                res = (HttpWebResponse)ex.Response;
                Console.WriteLine("ex:555555555");

            }
            Console.WriteLine("ex:1111111");

            StreamReader sr = new StreamReader(res.GetResponseStream());
            Console.WriteLine("ex:22222222" );

            string result = sr.ReadToEnd();
            Console.WriteLine("ex:333333333");

            Console.WriteLine("ex:" + result);

            return result;
        }

            public static string sendPOSTHttpWebRequest(string url, string param)
        {
           

            try
            {            
                ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;
                byte[] bs = Encoding.GetEncoding("UTF-8").GetBytes(param);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                //req.ContentType = "application/x-www-form-urlencoded";
                req.ContentType = "application/json";
                //req.Headers.Add(HttpRequestHeader.Authorization,@"string");//给请求添加权限
                req.ContentLength = bs.Length;
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

        public static string sendGetWithAuthorHttpWebRequest(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //req.Host = "example.com";
            req.Headers.Add(HttpRequestHeader.Authorization, "Bearer 37e1ebe3-fbeb-4a0a-b899-1ac606ab978b");//给请求添加权限
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



        static bool MyCertHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
        {
            Console.WriteLine("hi");
            // Ignore errors
            return true;
        }

   
    }
}
