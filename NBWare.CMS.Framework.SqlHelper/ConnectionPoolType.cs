
namespace NBWare.CMS.Framework.SqlHelper
{
    public class ConnectionPoolType
    {
        /// <summary> DB명 </summary>
        public DBNameType dBName = DBNameType.None;
        public DBNameType DBName
        {
            get
            {
                return dBName;
            }
            set
            {
                dBName = value;
            }
        }

        /// <summary> ProviderName </summary>
        public ProviderNameType providerName = ProviderNameType.MSSQL;
        public ProviderNameType ProviderName
        {
            get
            {
                return providerName;
            }
            set
            {
                providerName = value;
            }
        }

        /// <summary> ConfigPath </summary>
        public string configPath = string.Empty;
        public string ConfigPath
        {
            get
            {
                return configPath;
            }
            set
            {
                configPath = value;
            }
        }
    }
}
