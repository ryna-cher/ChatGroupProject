namespace ClientWinForms
{
    partial class ChatForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lbType = new Label();
            lbChoose = new Label();
            txtbMessage = new TextBox();
            lstUser = new ListBox();
            btnSendMessage = new Button();
            txtbHistory = new TextBox();
            SuspendLayout();
            // 
            // lbType
            // 
            lbType.AutoSize = true;
            lbType.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbType.ForeColor = SystemColors.GrayText;
            lbType.Location = new Point(194, 16);
            lbType.Name = "lbType";
            lbType.Size = new Size(198, 28);
            lbType.TabIndex = 0;
            lbType.Text = "Type Your Message:";
            // 
            // lbChoose
            // 
            lbChoose.AutoSize = true;
            lbChoose.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lbChoose.ForeColor = SystemColors.GrayText;
            lbChoose.Location = new Point(15, 25);
            lbChoose.Name = "lbChoose";
            lbChoose.Size = new Size(131, 21);
            lbChoose.TabIndex = 1;
            lbChoose.Text = "Choose contact:";
            // 
            // txtbMessage
            // 
            txtbMessage.Location = new Point(169, 52);
            txtbMessage.Multiline = true;
            txtbMessage.Name = "txtbMessage";
            txtbMessage.Size = new Size(250, 126);
            txtbMessage.TabIndex = 2;
            // 
            // lstUser
            // 
            lstUser.ForeColor = Color.DimGray;
            lstUser.FormattingEnabled = true;
            lstUser.Location = new Point(17, 52);
            lstUser.Name = "lstUser";
            lstUser.Size = new Size(125, 364);
            lstUser.TabIndex = 3;
            // 
            // btnSendMessage
            // 
            btnSendMessage.BackColor = Color.SeaGreen;
            btnSendMessage.Cursor = Cursors.Hand;
            btnSendMessage.FlatAppearance.BorderSize = 0;
            btnSendMessage.FlatStyle = FlatStyle.Flat;
            btnSendMessage.Font = new Font("Segoe UI", 12F);
            btnSendMessage.ForeColor = SystemColors.Window;
            btnSendMessage.Location = new Point(169, 199);
            btnSendMessage.Name = "btnSendMessage";
            btnSendMessage.Size = new Size(250, 45);
            btnSendMessage.TabIndex = 4;
            btnSendMessage.Text = "Send Message";
            btnSendMessage.UseVisualStyleBackColor = false;
            btnSendMessage.Click += btnSendMessage_Click;
            // 
            // txtbHistory
            // 
            txtbHistory.Location = new Point(169, 264);
            txtbHistory.Multiline = true;
            txtbHistory.Name = "txtbHistory";
            txtbHistory.Size = new Size(250, 152);
            txtbHistory.TabIndex = 5;
            // 
            // ChatForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(437, 428);
            Controls.Add(txtbHistory);
            Controls.Add(btnSendMessage);
            Controls.Add(lstUser);
            Controls.Add(txtbMessage);
            Controls.Add(lbChoose);
            Controls.Add(lbType);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "ChatForm";
            StartPosition = FormStartPosition.Manual;
            Text = "ChatForm";
            FormClosing += ChatForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbType;
        private Label lbChoose;
        private TextBox txtbMessage;
        private ListBox lstUser;
        private Button btnSendMessage;
        private TextBox txtbHistory;
    }
}