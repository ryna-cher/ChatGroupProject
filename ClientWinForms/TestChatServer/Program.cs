using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace TestChatServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
           

            TcpListener listener = new TcpListener(IPAddress.Any, 9000);
            listener.Start();
            Console.WriteLine("Server is working");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Add new user");
                _ = HandleClientAsync(client);
            }
        }

        static async Task HandleClientAsync(TcpClient client)
        {
            using NetworkStream stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            try
            {
                string login = await reader.ReadLineAsync();
                string password = await reader.ReadLineAsync();


                Console.WriteLine($"{login}, {password}");

                Console.WriteLine("Role: user"); 
                await writer.WriteLineAsync("user"); 

                while (true)
                {
                    string msg = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(msg))
                        break;

                    Console.WriteLine($"{login}: {msg}");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}