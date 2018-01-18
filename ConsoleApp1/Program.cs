using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Configuration;



namespace ConsoleApp1
{
    class Program
    {
        private static string GetConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static List<string> ReadMailbox(string HOST, int PORT, string USER, string PASS, string strTemp, List<string> UIDL)
        {
            var client = new ClientGrazyna(HOST, PORT);

            client.WriteAndPrintIntoConsole("USER " + USER);
            client.WriteAndPrintIntoConsole("PASS " + PASS);

            client.Write("LIST");
            var responseMultiline = client.ReadMessage().Split('\n');

            foreach (var line in responseMultiline)
            {
                string[] strarr = line.Split(' ');
                Console.WriteLine(strarr[0]);
                if (strarr.Length == 2)
                {
                    UIDL.Add(strarr[1]);
                    Console.WriteLine(strarr[1]);
                }
            }
            return UIDL;
        }

        public static List<string> CheckMailbox(string HOST, int PORT, string USER, string PASS, string strTemp, List<string> UIDL, int counter)
        {
            var client = new ClientGrazyna(HOST, PORT);


            client.WriteAndPrintIntoConsole("USER " + USER);
            client.WriteAndPrintIntoConsole("PASS " + PASS);

            client.Write("LIST");
            var responseMultiline = client.ReadMessage().Split('\n');

            foreach (var line in responseMultiline)
            {
                UIDL.Insert(0, counter.ToString());
                string[] strarr = line.Split(' ');
                if (strarr.Length == 2)
                {
                    UIDL.Remove(counter.ToString());
                    if (!UIDL.Contains(strarr[1]))
                    {
                        UIDL.Add(strarr[1]);
                        Console.WriteLine("Check your mailbox.");
                        counter++;
                        UIDL.Insert(0, counter.ToString());
                    }

                }
            }
            return UIDL;
        }

        private static void RefreshMailbox(string HOST, int PORT, string USER, string PASS, string strTemp, List<string> UIDL, int count)
        {
            using (TcpClient tc = new TcpClient())
            {
                tc.Connect(HOST, PORT);
                using (SslStream sl = new SslStream(tc.GetStream()))
                {
                    sl.AuthenticateAsClient(HOST);

                    CheckMailbox(HOST, PORT, USER, PASS, strTemp, UIDL, count);
                }
                tc.Close();
            }
        }


        static void Main(string[] args)
        {
            string HOST = GetConfig("HOST");
            string USER = GetConfig("USER");
            int PORT = Int32.Parse(GetConfig("PORT"));
            string PASS = GetConfig("PASS");
            int TIME = Int32.Parse(GetConfig("TIME"));
            int counter = 0;

            string str = string.Empty;
            string strTemp = string.Empty;
            List<string> UIDL = new List<string>();

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(TIME);

            ReadMailbox(HOST, PORT, USER, PASS, strTemp, UIDL);

            using (var timer = new System.Threading.Timer((e) =>
             {
                 RefreshMailbox(HOST, PORT, USER, PASS, strTemp, UIDL, counter);
             }, null, startTimeSpan, periodTimeSpan)) { Console.ReadKey(); }

            //timer.Dispose();

            Console.WriteLine(String.Format("\nIn this session you recived {0} emails.", UIDL[0]));
            Console.ReadKey();

        }


    }

}


