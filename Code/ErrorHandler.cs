using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISSetup.Code
{
    public class ErrorHandler
    {
        public static void WriteError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.InnerException.ToString());
            Console.ResetColor();

        }

    }
}
