using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using Owin;
using RetailKing.Models;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.Routing;

using System.Web.Hosting;
using System.Web.Razor;

using System.Web.SessionState;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using crypto;
using RetailKing.DataAcess;
using RetailKing.RavendbClasses;
using System.Data;

namespace RetailKing.ChatServer
{
    
    public class ChatHub : Hub  
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        XmlSupplier sup = new XmlSupplier();
        XmlCustomers cus = new XmlCustomers();
        #region Data Members

       // static List<ChatUsers> ConnectedUsers = new List<ChatUsers>();
        static List<Message> CurrentMessage = new List<Message>();

        #endregion

        #region Mainhub
        public void Connect(string UserId )
        {
            if (UserId != "")
            {
                var id = Context.ConnectionId;
                NHibernateDataProvider np = new NHibernateDataProvider();
                var connection = db.logins.Where(u => u.username == UserId).FirstOrDefault();
                if(connection != null)
                {
                    string Username = connection.username.Trim();
                    connection.DateLastAccess = DateTime.Now;
                    connection.ConnectionId = id;
                    db.Entry(connection).State = EntityState.Modified;
                    db.SaveChanges();
                    Clients.Caller.onConnected(UserId.Trim(), Username, CurrentMessage);
                }
                
            }
        }

        public void ConnectPrinter(string UserId, string Location, string Postype, string IP)
        {
            if (UserId != "")
            {
                var id = Context.ConnectionId;
                NHibernateDataProvider np = new NHibernateDataProvider();
                var connection = new PosPrinter();
                var connections = db.PosPrinters.Where(u => u.Type  == Postype).ToList();
                if(connections.Count > 1)
                {
                    connection = connections.Where(u => u.Name == UserId).FirstOrDefault();
                }
                {
                    connection = connections.FirstOrDefault();
                }
                if (connection != null)
                {
                    //string Username = connection.username.Trim();
                    connection.DateLastAccess = DateTime.Now;
                    connection.ConnectionId = id;
                    connection.IpAddress = IP;
                    db.Entry(connection).State = EntityState.Modified;
                    db.SaveChanges();
                   
                }
                else
                {
                    connection = new Models.PosPrinter();
                    connection.DateLastAccess = DateTime.Now;
                    connection.ConnectionId = id;
                    connection.IpAddress = IP;
                    connection.Name = UserId;
                    connection.Location = Location;
                    connection.Type = Postype;
                    db.PosPrinters.Add(connection);
                    db.SaveChanges();
                }

            }
        }

        public void SendPrintMessage(string views, string company)
        {
              NHibernateDataProvider np = new NHibernateDataProvider();
                var toUser = db.logins.Where(u => u.UsesDispatch == true && u.Location == company).FirstOrDefault();
                Clients.Client(toUser.ConnectionId.Trim()).sendPrintMessage(views);
               

        }

        public void JoinGroup(string GroupId)
        {
            
            if (GroupId != "")
            {
                this.Groups.Add(Context.ConnectionId, GroupId);
            }
            
        }

        public void ExitGroup(string GroupId)
        {
            if (GroupId != "")
            {
                this.Groups.Remove(Context.ConnectionId, GroupId);
            }
        }

        public static string RenderPartialView(string controllerName, string partialView, object model)
        {
           //var context = new HttpContextWrapper(System.Web.HttpContext.Current) as HttpContextBase;
            var contxt = MockHelper.FakeHttpContext();
            var context = new HttpContextWrapper(contxt) as HttpContextBase;
            var routes = new System.Web.Routing.RouteData();
            routes.Values.Add("controller", controllerName);

            var requestContext = new RequestContext(context, routes);

            string requiredString = requestContext.RouteData.GetRequiredString("controller");
            var controllerFactory = ControllerBuilder.Current.GetControllerFactory();
            var controller = controllerFactory.CreateController(requestContext, requiredString) as ControllerBase;

            controller.ControllerContext = new ControllerContext(context, routes, controller);

            var ViewData = new ViewDataDictionary();

            var TempData = new TempDataDictionary();

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, partialView);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, ViewData, TempData, sw);

                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

      /*  public override System.Threading.Tasks.Task OnDisconnected()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = cus.GetCustomerProfileByConnectionId(Context.ConnectionId);
            if (item != null)
            {
                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.Name.Trim(),item.Id);

            }
           return base.OnDisconnected();
        }*/

        #endregion

    
    }

    public class FakeController : Controller
    {
    }

    public class MockHelper
    {
        public static HttpContext FakeHttpContext()
        {
            var httpRequest = new HttpRequest(string.Empty, "http://novomatic/", string.Empty);
            var stringWriter = new StringWriter();
            var httpResponce = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponce);

            var sessionContainer = new HttpSessionStateContainer(
                "id",
                new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(),
                10,
                true,
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc,
                false);

            httpContext.Items["AspSession"] =
                typeof(HttpSessionState).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    CallingConventions.Standard,
                    new[] { typeof(HttpSessionStateContainer) },
                    null).Invoke(new object[] { sessionContainer });

            return httpContext;
        }
    }

}