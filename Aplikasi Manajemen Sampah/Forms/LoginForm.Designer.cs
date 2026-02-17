namespace Aplikasi_Manajemen_Sampah
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panelLogin = new GlassPanel();
            pnlPasswordContainer = new BorderedPanel();
            txtPassword = new TextBox();
            pbPassIcon = new PictureBox();
            lblTitle = new Label();
            pnlUsernameContainer = new BorderedPanel();
            txtUsername = new TextBox();
            pbUserIcon = new PictureBox();
            btnLogin = new RoundedButton();
            lblRegisterInfo = new Label();
            lnkRegister = new LinkLabel();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            panelLogin.SuspendLayout();
            pnlPasswordContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbPassIcon).BeginInit();
            pnlUsernameContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbUserIcon).BeginInit();
            SuspendLayout();
            // 
            // panelLogin
            // 
            panelLogin.BackColor = Color.Transparent;
            panelLogin.Controls.Add(pnlPasswordContainer);
            panelLogin.Controls.Add(lblTitle);
            panelLogin.Controls.Add(pnlUsernameContainer);
            panelLogin.Controls.Add(btnLogin);
            panelLogin.Controls.Add(lblRegisterInfo);
            panelLogin.Controls.Add(lnkRegister);
            panelLogin.Location = new Point(320, 60);
            panelLogin.Name = "panelLogin";
            panelLogin.Opacity = 150;
            panelLogin.Size = new Size(360, 480);
            panelLogin.TabIndex = 12;
            // 
            // pnlPasswordContainer
            // 
            pnlPasswordContainer.BackColor = Color.Transparent;
            pnlPasswordContainer.BorderColor = Color.White;
            pnlPasswordContainer.BorderRadius = 20;
            pnlPasswordContainer.Controls.Add(txtPassword);
            pnlPasswordContainer.Controls.Add(pbPassIcon);
            pnlPasswordContainer.FillColor = Color.FromArgb(30, 30, 30);
            pnlPasswordContainer.Location = new Point(40, 183);
            pnlPasswordContainer.Name = "pnlPasswordContainer";
            pnlPasswordContainer.Padding = new Padding(15, 10, 15, 10);
            pnlPasswordContainer.Size = new Size(280, 50);
            pnlPasswordContainer.TabIndex = 5;
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.FromArgb(64, 64, 64);
            txtPassword.BorderStyle = BorderStyle.None;
            txtPassword.ForeColor = Color.White;
            txtPassword.Location = new Point(50, 14);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(212, 20);
            txtPassword.TabIndex = 6;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // pbPassIcon
            // 
            pbPassIcon.Image = Properties.Resources.lock_24dp_FFFFFF_FILL0_wght400_GRAD0_opsz24;
            pbPassIcon.Location = new Point(10, 12);
            pbPassIcon.Name = "pbPassIcon";
            pbPassIcon.Size = new Size(24, 24);
            pbPassIcon.TabIndex = 10;
            pbPassIcon.TabStop = false;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(121, 5);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(124, 46);
            lblTitle.TabIndex = 2;
            lblTitle.Text = "LOGIN";
            // 
            // pnlUsernameContainer
            // 
            pnlUsernameContainer.BackColor = Color.Transparent;
            pnlUsernameContainer.BorderColor = Color.White;
            pnlUsernameContainer.BorderRadius = 20;
            pnlUsernameContainer.Controls.Add(txtUsername);
            pnlUsernameContainer.Controls.Add(pbUserIcon);
            pnlUsernameContainer.FillColor = Color.FromArgb(30, 30, 30);
            pnlUsernameContainer.Location = new Point(40, 113);
            pnlUsernameContainer.Name = "pnlUsernameContainer";
            pnlUsernameContainer.Padding = new Padding(15, 10, 15, 10);
            pnlUsernameContainer.Size = new Size(280, 50);
            pnlUsernameContainer.TabIndex = 1;
            // 
            // txtUsername
            // 
            txtUsername.BackColor = Color.FromArgb(64, 64, 64);
            txtUsername.BorderStyle = BorderStyle.None;
            txtUsername.ForeColor = Color.White;
            txtUsername.Location = new Point(50, 14);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(212, 20);
            txtUsername.TabIndex = 4;
            // 
            // pbUserIcon
            // 
            pbUserIcon.Image = Properties.Resources.person_24dp_FFFFFF_FILL0_wght400_GRAD0_opsz24___Copy;
            pbUserIcon.Location = new Point(10, 12);
            pbUserIcon.Name = "pbUserIcon";
            pbUserIcon.Size = new Size(24, 24);
            pbUserIcon.TabIndex = 3;
            pbUserIcon.TabStop = false;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.White;
            btnLogin.BorderRadius = 20;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnLogin.ForeColor = Color.Black;
            btnLogin.Location = new Point(40, 308);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(280, 45);
            btnLogin.TabIndex = 0;
            btnLogin.Text = "LOGIN";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // lblRegisterInfo
            // 
            lblRegisterInfo.AutoSize = true;
            lblRegisterInfo.ForeColor = Color.White;
            lblRegisterInfo.Location = new Point(57, 379);
            lblRegisterInfo.Name = "lblRegisterInfo";
            lblRegisterInfo.Size = new Size(163, 20);
            lblRegisterInfo.TabIndex = 8;
            lblRegisterInfo.Text = "Don't have an account?";
            // 
            // lnkRegister
            // 
            lnkRegister.AutoSize = true;
            lnkRegister.LinkColor = Color.LightSkyBlue;
            lnkRegister.Location = new Point(227, 379);
            lnkRegister.Name = "lnkRegister";
            lnkRegister.Size = new Size(63, 20);
            lnkRegister.TabIndex = 9;
            lnkRegister.TabStop = true;
            lnkRegister.Text = "Register";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources._0zun_56kz_211202;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(982, 553);
            Controls.Add(panelLogin);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            panelLogin.ResumeLayout(false);
            panelLogin.PerformLayout();
            pnlPasswordContainer.ResumeLayout(false);
            pnlPasswordContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbPassIcon).EndInit();
            pnlUsernameContainer.ResumeLayout(false);
            pnlUsernameContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbUserIcon).EndInit();
            ResumeLayout(false);
        }
        #endregion

        private GlassPanel panelLogin;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TextBox txtPassword;
        private BorderedPanel pnlPasswordContainer;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.PictureBox pbUserIcon;
        private System.Windows.Forms.Label lblTitle;
        private BorderedPanel pnlUsernameContainer;
        private RoundedButton btnLogin;
        private System.Windows.Forms.PictureBox pbPassIcon;
        private System.Windows.Forms.Label lblRegisterInfo;
        private System.Windows.Forms.LinkLabel lnkRegister;
    }
}