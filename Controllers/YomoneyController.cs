using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.DataAcess;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.Helpers;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RetailKing.RavendbClasses;
using System.Web.Http.Cors;
using System.Text;
using Newtonsoft.Json;

namespace RetailKing.Controllers
{
    public class YomoneyController : ApiController
    {
        RetailKingEntities db = new RetailKingEntities();
        NHibernateSessionManager ns = new NHibernateSessionManager();
        XmlCustomers cus = new XmlCustomers();
        XmlSupplier sup = new XmlSupplier();
        XmlTransactionLogs trnl = new XmlTransactionLogs();
        public string Cashier;
        public string Suppliername;
        public string SupplierId;
        //YomoneyRequest rq = new YomoneyRequest();

        //public YomoneyRequest PaymentMethods()
        //{
        //    rq.MTI = "0300";
        //    rq.ProcessingCode = "";

        //        return View();

        //}

        public YomoneyResponse GetExternal(YomoneyRequest trn)
        {
            YomoneyResponse tr = new YomoneyResponse();
            String Body = "";
            Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
            Body += "&CustomerData=" + trn.CustomerData;
            Body += "&CustomerAccount=" + trn.CustomerAccount;
            Body += "&AgentCode=" + trn.AgentCode;
            Body += "&Action=SMS";
            Body += "&Narrative=" + trn.Narrative;
            Body += "&Note=" + trn.Note;
            Body += "&ServiceId=" + trn.ServiceId;
            Body += "&Amount=" + trn.Amount;
            Body += "&MTI=" + trn.MTI;
            Body += "&TransactionType=" + trn.TransactionType;
            Body += "&ProcessingCode=" + trn.ProcessingCode;


            //int ContentLength = Body.Length;

            String Host = "www.yomoneyservice.com";//24005

            int ContentLength = Body.Length;
            //int ContentLength = json.Length;
            //String Header = "POST  /bulksms/bulksms?"+ Body +" HTTP/1.0\n" + "Host: " + Host + "\n";
            RetailKingEntities db = new RetailKingEntities();
            String returnvalue = "";
            try
            {
                var pro = db.ProxySettings.FirstOrDefault();
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("http://" + Host + "/yoclient/Transaction?" + Body);
                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.Method = "Post";
                httpWReq.Timeout = 150000;

                if (pro != null && pro.IpAddress.Trim() != "0.0.0.0")
                {
                    //"172.18.0.140"
                    WebProxy myproxy = new WebProxy(pro.IpAddress.Trim(), int.Parse(pro.Port));
                    myproxy.BypassProxyOnLocal = true;
                    httpWReq.Proxy = myproxy;
                }
                httpWReq.ContentLength = ContentLength;
                using (var streamWriter = new StreamWriter(httpWReq.GetRequestStream()))
                {
                    streamWriter.Write(Body, 0, Body.Length);
                }
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                returnvalue = sr.ReadToEnd();
                if (returnvalue.Length >= 4 && returnvalue.Substring(0, 4) == "1701")
                {
                    tr.ResponseCode = "00000";
                }
                else
                {
                //tr = JsonConvert.DeserializeObject<TransactionResponse>(returnvalue);
                tr = new JavaScriptSerializer().Deserialize<YomoneyResponse>(returnvalue);
                }
            }
            catch (Exception ex)
            {
                tr.ResponseCode = "00000";
                tr.Narrative = "";

            }

            return tr;//Header + " " + line;
        }
        public YomoneyResponse Payment(YomoneyRequest req)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            YomoneyResponse resp = new YomoneyResponse();
                       
            IList<YomoneyResponse> pd = new List<YomoneyResponse>();
            YomoneyController pc = new YomoneyController();
            switch (req.TransactionType)
            {
                case 1:
                    break;
                case 2:
                    switch (req.MTI)
                    {
                        case "0300":
                            switch (req.ProcessingCode)
                            {
                                case "420000":
                                    var getlist = GetExternal(req);

                                    resp = getlist;

                                    //foreach (var res in resp)
                                    //{
                                    //    resp.Name = res.Name;
                                    //    pd.Add(resp);
                                    //}


                                    // RedirectToAction("PaymentMethods",pd);
                                    // return pd;
                                    break;

                                case "400000":
                                    break;

                                case "430000":
                                    break;

                                case "440000":
                                    break;
                            }
                            break;
                        case "0200":
                            switch (req.ProcessingCode)
                            {
                                case "300000":

                                    break;

                                case "310000":
                                    break;

                                case "340000":
                                    break;

                                case "320000":
                                    var getdata = GetExternal(req);
                                    resp = getdata;
                                    break;

                                case "330000":
                                    var getbills = GetExternal(req);
                                    resp = getbills;
                                    break;
                            }
                            break;
                        case "0100":

                            break;
                    }
                    break;
                case 3:
                    switch (req.MTI)
                    {
                        case "0300":
                            switch (req.ProcessingCode)
                            {
                                case "420000":
                                    var getlist = pc.GetExternal(req);

                                    resp = getlist;

                                    //foreach (var res in resp)
                                    //{
                                    //    resp.Name = res.Name;
                                    //    pd.Add(resp);
                                    //}


                                    // RedirectToAction("PaymentMethods",pd);
                                    // return pd;
                                    break;

                                case "400000":
                                    break;

                                case "430000":
                                    break;

                                case "440000":
                                    break;
                            }
                            break;
                        case "0200":
                            switch (req.ProcessingCode)
                            {
                                case "300000":

                                    break;

                                case "310000":
                                    break;

                                case "340000":
                                    break;

                                case "320000":
                                    var getdata = pc.GetExternal(req);
                                    resp = getdata;
                                    break;

                                case "330000":
                                    var getbills = pc.GetExternal(req);
                                    resp = getbills;
                                    break;
                            }
                            break;
                        case "0100":

                            break;
                    }
                    break;
                case 4:
                    break;
                case 5:
                    switch (req.MTI)
                    {
                        case "0300": // gelists
                            switch (req.ProcessingCode)
                            {
                                case "420000":
                                    var getlist = pc.GetExternal(req);

                                    resp = getlist;

                                    //foreach (var res in resp)
                                    //{
                                    //    resp.Name = res.Name;
                                    //    pd.Add(resp);
                                    //}


                                    // RedirectToAction("PaymentMethods",pd);
                                    // return pd;
                                    break;

                                case "400000":
                                    break;

                                case "430000":
                                    break;

                                case "440000":
                                    break;
                            }
                            break;
                        case "0200":
                            switch (req.ProcessingCode)
                            {
                                case "300000":

                                    break;

                                case "310000":
                                    break;

                                case "340000":
                                    break;

                                case "320000":
                                    var getdata = GetExternal(req);
                                    resp = getdata;
                                    break;

                                case "330000":
                                    break;
                            }
                            break;
                        case "0100":

                            break;
                    }
                    break;

            }



            return resp;
        }
        
    }
}
    
