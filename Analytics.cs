using RetailKing.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RetailKing
{
    public class Analytics
    {
        RetailKingEntities db = new RetailKingEntities();
       public void logViews(string ProductId,bool isMobile)
        {
            string todate = DateTime.Now.ToString("ddMMyyyy");
            string thisyear = DateTime.Now.Year.ToString();
            string theid = "";
            string theyear = "";
            theid = todate + ProductId;
            theyear = thisyear + ProductId;
            var daysv = (from e in db.DailyStats
                         where (e.ProductId == theid)
                         select e).FirstOrDefault();
            if (daysv == null)
            {
                daysv = new DailyStat();
                if (isMobile)
                {
                    if (daysv.Mobile == null) daysv.Mobile = 0;
                    daysv.Mobile += 1;
                }
                else
                {
                    if (daysv.PC == null) daysv.PC = 0;
                    daysv.PC += 1;
                }
                daysv.ProductId = DateTime.Now.ToString("ddMMyyyy") + ProductId;
                daysv.Date = DateTime.Now.Date;
                daysv.Views  = 1;
                db.Entry(daysv).State = EntityState.Added;
                db.SaveChanges();
            }
            else
            {
                if (isMobile)
                {
                    if (daysv.Mobile == null) daysv.Mobile = 0;
                    daysv.Mobile += 1;
                }
                else
                {
                    if (daysv.PC == null) daysv.PC = 0;
                    daysv.PC += 1;
                }
                daysv.ProductId = DateTime.Now.ToString("ddMMyyyy") + ProductId;
                daysv.Date = DateTime.Now.Date;
                daysv.Views  += 1;
                db.Entry(daysv).State = EntityState.Modified;
                db.SaveChanges();
            }
            var monthly = (from e in db.MonthlyStats
                           where (e.ProductId == theyear)
                           select e).FirstOrDefault();

            int mm = DateTime.Now.Month;

            if (monthly == null)
            {
                monthly = new MonthlyStat();
                monthly.ProductId = DateTime.Now.Year.ToString() + ProductId;
                monthly.Year = DateTime.Now.Year;

                switch (mm)
                {
                    case 1:
                        monthly.JanV = 1;
                        break;
                    case 2:
                        monthly.FebV = 1;
                        break;
                    case 3:
                        monthly.MarV = 1;
                        break;
                    case 4:
                        monthly.AprV = 1;
                        break;
                    case 5:
                        monthly.MayV = 1;
                        break;
                    case 6:
                        monthly.JunV = 1;
                        break;
                    case 7:
                        monthly.JulV = 1;
                        break;
                    case 8:
                        monthly.AugV = 1;
                        break;
                    case 9:
                        monthly.SepV = 1;
                        break;
                    case 10:
                        monthly.OctV = 1;
                        break;
                    case 11:
                        monthly.NovV = 1;
                        break;
                    case 12:
                        monthly.DecV = 1;
                        break;
                }
                db.Entry(monthly).State = EntityState.Added;
                db.SaveChanges();
            }
            else
            {
                switch (mm)
                {
                    case 1:
                        if (monthly.JanV == null)
                        {
                            monthly.JanV = 1;
                        }
                        else
                        {
                            monthly.JanV += 1;
                        }
                        break;
                    case 2:
                        if (monthly.FebV == null)
                        {
                            monthly.FebV = 1;
                        }
                        else
                        {
                            monthly.FebV += 1;
                        }
                        break;
                    case 3:
                        if (monthly.MarV == null)
                        {
                            monthly.MarV = 1;
                        }
                        else
                        {
                            monthly.MarV += 1;
                        }
                        break;
                    case 4:
                        if (monthly.AprV == null)
                        {
                            monthly.AprV = 1;
                        }
                        else
                        {
                            monthly.AprV += 1;
                        }
                        break;
                    case 5:
                        if (monthly.MayV == null)
                        {
                            monthly.MayV = 1;
                        }
                        else
                        {
                            monthly.MayV += 1;
                        }
                        break;
                    case 6:
                        if (monthly.JunV == null)
                        {
                            monthly.JunV = 1;
                        }
                        else
                        {
                            monthly.JunV += 1;
                        }
                        break;
                    case 7:
                        if (monthly.JulV == null)
                        {
                            monthly.JulV = 1;
                        }
                        else
                        {
                            monthly.JulV += 1;
                        }
                        break;
                    case 8:
                        if (monthly.AugV == null)
                        {
                            monthly.AugV = 1;
                        }
                        else
                        {
                            monthly.AugV += 1;
                        }
                        break;
                    case 9:
                        if (monthly.SepV == null)
                        {
                            monthly.SepV = 1;
                        }
                        else
                        {
                            monthly.SepV += 1;
                        }
                        break;
                    case 10:
                        if (monthly.OctV == null)
                        {
                            monthly.OctV = 1;
                        }
                        else
                        {
                            monthly.OctV = monthly.OctV + 1;
                        }
                        break;
                    case 11:
                        if (monthly.NovV == null)
                        {
                            monthly.NovV = 1;
                        }
                        else
                        {
                            monthly.NovV += 1;
                        }
                        break;
                    case 12:
                        if (monthly.DecV == null)
                        {
                            monthly.DecV = 1;
                        }
                        else
                        {
                            monthly.DecV += 1;
                        }
                        break;
                }
                db.Entry(monthly).State = EntityState.Modified;
                db.SaveChanges();
            }

        }
       public void logClicks(string ProductId, bool isMobile)
       {
           string todate = DateTime.Now.ToString("ddMMyyyy");
           string thisyear = DateTime.Now.Year.ToString();
           string theid = "";
           string theyear = "";
           theid = todate + ProductId;
           theyear = thisyear + ProductId;
           var daysv = (from e in db.DailyStats
                        where (e.ProductId == theid)
                        select e).FirstOrDefault();
           if (daysv == null)
           {
               daysv = new DailyStat();
               if (isMobile)
               {
                   if (daysv.Mobile == null) daysv.Mobile = 0;
                   daysv.Mobile += 1;
               }
               else
               {
                   if (daysv.PC == null) daysv.PC = 0;
                   daysv.PC += 1;
               }
               daysv.ProductId = DateTime.Now.ToString("ddMMyyyy") + ProductId;
               daysv.Date = DateTime.Now.Date;
               daysv.Clicks = 1;
               db.Entry(daysv).State = EntityState.Added;
               db.SaveChanges();
           }
           else
           {
               if (isMobile)
               {
                   if (daysv.Mobile == null) daysv.Mobile = 0;
                   daysv.Mobile += 1;
               }
               else
               {
                   if (daysv.PC == null) daysv.PC = 0;
                   daysv.PC += 1;
               }
               daysv.ProductId = DateTime.Now.ToString("ddMMyyyy") + ProductId;
               daysv.Date = DateTime.Now.Date;
               daysv.Clicks += 1;
               db.Entry(daysv).State = EntityState.Modified;
               db.SaveChanges();
           }
           var monthly = (from e in db.MonthlyStats
                          where (e.ProductId == theyear)
                          select e).FirstOrDefault();

           int mm = DateTime.Now.Month;

           if (monthly == null)
           {
               monthly = new MonthlyStat();
               monthly.ProductId = DateTime.Now.Year.ToString() + ProductId;
               monthly.Year = DateTime.Now.Year;

               switch (mm)
               {
                   case 1:
                       monthly.JanC = 1;
                       break;
                   case 2:
                       monthly.FebC = 1;
                       break;
                   case 3:
                       monthly.MarC = 1;
                       break;
                   case 4:
                       monthly.AprC = 1;
                       break;
                   case 5:
                       monthly.MayC = 1;
                       break;
                   case 6:
                       monthly.JunC = 1;
                       break;
                   case 7:
                       monthly.JulC = 1;
                       break;
                   case 8:
                       monthly.AugC = 1;
                       break;
                   case 9:
                       monthly.SepC = 1;
                       break;
                   case 10:
                       monthly.OctC = 1;
                       break;
                   case 11:
                       monthly.NovC = 1;
                       break;
                   case 12:
                       monthly.DecC = 1;
                       break;
               }
               db.Entry(monthly).State = EntityState.Added;
               db.SaveChanges();
           }
           else
           {
               switch (mm)
               {
                   case 1:
                       if (monthly.JanC == null)
                       {
                           monthly.JanC = 1;
                       }
                       else
                       {
                           monthly.JanC += 1;
                       }
                       break;
                   case 2:
                       if (monthly.FebC == null)
                       {
                           monthly.FebC = 1;
                       }
                       else
                       {
                           monthly.FebC += 1;
                       }
                       break;
                   case 3:
                       if (monthly.MarC == null)
                       {
                           monthly.MarC = 1;
                       }
                       else
                       {
                           monthly.MarC += 1;
                       }
                       break;
                   case 4:
                       if (monthly.AprC == null)
                       {
                           monthly.AprC = 1;
                       }
                       else
                       {
                           monthly.AprC += 1;
                       }
                       break;
                   case 5:
                       if (monthly.MayC == null)
                       {
                           monthly.MayC = 1;
                       }
                       else
                       {
                           monthly.MayC += 1;
                       }
                       break;
                   case 6:
                       if (monthly.JunC == null)
                       {
                           monthly.JunC = 1;
                       }
                       else
                       {
                           monthly.JunC += 1;
                       }
                       break;
                   case 7:
                       if (monthly.JulC == null)
                       {
                           monthly.JulC = 1;
                       }
                       else
                       {
                           monthly.JulC += 1;
                       }
                       break;
                   case 8:
                       if (monthly.AugC == null)
                       {
                           monthly.AugC = 1;
                       }
                       else
                       {
                           monthly.AugC += 1;
                       }
                       break;
                   case 9:
                       if (monthly.SepC == null)
                       {
                           monthly.SepC = 1;
                       }
                       else
                       {
                           monthly.SepC += 1;
                       }
                       break;
                   case 10:
                       if (monthly.OctC == null)
                       {
                           monthly.OctC = 1;
                       }
                       else
                       {
                           monthly.OctC = monthly.OctV + 1;
                       }
                       break;
                   case 11:
                       if (monthly.NovC == null)
                       {
                           monthly.NovC = 1;
                       }
                       else
                       {
                           monthly.NovC += 1;
                       }
                       break;
                   case 12:
                       if (monthly.DecC == null)
                       {
                           monthly.DecC = 1;
                       }
                       else
                       {
                           monthly.DecC += 1;
                       }
                       break;
               }
               db.Entry(monthly).State = EntityState.Modified;
               db.SaveChanges();
           }
       }

    }
}