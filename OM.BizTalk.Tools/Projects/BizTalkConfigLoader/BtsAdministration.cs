using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management;

namespace BiztalkConfigLoader
{
    public class BtsAdministration
    {
        #region Private constants
        private const string BtsWmiNameSpace = @"root\MicrosoftBizTalkServer";
        private const string BtsHostSettingNameSpace = "MSBTS_HostSetting";
        private const string BtsServerAppTypeNameSpace = "MSBTS_ServerHost";
        private const string BtsHostInstanceNameSpace = "MSBTS_HostInstance";
        private const string BtsAdapterSettingNameSpace = "MSBTS_AdapterSetting";
        private const string BtsReceiveHandlerNameSpace = "MSBTS_ReceiveHandler";
        private const string BtsSendHandlerNameSpace = "MSBTS_SendHandler2"; 
        #endregion

        #region Hosts
        public static void AddHost(string hostName, string ntGroupName, bool isDefault, bool hostTracking, bool authTrusted, string hostType, bool isHost32BitOnly, Dictionary<string, string> hostSettings)
        {
            var options = new PutOptions {Type = PutType.CreateOnly};
            var btsAdminObjClass = new ManagementClass(BtsWmiNameSpace, BtsHostSettingNameSpace, new ObjectGetOptions());
            var btsAdminObject = btsAdminObjClass.CreateInstance();
            var appObject = btsAdminObject;
            if (appObject != null)
            {
                appObject["Name"] = hostName;
                appObject["NTGroupName"] = ntGroupName;
                appObject["IsDefault"] = isDefault;
                appObject["HostTracking"] = hostTracking;
                appObject["AuthTrusted"] = authTrusted;
                if(hostType.ToString().ToLower().Contains("inprocess"))
                {
                    appObject["HostType"] = 1;
                }
                else
                {
                    appObject["HostType"] = 2;
                }
                            
                appObject["IsHost32BitOnly"] = isHost32BitOnly;
                appObject.Put(options);

                foreach (var hostSetting in hostSettings)
                {
                    SetHostSettings(hostSetting.Key, hostSetting.Value);
                }
              
                
            }
        }

        public static void CheckForHostSettings(Dictionary<string, string> hostSettings, string hostname)
        {
           ManagementObjectCollection QueryCol = Searcher(BtsWmiNameSpace, BtsHostSettingNameSpace);
            foreach (ManagementObject Inst in QueryCol)
            {
                if (Inst["NAME"].ToString().ToLower() == hostname.ToLower())
                {
                    foreach (var setting in hostSettings)
                    {
                        uint value = Convert.ToUInt32(Inst.GetPropertyValue(setting.Key));

                        foreach (var item in Inst.Properties)
                        {
                            if (item.Name.ToLower().Contains(setting.Key.ToLower()))
                            {
                                if (value != Convert.ToInt32(setting.Value))
                                {
                                    Inst.SetPropertyValue(setting.Key, setting.Value);
                                    Inst.Put();
                                    break;
                                }
                            }
                        }

                    }
                }
            }
        }
        public static void SetHostSettings(string name, string value)
        {
           ManagementObjectCollection QueryCol = Searcher(BtsWmiNameSpace, BtsHostSettingNameSpace);

            foreach (ManagementObject Inst in QueryCol)
            {
                Inst.SetPropertyValue(name, value);
               Inst.Put();
                break;
            }
        }
     

        public static bool IsHostExist(string hostName)
        {
            var btsAdminObjClass = new ManagementClass(BtsWmiNameSpace, BtsHostSettingNameSpace, new ObjectGetOptions());
            if (ObjectExists(btsAdminObjClass, "Name", hostName))
            {
                return true;
            }
            return false;
          }
        public static void RemoveHost(string hostName)
        {
            var btsAdminObjClass = new ManagementClass(BtsWmiNameSpace, BtsHostSettingNameSpace, new ObjectGetOptions());
            if (ObjectExists(btsAdminObjClass, "Name", hostName))
            {
                var btsAdminObject = btsAdminObjClass.CreateInstance();
                var appObject = btsAdminObject;
                if (appObject != null)
                {
                    appObject["Name"] = hostName;
                    appObject.Delete();
                }
            }
        } 
        #endregion

        #region Host instances
        public static void AddHostInstance(string servername, string hostname, string username, string password, bool startInstance, Dictionary<string, string> hostInstanceSettings)
        {
            var btsAdminObjClassServerHost = new ManagementClass(BtsWmiNameSpace, BtsServerAppTypeNameSpace, new ObjectGetOptions());
            var btsAdminObjectServerHost = btsAdminObjClassServerHost.CreateInstance();
            if (btsAdminObjectServerHost != null)
            {
                btsAdminObjectServerHost["ServerName"] = servername;
                btsAdminObjectServerHost["HostName"] = hostname;
                btsAdminObjectServerHost.InvokeMethod("Map", null);
            }
            var btsAdminObjClassHostInstance = new ManagementClass(BtsWmiNameSpace, BtsHostInstanceNameSpace, new ObjectGetOptions());
            var btsAdminObjectHostInstance = btsAdminObjClassHostInstance.CreateInstance();

            //foreach (var setting in hostInstanceSettings)
            //{
            //    btsAdminObjectHostInstance[setting.Key] = setting.Value;
            //}

            var objparams = new object[3];
            objparams[0] = username;
            objparams[1] = password;
            objparams[2] = true;

            if (btsAdminObjectHostInstance != null)
            {
                btsAdminObjectHostInstance["Name"] = "Microsoft BizTalk Server " + hostname + " " + servername;
                btsAdminObjectHostInstance.InvokeMethod("Install", objparams);
                if (startInstance)
                {
                    btsAdminObjectHostInstance.InvokeMethod("Start", null);
                }
            }
        }

