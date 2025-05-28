using System.Net.Sockets;
using System.Text;

namespace ClientWinForms
{
    public partial class LoginForm : Form
    {
        private TcpClient client;  //�ᒺ��, ���� ����������� �� �������
        private NetworkStream stream; //���� ��� ����������� ����������
        public LoginForm()
        {
            InitializeComponent();
        }


        //1. ����� btnLogin_Click. ����������� ��� ��������� ������  "Login"

        //����������� �볺�� �� ����������� �� �������
        //�������� ��������� ����, ��� � ���� ������ �������� ������
        //������� ���� � ������, ����������� ���� � ������ �� ������
        //�������� �� ������� ����
        //���� ���� "user", ����������� ����� ����, ����� ����� ��������

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            try
            {
                await client.ConnectAsync("127.0.0.1", 9000); //��� ���� �� �������� ������
                stream = client.GetStream();

                string login = txtbLogin.Text;
                string password = txtbPassword.Text;

                await SendAsync(login);
                await SendAsync(password);
                string role = await ReceiveAsync();

                if (role == "user")
                {
                    var chat = new ChatForm(client, stream, login);

                    chat.Left = this.Left;  //�� ��� ����� ���������� ������� ����� ���� WinForms
                    chat.Top = this.Top;   // � ���� � ����, �� ���� �� ���� ����� ���� 
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

        //2. ����� SendAsync. ���������� ����������� � ����� � ��������� �� � ����
        private async Task SendAsync(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
            await stream.WriteAsync(data, 0, data.Length);
        }


        //3. ����� ReceiveAsync. ���� � ������ � ���� �� ���������� ����� � string (� ���� �����)
        //leaveOpen: true �������� �� ��������� ���� ���� �������� �����, �� ������� ��� �������������� ������� ��������� ����������
        //���� �� ������ - ������� ������� �����
        private async Task<string> ReceiveAsync()
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            return (await reader.ReadLineAsync())?.Trim() ?? "";
        }





    }
}
