using NBWare.CMS.Framework.SqlHelper;
using System;
using System.Data;

namespace ConsoleApplication1
{
    public class MultiDLManager : IDisposable
    {
        private DataAccessBase CMSoDataAccess = new DataAccessBase().New(DBNameType.CMS);  // 연결할 DB 선택
        private DataAccessBase MTVoDataAccess = new DataAccessBase().New(DBNameType.MTV);  // 연결할 DB 선택
        private DataAccessBase MNGoDataAccess = new DataAccessBase().New(DBNameType.MNG);  // 연결할 DB 선택
        public void Dispose()
        {
            CMSoDataAccess.Dispose();
            MTVoDataAccess.Dispose();
            MNGoDataAccess.Dispose();
        }

        public int outputTest1()
        {
            CMSoDataAccess.SetOutParameter("@purchaseID", DbType.Int32);

            CMSoDataAccess.ExecuteNonQuery(CommandType.StoredProcedure, @"output_test");

            return CMSoDataAccess.GetOutParameter<int>("@purchaseID");
        }

        public string outputTest2()
        {
            CMSoDataAccess.SetOutParameter("@SID", DbType.String);

            CMSoDataAccess.ExecuteNonQuery(CommandType.StoredProcedure, @"output_test1");

            return CMSoDataAccess.GetOutParameter<string>("@SID"); 
        }

        public int multiTest()
        {
            int result = -2;

            CMSoDataAccess.MultiTransactionHandler(TransactionType.Open);
            MTVoDataAccess.MultiTransactionHandler(TransactionType.Open);
            MNGoDataAccess.MultiTransactionHandler(TransactionType.Open);

            try
            {
                result = CMSoDataAccess.MultiExecutneNonQuery(CommandType.Text, @"INSERT INTO [dbo].[test2]
           ([a]
           ,[b]
           ,[c])
     VALUES
           (1
           ,'2'
           ,'3')");

                result = MTVoDataAccess.MultiExecutneNonQuery(CommandType.Text, @"INSERT INTO [dbo].[IPGInfo]
           ([testYn]
           ,[serviceYn]
           ,[delYn]
           ,[regdate]
           ,[updateDate])
     VALUES
           ('Y'
           ,'N'
           ,'N'
           ,getdate()
           ,getdate())");

                result = MNGoDataAccess.MultiExecutneNonQuery(CommandType.Text, @"INSERT INTO [dbo].[SITE]
           ([SITECD]
           ,[SITENAME]
           ,[USEYN])
     VALUES
           ('1'
           ,'test'
           ,'Y')");

                //int.Parse("d");

                CMSoDataAccess.MultiTransactionHandler(TransactionType.Commit);
                MTVoDataAccess.MultiTransactionHandler(TransactionType.Commit);
                MNGoDataAccess.MultiTransactionHandler(TransactionType.Commit);
            }
            catch (System.Exception ex)
            {
                result = -2;

                CMSoDataAccess.MultiTransactionHandler(TransactionType.Rollback);
                MTVoDataAccess.MultiTransactionHandler(TransactionType.Rollback);
                MNGoDataAccess.MultiTransactionHandler(TransactionType.Rollback);
            }
            return result;

        }

    }
}
