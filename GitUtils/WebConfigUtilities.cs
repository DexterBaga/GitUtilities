using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Hosting;
namespace GitUtils
{
    public static class WebConfigUtilities
    {
        public static void ResetCredentials(string webConfigPath,string userName="",string password="")
        {
            if(!File.Exists(webConfigPath))
                throw new Exception($"{webConfigPath} does not exist");
            var webConfiguration = GetWebConfigurationFromFile(webConfigPath);

            var connectionStringSettingsCollection = ((ConnectionStringsSection)webConfiguration.GetSection("connectionStrings")).ConnectionStrings;

            foreach (ConnectionStringSettings setting in connectionStringSettingsCollection)
            {
                
                if (!IsEntityFrameworkConnectionString(setting.ConnectionString)) continue;

                setting.ConnectionString =  UpdateUserPasswordInConnectionString(setting.ConnectionString, userName, password);

            }
            webConfiguration.Save();

        }

        public static void ResetCredentials(string webConfigPath, string connectionStringName, string userName, string password)
        {
            if (!File.Exists(webConfigPath))
                throw new Exception($"{webConfigPath} does not exist");

            var webConfiguration = GetWebConfigurationFromFile(webConfigPath);

            var connectionStringSettingsCollection = ((ConnectionStringsSection)webConfiguration.GetSection("connectionStrings")).ConnectionStrings;

            var entry = connectionStringSettingsCollection[connectionStringName];
            entry.ConnectionString = UpdateUserPasswordInConnectionString(entry.ConnectionString, userName, password);
            webConfiguration.Save();
        }

        private static string UpdateUserPasswordInConnectionString(string connectionString,string userName, string newPassword)
        {
            var entityConnectionStringBuilder = new EntityConnectionStringBuilder(connectionString);

            var builder = new SqlConnectionStringBuilder(entityConnectionStringBuilder.ProviderConnectionString)
            {
                Password = newPassword,
                UserID = userName
            };
            entityConnectionStringBuilder.ProviderConnectionString = builder.ConnectionString;
            return  entityConnectionStringBuilder.ConnectionString;
        }

        private static Configuration GetWebConfigurationFromFile(string webConfigPath)
        {
            var webConfigFileInfo = new FileInfo(webConfigPath);

            var virtualDirectoryMapping = new VirtualDirectoryMapping(webConfigFileInfo.DirectoryName, true,
                webConfigFileInfo.Name);
            var webConfigurationFileMap = new WebConfigurationFileMap();
            webConfigurationFileMap.VirtualDirectories.Add("/", virtualDirectoryMapping);
            var webConfiguration = WebConfigurationManager.OpenMappedWebConfiguration(webConfigurationFileMap, "/");
            return webConfiguration;
        }

        private static bool IsEntityFrameworkConnectionString(string setting)
        {
            return setting.Contains("metadata=res:");
        }
    }
}
