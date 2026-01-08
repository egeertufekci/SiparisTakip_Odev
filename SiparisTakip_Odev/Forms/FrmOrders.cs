using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SiparisTakip_Odev.Data;

namespace SiparisTakip_Odev.Forms
{
    public partial class FrmOrders : Form
    {
        private int _userId;

        public FrmOrders(int userId)
        {
            _userId = userId;
            InitializeComponent();

            dgvOrders.SelectionChanged += DgvOrders_SelectionChanged;
            Load += FrmOrders_Load;

            SiparisTakip_Odev.Data.Theme.ApplyToForm(this);
        }

        private void FrmOrders_Load(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void LoadOrders()
        {
            var dt = Db.GetTable("SELECT Id, Tarih, Toplam, Durum FROM Siparisler WHERE KullaniciId=@k ORDER BY Tarih DESC", new SqlParameter("@k", _userId));
            dgvOrders.DataSource = dt;
        }

        private void DgvOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrders.CurrentRow == null) return;
            var id = Convert.ToInt32(dgvOrders.CurrentRow.Cells["Id"].Value);
            var dt = Db.GetTable("SELECT sd.Id, sd.UrunId, u.UrunAdi, sd.Adet, sd.BirimFiyat, (sd.Adet*sd.BirimFiyat) AS Tutar FROM SiparisDetaylar sd JOIN Urunler u ON sd.UrunId=u.Id WHERE sd.SiparisId=@s", new SqlParameter("@s", id));
            dgvDetails.DataSource = dt;
        }
    }
}
