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
        public static UpdatedConnectionStrings ResetCredentials(string webConfigPath,string userName="",string password="",string connectionStringName="")
        {
            var connectionStringItems = new UpdatedConnectionStrings();

            if(!File.Exists(webConfigPath))
                throw new Exception($"{webConfigPath} does not exist!");
            var webConfiguration = GetWebConfigurationFromFile(webConfigPath);

            ConnectionStringSettingsCollection connectionStringSettingsCollection = ((ConnectionStringsSection)webConfiguration.GetSection("connectionStrings")).ConnectionStrings;

            var connectionStringSettings =
                connectionStringSettingsCollection.Cast<ConnectionStringSettings>()
                    .AsQueryable()
                    .Where(x => x.Name == connectionStringName || connectionStringName == string.Empty);
            

            foreach (ConnectionStringSettings setting in connectionStringSettings)
            {
                if (!IsEntityFrameworkConnectionString(setting.ConnectionString)) continue;

                setting.ConnectionString =  UpdateUserPasswordInConnectionString(setting.ConnectionString, userName, password);
                connectionStringItems.Add(setting.Name);
            }
            webConfiguration.Save();
            return connectionStringItems;
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

    public class UpdatedConnectionStrings
    {
        private readonly List<string> items = new List<string>();

        public List<string> Items
        {
            get { return items; }
        }

        public int Count
        {
            get { return items.Count; }
        }

        public void Add(string connectionStringName)
        {
            Items.Add(connectionStringName);
        }
    }
}
