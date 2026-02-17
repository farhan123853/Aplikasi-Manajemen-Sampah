using System.Drawing;
using System.Windows.Forms;
using Aplikasi_Manajemen_Sampah.Properties; // For Resources

namespace Aplikasi_Manajemen_Sampah.Forms
{
    partial class Register
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
            panelRegister = new GlassPanel();
            lblTitle = new Label();
            pnlUsernameContainer = new BorderedPanel();
            pbUserIcon = new PictureBox();
            txtUsername = new TextBox();
            pnlPasswordContainer = new BorderedPanel();
            pbPassIcon = new PictureBox();
            txtPassword = new TextBox();
            pnlConfirmPassContainer = new BorderedPanel();
            pbConfirmPassIcon = new PictureBox();
            txtConfirmPassword = new TextBox();
            btnRegister = new RoundedButton();
            lblLoginInfo = new Label();
            lnkLogin = new LinkLabel();
            panelRegister.SuspendLayout();
            pnlUsernameContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbUserIcon).BeginInit();
            pnlPasswordContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbPassIcon).BeginInit();
            pnlConfirmPassContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbConfirmPassIcon).BeginInit();
            SuspendLayout();
            // 
            // panelRegister
            // 
            panelRegister.BackColor = Color.Transparent;
            panelRegister.Controls.Add(lblTitle);
            panelRegister.Controls.Add(pnlUsernameContainer);
            panelRegister.Controls.Add(pnlPasswordContainer);
            panelRegister.Controls.Add(pnlConfirmPassContainer);
            panelRegister.Controls.Add(btnRegister);
            panelRegister.Controls.Add(lblLoginInfo);
            panelRegister.Controls.Add(lnkLogin);
            panelRegister.Location = new Point(311, 60);
            panelRegister.Name = "panelRegister";
            panelRegister.Opacity = 150;
            panelRegister.Size = new Size(360, 450);
            panelRegister.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(100, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(174, 46);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "REGISTER";
            // 
            // pnlUsernameContainer
            // 
            pnlUsernameContainer.BackColor = Color.Transparent;
            pnlUsernameContainer.BorderColor = Color.White;
            pnlUsernameContainer.BorderRadius = 15;
            pnlUsernameContainer.Controls.Add(pbUserIcon);
            pnlUsernameContainer.Controls.Add(txtUsername);
            pnlUsernameContainer.FillColor = Color.FromArgb(30, 30, 30);
            pnlUsernameContainer.Location = new Point(40, 80);
            pnlUsernameContainer.Name = "pnlUsernameContainer";
            pnlUsernameContainer.Size = new Size(280, 50);
            pnlUsernameContainer.TabIndex = 3;
            // 
            // pbUserIcon
            // 
            pbUserIcon.Image = Resources.person_24dp_FFFFFF_FILL0_wght400_GRAD0_opsz24___Copy;
            pbUserIcon.Location = new Point(10, 12);
            pbUserIcon.Name = "pbUserIcon";
            pbUserIcon.Size = new Size(24, 24);
            pbUserIcon.SizeMode = PictureBoxSizeMode.Zoom;
            pbUserIcon.TabIndex = 0;
            pbUserIcon.TabStop = false;
            // 
            // txtUsername
            // 
            txtUsername.BorderStyle = BorderStyle.None;
            txtUsername.Location = new Point(50, 14);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(210, 20);
            txtUsername.TabIndex = 1;
            // 
            // pnlPasswordContainer
            // 
            pnlPasswordContainer.BackColor = Color.Transparent;
            pnlPasswordContainer.BorderColor = Color.White;
            pnlPasswordContainer.BorderRadius = 15;
            pnlPasswordContainer.Controls.Add(pbPassIcon);
            pnlPasswordContainer.Controls.Add(txtPassword);
            pnlPasswordContainer.FillColor = Color.FromArgb(30, 30, 30);
            pnlPasswordContainer.Location = new Point(40, 140);
            pnlPasswordContainer.Name = "pnlPasswordContainer";
            pnlPasswordContainer.Size = new Size(280, 50);
            pnlPasswordContainer.TabIndex = 4;
            // 
            // pbPassIcon
            // 
            pbPassIcon.Image = Resources.lock_24dp_FFFFFF_FILL0_wght400_GRAD0_opsz24;
            pbPassIcon.Location = new Point(10, 12);
            pbPassIcon.Name = "pbPassIcon";
            pbPassIcon.Size = new Size(24, 24);
            pbPassIcon.SizeMode = PictureBoxSizeMode.Zoom;
            pbPassIcon.TabIndex = 0;
            pbPassIcon.TabStop = false;
            // 
            // txtPassword
            // 
            txtPassword.BorderStyle = BorderStyle.None;
            txtPassword.Location = new Point(50, 14);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(210, 20);
            txtPassword.TabIndex = 1;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // pnlConfirmPassContainer
            // 
            pnlConfirmPassContainer.BackColor = Color.Transparent;
            pnlConfirmPassContainer.BorderColor = Color.White;
            pnlConfirmPassContainer.BorderRadius = 15;
            pnlConfirmPassContainer.Controls.Add(pbConfirmPassIcon);
            pnlConfirmPassContainer.Controls.Add(txtConfirmPassword);
            pnlConfirmPassContainer.FillColor = Color.FromArgb(30, 30, 30);
            pnlConfirmPassContainer.Location = new Point(40, 200);
            pnlConfirmPassContainer.Name = "pnlConfirmPassContainer";
            pnlConfirmPassContainer.Size = new Size(280, 50);
            pnlConfirmPassContainer.TabIndex = 5;
            // 
            // pbConfirmPassIcon
            // 
            pbConfirmPassIcon.Image = Resources.lock_24dp_FFFFFF_FILL0_wght400_GRAD0_opsz24;
            pbConfirmPassIcon.Location = new Point(10, 12);
            pbConfirmPassIcon.Name = "pbConfirmPassIcon";
            pbConfirmPassIcon.Size = new Size(24, 24);
            pbConfirmPassIcon.SizeMode = PictureBoxSizeMode.Zoom;
            pbConfirmPassIcon.TabIndex = 0;
            pbConfirmPassIcon.TabStop = false;
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.BorderStyle = BorderStyle.None;
            txtConfirmPassword.Location = new Point(50, 14);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.Size = new Size(210, 20);
            txtConfirmPassword.TabIndex = 1;
            txtConfirmPassword.UseSystemPasswordChar = true;
            // 
            // btnRegister
            // 
            btnRegister.BackColor = Color.White;
            btnRegister.BorderRadius = 20;
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnRegister.ForeColor = Color.Black;
            btnRegister.Location = new Point(40, 280);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(280, 45);
            btnRegister.TabIndex = 6;
            btnRegister.Text = "REGISTER";
            btnRegister.UseVisualStyleBackColor = false;
            // 
            // lblLoginInfo
            // 
            lblLoginInfo.AutoSize = true;
            lblLoginInfo.ForeColor = Color.White;
            lblLoginInfo.Location = new Point(50, 350);
            lblLoginInfo.Name = "lblLoginInfo";
            lblLoginInfo.Size = new Size(158, 20);
            lblLoginInfo.TabIndex = 7;
            lblLoginInfo.Text = "Already have account?";
            // 
            // lnkLogin
            // 
            lnkLogin.AutoSize = true;
            lnkLogin.LinkColor = Color.LightSkyBlue;
            lnkLogin.Location = new Point(210, 350);
            lnkLogin.Name = "lnkLogin";
            lnkLogin.Size = new Size(46, 20);
            lnkLogin.TabIndex = 8;
            lnkLogin.TabStop = true;
            lnkLogin.Text = "Login";
            // 
            // Register
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Resources._0zun_56kz_211202;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(982, 653);
            Controls.Add(panelRegister);
            Name = "Register";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Register";
            panelRegister.ResumeLayout(false);
            panelRegister.PerformLayout();
            pnlUsernameContainer.ResumeLayout(false);
            pnlUsernameContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbUserIcon).EndInit();
            pnlPasswordContainer.ResumeLayout(false);
            pnlPasswordContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbPassIcon).EndInit();
            pnlConfirmPassContainer.ResumeLayout(false);
            pnlConfirmPassContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbConfirmPassIcon).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GlassPanel panelRegister;
        private System.Windows.Forms.Label lblTitle;
        private RoundedButton btnRegister;
        private System.Windows.Forms.Label lblLoginInfo;
        private System.Windows.Forms.LinkLabel lnkLogin;

        private BorderedPanel pnlUsernameContainer;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.PictureBox pbUserIcon;

        private BorderedPanel pnlPasswordContainer;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.PictureBox pbPassIcon;

        private BorderedPanel pnlConfirmPassContainer;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.PictureBox pbConfirmPassIcon;
    }
}