        public static bool IsAuthenticated( string usr, string pwd)
        {
            string ServACcount = null;
            string [] splitUserName = usr.Split('\\');
            if (splitUserName.Length > 1 && usr.ToLower().Contains(usr.ToLower()))
            {
                ServACcount = splitUserName[1];
            }else
            {
                ServACcount = usr;
            }          
            string srvr =  @"LDAP://omi.com";
            bool authenticated = false;

            try
            {
                DirectoryEntry entry = new DirectoryEntry(srvr, ServACcount, pwd);
                object nativeObject = entry.NativeObject;
                authenticated = true;
            }
            catch (DirectoryServicesCOMException cex)
            {
                //not authenticated; reason why is in cex
            }
                     
            return authenticated;
        }

        public static bool IsHostInstanceExist(string servername, string hostname)
        {
            var hostInstanceName = "Microsoft BizTalk Server " + hostname + " " + servername;
            var hostInstClass = new ManagementClass(BtsWmiNameSpace, BtsHostInstanceNameSpace, new ObjectGetOptions());
            var enumOptions = new EnumerationOptions { ReturnImmediately = false };
            var hostInstCollection = hostInstClass.GetInstances(enumOptions);
            ManagementObject hostInstance = null;
            foreach (ManagementObject inst in hostInstCollection)
            {
                if (inst["Name"] != null)
                {
                    if (inst["Name"].ToString().ToUpper().Contains(hostInstanceName.ToUpper()))
                    {
                        hostInstance = inst;
                    }
                }
            }

            if (hostInstance != null)
            {
                return true;
            }
            return false;
        }
        public static void RemoveHostInstance(string servername, string hostname)
        {
            var hostInstanceName = "Microsoft BizTalk Server " + hostname + " " + servername;
            var hostInstClass = new ManagementClass(BtsWmiNameSpace, BtsHostInstanceNameSpace, new ObjectGetOptions());
            var enumOptions = new EnumerationOptions {ReturnImmediately = false};
            var hostInstCollection = hostInstClass.GetInstances(enumOptions);
            ManagementObject hostInstance = null;
            foreach (ManagementObject inst in hostInstCollection)
            {
                if (inst["Name"] != null)
                {
                    if (inst["Name"].ToString().ToUpper() == hostInstanceName.ToUpper())
                    {
                        hostInstance = inst;
                    }
                }
            }

            if (hostInstance == null)
                return;

            if (hostInstance["HostType"].ToString() != "2" && hostInstance["ServiceState"].ToString() == "4")
            {
                hostInstance.InvokeMethod("Stop", null);
            }

            hostInstance.InvokeMethod("UnInstall", null);

            var svrHostClass = new ManagementClass(BtsWmiNameSpace, BtsServerAppTypeNameSpace, new ObjectGetOptions());
            var svrHostObject = svrHostClass.CreateInstance();

            if (svrHostObject != null)
            {
                svrHostObject["ServerName"] = servername;
                svrHostObject["HostName"] = hostname;
                svrHostObject.InvokeMethod("UnMap", null);
            }
        } 
        #endregion

        #region Adapters and handlers
        public static void AddAdapter(string name)
        {
            ManagementObject objInstance = null;
            var objClass = new ManagementClass(BtsWmiNameSpace, BtsAdapterSettingNameSpace, new ObjectGetOptions());
            var enumOptions = new EnumerationOptions {ReturnImmediately = false};
            var hostInstCollection = objClass.GetInstances(enumOptions);
            foreach (ManagementObject inst in hostInstCollection)
            {
                if (inst["Name"] != null)
                {
                    if (inst["Name"].ToString().ToUpper() == name.ToUpper())
                    {
                        objInstance = inst;
                    }
                }
            }
            if (objInstance == null)
            {
                objInstance = objClass.CreateInstance();
                if (objInstance != null)
                {
                    objInstance.SetPropertyValue("Name", name);
                    
                }
            }
            try
            {
                var options = new PutOptions() { Type = PutType.UpdateOrCreate };
                objInstance.Put(options);
            }
            catch (Exception)
            {
                return;
            }
        }

