using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.Http;
using RetailKing.Models;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;
using RetailKing.DataAcess;
using System.Web;
using System.Text;
using System.IO;
using crypto;
using RetailKing.RavendbClasses;


namespace RetailKing.Controllers
{
    public class VendController : ApiController
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        XmlCustomers cus = new XmlCustomers();
        XmlSupplier sup = new XmlSupplier();
        XmlTransactionLogs trnl = new XmlTransactionLogs();
        public string Cashier;
        public string Suppliername;
        public string SupplierId;
       
        // GET api/<controller>/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public TransactionResponse Transaction([FromBody] TransactionRequest trn)
        {
            TransactionResponse response = new TransactionResponse();
            #region test
            /*
            string Body = "";
              Body += "CustomerMSISDN=" + trn.CustomerMSISDN ;
                Body += "&CustomerAccount=" + trn.CustomerMSISDN;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=SMS";
                Body += "&Narrative=" + trn.Narrative;
                Body += "&ServiceId=17";
                Body += "&Amount=0";
                Body += "&MTI=0200";
                Body += "&ProcessingCode=320000";
       
            int ContentLength = Body.Length;
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("http://" + "localhost:24000" + "/api/Vend/Transaction");
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("263737444142:46495"));
            //live
            // String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("600024:a99L3v1N3@2017#"));
            httpWReq.Headers.Add("Authorization", "Basic " + encoded);
            httpWReq.Method = "Post";
            httpWReq.ContentLength = ContentLength;
            using (var streamWriter = new StreamWriter(httpWReq.GetRequestStream()))
            {
                // var json = new JavaScriptSerializer().Serialize(Body);
                streamWriter.Write(Body, 0, Body.Length);
            }
            HttpWebResponse tresponse = (HttpWebResponse)httpWReq.GetResponse();
            StreamReader sr = new StreamReader(tresponse.GetResponseStream());
            string returnvalue = sr.ReadToEnd();
            return new JavaScriptSerializer().Deserialize<TransactionResponse>(returnvalue); 
            */
            #endregion

            string authHeader = Request.Headers.Authorization.Parameter;
            string scheme = Request.Headers.Authorization.Scheme;
            if (authHeader != null && scheme == "Basic")
            {
                //Extract credentials
                string encodedUsernamePassword = authHeader.Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                int seperatorIndex = usernamePassword.IndexOf(':');

                string username = usernamePassword.Substring(0, seperatorIndex);
                string password = usernamePassword.Substring(seperatorIndex + 1);
                LoginController lg = new LoginController();
                string pass = lg.AuthenticateSupplier(username, password);
                char[] delimiter = new char[] { ',' };
                string[] parts = pass.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                     trn.AgentCode = parts[1];
                    var usa = db.AgentUsers.Where(u => u.UserName == username && u.AccountNumber == trn.AgentCode ).FirstOrDefault();
                              
                    SupplierId = parts[0];
                    Cashier = parts[2];
                    Suppliername = usa.FullName.Trim();
                    try
                    {
                        switch (trn.MTI.Substring(1, 1))
                        {
                            case "1":// authorisation Message
                                response.ResponseCode = "SignedIn";
                                response.Description = "Signed in";
                                break;
                            case "2":// financial message
                               response = serviceRequest(trn);
                                break;
                            case "3":// File Action message
                                break;
                            case "4":// Reversal
                                break;
                            case "5":// Reconciliation message(Transmits settlement information message)
                                break;
                            case "8":// networking message
                                break;
                            default:
                                response.ResponseCode = "06";
                                response.Description = "All request should follow ISO8583 Message type";
                                break;

                        }


                        return response;
                    }
                    catch (Exception e)
                    {
                        response.ResponseCode = "12";
                        response.Description = "transaction type not known";
                        return response;
                    }
                }
                else
                {
                    response.ResponseCode = "12";
                    response.Description = "Authentication error";
                    return response;

                }

            }
            else
            {
                response.ResponseCode = "12";
                response.Description = "No Authentication header";
                return response;
            }

        }

        // POST api/<controller>
        private TransactionResponse serviceRequest(TransactionRequest trn)
        {
            TransactionResponse response = new TransactionResponse();
            AccountController acc = new AccountController();
            try
            {
                switch (trn.ProcessingCode)
                {
                    case "310000":// Balance vendor enquiry
                        //acc.ser
                        break;
                    case "300000"://customer infor
                        response = VerifyAccount(trn.CustomerMSISDN);  //;
                        break;
                    case "320000"://get service 
                        response = GetServices(trn);
                        break;
                }
                return response;
            }
            catch (Exception e)
            {
                response.ResponseCode = "00008";
                response.Description = "Service Provider is unreachable. Please try later.";
                return response;
            }
        }

        private TransactionResponse GetProducts(TransactionRequest trn)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            TransactionResponse response = new TransactionResponse();

           /* 
            var co = np.GetProductByServiceId(trn.ServiceId).ToList();
            var toSend = (from e in co
                          select new { Name = e.Name.Trim(), Price = e.Price, Image = e.Image, ServiceProvider = e.ServiceProvider, Points = e.Points });
            response.MTI = "0210";
            response.ResponseCode = "00";
            response.ServiceId = trn.ServiceId;
            response.Narrative = new JavaScriptSerializer().Serialize(toSend);*/
            return response;
        }

        private TransactionResponse GetServices(TransactionRequest trn)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            TransactionResponse response = new TransactionResponse();
            var tt = db.Suppliers.Where(u => u.AccountCode == trn.AgentCode);
            /*var ser = np.GetServicesByAccountType(tt.AccountType);
            var co = ser.Where(u => u.AccountGroup == null);
            var toSend = (from e in co
                          select new { Name = e.Description.Trim(), Id = e.Id, Image = e.Image, ServiceProvider = e.ServiceProvider });
            response.MTI = "0210";
            response.ResponseCode = "00";
            response.ServiceId = trn.ServiceId;

            response.Narrative = new JavaScriptSerializer().Serialize(toSend);*/
            return response;
        }

        private TransactionResponse VerifyAccount(string id)
        {
           // NHibernateDataProvider np = new NHibernateDataProvider();
            TransactionResponse tr = new TransactionResponse();
            var customer = cus.GetCustomerProfile(id);
            customer.LoyaltySchemes = null;
            customer.suppliers =null;

            tr.CustomerData = new JavaScriptSerializer().Serialize(customer);
            return tr;
        }

        private TransactionResponse addPoints(Loyalty customer)
        {
            TransactionResponse tr = new TransactionResponse();
            var cc = sup.GetLoyalCustomers(customer.SupplierId, customer.CardNumber);
            if (cc != null && cc.Id != null)
            {
                customer.Id = cc.Id;
                customer.PhoneNumber = cc.PhoneNumber;
                // customer exists add loyalty points 
                var scheme = db.LoyaltySchemes.Find(cc.SchemeId);
                int points = Convert.ToInt32(Convert.ToDecimal(customer.MonetaryValue) / Convert.ToDecimal(scheme.PointCost));
                var uu = sup.AddLoyaltyPoints(customer.SupplierId, cc.PhoneNumber, points,decimal.Parse(scheme.PointValue.ToString()));
                tr.Description = "Success";
            }
            else
            {
                  if (customer.CardNumber.StartsWith("8") && customer.CardNumber.Length == 12)
                        {
                            customer.CardNumber = customer.CardNumber.Substring(0, 1) + "-" + customer.CardNumber.Substring(1, 4) + "-" + customer.CardNumber.Substring(5, 7);
                        }
                        else if (customer.CardNumber != null && customer.CardNumber.Length == 10)
                        {
                            customer.CardNumber = "263" + customer.CardNumber.Substring(1, customer.CardNumber.Length - 1);
                        }
                        else if (customer.CardNumber != null && customer.CardNumber.Length == 9)
                        {
                            customer.CardNumber = "263" + customer.CardNumber;
                        }
                        else if (customer.CardNumber != null && customer.CardNumber.Length == 12)
                        {
                        }
                        var accs = db.Customers.Where(u => u.Phone1.Trim() == customer.CardNumber || u.AccountCode.Trim() == customer.CardNumber).FirstOrDefault();
                        if (accs != null)
                        {
                            var lsc = db.LoyaltySchemes.Where(u => u.SupplierId == customer.SupplierId && u.Tier == 1).FirstOrDefault();
                            int points = Convert.ToInt32(Convert.ToDecimal(customer.MonetaryValue) / Convert.ToDecimal(lsc.PointCost));
                            customer.Points = points;
                            customer.MonetaryValue = Convert.ToDecimal(lsc.PointValue * points);
                            customer.Points = points;
                            customer.Id = accs.AccountCode.Trim();
                            customer.Name = accs.CustomerName;
                            customer.DOB = accs.Dated;
                            customer.PhoneNumber = accs.Phone1;
                            customer.SchemeId = lsc.Id;
                            //customer.SupplierName = part[1];
                            sup.CreateCustomerLoyalty(customer);
                            tr.Description = "Success";
                        }
                        else
                        {
                            tr.Description = "Customer not registered";
                        }
            }
            #region TransactionLog
            Tranlog log = new Tranlog();
            log.DestinationAccountNumber = customer.Id;
            log.SourceAccountNumber = SupplierId;
            log.Description = customer.Points + " " + customer.SupplierName + " loyalty points added on receipt No: " + customer.Branch + " receipt amount: " + customer.MonetaryValue;
            log.ReceiptNumber = customer.Branch;
            log.Amount = customer.MonetaryValue;
            log.TransactionTotal = customer.MonetaryValue;
            log.Username = Cashier;
            log.Status = "Complete";
            log.TransactionCharge = decimal.Parse("0.02");
            log.Commision = decimal.Parse("0.00");
            log.Miscellaneous = new JavaScriptSerializer().Serialize(customer);
            trnl.AddLog(log);
            #endregion

            #region External
            RoutoSMSTelecom routo = new RoutoSMSTelecom();
            routo.SetUser("Faithwork");//0772181813
            routo.SetPass("ks4kP1w");
            routo.SetNumber(customer.PhoneNumber);
            routo.SetOwnNumber(customer.SupplierId);
            routo.SetType("0");
            routo.SetMessage("You have received " + log.Description);
            //routo.unicodeEncodeMessage();
            string header = routo.Send();
            #endregion
            return tr;
        }
        private TransactionResponse redeemPoints(Loyalty customer)
        {
            TransactionResponse trn = new TransactionResponse();
            var cc = sup.GetRewardsBySchemeId(customer.SupplierId, customer.SchemeId).FirstOrDefault();


            if (cc != null)
            {
                // customer exists add loyalty points 
                // var scheme = db.LoyaltySchemes.Find(cc.SchemeId);
                int points = cc.Points;

                var uu = sup.RedeemLoyaltyPoints(customer.SupplierId, customer.CardNumber, points);

                #region TransactionLog
                Tranlog log = new Tranlog();
                log.DestinationAccountNumber = customer.Id;
                log.SourceAccountNumber = SupplierId;
                log.Description = customer.Points + " " + customer.SupplierName + " loyalty points for the " + cc.Name + " reward.";
                log.ReceiptNumber = customer.Branch;
                log.Amount = customer.MonetaryValue;
                log.TransactionTotal = customer.MonetaryValue;
                log.Username = Cashier;
                log.Status = "Complete";
                log.TransactionCharge = decimal.Parse("0.02");
                log.Commision = decimal.Parse("0.00");
                log.Miscellaneous = new JavaScriptSerializer().Serialize(customer);
                trnl.AddLog(log);
                #endregion

                #region External
                RoutoSMSTelecom routo = new RoutoSMSTelecom();
                routo.SetUser("Faithwork");//0772181813
                routo.SetPass("ks4kP1w");
                routo.SetNumber(customer.PhoneNumber);
                routo.SetOwnNumber(customer.SupplierId);
                routo.SetType("0");
                routo.SetMessage("You have redeemed " + log.Description);
                //routo.unicodeEncodeMessage();
                string header = routo.Send();
                #endregion
                trn.Description = "Success";
            }
            else
            {
                trn.Description = "Customer not registered";
            }
            return trn;
        }
      
    }
}