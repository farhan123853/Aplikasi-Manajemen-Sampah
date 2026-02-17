using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Models;
using System.Linq;
using BCrypt.Net;
using Aplikasi_Manajemen_Sampah.Services;

namespace Aplikasi_Manajemen_Sampah.Forms
{
    /// <summary>
    /// Form manajemen pengguna (User Management).
    /// Khusus untuk Role Admin: Create, Update, Delete user serta pengaturan Role.
    /// </summary>
    public partial class FormUsers : Form
    {
        private User currentUser;
        private MongoService mongo;
        private string selectedId = "";

        public FormUsers(User user)
        {
            this.currentUser = user;

            // Security Check: Hanya Admin yang boleh akses halaman ini di level konstruktor
            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("Akses Ditolak!");
                this.Close();
                return;
            }

            this.mongo = new MongoService();
            InitializeComponent();

            if (dgvUsers != null) UIHelper.SetGridStyle(dgvUsers);

            SetupEvents();
            LoadData();
        }

        private void SetupEvents()
        {
            btnSimpan.Click += BtnSimpan_Click;
            btnHapus.Click += BtnHapus_Click;
            btnClear.Click += (s, e) => ClearInputs();
            dgvUsers.CellClick += DgvUsers_CellClick;

            // Update Role Options dropdown
            cboRole.Items.Clear();
            cboRole.Items.Add("User");
            cboRole.Items.Add("Petugas");
            cboRole.Items.Add("Admin");
            cboRole.SelectedIndex = 0;
        }

        /// <summary>
        /// Memuat data pengguna dari database dan menampilkannya di DataGridView.
        /// Password akan dimasking (••••••••) demi keamanan tampilan.
        /// </summary>
        private async void LoadData()
        {
            try
            {
                var users = await mongo.Users.Find(_ => true).ToListAsync();

                // Proyeksi data anonim untuk masking password di tabel UI
                var displayList = users.Select(u => new
                {
                    u.Id,
                    u.Username,
                    Password = "••••••••", // Masking visual
                    u.Role
                }).ToList();

                dgvUsers.DataSource = displayList;
                if (dgvUsers.Columns["Id"] != null) dgvUsers.Columns["Id"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        /// <summary>
        /// Menangani logika Simpan (Create) dan Update (Edit).
        /// </summary>
        private async void BtnSimpan_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text; // Password input bisa kosong saat update
            string role = cboRole.SelectedItem?.ToString() ?? "Petugas";

            if (string.IsNullOrEmpty(user)) { MessageBox.Show("Username wajib diisi!"); return; }

            try
            {
                // Mode UPDATE
                if (!string.IsNullOrEmpty(selectedId))
                {
                    var updateDef = Builders<User>.Update
                        .Set(u => u.Username, user)
                        .Set(u => u.Role, role);

                    // Hanya update password & hash ulang JIKA field password diisi.
                    // Jika kosong, berarti user tidak ingin mengubah password.
                    if (!string.IsNullOrEmpty(pass))
                    {
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(pass);
                        updateDef = updateDef.Set(u => u.PasswordHash, hashedPassword);
                    }

                    await mongo.Users.UpdateOneAsync(x => x.Id == selectedId, updateDef);
                }
                // Mode INSERT (User Baru)
                else
                {
                    if (string.IsNullOrEmpty(pass)) { MessageBox.Show("Password wajib diisi user baru!"); return; }

                    var exist = await mongo.Users.Find(u => u.Username == user).AnyAsync();
                    if (exist) { MessageBox.Show("Username sudah ada!"); return; }

                    var newUser = new User
                    {
                        Username = user,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(pass),
                        Role = role
                    };
                    await mongo.Users.InsertOneAsync(newUser);
                }

                MessageBox.Show("Data berhasil disimpan!");
                ClearInputs();
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private async void BtnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedId)) return;

            // Mencegah Admin menghapus akun sendiri agar tidak terkunci dari sistem
            if (selectedId == currentUser.Id)
            {
                MessageBox.Show("Tidak bisa menghapus akun sendiri!", "Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Hapus user ini?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await mongo.Users.DeleteOneAsync(x => x.Id == selectedId);
                ClearInputs();
                LoadData();
            }
        }

        private void DgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvUsers.Rows[e.RowIndex];

            selectedId = row.Cells["Id"].Value?.ToString();
            txtUsername.Text = row.Cells["Username"].Value?.ToString();
            cboRole.SelectedItem = row.Cells["Role"].Value?.ToString();

            txtPassword.Clear(); // Kosongkan password saat edit agar tidak membingungkan (placeholder "••••••••" di grid cukup)
            btnSimpan.Text = "Update";
        }

        private void ClearInputs()
        {
            selectedId = "";
            txtUsername.Clear();
            txtPassword.Clear();
            cboRole.SelectedIndex = 0;
            btnSimpan.Text = "Simpan";
        }
    }
}