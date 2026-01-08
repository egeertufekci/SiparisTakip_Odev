using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using SiparisTakip_Odev.Data;

namespace SiparisTakip_Odev.Forms
{
    public partial class FrmMain : Form
    {
        private int _userId;
        private System.ComponentModel.BindingList<CartItem> cart = new System.ComponentModel.BindingList<CartItem>();
        private Form _owner;

        public FrmMain(Form owner, int userId)
        {
            _owner = owner;
            _userId = userId;
            InitializeComponent();
            btnAddToCart.Click += BtnAddToCart_Click;
            btnRemoveFromCart.Click += BtnRemoveFromCart_Click;
            btnCreateOrder.Click += BtnCreateOrder_Click;
            btnLogout.Click += BtnLogout_Click;
            btnMyOrders.Click += BtnMyOrders_Click;
            cmbTheme.SelectedIndexChanged += CmbTheme_SelectedIndexChanged;
            dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;
            Load += FrmMain_Load;
        }

        private void DgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                lblSelectedProduct.Text = "Seçili Ürün: -";
                return;
            }
            if (dgvProducts.SelectedRows.Count > 1)
            {
                lblSelectedProduct.Text = $"Seçili Ürünler: {dgvProducts.SelectedRows.Count} adet";
            }
            else
            {
                var row = dgvProducts.SelectedRows[0];
                lblSelectedProduct.Text = "Seçili Ürün: " + row.Cells["UrunAdi"].Value?.ToString();
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadProducts();
            RefreshCartGrid();
            cmbTheme.SelectedItem = SiparisTakip_Odev.Data.Theme.Current;
            SiparisTakip_Odev.Data.Theme.ApplyToForm(this);
            SiparisTakip_Odev.Data.Theme.ThemeChanged += (s, ea) => SiparisTakip_Odev.Data.Theme.ApplyToForm(this);
        }

        private void CmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTheme.SelectedItem == null) return;
            var theme = cmbTheme.SelectedItem.ToString();
            SiparisTakip_Odev.Data.Theme.Set(theme);
            SiparisTakip_Odev.Data.Theme.ApplyToAllOpenForms();
        }

        private void LoadProducts()
        {
            var dt = Db.GetTable("SELECT Id,UrunAdi,Fiyat,Stok FROM Urunler WHERE Stok>0");
            dgvProducts.DataSource = dt;
        }

        private void RefreshCartGrid()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("UrunId", typeof(int));
            dt.Columns.Add("UrunAdi", typeof(string));
            dt.Columns.Add("Adet", typeof(int));
            dt.Columns.Add("BirimFiyat", typeof(decimal));
            dt.Columns.Add("Tutar", typeof(decimal));

            foreach (var c in cart)
            {
                dt.Rows.Add(c.UrunId, c.UrunAdi, c.Adet, c.BirimFiyat, c.Adet * c.BirimFiyat);
            }

            dgvCart.DataSource = dt;
            decimal toplam = cart.Sum(x => x.Adet * x.BirimFiyat);
            lblTotal.Text = $"Toplam: {toplam:0.00}";
        }

        private void BtnMyOrders_Click(object sender, EventArgs e)
        {
            var f = new FrmOrders(_userId);
            f.ShowDialog(this);
        }

        private void BtnAddToCart_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0) { MessageBox.Show("Ürün seçiniz"); return; }
            var qty = (int)nudQty.Value;
            foreach (DataGridViewRow row in dgvProducts.SelectedRows)
            {
                var id = Convert.ToInt32(row.Cells["Id"].Value);
                var name = row.Cells["UrunAdi"].Value.ToString();
                var price = Convert.ToDecimal(row.Cells["Fiyat"].Value);
                var stok = Convert.ToInt32(row.Cells["Stok"].Value);
                if (qty > stok) { MessageBox.Show($"Yeterli stok yok: {name}"); continue; }

                var existing = cart.FirstOrDefault(x => x.UrunId == id);
                if (existing != null) existing.Adet += qty;
                else cart.Add(new CartItem { UrunId = id, UrunAdi = name, BirimFiyat = price, Adet = qty });
            }
            RefreshCartGrid();
        }

        private void BtnRemoveFromCart_Click(object sender, EventArgs e)
        {
            if (dgvCart.CurrentRow == null) { MessageBox.Show("Sepet kalemi seçiniz"); return; }
            var id = Convert.ToInt32(dgvCart.CurrentRow.Cells["UrunId"].Value);
            var item = cart.FirstOrDefault(x => x.UrunId == id);
            if (item != null) cart.Remove(item);
            RefreshCartGrid();
        }

        private void BtnCreateOrder_Click(object sender, EventArgs e)
        {
            if (!cart.Any()) { MessageBox.Show("Sepet boþ"); return; }

            try
            {
                decimal total = 0;
                foreach (var it in cart)
                {
                    var dt = Db.GetTable("SELECT Stok FROM Urunler WHERE Id=@id", new SqlParameter("@id", it.UrunId));
                    if (dt.Rows.Count == 0) throw new Exception("Ürün bulunamadý: " + it.UrunAdi);
                    var stok = Convert.ToInt32(dt.Rows[0]["Stok"]);
                    if (stok < it.Adet) throw new Exception($"Yetersiz stok: {it.UrunAdi}");
                    total += it.Adet * it.BirimFiyat;
                }

                bool hasToplam = Db.ColumnExists("Siparisler", "Toplam");

                int siparisId;
                if (hasToplam)
                {
                    var insertOrder = "INSERT INTO Siparisler (KullaniciId,Tarih,Durum,Toplam) VALUES (@k,@t,@d,@top); SELECT SCOPE_IDENTITY();";
                    var obj = Db.ExecuteScalar(insertOrder,
                        new SqlParameter("@k", _userId),
                        new SqlParameter("@t", DateTime.Now),
                        new SqlParameter("@d", "Hazýrlanýyor"),
                        new SqlParameter("@top", total));
                    siparisId = Convert.ToInt32(Convert.ToDecimal(obj));
                }
                else
                {
                    var insertOrder = "INSERT INTO Siparisler (KullaniciId,Tarih,Durum) VALUES (@k,@t,@d); SELECT SCOPE_IDENTITY();";
                    var obj = Db.ExecuteScalar(insertOrder,
                        new SqlParameter("@k", _userId),
                        new SqlParameter("@t", DateTime.Now),
                        new SqlParameter("@d", "Hazýrlanýyor"));
                    siparisId = Convert.ToInt32(Convert.ToDecimal(obj));
                }

                foreach (var it in cart)
                {
                    Db.Execute("INSERT INTO SiparisDetaylar (SiparisId,UrunId,Adet,BirimFiyat) VALUES (@s,@u,@a,@b)",
                        new SqlParameter("@s", siparisId),
                        new SqlParameter("@u", it.UrunId),
                        new SqlParameter("@a", it.Adet),
                        new SqlParameter("@b", it.BirimFiyat));

                    Db.Execute("UPDATE Urunler SET Stok=Stok-@a WHERE Id=@u",
                        new SqlParameter("@a", it.Adet),
                        new SqlParameter("@u", it.UrunId));
                }

                cart.Clear();
                RefreshCartGrid();
                LoadProducts();
                MessageBox.Show("Sipariþ oluþturuldu. Sipariþ No: " + siparisId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sipariþ oluþturulurken hata: " + ex.Message);
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            Close();
        }

        private class CartItem
        {
            public int UrunId { get; set; }
            public string UrunAdi { get; set; }
            public int Adet { get; set; }
            public decimal BirimFiyat { get; set; }
        }

    }
}
