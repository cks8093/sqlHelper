using NBWare.CMS.Framework.Common;
using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NBWare.CMS.Framework.SqlHelper
{
    /// <summary>
    /// 설정 파일 관리
    /// </summary>
    public class ConfigManager : IEnumerable
    {

        // Fields
        private static System.Configuration.Configuration _config = null;
        private static string _configFilePath = string.Empty;
        private static object SycRoot = new object();
        private static ConfigManager _Self;

        public static string ConfigPath
        {
            set { _configFilePath = value; }
        }

        // Properties
        public static System.Configuration.Configuration Config
        {
            get
            {
                lock (SycRoot)
                {
                    if (_config == null)
                    {
                        do
                        {
                            if (!String.IsNullOrEmpty(_configFilePath)) break;

                            if (IsWeb)
                            {
                                _configFilePath = HttpContext.Current.Server.MapPath("~\\CMSWeb.config");
                                break;
                            }

                            _configFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CMSWeb.config";

                        } while (false);

                        ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                        fileMap.ExeConfigFilename = _configFilePath;
                        _config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                    }


                    return _config;
                }
            }
        }

        private static bool IsWeb
        {
            get
            {
                return (HttpContext.Current != null);
            }
        }

        public string this[string key]
        {
            get
            {
                return GetString(key, true);
            }
        }

        public static ConfigManager Default
        {
            get
            {
                lock (SycRoot)
                {
                    if (_Self == null)
                    {
                        _Self = new ConfigManager();
                    }
                    return _Self;
                }
            }
        }

        public static void SetAppConfigFilePath(string appConfigExternalFIlePath)
        {
            _configFilePath = appConfigExternalFIlePath;
        }


        public static string GetString(string key, bool isEncrypt)
        {
            string encryptedText = string.Empty;
            try
            {
                System.Configuration.Configuration config = Config;

                CryptoManager cryptoManager = new CryptoManager();
                if (ContainsKey(config, key))
                {
                    encryptedText = config.AppSettings.Settings[key].Value;
                }
                if (isEncrypt)
                {
                    encryptedText = cryptoManager.Decrypt(encryptedText);
                }
            }
            catch (Exception ex)
            {
                encryptedText = string.Empty;

                string exMsg = ex.ToString();
            }

            return encryptedText;

        }

        public static bool ContainsKey(System.Configuration.Configuration cfg, string key)
        {
            lock (SycRoot)
            {
                return cfg.AppSettings.Settings.AllKeys.Contains<string>(key);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return Config.AppSettings.Settings.GetEnumerator();
        }
    }
}
