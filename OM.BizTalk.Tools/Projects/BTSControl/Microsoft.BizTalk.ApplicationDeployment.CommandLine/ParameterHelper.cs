namespace Microsoft.BizTalk.ApplicationDeployment.CommandLine
{
    using Microsoft.BizTalk.ApplicationDeployment;
    using Microsoft.BizTalk.BtsDbVersion;
    using Microsoft.BizTalk.Deployment;
    using Microsoft.BizTalk.ExplorerOM;
    using System;
    using System.Collections.Specialized;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    internal static class ParameterHelper
    {
        public static string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            if (bytes != null)
            {
                foreach (byte num in bytes)
                {
                    builder.Append(num.ToString("x2", CultureInfo.InvariantCulture));
                }
            }
            return builder.ToString();
        }

        private static void CheckDatabaseCompatibility(string serverName, string databaseName)
        {
            BtsDbCompatibility compatibility;
            BizTalkDBVersion version = new BizTalkDBVersion();
            try
            {
                compatibility = version.CheckCompatibility(serverName, databaseName, "Management DB", "3.12.843.2");
            }
            catch (Exception exception)
            {
                throw new CommandException(CommandResources.GetFormattedString(CommandResources.ResourceID.UnableToValidateDatabase, new object[] { databaseName, serverName }), exception);
            }
            switch (compatibility)
            {
                case BtsDbCompatibility.BtsDbFullyCompatible:
                case BtsDbCompatibility.BtsDbPartiallyCompatible:
                    return;

                case BtsDbCompatibility.BtsDbIncompatible:
                    throw new CommandLineArgumentException(CommandResources.GetString(CommandResources.ResourceID.IncompatibleDbVersion), "Database", System.Diagnostics.TraceLevel.Error);
                case BtsDbCompatibility.BtsDbNonBiztalkDatabase:
                    throw new CommandLineArgumentException(CommandResources.GetString(CommandResources.ResourceID.IncompatibleDbVersion), "Database", System.Diagnostics.TraceLevel.Error);
                case BtsDbCompatibility.BtsDbBlankDatabase:
                    throw new CommandLineArgumentException(CommandResources.GetString(CommandResources.ResourceID.IncompatibleDbVersion), "Database", System.Diagnostics.TraceLevel.Error);
            }
        }

        public static string GetConnectionString(string server, string database)
        {
//            ////Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine("Building SQL connection string...", new object[0]);
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = server;
            builder.InitialCatalog = database;
            builder.IntegratedSecurity = true;
            builder.ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
            string str = builder.ToString();
//            ////Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine("ConnectionString: " + str, new object[0]);
            return str;
        }

        public static void ValidateApplicationName(NameValueCollection nameValueArgs)
        {
//            ////Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine("Validating ApplicationName parameter...", new object[0]);
            if (nameValueArgs["ApplicationName"] != null)
            {
                nameValueArgs.Set("ApplicationName", nameValueArgs["ApplicationName"].Trim());
            }
            if ((nameValueArgs["ApplicationName"] == null) || (nameValueArgs["ApplicationName"].Length == 0))
            {
                string server = nameValueArgs["Server"];
                string database = nameValueArgs["Database"];
                BtsCatalogExplorer explorer = new BtsCatalogExplorer();
                explorer.ConnectionString = GetConnectionString(server, database);
                Microsoft.BizTalk.ExplorerOM.Application defaultApplication = explorer.DefaultApplication;
                if (defaultApplication != null)
                {
                    nameValueArgs.Set("ApplicationName", defaultApplication.Name);
                }
            }
        }

        public static void ValidateDatabase(string database)
        {
            Regex regex = new Regex(@"^[a-zA-Z]([a-zA-Z]|[0-9]|[!@#\$%\^&'\)\(\.\-_\{\}~\.]){0,62}$");
            if (!regex.Match(database).Success)
            {
                throw new CommandLineArgumentException(CommandResources.GetFormattedString(CommandResources.ResourceID.DatabaseInvalid, new object[] { database }), "Database", System.Diagnostics.TraceLevel.Error);
            }
        }

        public static void ValidateServer(string server)
        {
            IPAddress address;
            if (!IPAddress.TryParse(server, out address))
            {
                Regex regex = new Regex(@"^((np|tcp|spx|adsp|rpc|vines):)?([a-zA-Z]|[0-9]|[!@#\$%\^&'\)\(\.\-_\{\}~\.\\]){0,256}((,[0-9]{1,5})|(,(ncacn_np|ncacn_ip_tcp|ncacn_nb_nb|ncacn_spx|ncacn_vns_spp|ncadg_ip_udp|ncadg_ipx|ncalrpc)))?(\\([a-zA-Z]|[0-9]|[!@#\$%\^&'\)\(\.\-_\{\}~\.\\]){0,256})?$");
                if (!regex.Match(server).Success)
                {
                    throw new CommandLineArgumentException(CommandResources.GetFormattedString(CommandResources.ResourceID.ServerInvalid, new object[] { server }), "Server", System.Diagnostics.TraceLevel.Error);
                }
            }
        }

        public static void ValidateServerDatabase(NameValueCollection nameValueArgs)
        {
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine("Validating Server and Database parameters...", new object[0]);
            if ((nameValueArgs["Server"] != null) && (nameValueArgs["Server"].Length > 0))
            {
                if ((nameValueArgs["Database"] == null) || (nameValueArgs["Database"].Length == 0))
                {
                    throw new CommandLineArgumentException(CommandResources.GetString(CommandResources.ResourceID.DatabaseNotSpecified), "Database", System.Diagnostics.TraceLevel.Error);
                }
            }
            else
            {
                nameValueArgs.Set("Server", Environment.MachineName);
                if ((nameValueArgs["Database"] == null) || (nameValueArgs["Database"].Length == 0))
                {
                    ConfigurationDatabase database = new ConfigurationDatabase();
                    if ((database.Server.Length == 0) || (database.Database.Length == 0))
                    {
                        throw new CommandLineArgumentException(CommandResources.GetString(CommandResources.ResourceID.DefaultServerDatabaseFailed), "Database", System.Diagnostics.TraceLevel.Error);
                    }
                    nameValueArgs.Set("Server", database.Server);
                    nameValueArgs.Set("Database", database.Database);
                }
            }
            ValidateServer(nameValueArgs["Server"]);
            ValidateDatabase(nameValueArgs["Database"]);
            CheckDatabaseCompatibility(nameValueArgs["Server"], nameValueArgs["Database"]);
        }
    }
}

