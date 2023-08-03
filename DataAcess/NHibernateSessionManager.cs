using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Cfg;
using NHibernate;
using NHibernate.Criterion;
using log4net;
using Configuration = NHibernate.Cfg.Configuration;
using NHibernate.Search.Event;

namespace RetailKing.DataAcess
{
    public class NHibernateSessionManager
    {

        private ISessionFactory _sessionFactory;
        //Add NHibernate.Search listeners

        /// <summary>
        /// Initializes a new instance of the NHibernateSessionManager class.
        /// </summary>
        public NHibernateSessionManager()
        {
           
           /* _sessionFactory = GetSessionFactory();*/
        }

        public ISession GetSession()
        {
            return _sessionFactory.OpenSession();
        }

        private ISessionFactory GetSessionFactory()
        {
           /* Configuration.SetListener(NHibernate.Event.ListenerType.PostUpdate, new FullTextIndexEventListener());
            Configuration.SetListener(NHibernate.Event.ListenerType.PostInsert, new FullTextIndexEventListener());
            Configuration.SetListener(NHibernate.Event.ListenerType.PostDelete, new FullTextIndexEventListener());
            */
            
            return (new Configuration()).Configure().BuildSessionFactory();

        }

    }
}
