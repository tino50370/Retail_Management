using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Text;
using RetailKing.Models;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for RoutoSMSTelecom
/// </summary>
public class RoutoSMSTelecom
{
	public RoutoSMSTelecom()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public String username = "";
    public String password = "";
    public String destination = "";
    public String source = "";
    public String message = "";
    public String messageId = "";
    public String type = "";
    public String model = "";
    public String op = "";
    public String dlr="1";
    public String SetUser(String newuser)
    {
        this.username = newuser;
        return username;
    }

    public String SetPass(String newpass)
    {
        this.password = newpass;
        return password;
    }

    public String SetNumber(String newnumber)
    {
        this.destination = newnumber;
        return destination;
    }

    public String SetOwnNumber(String newownnum)
    {
        this.source = newownnum;
        return source;
    }

    public String SetType(String newtype)
    {
        this.type = newtype;
        return type;
    }

    public String SetModel(String newmodel)
    {
        this.model = newmodel;
        return model;
    }

    public String SetMessage(String newmessage)
    {
        this.message = newmessage;
        return message;
    }

    public String SetMessageId(String newmessageid)
    {
        this.messageId = newmessageid;
        return messageId;
    }

    public String SetOp(String newop)
    {
        this.op = newop;
        return op;
    }

    private String urlencode(String str)
    {
        return HttpUtility.UrlEncode(str);
    }

    public String Send()
    {
        String Body = "";
        Body += "CustomerMSISDN=" + this.destination;
        Body += "&CustomerAccount=" + this.destination;
        Body += "&AgentCode=" + urlencode(this.username);
        Body += "&Action=SMS";
        Body += "&Narrative=" + urlencode(this.message.Trim());
        Body += "&ServiceId=17";
        Body += "&Amount=0";
        Body += "&MTI=0200";
        Body += "&ProcessingCode=320000";
       
       
        if ((this.source ) != "")
        {
            Body += "&source=" + urlencode(this.source);
        }

        if ((this.model) != "")
        {
            Body += "&model=" + urlencode(this.model);
        }

        if ((this.op) != "")
        {
            Body += "&Action=SMS" ;
        }

        //int ContentLength = Body.Length;

        String Host = "mobile.esolutions.co.zw";//"yomoneyservice.com";
       
        int ContentLength = Body.Length;
        //int ContentLength = json.Length;
        //String Header = "POST  /bulksms/bulksms?"+ Body +" HTTP/1.0\n" + "Host: " + Host + "\n";
        RetailKingEntities db = new RetailKingEntities();
        String  returnvalue = "";
        try
        {
            var pro = db.ProxySettings.FirstOrDefault();
           // https://mobile.esolutions.co.zw/bmg/api/single/[your-account-name-here]/targetMobileNumber/messageText
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("https://" + Host + "/bmg/api/single/" + urlencode(this.source) +"/"+ this.destination + "/"+ urlencode(this.message.Trim()));
           // HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("https://" + Host + "/api/Vend/Transaction");
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("263778100222:44148"));
            //live
            // String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("600024:a99L3v1N3@2017#"));
            httpWReq.Headers.Add("Authorization", "Basic " + encoded);
            httpWReq.Method = "Get";
            
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
            if(returnvalue.Length >= 4 && returnvalue.Substring(0,4) == "1701")
            {
                returnvalue = "Success";
            }else
            {
                
                var uu = new JavaScriptSerializer().Deserialize<TransactionResponse>(returnvalue);
                if (uu.Description != null)
                {
                    returnvalue = uu.Description;
                }
                else
                {
                    returnvalue = "Success";
                }
            }
        }
        catch (Exception )
        {
            //throw new Exception(ex.Message);
        }

