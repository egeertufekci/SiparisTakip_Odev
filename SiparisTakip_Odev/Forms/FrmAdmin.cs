using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SiparisTakip_Odev.Data;

namespace SiparisTakip_Odev.Forms
{
    public class FrmAdmin : Form
    {
        private DataGridView dgv;
        private Button btnAdd;
        private Button btnDelete;
        private TextBox txtName;
        private TextBox txtPrice;
        private TextBox txtStock;

        public FrmAdmin()
        {
            Text = "Admin - Ürün Yönetimi";
            Width = 800;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;

            dgv = new DataGridView { Left = 10, Top = 10, Width = 760, Height = 400, ReadOnly = true, AllowUserToAddRows = false };
            Load += FrmAdmin_Load;

            var lbl1 = new Label { Text = "Ürün:", Left = 10, Top = 420 };
            txtName = new TextBox { Left = 60, Top = 418, Width = 200 };
            var lbl2 = new Label { Text = "Fiyat:", Left = 280, Top = 420 };
            txtPrice = new TextBox { Left = 330, Top = 418, Width = 80 };
            var lbl3 = new Label { Text = "Stok:", Left = 420, Top = 420 };
            txtStock = new TextBox { Left = 460, Top = 418, Width = 80 };
            btnAdd = new Button { Text = "Ekle", Left = 560, Top = 416 };
            btnDelete = new Button { Text = "Sil", Left = 640, Top = 416 };

            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;

            Controls.AddRange(new Control[] { dgv, lbl1, txtName, lbl2, txtPrice, lbl3, txtStock, btnAdd, btnDelete });
        }

        private void FrmAdmin_Load(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            var dt = Db.GetTable("SELECT Id,UrunAdi,Fiyat,Stok FROM Urunler");
            dgv.DataSource = dt;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            decimal fiyat;
            int stok;
            if (string.IsNullOrWhiteSpace(txtName.Text) || !decimal.TryParse(txtPrice.Text, out fiyat) || !int.TryParse(txtStock.Text, out stok))
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
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var id = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            var sql = "DELETE FROM Urunler WHERE Id=@id";
            Db.Execute(sql, new SqlParameter("@id", id));
            LoadProducts();
        }
    }
}
