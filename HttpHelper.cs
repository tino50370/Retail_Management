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
using System.Net;
using System.Collections.Specialized;
using System.Text;

/// <summary>
/// Summary description for HttpHelper
/// </summary>
/// <Author>Samer Abu Rabie</Author>
public  class HttpHelper
{
    /// <summary>
    /// This method prepares an Html form which holds all data in hidden field in the addetion to form submitting script.
    /// </summary>
    /// <param name="url">The destination Url to which the post and redirection will occur, the Url can be in the same App or ouside the App.</param>
    /// <param name="data">A collection of data that will be posted to the destination Url.</param>
    /// <returns>Returns a string representation of the Posting form.</returns>
    /// <Author>Samer Abu Rabie</Author>
    public static String PreparePOSTForm(string url, NameValueCollection data)
    {
        //Set a name for the form
        string formID = "PostForm";

        //Build the form using the specified data to be posted.
        StringBuilder strForm = new StringBuilder();
        strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\">");
        foreach (string key in data)
        {
            strForm.Append("<input type=\"hidden\" name=\"" + key + "\" value=\"" + data[key] + "\">");
        }
        strForm.Append("</form>");

        //Build the JavaScript which will do the Posting operation.
        StringBuilder strScript = new StringBuilder();
        strScript.Append("<script language='javascript'>");
        strScript.Append("var v" + formID + " = document." + formID + ";");
        strScript.Append("v" + formID + ".submit();");
        strScript.Append("</script>");

        //Return the form and the script concatenated. (The order is important, Form then JavaScript)
        return strForm.ToString() + strScript.ToString();
    }
    /// <summary>
    /// POST data and Redirect to the specified url using the specified page.
    /// </summary>
    /// <param name="page">The page which will be the referrer page.</param>
    /// <param name="destinationUrl">The destination Url to which the post and redirection is occuring.</param>
    /// <param name="data">The data should be posted.</param>
    /// <Author>Samer Abu Rabie</Author>
    public string RedirectAndPOST(Page page, string destinationUrl, NameValueCollection data)
    {
        //Prepare the Posting form
        string strForm = PreparePOSTForm(destinationUrl, data);

        //Add a literal control the specified page holding the Post Form, this is to submit the Posting form with the request.
        return strForm;
       // page.Controls.Add(new LiteralControl(strForm));
    }

    public static String DecodeFromUtf8(string utf8String)
    {
        // copy the string as UTF-8 bytes.
        byte[] utf8Bytes = new byte[utf8String.Length];
        for (int i = 0; i < utf8String.Length; ++i)
        {
            //Debug.Assert( 0 <= utf8String[i] && utf8String[i] <= 255, "the char must be in byte's range");
            utf8Bytes[i] = (byte)utf8String[i];
        }

        return Encoding.UTF8.GetString(utf8Bytes, 0, utf8Bytes.Length);
    }
}
