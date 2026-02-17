using System.Collections.Generic;

namespace Aplikasi_Manajemen_Sampah.Services
{
    /// <summary>
    /// Kelas statis yang menyediakan data referensi lokasi (koordinat Geospasial).
    /// Berisi daftar koordinat Kab/Kota di Jawa Barat dan lokasi Bank Sampah / TPS 3R.
    /// </summary>
    public static class LocationData
    {
        /// <summary>
        /// Dictionary berisi koordinat Latitude & Longitude untuk setiap Kota dan Kabupaten di Jawa Barat.
        /// Key: Nama Kota/Kabupaten, Value: (Latitude, Longitude).
        /// </summary>
        public static readonly Dictionary<string, (double Lat, double Lng)> CityCoords = new Dictionary<string, (double Lat, double Lng)>
        {
            { "Kota Bandung", (-6.9175, 107.6191) },
            { "Kab. Bandung", (-7.0252, 107.5197) },
            { "Kab. Bandung Barat", (-6.8279, 107.4566) },
            { "Kota Bogor", (-6.5971, 106.8060) },
            { "Kab. Bogor", (-6.5518, 106.6291) },
            { "Kota Bekasi", (-6.2383, 106.9756) },
            { "Kab. Bekasi", (-6.3642, 107.1724) },
            { "Kota Depok", (-6.4025, 106.7942) },
            { "Kota Cimahi", (-6.8722, 107.5422) },
            { "Kota Tasikmalaya", (-7.3274, 108.2207) },
            { "Kab. Tasikmalaya", (-7.3595, 108.1065) },
            { "Kota Sukabumi", (-6.9277, 106.9299) },
            { "Kab. Sukabumi", (-6.9890, 106.8159) },
            { "Kota Cirebon", (-6.7320, 108.5523) },
            { "Kab. Cirebon", (-6.7570, 108.4800) },
            { "Kota Banjar", (-7.3745, 108.5583) },
            { "Kab. Cianjur", (-6.8206, 107.1429) },
            { "Kab. Garut", (-7.2279, 107.9087) },
            { "Kab. Indramayu", (-6.3268, 108.3207) },
            { "Kab. Karawang", (-6.3013, 107.2917) },
            { "Kab. Kuningan", (-6.9769, 108.4813) },
            { "Kab. Majalengka", (-6.8364, 108.2278) },
            { "Kab. Pangandaran", (-7.7013, 108.4950) },
            { "Kab. Purwakarta", (-6.5564, 107.4428) },
            { "Kab. Subang", (-6.5716, 107.7587) },
            { "Kab. Sumedang", (-6.8586, 107.9266) },
            { "Kab. Ciamis", (-7.3262, 108.3537) }
        };

        /// <summary>
        /// Daftar lokasi Bank Sampah dan Pusat Daur Ulang statis (Data Dummy).
        /// Digunakan untuk visualisasi pin pada peta.
        /// Format: (Nama Tempat, Latitude, Longitude).
        /// </summary>
        public static readonly List<(string Nama, double Lat, double Lng)> BankSampahList = new List<(string, double, double)>
        {
            ("Bank Sampah Induk Kota Bandung", -6.943097, 107.633545),
            ("TPS 3R Sarijadi", -6.880562, 107.576856),
            ("Bank Sampah Bersinar Solokan Jeruk", -7.0252, 107.7597),
            ("Rumah Kompos ITB", -6.890691, 107.609459),
            ("Bank Sampah Resik PD Kebersihan", -6.920512, 107.615213),
            ("TPST Bantargebang (Bekasi)", -6.3642, 106.9956),
            ("Bank Sampah Gemah Ripah", -6.9500, 107.6000),
            ("Pusat Daur Ulang Bogor", -6.5971, 106.7900),
            ("Bank Sampah Depok Bersih", -6.3900, 106.8100),
            ("TPS Terpadu Cimahi", -6.8800, 107.5300),
            ("Bank Sampah Karawang Hijau", -6.3100, 107.3000),
            ("Bank Sampah Cirebon Berintan", -6.7200, 108.5500),
            ("Bank Sampah Sukabumi Pelita", -6.9200, 106.9200),
            ("Bank Sampah Tasik Resik", -7.3300, 108.2100)
        };
    }
}
