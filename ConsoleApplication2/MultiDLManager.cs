using NBWare.CMS.Framework.Persistent;
using System;
using System.Data;
using System.Transactions;

namespace ConsoleApplication2
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

        public int multiTest()
        {
            int result = -2;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                   result = CMSoDataAccess.ExecuteNonQuery(CommandType.Text, @"INSERT INTO [dbo].[test2]
           ([a]
           ,[b]
           ,[c])
     VALUES
           (1
           ,'2'
           ,'3')");

                   result = MTVoDataAccess.ExecuteNonQuery(CommandType.Text, @"INSERT INTO [dbo].[IPGInfo]
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

                   result = MNGoDataAccess.ExecuteNonQuery(CommandType.Text, @"INSERT INTO [dbo].[SITE]
           ([SITECD]
           ,[SITENAME]
           ,[USEYN])
     VALUES
           ('1'
           ,'test'
           ,'Y')");
                   int.Parse("d");
                    scope.Complete();
                }
                catch (System.Exception ex)
                {
                    result = -2;
                }
                return result;
            }
        }

    }
}
