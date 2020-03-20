namespace Microsoft.BizTalk.ApplicationDeployment.CommandLine
{
    using System;
    using System.Collections.Specialized;

    internal static class CommandFactory
    {
        public static Command Create(NameValueCollection args)
        {
            if (args == null)
            {
                args = new NameValueCollection();
            }
            string str = null;
            string[] values = args.GetValues((string) null);
            if ((values != null) && (values.Length > 0))
            {
                str = values[0].ToUpperInvariant();
            }
            switch (str)
            {
                case "STARTAPP":
                    return new StartApplicationCommand(args);

                case "PARTIALSTOPAPP":
                    return new PartialStopApplicationCommand(args);

                case "FULLSTOPAPP":
                    return new FullStopApplicationCommand(args);

                case "RESTARTHOSTINSTANCES":
                    return new RestartHostInstancesCommand(args);

                case "GETINSTANCECOUNT":
                    return new GetInstanceCountCommand(args);
            }
            return new HelpCommand(args);
        }
    }
}

