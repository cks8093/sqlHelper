using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace NBWare.CMS.Framework.SqlHelper
{
    public class DataAccessBase : IDisposable
    {
        #region DECLARATIONS
        private DbProviderFactory oFactory;
        private DbConnection oConnection;
        private DbTransaction oTransaction;
        private bool bTransaction;
        private bool bOutputParam = false;

        private ConnectionPoolType conn = new ConnectionPoolType();
        public ConnectionPoolType Conn
        {
            get
            {
                if (conn != null)
                    return conn;
                else
                    return new ConnectionPoolType();
            }
            set
            {
                conn = value;
            }
        }

        #region property

        #endregion

        #region Item
        private Queue<DynamicParameters> outItemQueue;
        private DynamicParameters outItem;
        private Queue<DataValue> inputItemQueue;
        #endregion

        #region SET PARAMETER
        #region SetParameter
        public void SetParameter(string name, object val, DbType dbtype = DbType.String)
        {
            inputItemQueue.Enqueue(new DataValue(name, val, ParameterDirection.Input, dbtype));
        }
        #endregion
        #endregion

        #region SetOutParameter
        public void SetOutParameter(string name, DbType dbtype = DbType.String)
        {
            inputItemQueue.Enqueue(new DataValue(name, null, ParameterDirection.Output, dbtype));
            bOutputParam = true;
        }

        public T GetOutParameter<T>(string paramName)
        {
            T result = default(T);

            if (outItemQueue.Count > 0)
            {
                outItem = this.outItemQueue.Dequeue();
            }

            if (outItem != null)
            {
                result = outItem.Get<T>(paramName);
            }

            return result;
        }
        #endregion

        #region TRANSACTION
        public void TransactionHandler(TransactionType veTransactionType)
        {
            switch (veTransactionType)
            {
                case TransactionType.Open:  //open a transaction
                    try
                    {
                        oTransaction = oConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                        bTransaction = true;
                    }
                    catch (InvalidOperationException oErr)
                    {
                        throw new Exception("@TransactionHandler - BeginTransaction" + oErr.Message);
                    }
                    break;

                case TransactionType.Commit:  //commit the transaction
                    if (null != oTransaction.Connection)
                    {
                        try
                        {
                            oTransaction.Commit();
                            bTransaction = false;
                        }
                        catch (InvalidOperationException oErr)
                        {
                            throw new Exception("@TransactionHandler - Commit" + oErr.Message);
                        }
                    }
                    break;

                case TransactionType.Rollback:  //rollback the transaction
                    try
                    {
                        if (bTransaction)
                        {
                            oTransaction.Rollback();
                        }
                        bTransaction = false;
                    }
                    catch (InvalidOperationException oErr)
                    {
                        throw new Exception("@TransactionHandler - Rollback" + oErr.Message);
                    }
                    break;
            }
        }

        public void MultiTransactionHandler(TransactionType veTransactionType)
        {
            switch (veTransactionType)
            {
                case TransactionType.Open:  //open a transaction
                    try
                    {
                        ConnectionFactory();
                        oTransaction = oConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                        bTransaction = true;
                    }
                    catch (InvalidOperationException oErr)
                    {
                        throw new Exception("@MultiTransactionHandler - " + oErr.Message);
                    }
                    break;

                case TransactionType.Commit:  //commit the transaction
                    if (null != oTransaction.Connection)
                    {
                        try
                        {
                            oTransaction.Commit();
                            bTransaction = false;
                            CloseConnectionFactory();
                        }
                        catch (InvalidOperationException oErr)
                        {
                            throw new Exception("@MultiTransactionHandler - " + oErr.Message);
                        }
                    }
                    break;

                case TransactionType.Rollback:  //rollback the transaction
                    try
                    {
                        if (bTransaction)
                        {
                            oTransaction.Rollback();
                        }
                        bTransaction = false;
                        CloseConnectionFactory();
                    }
                    catch (InvalidOperationException oErr)
                    {
                        throw new Exception("@MultiTransactionHandler - " + oErr.Message);
                    }
                    break;
            }
        }
        #endregion

        #endregion

        #region DataAccessBase
        public DataAccessBase()
        {
            oFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            outItemQueue = new Queue<DynamicParameters>();
            inputItemQueue = new Queue<DataValue>();
        }

        public DataAccessBase New(DBNameType dbName)
        {
            try
            {
                conn.DBName = dbName;
                conn.ProviderName = ProviderNameType.MSSQL;

                conn = new ConnectionPool().GetObjectFromPool(conn);

                return this;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            if (oTransaction != null)
            {
                oTransaction.Dispose();
                oTransaction = null;
            }

            if (null != oConnection)
            {
                oConnection.Dispose();
                oConnection = null;
            }
        } 
        #endregion

        #region ExecuteNonQuery
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, bool bTransaction = false)
        {
            int result = -2;
            if (String.IsNullOrEmpty(cmdText)) return result;

            try
            {
                ConnectionFactory();
                if (bTransaction) TransactionHandler(TransactionType.Open);

                DynamicParameters item = new DynamicParameters();
                DataValue datavalue;
                while (inputItemQueue.Count > 0)
                {
                    datavalue = inputItemQueue.Dequeue();
                    item.Add(datavalue.name, datavalue.data, datavalue.ParamType, datavalue.direction, datavalue.Length);
                }

                result = oConnection.Execute(sql: cmdText, commandType: cmdType, param: item, transaction: this.oTransaction);

                if (bTransaction) TransactionHandler(TransactionType.Commit);

                if (bOutputParam)
                {
                    outItemQueue.Enqueue(item);
                }
            }
            catch (Exception ex)
            {
                if (bTransaction) TransactionHandler(TransactionType.Rollback);
                throw ex;
            }
            finally
            {
                CloseConnectionFactory();
            }

            return result;
        }
        #endregion

        #region MultiExecutneNonQuery
        public int MultiExecutneNonQuery(CommandType cmdType, string cmdText)
        {
            int result = -2;
            if (String.IsNullOrEmpty(cmdText)) return result;

            try
            {
                DynamicParameters item = new DynamicParameters();
                DataValue datavalue;
                while (inputItemQueue.Count > 0)
                {
                    datavalue = inputItemQueue.Dequeue();
                    item.Add(datavalue.name, datavalue.data, datavalue.ParamType, datavalue.direction, datavalue.Length);
                }

                result = oConnection.Execute(sql: cmdText, commandType: cmdType, param: item, transaction: this.oTransaction);

                if (bOutputParam)
                {
                    outItemQueue.Enqueue(item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                
            }

            return result;
        }
        #endregion

        #region ExecuteScalar
        public T ExecuteScalar<T>(CommandType cmdType, string cmdText, bool bTransaction = false)
        {
            T result;

            try
            {
                ConnectionFactory();
                if (bTransaction) TransactionHandler(TransactionType.Open);

                DynamicParameters item = new DynamicParameters();
                DataValue datavalue;
                while (inputItemQueue.Count > 0)
                {
                    datavalue = inputItemQueue.Dequeue();
                    item.Add(datavalue.name, datavalue.data, datavalue.ParamType, datavalue.direction, datavalue.Length);
                }

                result = oConnection.ExecuteScalar<T>(sql: cmdText, commandType: cmdType, param: item, transaction: this.oTransaction);

                if (bTransaction) TransactionHandler(TransactionType.Commit);

                if (bOutputParam)
                {
                    outItemQueue.Enqueue(item);
                }
            }
            catch (Exception ex)
            {
                if (bTransaction) TransactionHandler(TransactionType.Rollback);
                throw ex;
            }
            finally
            {
                CloseConnectionFactory();
            }

            return result;
        }
        #endregion

        #region ExecuteDataTable
        public IEnumerable<T> ExecuteDataTable<T>(CommandType cmdType, string cmdText, bool bTransaction = false)
        {
            if (string.IsNullOrEmpty(cmdText))
            {
                return null;
            }

            IEnumerable<T> result;

            try
            {
                ConnectionFactory();
                if (bTransaction) TransactionHandler(TransactionType.Open);

                DynamicParameters item = new DynamicParameters();
                DataValue datavalue;
                while (inputItemQueue.Count > 0)
                {
                    datavalue = inputItemQueue.Dequeue();
                    item.Add(datavalue.name, datavalue.data, datavalue.ParamType, datavalue.direction, datavalue.Length);
                }

                result = oConnection.Query<T>(sql: cmdText, commandType: cmdType, param: item, transaction: this.oTransaction);

                if (bTransaction) TransactionHandler(TransactionType.Commit);

                if (bOutputParam)
                {
                    outItemQueue.Enqueue(item);
                }
            }
            catch (Exception ex)
            {
                if (bTransaction) TransactionHandler(TransactionType.Rollback);
                throw ex;
            }
            finally
            {
                CloseConnectionFactory();
            }

            return result;
        } 
        #endregion

        #region ExecuteDataset
        public Tuple<IEnumerable<T1>, IEnumerable<T2>> ExecuteDataset<T1, T2>(CommandType cmdType, string cmdText, bool bTransaction = false)
        {
            if (string.IsNullOrEmpty(cmdText))
            {
                return null;
            }

            IEnumerable<T1> t1 = null;
            IEnumerable<T2> t2 = null;

            try
            {
                ConnectionFactory();

                if (bTransaction) TransactionHandler(TransactionType.Open);

                DynamicParameters item = new DynamicParameters();
                DataValue datavalue;
                while (inputItemQueue.Count > 0)
                {
                    datavalue = inputItemQueue.Dequeue();
                    item.Add(datavalue.name, datavalue.data, datavalue.ParamType, datavalue.direction, datavalue.Length);
                }

                using (var querymultiple = oConnection.QueryMultiple(sql: cmdText, commandType: cmdType, param: item, transaction: oTransaction))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (!querymultiple.IsConsumed)
                        {
                            switch (i)
                            {
                                case 0: t1 = querymultiple.Read<T1>(); break;
                                case 1: t2 = querymultiple.Read<T2>(); break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }

                if (bTransaction) TransactionHandler(TransactionType.Commit);

                if (bOutputParam)
                {
                    outItemQueue.Enqueue(item);
                }
            }
            catch (Exception ex)
            {
                if (bTransaction) TransactionHandler(TransactionType.Rollback);
                throw ex;
            }
            finally
            {
                CloseConnectionFactory();
            }

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(t1, t2);
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> ExecuteDataset<T1, T2, T3>(CommandType cmdType, string cmdText, bool bTransaction = false)
        {
            if (string.IsNullOrEmpty(cmdText))
            {
                return null;
            }

            IEnumerable<T1> t1 = null;
            IEnumerable<T2> t2 = null;
            IEnumerable<T3> t3 = null;

            try
            {
                ConnectionFactory();
                if (bTransaction) TransactionHandler(TransactionType.Open);

                DynamicParameters item = new DynamicParameters();
                DataValue datavalue;
                while (inputItemQueue.Count > 0)
                {
                    datavalue = inputItemQueue.Dequeue();
                    item.Add(datavalue.name, datavalue.data, datavalue.ParamType, datavalue.direction, datavalue.Length);
                }

                using (var querymultiple = oConnection.QueryMultiple(sql: cmdText, commandType: cmdType, param: item, transaction: oTransaction))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (!querymultiple.IsConsumed)
                        {
                            switch (i)
                            {
                                case 0: t1 = querymultiple.Read<T1>(); break;
                                case 1: t2 = querymultiple.Read<T2>(); break;
                                case 2: t3 = querymultiple.Read<T3>(); break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (bTransaction) TransactionHandler(TransactionType.Commit);

                if (bOutputParam)
                {
                    outItemQueue.Enqueue(item);
                }
            }
            catch (Exception ex)
            {
                if (bTransaction) TransactionHandler(TransactionType.Rollback);
                throw ex;
            }
            finally
            {
                CloseConnectionFactory();
            }

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>(t1, t2, t3);
        } 
        #endregion

        #region CONNECTIONS
        public void ConnectionFactory()
        {
            if (String.IsNullOrEmpty(Conn.ConfigPath))
                throw new Exception("Connection String Is Null.");

            try
            {
                if (oConnection == null)
                    oConnection = oFactory.CreateConnection();

                if (oConnection.State != ConnectionState.Open)
                {
                    oConnection.ConnectionString = Conn.ConfigPath;
                    oConnection.Open();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CloseConnectionFactory()
        {
            try
            {
                if (oConnection != null && oConnection.State == ConnectionState.Open)
                {
                    oConnection.Close();
                }

                if (oTransaction != null)
                {
                    oTransaction.Dispose();
                    oTransaction = null;
                }

                bOutputParam = false;
            }
            catch (DbException oDbErr)
            {
                throw new Exception(oDbErr.Message);
            }
            catch (System.NullReferenceException oNullErr)
            {
                throw new Exception(oNullErr.Message);
            }
        }

        #endregion
    }
}
