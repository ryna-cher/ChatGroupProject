using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace admin.c
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private TcpClient admin;
        private NetworkStream str;
       
        
        private async void connect_Click(object sender, EventArgs e)
        {
            try
            {
                admin = new TcpClient();
                string l = pass.Text;
                string p = login.Text;
                await admin.ConnectAsync("127.0.0.1",9000);
                 str = admin.GetStream();

                string msg = $"{l}, {p}|admin ";
                await SendCommand(msg);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void add_user_Click(object sender, EventArgs e)
        {
            try
            {
                string u = new_user_pass.Text;
                string l = new_user_login.Text;
                str = admin.GetStream();
                string c = $"add_user|{u}|{l}";
                await SendCommand(c);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void block_user_Click(object sender, EventArgs e)
        {
            try {
                string u = blcok_user_name.Text.Trim();

                string l = block_user_pass.Text.Trim();
                str = admin.GetStream();
                string c = $"block_user|{u}|{l}";

                await SendCommand(c);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            }

        private async Task SendCommand(string command)
        {
            if (admin == null || !admin.Connected)
            {
                MessageBox.Show("Not connected.");
                return;
            }

            byte[] buf = Encoding.UTF8.GetBytes(command + "\n");
            await str.WriteAsync(buf, 0, buf.Length);
        }
    }
}
