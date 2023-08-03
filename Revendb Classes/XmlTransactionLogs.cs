using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.IO;
using RetailKing.Models;
using System.Web.Helpers;
using System.Web.Hosting;
using Raven.Client;
using RetailKing.Controllers;
using Raven.Abstractions.Data;
using Raven.Json.Linq;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;
using crypto;
using System.Net;
using RetailKing.DataAcess;
using System.Diagnostics;


namespace RetailKing.RavendbClasses
{
    public class XmlTransactionLogs : RavenController
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();

        #region Transactions
        public TransactionLog AddLog(Tranlog log)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            log.DateCreated = DateTime.Now;
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            #region Sourcelog
            var gx = RavenSession.Load<TransactionLog>("logs/" + log.SourceAccountNumber + "_" + month);
            try
            {
                if (gx != null)
                {
                    var cnt = gx.logs.Count();
                    long id = cnt + 1;
                    log.Id = id.ToString();
                    gx.logs.Insert(0, log);
                    RavenSession.Store(gx);
                }
                else
                {
                    List<Tranlog> tg = new List<Tranlog>();
                    DocumentStore.Conventions.RegisterIdConvention<TransactionLog>((dbname, commands, post) => "logs/" + log.SourceAccountNumber + "_" + month);
                    gx = new TransactionLog();
                    gx.Id = "logs/" + log.SourceAccountNumber + "_" + month;
                    log.Id = "1";
                    tg.Add(log);
                    gx.logs = tg;
                    RavenSession.Store(gx);
                }
            }
            catch
            {}

            #endregion

            #region Parentlog
            try
            { 
                if (!string.IsNullOrEmpty(log.ParentId))
                {
                    var px = RavenSession.Load<TransactionLog>("logs/" + log.ParentId + "_" + month);

                    if (px != null)
                    {
                        var cnt = px.logs.Count();
                        long id = cnt + 1;
                        log.Id = id.ToString();
                        px.logs.Insert(0, log);
                        RavenSession.Store(px);
                    }
                    else
                    {
                        List<Tranlog> tg = new List<Tranlog>();
                        DocumentStore.Conventions.RegisterIdConvention<TransactionLog>((dbname, commands, post) => "logs/" + log.ParentId + "_" + month);
                        px = new TransactionLog();
                        px.Id = "logs/" + log.ParentId + "_" + month;
                        log.Id = "1";
                        tg.Add(log);
                        px.logs = tg;
                        RavenSession.Store(px);
                    }
                }
            }
            catch
            { }
            #endregion

