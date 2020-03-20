namespace Microsoft.BizTalk.ApplicationDeployment.CommandLine
{
    using Microsoft.BizTalk.ApplicationDeployment;
    using Microsoft.BizTalk.ExplorerOM;
    using Microsoft.BizTalk.Operations;
    using Microsoft.BizTalk.Log;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Management;
    using System.Reflection;
    using System.Text;

    internal sealed class RestartHostInstancesCommand : Command
    {
        public RestartHostInstancesCommand(NameValueCollection nameValueArgs) : base(nameValueArgs)
        {
        }

        public override void Execute()
        {
            BtsCatalogExplorer explorer = null;

            try
            {
                this.Validate();
                string applicationName = base.Args["ApplicationName"];
                string server = base.Args["Server"];
                string database = base.Args["Database"];
                string formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.RestartHostInstances, new object[] { applicationName, server, database });
                base.WriteLogEntry(LogEntryType.Information, formattedString);
                explorer = new BtsCatalogExplorer();
                explorer.ConnectionString = ParameterHelper.GetConnectionString(server, database);
                Microsoft.BizTalk.ExplorerOM.Application application = explorer.Applications[applicationName];
                if (application == null)
                {
                    throw new ApplicationException("Unable to find application named " + applicationName);
                }

                ArrayList distinctHosts = new ArrayList();
                getHostsFromApplication(distinctHosts, application);
                restartAlreadyRunningInProcHostInstances(explorer, distinctHosts);

                formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.RestartHostInstancesSuccess, new object[] { applicationName });
                base.WriteLogEntry(LogEntryType.Information, formattedString);
                base.commandResult = new CommandResult();
            }
            catch (Exception exception)
            {
                //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(exception.ToString(), new object[0]);
                base.WriteLogEntry(LogEntryType.Error, exception.Message);
                base.commandResult = new CommandResult(exception);
                if ((exception is OutOfMemoryException) || /*(exception is ExecutionEngineException)) ||*/ (exception is StackOverflowException))
                {
                    throw;
                }
            }
            finally
            {
                if (explorer != null)
                {
                    explorer.Dispose();
                    explorer = null;
                }
            }
        }

        private void restartAlreadyRunningInProcHostInstances(BtsCatalogExplorer explorer, ArrayList distinctHosts)
        {
            bool oneOrMoreFailedToRestart = false;
            StringBuilder sbuilder = new StringBuilder();

            foreach (string hostname in distinctHosts)
            {
                ManagementScope scope = new ManagementScope(@"root\MicrosoftBizTalkServer");
                EnumerationOptions options = new EnumerationOptions();
                options.ReturnImmediately = false;

                ManagementObjectSearcher searcher = null;
                ManagementObjectCollection managementObjectCollection = null;

                try
                {
                    SelectQuery query = new SelectQuery("MSBTS_HostInstance",
                        string.Format("HostName=\"{0}\"", hostname));
                    searcher = new ManagementObjectSearcher(scope, query, options);
                    managementObjectCollection = searcher.Get();
                    foreach (ManagementObject hostInstance in managementObjectCollection)
                    {
                        uint num = (uint)hostInstance["ServiceState"];
                        if (num == 4)
                        {
                            // it is already running, restart it.
                            object[] objparams = new object[0];
                            hostInstance.InvokeMethod("Stop", objparams);
                            hostInstance.InvokeMethod("Start", objparams);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                    sbuilder.AppendLine(ex.ToString());
                    oneOrMoreFailedToRestart = true;
                }
                finally
                {
                    if (searcher != null)
                    {
                        searcher.Dispose();
                        searcher = null;
                    }
                    if (managementObjectCollection != null)
                    {
                        managementObjectCollection.Dispose();
                        managementObjectCollection = null;
                    }
                }
            }

            if (oneOrMoreFailedToRestart)
            {
                throw new ApplicationException("One or more host instances failed to restart.", new ApplicationException(sbuilder.ToString()));
            }
        }

        // Code from Microsoft.BizTalk.Administration.SnapIn, Version=3.0.1.0
        private static void getHostsFromApplication(ArrayList hosts, IBizTalkApplication application)
        {
            foreach (IBtsOrchestration orchestration in application.Orchestrations)
            {
                if (((orchestration.Host != null) && (orchestration.Host.Type == HostType.InProcess)) && !hosts.Contains(orchestration.Host.Name))
                {
                    hosts.Add(orchestration.Host.Name);
                }
            }
            foreach (ISendPort port in application.SendPorts)
            {
                ISendHandler sendHandler = null;
                if (port.PrimaryTransport != null)
                {
                    sendHandler = ((ITransportInfo2)port.PrimaryTransport).SendHandler;
                    if (((sendHandler != null) && (sendHandler.Host.Type == HostType.InProcess)) && !hosts.Contains(sendHandler.Host.Name))
                    {
                        hosts.Add(sendHandler.Host.Name);
                    }
                }
                if (port.SecondaryTransport != null)
                {
                    sendHandler = ((ITransportInfo2)port.SecondaryTransport).SendHandler;
                    if (((sendHandler != null) && (sendHandler.Host.Type == HostType.InProcess)) && !hosts.Contains(sendHandler.Host.Name))
                    {
                        hosts.Add(sendHandler.Host.Name);
                    }
                }
            }
            foreach (IReceivePort port2 in application.ReceivePorts)
            {
                foreach (IReceiveLocation location in port2.ReceiveLocations)
                {
                    if (((location.ReceiveHandler != null) && (location.ReceiveHandler.Host.Type == HostType.InProcess)) && !hosts.Contains(location.ReceiveHandler.Host.Name))
                    {
                        hosts.Add(location.ReceiveHandler.Host.Name);
                    }
                }
            }
        }

        protected override CommandLineArgDescriptorList GetParameterDescriptors()
        {
            CommandLineArgDescriptor[] collection = new CommandLineArgDescriptor[] { 
                new CommandLineArgDescriptor(true, "ApplicationName", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_RequiredApplicationName), CommandLineArgDescriptor.ArgumentType.String, 1, 1), 
                new CommandLineArgDescriptor(true, "Server", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_Server), CommandLineArgDescriptor.ArgumentType.String), 
                new CommandLineArgDescriptor(true, "Database", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_Database), CommandLineArgDescriptor.ArgumentType.String) };
            CommandLineArgDescriptorList list = new CommandLineArgDescriptorList();
            list.AddRange(collection);
            return list;
        }

        public override void Validate()
        {
            ParameterHelper.ValidateServerDatabase(base.Args);
            ParameterHelper.ValidateApplicationName(base.Args);
        }

        public override string Name
        {
            get
            {
                return "RestartHostInstances";
            }
        }
    }
}

