using BiztalkConfigLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkConfigLoader
{
  public  class Processor:IProcessor
    {
        #region Privates
       
        private static UTF8Encoding encoding = new UTF8Encoding();
        private static string logFile;
        private static string defaultUserName;
        private static string defaultPassword;
        #endregion


        private static string GetServername( string servername)
        {
            if ( string.IsNullOrEmpty(servername) ||servername == "." || servername.ToLower().Contains("local"))
            {
                servername = Environment.MachineName;
            }
            return servername;
        }
        #region Hosts

        private static void Process(Configuration hosts, string username, string password)
        {
            if (BtsAdministration.IsAuthenticated(username, password))
            {
                foreach (var item in hosts.Hosts)
                {


                    try
                    {

                        Dictionary<string, string> hostSettings = PopulateSettings(item);
                        if (!BtsAdministration.IsHostExist(item.Name))
                        {

                            WriteLog("Adding host " + item.Name);

                            BtsAdministration.AddHost(item.Name, item.Ntgroupname, item.Isdefault, item.Tracking, item.Authtrusted, item.Hosttype, Convert.ToBoolean(item.Ishost32bitonly), hostSettings);
                            WriteLog("  Succeeded");
                        }
                        else
                        {
                            WriteLog(string.Format("This {0} Exists. Checking Host settings", item.Name));
                            BtsAdministration.CheckForHostSettings(hostSettings, item.Name);
                            WriteLog(string.Format("This {0} Exists. Installation skipped this host", item.Name));
                        }

                        AddHostInstances(item.Instanceinfo, item.Name, username, password);
                        AddAdapters(item.Adapters, item.Name);
                    }
                    catch (Exception e)
                    {
                        WriteLog("  Error " + e.Message);
                    }
                }
            }
            else
            {
                WriteLog("  Error " + "Service Account not Authenticated");

            }

        }

        private static Dictionary<string, string> PopulateSettings(HostType item)
        {
            var hostSettingNodes = item.Settings;
            var hostSettings = new Dictionary<string, string>();
            WriteLog("Adding " + item.Settings.InflightMessageThreshold);
            hostSettings.Add("SubscriptionPauseAt", Convert.ToString(item.Settings.SubscriptionPauseAt));
            WriteLog("Adding host setting SubscriptionPauseAt " + "value = " + item.Settings.SubscriptionPauseAt);
            hostSettings.Add("SubscriptionResumeAt", Convert.ToString(item.Settings.SubscriptionResumeAt));
            WriteLog("Adding host setting SubscriptionResumeAt " + "value = " + item.Settings.SubscriptionResumeAt);
            hostSettings.Add("ProcessMemoryThreshold", Convert.ToString(item.Settings.ProcessMemoryThreshold));
            WriteLog("Adding host setting ProcessMemoryThreshold " + "value = " + item.Settings.ProcessMemoryThreshold);
            hostSettings.Add("MessageDeliveryOverdriveFactor", Convert.ToString(item.Settings.MessageDeliveryOverdriveFactor));
            WriteLog("Adding host setting MessageDeliveryOverdriveFactor " + "value = " + item.Settings.MessageDeliveryOverdriveFactor);
            return hostSettings;
        }

        private static void RemoveHosts(Configuration hosts)
        {

            if (hosts == null) return;
            foreach (var host in hosts.Hosts)
            {
                if (host != null)
                {
                    string hostName = host.Name;
                    WriteLog("Removing host " + hostName);
                    try
                    {
                        BtsAdministration.RemoveHost(hostName);
                        WriteLog("  Succeeded");
                    }
                    catch (Exception e)
                    {
                        WriteLog("  Error " + e.Message);
                    }
                }
            }
        }
        #endregion

        #region Host instances
        private static void AddHostInstances(InstanceinfoType[] obj, string hostname, string instusername, string instpassword)
        {
            foreach (InstanceinfoType instance in obj)
            {
                if (string.IsNullOrEmpty(instance.servername))
                {

                    instance.servername = GetServername(instance.servername);
                }

                if (!BtsAdministration.IsHostInstanceExist(instance.servername, hostname))
                {
                    if (instance != null)                    {
                                           
                        var username = instusername;
                        if (username.Length == 0)
                            username = defaultUserName;
                        var password = instpassword;
                        if (password.Length == 0)
                            password = defaultPassword;
                        var startinstance = Convert.ToBoolean(instance.StartInstance);
                        WriteLog("Adding hostinstance " + hostname + " on " + instance.servername);
                        try
                        {
                            if (startinstance)
                            {
                                WriteLog("and starting it...");
                                BtsAdministration.AddHostInstance(instance.servername, hostname, username, password, startinstance, null);
                                WriteLog("  Succeeded");
                            }
                        }

                        catch (Exception e)
                        {
                            WriteLog("  Error " + e.Message);
                        }
                    }
                }
            }
        }

        private static void RemoveHostInstances(Configuration obj)
        {
            foreach (var host in obj.Hosts)
            {
                foreach (var instance in host.Instanceinfo)
                {
                    string servername = GetServername(instance.servername);                  
                    string hostname = host.Name;
                    WriteLog("Removing hostinstance " + hostname + " on " + servername);
                    try
                    {
                        BtsAdministration.RemoveHostInstance(servername, hostname);
                        WriteLog("  Succeeded");
                    }
                    catch (Exception e)
                    {
                        WriteLog("  Error " + e.Message);
                    }
                }
            }

        }
        #endregion

        #region Adapters and handlers
        private static void AddAdapters(HostTypeAdapter[] adapters, string hostname)
        {
            foreach (var item in adapters)
            {

                WriteLog("Adding adapter " + item.Name);
                try
                {
                    if (!BtsAdministration.IsAdapterExist(hostname))
                    {
                        WriteLog("Adding adapter " + item.Name);
                        BtsAdministration.AddAdapter(item.Name);
                        WriteLog("  Succeeded");                       
                        WriteLog(string.Format("{0} ha been succesfully addedd", item.Name));
                    }
                    else
                    {
                        WriteLog(string.Format("This {0} already installed", item.Name));
                    }

                    if (item.Direction == HostTypeAdapterDirection.Receive)
                    {
                        if (!BtsAdministration.IsReceiveHandlerExist(item.Name, hostname))
                        {
                            WriteLog("Adding ReceiveHandler" + item.Name);
                            AddReceiveHandlers(item.Name, hostname);
                            WriteLog("Successfully added ReceiveHandler" + item.Name);
                        }else
                        {
                            WriteLog(string.Format("ReceiveHandler: {0} already installed", item.Name));
                        }
                    }
                    if (item.Direction == HostTypeAdapterDirection.Send)
                    {
                        if (!BtsAdministration.IsSendHandlerExist(item.Name, hostname))
                        {
                            WriteLog("Adding SendHandler" + item.Name);
                            AddSendHandlers(item.Name, hostname);
                            WriteLog("Successfully added SendHanler" + item.Name);
                        }
                        else
                        {
                            WriteLog(string.Format("SendHandler: {0} already installed", item.Name));
                        }
                    }
                }
                catch (Exception e)
                {
                    WriteLog("  Error " + e.Message);
                }
            }


        }

        private static void AddReceiveHandlers(string adaptername, string hostname)
        {

            WriteLog("Adding receive handler for host " + hostname);
            try
            {
                BtsAdministration.AddReceiveHandler(adaptername, hostname);
                WriteLog("  Succeeded");
            }
            catch (Exception e)
            {
                WriteLog("  Error " + e.Message);
            }

        }

        private static void AddSendHandlers(string adaptername, string hostname)
        {

            WriteLog("Adding send handler for host " + hostname);
            try
            {
                BtsAdministration.AddSendHandler(adaptername, hostname);
                WriteLog("  Succeeded");
            }
            catch (Exception e)
            {
                WriteLog("  Error " + e.Message);
            }

        }

        private static void RemoveAdapters(Configuration obj)
        {

            foreach (var item in obj.Hosts)
            {
                foreach (var adapter in item.Adapters)
                {

                    WriteLog("Removing adapter " + adapter.Name);
                    try
                    {
                        RemoveReceiveHandlers(adapter.Name, item.Name);
                        RemoveSendHandlers(adapter.Name, item.Name);
                        BtsAdministration.RemoveAdapter(adapter.Name);
                        WriteLog("  Succeeded");
                    }
                    catch (Exception e)
                    {
                        WriteLog("  Error " + e.Message);
                    }
                }
            }
        }
        private static void RemoveReceiveHandlers(string adaptername, string hostname)
        {

            WriteLog("Removing receive handler for host " + hostname);
            try
            {
                BtsAdministration.RemoveReceiveHandler(adaptername, hostname);
                WriteLog("  Succeeded");
            }
            catch (Exception e)
            {
                WriteLog("  Error " + e.Message);
            }

        }

        private static void RemoveSendHandlers(string adaptername, string hostname)
        {

            WriteLog("Removing send handler for host " + hostname);
            try
            {
                BtsAdministration.RemoveSendHandler(adaptername, hostname);
                WriteLog("  Succeeded");
            }
            catch (Exception e)
            {
                WriteLog("  Error " + e.Message);
            }

        }
        #endregion

        #region Write to log
        private static void WriteLog(string message)
        {
            Console.WriteLine(message);
            File.AppendAllText(logFile, message + Environment.NewLine, encoding);
        }
        #endregion
        public void Execute(string username, string password, string configfile)
        {
            try
            {
               // string Configfile = Properties.Settings.Default.Configfile;
                if (!string.IsNullOrEmpty(configfile))
                {
                    var runRemove = Properties.Settings.Default.runRemove;
                    defaultUserName = Properties.Settings.Default.defaultUserName;
                    defaultPassword = Properties.Settings.Default.defaultPassword;
                    logFile = Path.Combine(Directory.GetCurrentDirectory(), "BTSConfig" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".log");
                    var Configsettings = Utilities.DeserializeXMLFileToObject<Configuration>(configfile);
                    
                    if (runRemove)
                    {
                        RemoveAdapters(Configsettings);
                        RemoveHostInstances(Configsettings);
                        RemoveHosts(Configsettings);
                    }

                   Process(Configsettings, username, password);
                }

            }
            catch (Exception e)
            {
                WriteLog("Fatal Error : " + e.Message + " " + e.StackTrace);
            }
            finally
            {
                WriteLog("Done!!!");
            }

        }
       
    }
}
