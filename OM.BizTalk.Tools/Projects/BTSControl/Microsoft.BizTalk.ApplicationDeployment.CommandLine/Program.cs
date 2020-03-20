namespace Microsoft.BizTalk.ApplicationDeployment.CommandLine
{
    using Microsoft.BizTalk.ApplicationDeployment;
    //using Microsoft.BizTalk.Internal;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    internal sealed class Program
    {
        private int errors;
        private int warnings;

        public Program()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Program.Console_CancelKeyPress);
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            using (new Microsoft.BizTalk.ApplicationDeployment.CommandLine.MethodTracer(MethodBase.GetCurrentMethod()))
            {
                string format = Microsoft.BizTalk.ApplicationDeployment.CommandLine.StringResources.GetString(Microsoft.BizTalk.ApplicationDeployment.CommandLine.StringResources.ResourceID.CancelKeyPress);
                using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(TraceLevel.Warning)))
                {
                    //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(format, new object[0]);
                    Console.WriteLine(format);
                }
            }
        }

        public static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.Program_UnhandledException);
            using (new Microsoft.BizTalk.ApplicationDeployment.CommandLine.MethodTracer(MethodBase.GetCurrentMethod()))
            {
                Microsoft.BizTalk.ApplicationDeployment.CommandLine.MethodTracer.TraceAssembly();
                Program program = new Program();
                return program.Run(args);
            }
        }

        private static void Program_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //new WatsonReportGenerator().WatsonExceptionHandler(sender, e);
            if (!e.IsTerminating)
            {
                Exception exceptionObject = e.ExceptionObject as Exception;
                if (exceptionObject != null)
                {
                    string format = "FATAL ERROR: An unhandled exception occurred in a thread pool or finalizer thread.";
                    //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(format, new object[0]);
                    Console.WriteLine(format);
                    //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(exceptionObject.ToString(), new object[0]);
                    Console.WriteLine(exceptionObject.ToString());
                }
            }
            else
            {
                string str2 = "FATAL ERROR: An unhandled exception occurred in a managed thread.";
                //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(str2, new object[0]);
                Console.WriteLine(str2);
            }
        }

        private int Run(string[] args)
        {
            try
            {
                NameValueCollection nameValueArgs = CommandLineParser.Parse(args);
                WriteHeader();
                CommandLineHelper.TraceWriteArguments(args, nameValueArgs);
                if (CommandLineHelper.ArgSpecified(nameValueArgs, "Debug"))
                {
                    nameValueArgs.Remove("Debug");
                    CommandLineHelper.ConsoleWriteArguments(nameValueArgs);
                }
                Command command = CommandFactory.Create(nameValueArgs);
                bool flag2 = CommandLineHelper.ArgSpecified(nameValueArgs, "?") || CommandLineHelper.ArgSpecified(nameValueArgs, "Help");
                if ((nameValueArgs.Count == 0) || flag2)
                {
                    using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(TraceLevel.Info)))
                    {
                        //if (command is AddResourceCommand)
                        //{
                        //    command.ValidateArgs();
                        //}
                        command.WriteUsage();
                        goto Label_02F4;
                    }
                }
                if (command is HelpCommand)
                {
                    command.Execute();
                    this.errors += command.Result.ErrorCount;
                    this.warnings += command.Result.WarningCount;
                    this.WriteCommandSummary();
                    return this.errors;
                }
                List<CommandLineArgumentException> list = command.ValidateArgs();
                if (list.Count > 0)
                {
                    foreach (CommandLineArgumentException exception in list)
                    {
                        this.errors += (exception.Severity == TraceLevel.Error) ? 1 : 0;
                        this.warnings += (exception.Severity == TraceLevel.Warning) ? 1 : 0;
                        using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(exception.Severity)))
                        {
                            Console.WriteLine(exception.Severity.ToString() + ": " + exception.Message);
                            Console.WriteLine(string.Empty);
                            continue;
                        }
                    }
                }
                if (this.errors == 0)
                {
                    command.Execute();
                    this.errors += command.Result.ErrorCount;
                    this.warnings += command.Result.WarningCount;
                    this.WriteCommandSummary();
                }
                else
                {
                    using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(TraceLevel.Info)))
                    {
                        command.WriteUsageHint();
                    }
                }
            }
            catch (Exception exception2)
            {
                //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(exception2.ToString(), new object[0]);
                this.errors++;
                using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(TraceLevel.Error)))
                {
                    Console.WriteLine(exception2.Message);
                }
                if ((exception2 is OutOfMemoryException) || /*(exception2 is ExecutionEngineException)) ||*/ (exception2 is StackOverflowException))
                {
                    throw;
                }
            }
            finally
            {
                string formattedString = Microsoft.BizTalk.ApplicationDeployment.CommandLine.StringResources.GetFormattedString(Microsoft.BizTalk.ApplicationDeployment.CommandLine.StringResources.ResourceID.CommandCompleted, new object[] { this.errors.ToString(CultureInfo.InvariantCulture), this.warnings.ToString(CultureInfo.InvariantCulture) });
                //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(string.Empty, new object[0]);
                //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(formattedString, new object[0]);
            }
        Label_02F4:
            return this.errors;
        }

        private void WriteCommandSummary()
        {
            string formattedString;
            if (this.errors > 0)
            {
                formattedString = Microsoft.BizTalk.ApplicationDeployment.CommandLine.StringResources.GetFormattedString(Microsoft.BizTalk.ApplicationDeployment.CommandLine.StringResources.ResourceID.CommandResultFailed, new object[] { this.errors.ToString(CultureInfo.InvariantCulture), this.warnings.ToString(CultureInfo.InvariantCulture) });
            }
            else
            {
                formattedString = Microsoft.BizTalk.ApplicationDeployment.CommandLine.StringResources.GetFormattedString(Microsoft.BizTalk.ApplicationDeployment.CommandLine.StringResources.ResourceID.CommandResultSucceeded, new object[] { this.errors.ToString(CultureInfo.InvariantCulture), this.warnings.ToString(CultureInfo.InvariantCulture) });
            }
            using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(TraceLevel.Info)))
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine(formattedString);
                Console.WriteLine(string.Empty);
            }
        }

        private static void WriteHeader()
        {
            using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(TraceLevel.Info)))
            {
                string fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                string formattedString = Microsoft.BizTalk.ApplicationDeployment.CommandLine.StringResources.GetFormattedString(Microsoft.BizTalk.ApplicationDeployment.CommandLine.StringResources.ResourceID.HeaderDescription, new object[] { fileVersion });
                string str3 = Microsoft.BizTalk.ApplicationDeployment.CommandLine.StringResources.GetString(Microsoft.BizTalk.ApplicationDeployment.CommandLine.StringResources.ResourceID.HeaderCopyright);
                Console.WriteLine(formattedString);
                Console.WriteLine(str3);
                Console.WriteLine(string.Empty);
            }
        }
    }
}

