
namespace NBWare.CMS.Framework.SqlHelper
{
    /// <summary>
    /// 데이터비이스의 종류를 구분합니다.
    /// </summary>
    public enum DBNameType
    {
        /// <summary>
        /// 설정 안됨 - 기본값이 사용됩니다.
        /// </summary>
        None = 0, // 설정 안됨 - 기본값 사용 

        /// <summary>
        /// CMS Database
        /// </summary>
        CMS = 1,

        /// <summary>
        /// Log Database
        /// </summary>
        LOG = 2,

        /// <summary>
        /// Management Database
        /// </summary>
        MNG = 3,

        /// <summary>
        /// MTVDB01 Database
        /// </summary>
        API = 4,

        /// <summary>
        /// MTVDB01 Database
        /// </summary>
        MTV = 4,

        /// <summary>
        /// 가입자DB_A = OTN_CUST_A
        /// </summary>
        CUA = 5,
    }

    public enum ProviderNameType
    {
        MSSQL,
        MYSQL,
        ORACLE,
        OLEDB,
        ODBC,
        MSSQLCE,
        MSORACLE
    }

    public enum TransactionType : uint
    {
        Open = 1,
        Commit = 2,
        Rollback = 3
    }
}
