using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;

namespace ConsoleApp1
{
    public class ClientGrazyna
    {
        private TcpClient tcpClient;
        private SslStream sslStream;
        private StreamReader streamReader;
        private StreamWriter streamWriter;

        public ClientGrazyna(string HOST, int PORT)
        {
            this.tcpClient = new TcpClient();
            this.tcpClient.Connect(HOST, PORT);

            this.sslStream = new SslStream(this.tcpClient.GetStream());
            this.sslStream.AuthenticateAsClient(HOST);

            this.streamReader = new StreamReader(this.sslStream);
            this.streamWriter = new StreamWriter(this.sslStream);
        }

        public void Write(string message)
        {
            this.streamWriter.WriteLine(message);
            this.streamWriter.Flush();
        }

        public void WriteAndPrintIntoConsole(string message)
        {
            this.Write(message);
            this.PrintIntoConsole();
        }

        public void PrintIntoConsole()
        {
            Console.WriteLine(this.streamReader.ReadLine());
        }

        public string ReadMessage()
        {
            string message = "", temp = "";
            while ((temp = this.streamReader.ReadLine()) != null)
            {
                if (temp == "." || temp.IndexOf("-ERR") != -1)
                {
                    break;
                }

                message += temp + "\n";
            }
            return message;
        }

        public void Close()
        {
            this.tcpClient.Close();
        }
    }
}