            #region Customerlog
            try { 
            if (!string.IsNullOrEmpty(log.DestinationAccountNumber) && log.ServiceId != "SMS MESSAGE")
            {
                var dx = RavenSession.Load<TransactionLog>("logs/" + log.DestinationAccountNumber + "_" + year);

                if (dx != null)
                {
                    var cnt = dx.logs.Count();
                    long id = cnt + 1;
                    log.Id = id.ToString();
                    dx.logs.Insert(0, log);
                    RavenSession.Store(dx);
                }
                else
                {
                    List<Tranlog> tg = new List<Tranlog>();
                    DocumentStore.Conventions.RegisterIdConvention<TransactionLog>((dbname, commands, post) => "logs/" + log.DestinationAccountNumber + "_" + year);
                    dx = new TransactionLog();
                    dx.Id = "logs/" + log.DestinationAccountNumber + "_" + year;
                    log.Id = "1";
                    tg.Add(log);
                    dx.logs = tg;
                    RavenSession.Store(dx);
                }
                    
                }
            }
            catch
            { }
            #endregion
            RavenSession.SaveChanges();
            RavenSession.Dispose();
            return gx;
        }

        public TransactionLog GetLogs(string SupplierId, int month)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            
            var gx = RavenSession.Load<TransactionLog>("logs/" + SupplierId + "_" + month);
            RavenSession.Dispose();
            return gx;
        }

        public TransactionLog GetCustomerLogs(string CustomerId, int year)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();

            var gx = RavenSession.Load<TransactionLog>("logs/" + CustomerId + "_" + year);
            RavenSession.Dispose();
            return gx;
        }

        public List<Tranlog> GetCustomerLogsByDate(string CustomerId, int year, DateTime date )
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();

            var gx = RavenSession.Load<TransactionLog>("logs/" + CustomerId + "_" + year);
            var px = gx.logs.Where(u => u.DateCreated.Date == date).ToList();
            RavenSession.Dispose();
            return px;
        }

        public Tranlog GetLogsById(string SupplierId, string Id, int month)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
           
            var gx = RavenSession.Load<TransactionLog>("logs/" + SupplierId + "_" + month);
            var px = gx.logs.Where(u => u.Id == Id).FirstOrDefault();
            RavenSession.Dispose();
            return px;
        }

        public string CheckLogReceipts(string SupplierId,string TerminalId, string receipt, int month)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
           // NHibernateDataProvider np = new NHibernateDataProvider();
           
            var gx = RavenSession.Load<TransactionLog>("logs/" + SupplierId + "_" + month);
            if (gx != null)
            {
                var dd = DateTime.Now.Date;
                var rr = gx.logs.Where(u => u.TerminalId == TerminalId && u.ReceiptNumber == receipt);
                if (rr.Count() > 0)
                {
                    return "invalid";
                }
                else
                {
                    return "valid";
                }
            }
            else
            {
                return "valid";
            }
        }
        
        public MultipleLogs SyncBranchMaster(string ParentId,  int month, int year)
        {
            Stopwatch tim = new Stopwatch();
            tim.Start();
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            RetailKingEntities db = new RetailKingEntities();
            var co = db.Suppliers.Where(u => u.ParentId == ParentId).ToList();
            var cnt = 0;
            var bcnt = 0;
            var ogsize = 0;
            var finalsize = 0;
            foreach (var itm in co)
            {
                cnt += 1;
                RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
                var px = RavenSession.Load<TransactionLog>("logs/" + ParentId + "_" + month);
                var bx = RavenSession.Load<TransactionLog>("logs/" + itm.AccountCode.Trim() + "_" + month);
                if (cnt == 1 )
                {
                    ogsize = px.logs.Count();
                }
                if (bx != null)
                {
                    bcnt += 1;
                    var ph = px.logs.Where(u => u.BranchName.Trim() == itm.Branch.Trim()).ToList();
                    var bh = bx.logs.ToList();

                    var hashParent = new HashSet<Tranlog>(ph);
                    var hashBranch = new HashSet<Tranlog>(bh);
                    //var rh = MergeShuffle<Tranlog>(ph, bh);
                    var dict = ph.ToDictionary(p => p.Miscellaneous);
                    foreach (var person in bh)
                    {
                        dict[person.Miscellaneous] = person;
                    }
                    var merged = dict.Values.ToList();
                   
                    bx.logs = merged.OrderByDescending(u => u.DateCreated).ToList();
                    var plogs = px.logs.Where(u => u.BranchName.Trim() != itm.Branch.Trim()).ToList();
                    plogs.AddRange(merged);
                    px.logs = plogs;
                    RavenSession.Store(bx);
                    RavenSession.Store(px);
                    RavenSession.SaveChanges();
                }
                RavenSession.Dispose();
            }
            var pfx = RavenSession.Load<TransactionLog>("logs/" + ParentId + "_" + month);
            finalsize = pfx.logs.Count();
            var pflogs = pfx.logs.OrderByDescending(u => u.DateCreated).ToList();
            pfx.logs = pflogs;
            RavenSession.Store(pfx);
            RavenSession.SaveChanges();
            tim.Stop();
            MultipleLogs ml = new Models.MultipleLogs();
            ml.OgSize  = ogsize;
            ml.FinalSize  = finalsize;
            ml.branches  = co.Count();
            ml.brancheDone  = bcnt;
            ml.duration = tim.Elapsed.TotalSeconds;
            RavenSession.Dispose();
            return ml;
        }

        static IEnumerable<T> MergeShuffle<T>(IEnumerable<T> lista, IEnumerable<T> listb)
        {
            var first = lista.GetEnumerator();
            var second = listb.GetEnumerator();

            var rand = new Random();
            bool exhaustedA = false;
            bool exhaustedB = false;
            while (!(exhaustedA && exhaustedB))
            {
                bool found = false;
                if (!exhaustedB && (exhaustedA || rand.Next(0, 2) == 0))
                {
                    exhaustedB = !(found = second.MoveNext());
                    if (found)
                        yield return second.Current;
                }
                if (!found && !exhaustedA)
                {
                    exhaustedA = !(found = first.MoveNext());
                    if (found)
                        yield return first.Current;
                }
            }
        }
        #endregion

        #region Transaction Stats
        public void AddStats(Stats log)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            int month = DateTime.Now.Month;
            log.Date = DateTime.Now.Date;
            string id = DateTime.Now.Day.ToString() + "_" + log.ServiceId;
            log.Id = id;
            #region main Stats
            var mx = RavenSession.Load<TransactionStats>("Stats/" + "5-0001-0000000" + "_" + month);
           
            if (mx != null)
            {
                var ts = mx.stats.Where(u => u.Id == log.Id).FirstOrDefault();
                if (ts == null)
                {
                   
                    mx.stats.Insert(0, log);
                    RavenSession.Store(mx);
                }
                else
                {
                    ts.Successful += log.Successful;
                    ts.Failed += log.Failed;
                    //mx.stats.Insert(0, log);
                    RavenSession.Store(mx);
                }
            }
            else
            {
                List<Stats> tg = new List<Stats>();
                DocumentStore.Conventions.RegisterIdConvention<TransactionStats>
                    ((dbname, commands, post) => "Stats/" + "5-0001-0000000" + "_" + month);
                mx = new TransactionStats();
                mx.Id = "Stats/" + "5-0001-0000000" + "_" + month;
                tg.Add(log);
                mx.stats = tg;
                RavenSession.Store(mx);
            }
            #endregion

            //log.ParentId
     
            #region Supplier Stats
            var gx = RavenSession.Load<TransactionStats>("Stats/" + log.SupplierId + "_" + month);

            if (gx != null)
            {
                var ts = gx.stats.Where(u => u.Id == log.Id).FirstOrDefault();
                if (ts == null)
                {
                    gx.stats.Insert(0, log);
                    RavenSession.Store(gx);
                }
                else
                {
                    ts.Successful += log.Successful;
                    ts.Failed += log.Failed;
                   // gx.stats.Insert(0, log);
                    RavenSession.Store(gx);
                }
                
            }
            else
            {
                List<Stats> tg = new List<Stats>();
                DocumentStore.Conventions.RegisterIdConvention<TransactionStats>
                    ((dbname, commands, post) => "Stats/" + log.SupplierId + "_" + month);
                gx = new TransactionStats();
                gx.Id = "Stats/" + log.SupplierId + "_" + month;
                tg.Add(log);
                gx.stats = tg;
                RavenSession.Store(gx);
            }
            #endregion

            #region Branch Stats
            if (!string.IsNullOrEmpty(log.BranchId))
            {
                var bx = RavenSession.Load<TransactionStats>("Stats/" + log.BranchId + "_" + month);
              
                if (bx != null)
                {
                    var ts = bx.stats.Where(u => u.Id == log.Id).FirstOrDefault();
                    if (ts == null)
                    {
                        bx.stats.Insert(0, log);
                        RavenSession.Store(bx);
                    }
                    else
                    {
                        ts.Successful += log.Successful;
                       
                        ts.Failed += log.Failed;
                       // bx.stats.Insert(0, log);
                        RavenSession.Store(bx);
                    }
                }
                else
                {
                    List<Stats> tg = new List<Stats>();
                    DocumentStore.Conventions.RegisterIdConvention<TransactionStats>
                        ((dbname, commands, post) => "Stats/" + log.BranchId + "_" + month);
                    bx = new TransactionStats();
                    bx.Id = "Stats/" + log.BranchId + "_" + month;
                    tg.Add(log);
                    bx.stats = tg;
                    RavenSession.Store(bx);
                }
            }
            #endregion
            RavenSession.SaveChanges();
            RavenSession.Dispose();
            //return gx;
        }

        public TransactionStats GetStats(string SupplierId)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<TransactionStats>("Stats/" + SupplierId);
            return gx;
        }

        public Stats GetStatsById(string SupplierId, string Id)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<TransactionStats>("Stats/" + SupplierId);
            var px = gx.stats.Where(u => u.Id == Id).FirstOrDefault();
            return px;
        }

        public string CheckStatsReceipts(string SupplierId, string username, string receipt)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<TransactionStats>("Stats/" + SupplierId);
            var dd = DateTime.Now.Date;
            var rr = gx.stats.Where(u => u.Date.Date == dd);
            if (rr.Count() > 0)
            {
                return "invalid";
            }
            else
            {
                return "valid";
            }

        }
        #endregion

        #region Rewards
        public TransactionLog AddReward(Tranlog log)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            log.DateCreated = DateTime.Now;
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            #region Sourcelog
            var gx = RavenSession.Load<TransactionLog>("RewardsLog/" + log.SourceAccountNumber );

            if (gx != null)
            {
                var cnt = gx.logs.Count();
                long id = cnt + 1;
                log.Id = id.ToString();
                gx.logs.Insert(0, log);
                RavenSession.Store(gx);
            }
            else
            {
                List<Tranlog> tg = new List<Tranlog>();
                DocumentStore.Conventions.RegisterIdConvention<TransactionLog>((dbname, commands, post) => "RewardsLog/" + log.SourceAccountNumber );
                gx = new TransactionLog();
                gx.Id = "RewardsLog/" + log.SourceAccountNumber ;
                log.Id = "1";
                tg.Add(log);
                gx.logs = tg;
                RavenSession.Store(gx);
            }
            #endregion

            #region Parentlog
            if (!string.IsNullOrEmpty(log.ParentId))
            {
                var px = RavenSession.Load<TransactionLog>("RewardsLog/" + log.ParentId );

                if (px != null)
                {
                    var cnt = px.logs.Count();
                    long id = cnt + 1;
                    log.Id = id.ToString();
                    px.logs.Insert(0, log);
                    RavenSession.Store(px);
                }
                else
                {
                    List<Tranlog> tg = new List<Tranlog>();
                    DocumentStore.Conventions.RegisterIdConvention<TransactionLog>((dbname, commands, post) => "RewardsLog/" + log.ParentId );
                    px = new TransactionLog();
                    px.Id = "RewardsLog/" + log.ParentId ;
                    log.Id = "1";
                    tg.Add(log);
                    px.logs = tg;
                    RavenSession.Store(px);
                }
            }
            #endregion

            #region Customerlog
            if (!string.IsNullOrEmpty(log.DestinationAccountNumber) && log.ServiceId != "SMS MESSAGE")
            {
                var dx = RavenSession.Load<TransactionLog>("logs/" + log.DestinationAccountNumber + "_" + year);

                if (dx != null)
                {
                    var cnt = dx.logs.Count();
                    long id = cnt + 1;
                    log.Id = id.ToString();
                    dx.logs.Insert(0, log);
                    RavenSession.Store(dx);
                }
                else
                {
                    List<Tranlog> tg = new List<Tranlog>();
                    DocumentStore.Conventions.RegisterIdConvention<TransactionLog>((dbname, commands, post) => "logs/" + log.DestinationAccountNumber + "_" + year);
                    dx = new TransactionLog();
                    dx.Id = "logs/" + log.DestinationAccountNumber + "_" + year;
                    log.Id = "1";
                    tg.Add(log);
                    dx.logs = tg;
                    RavenSession.Store(dx);
                }
            }
            #endregion

            RavenSession.SaveChanges();
            RavenSession.Dispose();
            return gx;
        }

        public TransactionLog GetRewards(string SupplierId)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<TransactionLog>("RewardsLog/" + SupplierId);
            return gx;
        }

        public Tranlog GetRewardById(string SupplierId, string Id)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<TransactionLog>("RewardsLog/" + SupplierId);
            var px = gx.logs.Where(u => u.Id == Id).FirstOrDefault();
            return px;
        }
        #endregion
    }
}
