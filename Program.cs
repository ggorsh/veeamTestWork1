using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace monitor
{
    class Program
    {

        static void Main(string[] args)
        {
            int lifeTime = 0;
            int checkingTime = 0;
            String processName = "";
            try
            {
                if (!args.Any())
                    throw new ArgumentException("Parameters are not provided. Type [help] for help");
                if (args[0] == "help")
                {
                    Console.WriteLine("monitor.exe [p] [l] [t] \n [p] - process name \n [l] - process life time in minuts \n [t] - checking period in minuts");
                    Environment.Exit(1);
                }
                if (args.Length == 3)
                {
                    processName = args[0];
                    if (!Int32.TryParse(args[1], out lifeTime)) throw new ArgumentException("[l] process life time must be integer");
                    if (!Int32.TryParse(args[2], out checkingTime)) throw new ArgumentException("[t] cheking time must be integer");
                }
                else throw new ArgumentException("Wrong parametrs.Type [help] for help");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            Console.WriteLine("Monitoring process name : {0} / Life time : {1} min. / Period of checking : {2} min.", processName, lifeTime, checkingTime);
            Console.WriteLine("For Exit press [Esc]");
            Console.WriteLine("{0,-12}{1,-15}{2,-17}{3,-10}", "Id", "Process Name", "Life time(min)", "Status");

            Thread chekingThread = new Thread(() => Checking(processName, lifeTime, checkingTime));
            chekingThread.Start();

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape) Environment.Exit(1);
            }
        }

        public static void Checking(String processName, int lifeTime, int checkingTime)
        {
            while (true)
            {
                Process[] processes = Process.GetProcessesByName(processName);

                foreach (Process process in processes)
                {
                    int currentLifeTime = (int)((DateTime.Now - process.StartTime).TotalMinutes);

                    Console.Write("{0,-12}{1,-15}{2,-17}", process.Id, process.ProcessName, currentLifeTime);
                    if (currentLifeTime >= lifeTime)
                    {
                        process.Kill();
                        Console.WriteLine("killed");
                    }
                    else Console.WriteLine();
                }
                Thread.Sleep(checkingTime * 60000);
            }
        }
    }
}
