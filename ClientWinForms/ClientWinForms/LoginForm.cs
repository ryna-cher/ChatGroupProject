using System.Net.Sockets;
using System.Text;

namespace ClientWinForms
{
    public partial class LoginForm : Form
    {
        private TcpClient client;  //об’єкт, який підключається до сервера
        private NetworkStream stream; //потік для відправлення повідомлень
        public LoginForm()
        {
            InitializeComponent();
        }


        //1. Метод btnLogin_Click. Викликається при натисканні кнопки  "Login"

        //Створюється клієнт та підключається до сервера
        //Отримуємо мережевий потік, тут у мене обрано локальну мережу
        //Зчитуємо логін і пароль, відправляємо логін і пароль на сервер
        //Отримуємо від сервера роль
        //Якщо роль "user", відкривається форма чату, форма входу ховається

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            try
            {
                await client.ConnectAsync("127.0.0.1", 9000); //тут поки що локальна адреса
                stream = client.GetStream();

                string login = txtbLogin.Text;
                string password = txtbPassword.Text;

                await SendAsync(login);
                await SendAsync(password);
                string role = await ReceiveAsync();

                if (role == "user")
                {
                    var chat = new ChatForm(client, stream, login);

                    chat.Left = this.Left;  //ці два рядка дозволяють відкрити друге вікно WinForms
                    chat.Top = this.Top;   // в тому ж місці, де було до того перше вікно 
                    chat.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Access denied or user not approved.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //2. Метод SendAsync. Перетворює повідомлення у байти і відправляє їх у потік
        private async Task SendAsync(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
            await stream.WriteAsync(data, 0, data.Length);
        }


        //3. Метод ReceiveAsync. Читає з потоку у рідер та перетворює назад у string (в один рядок)
        //leaveOpen: true дозволяє не закривати потік після закриття рідера, це потрібно для багаторазового читання наступних повідомлень
        //якщо не зчитав - повертає порожній рядок
        private async Task<string> ReceiveAsync()
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            return (await reader.ReadLineAsync())?.Trim() ?? "";
        }





    }
}
