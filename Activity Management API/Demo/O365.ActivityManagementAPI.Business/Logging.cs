using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O365.ActivityManagementAPI.Business
{
    public class Logging
    {
        public static void LogMessage (String message)
        {
            Console.WriteLine("[{0}] {1}", DateTime.Now, message);
        }
    }
}
