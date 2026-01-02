using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aplikasi_Manajemen_Sampah.Services
{
    public class PdfService
    {
        private readonly IMongoCollection<Sampah> _sampahCollection;

        public PdfService()
        {
            var mongo = new MongoService();
            _sampahCollection = mongo.Database.GetCollection<Sampah>("Sampah");
        }

        // Update method ini supaya menerima parameter Tanggal
        public async Task ExportLaporanAsync(DateTime tglMulai, DateTime tglAkhir)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = "Laporan_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".pdf";
            savefile.Filter = "PDF Files|*.pdf";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 1. FILTER BERDASARKAN TANGGAL
                    var builder = Builders<Sampah>.Filter;

                    // Kita ambil dari jam 00:00:00 tanggal mulai
                    var start = tglMulai.Date;
                    // Sampai jam 23:59:59 tanggal akhir
                    var end = tglAkhir.Date.AddDays(1).AddTicks(-1);

                    // Logika: TanggalMasuk >= start DAN TanggalMasuk <= end
                    var filter = builder.Gte(x => x.TanggalMasuk, start) &
                                 builder.Lte(x => x.TanggalMasuk, end);

                    var dataSampah = await _sampahCollection.Find(filter).ToListAsync();

                    if (dataSampah.Count == 0)
                    {
                        MessageBox.Show($"Tidak ada data sampah pada rentang tanggal:\n{start:dd/MM/yyyy} s/d {end:dd/MM/yyyy}", "Data Kosong");
                        return;
                    }

                    // 2. MULAI TULIS PDF
                    using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                    {
                        PdfWriter writer = new PdfWriter(stream);
                        PdfDocument pdf = new PdfDocument(writer);
                        Document document = new Document(pdf);

                        // Judul Laporan
                        document.Add(new Paragraph("Laporan Data Sampah")
                            .SetFontSize(20)
                            .SetTextAlignment(TextAlignment.CENTER));

                        // Sub-judul (Periode)
                        document.Add(new Paragraph($"Periode: {start:dd/MM/yyyy} - {end:dd/MM/yyyy}")
                            .SetFontSize(12)
                            .SetTextAlignment(TextAlignment.CENTER));

                        document.Add(new Paragraph("\n"));

                        // Tabel
                        Table table = new Table(5);
                        table.SetWidth(UnitValue.CreatePercentValue(100));

                        table.AddHeaderCell("Tanggal");
                        table.AddHeaderCell("User");
                        table.AddHeaderCell("Item");
                        table.AddHeaderCell("Jenis");
                        table.AddHeaderCell("Berat (Kg)");

                        foreach (var item in dataSampah)
                        {
                            table.AddCell(new Paragraph(item.TanggalMasuk.ToString("dd/MM/yyyy")));
                            table.AddCell(new Paragraph(item.InputBy ?? "-"));
                            table.AddCell(new Paragraph(item.Nama ?? "-"));
                            table.AddCell(new Paragraph(item.Jenis ?? "-"));
                            table.AddCell(new Paragraph(item.BeratKg.ToString()));
                        }

                        document.Add(table);
                        document.Close();
                    }

                    MessageBox.Show("Laporan PDF Berhasil Dibuat!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException)
                {
                    MessageBox.Show("Gagal menyimpan! File PDF sedang terbuka. Tutup dulu file tersebut.", "File Terkunci");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}