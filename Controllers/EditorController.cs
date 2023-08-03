using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace RetailKing.Controllers
{
    public class EditorController : ApiController
    {
        [Route("api/Editor/ConvertImageToBase64")]
        [HttpPost]
        public HttpResponseMessage ConvertImageToBase64()
        {
            string base64String = "";
            HttpResponseMessage response = new HttpResponseMessage();

            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                // Get the uploaded image from the Files collection
                var httpPostedFile = HttpContext.Current.Request.Files["file"];

                if (httpPostedFile != null)
                {
                    // Validate the uploaded image(optional)

                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(httpPostedFile.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(httpPostedFile.ContentLength);
                        base64String = System.Convert.ToBase64String(fileData, 0, fileData.Length);
                    }
                }
            }

            response.Content = new StringContent(base64String);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}
