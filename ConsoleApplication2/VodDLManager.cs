using NBWare.CMS.Framework.Persistent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
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

        public DataTable CMS1()
        {
            try
            {
                oDataAccess.SetParameter("@startno", 1);
                oDataAccess.SetParameter("@endno", 100);
                oDataAccess.SetParameter("@sDate", "2001-01-01");
                oDataAccess.SetParameter("@eDate", "2014-08-13 23:59:59");
                return oDataAccess.ExecuteDataTable(CommandType.StoredProcedure, @"usp_ContentsVOD_List");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataTable CMS2()
        {
            try
            {
                oDataAccess.SetParameter("@startno", 1);
                oDataAccess.SetParameter("@endno", 100);
                oDataAccess.SetParameter("@TYPE", "SUBJECT");
                return oDataAccess.ExecuteDataTable(CommandType.StoredProcedure, @"usp_Board_Select");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataTable MNG1()
        {
            oDataAccess.SetParameter("@UserID", "admin");
            oDataAccess.SetParameter("@PARENTID", "im.admin");
            return oDataAccess.ExecuteDataTable(CommandType.StoredProcedure, @"usp_Main_Select");
        }

        public DataTable MNG2()
        {
            oDataAccess.SetParameter("@UserID", "admin");
            return oDataAccess.ExecuteDataTable(CommandType.StoredProcedure, @"usp_Left_Select");
        }

        public DataTable API1()
        {
            oDataAccess.SetParameter("@sDate", "2001-01-01");
            oDataAccess.SetParameter("@eDate", "2015-08-13 23:59:59");
            return oDataAccess.ExecuteDataTable(CommandType.StoredProcedure, @"usp_IPGInfo_List");
        }

        public DataTable API2()
        {
            oDataAccess.SetParameter("@sDate", "2001-01-01");
            oDataAccess.SetParameter("@eDate", "2015-08-13 23:59:59");
            return oDataAccess.ExecuteDataTable(CommandType.StoredProcedure, @"usp_IPGInfo_List");
        }
    }
}
