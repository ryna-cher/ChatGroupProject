
/*

1. АВТОРИЗАЦІЯ
 "Type": "LOGIN", "Content": "user1" 
 "Type": "PASSWORD", "Content": "1234" 

Відповідь сервера:
 "Type": "AUTH", "Content": "OK 0"  - 0 - звичайний користувач, 1 - адмін
 "Type": "ERROR", "Content": "Authorization failed" 
 "Type": "ERROR", "Content": "You are banned" 


2. СПИСОК КОРИСТУВАЧІВ

 Клієнт:
 "Type": "GET_USERS", "Content": "" 

 Відповідь:
 "Type": "USERS", "Content"


3. НАДСИЛАННЯ ПОВІДОМЛЕННЯ

 Клієнт:
 "Type": "MESSAGE", "Content": "user2::Привіт!" 

Відповідь сервера:
 "Type": "STATUS", "Content": "MESSAGE_SAVED" 
 "Type": "STATUS", "Content": "USER_NOT_FOUND"

Отримувач
 "Type": "INCOMING", "Content": "user1::Привіт!" 


4. ІСТОРІЯ ПОВІДОМЛЕНЬ З КОРИСТУВАЧЕМ

Клієнт:
"Type": "GET_HISTORY", "Content": "user2" 

Відповідь:
 "Type": "HISTORY","Content" 

⬇ Якщо помилка:
 "Type": "STATUS", "Content": "USER_NOT_FOUND" 

*/


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
        private readonly Dictionary<int, NetworkStream> activeClients = new(); // 4 - активні клієнти

        // 5 - створення сервера: IP + порт
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

        // 7 - обробка клієнта
        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            var netStream = client.GetStream();
            byte[] buffer = new byte[2048];
            int userId = -1;

            try
            {
                // авторизація: читаємо логін і пароль
                string loginJson = await ReadJsonAsync(netStream, token);
                var loginDto = JsonSerializer.Deserialize<MessageDTO>(loginJson);

                string passJson = await ReadJsonAsync(netStream, token);
                var passDto = JsonSerializer.Deserialize<MessageDTO>(passJson);

                string userName = loginDto?.Content;
                string password = passDto?.Content;

                var userInfo = GetUserInfo(userName, password); // отримуємо інфу про користувача
                userId = userInfo.userId;

                if (userId == -1)
                {
                    await SendJsonAsync(netStream, new MessageDTO { Type = "ERROR", Content = "Authorization failed" }, token);
                    return;
                }

                if (userInfo.isBanned)
                {
                    await SendJsonAsync(netStream, new MessageDTO { Type = "ERROR", Content = "You are banned" }, token);
                    return;
                }

                // додаємо клієнта в активних
                activeClients[userId] = netStream;

                await SendJsonAsync(netStream, new MessageDTO { Type = "AUTH", Content = $"OK {(userInfo.isAdmin ? 1 : 0)}" }, token);


                // головний цикл прийому повідомлень від клієнта
                while (!token.IsCancellationRequested)
                {
                    int len = await netStream.ReadAsync(buffer, 0, buffer.Length, token);
                    string json = Encoding.UTF8.GetString(buffer, 0, len);
                    var dto = JsonSerializer.Deserialize<MessageDTO>(json);

                    if (dto.Type == "MESSAGE") // надсилання повідомлення іншому користувачу
                    {
                        string[] parts = dto.Content.Split("::");
                        string recipientName = parts[0];
                        string messageText = parts[1];

                        int recipientId = GetUserIdByName(recipientName);
                        if (recipientId != -1)
                        {
                            SaveMessage(userId, recipientId, messageText); // зберігаємо в БД
                            await SendJsonAsync(netStream, new MessageDTO { Type = "STATUS", Content = "MESSAGE_SAVED" }, token);

                            if (activeClients.TryGetValue(recipientId, out var recipientStream))
                            {
                                var forwardMessage = new MessageDTO
                                {
                                    Type = "INCOMING",
                                    Content = userName // ім’я відправника
                                };
                                await SendJsonAsync(recipientStream, forwardMessage, token);
                            }
                        }
                        else
                        {
                            await SendJsonAsync(netStream, new MessageDTO { Type = "STATUS", Content = "USER_NOT_FOUND" }, token);
                        }
                    }
                    else if (dto.Type == "HISTORY")
                    {
                        int targetId = GetUserIdByName(dto.Content);
                        if (targetId != -1)
                        {
                            var messages = GetMessagesBetweenUsers(userId, targetId);
                            await SendJsonAsync(netStream, new MessageDTO
                            {
                                Type = "HISTORY",
                                Content = JsonSerializer.Serialize(messages, new JsonSerializerOptions
                                {
                                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                })
                            }, token);
                        }
                        else
                        {
                            await SendJsonAsync(netStream, new MessageDTO
                            {
                                Type = "STATUS",
                                Content = "USER_NOT_FOUND"
                            }, token);
                        }
                    }
                    else if (dto.Type == "GET_USERS")
                    {
                        var usersAll = GetAllUsers();
                        await SendJsonAsync(netStream, new MessageDTO
                        {
                            Type = "USERS",
                            Content = JsonSerializer.Serialize(usersAll)
                        }, token);
                    }
                    else if (dto.Type == "GET_HISTORY")
                    {
                        int targetId = GetUserIdByName(dto.Content);
                        if (targetId != -1)
                        {
                            var messages = GetMessagesBetweenUsers(userId, targetId);
                            await SendJsonAsync(netStream, new MessageDTO
                            {
                                Type = "HISTORY",
                                Content = JsonSerializer.Serialize(messages, new JsonSerializerOptions
                                {
                                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                })
                            }, token);
                        }
                        else
                        {
                            await SendJsonAsync(netStream, new MessageDTO
                            {
                                Type = "STATUS",
                                Content = "USER_NOT_FOUND"
                            }, token);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Клієнтський потік завершено: " + ex.Message);
            }
            finally
            {
                if (userId != -1)
                    activeClients.Remove(userId); // видаляємо користувача зі списку активних клієнтів
                try { netStream?.Close(); } catch { }
                try { client?.Close(); } catch { }
            }
        }
            catch (Exception ex)
            {
               
                Console.WriteLine("Клієнтський потік завершено: " + ex.Message);
            }
            finally
            {
                // Видаляємо користувача зі списку активних клієнтів, закриваємо з’єднання
                if (userId != -1)
                    activeClients.Remove(userId);

                try { netStream?.Close(); } catch { }
                try { client?.Close(); } catch { }
            }
        }
            // Отримання історії повідомлень між двома користувачами
        public List<string> GetMessagesBetweenUsers(int user1Id, int user2Id)
        {
            var messages = new List<string>();

            using SQLiteConnection conn = new(connectionString); conn.Open();
            using SQLiteCommand cmd = new(@"
                SELECT SenderId, RecipientId, Content, Timestamp 
                FROM Messages 
                WHERE (SenderId = @u1 AND RecipientId = @u2)
                   OR (SenderId = @u2 AND RecipientId = @u1)
                ORDER BY Id", conn);

            cmd.Parameters.AddWithValue("@u1", user1Id);
            cmd.Parameters.AddWithValue("@u2", user2Id);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int senderId = reader.GetInt32(0);
                int recipientId = reader.GetInt32(1);
                string content = reader.GetString(2);
                string timestamp = reader.GetValue(3).ToString(); 

                string direction = senderId == user1Id
                    ? $"\ud83d\udce4 Відправлено: {timestamp} → {GetUserNameById(recipientId)}: {content}"
                    : $"\ud83d\udce5 Отримано: {timestamp} ← {GetUserNameById(senderId)}: {content}";

                messages.Add(direction);
            }

            return messages;
        }

            //читання JSON 
        private async Task<string> ReadJsonAsync(NetworkStream stream, CancellationToken token)
        {
            using MemoryStream ms = new();
            byte[] buffer = new byte[512];

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                if (bytesRead == 0)
                    break;

                ms.Write(buffer, 0, bytesRead);

                try
                {
                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    JsonDocument.Parse(json); 
                    return json;
                }
                catch (JsonException)
                {
                    continue; 
                }
            }

            throw new Exception("Failed to read complete JSON.");
        }


        // 12 - відправка повідомлення клієнту
        private async Task SendJsonAsync(NetworkStream stream, MessageDTO dto, CancellationToken token)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = JsonSerializer.Serialize(dto, options) + "\n";
            byte[] data = Encoding.UTF8.GetBytes(json);
            await stream.WriteAsync(data, 0, data.Length, token);
            await stream.FlushAsync(token);
        }

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
            try
            {
                using SQLiteConnection conn = new(connectionString); conn.Open();
                using SQLiteCommand cmd = new("INSERT INTO Messages (SenderId, RecipientId, Content, Timestamp) VALUES (@s, @r, @c, @t)", conn);
                cmd.Parameters.AddWithValue("@s", senderId);
                cmd.Parameters.AddWithValue("@r", recipientId);
                cmd.Parameters.AddWithValue("@c", content);
                cmd.Parameters.AddWithValue("@t", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ПОМИЛКА збереження: " + ex.Message);
            }
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
                users.Add(reader.GetString(0));

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
