namespace Microsoft.BizTalk.ApplicationDeployment.CommandLine
{
    using Microsoft.BizTalk.ApplicationDeployment;
    using Microsoft.BizTalk.Log;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Text;

    internal sealed class HelpCommand : Command
    {
        public HelpCommand(NameValueCollection nameValueArgs) : base(nameValueArgs)
        {
        }

        public override void Execute()
        {
            try
            {
                string[] values = base.Args.GetValues((string) null);
                if ((values != null) && (values.Length > 0))
                {
                    string str = values[0];
                    throw new CommandLineArgumentException(CommandResources.GetFormattedString(CommandResources.ResourceID.UnknownCommand, new object[] { str }), null, TraceLevel.Error);
                }
                this.WriteUsage();
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
        }

        protected override CommandLineArgDescriptorList GetParameterDescriptors()
        {
            CommandLineArgDescriptor[] collection = new CommandLineArgDescriptor[0];
            CommandLineArgDescriptorList list = new CommandLineArgDescriptorList();
            list.AddRange(collection);
            return list;
        }

        public override void WriteUsage()
        {
            Console.WriteLine(this.Name + ": " + base.Description);
            Console.WriteLine(string.Empty);
            Console.WriteLine(CommandResources.GetString(CommandResources.ResourceID.Label_Usage));
            Console.WriteLine("  " + CommandResources.GetString(CommandResources.ResourceID.ProgramUsage));
            Console.WriteLine(string.Empty);
            NameValueCollection nameValueArgs = new NameValueCollection();
            List<Command> list = new List<Command>();
            list.Add(new StartApplicationCommand(nameValueArgs));
            list.Add(new PartialStopApplicationCommand(nameValueArgs));
            list.Add(new FullStopApplicationCommand(nameValueArgs));
            list.Add(new RestartHostInstancesCommand(nameValueArgs));
            list.Add(new GetInstanceCountCommand(nameValueArgs));
            StringBuilder builder = new StringBuilder();
            string str4 = CommandResources.GetString(CommandResources.ResourceID.Label_Commands);
            builder.Append(str4);
            foreach (Command command in list)
            {
                builder.Append(Environment.NewLine);
                builder.Append(command.GetSummary());
            }
            Console.WriteLine(builder.ToString());
            Console.WriteLine(string.Empty);
            Console.WriteLine(base.Example);
            Console.WriteLine(string.Empty);
            Console.WriteLine(ConsoleHelper.Wrap(base.Notes, 2, Console.BufferWidth, -2));
            Console.WriteLine(string.Empty);
        }

        public override void WriteUsageHint()
        {
            Console.WriteLine(CommandResources.GetFormattedString(CommandResources.ResourceID.ProgramUsageHint, new object[] { this.Name }));
        }

        public override string Name
        {
            get
            {
                return "BTSControl.exe";
            }
        }
    }
}

