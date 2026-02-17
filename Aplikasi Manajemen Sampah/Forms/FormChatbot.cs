using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Aplikasi_Manajemen_Sampah.Models;
using Aplikasi_Manajemen_Sampah.Services;
using Aplikasi_Manajemen_Sampah.Data;
using Aplikasi_Manajemen_Sampah.Config;
using MongoDB.Driver;

namespace Aplikasi_Manajemen_Sampah.Forms
{
    /// <summary>
    /// Form utama untuk antarmuka Chatbot AI yang membantu pengguna dengan informasi terkait pengelolaan sampah.
    /// Menggunakan API Mistral AI untuk respons cerdas dan MongoDB untuk penyimpanan riwayat percakapan.
    /// </summary>
    public partial class FormChatbot : Form
    {
        /// <summary>
        /// Klien HTTP untuk berkomunikasi dengan API eksternal (Mistral AI).
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Daftar pesan dalam sesi percakapan saat ini untuk menjaga konteks dialog.
        /// </summary>
        private List<Models.ChatMessage> _conversationHistory;

        /// <summary>
        /// Pengguna yang sedang login saat ini.
        /// </summary>
        private User currentUser;

        // History management
        private MongoService _mongoService;
        private DataSampahService _dataSampahService;
        private string _currentHistoryId = "";

        /// <summary>
        /// Daftar model AI Mistral yang didukung beserta deskripsi ramah pengguna.
        /// </summary>
        private readonly Dictionary<string, string> _mistralModels = new Dictionary<string, string>
        {
            { "mistral-tiny", "Mistral 7B (Cepat & Hemat)" },
            { "mistral-small", "Mixtral 8x7B (Seimbang)" },
            { "mistral-medium", "Mistral Medium (Terbaik)" },
            { "open-mistral-7b", "Open Mistral 7B" },
            { "open-mixtral-8x7b", "Open Mixtral 8x7B" }
        };

        /// <summary>
        /// Inisialisasi instance baru dari <see cref="FormChatbot"/>.
        /// </summary>
        /// <param name="user">objek <see cref="User"/> yang merepresentasikan pengguna yang sedang login.</param>
        public FormChatbot(User user)
        {
            this.currentUser = user;
            InitializeComponent();
            _httpClient = new HttpClient();
            _conversationHistory = new List<Models.ChatMessage>();
            _mongoService = new MongoService();
            _dataSampahService = new DataSampahService();

            // Mencoba mengisi database dengan data awal jika masih kosong
            // agar chatbot memiliki konteks data dasar untuk dijawab.
            try { _dataSampahService.SeedDefaultData(); } catch { }

            InitializeConversation();
            InitializeModelDropdown();
            CustomizeDesign();

            // Menyembunyikan input API Key dari UI karena kunci akses dikelola di backend/konfigurasi
            // untuk alasan keamanan dan penyederhanaan antarmuka pengguna.
            txtApiKey.Visible = false;
            lblApiKey.Visible = false;
            lblModel.Location = new Point(17, 13);
            cmbModel.Location = new Point(17, 40);
            lblModelInfo.Location = new Point(260, 44);

            // Memuat daftar riwayat percakapan sebelumnya agar pengguna bisa melanjutkan sesi lama.
            LoadHistoryList();
        }

        /// <summary>
        /// Mengatur tata letak dan gaya visual komponen agar sesuai dengan tema aplikasi.
        /// </summary>
        private void CustomizeDesign()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);

            if (panelHeader != null)
            {
                panelHeader.BackColor = Color.FromArgb(30, 50, 40);
                lblTitle.ForeColor = Color.White;
            }

            if (txtConversation != null)
            {
                txtConversation.BackColor = Color.White;
                txtConversation.Font = new Font("Segoe UI", 10F);
            }

            if (txtMessage != null)
            {
                txtMessage.Font = new Font("Segoe UI", 10F);
            }

