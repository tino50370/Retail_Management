using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;
using System.Net.Mail;
using System.IO;
using System.Security.Principal;
using RetailKing.Controllers;
using System.Timers;
using System.Threading.Tasks;
using System.Threading;


namespace RetailKing
{

    public class MailService
    {

         RetailKingEntities db = new RetailKingEntities();
         public string Directory { get; set; }
         public long ServiceId { get; set; }
         public string  Filter { get; set; }
         public string HasList { get; set; }  

         private Delegate _changeMethod;
 
         public Delegate ChangeMethod
         {
            get { return _changeMethod; }
            set { _changeMethod = value; }
         }

         FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

         public MailService(string directory, string filter,string HasList, long ServiceId,Delegate invokeMethod)
         {
             this._changeMethod = invokeMethod;
             this.Directory = directory;
             this.Filter = filter;
             this.ServiceId = ServiceId;
             this.HasList = HasList;
         }

         public void StartWatch()
         {
             fileSystemWatcher.Filter = this.Filter;
             fileSystemWatcher.Path = this.Directory;

             fileSystemWatcher.EnableRaisingEvents = true;
             fileSystemWatcher.Changed += new FileSystemEventHandler(fileSystemWatcher_Changed);
             fileSystemWatcher.Created += new FileSystemEventHandler(fileSystemWatcher_Changed);
                       
         }

         public void StopWatch()
         {
             fileSystemWatcher.Filter = this.Filter;
             fileSystemWatcher.Path = this.Directory;
             fileSystemWatcher.EnableRaisingEvents = false;

             fileSystemWatcher.Changed += new FileSystemEventHandler(fileSystemWatcher_Changed);
             fileSystemWatcher.Created += new FileSystemEventHandler(fileSystemWatcher_Changed);

         }

         void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
         {
             try
             {
                 if (_changeMethod != null)
                 {

                     var dd =DateTime.Now;
                     var pre = DateTime.Now.AddMinutes(-1);
                     var ww = db.Emails.Where(u => u.Receiver.Trim() == e.FullPath &&  u.Date >= pre ).ToList();
                     if (ww.Count == 0)
                     {
                         Email em = new Email();
                         em.Receiver = e.FullPath;
                         em.Date = dd;
                         db.Emails.Add(em);
                         db.SaveChanges();
                         Thread.Sleep(5000);
                         _changeMethod.DynamicInvoke(sender, e, this,em.Id);
                     } 
                 }
             }
             catch(Exception)
             {
                 MailSettingsController mset = new MailSettingsController();
                 mset.StartAllServices(); 
             }
             
         }

    }
}