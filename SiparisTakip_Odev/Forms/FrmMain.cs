using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using SiparisTakip_Odev.Data;

namespace SiparisTakip_Odev.Forms
{
    public class FrmMain : Form
    {
        private int _userId;
        private DataGridView dgvProducts;
        private DataGridView dgvCart;
        private NumericUpDown nudQty;
        private Button btnAddToCart;
        private Button btnRemoveFromCart;
        private Button btnCreateOrder;
        private Button btnLogout;
        // Use BindingList so UI can bind and later be extended easily
        private System.ComponentModel.BindingList<CartItem> cart = new System.ComponentModel.BindingList<CartItem>();
        private Form _owner;

        public FrmMain(Form owner, int userId)
        {
            _owner = owner;
            _userId = userId;
            Text = "Ürünler";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            dgvProducts = new DataGridView { Left = 10, Top = 10, Width = 600, Height = 500, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvCart = new DataGridView { Left = 620, Top = 10, Width = 350, Height = 400, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            nudQty = new NumericUpDown { Left = 10, Top = 520, Width = 60, Minimum = 1, Maximum = 100, Value = 1 };
            btnAddToCart = new Button { Text = "Sepete Ekle", Left = 80, Top = 516, Width = 120 };
            btnRemoveFromCart = new Button { Text = "Sepetten Kaldýr", Left = 620, Top = 420, Width = 140 };
            btnCreateOrder = new Button { Text = "Sipariþ Ver", Left = 780, Top = 420, Width = 120 };
            btnLogout = new Button { Text = "Çýkýþ", Left = 780, Top = 480, Width = 120 };

            btnAddToCart.Click += BtnAddToCart_Click;
            btnRemoveFromCart.Click += BtnRemoveFromCart_Click;
            btnCreateOrder.Click += BtnCreateOrder_Click;
            btnLogout.Click += BtnLogout_Click;

            Controls.AddRange(new Control[] { dgvProducts, dgvCart, nudQty, btnAddToCart, btnRemoveFromCart, btnCreateOrder, btnLogout });

            Load += FrmMain_Load;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadProducts();
            RefreshCartGrid();
        }

        private void LoadProducts()
        {
            var dt = Db.GetTable("SELECT Id,UrunAdi,Fiyat,Stok FROM Urunler WHERE Stok>0");
            dgvProducts.DataSource = dt;
        }

        // Build cart grid from BindingList; compute totals in code to avoid relying on DB column names
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
        }

        private void BtnAddToCart_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) { MessageBox.Show("Ürün seçiniz"); return; }
            var id = Convert.ToInt32(dgvProducts.CurrentRow.Cells["Id"].Value);
            var name = dgvProducts.CurrentRow.Cells["UrunAdi"].Value.ToString();
            var price = Convert.ToDecimal(dgvProducts.CurrentRow.Cells["Fiyat"].Value);
            var qty = (int)nudQty.Value;

            var stok = Convert.ToInt32(dgvProducts.CurrentRow.Cells["Stok"].Value);
            if (qty > stok) { MessageBox.Show("Yeterli stok yok"); return; }

            var existing = cart.FirstOrDefault(x => x.UrunId == id);
            if (existing != null) existing.Adet += qty;
            else cart.Add(new CartItem { UrunId = id, UrunAdi = name, BirimFiyat = price, Adet = qty });

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

            // Transactional order creation
            try
            {
                // Calculate total and validate stock in C# (do not rely on DB column names)
                decimal total = 0;
                foreach (var it in cart)
                {
                    var dt = Db.GetTable("SELECT Stok FROM Urunler WHERE Id=@id", new SqlParameter("@id", it.UrunId));
                    if (dt.Rows.Count == 0) throw new Exception("Ürün bulunamadý: " + it.UrunAdi);
                    var stok = Convert.ToInt32(dt.Rows[0]["Stok"]);
                    if (stok < it.Adet) throw new Exception($"Yetersiz stok: {it.UrunAdi}");
                    total += it.Adet * it.BirimFiyat;
                }

                // Insert order: explicitly list columns to avoid SELECT * mismatch
                // Check if 'Toplam' column exists in Siparisler
                bool hasToplam = Db.ColumnExists("Siparisler", "Toplam");

                int siparisId;
                if (hasToplam)
                {
                    // Insert including Toplam column
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
                    // Insert without Toplam column (backward compatible)
                    var insertOrder = "INSERT INTO Siparisler (KullaniciId,Tarih,Durum) VALUES (@k,@t,@d); SELECT SCOPE_IDENTITY();";
                    var obj = Db.ExecuteScalar(insertOrder,
                        new SqlParameter("@k", _userId),
                        new SqlParameter("@t", DateTime.Now),
                        new SqlParameter("@d", "Hazýrlanýyor"));
                    siparisId = Convert.ToInt32(Convert.ToDecimal(obj));
                }

                // Insert details and update stock
                foreach (var it in cart)
                {
                    // Explicit column list for detail insert
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
