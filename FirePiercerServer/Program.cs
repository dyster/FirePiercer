using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using sonesson_tools;
using sonesson_tools.Strump;

namespace FirePiercerServer
{
    class Program
    {
        private static ConcurrentQueue<ConsoleLog> Log = new ConcurrentQueue<ConsoleLog>();
        private static int LogLimit = 20;

        public static ConcurrentDictionary<uint, TcpPoint> SockConnections = new ConcurrentDictionary<uint, TcpPoint>();
        static void Main(string[] args)
        {
            
            Logger.Instance.LogAdded += (sender, log) =>
            {
                if (log.Severity == Severity.Debug)
                    return;

                if (log.Severity == Severity.Error)
                    AddLog(ConsoleColor.Red, log.ToString());
                else if (log.Severity == Severity.Warning)
                    AddLog(ConsoleColor.Yellow, log.ToString());
                else
                    AddLog(ConsoleColor.Cyan, log.ToString());


               

               
            };
            
            new MainApp();
        }

        public static void AddLog(ConsoleColor colour, string text)
        {
            Log.Enqueue(new ConsoleLog(){Color = colour, Log = text});
            while (Log.Count > LogLimit)
            {
                Log.TryDequeue(out var result);
            }
        }

        public static ConsoleLog[] GetLog()
        {
            return Log.ToArray();
        }

        public static void SetSockConnections(List<TcpPoint> list)
        {
            SockConnections.Clear();
            foreach (var sockConnectionBase in list)
            {
                SockConnections.TryAdd(sockConnectionBase.UniqueId, sockConnectionBase);
            }
        }
    }

    public struct ConsoleLog
    {
        public ConsoleColor Color;
        public string Log;
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
                var windowWidth = Console.WindowWidth;

                void PrintLinePad(int i, string stat1, ConsoleColor colour)
                {
                    Console.SetCursorPosition(0, i);
                    Console.ForegroundColor = colour;
                    Console.WriteLine(stat1.PadRight(windowWidth));
                }

                void PutListInConsole(ConsoleLog[] consoleLogs1, int logSpace1, int logCursor1)
                {
                    if (consoleLogs1.Length > logSpace1)
                    {
                        var tmp = new ConsoleLog[logSpace1];
                        Array.Copy(consoleLogs1, consoleLogs1.Length - logSpace1, tmp, 0, tmp.Length);
                        consoleLogs1 = tmp;
                    }


                    for (var index = 0; index < logSpace1; index++)
                    {
                        var cursor = logCursor1 + index;
                        if (index < consoleLogs1.Length)
                        {
                            var consoleLog = consoleLogs1[index];
                            PrintLinePad(cursor, consoleLog.Log, consoleLog.Color);
                        }
                        else
                        {
                            PrintLinePad(cursor, "", ConsoleColor.White);
                        }
                    }
                }

                Thread.Sleep(1000);
                var totalHeight = Console.WindowHeight - 1;

                var socketSpace = (int)Math.Floor(totalHeight * 0.3);

                var statCursor = 0;
                var line1Cursor = 1;
                var socketCursor = 2;
                var line2Cursor = line1Cursor + socketSpace + 1;

                var logSpace = totalHeight - socketSpace - 3;
                var logCursor = line2Cursor + 1;

                //Console.Clear();

                var stat = _piercer.Stats.ToString();

                //if (stat != lastline)
                //{
                    PrintLinePad(statCursor, stat, ConsoleColor.White);
                    //}
                lastline = stat;

                
                PrintLinePad(line1Cursor, "------------------------------", ConsoleColor.White);
                PrintLinePad(line2Cursor, "------------------------------", ConsoleColor.White);


                var sockConns = Program.SockConnections.ToArray().Select(s => new ConsoleLog(){Color = ConsoleColor.Cyan, Log = s.Value.ToString()}).ToArray();

                PutListInConsole(sockConns, socketSpace, socketCursor);
                
                var consoleLogs = Program.GetLog();

                PutListInConsole(consoleLogs, logSpace, logCursor);
            }

            //DoMenu();
        }

        private void DoStatus()
        {
            Console.WriteLine("********** STATUS **********");
            if (_piercer == null)
                Console.WriteLine("PierceServer: Not Started");
            else
            {
                Console.WriteLine("PierceServer: Started"); ;
            }
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