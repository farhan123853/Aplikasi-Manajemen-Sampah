using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Aplikasi_Manajemen_Sampah.Models;

namespace Aplikasi_Manajemen_Sampah.Forms
{
    public partial class DashboardAdmin : Form
    {
        private User currentUser;
        private Form activeForm = null;

        public DashboardAdmin(User user)
        {
            this.currentUser = user;
            InitializeComponent();

            InitializeCustomDesign();

            // Setup Navigasi Sidebar
            if (btnSampah != null)
                btnSampah.Click += (s, e) => OpenChildForm(new FormSampah(currentUser));

            if (btnPenjemputan != null)
                btnPenjemputan.Click += (s, e) => OpenChildForm(new FormPenjemputan(currentUser));

            if (btnUsers != null)
                btnUsers.Click += (s, e) => OpenChildForm(new FormUsers(currentUser));

            if (btnCetak != null)
                btnCetak.Click += btnCetakAdmin_Click;

            // TAMBAHAN BARU: Setup button Chatbot
            if (btnChatbot != null)
                btnChatbot.Click += (s, e) => OpenChildForm(new FormChatbot(currentUser));

            // TAMBAHAN BARU: Setup button Grafik
            if (btnGrafik != null)
                btnGrafik.Click += (s, e) => OpenChildForm(new FormGrafik());

            if (btnLogout != null)
                btnLogout.Click += BtnLogout_Click;
        }

        private void InitializeCustomDesign()
        {
            // Set Username di Header
            if (Controls.Find("lblWelcome", true).Length > 0)
                ((Label)Controls.Find("lblWelcome", true)[0]).Text = $"Welcome, {currentUser.Username}";

            // Styling Tombol Sidebar
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

            // TAMBAHAN BARU: Styling button Chatbot
            if (btnChatbot != null)
            {
                UIHelper.SetSidebarButton(btnChatbot);
                SetupButtonHover(btnChatbot);
                btnChatbot.Visible = true;
                btnChatbot.BringToFront();
            }

            // TAMBAHAN BARU: Styling button Grafik
            if (btnGrafik != null)
            {
                UIHelper.SetSidebarButton(btnGrafik);
                SetupButtonHover(btnGrafik);
                btnGrafik.Visible = true;
                btnGrafik.BringToFront();
            }

            if (btnLogout != null)
            {
                UIHelper.SetSidebarButton(btnLogout);
                btnLogout.BackColor = Color.FromArgb(192, 57, 43); // Merah khusus Logout
            }

            // Sembunyikan menu User jika bukan Admin
            if (currentUser.Role != "Admin")
            {
                if (btnUsers != null) btnUsers.Visible = false;
            }
        }

        private void SetupButtonHover(Button btn)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(46, 204, 113);
            btn.MouseLeave += (s, e) => btn.BackColor = UIHelper.PrimaryColor;
        }

        private void OpenChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();

            activeForm = childForm;

            // Konfigurasi agar Form bisa masuk ke dalam Panel
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
            OpenChildForm(new FormLaporan());
        }
    }
}