using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.DataAcess;


namespace RetailKing.Controllers
{   
    [Authorize]
    public class MenuController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Items/

        public ActionResult Index(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Inventory inv = new Inventory();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = db.Menus.ToList();
           
        
            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }
            var nn = (from emp in px.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                          select emp).ToList();

            int pages = px.Count / param.iDisplayLength;
            if (px.Count > 0 && pages == 0)
            {
                pages = 1;
            }
            else if (px.Count > 0 && px.Count % param.iDisplayLength > 0)
            {
                pages += 1;
            }
            int page = param.iDisplayStart;

            int start = (param.iDisplayStart - 1) * param.iDisplayLength;
            if (start == 0) start = 1;
            ViewData["listSize"] = param.iDisplayLength;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;
          
            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);
        }

        [HttpPost]
        public ActionResult Index(string category ,string ItemCode,string company,JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            
            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }
            
            
                var px =db.Menus.ToList();
                var gx = (from ee in px
                          where ItemCode.ToLower() == ItemCode.Trim().ToLower()
                          select ee).ToList();

                var nn = (from u in gx.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                          select u).ToList();

                int pages = gx.Count / param.iDisplayLength;
                if (gx.Count > 0 && pages == 0)
                {
                    pages = 1;
                }
                else if (gx.Count > 0 && gx.Count % param.iDisplayLength > 0)
                {
                    pages += 1;
                }
                int page = param.iDisplayStart;

                int start = (param.iDisplayStart - 1) * param.iDisplayLength;
                if (start == 0) start = 1;
                ViewData["listSize"] = param.iDisplayLength;
                ViewData["Pages"] = pages;
                ViewData["ThisPage"] = page;
                ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + nn.Count;
                Inventory inv = new Inventory();
               
                return PartialView(nn);
           
                
        }
        //
        // GET: /Items/Details/5
        #region management menu

        [HttpGet]
        public ActionResult Menus(string Id = "")
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                var current = User.Identity.Name;
                 char[] delimiter = new char[] { '~' };
                string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
                {
                    if (Id == "")
                    {
                        Id = parts[0];
                    }
                    ViewData["Name"] = parts[0];
                    ViewData["Role"] = parts[2];

                    var ins = db.Menus.Where(u => u.Type == null).ToList();
                    return View("Menus", "_AdminLayout", ins);
                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult ResourceMenus(string Id = "")
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                var current = User.Identity.Name;
                 char[] delimiter = new char[] { '~' };
                string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
                {
                    if (Id == "")
                    {
                        Id = parts[0];
                    }
                    ViewData["Name"] = parts[0];
                    ViewData["Role"] = parts[2];

                    var ins = db.Menus.Where(u => u.Type != null && u.Type.Trim() == "RESOURCE").ToList();
                    return View("ResourceMenus", "_AdminLayout", ins);
                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult AdminMenus(string Id = "")
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                var current = User.Identity.Name;
                 char[] delimiter = new char[] { '~' };
                string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
                {
                    if (Id == "")
                    {
                        Id = parts[0];
                    }
                    ViewData["Name"] = parts[0];
                    ViewData["Role"] = parts[2];

                    var ins = db.Menus.Where(u => u.Type != null && u.Type.Trim() == "ADMIN").FirstOrDefault();

                    return RedirectToAction("MenuItems", new { Id = ins.Name.Trim() });
                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult CreateMenus(string Type)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var current = User.Identity.Name;
             char[] delimiter = new char[] { '~' };
            string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
            {
                //var ip = getClientIP();
                ViewData["Name"] = parts[0];
                ViewData["Role"] = parts[2];
                Menu mm = new Menu();
                if (!string.IsNullOrEmpty(Type))
                {
                    mm.Type = Type;
                }
                return View("CreateMenus", "_AdminLayout", mm);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpGet]
        public ActionResult EditMenus(string Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var current = User.Identity.Name;
             char[] delimiter = new char[] { '~' };
            string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
            {
                //var ip = getClientIP();
                ViewData["Name"] = parts[0];
                ViewData["Role"] = parts[2];
                var pg = db.Menus.Where(u => u.Name.Trim() == Id.Trim()).FirstOrDefault();
                return View("EditMenus", "_AdminLayout", pg);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult CreateMenus(Menu page)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var current = User.Identity.Name;
             char[] delimiter = new char[] { '~' };
            string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
            {
                ViewData["Name"] = parts[0];
                ViewData["Role"] = parts[2];
                db.Menus.Add(page);
                db.SaveChanges();
                if (string.IsNullOrEmpty(page.Type))
                {
                    var pgs = db.Menus.Where(u => u.Type == null).ToList();
                    return PartialView("Menus",  pgs);
                }
                else
                {
                    var pgs = db.Menus.Where(u => u.Type != null && u.Type.Trim() == page.Type.Trim()).ToList();
                    return PartialView("Menus",  pgs);
                }


            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult MenuItems(string Id = "")
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                var current = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if ( parts[3] == "ADMINISTRATOR")
                {
                    ViewData["MName"] = "";
                    ViewData["Name"] = parts[1];
                    ViewData["Role"] = parts[3];
                    var mnuId = long.Parse(Id);
                    
                    ViewData["Id"] = mnuId;
                    var ins = db.MenuItems.Where(u => u.Id == mnuId).ToList();
                    return PartialView("MenuItems",  ins);
                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult CreateMenuItems(long Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var current = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
           if (parts[3] == "ADMINISTRATOR")
            {
                //var ip = getClientIP();
                ViewData["Name"] = parts[1];
                ViewData["Role"] = parts[3];
                var mnu = db.Menus.Find(Id);
                var itm = new MenuItem();
                itm.MenuId = mnu.Id;
                itm.Id = 0;
                ViewData["MName"] = mnu.Name.Trim();
                var syl = db.AccessOptions.ToList();
                List<SelectListItem> cc = new List<SelectListItem>();

                foreach (var item in syl)
                {

                    cc.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Name
                    });
                }
                ViewBag.section = cc;
                return PartialView("CreateMenuItems", itm);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpGet]
        public ActionResult EditMenuItems(long Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var current = User.Identity.Name;
             char[] delimiter = new char[] { '~' };
            string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
            {
                //var ip = getClientIP();
                ViewData["Name"] = parts[0];
                ViewData["Role"] = parts[2];
                var pg = db.Menus.Find(Id);
                var syl = db.AccessOptions.ToList();
                List<SelectListItem> cc = new List<SelectListItem>();

                foreach (var item in syl)
                {

                    cc.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Name
                    });
                }
                ViewBag.section = cc;
                return PartialView("EditMenuItems", pg);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult CreateMenuItems(MenuItem page)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var current = User.Identity.Name;
             char[] delimiter = new char[] { '~' };
            string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
            {
                ViewData["Name"] = parts[0];
                ViewData["Role"] = parts[2];
                ViewData["Id"] = page.MenuId;
                page.Id = 0;
                db.MenuItems.Add(page);
                db.SaveChanges();
                var pgs = db.MenuItems.Where(u => u.MenuId == page.MenuId).ToList();
                return PartialView("MenuItems", pgs);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult EditMenuItems(MenuItem page)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var current = User.Identity.Name;
             char[] delimiter = new char[] { '~' };
            string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
            {
                ViewData["Name"] = parts[0];
                ViewData["Role"] = parts[2];
                ViewData["Id"] = page.MenuId;
                db.Entry(page).State = EntityState.Modified;
                db.SaveChanges();
               
                var pgs = db.MenuItems.Where(u => u.MenuId == page.MenuId).ToList();
                return PartialView("MenuItems", pgs);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpGet]
        public ActionResult SubMenuItems(long Id = 0)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                var current = User.Identity.Name;
                 char[] delimiter = new char[] { '~' };
                string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
                {
                    ViewData["MName"] = "";
                    ViewData["Name"] = parts[0];
                    ViewData["Role"] = parts[2];
                    ViewData["Id"] = Id;
                    var ins = db.SubMenuItems.Where(u => u.MenuItemId == Id).ToList();
                    return PartialView("SubMenuItems", ins);
                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult CreateSubMenuItems(long Id)
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                var current = User.Identity.Name;
                 char[] delimiter = new char[] { '~' };
                string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
                {
                    //var ip = getClientIP();
                    ViewData["Name"] = parts[0];
                    ViewData["Role"] = parts[2];
                    var mnu = db.MenuItems.Find(Id);
                    var itm = new SubMenuItem();
                    itm.MenuItemId = mnu.Id;
                    itm.Id = 0;
                    ViewData["MName"] = mnu.Title;
                    return PartialView("CreateSubMenuItems",  itm);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpGet]
        public ActionResult EditSubMenuItems(long Id)
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                var current = User.Identity.Name;
                 char[] delimiter = new char[] { '~' };
                string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
                {
                    //var ip = getClientIP();
                    ViewData["Name"] = parts[0];
                    ViewData["Role"] = parts[2];
                    var pg = db.SubMenuItems.Find(Id);
                    return PartialView("EditSubMenuItems", pg);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult CreateSubMenuItems(SubMenuItem page)
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                var current = User.Identity.Name;
                 char[] delimiter = new char[] { '~' };
                string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
                {
                    ViewData["Name"] = parts[0];
                    ViewData["Role"] = parts[2];
                    ViewData["Id"] = page.MenuItemId;
                    page.Id = 0;
                    db.SubMenuItems.Add(page);
                    db.SaveChanges();
                    var mnuitm = db.MenuItems.Find(page.MenuItemId);
                    mnuitm.HasSubMenu = "YES";
                    db.Entry(mnuitm).State = EntityState.Modified;
                    db.SaveChanges();
                    
                    var pgs = db.SubMenuItems.Where(u => u.MenuItemId == page.MenuItemId).ToList();
                    return PartialView("SubMenuItems", pgs);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult EditSubMenuItems(SubMenuItem page)
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                var current = User.Identity.Name;
                 char[] delimiter = new char[] { '~' };
                string[] parts = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && (parts[2] == "SupaAdmin" || parts[3] == "ADMINISTRATOR"))
                {
                    ViewData["Name"] = parts[0];
                    ViewData["Role"] = parts[2];
                    ViewData["Id"] = page.MenuItemId;
                    db.Entry(page).State = EntityState.Modified;
                    db.SaveChanges();
                    var pgs = db.SubMenuItems.Where(u => u.MenuItemId == page.MenuItemId).ToList();
                    return PartialView("SubMenuItems", pgs);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
               // context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}