using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using HenIT.Utilities;
using System.Management;
using BizTalkConfigLoader;

//Credits: Original source and article
//http://grounding.co.za/blogs/romiko/archive/2008/03/24/automating-hosts-host-instances-and-adapter-handlers-configuration-in-biztalk-2006.aspx

namespace BiztalkConfigLoader
{
    public class Program
    {

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                
                System.Console.WriteLine("Please enter parameter values.");
                Console.Read();
            }
            else
            {
                IProcessor process = new Processor();
                string username = Utilities.GetUsername( args[0]);
                string password = Utilities.GetPassword( args[1]);
                string config = Utilities.GetConfig(args[2]);

                process.Execute(username, password, config);
               
#if DEBUG
                System.Threading.Thread.Sleep(5000); //wait a bit to see output in screen
#endif
            }
        }

    }
}