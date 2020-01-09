using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using sonesson_tools;

namespace FirePiercerServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Instance.LogAdded += (sender, log) =>
            {
                if (log.Severity == Severity.Debug)
                    return;

                if (log.Severity == Severity.Error)
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (log.Severity == Severity.Warning)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Blue;


                Console.WriteLine(log.ToString());

                Console.ForegroundColor = ConsoleColor.White;
            };

            new MainApp();
        }
    }

    public class MainApp
    {
        private Piercer _piercer;

        public MainApp()
        {
            Console.WriteLine("FirePiercer Server is starting, listing local IP's");

            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            Console.WriteLine(ip.Address.ToString());
                        }
                    }
                }
            }

            _piercer = new Piercer();
            _piercer.StartServer();

            DoStatus();

            var lastline = "";
            while (true)
            {
                Thread.Sleep(1000);
                var stat = _piercer.Stats.ToString();
                if(stat != lastline)
                    Console.WriteLine(stat);
                lastline = stat;
            }

            //DoMenu();
        }

        private void DoStatus()
        {
            Console.WriteLine("********** STATUS **********");
            if (_piercer == null)
                Console.WriteLine("PierceServer: Not Started");
            else
                Console.WriteLine("PierceServer: Started");
            Console.WriteLine("********** ------ **********");
        }

        private void DoMenu()
        {
            Console.WriteLine("********** MENU **********");
            Console.WriteLine();
            Console.WriteLine("\t1 - Start Server");
            Console.WriteLine("\t2 - Start StrumpServer");
            Console.WriteLine("\t3 - Start StrumpEndPoint");
            Console.WriteLine();
            Console.WriteLine("********** ---- **********");
        }
    }
}