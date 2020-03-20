using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkConfigLoader
{
    interface IProcessor
    {
        void Execute(string username, string password, string configfile);
    }
}
