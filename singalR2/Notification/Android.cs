using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Notification
{
    public class Android
    {
        public string CheckAuthentication(string SenderID, string Password)
        {
            string Array = "";

            string URL = "https://www.google.com/accounts/ClientLogin?";
            string fullURL = URL + "Email=" + SenderID.Trim() + "&Passwd=" + Password.Trim() +
              "&accountType=GOOGLE" + "&source=Company-App-Version" + "&service=ac2dm";
            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(fullURL);

            try
            {
                //-- Post Authentication URL --//
                HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
                StreamReader Reader;
                int Index = 0;

                //-- Check Response Status --//
                if (Response.StatusCode == HttpStatusCode.OK)
                {
                    Stream Stream = Response.GetResponseStream();
                    Reader = new StreamReader(Stream);
                    string File = Reader.ReadToEnd();

                    Reader.Close();
                    Stream.Close();
                    Index = File.ToString().IndexOf("Auth=") + 5;
                    int len = File.Length - Index;
                    Array = File.ToString().Substring(Index, len);
                }
            }
            catch (Exception ex)
            {
                Array = ex.Message;
                ex = null;
            }
            return Array;
        }

        public string SendMessage(string RegistrationID, string Message, string AuthString)
        {
            //-- Create C2DM Web Request Object --//
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(
                     "https://android.clients.google.com/c2dm/send");
            Request.Method = "POST";
            Request.KeepAlive = false;

            //-- Create Query String --//
            NameValueCollection postFieldNameValue = new NameValueCollection();
            postFieldNameValue.Add("registration_id", RegistrationID);
            postFieldNameValue.Add("collapse_key", "1");
            postFieldNameValue.Add("delay_while_idle", "0");
            // postFieldNameValue.Add("data.message", Message);
            postFieldNameValue.Add("data.payload", Message);
            string postData = GetPostStringFrom(postFieldNameValue);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            Request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            Request.ContentLength = byteArray.Length;

            Request.Headers.Add(HttpRequestHeader.Authorization, "GoogleLogin auth=" + AuthString);

            //-- Delegate Modeling to Validate Server Certificate --//
            ServicePointManager.ServerCertificateValidationCallback += delegate(
                        object
                        sender,
                        System.Security.Cryptography.X509Certificates.X509Certificate
                        pCertificate,
                        System.Security.Cryptography.X509Certificates.X509Chain pChain,
                        System.Net.Security.SslPolicyErrors pSSLPolicyErrors)
            {
                return true;
            };

            //-- Create Stream to Write Byte Array --// 
            Stream dataStream = Request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //-- Post a Message --//
            WebResponse Response = Request.GetResponse();
            HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
            if (ResponseCode.Equals(HttpStatusCode.Unauthorized) ||
                      ResponseCode.Equals(HttpStatusCode.Forbidden))
            {
                return "Unauthorized - need new token";
            }
            else if (!ResponseCode.Equals(HttpStatusCode.OK))
            {
                return "Response from web service isn't OK";
            }

            StreamReader Reader = new StreamReader(Response.GetResponseStream());
            string responseLine = Reader.ReadLine();
            Reader.Close();

            return responseLine;
        }

        private string GetPostStringFrom(NameValueCollection postFieldNameValue)
        {
            //throw new NotImplementedException();
            List<string> items = new List<string>();

            foreach (String name in postFieldNameValue)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(postFieldNameValue[name])));

            return String.Join("&", items.ToArray());
        }
    }
}
