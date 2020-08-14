using System;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Hotels.ServiceHost
{
    static class Program
    {
        static void Main()
        {
            var service = new HotelsService();
            if (Environment.UserInteractive)
            {
                ConsoleHelper.RunConsole();
                try
                {
                    service.DoStart();
                    Console.Clear();
                    Console.WriteLine("Service is started. Press any key to terminate.");
                    Console.ReadKey();
                }
                finally
                {
                    service.DoStop();
                }
            }
            else
            {
                ServiceBase.Run(service);
            }
        }
    }

    public static class ConsoleHelper
    {
        private const int StdOutputHandle = -11;
        private const int MyCodePage = 437;

        [DllImport("kernel32.dll",
            EntryPoint = "GetStdHandle",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        public static void RunConsole()
        {
            AllocConsole();
            var stdHandle = GetStdHandle(StdOutputHandle);
            var safeFileHandle = new SafeFileHandle(stdHandle, true);
            var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            var encoding = Encoding.GetEncoding(MyCodePage);
            var standardOutput = new StreamWriter(fileStream, encoding) { AutoFlush = true };
            Console.SetOut(standardOutput);
        }
    }
}
