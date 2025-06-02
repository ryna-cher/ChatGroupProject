using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Collections.Generic;

namespace ServerChat
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string ip = "127.0.0.1";
            int port = 9000;

            TcpServer server = new TcpServer(ip, port);
            server.Start();

            Console.WriteLine("Press enter to stop the server");
            Console.ReadLine();

            server.Stop();
        }



    }
    
}