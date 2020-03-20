namespace Microsoft.BizTalk.ApplicationDeployment.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal sealed class CommandResult
    {
        private List<CommandException> commandExceptions;
        private int errorCount;
        private int warningCount;

        public CommandResult()
        {
            this.commandExceptions = new List<CommandException>();
        }

        public CommandResult(Exception exception)
        {
            this.commandExceptions = new List<CommandException>();
            this.commandExceptions = this.GetCommandExceptions(exception);
            this.errorCount = 1;
            this.warningCount = 0;
        }

        public CommandResult(int warnings)
        {
            this.commandExceptions = new List<CommandException>();
            this.warningCount = warnings;
        }

        private List<CommandException> GetCommandExceptions(Exception exception)
        {
            List<CommandException> list = new List<CommandException>();
            if (exception != null)
            {
                CommandException item = new CommandException(exception.Message, exception);
                item.Severity = TraceLevel.Error;
                list.Add(item);
            }
            return list;
        }

        public int ErrorCount
        {
            get
            {
                return this.errorCount;
            }
            set // setter only called by GetInstanceCountCommand
            {
                this.errorCount = value;
            }
        }

        public int WarningCount
        {
            get
            {
                return this.warningCount;
            }
        }
    }
}

