using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

namespace TestThreadPool
{
    class Program
    {
        private const int MaxThreads = 20;
        private const int FilesToProcess = 37;
        private static BackgroundWorker[] threadArray = new BackgroundWorker[MaxThreads];
        private static int Sleepintervall = 0;

        static void Main(string[] args)
        {
            bool RUN = true;
            string input = "";
            InitializeBackgoundWorkers();

            while(RUN == true)
            {
                Console.WriteLine("Wollen Sie den Prozess starten? (Y/N): ");
                input = Console.ReadLine();

                if(input.ToUpper() == "Y" )
                {
                    Console.WriteLine("Sekunden Pause: ");
                    Sleepintervall = Convert.ToInt32(Console.ReadLine());
                    Starting();
                }
                else
                {
                    RUN = false;
                    break;
                }
            }
            
        }



        private static void Starting()
        {
            Stopwatch mywatch = new Stopwatch();
            mywatch.Start();
            for (var f = 0; f < FilesToProcess; f++)
            {
                var fileProcessed = false;
                while (!fileProcessed)
                {
                    for (var threadNum = 0; threadNum < MaxThreads; threadNum++)
                    {
                        if (!threadArray[threadNum].IsBusy)
                        {
                            Console.WriteLine("Starting Thread: {0}", threadNum);

                            threadArray[threadNum].RunWorkerAsync(f);
                            fileProcessed = true;
                            break;
                        }
                    }
                    if (!fileProcessed)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }

            bool Exit = false;
            while (Exit == false)
            {
                bool all = true;
                for (var threadNum = 0; threadNum < MaxThreads; threadNum++)
                {
                    if (threadArray[threadNum].IsBusy)
                    {
                        all = false;
                    }
                }

                if (all == true)
                    Exit = true;
                else Thread.Sleep(1000);
            }

            mywatch.Stop();

            Console.WriteLine("Dauer: " + mywatch.ElapsedMilliseconds.ToString());
            Console.WriteLine("");
            Console.WriteLine("ENDE");
            Console.WriteLine("____________________________________________________");
        }

        private static void InitializeBackgoundWorkers()
        {
            for (var f = 0; f < MaxThreads; f++)
            {
                threadArray[f] = new BackgroundWorker();
                threadArray[f].DoWork += new DoWorkEventHandler(BackgroundWorkerFilesDoWork);
                threadArray[f].RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorkerFilesRunWorkerCompleted);
                threadArray[f].WorkerReportsProgress = true;
                threadArray[f].WorkerSupportsCancellation = true;
            }
        }

        private static void BackgroundWorkerFilesDoWork(object sender, DoWorkEventArgs e)
        {
            ProcessFile((int)e.Argument);

            for(int i = 0; i < 1000; i++)
            {
                Console.WriteLine(e.Argument + ": " + i.ToString());
                Thread.Sleep(Sleepintervall);
            }
                

            e.Result = (int)e.Argument;
        }

        private static void ProcessFile(int file)
        {
            Console.WriteLine("Processing File: {0}", file);
        }

        private static void BackgroundWorkerFilesRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine(e.Error.Message);
            }

            Console.WriteLine("Ending Thread: {0}", (int)e.Result);    
        }


    }
}
