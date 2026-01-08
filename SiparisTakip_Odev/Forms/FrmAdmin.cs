using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SiparisTakip_Odev.Data;

namespace SiparisTakip_Odev.Forms
{
    public partial class FrmAdmin : Form
    {      

        private Form _owner;

        public FrmAdmin(Form owner)
        {
            _owner = owner;
            InitializeComponent();
            
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            btnUpdateStatus.Click += BtnUpdateStatus_Click;
            btnUpdate.Click += BtnUpdate_Click;
            dgv.SelectionChanged += Dgv_SelectionChanged;
            Load += FrmAdmin_Load;

            SiparisTakip_Odev.Data.Theme.ApplyToForm(this);
            SiparisTakip_Odev.Data.Theme.ThemeChanged += (s, e) => SiparisTakip_Odev.Data.Theme.ApplyToForm(this);
        }

        private void FrmAdmin_Load(object sender, EventArgs e)
        {
            LoadProducts();
            LoadOrders();
        }

        private void LoadProducts()
        {
            var dt = Db.GetTable("SELECT Id,UrunAdi,Fiyat,Stok FROM Urunler");
            dgv.DataSource = dt;
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            txtName.Text = dgv.CurrentRow.Cells["UrunAdi"].Value?.ToString();
            txtPrice.Text = dgv.CurrentRow.Cells["Fiyat"].Value?.ToString();
            txtStock.Text = dgv.CurrentRow.Cells["Stok"].Value?.ToString();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Güncellenecek ürün seçiniz"); return; }
            var id = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            if (string.IsNullOrWhiteSpace(txtName.Text) || !decimal.TryParse(txtPrice.Text, out decimal fiyat) || !int.TryParse(txtStock.Text, out int stok))
            {
                MessageBox.Show("Geçersiz alan");
                return;
            }
            var sql = "UPDATE Urunler SET UrunAdi=@u, Fiyat=@f, Stok=@s WHERE Id=@id";
            Db.Execute(sql,
                new SqlParameter("@u", txtName.Text),
                new SqlParameter("@f", fiyat),
                new SqlParameter("@s", stok),
                new SqlParameter("@id", id));
            LoadProducts();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || !decimal.TryParse(txtPrice.Text, out decimal fiyat) || !int.TryParse(txtStock.Text, out int stok))
            {
                MessageBox.Show("Geçersiz alan");
                return;
            }

            var sql = "INSERT INTO Urunler (UrunAdi,Fiyat,Stok) VALUES (@u,@f,@s)";
            var rows = Db.Execute(sql,
                new SqlParameter("@u", txtName.Text),
                new SqlParameter("@f", fiyat),
                new SqlParameter("@s", stok));
            if (rows > 0)
            {
                LoadProducts();
                txtName.Clear(); txtPrice.Clear(); txtStock.Clear();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv.CurrentRow == null)
                {
                    MessageBox.Show("Silinecek ürün seçiniz");
                    return;
                }
                var id = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);

                var sqlDetails = "DELETE FROM SiparisDetaylar WHERE UrunId=@id";
                var sqlDelete = "DELETE FROM Urunler WHERE Id=@id";
                Db.Execute(sqlDetails, new SqlParameter("@id", id));
                Db.Execute(sqlDelete, new SqlParameter("@id", id));
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme iþlemi baþarýsýz: " + ex.Message);
            }
        }

        private void LoadOrders()
        {
            var dt = Db.GetTable("SELECT Id,SiparisTarihi=Tarih,Toplam,Durum,KullaniciId FROM Siparisler ORDER BY Tarih DESC");
            dgvOrders.DataSource = dt;
        }

        private void BtnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count == 0) { MessageBox.Show("Sipariþ seçiniz"); return; }
            if (cmbStatus.SelectedItem == null) { MessageBox.Show("Durum seçiniz"); return; }
            var durum = cmbStatus.SelectedItem.ToString();
            try
            {
                foreach (DataGridViewRow row in dgvOrders.SelectedRows)
                {
                    var id = Convert.ToInt32(row.Cells["Id"].Value);
                    Db.Execute("UPDATE Siparisler SET Durum=@d WHERE Id=@id", new SqlParameter("@d", durum), new SqlParameter("@id", id));
                }
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Durum güncellenirken hata: " + ex.Message);
            }
        }

    }
}
