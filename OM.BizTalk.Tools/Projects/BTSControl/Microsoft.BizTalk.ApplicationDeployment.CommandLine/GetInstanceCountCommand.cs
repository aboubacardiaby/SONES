using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.BizTalk.Log;

namespace Microsoft.BizTalk.ApplicationDeployment.CommandLine
{

    internal sealed class GetInstanceCountCommand : Command
    {
        public GetInstanceCountCommand(NameValueCollection nameValueArgs) : base(nameValueArgs)
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
                string formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.GetInstanceCount, new object[] { applicationName, server, database });
                base.WriteLogEntry(LogEntryType.Information, formattedString);
                explorer = new BtsCatalogExplorer();
                explorer.ConnectionString = ParameterHelper.GetConnectionString(server, database);
                Microsoft.BizTalk.ExplorerOM.Application application = explorer.Applications[applicationName];
                if (application == null)
                {
                    throw new ApplicationException("Unable to find application named " + applicationName);
                }

                int instanceCount = getInstanceCount(applicationName, server, database);

                formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.GetInstanceCountSuccess, new object[] { applicationName, instanceCount });
                base.WriteLogEntry(LogEntryType.Information, formattedString);
                base.commandResult = new CommandResult();

                if (instanceCount > 0)
                {
                    base.commandResult.ErrorCount += instanceCount;
                }
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

        private int getInstanceCount(string applicationName, string MgmtDBServerName, string MgmtDBName)
        {
            int instanceCount = 0;

            // Load the Operations assembly that has all the types we need (found in GAC on BizTalk installations)
            Assembly operationsAssembly = Assembly.Load(new AssemblyName("Microsoft.BizTalk.Operations, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"));

            // Create an instance of the OperationsGroup class, passing in the MmgtDBServer and MgmtDBName
            Type operationsGroupType = operationsAssembly.GetType("Microsoft.BizTalk.Operations.OperationsGroup", true);
            Type[] operationsGroupCITypes = new Type[2];
            operationsGroupCITypes[0] = typeof(string);
            operationsGroupCITypes[1] = typeof(string);
            ConstructorInfo operationsGroupCI = operationsGroupType.GetConstructor(operationsGroupCITypes);
            object operationsGroupObject = operationsGroupCI.Invoke(new object[] { MgmtDBServerName, MgmtDBName });

            // Get types for the InstanceStatistic we hope to obtain
            Type instanceStatisticType = operationsAssembly.GetType("Microsoft.BizTalk.Operations.InstanceStatistic", true);
            PropertyInfo instanceStatisticApplicationPI = instanceStatisticType.GetProperty("Application");
            PropertyInfo instanceStatisticCountPI = instanceStatisticType.GetProperty("Count");

            // Get the method we need to invoke on the OperationsGroup object
            Type instanceGroupingCriteriaType = operationsAssembly.GetType("Microsoft.BizTalk.Operations.InstanceGroupingCriteria", true);
            Type[] getInstanceStatsMITypes = new Type[2];
            getInstanceStatsMITypes[0] = instanceGroupingCriteriaType;
            getInstanceStatsMITypes[1] = typeof(Int32);
            MethodInfo getInstanceStatsMI = operationsGroupType.GetMethod("GetInstanceStatistics", getInstanceStatsMITypes);

            // Invoke the method, passing an InstanceGroupingCriteria of 1 (Group by applications) 
            // and a min value of 0 (no limit as to the number of instances queried)
            object[] getInstanceStatsParams = new object[2];
            getInstanceStatsParams[0] = 1;
            getInstanceStatsParams[1] = 0;
            IEnumerable numerable = (IEnumerable)getInstanceStatsMI.Invoke(operationsGroupObject, getInstanceStatsParams);
            foreach (object statisticObj in numerable)
            {
                // We have statistics for an application (we will not have a statisticsObject if there are no instances)
                // but is it the application that we care about?
                string appName = (string)instanceStatisticApplicationPI.GetValue(statisticObj,null);
                if(appName.ToUpperInvariant().Equals(applicationName.ToUpperInvariant()))
                {
                    instanceCount = (int)instanceStatisticCountPI.GetValue(statisticObj, null);
                }
            }

            MethodInfo operationsGroupDisposeMI = operationsGroupType.GetMethod("Dispose");
            operationsGroupDisposeMI.Invoke(operationsGroupObject, null);

            return instanceCount;

            // Code from Microsoft.BizTalk.Administration.SnapIn.Forms.GroupHub.InstancesGrpByStatusStatisticLoader.ExecuteTaskOnAsyncThread()
            // called from Microsoft.BizTalk.Administration.SnapIn.Forms.GroupHub.HubPage.LoadStatistics()
            
            //using (OperationsGroup group = new OperationsGroup(MgmtDBServerName, MgmtDBName))
            //{
            //    foreach (InstanceStatistic statistic in group.GetInstanceStatistics(this.criteria, this.filter, 0))
            //    {
            //        switch (statistic.Filter.InstanceStatus.Value)
            //        {
            //            case InstanceStatus.SuspendedNotResumable:
            //                {
            //                    //this.suspNonResumableCount += statistic.Count;
            //                    continue;
            //                }
            //            case InstanceStatus.Reserved1:
            //                {
            //                    //this.inBreakpointCount += statistic.Count;
            //                    continue;
            //                }
            //            case InstanceStatus.Scheduled:
            //                {
            //                    //this.scheduledCount += statistic.Count;
            //                    continue;
            //                }
            //            case InstanceStatus.ReadyToRun:
            //                {
            //                    //this.readyToRunCount += statistic.Count;
            //                    continue;
            //                }
            //            case InstanceStatus.Active:
            //                {
            //                    //this.activeCount += statistic.Count;
            //                    continue;
            //                }
            //            case InstanceStatus.Suspended:
            //                {
            //                    //this.suspResumableCount += statistic.Count;
            //                    continue;
            //                }
            //            case InstanceStatus.Dehydrated:
            //                break;

            //            default:
            //                throw new InvalidOperationException();
            //        }
            //        if (!statistic.Filter.Class.Set)
            //        {
            //            throw new InvalidOperationException();
            //        }
            //        ServiceClass class2 = statistic.Filter.Class.Value;
            //        if (class2 <= ServiceClass.Messaging)
            //        {
            //            switch (class2)
            //            {
            //                case ServiceClass.Orchestration:
            //                    goto Label_0117;

            //                case ServiceClass.Messaging:
            //                    goto Label_00FE;
            //            }
            //            goto Label_0130;
            //        }
            //        if ((class2 != ServiceClass.MSMQT) && (class2 != ServiceClass.NonCreatableReceiver))
            //        {
            //            goto Label_0130;
            //        }
            //    Label_00FE:
            //        //this.retryingCount += statistic.Count;
            //        continue;
            //    Label_0117:
            //        //this.dehydratedOrchCount += statistic.Count;
            //        continue;
            //    Label_0130:
            //        //throw new InvalidOperationException();
            //        bool stop = true;
            //    }
            //}
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
                return "GetInstanceCount";
            }
        }
    }
}

