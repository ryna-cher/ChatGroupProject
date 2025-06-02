using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientWinForms
{
    public partial class ChatForm : Form
    {

        private TcpClient client; //TCP з’єднання з сервером
        private NetworkStream stream; //потік для відправки повідомлень
        private string username; //нейм користувача


        //1. Конструктор ChatForm. Викликається одразу після входу в LoginForm,
        //запускає прослуховування повідомлення через функцію Task.Run(ReceiveLoopAsync)
        public ChatForm(TcpClient client, NetworkStream stream, string username)
        {
            InitializeComponent();
            this.client = client;
            this.stream = stream;
            this.username = username;

            Task.Run(ReceiveAsync);

            // додані для тесту

            lstUser.Items.Add("Ivan");
            lstUser.Items.Add("Kateryna");
            lstUser.Items.Add("Igor");
            lstUser.Items.Add("Serhii");
            lstUser.Items.Add("Svitlana");
            lstUser.Items.Add("Tatyana");
        }


        //2. Метод btnSendMessage_Click. Виконується при натисканні кнопки Send message. 
        //- перевірка на те, чи обраний юзер, якому відправляють повідомлення
        //- зчитування повідомлення з txtbMessage, і перевірка щоб повідомлення було не порожнє
        //- утворення нового повідомлення через SendAsync
        //- додавання меседжів в історію чату

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (lstUser.SelectedItem == null)
            {
                MessageBox.Show("Choose a user");
                return;
            }

            string receiver = lstUser.SelectedItem.ToString();
            string text = txtbMessage.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Write a message");
                return;
            }

            string message = $"From:{username}To:{receiver} -- {text}";
            await SendAsync(message);


            AddChatHistory($"From:{username} To:{receiver} -- {text}");

            txtbMessage.Clear();
        }

        //3.  Метод SendAsync. Перетворює повідомлення у байти і відправляє їх у потік
        private async Task SendAsync(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
            await stream.WriteAsync(data, 0, data.Length);
        }


        //4. Метод ReceiveAsync. Цикл, який читає повідомлення з сервера в буфер,
        //якщо повідомлення не прийшло, або порожнє, цикл переривається,
        //якщо повідомлення прийшло, перетворює масив байтів у string, 
        //і додає нове повідомлення в історію чату
        private async Task ReceiveAsync()
        {
            try
            {
                byte[] buf = new byte[1024];
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buf, 0, buf.Length);
                    if (bytesRead <= 0) break;

                    string message = Encoding.UTF8.GetString(buf, 0, bytesRead);


                    Invoke(() => AddChatHistory($"{message}"));
                }
            }
            catch (Exception ex)
            {
                Invoke(() => AddChatHistory(ex.Message));
            }
        }

        //5. Метод AddChatHistory. Додає новий меседж в історію чату txtbHistory.
        // ScrollToCaret - означає, що у текст-боксу є автоматичн прокрутка до низу
        private void AddChatHistory(string text)
        {
            txtbHistory.AppendText(text + Environment.NewLine);
            txtbHistory.SelectionStart = txtbHistory.Text.Length;
            txtbHistory.ScrollToCaret();
        }

        //6. Метод ChatForm_FormClosing. Працює при натисканні кнопки [Х] на формі
        // Закриває потік, клієнта, і повністю застосунок
        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                stream?.Close();
                client?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}