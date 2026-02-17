using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Aplikasi_Manajemen_Sampah.Models;

namespace Aplikasi_Manajemen_Sampah.Forms
{
    /// <summary>
    /// Form Utama (Dashboard) aplikasi yang berfungsi sebagai kontainer navigasi.
    /// Menerapkan pola "Single Page Application" sederhana dengan panel konten dinamis.
    /// Menangani Role-Based Access Control (RBAC) dengan menyembunyikan menu yang tidak relevan.
    /// </summary>
    public partial class DashboardAdmin : Form
    {
        private User currentUser;
        
        /// <summary>
        /// Menyimpan referensi form aktif yang sedang ditampilkan di panel konten.
        /// </summary>
        private Form activeForm = null;

        public DashboardAdmin(User user)
        {
            this.currentUser = user;
            InitializeComponent();

            InitializeCustomDesign();

            // Setup Navigasi Sidebar menggunakan Event Handler Lambda
            if (btnSampah != null)
                btnSampah.Click += (s, e) => OpenChildForm(new FormSampah(currentUser));

            if (btnPenjemputan != null)
                btnPenjemputan.Click += (s, e) => OpenChildForm(new FormPenjemputan(currentUser));

            if (btnUsers != null)
                btnUsers.Click += (s, e) => OpenChildForm(new FormUsers(currentUser));

            if (btnCetak != null)
                btnCetak.Click += btnCetakAdmin_Click;

            // TAMBAHAN BARU: Setup button Grafik
            if (btnGrafik != null)
                btnGrafik.Click += (s, e) => OpenChildForm(new FormGrafik());

            // TAMBAHAN BARU: Setup button Chatbot
            if (btnChatbot != null)
                btnChatbot.Click += (s, e) => OpenChildForm(new FormChatbot(currentUser));

            // TAMBAHAN BARU: Setup button Maps/Peta
            if (btnMaps != null)
                btnMaps.Click += (s, e) => OpenChildForm(new FormMaps());

            if (btnLogout != null)
                btnLogout.Click += BtnLogout_Click;
        }

        /// <summary>
        /// Inisialisasi tampilan UI, termasuk styling tombol dan penerapan restriksi akses berdasarkan Role.
        /// </summary>
        private void InitializeCustomDesign()
        {
            // Set Username di Header
            if (Controls.Find("lblWelcome", true).Length > 0)
                ((Label)Controls.Find("lblWelcome", true)[0]).Text = $"Welcome, {currentUser.Username}";

            // Styling Tombol Sidebar (Hover effects & Base style)
            if (btnSampah != null) { UIHelper.SetSidebarButton(btnSampah); SetupButtonHover(btnSampah); }
            if (btnPenjemputan != null) { UIHelper.SetSidebarButton(btnPenjemputan); SetupButtonHover(btnPenjemputan); }
            if (btnUsers != null) { UIHelper.SetSidebarButton(btnUsers); SetupButtonHover(btnUsers); }
            if (btnCetak != null)
            {
                UIHelper.SetSidebarButton(btnCetak);
                SetupButtonHover(btnCetak);
                btnCetak.Visible = true;
                btnCetak.BringToFront();
            }

            if (btnGrafik != null)
            {
                UIHelper.SetSidebarButton(btnGrafik);
                SetupButtonHover(btnGrafik);
                btnGrafik.Visible = true;
                btnGrafik.BringToFront();
            }

            if (btnChatbot != null)
            {
                UIHelper.SetSidebarButton(btnChatbot);
                SetupButtonHover(btnChatbot);
                btnChatbot.Visible = true;
                btnChatbot.BringToFront();
            }

            if (btnMaps != null)
            {
                UIHelper.SetSidebarButton(btnMaps);
                SetupButtonHover(btnMaps);
                btnMaps.Visible = true;
                btnMaps.BringToFront();
            }

            if (btnLogout != null)
            {
                // UIHelper.SetSidebarButton(btnLogout); // Hapus sidebar styling standard
                UIHelper.SetDangerButton(btnLogout);     // Ganti dengan style tombol merah (Logout)
            }

            // LOGIKA ROLE AKSES (Case-Insensitive)
            // Menyembunyikan tombol navigasi berdasarkan hak akses user.
            string userRole = currentUser.Role?.Trim();

            if (string.Equals(userRole, "Petugas", StringComparison.OrdinalIgnoreCase))
            {
                // Petugas tidak bisa kelola user
                if (btnUsers != null) btnUsers.Visible = false;
            }
            else if (string.Equals(userRole, "User", StringComparison.OrdinalIgnoreCase))
            {
                // User biasa hanya bisa lihat Grafik, Chatbot, Peta, dan Cetak Laporan pribadi
                // Tidak boleh akses CRUD Sampah/Penjemputan/User
                if (btnUsers != null) btnUsers.Visible = false;
                if (btnSampah != null) btnSampah.Visible = false;
                if (btnPenjemputan != null) btnPenjemputan.Visible = false;
            }
        }

        private void SetupButtonHover(Button btn)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(46, 204, 113); // Hijau terang saat hover
            btn.MouseLeave += (s, e) => btn.BackColor = UIHelper.PrimaryColor; // Kembali ke warna dasar
        }

        /// <summary>
        /// Membuka form anak (child form) di dalam panel konten utama.
        /// Menutup form yang sedang aktif sebelumnya untuk mencegah tumpukan memori.
        /// </summary>
        /// <param name="childForm">Form yang akan ditampilkan.</param>
        private void OpenChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();

            activeForm = childForm;

            // Konfigurasi agar Form bisa behave sebagai kontrol di dalam Panel
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panelContent.Controls.Add(childForm);
            panelContent.Tag = childForm;

            childForm.BringToFront();
            childForm.Show();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Apakah Anda yakin ingin logout?", "Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                new LoginForm().Show();
                this.Hide();
            }
        }

        private async void btnCetakAdmin_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormLaporan(currentUser));
        }
    }
}