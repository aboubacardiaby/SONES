namespace Microsoft.BizTalk.ApplicationDeployment.CommandLine
{
    using Microsoft.BizTalk.ApplicationDeployment;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    internal class MethodTracer : IDisposable
    {
        private MethodBase currentMethod;
        private DateTime endTime;
        private string separator = new string('-', 60);
        private DateTime startTime;

        public MethodTracer(MethodBase currentMethod)
        {
            this.currentMethod = currentMethod;
            this.startTime = DateTime.Now;
            this.Enter();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.endTime = DateTime.Now;
                this.Exit();
            }
        }

        private void Enter()
        {
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(this.separator, new object[0]);
            string str = this.startTime.ToString("T", CultureInfo.InvariantCulture);
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, ">>> Entering {0}.{1} @ {2}", new object[] { this.currentMethod.DeclaringType.Name, this.currentMethod.Name, str }), new object[0]);
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(string.Empty, new object[0]);
        }

        private void Exit()
        {
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(string.Empty, new object[0]);
            string str = this.endTime.ToString("T", CultureInfo.InvariantCulture);
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "<<< Exiting {0}.{1} @ {2}", new object[] { this.currentMethod.DeclaringType.Name, this.currentMethod.Name, str }), new object[0]);
            TimeSpan span = this.endTime.Subtract(this.startTime);
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "(Elapsed time: {0} seconds)", new object[] { span.TotalSeconds }), new object[0]);
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(this.separator, new object[0]);
        }

        ~MethodTracer()
        {
            this.Dispose(false);
        }

        public static void TraceAssembly()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Assembly: {0}", new object[] { executingAssembly.FullName }), new object[0]);
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Location: {0}", new object[] { executingAssembly.Location }), new object[0]);
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Product: {0} {1}", new object[] { versionInfo.ProductName, versionInfo.ProductVersion }), new object[0]);
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "File: {0} {1}", new object[] { versionInfo.OriginalFilename, versionInfo.FileVersion }), new object[0]);
            //Microsoft.BizTalk.ApplicationDeployment.Trace.WriteLine(string.Empty, new object[0]);
        }
    }
}

