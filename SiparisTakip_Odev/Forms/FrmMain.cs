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
        private Button btnAddToCart;
        private Button btnCreateOrder;
        private List<CartItem> cart = new List<CartItem>();

        public FrmMain(int userId)
        {
            _userId = userId;
            Text = "Ürünler";
            Width = 800;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;

            dgvProducts = new DataGridView { Left = 10, Top = 10, Width = 760, Height = 400, ReadOnly = true, AllowUserToAddRows = false };
            Load += FrmMain_Load;

            btnAddToCart = new Button { Text = "Sepete Ekle", Left = 10, Top = 420 };
            btnCreateOrder = new Button { Text = "Sipariþ Oluþtur", Left = 110, Top = 420 };

            btnAddToCart.Click += BtnAddToCart_Click;
            btnCreateOrder.Click += BtnCreateOrder_Click;

            Controls.AddRange(new Control[] { dgvProducts, btnAddToCart, btnCreateOrder });
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            var dt = Db.GetTable("SELECT Id,UrunAdi,Fiyat,Stok FROM Urunler WHERE Stok>0");
            dgvProducts.DataSource = dt;
        }

        private void BtnAddToCart_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;
            var id = Convert.ToInt32(dgvProducts.CurrentRow.Cells["Id"].Value);
            var name = dgvProducts.CurrentRow.Cells["UrunAdi"].Value.ToString();
            var price = Convert.ToDecimal(dgvProducts.CurrentRow.Cells["Fiyat"].Value);

            var existing = cart.FirstOrDefault(x => x.UrunId == id);
            if (existing != null) existing.Adet++;
            else cart.Add(new CartItem { UrunId = id, UrunAdi = name, BirimFiyat = price, Adet = 1 });

            MessageBox.Show("Sepete eklendi");
        }

        private void BtnCreateOrder_Click(object sender, EventArgs e)
        {
            if (!cart.Any())
            {
                MessageBox.Show("Sepet boþ");
                return;
            }

            var sql = "INSERT INTO Siparisler (KullaniciId,Tarih,Durum) VALUES (@k,@t,@d); SELECT SCOPE_IDENTITY();";
            var obj = Db.ExecuteScalar(sql,
                new SqlParameter("@k", _userId),
                new SqlParameter("@t", DateTime.Now),
                new SqlParameter("@d", "Beklemede"));
            var siparisId = Convert.ToInt32(Convert.ToDecimal(obj));

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
            MessageBox.Show("Sipariþ oluþturuldu");
            LoadProducts();
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
