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
    internal class TcpServer
    {


        private TcpListener listener; // 1 - TCP слухач
        private CancellationTokenSource cancellationTokenSource; // 2 - токен для зупинки сервера
        private string connectionString = "Data Source=chat.db"; // 3 - Шлях до SQLite бази
       
        // 4 - створення сервера: IP + порт
        public TcpServer(string ip, int port)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
        }

        // 5 - запуск сервера
        public void Start()
        {
            cancellationTokenSource = new();
            Task.Run(() => StartAsync(cancellationTokenSource.Token)); // Запуск сервера у фоновому потоці
        }

        // 6 - основний цикл прийому підключень
        private async Task StartAsync(CancellationToken token)
        {
            listener.Start();
            Console.WriteLine("\nServer is listening...");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync(token); // Приймаємо підключення
                    _ = HandleClientAsync(client, token); // Обробка клієнта в окремому потоці
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server stopped: " + ex.Message);
            }
        }

        // 7 - обробка клієнта (авторизація + обробка повідомлень)
        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            NetworkStream netStream = client.GetStream();
            byte[] myBuffer = new byte[1024];

            // 8 - читаємо логін
            int count1 = await netStream.ReadAsync(myBuffer, 0, myBuffer.Length, token);
            string userName = Encoding.UTF8.GetString(myBuffer, 0, count1).Trim();

            // 9 - читаємо пароль
            int count2 = await netStream.ReadAsync(myBuffer, 0, myBuffer.Length, token);
            string password = Encoding.UTF8.GetString(myBuffer, 0, count2).Trim();

            // 10 - перевірка користувача
            var userInfo = GetUserInfo(userName, password);
            int userId = userInfo.userId;
            bool isAdmin = userInfo.isAdmin;
            bool isBanned = userInfo.isBanned;

            if (userId == -1)
            {
                await SendAsync(netStream, "Authorization failed", token);
                return;
            }

            if (isBanned)
            {
                await SendAsync(netStream, "You are banned", token);
                return;
            }

            await SendAsync(netStream, $"AUTHORIZATION_OK ({(isAdmin ? 1 : 0)})", token);

            // 11 - цикл прийому повідомлень
            while (!token.IsCancellationRequested)
            {
                int count3 = await netStream.ReadAsync(myBuffer, 0, myBuffer.Length, token);
                string recipientName = Encoding.UTF8.GetString(myBuffer, 0, count3).Trim();
                int recipientId = GetUserIdByName(recipientName);

                int count4 = await netStream.ReadAsync(myBuffer, 0, myBuffer.Length, token);
                string message = Encoding.UTF8.GetString(myBuffer, 0, count4).Trim();

                if (recipientId != -1)
                {
                    SaveMessage(userId, recipientId, message);
                    await SendAsync(netStream, "MESSAGE_SAVED", token);
                }
                else
                {
                    await SendAsync(netStream, "USER_NOT_FOUND", token);
                }
            }
        }

        // 12 - відправка повідомлення клієнту
        private async Task SendAsync(NetworkStream stream, string msg, CancellationToken token)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            await stream.WriteAsync(data, 0, data.Length, token);
            await stream.FlushAsync(token);
            Console.WriteLine("Sent: " + msg);
        }

        // 13 - авторизація: отримання ID, isAdmin, isBanned
        public (int userId, bool isAdmin, bool isBanned) GetUserInfo(string username, string password)
        {
            using SQLiteConnection conn = new(connectionString); conn.Open();
            using SQLiteCommand cmd = new("SELECT Id, IsAdmin, IsBanned FROM Users WHERE UserName = @u AND Password = @p", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", password);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                int id = reader.GetInt32(0);
                bool isAdmin = reader.GetInt32(1) == 1;
                bool isBanned = reader.GetInt32(2) == 1;
                return (id, isAdmin, isBanned);
            }

            return (-1, false, false);
        }

        // 14 - отримання ID користувача за логіном
        public int GetUserIdByName(string username)
        {
            using SQLiteConnection conn = new(connectionString); conn.Open();
            using SQLiteCommand cmd = new("SELECT Id FROM Users WHERE UserName = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : -1;
        }

        // 15 - збереження повідомлення
        public void SaveMessage(int senderId, int recipientId, string content)
        {
            using SQLiteConnection conn = new(connectionString); conn.Open();
            using SQLiteCommand cmd = new("INSERT INTO Messages (SenderId, RecipientId, Content) VALUES (@s, @r, @c)", conn);
            cmd.Parameters.AddWithValue("@s", senderId);
            cmd.Parameters.AddWithValue("@r", recipientId);
            cmd.Parameters.AddWithValue("@c", content);
            cmd.ExecuteNonQuery();
        }

        // 16 - отримання повідомлень лише за ID відправника
        public List<string> GetMessagesBySenderId(int senderId)
        {
            var messages = new List<string>();
            using SQLiteConnection conn = new(connectionString); conn.Open();
            using SQLiteCommand cmd = new("SELECT Content FROM Messages WHERE SenderId = @id", conn);
            cmd.Parameters.AddWithValue("@id", senderId);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                messages.Add(reader.GetString(0));
            }

            return messages;
        }

        // 17 - отримання повідомлень за ID відправника і отримувача
        public List<string> GetMessagesBySenderAndRecipient(int senderId, int recipientId)
        {
            var messages = new List<string>();
            using SQLiteConnection conn = new(connectionString); conn.Open();
            using SQLiteCommand cmd = new("SELECT Content FROM Messages WHERE SenderId = @s AND RecipientId = @r", conn);
            cmd.Parameters.AddWithValue("@s", senderId);
            cmd.Parameters.AddWithValue("@r", recipientId);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                messages.Add(reader.GetString(0));
            }

            return messages;
        }

        // 18 - отримання списку всіх користувачів
        public List<string> GetAllUsers()
        {
            var users = new List<string>();
            using SQLiteConnection conn = new(connectionString); conn.Open();
            using SQLiteCommand cmd = new("SELECT UserName FROM Users", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                users.Add(reader.GetString(0));
            }

            return users;
        }

        // 19 - зупинка сервера
        public void Stop()
        {
            cancellationTokenSource?.Cancel();
            listener?.Stop();
            Console.WriteLine("Server stopped.");
        }



    }
}