using System;
using System.Drawing;
using System.Windows.Forms;
using Aplikasi_Manajemen_Sampah.Models;

namespace Aplikasi_Manajemen_Sampah.Forms
{
    public partial class DashboardAdmin : Form
    {
        private User currentUser;
        private Form activeForm = null;

        public DashboardAdmin(User user)
        {
            currentUser = user;
            InitializeComponent();

            InitializeCustomDesign();
            SetupNavigation();
        }

        // ===============================
        // SETUP NAVIGATION
        // ===============================
        private void SetupNavigation()
        {
            if (btnSampah != null)
                btnSampah.Click += (s, e) =>
                    OpenChildForm(new FormSampah(currentUser));

            if (btnPenjemputan != null)
                btnPenjemputan.Click += (s, e) =>
                    OpenChildForm(new FormPenjemputan(currentUser));

            if (btnUsers != null)
                btnUsers.Click += (s, e) =>
                    OpenChildForm(new FormUsers(currentUser));

            if (btnCetak != null)
                btnCetak.Click += btnCetakAdmin_Click;

            if (btnChatbot != null)
                btnChatbot.Click += (s, e) =>
                    OpenChildForm(new FormChatbot(currentUser));

            if (btnGrafik != null)
                btnGrafik.Click += (s, e) =>
                    OpenChildForm(new FormGrafik());

            if (btnLogout != null)
                btnLogout.Click += BtnLogout_Click;
        }

        // ===============================
        // UI DESIGN
        // ===============================
        private void InitializeCustomDesign()
        {
            var lbl = Controls.Find("lblWelcome", true);
            if (lbl.Length > 0)
                ((Label)lbl[0]).Text = $"Welcome, {currentUser.Username}";

            if (btnSampah != null) { UIHelper.SetSidebarButton(btnSampah); SetupButtonHover(btnSampah); }
            if (btnPenjemputan != null) { UIHelper.SetSidebarButton(btnPenjemputan); SetupButtonHover(btnPenjemputan); }
            if (btnUsers != null) { UIHelper.SetSidebarButton(btnUsers); SetupButtonHover(btnUsers); }

            if (btnCetak != null)
            {
                UIHelper.SetSidebarButton(btnCetak);
                SetupButtonHover(btnCetak);
            }

            if (btnChatbot != null)
            {
                UIHelper.SetSidebarButton(btnChatbot);
                SetupButtonHover(btnChatbot);
            }

            if (btnGrafik != null)
            {
                UIHelper.SetSidebarButton(btnGrafik);
                SetupButtonHover(btnGrafik);
            }

            if (btnLogout != null)
            {
                UIHelper.SetSidebarButton(btnLogout);
                btnLogout.BackColor = Color.FromArgb(192, 57, 43);
            }

            // sembunyikan menu user jika bukan admin
            if (currentUser.Role != "Admin" && btnUsers != null)
                btnUsers.Visible = false;
        }

        private void SetupButtonHover(Button btn)
        {
            btn.MouseEnter += (s, e) =>
                btn.BackColor = Color.FromArgb(46, 204, 113);

            btn.MouseLeave += (s, e) =>
                btn.BackColor = UIHelper.PrimaryColor;
        }

        // ===============================
        // OPEN CHILD FORM
        // ===============================
        private void OpenChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();

            activeForm = childForm;

            panelContent.Controls.Clear();

            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panelContent.Controls.Add(childForm);
            panelContent.Tag = childForm;

            childForm.BringToFront();
            childForm.Show();
        }

        // ===============================
        // LOGOUT
        // ===============================
        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Apakah Anda yakin ingin logout?",
                "Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                new LoginForm().Show();
                this.Close();
            }
        }

        private void btnCetakAdmin_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormLaporan());
        }
    }
}