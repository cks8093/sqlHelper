using NBWare.CMS.Framework.SqlHelper;
using System;
using System.Collections.Generic;
using System.Data;

namespace ConsoleApplication1
{
    public class VodDLManager : IDisposable
    {
        private DataAccessBase oDataAccess;  // 연결할 DB 선택

        public VodDLManager()
        {
            oDataAccess = new DataAccessBase().New(DBNameType.CMS);  // 연결할 DB 선택
        }

        public VodDLManager(DBNameType dbnametype)
        {
            oDataAccess = new DataAccessBase().New(dbnametype);  // 연결할 DB 선택
        }

        public void Dispose()
        {
            oDataAccess.Dispose();
        }

        public Tuple<IEnumerable<User>, IEnumerable<User_test>> CtgrDetail(string UserID)
        {
            oDataAccess.SetParameter("@UserID", UserID);
            return oDataAccess.ExecuteDataset<User, User_test>(CommandType.StoredProcedure, @"GetUserById");
        }

        public IEnumerable<object> CMS1()
        {
            try
            {
                oDataAccess.SetParameter("@startno", 1);
                oDataAccess.SetParameter("@endno", 100);
                oDataAccess.SetParameter("@sDate", "2001-01-01");
                oDataAccess.SetParameter("@eDate", "2014-08-13 23:59:59");
                return oDataAccess.ExecuteDataTable<object>(CommandType.StoredProcedure, @"usp_ContentsVOD_List");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<test> CMS2()
        {
            return oDataAccess.ExecuteDataTable<test>(CommandType.Text, @"select null as purtype, 'test' as purtype1, 11 as price");
        }

        public IEnumerable<object> MNG1()
        {
            oDataAccess.SetParameter("@UserID", "admin");
            oDataAccess.SetParameter("@PARENTID", "im.admin");
            return oDataAccess.ExecuteDataTable<object>(CommandType.StoredProcedure, @"usp_Main_Select");
        }

        public IEnumerable<object> MNG2()
        {
            oDataAccess.SetParameter("@UserID", "admin");
            return oDataAccess.ExecuteDataTable<object>(CommandType.StoredProcedure, @"usp_Left_Select");
        }

        public IEnumerable<object> API1()
        {
            oDataAccess.SetParameter("@sDate", "2001-01-01");
            oDataAccess.SetParameter("@eDate", "2015-08-13 23:59:59");
            return oDataAccess.ExecuteDataTable<object>(CommandType.StoredProcedure, @"usp_IPGInfo_List");
        }

        public IEnumerable<object> API2()
        {
            oDataAccess.SetParameter("@sDate", "2001-01-01");
            oDataAccess.SetParameter("@eDate", "2015-08-13 23:59:59");
            return oDataAccess.ExecuteDataTable<object>(CommandType.StoredProcedure, @"usp_IPGInfo_List");
        }
    }
}
