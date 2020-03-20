namespace Microsoft.BizTalk.ApplicationDeployment.CommandLine
{
    using Microsoft.BizTalk.ApplicationDeployment;
    using Microsoft.BizTalk.ExplorerOM;
    using Microsoft.BizTalk.Log;
    using System;
    using System.Collections.Specialized;
    using System.IO;

    internal sealed class PartialStopApplicationCommand : Command
    {
        public PartialStopApplicationCommand(NameValueCollection nameValueArgs) : base(nameValueArgs)
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
                string formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.PartialStopApplication, new object[] { applicationName, server, database });
                base.WriteLogEntry(LogEntryType.Information, formattedString);
                explorer = new BtsCatalogExplorer();
                explorer.ConnectionString = ParameterHelper.GetConnectionString(server, database);
                Microsoft.BizTalk.ExplorerOM.Application application = explorer.Applications[applicationName];
                if (application == null)
                {
                    throw new ApplicationException("Unable to find application named " + applicationName);
                }
                application.Stop(Microsoft.BizTalk.ExplorerOM.ApplicationStopOption.DisableAllReceiveLocations);
                explorer.SaveChanges();
                formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.PartialStopApplicationSuccess, new object[] { applicationName });
                base.WriteLogEntry(LogEntryType.Information, formattedString);
                base.commandResult = new CommandResult();
            }
            catch (Exception exception)
            {
                if (explorer != null)
                {
                    try
                    {
                        explorer.DiscardChanges();
                    }
                    catch (Exception)
                    {
                    }
                }
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
                return "PartialStopApp";
            }
        }
    }
}

