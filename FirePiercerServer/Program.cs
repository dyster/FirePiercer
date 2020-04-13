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
                Thread.Sleep(1000);

                var windowWidth = Console.WindowWidth;
                var totalHeight = Console.WindowHeight - 1;

                var statString = _piercer.Stats.ToString();
                var consoleLogs = Program.GetLog();
                var sockConns = _piercer.GetTcpPointStatus().Select(p => new ConsoleLog() { Color = ConsoleColor.Cyan, Log = p }).ToArray();

                var socketSpaceMin = (int)Math.Floor(totalHeight * 0.3);
                var socketSpaceMax = (int)Math.Floor(totalHeight * 0.5);
                var socketSpace = socketSpaceMin;

                if (sockConns.Length > socketSpaceMin)
                {
                    socketSpace = sockConns.Length < socketSpaceMax ? sockConns.Length : socketSpaceMax;
                }

                var statCursor = 0;
                var line1Cursor = 1;
                var socketCursor = 2;
                var line2Cursor = line1Cursor + socketSpace + 1;

                var logSpace = totalHeight - socketSpace - 3;
                var logCursor = line2Cursor + 1;

                
                
                PrintLinePad(statCursor, statString, ConsoleColor.White, windowWidth);
                PrintLinePad(line1Cursor, "------------------------------", ConsoleColor.White, windowWidth);

                if (sockConns.Length > socketSpace)
                {
                    PrintLinePad(line2Cursor, $"------ +{sockConns.Length - socketSpace} sockets ".PadRight(30, '-'), ConsoleColor.White, windowWidth);
                }
                else
                {
                    PrintLinePad(line2Cursor, "------------------------------", ConsoleColor.White, windowWidth);
                }
                

                

                PutListInConsole(sockConns, socketSpace, socketCursor, windowWidth);
                
                

                PutListInConsole(consoleLogs, logSpace, logCursor, windowWidth);
            }

            //DoMenu();
        }

        private void PutListInConsole(ConsoleLog[] consoleLogs, int lines, int startLine, int windowWidth)
        {
            if (consoleLogs.Length > lines)
            {
                var tmp = new ConsoleLog[lines];
                Array.Copy(consoleLogs, consoleLogs.Length - lines, tmp, 0, tmp.Length);
                consoleLogs = tmp;
            }


            for (var index = 0; index < lines; index++)
            {
                var cursor = startLine + index;
                if (index < consoleLogs.Length)
                {
                    var consoleLog = consoleLogs[index];
                    PrintLinePad(cursor, consoleLog.Log, consoleLog.Color, windowWidth);
                }
                else
                {
                    PrintLinePad(cursor, "", ConsoleColor.White, windowWidth);
                }
            }
        }

        private static void PrintLinePad(int line, string text, ConsoleColor colour, int windowWidth)
        {
            Console.SetCursorPosition(0, line);
            Console.ForegroundColor = colour;
            Console.WriteLine(text.PadRight(windowWidth));
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