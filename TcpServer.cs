using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace ServerChat
{
    internal class TcpServer
    {
        private TcpListener listener; // 1 - TCP слухач
        private CancellationTokenSource cancellationTokenSource; // 2 - токен для зупинки

        private string connectionString = ""; // 3 - шлях до SQLite

        // 4 - створення сервера: IP + порт
        public TcpServer(string ip, int port)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
        }

        // 5 - запуск сервера
        public void Start()
        {
            cancellationTokenSource = new();
            Task.Run(() => StartAsync(cancellationTokenSource.Token));
        }

        // 6 - прийом клієнтів
        private async Task StartAsync(CancellationToken token)
        {
            listener.Start();
            Console.WriteLine("Server is listening...");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync(token);
                    _ = HandleClientAsync(client, token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server stopped: " + ex.Message);
            }
        }

        // 7 - обробка одного клієнта — авторизація + прийом повідомлень і запис у БД
        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            // з'єднання  з юзером
            // Прочитати логін (рядок)
            // Прочитати пароль (рядок)

            //перевірити, чи існує користувач із такими логіном і паролем

            // Якщо існує — надіслати "роль" (user або admin)

            // Далі, у циклі:
            //    Приймати повідомлення до кого і меседж

            //    Перевірити, чи існує одержувач

            //    Якщо так — зберегти повідомлення в базу 

        }

        // 10 - відправка повідомлень клієнту
        private async Task SendAsync(NetworkStream stream, string msg, CancellationToken token)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            await stream.WriteAsync(data, 0, data.Length, token);
            await stream.FlushAsync(token);
            Console.WriteLine("Sent: " + msg);

        }

        // 11 - отримання ID користувача за логіном і паролем
        private int GetUserId(string username, string password)
        {
            // Встановити з'єднання з SQLite базою
            // Виконати SELECT Id FROM Users WHERE UserName = username AND PasswordHash = password
            // Перевірити, чи користувач знайдений (і він підтверджений, не забанений)
            // Якщо знайдений — повернути його ID
            // Якщо ні — повернути -1
        }

        // 12 - отримання ID користувача за логіном
        private int GetUserIdByName(string username)
        {
            // Встановити з'єднання з SQLite
            // Виконати SELECT Id FROM Users WHERE UserName = username
            // Якщо знайдений — повернути ID
            // Якщо ні — повернути -1
        }


        // 13 - збереження повідомлення у Messages
        private void SaveMessage(int senderId, int recipientId, string content)
        {
            // Встановити з'єднання з SQLite

            // Додати запис у Messages (SenderId, Content)

            // Додати запис у MessageRecipients (MessageId, RecipientId)

        }

        // 14 - зупинка сервера
        public void Stop()
        {
            cancellationTokenSource?.Cancel();
            listener?.Stop();
            Console.WriteLine("Server stopped.");
        }
    }
}
