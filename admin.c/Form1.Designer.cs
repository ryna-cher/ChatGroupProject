namespace admin.c
{
    partial class Form1
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
            connect = new Button();
            login = new TextBox();
            pass = new TextBox();
            add_user = new Button();
            block_user = new Button();
            block_user_pass = new TextBox();
            blcok_user_name = new TextBox();
            new_user_pass = new TextBox();
            new_user_login = new TextBox();
            SuspendLayout();
            // 
            // connect
            // 
            connect.Location = new Point(308, 221);
            connect.Name = "connect";
            connect.Size = new Size(150, 46);
            connect.TabIndex = 0;
            connect.Text = "connect";
            connect.UseVisualStyleBackColor = true;
            connect.Click += connect_Click;
            // 
            // login
            // 
            login.Location = new Point(280, 327);
            login.Name = "login";
            login.Size = new Size(200, 39);
            login.TabIndex = 1;
            // 
            // pass
            // 
            pass.Location = new Point(280, 273);
            pass.Name = "pass";
            pass.Size = new Size(200, 39);
            pass.TabIndex = 2;
            // 
            // add_user
            // 
            add_user.Location = new Point(116, 51);
            add_user.Name = "add_user";
            add_user.Size = new Size(150, 46);
            add_user.TabIndex = 3;
            add_user.Text = "add_user";
            add_user.UseVisualStyleBackColor = true;
            add_user.Click += add_user_Click;
            // 
            // block_user
            // 
            block_user.Location = new Point(508, 51);
            block_user.Name = "block_user";
            block_user.Size = new Size(150, 46);
            block_user.TabIndex = 4;
            block_user.Text = "block user";
            block_user.UseVisualStyleBackColor = true;
            block_user.Click += block_user_Click;
            // 
            // block_user_pass
            // 
            block_user_pass.Location = new Point(497, 99);
            block_user_pass.Name = "block_user_pass";
            block_user_pass.Size = new Size(200, 39);
            block_user_pass.TabIndex = 5;
            // 
            // blcok_user_name
            // 
            blcok_user_name.Location = new Point(497, 144);
            blcok_user_name.Name = "blcok_user_name";
            blcok_user_name.Size = new Size(200, 39);
            blcok_user_name.TabIndex = 6;
            // 
            // new_user_pass
            // 
            new_user_pass.Location = new Point(78, 99);
            new_user_pass.Name = "new_user_pass";
            new_user_pass.Size = new Size(200, 39);
            new_user_pass.TabIndex = 7;
            // 
            // new_user_login
            // 
            new_user_login.Location = new Point(78, 144);
            new_user_login.Name = "new_user_login";
            new_user_login.Size = new Size(200, 39);
            new_user_login.TabIndex = 8;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(new_user_login);
            Controls.Add(new_user_pass);
            Controls.Add(blcok_user_name);
            Controls.Add(block_user_pass);
            Controls.Add(block_user);
            Controls.Add(add_user);
            Controls.Add(pass);
            Controls.Add(login);
            Controls.Add(connect);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button connect;
        private TextBox login;
        private TextBox pass;
        private Button add_user;
        private Button block_user;
        private TextBox block_user_pass;
        private TextBox blcok_user_name;
        private TextBox new_user_pass;
        private TextBox new_user_login;
    }
}
