using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SiparisTakip_Odev.Data;

namespace SiparisTakip_Odev.Forms
{
    public class FrmOrders : Form
    {
        private int _userId;
        private DataGridView dgvOrders;
        private DataGridView dgvDetails;

        public FrmOrders(int userId)
        {
            _userId = userId;
            Text = "Sipariþlerim";
            Width = 800; Height = 600; StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false;

            dgvOrders = new DataGridView { Left = 10, Top = 10, Width = 760, Height = 250, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvDetails = new DataGridView { Left = 10, Top = 280, Width = 760, Height = 250, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            dgvOrders.SelectionChanged += DgvOrders_SelectionChanged;

            Controls.AddRange(new Control[] { dgvOrders, dgvDetails });

            Load += FrmOrders_Load;
            // Apply theme
            SiparisTakip_Odev.Data.Theme.ApplyToForm(this);
        }

        private void FrmOrders_Load(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void LoadOrders()
        {
            // explicit select
            // Select only orders for this user
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmOrders
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "FrmOrders";
            this.ResumeLayout(false);

        }
    }
}
