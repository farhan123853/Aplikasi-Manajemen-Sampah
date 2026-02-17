using System;
using System.Drawing;
using System.Windows.Forms;
using Aplikasi_Manajemen_Sampah.Services;
using Aplikasi_Manajemen_Sampah; // For components like RoundedButton

namespace Aplikasi_Manajemen_Sampah.Forms
{
    /// <summary>
    /// Form Registrasi: Menangani validasi dan pendaftaran pengguna baru.
    /// </summary>
    public partial class Register : Form
    {
        #region Fields & Constants

        private readonly AuthService _authService;
        private const string UserPlaceholder = "Username";
        private const string PassPlaceholder = "Password";
        private const string ConfirmPassPlaceholder = "Konfirmasi Password";

        private readonly Color _textColor = Color.White;
        private readonly Color _placeholderColor = Color.Silver;
        private readonly Color _inputBackground = Color.FromArgb(30, 30, 30);

        #endregion

        public Register()
        {
            InitializeComponent();
            _authService = new AuthService();
            SetupEvents();
            InitializeCustomDesign();
        }

        #region UI Initialization

        private void InitializeCustomDesign()
        {
            SetupPanelColor(pnlUsernameContainer);
            SetupPanelColor(pnlPasswordContainer);
            SetupPanelColor(pnlConfirmPassContainer);

            ApplyInputStyle(txtUsername, pbUserIcon, UserPlaceholder);
            ApplyInputStyle(txtPassword, pbPassIcon, PassPlaceholder, true);
            ApplyInputStyle(txtConfirmPassword, pbConfirmPassIcon, ConfirmPassPlaceholder, true);

            this.ActiveControl = lblTitle;
        }

        private void SetupEvents()
        {
            this.Resize += (s, e) => CenterPanel();
            this.Load += (s, e) => CenterPanel();

            SetupPlaceholderEvents(txtUsername, UserPlaceholder);
            SetupPasswordPlaceholderEvents(txtPassword, PassPlaceholder);
            SetupPasswordPlaceholderEvents(txtConfirmPassword, ConfirmPassPlaceholder);

            btnRegister.Click += BtnRegister_Click;
            lnkLogin.Click += (s, e) => 
            {
                new LoginForm().Show();
                this.Close();
            };
        }

        #endregion

        #region Helper Methods

        private void SetupPanelColor(BorderedPanel pnl) => pnl.FillColor = _inputBackground;

        private void ApplyInputStyle(TextBox txt, PictureBox icon, string placeholder, bool isPassword = false)
        {
            txt.BackColor = _inputBackground;
            txt.ForeColor = _textColor;
            txt.BorderStyle = BorderStyle.None;
            icon.BackColor = _inputBackground;
            SetPlaceholder(txt, placeholder);
            if (isPassword) txt.UseSystemPasswordChar = false;
        }

        private void CenterPanel()
        {
            if (panelRegister != null)
                panelRegister.Location = new Point((this.ClientSize.Width - panelRegister.Width) / 2, (this.ClientSize.Height - panelRegister.Height) / 2);
        }

        private void SetPlaceholder(TextBox txt, string placeholder) { txt.Text = placeholder; txt.ForeColor = _placeholderColor; }
        private void RemovePlaceholder(TextBox txt) { txt.Text = ""; txt.ForeColor = _textColor; }
        private void SetupPlaceholderEvents(TextBox txt, string placeholder) { txt.Enter += (s, e) => { if (txt.Text == placeholder) RemovePlaceholder(txt); }; txt.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txt.Text)) SetPlaceholder(txt, placeholder); }; }
        private void SetupPasswordPlaceholderEvents(TextBox txt, string placeholder) { txt.Enter += (s, e) => { if (txt.Text == placeholder) { RemovePlaceholder(txt); txt.UseSystemPasswordChar = true; } }; txt.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txt.Text)) { txt.UseSystemPasswordChar = false; SetPlaceholder(txt, placeholder); } }; }

        #endregion

        #region Event Handlers

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string confirm = txtConfirmPassword.Text;

            // Validasi Input
            if (IsPlaceholder(username, UserPlaceholder)) { ShowWarning("Username harus diisi!"); return; }
            if (IsPlaceholder(password, PassPlaceholder)) { ShowWarning("Password harus diisi!"); return; }
            if (password != confirm) { ShowWarning("Konfirmasi password tidak cocok!"); return; }

            btnRegister.Enabled = false;
            btnRegister.Text = "LOADING...";

            try
            {
                // Default Role: "User". Only Admin can create other roles via User Management.
                bool success = await _authService.CreateUserAsync(username, password, "User");

                if (success)
                {
                    MessageBox.Show("Registrasi Berhasil! Silakan Login.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    new LoginForm().Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Registrasi gagal.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex) 
            { 
                MessageBox.Show("Error: " + ex.Message, "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
            finally
            {
                btnRegister.Enabled = true;
                btnRegister.Text = "REGISTER";
            }
        }

        private bool IsPlaceholder(string text, string placeholder) => text == placeholder || string.IsNullOrWhiteSpace(text);
        private void ShowWarning(string msg) => MessageBox.Show(msg, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        #endregion
    }
}