        public static bool IsAdapterExist(string name)
        {
            ManagementObject objInstance = null;
            var objClass = new ManagementClass(BtsWmiNameSpace, BtsAdapterSettingNameSpace, new ObjectGetOptions());
            objInstance = objClass.CreateInstance();
            if (objInstance != null)
            {
                objInstance.SetPropertyValue("Name", name);
                if (ObjectExists(objClass, "Name", name))
                {
                    return true;
                }
            }
            return false;
        }
        public static void RemoveAdapter(string name)
        {
            ManagementObject objInstance = null;
            var objClass = new ManagementClass(BtsWmiNameSpace, BtsAdapterSettingNameSpace, new ObjectGetOptions());
            //Leave the adapter if there are other instances depending on it.
            if (objClass.GetInstances().Count > 0)
                return;
            objInstance = objClass.CreateInstance();
            if (objInstance != null)
            {
                objInstance.SetPropertyValue("Name", name);
                if (ObjectExists(objClass, "Name", name))
                {
                    objInstance.Delete();
                }
            }
        }
        public static bool IsReceiveHandlerExist(string adaptername, string hostname)
        {
            
            ManagementObjectCollection QueryCol = Searcher(BtsWmiNameSpace, BtsReceiveHandlerNameSpace);

            foreach (ManagementObject Inst in QueryCol)
            {
               if (Inst.Path.Path.Contains(hostname)& Inst.Path.Path.Contains(adaptername.ToUpper()))
                {
                    return true;
                        
                }
            }

                return false;
        }

        public static void AddReceiveHandler(string adaptername, string hostname)
        {
            var objClass = new ManagementClass(BtsWmiNameSpace, BtsReceiveHandlerNameSpace, new ObjectGetOptions());
            var objInstance = objClass.CreateInstance();
            if (objInstance != null)
            {
                objInstance.SetPropertyValue("AdapterName", adaptername);
                objInstance.SetPropertyValue("HostName", hostname);
                objInstance.Put();
            }
        }

        public static void RemoveReceiveHandler(string adaptername, string hostname)
        {
            var objClass = new ManagementClass(BtsWmiNameSpace, BtsReceiveHandlerNameSpace, new ObjectGetOptions());
            if (ObjectExists(objClass, "HostName", hostname, "AdapterName", adaptername))
            {
                var objInstance = objClass.CreateInstance();
                if (objInstance != null)
                {
                    objInstance.SetPropertyValue("AdapterName", adaptername);
                    objInstance.SetPropertyValue("HostName", hostname);
                    objInstance.Delete();
                }
            }
        }

        public static bool IsSendHandlerExist(string adaptername, string hostname)
        {
            ManagementObjectCollection QueryCol = Searcher(BtsWmiNameSpace, BtsSendHandlerNameSpace);

            foreach (ManagementObject Inst in QueryCol)
            {
                if (Inst.Path.Path.Contains(hostname) & Inst.Path.Path.Contains(adaptername.ToUpper()))
                {
                    return true;

                }
            }

            return false;
        }

        private static ManagementObjectCollection Searcher( string btswminamespace, string btsNamespace )
        {
            ManagementObjectSearcher Searcher = new ManagementObjectSearcher();
            ManagementScope Scope = new ManagementScope(btswminamespace);
            Searcher.Scope = Scope;
            SelectQuery Query = new SelectQuery();
            Query.QueryString = "SELECT * FROM " + btsNamespace;
            Searcher.Query = Query;
            ManagementObjectCollection QueryCol = Searcher.Get();
            return QueryCol;
        }

        public static void AddSendHandler(string adaptername, string hostname)
        {
            var objClass = new ManagementClass(BtsWmiNameSpace, BtsSendHandlerNameSpace, new ObjectGetOptions());
            var objInstance = objClass.CreateInstance();
            if (objInstance != null)
            {
                objInstance.SetPropertyValue("AdapterName", adaptername);
                objInstance.SetPropertyValue("HostName", hostname);
                objInstance.Put();
            }
        }

        public static void RemoveSendHandler(string adaptername, string hostname)
        {
            var objClass = new ManagementClass(BtsWmiNameSpace, BtsSendHandlerNameSpace, new ObjectGetOptions());
            if (ObjectExists(objClass, "HostName", hostname, "AdapterName", adaptername))
            {
                var objInstance = objClass.CreateInstance();
                if (objInstance != null)
                {
                    objInstance.SetPropertyValue("AdapterName", adaptername);
                    objInstance.SetPropertyValue("HostName", hostname);
                    objInstance.Delete();
                }
            }
        } 
        #endregion

        #region ObjectExists
        private static bool ObjectExists(ManagementClass objClass, string key, string value)
        {
            foreach (ManagementObject obj in objClass.GetInstances())
            {
                if (obj[key].ToString() == value)
                    return true;
            }
            return false;
        }

        private static bool ObjectExists(ManagementClass objClass, string key, string value, string key2, string value2)
        {
            foreach (ManagementObject obj in objClass.GetInstances())
            {
                if (obj[key].ToString() == value && obj[key2].ToString() == value2)
                    return true;
            }
            return false;
        } 
        #endregion
       
    }
}
