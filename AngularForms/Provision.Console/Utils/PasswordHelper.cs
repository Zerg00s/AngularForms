using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Utils
{
    public class PasswordHelper
    {
        public static SecureString GetPasswordFromConsoleInput()
        {
            ConsoleColor defaultForeground = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Enter your password.");
            System.Console.ForegroundColor = defaultForeground;

            ConsoleKeyInfo info;

            //Get the user's password as a SecureString
            SecureString securePassword = new SecureString();
            do
            {
                info = System.Console.ReadKey(true);
				System.Console.Write("*");
				if (info.Key != ConsoleKey.Enter)
                {
                    securePassword.AppendChar(info.KeyChar);
                }
            }
            while (info.Key != ConsoleKey.Enter);
            return securePassword;
        }
    }
}