            UIHelper.SetPrimaryButton(btnSend);
            UIHelper.SetSecondaryButton(btnClear);
            UIHelper.SetSecondaryButton(btnSaveChat);
        }

        /// <summary>
        /// Mengisi dropdown pilihan model AI dengan data dari kamus model yang tersedia.
        /// </summary>
        private void InitializeModelDropdown()
        {
            cmbModel.Items.Clear();
            foreach (var model in _mistralModels)
            {
                cmbModel.Items.Add($"{model.Key}");
            }
            cmbModel.SelectedIndex = 0;
        }

        /// <summary>
        /// Menangani perubahan pilihan model AI oleh pengguna.
        /// </summary>
        private void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbModel.SelectedItem != null)
            {
                string selectedKey = cmbModel.SelectedItem.ToString();
                if (_mistralModels.ContainsKey(selectedKey))
                {
                    lblModelInfo.Text = $"Model: {_mistralModels[selectedKey]}";
                }
            }
        }

        /// <summary>
        /// Memulai sesi percakapan baru dengan menyuntikkan prompt sistem.
        /// Prompt ini berisi instruksi kepribadian AI dan konteks data sampah Jawa Barat.
        /// </summary>
        private void InitializeConversation()
        {
            // Mengambil data statistik statis sebagai pengetahuan dasar
            string staticKnowledge = DataSampahJabar.GetKnowledgeBase();

            // Mengambil data spesifik lokasi dari database untuk memberikan jawaban yang kontekstual
            // berdasarkan data real-time yang ada di sistem.
            string dbKnowledge = "";
            try { dbKnowledge = _dataSampahService.GetKnowledgeBaseFromDb(); } catch { }

            _conversationHistory.Add(new Models.ChatMessage
            {
                role = "system",
                content = $"Anda adalah asisten AI khusus untuk Aplikasi Manajemen Bank Sampah. " +
                          $"Nama pengguna: {currentUser.Username}, Role: {currentUser.Role}. " +
                          "\n\nATURAN PENTING:\n" +
                          "1. Anda HANYA boleh menjawab pertanyaan tentang: sampah, pengelolaan sampah, " +
                          "bank sampah, daur ulang, lingkungan hidup, kebersihan, komposting, limbah, " +
                          "polusi, 3R (Reduce Reuse Recycle), TPA, dan topik terkait lingkungan.\n" +
                          "2. Jika pengguna bertanya di LUAR topik tersebut, jawab dengan sopan: " +
                          "'Maaf, saya hanya bisa membantu dengan topik seputar sampah dan pengelolaan lingkungan. " +
                          "Silakan ajukan pertanyaan tentang sampah, daur ulang, atau lingkungan hidup.'\n" +
                          "3. Gunakan bahasa Indonesia yang ramah dan mudah dipahami.\n" +
                          "4. Berikan jawaban yang informatif, praktis, dan berdasarkan data jika tersedia.\n" +
                          "5. Gunakan data statistik Jawa Barat berikut sebagai referensi:\n\n" +
                          staticKnowledge + "\n\n" + dbKnowledge
            });

            UpdateConversationDisplay();
        }

        // ==========================================
        //  KIRIM PESAN
        // ==========================================

        private async void btnSend_Click(object sender, EventArgs e)
        {
            await SendMessage();
        }

        private async void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kirim pesan saat tombol Enter ditekan tanpa Shift (Shift+Enter untuk baris baru)
            if (e.KeyChar == (char)Keys.Enter && !ModifierKeys.HasFlag(Keys.Shift))
            {
                e.Handled = true;
                await SendMessage();
            }
        }

        /// <summary>
        /// Mengirim pesan pengguna ke API AI dan menangani responsnya.
        /// </summary>
        private async Task SendMessage()
        {
            string userMessage = txtMessage.Text.Trim();

            if (string.IsNullOrEmpty(userMessage))
            {
                MessageBox.Show("Silakan ketik pesan terlebih dahulu.", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Melakukan validasi awal topik di sisi klien untuk mengurangi beban request yang tidak relevan
            if (!IsTopicRelated(userMessage))
            {
                // Tetap kirim ke AI, namun ini bisa jadi indikasi awal untuk logging atau handling khusus di masa depan
            }

            _conversationHistory.Add(new Models.ChatMessage
            {
                role = "user",
                content = userMessage
            });

            UpdateConversationDisplay();

            string originalMessage = userMessage;
            txtMessage.Clear();
            txtMessage.Focus();

            try
            {
                btnSend.Enabled = false;
                btnSend.Text = "Mengirim...";

                string response = await GetMistralResponse(originalMessage);

                _conversationHistory.Add(new Models.ChatMessage
                {
                    role = "assistant",
                    content = response
                });

                UpdateConversationDisplay();
                lblStatus.Text = "Siap";

                // Menyimpan riwayat percakapan secara otomatis setiap kali ada interaksi baru
                // untuk mencegah kehilangan data jika aplikasi tertutup tiba-tiba.
                await SaveCurrentHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Error terjadi";

                // Menghapus pesan user terakhir dari history lokal jika gagal terkirim
                // agar status UI tetap konsisten dengan status server.
                _conversationHistory.RemoveAt(_conversationHistory.Count - 1);

                _conversationHistory.Add(new Models.ChatMessage
                {
                    role = "assistant",
                    content = $"❌ Gagal mendapatkan respons: {ex.Message}"
                });
                UpdateConversationDisplay();
            }
            finally
            {
                btnSend.Enabled = true;
                btnSend.Text = "Kirim";
            }
        }

        /// <summary>
        /// Mengirim request ke endpoint API Mistral dan mengembalikan konten respons.
        /// </summary>
        /// <param name="userMessage">Pesan teks dari pengguna.</param>
        /// <returns>String berisi jawaban dari AI.</returns>
        /// <exception cref="Exception">Melempar exception jika API key invalid, rate limit, atau error server lainnya.</exception>
        private async Task<string> GetMistralResponse(string userMessage)
        {
            lblStatus.Text = "Mengirim ke Mistral AI...";

            string selectedModel = GetSelectedModel();

            var requestData = new
            {
                model = selectedModel,
                messages = _conversationHistory,
                max_tokens = 2000,
                temperature = 0.7,
                top_p = 0.9,
                stream = false
            };

            string jsonData = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AppConfig.MistralApiKey}");

            HttpResponseMessage response = await _httpClient.PostAsync(AppConfig.MistralApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);

                string errorMessage = errorResponse?.error?.message ?? "Unknown error";

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception($"API Key tidak valid. Periksa kembali API Key Anda.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    throw new Exception($"Terlalu banyak request. Tunggu sebentar dan coba lagi.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new Exception($"Request tidak valid: {errorMessage}");
                }
                else
                {
                    throw new Exception($"Mistral API Error ({response.StatusCode}): {errorMessage}");
                }
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<MistralApiResponse>(responseContent);

            lblStatus.Text = "Menerima respons...";
            return apiResponse.choices[0].message.content;
        }

        /// <summary>
        /// Mendapatkan kode model AI yang dipilih dari dropdown.
        /// </summary>
        /// <returns>String kode model (contoh: "mistral-tiny").</returns>
        private string GetSelectedModel()
        {
            if (cmbModel.SelectedItem == null)
                return "mistral-tiny";

            return cmbModel.SelectedItem.ToString();
        }

        /// <summary>
        /// Memperbarui tampilan RichTextBox dengan riwayat percakapan terbaru.
        /// Mengatur format warna dan gaya font untuk membedakan pengirim.
        /// </summary>
        private void UpdateConversationDisplay()
        {
            txtConversation.Clear();

            foreach (var message in _conversationHistory)
            {
                if (message.role == "system") continue;

                // 1. Render Nama Pengirim dengan Warna Berbeda
                string prefix = message.role == "user" ? "👤 Anda: " : "🤖 Asisten: ";
                Color color = message.role == "user" ? Color.FromArgb(30, 50, 40) : Color.FromArgb(46, 204, 113);
                
                txtConversation.SelectionColor = color;
                txtConversation.SelectionFont = new Font(txtConversation.Font, FontStyle.Bold);
                txtConversation.AppendText($"{prefix}\n");

                // 2. Render Isi Pesan dengan Formatting Teks
                txtConversation.SelectionColor = Color.Black;
                txtConversation.SelectionFont = new Font(txtConversation.Font, FontStyle.Regular);
                
                AppendFormattedMessage(message.content);
                
                txtConversation.AppendText("\n"); // Spasi antar pesan untuk keterbacaan
            }

            txtConversation.ScrollToCaret();
        }

        /// <summary>
        /// Memformat pesan teks menjadi tampilan yang lebih terstruktur di RichTextBox.
        /// Mendukung deteksi header, list, dan tabel sederhana.
        /// </summary>
        /// <param name="content">Konten pesan teks.</param>
        private void AppendFormattedMessage(string content)
        {
            var lines = content.Split('\n');
            bool isTable = false;

            foreach (var line in lines)
            {
                string text = line.Trim();
                if (string.IsNullOrEmpty(text))
                {
                    txtConversation.AppendText("\n");
                    continue;
                }

                // A. TABLE HANDLING (sederhana: ganti font monospace)
                // Deteksi baris yang dimulai dengan '|' sebagai bagian dari tabel Markdown
                if (text.StartsWith("|"))
                {
                    txtConversation.SelectionFont = new Font("Consolas", 9, FontStyle.Regular); // Monospace agar kolom sejajar
                    txtConversation.AppendText(text.Replace("**", "") + "\n"); // Hapus bold di tabel biar rapi
                    continue;
                }

                // Kembali ke font normal jika bukan tabel
                txtConversation.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);

                // B. HEADERS (###)
                if (text.StartsWith("#"))
                {
                    // Hapus karakter pagar penanda header
                    string headerText = text.TrimStart('#', ' ');
                    txtConversation.SelectionFont = new Font("Segoe UI", 11, FontStyle.Bold); // Lebih besar & bold
                    // Process inline bolding di header juga
                    ProcessInlineFormatting(headerText);
                    txtConversation.AppendText("\n");
                    continue;
                }

                // C. LIST ITEMS (- atau *)
                if (text.StartsWith("- ") || text.StartsWith("* "))
                {
                    txtConversation.SelectionBullet = true;
                    txtConversation.SelectionIndent = 15;
                    ProcessInlineFormatting(text.Substring(2));
                    txtConversation.SelectionBullet = false;
                    txtConversation.SelectionIndent = 0;
                    txtConversation.AppendText("\n");
                    continue;
                }

                // D. NORMAL TEXT (dengan inline formatting)
                ProcessInlineFormatting(line);
                txtConversation.AppendText("\n");
            }
        }

        /// <summary>
        /// Memproses format teks tebal (bold) yang ditandai dengan **text**.
        /// </summary>
        /// <param name="text">Teks yang akan diproses.</param>
        private void ProcessInlineFormatting(string text)
        {
            // Split berdasarkan tanda ** (bold marker)
            var parts = text.Split(new[] { "**" }, StringSplitOptions.None);
            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 1) // Bagian ganjil adalah teks di dalam **...** (BOLD)
                {
                    var currentFont = txtConversation.SelectionFont;
                    txtConversation.SelectionFont = new Font(currentFont, FontStyle.Bold);
                    txtConversation.AppendText(parts[i]);
                }
                else // Bagian genap adalah teks biasa
                {
                    var currentFont = txtConversation.SelectionFont;
                    txtConversation.SelectionFont = new Font(currentFont, FontStyle.Regular);
                    txtConversation.AppendText(parts[i]);
                }
            }
        }

        // ==========================================
        //  HISTORY MANAGEMENT (MongoDB)
        // ==========================================

        /// <summary>
        /// Menyimpan riwayat percakapan saat ini ke database MongoDB.
        /// </summary>
        private async Task SaveCurrentHistory()
        {
            try
            {
                // Mencegah penyimpanan jika percakapan hanya berisi sistem prompt (kosong dari sisi user)
                if (_conversationHistory.Count <= 1) return;

                var historyDoc = new ChatHistory
                {
                    UserId = currentUser.Id,
                    Messages = _conversationHistory,
                    LastModified = DateTime.Now
                };

                if (string.IsNullOrEmpty(_currentHistoryId))
                {
                    // Buat dokumen history baru
                    historyDoc.Title = GenerateHistoryTitle();
                    historyDoc.CreatedDate = DateTime.Now;
                    await _mongoService.ChatHistory.InsertOneAsync(historyDoc);
                    _currentHistoryId = historyDoc.Id;
                }
                else
                {
                    // Update dokumen history yang sudah ada
                    var filter = Builders<ChatHistory>.Filter.Eq(h => h.Id, _currentHistoryId);
                    var existing = await _mongoService.ChatHistory.Find(filter).FirstOrDefaultAsync();
                    if (existing != null)
                    {
                        existing.Messages = _conversationHistory;
                        existing.LastModified = DateTime.Now;
                        await _mongoService.ChatHistory.ReplaceOneAsync(filter, existing);
                    }
                }

                LoadHistoryList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving history: {ex.Message}");
            }
        }

        /// <summary>
        /// Memuat riwayat percakapan spesifik dari database berdasarkan ID.
        /// </summary>
        /// <param name="historyId">ID dokumen history yang akan dimuat.</param>
        private async Task LoadHistory(string historyId)
        {
            try
            {
                var filter = Builders<ChatHistory>.Filter.Eq(h => h.Id, historyId);
                var history = await _mongoService.ChatHistory.Find(filter).FirstOrDefaultAsync();

                if (history != null)
                {
                    _currentHistoryId = history.Id;
                    _conversationHistory = history.Messages;
                    UpdateConversationDisplay();
                    lblStatus.Text = $"Dimuat: {history.Title}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat history: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Menghapus riwayat percakapan tertentu dari database.
        /// </summary>
        /// <param name="historyId">ID dokumen history yang akan dihapus.</param>
        private async Task DeleteHistory(string historyId)
        {
            try
            {
                var filter = Builders<ChatHistory>.Filter.Eq(h => h.Id, historyId);
                await _mongoService.ChatHistory.DeleteOneAsync(filter);

                // Jika yang dihapus adalah percakapan aktif, reset tampilan ke percakapan baru
                if (_currentHistoryId == historyId)
                {
                    await CreateNewConversation();
                }
                LoadHistoryList();
                lblStatus.Text = "History dihapus";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menghapus: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Memulai sesi percakapan baru.
        /// Menyimpan percakapan lama terlebih dahulu jika ada isinya.
        /// </summary>
        private async Task CreateNewConversation()
        {
            // Simpan percakapan saat ini sebelum membuat baru jika belum tersimpan
            if (_conversationHistory.Count > 1)
            {
                await SaveCurrentHistory();
            }

            _currentHistoryId = "";
            _conversationHistory.Clear();
            InitializeConversation();
            txtMessage.Clear();
            LoadHistoryList();
            lblStatus.Text = "Percakapan baru dimulai";
        }

        /// <summary>
        /// Mengambil daftar riwayat percakapan milik user dari database untuk ditampilkan di sidebar.
        /// </summary>
        private void LoadHistoryList()
        {
            try
            {
                var filter = Builders<ChatHistory>.Filter.Eq(h => h.UserId, currentUser.Id);
                var sort = Builders<ChatHistory>.Sort.Descending(h => h.LastModified);
                var histories = _mongoService.ChatHistory.Find(filter).Sort(sort).Limit(50).ToList();

                lstHistory.Items.Clear();
                foreach (var history in histories)
                {
                    lstHistory.Items.Add(new HistoryListItem
                    {
                        Id = history.Id,
                        DisplayText = $"{history.Title} ({history.LastModified:dd/MM HH:mm})"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading history list: {ex.Message}");
            }
        }

        /// <summary>
        /// Membuat judul percakapan secara otomatis dari pesan pertama pengguna.
        /// </summary>
        /// <returns>Judul singkat (max 50 karakter) atau "Percakapan Baru".</returns>
        private string GenerateHistoryTitle()
        {
            var firstUserMessage = _conversationHistory.FirstOrDefault(m => m.role == "user");
            if (firstUserMessage != null)
            {
                string title = firstUserMessage.content;
                if (title.Length > 50)
                    title = title.Substring(0, 47) + "...";
                return title;
            }
            return "Percakapan Baru";
        }

        // ==========================================
        //  EVENT HANDLERS
        // ==========================================

        private void btnClear_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Apakah Anda yakin ingin membersihkan percakapan?",
                "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _conversationHistory.Clear();
                InitializeConversation();
                txtMessage.Clear();
                lblStatus.Text = "Percakapan dibersihkan";
            }
        }

        private void txtApiKey_TextChanged(object sender, EventArgs e)
        {
            // Handler kosong: Tidak digunakan lagi karena API key tidak diinput via UI.
        }

        private void btnSaveChat_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text Files (*.txt)|*.txt|JSON Files (*.json)|*.json";
            saveDialog.Title = "Simpan Percakapan";
            saveDialog.FileName = $"ChatBot_{DateTime.Now:yyyyMMdd_HHmmss}";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string content = "";

                    if (saveDialog.FilterIndex == 2)
                    {
                        content = JsonConvert.SerializeObject(_conversationHistory, Formatting.Indented);
                    }
                    else
                    {
                        content += $"=== PERCAKAPAN CHATBOT BANK SAMPAH ===\n";
                        content += $"User: {currentUser.Username} ({currentUser.Role})\n";
                        content += $"Tanggal: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n";
                        content += $"=====================================\n\n";

                        foreach (var message in _conversationHistory)
                        {
                            if (message.role == "system") continue;
                            content += $"{message.role.ToUpper()}: {message.content}\n\n";
                            content += "---\n\n";
                        }
                    }

                    System.IO.File.WriteAllText(saveDialog.FileName, content);
                    MessageBox.Show("Percakapan berhasil disimpan!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Gagal menyimpan: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // History panel event handlers
        private async void btnNewChat_Click(object sender, EventArgs e)
        {
            await CreateNewConversation();
        }

        private async void btnDeleteHistory_Click(object sender, EventArgs e)
        {
            if (lstHistory.SelectedItem is HistoryListItem selected)
            {
                var result = MessageBox.Show($"Hapus history ini?\n{selected.DisplayText}",
                    "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    await DeleteHistory(selected.Id);
                }
            }
            else
            {
                MessageBox.Show("Pilih history terlebih dahulu.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void lstHistory_DoubleClick(object sender, EventArgs e)
        {
            if (lstHistory.SelectedItem is HistoryListItem selected)
            {
                await LoadHistory(selected.Id);
            }
        }



        // ==========================================
        //  TOPIC VALIDATION
        // ==========================================

        /// <summary>
        /// Memeriksa apakah pesan pengguna relevan dengan topik pengelolaan sampah.
        /// </summary>
        /// <param name="message">Pesan dari pengguna.</param>
        /// <returns>True jika relevan, False jika off-topic (namun saat ini masih permisif).</returns>
        private bool IsTopicRelated(string message)
        {
            string[] keywords = {
                "sampah", "limbah", "daur ulang", "recycle", "recycling",
                "bank sampah", "kompos", "organik", "anorganik", "plastik",
                "kertas", "kaca", "logam", "b3", "berbahaya",
                "lingkungan", "polusi", "pencemaran", "kebersihan", "bersih",
                "tpa", "tempat pembuangan", "reduce", "reuse", "3r",
                "insinerasi", "rdf", "refuse", "pengelolaan", "kelola",
                "timbulan", "emisi", "udara", "air", "tanah",
                "laut", "sungai", "citarum", "sarimukti", "jabar",
                "jawa barat", "bandung", "ramah lingkungan", "eco",
                "sustainability", "berkelanjutan", "hijau", "green",
                "pupuk", "komposting", "gaslah", "pemilah", "pilah",
                "volume sampah", "data sampah", "statistik",
                "halo", "hai", "selamat", "terima kasih", "makasih", "tolong", "bantu"
            };

            string lowerMessage = message.ToLower();
            foreach (string keyword in keywords)
            {
                if (lowerMessage.Contains(keyword))
                    return true;
            }
            return false;
        }

        // ==========================================
        //  HELPER CLASSES
        // ==========================================

        /// <summary>
        /// Representasi item riwayat percakapan untuk ditampilkan di ListBox.
        /// </summary>
        private class HistoryListItem
        {
            public string Id { get; set; }
            public string DisplayText { get; set; }

            public override string ToString()
            {
                return DisplayText;
            }
        }

        /// <summary>
        /// Struktur data respons JSON dari API Mistral.
        /// </summary>
        public class MistralApiResponse
        {
            public string id { get; set; }
            public string @object { get; set; }
            public long created { get; set; }
            public string model { get; set; }
            public List<Choice> choices { get; set; }
            public Usage usage { get; set; }
        }

        public class Choice
        {
            public int index { get; set; }
            public ChatMessage message { get; set; }
            public string finish_reason { get; set; }
        }

        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int total_tokens { get; set; }
            public int completion_tokens { get; set; }
        }

        public class ErrorResponse
        {
            public ErrorDetail error { get; set; }
        }

        public class ErrorDetail
        {
            public string message { get; set; }
            public string type { get; set; }
            public string param { get; set; }
            public string code { get; set; }
        }
    }
}