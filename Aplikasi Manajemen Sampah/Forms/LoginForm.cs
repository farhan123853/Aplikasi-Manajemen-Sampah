using System;
using System.Drawing;
using System.Windows.Forms;
using Aplikasi_Manajemen_Sampah.Models;
using Aplikasi_Manajemen_Sampah.Services;

namespace Aplikasi_Manajemen_Sampah
{
    /// <summary>
    /// Form Login utama aplikasi.
    /// Menangani autentikasi user dan pengalihan ke Dashboard sesuai hak akses.
    /// Dilengkapi dengan custom design UI (Glassmorphism style).
    /// </summary>
    public partial class LoginForm : Form
    {
        private AuthService authService;
        
        // Konstanta untuk Placeholder Text
        private const string UserPlaceholder = "Username";
        private const string PassPlaceholder = "Password";
        
        private Color TextColor = Color.White;
        private Color PlaceholderColor = Color.DarkGray;

        public LoginForm()
        {
            InitializeComponent();
            authService = new AuthService();

            SetupEvents();
            InitializeCustomDesign();
        }

        /// <summary>
        /// Mengatur tampilan visual kustom untuk kontrol input.
        /// Mengubah warna background textbox agar menyatu dengan panel transparan/gelap.
        /// </summary>
        private void InitializeCustomDesign()
        {
            // Warna latar input agar menyatu dengan Glass Panel (Gelap Transparan)
            Color inputBackground = Color.FromArgb(30, 30, 30);

            if (pnlUsernameContainer is BorderedPanel pnlUser) pnlUser.FillColor = inputBackground;
            if (pnlPasswordContainer is BorderedPanel pnlPass) pnlPass.FillColor = inputBackground;

            txtUsername.BackColor = inputBackground;
            txtPassword.BackColor = inputBackground;

            txtUsername.BorderStyle = BorderStyle.None;
            txtPassword.BorderStyle = BorderStyle.None;

            txtUsername.ForeColor = Color.White;
            txtPassword.ForeColor = Color.White;

            // Set state awal placeholder
            SetPlaceholder(txtUsername, UserPlaceholder);
            SetPlaceholder(txtPassword, PassPlaceholder);
            txtPassword.UseSystemPasswordChar = false; // Tampilkan text password placeholder

            this.ActiveControl = lblTitle; // Fokus awal ke Label agar placeholder tidak hilang otomatis
        }

        /// <summary>
        /// Mendaftarkan event handler untuk interaksi UI.
        /// Termasuk logika responsif (Resize), placeholder behavior, dan shortcut keyboard.
        /// </summary>
        private void SetupEvents()
        {
            this.Resize += (s, e) => CenterPanel();
            this.Load += (s, e) => CenterPanel();

            // Link Register
            lnkRegister.Click += (s, e) => 
            {
                new Forms.Register().Show();
                this.Hide(); 
            };

            // Logika Placeholder Username
            txtUsername.Enter += (s, e) => { if (txtUsername.Text == UserPlaceholder) RemovePlaceholder(txtUsername); };
            txtUsername.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtUsername.Text)) SetPlaceholder(txtUsername, UserPlaceholder); };

            // Logika Placeholder Password (dengan handling Masking Character)
            txtPassword.Enter += (s, e) =>
            {
                if (txtPassword.Text == PassPlaceholder)
                {
                    RemovePlaceholder(txtPassword);
                    txtPassword.UseSystemPasswordChar = true; // Aktifkan masking password saat ketik
                }
            };

            txtPassword.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    txtPassword.UseSystemPasswordChar = false; // Matikan masking untuk placeholder
                    SetPlaceholder(txtPassword, PassPlaceholder);
                }
            };

            btnLogin.Click += btnLogin_Click;

            // Enter key shortcut untuk trigger login
            txtUsername.KeyPress += Txt_KeyPress;
            txtPassword.KeyPress += Txt_KeyPress;
        }

        /// <summary>
        /// Menjaga posisi panel login tetap di tengah layar saat ukuran form berubah.
        /// </summary>
        private void CenterPanel()
        {
            if (panelLogin != null)
            {
                panelLogin.Location = new Point(
                    (this.ClientSize.Width - panelLogin.Width) / 2,
                    (this.ClientSize.Height - panelLogin.Height) / 2
                );
            }
        }

        private void SetPlaceholder(TextBox txt, string placeholder)
        {
            txt.Text = placeholder;
            txt.ForeColor = PlaceholderColor;
        }

        private void RemovePlaceholder(TextBox txt)
        {
            txt.Text = "";
            txt.ForeColor = TextColor;
        }

        private void Txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) btnLogin_Click(sender, e);
        }

        /// <summary>
        /// Menangani proses Login secara asinkron.
        /// Validasi input, pemanggilan AuthService, dan navigasi ke Dashboard.
        /// </summary>
        private async void btnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Validasi input dasar
            if (username == UserPlaceholder || string.IsNullOrWhiteSpace(username) ||
                password == PassPlaceholder || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Harap isi Username dan Password!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // UX: Disable tombol saat loading
            btnLogin.Enabled = false;
            btnLogin.Text = "LOADING...";

            try
            {
                var user = await authService.LoginAsync(username, password);

                if (user != null)
                {
                    // Login Berhasil -> Buka Dashboard
                    var dashboard = new Forms.DashboardAdmin(user);
                    dashboard.Show();
                    this.Hide();
                    
                    // Tutup aplikasi login jika dashboard ditutup
                    dashboard.FormClosed += (s, args) => this.Close();
                }
                else
                {
                    MessageBox.Show("Username atau Password salah!", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi kesalahan sistem: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Reset tombol ke keadaan semula
                btnLogin.Enabled = true;
                btnLogin.Text = "LOGIN";
            }
        }
    }
}