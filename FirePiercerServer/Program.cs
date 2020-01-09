using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using Konsole;
using sonesson_tools;

namespace FirePiercerServer
{
    class Program
    {
        static void Main(string[] args)
        {
            

            new MainApp();
        }
    }

    public class MainApp
    {
        private Piercer _piercer;
        private Window con;

        public MainApp()
        {
            //var openBox = Window.OpenBox("Bandwidth", 11, 11, 100, 3, new BoxStyle() {ThickNess = LineThickNess.Single, Title = new Colors(ConsoleColor.White, ConsoleColor.Blue)});

            var (width, height) = Window.GetHostWidthHeight.Invoke();

            var parent = new Window(width, height);
            con = new Window(0, 0, width, height-2, parent);
            var bottom = new Window(0, height - 1, width, 1, ConsoleColor.Gray, ConsoleColor.DarkBlue, parent);

            Logger.Instance.LogAdded += (sender, log) =>
            {
                if (log.Severity == Severity.Debug)
                    return;

                if (log.Severity == Severity.Error)
                    con.WriteLine(ConsoleColor.Red, log.ToString());
                else if (log.Severity == Severity.Warning)
                    con.WriteLine(ConsoleColor.Yellow, log.ToString());
                else
                    con.WriteLine(ConsoleColor.Blue, log.ToString());
                
            };

            //bottom.WriteLine("hello");

            con.WriteLine("FirePiercer Server is starting, listing local IP's");
            //Console.WriteLine("FirePiercer Server is starting, listing local IP's");

            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            con.WriteLine(ip.Address.ToString());
                        }
                    }
                }
            }

            _piercer = new Piercer();
            _piercer.StartServer();

            DoStatus();

            while (true)
            {
                Thread.Sleep(1000);
                bottom.PrintAt(0,0, _piercer.Stats.ToString());
                //bottom.WriteLine(_piercer.Stats.ToString());
            }

            //DoMenu();
        }

        private void DoStatus()
        {
            con.WriteLine("********** STATUS **********");
            if (_piercer == null)
                con.WriteLine("PierceServer: Not Started");
            else
                con.WriteLine("PierceServer: Started");
            con.WriteLine("********** ------ **********");
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