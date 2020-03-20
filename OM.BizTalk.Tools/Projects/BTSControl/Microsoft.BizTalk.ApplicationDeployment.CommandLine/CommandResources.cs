namespace Microsoft.BizTalk.ApplicationDeployment.CommandLine
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Resources;
    using System.Text;

    internal static class CommandResources
    {
        private static ResourceManager resourceManager = new ResourceManager(typeof(CommandResources));

        public static string GetFormattedString(ResourceID name, params object[] args)
        {
            Exception exception = null;
            try
            {
                string format = resourceManager.GetString(name.ToString(), CultureInfo.CurrentUICulture);
                return ((format != null) ? string.Format(CultureInfo.CurrentUICulture, format, args) : name.ToString());
            }
            catch (InvalidOperationException exception2)
            {
                exception = exception2;
            }
            catch (MissingManifestResourceException exception3)
            {
                exception = exception3;
            }
            catch (FormatException exception4)
            {
                exception = exception4;
            }
            if (exception == null)
            {
                return name.ToString();
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(name.ToString());
            foreach (object obj2 in ArrayList.Adapter(args))
            {
                builder.Append(" ");
                builder.Append(obj2.ToString());
            }
            return builder.ToString();
        }

        public static string GetString(ResourceID name)
        {
            Exception exception = null;
            try
            {
                return resourceManager.GetString(name.ToString(), CultureInfo.CurrentUICulture);
            }
            catch (InvalidOperationException exception2)
            {
                exception = exception2;
            }
            catch (MissingManifestResourceException exception3)
            {
                exception = exception3;
            }
            return name.ToString();
        }

        public static string GetString(string name)
        {
            Exception exception = null;
            try
            {
                return resourceManager.GetString(name.ToString(), CultureInfo.CurrentUICulture);
            }
            catch (InvalidOperationException exception2)
            {
                exception = exception2;
            }
            catch (MissingManifestResourceException exception3)
            {
                exception = exception3;
            }
            return name.ToString();
        }

        public enum ResourceID
        {
            None,
            ProgramUsage,
            Label_Usage,
            Label_Commands,
            Label_Parameters,
            ProgramUsageHint,
            CommandUsageHint,
            ParamDesc_Server,
            ParamDesc_Database,
            ParamDesc_RequiredApplicationName,
            ParamDesc_ApplicationName,
            ParamDesc_ApplicationName_Req,
            ParamDesc_GroupLevelExport,
            ParamDesc_GroupLevelImport,
            ParamDesc_AssemblyName,
            ParamDesc_GlobalParties,
            ParamDesc_Description,
            ParamDesc_Default,
            ParamDesc_Package,
            ParamDesc_SourceBindings,
            ParamDesc_Type,
            ParamDesc_Luid,
            ParamDesc_OptionalLuid,
            ParamDesc_Source,
            ParamDesc_Destination,
            ParamDesc_DestinationBindings,
            ParamDesc_Options,
            ParamDesc_Property,
            ParamDesc_OverwriteResources,
            ParamDesc_OverwriteResource,
            ParamDesc_ResourceSpec,
            ParamDesc_Environment,
            ParamDesc_EnvironmentFolder,
            ExtraUnnamedArguments,
            ServerInvalid,
            DatabaseInvalid,
            UnableToValidateDatabase,
            DatabaseNotSpecified,
            DefaultServerDatabaseFailed,
            PropertyNameInvalid,
            PropertyExists,
            IncompatibleDbVersion,
            ParameterMissing,
            InvalidValue,
            EmptyResourceSpec,
            OptionSpecifiedTwice,
            StartApplication,
            StartApplicationSuccess,
            PartialStopApplication,
            PartialStopApplicationSuccess,
            FullStopApplication,
            FullStopApplicationSuccess,
            RestartHostInstances,
            RestartHostInstancesSuccess,
            GetInstanceCount,
            GetInstanceCountSuccess,
            ListTypeOutputPreamble,
            CommandSupportsNoParam,
            ListTypeOutputNotes,
            ResourceInvalidInApplication,
            InvalidResources,
            CannotExportSystemApp,
            CannotRemoveResourceFromSystemApp,
            CannotImportPackageIntoSystemApp,
            CannotExportSystemResources,
            ApplicationNotFound,
            ApplicationNotInstalledByBTS,
            UnknownCommand,
            DefaultFlag,
            SystemFlag
        }
    }
}

