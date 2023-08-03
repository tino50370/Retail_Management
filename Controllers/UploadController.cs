using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.DataAcess;
using RetailKing.Models;
using System.Reflection;
using crypto;
using System.IO;
using System.Collections.Specialized;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Data;

namespace RetailKing.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        RetailKingEntities db = new RetailKingEntities();
        [Authorize]
        public ActionResult File()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            var dd = (from e in np.GetAccountsByCode("3-01") where e.AccountCode.Length > 4 && e.CompanyId == 1 select e).ToList();
            List<SelectListItem> cc = new List<SelectListItem>();
            List<SelectListItem> sp = new List<SelectListItem>();
            foreach (var item in dd)
            {
                if (item.AccountName != null)
                {
                    cc.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountName.Trim()
                    });
                }
            }

            ViewBag.ServiceProvider = sp;
            ViewBag.Usertype = cc;
            ViewData["success"] = "";
            ViewData["cnt"] = "";
            if (Request.IsAjaxRequest()) return PartialView();
            return View("File", "_AdminLayout");
        }

        [Authorize]
        [HttpPost]
        public ActionResult File(Upload up, HttpPostedFileBase Image)
        {
            var Upfile = Image;

            NHibernateDataProvider np = new NHibernateDataProvider();
            var cnt = 0;
            if (Upfile != null)
            {
                var fileName = Upfile.FileName;
                var path = Path.Combine(HttpContext.Server.MapPath("~/Content//EVDText"), fileName);
                var Dpath = Path.Combine(HttpContext.Server.MapPath("~/Content//EVDText"), "Duplicates_" + fileName);
                Upfile.SaveAs(path);
                Thread.Sleep(2000);
                var line = "";
                if (System.IO.File.Exists(path))
                {
                    var file = new System.IO.StreamReader(path);
                    ST_encrypt en = new ST_encrypt();
                    var dup = new List<Voucher>();
                    while ((line = file.ReadLine()) != null)
                    {
                        char[] delimiters = new char[] { '|' };
                        string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 6 && up.Product == "BUDDIE EVD")
                        {
                            cnt += 1;
                            var voucha = new Voucher();
                            voucha.VoucherKey = en.encrypt(parts[0], "214ea9d5bda5277f6d1b3a3c58fd7034");
                            voucha.BatchNumber = parts[2];
                            voucha.SerialNumber = en.encrypt(parts[1], "214ea9d5bda5277f6d1b3a3c58fd7034");
                            voucha.Status = "A";
                            voucha.Supplier = up.Supplier;
                            voucha.Product = up.Product;
                            voucha.Manufacturer = up.Manufacturer;
                            voucha.ExpireryDate = parts[4];
                            voucha.Dated = DateTime.Now.ToString();
                            voucha.Denomination = Decimal.Parse(parts[3]);
                            voucha.VID = long.Parse(parts[5]);
                            var cc = db.Vouchers.Where(u => u.SerialNumber == voucha.SerialNumber && u.VoucherKey == voucha.SerialNumber).ToList();
                            if (cc.Count > 0)
                            {
                                voucha.SerialNumber = parts[1];
                                voucha.VoucherKey = parts[0].Substring(1, 6);
                                db.Vouchers.Add(voucha);

                                db.SaveChanges();
                               
                                //log(Dpath, "Duplicate" + up.Supplier + "voucher :" + parts[0] + "|" + parts[1] + "|" + parts[2] + "|" + parts[3] + "|" + parts[4] + "|" + parts[5]);
                            }
                            else
                            {
                                db.Entry(voucha).State = EntityState.Modified;
                                db.SaveChanges();
                               
                            }
                        }
                        else if (parts.Length == 6 && up.Product == "EASYCALL")
                        {
                            cnt += 1;
                            var voucha = new Voucher();
                            voucha.VoucherKey = en.encrypt(parts[0], "214ea9d5bda5277f6d1b3a3c58fd7034");
                            voucha.BatchNumber = parts[2];
                            voucha.SerialNumber = en.encrypt(parts[1], "214ea9d5bda5277f6d1b3a3c58fd7034");
                            voucha.Status = "A";
                            voucha.Supplier = up.Supplier;
                            voucha.Product = up.Product;
                            voucha.Manufacturer = up.Manufacturer;
                            voucha.ExpireryDate = parts[4];
                            voucha.Dated = DateTime.Now.ToString();
                            voucha.Denomination = Decimal.Parse(parts[3]);
                            voucha.VID = long.Parse(parts[5]);
                            var cc = db.Vouchers.Where(u => u.SerialNumber == voucha.SerialNumber && u.VoucherKey == voucha.SerialNumber).ToList();
                            if (cc.Count > 0)
                            {
                                db.Vouchers.Add(voucha);

                                db.SaveChanges();

                                //log(Dpath, "Duplicate" + up.Supplier + "voucher :" + parts[0] + "|" + parts[1] + "|" + parts[2] + "|" + parts[3] + "|" + parts[4] + "|" + parts[5]);
                            }
                            else
                            {
                                db.Entry(voucha).State = EntityState.Modified;
                                db.SaveChanges();
                               
                            }
                        }
                        else
                        {
                            if (cnt == 0)
                            {
                                ViewData["success"] = "Data for Buddie EVD is of wrong format";
                                var ddD = (from e in np.GetAccountsByCode("3-01") where e.AccountCode.Length > 4 && e.CompanyId == 1 select e).ToList();
                                List<SelectListItem> ccc = new List<SelectListItem>();
                                List<SelectListItem> spp = new List<SelectListItem>();
                                foreach (var item in ddD)
                                {
                                    if (item.AccountName  != null)
                                    {
                                        ccc.Add(new SelectListItem
                                        {
                                            Text = item.AccountName ,
                                            Value = item.AccountName.Trim()
                                        });

                                       
                                    }
                                }

                                ViewBag.ServiceProvider = spp;
                                ViewBag.Usertype = ccc;
                                ViewData["success"] = "";
                                ViewData["cnt"] = "";

                                return View("Index");
                            }
                        }
                    }
                    if (dup.Count > 0)
                    {
                        ViewData["success"] = (cnt - dup.Count) + " pins of " + cnt + " loaded successfully. " + dup.Count + " diplicates were not loaded";
                        ViewData["cnt"] = "duplicates";
                        up.duplicates = dup;
                    }
                    else
                    {

                        ViewData["success"] = "Success";
                        ViewData["cnt"] = cnt;
                    }
                    file.Close();
                }

            }
            else
            {
                ViewData["success"] = "Please select the upload file";
            }
            up.Product = "";
            up.Supplier = "";
            up.Manufacturer = "";

            var dd = (from e in np.GetAccountsByCode("3-01") where e.AccountCode.Length > 4 && e.CompanyId == 1 select e).ToList();
            List<SelectListItem> ccs = new List<SelectListItem>();
            List<SelectListItem> sp = new List<SelectListItem>();
            foreach (var item in dd)
            {
                if (item.AccountName  != null)
                {
                    ccs.Add(new SelectListItem
                    {
                        Text = item.AccountName ,
                        Value = item.AccountName.Trim()
                    });

                   
                }
            }

            ViewBag.ServiceProvider = sp;
            ViewBag.Usertype = ccs;
            if (Request.IsAjaxRequest()) return PartialView(up);
            return View("File", "_AdminLayout", up);
        }

        [Authorize]
        public ActionResult BulkSale()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
         
            var dd = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 && e.CompanyId == 1 select e).ToList();
            List<SelectListItem> cc = new List<SelectListItem>();
            List<SelectListItem> sp = new List<SelectListItem>();
            foreach (var item in dd)
            {
                if (item.AccountName  != null)
                {
                    cc.Add(new SelectListItem
                    {
                        Text = item.AccountName ,
                        Value = item.AccountName.Trim()
                    });

                    
                }
            }

            ViewBag.ServiceProvider = sp;
            ViewBag.Usertype = cc;
            ViewData["success"] = "";
            ViewData["cnt"] = "";
            return View();
        }

        


    }
}