        return returnvalue;//Header + " " + line;
    }

    public String SendDirect()
    {
        // esolutions
        String Body = "";
        Body += "CustomerMSISDN=" + this.destination;
        Body += "&CustomerAccount=" + this.destination;
        Body += "&AgentCode=" + urlencode(this.username);
        Body += "&Action=SMS";
        Body += "&Narrative=" + urlencode(this.message.Trim());
        Body += "&ServiceId=17";
        Body += "&Amount=0";
        Body += "&MTI=0200";
        Body += "&ProcessingCode=320000";


        if ((this.source) != "")
        {
            Body += "&source=" + urlencode(this.source);
        }

        if ((this.model) != "")
        {
            Body += "&model=" + urlencode(this.model);
        }

        if ((this.op) != "")
        {
            Body += "&Action=SMS";
        }

        //int ContentLength = Body.Length;

        String Host = "mobile.esolutions.co.zw";//"yomoneyservice.com";

        int ContentLength = Body.Length;
        //int ContentLength = json.Length;
        //String Header = "POST  /bulksms/bulksms?"+ Body +" HTTP/1.0\n" + "Host: " + Host + "\n";
        RetailKingEntities db = new RetailKingEntities();
        String returnvalue = "";
        try
        {
            var pro = db.ProxySettings.FirstOrDefault();
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("https://" + Host + "/bmg/api/single/" + urlencode(this.source) + "/" + this.destination + "/" +this.message.Trim());
            var request = WebRequest.Create("https://" + Host + "/bmg/api/single/" + urlencode(this.source) + "/" + this.destination + "/" + this.message.Trim());
            string authInfo = "FWAPI:yzg2MWf@1";
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

            //like this:
            request.Headers["Authorization"] = "Basic " + authInfo;
            var respons = request.GetResponse();

            if (returnvalue.Length >= 4 && returnvalue.Substring(0, 4) == "1701")
            {
                returnvalue = "Success";
            }
            else
            {

                var uu = new JavaScriptSerializer().Deserialize<TransactionResponse>(returnvalue);
                if (uu.Description != null)
                {
                    returnvalue = uu.Description;
                }
                else
                {
                    returnvalue = "Success";
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return returnvalue;//Header + " " + line;
    }

    public String SendDirectr()
    {
        //route SMS
        String Body = "";
        string returnvalue = "";
        Body += "destination=" + this.destination;
        Body += "&username=" + urlencode(this.username);
        Body += "&password=" + urlencode(this.password);
        Body += "&message=" + urlencode(this.message);
        Body += "&dlr=1";


        if ((this.source) != "")
        {
            Body += "&source=" + urlencode(this.source);
        }

        if ((this.model) != "")
        {
            Body += "&model=" + urlencode(this.model);
        }

        if ((this.op) != "")
        {
            Body += "&op=" + urlencode(this.op);
        }

        if ((this.type) != "")
        {
            Body += "&type=" + urlencode(this.type);
        }

        int ContentLength = Body.Length;

        String Host = "api.rmlconnect.net";

        String Header = "POST  /bulksms/bulksms?" + Body + " HTTP/1.0\n" + "Host: " + Host + "\n";

        //Get Response

        Socket mysocket = null;
        TcpClient tcpClient;
        StreamWriter streamWriter = null;
        StreamReader streamReader = null;
        NetworkStream networkStream = null;

        String line = "";
        try
        {
            WebRequest request = HttpWebRequest.Create("http://" + Host + "/bulksms/bulksms?" + Body);
            WebResponse respo = request.GetResponse();
            StreamReader sr = new StreamReader(respo.GetResponseStream());

            returnvalue = sr.ReadToEnd();
            if (returnvalue.Length >= 4 && returnvalue.Substring(0, 4) == "1701")
            {
                returnvalue = "Success";
            }
            /* tcpClient = new TcpClient();
            tcpClient.Connect(Host, 8080);
            
            mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mysocket.Connect(Host, 8080);
            
            networkStream = tcpClient.GetStream();
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);
            streamWriter.WriteLine(Header);
            streamWriter.Flush();

            line = streamReader.ReadToEnd();
            */
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return returnvalue; //Header + " " + line;
    }

    public String GetUser()
    {
        return this.username;
    }

    public String GetPass()
    {
        return this.password ;
    }

    public String GetNumber()
    {
        return this.destination;
    }

    public String GetMessage()
    {
        return this.message;
    }

    public String GetMessageId()
    {
        return this.messageId;
    }

    public String GetOwnNum()
    {
        return this.source;
    }

    public String GetOp()
    {
        return this.op;
    }

    public String GetType()
    {
        return this.type;
    }

    public String GetModel()
    {
        return this.model;
    }

    public static byte[] ReadFully(Stream stream)
    {
        byte[] buffer = new byte[32768];
        using (MemoryStream ms = new MemoryStream())
        {
            while (true)
            {
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return ms.ToArray();
                ms.Write(buffer, 0, read);
            }
        }
    }

    public string getImage(string url)
    {
        try

        {
            WebRequest request = WebRequest.Create(url);

            byte[] file = new byte[1024 * 1024];
            Stream stream = request.GetResponse().GetResponseStream();

            file = ReadFully(stream);

            return Convert.ToBase64String(file, Base64FormattingOptions.InsertLineBreaks);
        }

        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public string unicodeEncodeMessage()
    {
        try
        {
            Encoding encoding = new UnicodeEncoding(true, true, true);

            byte[] bytes = encoding.GetBytes(this.message);            

            string hexString = BitConverter.ToString(bytes);
            hexString = hexString.Replace("-", string.Empty);

            this.message = hexString;

            return this.message;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
