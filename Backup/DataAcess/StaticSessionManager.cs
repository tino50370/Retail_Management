using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Cfg;

namespace RetailKing.DataAcess
{
    public class StaticSessionManager
    {
        public static readonly ISessionFactory SessionFactory;
        
        static StaticSessionManager()
        {
            try
            {
                Configuration cfg = new Configuration();
                
                if (SessionFactory != null)
                    throw new Exception("trying to init SessionFactory twice!");
             
                SessionFactory = cfg.Configure().BuildSessionFactory();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw new Exception("NHibernate initialization failed", ex);
            }
        }
        public static ISession OpenSession()
        {
          
            return SessionFactory.OpenSession();
        }
    }
}
