using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console
{
    public static class Messenger
    {
        public static void Output(string message, ConsoleColor color)
        {
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(message);
            System.Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        public static void InfoBold(string message)
        {
            Output(message, ConsoleColor.White);
        }

        public static void Info(string message)
        {
            Output(message, ConsoleColor.DarkGray);
        }

        public static void Success(string message)
        {
            Output(message, ConsoleColor.Green);
        }

        public static void Attention(string message)
        {
            System.Console.WriteLine();
            Output(message, ConsoleColor.Cyan);
        }
    }
}
