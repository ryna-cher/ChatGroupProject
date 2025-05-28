namespace ClientWinForms
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            picbIcon = new PictureBox();
            lblogin = new Label();
            txtbLogin = new TextBox();
            txtbPassword = new TextBox();
            btnLogin = new Button();
            ((System.ComponentModel.ISupportInitialize)picbIcon).BeginInit();
            SuspendLayout();
            // 
            // picbIcon
            // 
            picbIcon.Image = Properties.Resources.vectorstock_green_icon;
            picbIcon.Location = new Point(96, 9);
            picbIcon.Name = "picbIcon";
            picbIcon.Size = new Size(237, 162);
            picbIcon.SizeMode = PictureBoxSizeMode.Zoom;
            picbIcon.TabIndex = 0;
            picbIcon.TabStop = false;
            // 
            // lblogin
            // 
            lblogin.AutoSize = true;
            lblogin.Font = new Font("Segoe UI", 19.5F, FontStyle.Bold, GraphicsUnit.Point, 1, true);
            lblogin.ForeColor = SystemColors.GrayText;
            lblogin.Location = new Point(30, 167);
            lblogin.Name = "lblogin";
            lblogin.Size = new Size(364, 45);
            lblogin.TabIndex = 1;
            lblogin.Text = "Login to Your Account";
            // 
            // txtbLogin
            // 
            txtbLogin.Location = new Point(36, 242);
            txtbLogin.Name = "txtbLogin";
            txtbLogin.PlaceholderText = "Login";
            txtbLogin.Size = new Size(350, 27);
            txtbLogin.TabIndex = 2;
            // 
            // txtbPassword
            // 
            txtbPassword.Location = new Point(36, 294);
            txtbPassword.Name = "txtbPassword";
            txtbPassword.PasswordChar = '*';
            txtbPassword.PlaceholderText = "Password";
            txtbPassword.Size = new Size(350, 27);
            txtbPassword.TabIndex = 3;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.SeaGreen;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 12F);
            btnLogin.ForeColor = SystemColors.Window;
            btnLogin.Location = new Point(36, 353);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(350, 45);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(437, 428);
            Controls.Add(btnLogin);
            Controls.Add(txtbPassword);
            Controls.Add(txtbLogin);
            Controls.Add(lblogin);
            Controls.Add(picbIcon);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "LoginForm";
            Text = "LoginForm";
            ((System.ComponentModel.ISupportInitialize)picbIcon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox picbIcon;
        private Label lblogin;
        private TextBox txtbLogin;
        private TextBox txtbPassword;
        private Button btnLogin;
    }
}
