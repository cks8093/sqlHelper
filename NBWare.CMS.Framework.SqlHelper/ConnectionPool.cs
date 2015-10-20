using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBWare.CMS.Framework.SqlHelper
{
    public class ConnectionPool
    {
        private const int MAX_POOL = 20;

        private static LinkedList<ConnectionPoolType> objectPool = new LinkedList<ConnectionPoolType>();

        public ConnectionPoolType GetObjectFromPool(ConnectionPoolType conn)
        {
            lock (this)
            {
                foreach (ConnectionPoolType weak in objectPool)
                {
                    if (conn.DBName == weak.DBName)
                    {
                        return weak;
                    }
                }
            }

            return CreateConnStr(conn);
        }

        private ConnectionPoolType CreateConnStr(ConnectionPoolType conn)
        {
            try
            {
                string str_connection = string.Empty;
                ConnectionPoolType aa = new ConnectionPoolType();
                string sDbName = "ConnectionString";
                if (conn.DBName != DBNameType.None)
                {
                    sDbName = string.Format("{0}ConnectionString", conn.DBName);
                }

                if (!String.IsNullOrEmpty(conn.ConfigPath))
                    ConfigManager.ConfigPath = conn.ConfigPath;

                str_connection = ConfigManager.Default[sDbName];

                if (String.IsNullOrEmpty(str_connection))
                    throw new Exception("Connection String Is Null.");

                conn.ConfigPath = str_connection;

                if (conn.DBName != DBNameType.None) Add(conn);

                return conn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Add(ConnectionPoolType conn)
        {
            objectPool.AddFirst(conn);
            if (objectPool.Count >= MAX_POOL)
            {
                objectPool.RemoveLast();
            }
        }

        private void Remove(ConnectionPoolType conn)
        {
            objectPool.Remove(conn);
        }

    }
}
