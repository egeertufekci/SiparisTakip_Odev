using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SiparisTakip_Odev.Data;

namespace SiparisTakip_Odev.Forms
{
    public class FrmAdmin : Form
    {
        private TabControl tab;
        private DataGridView dgv;
        private Button btnAdd;
        private Button btnDelete;
        private TextBox txtName;
        private TextBox txtPrice;
        private TextBox txtStock;

        // Orders controls
        private DataGridView dgvOrders;
        private ComboBox cmbStatus;
        private Button btnUpdateStatus;

        private Form _owner;

        public FrmAdmin(Form owner)
        {
            _owner = owner;
            Text = "Admin - Yönetim";
            Width = 900;
            Height = 650;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            tab = new TabControl { Dock = DockStyle.Fill };

            var tabProducts = new TabPage("Ürünler");
            var tabOrders = new TabPage("Sipariþler");

            // Products layout
            dgv = new DataGridView { Dock = DockStyle.Top, Height = 380, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var panel = new Panel { Dock = DockStyle.Bottom, Height = 80 };

            var lbl1 = new Label { Text = "Ürün:", Left = 10, Top = 15, Width = 40 };
            txtName = new TextBox { Left = 60, Top = 10, Width = 300, Height = 24 }; txtName.Multiline = false;
            var lbl2 = new Label { Text = "Fiyat:", Left = 380, Top = 15, Width = 40 };
            txtPrice = new TextBox { Left = 430, Top = 10, Width = 100, Height = 24 };
            var lbl3 = new Label { Text = "Stok:", Left = 540, Top = 15, Width = 40 };
            txtStock = new TextBox { Left = 590, Top = 10, Width = 80, Height = 24 };
            btnAdd = new Button { Text = "Ekle", Left = 690, Top = 8, Width = 80 };
            btnDelete = new Button { Text = "Sil", Left = 780, Top = 8, Width = 80 };

            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;

            panel.Controls.AddRange(new Control[] { lbl1, txtName, lbl2, txtPrice, lbl3, txtStock, btnAdd, btnDelete });
            tabProducts.Controls.AddRange(new Control[] { dgv, panel });

            // Orders layout
            dgvOrders = new DataGridView { Dock = DockStyle.Top, Height = 420, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var panel2 = new Panel { Dock = DockStyle.Bottom, Height = 80 };
            cmbStatus = new ComboBox { Left = 10, Top = 20, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new string[] { "Hazýrlanýyor", "Kargoya Verildi", "Teslim Edildi" });
            btnUpdateStatus = new Button { Text = "Durumu Güncelle", Left = 220, Top = 18, Width = 140 };
            btnUpdateStatus.Click += BtnUpdateStatus_Click;
            panel2.Controls.AddRange(new Control[] { cmbStatus, btnUpdateStatus });
            tabOrders.Controls.AddRange(new Control[] { dgvOrders, panel2 });

            tab.TabPages.Add(tabProducts);
            tab.TabPages.Add(tabOrders);

            Controls.Add(tab);

            Load += FrmAdmin_Load;
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

                // Delete in transaction: first delete any order details referencing this product
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
            if (dgvOrders.CurrentRow == null) { MessageBox.Show("Sipariþ seçiniz"); return; }
            if (cmbStatus.SelectedItem == null) { MessageBox.Show("Durum seçiniz"); return; }
            var id = Convert.ToInt32(dgvOrders.CurrentRow.Cells["Id"].Value);
            var sql = "UPDATE Siparisler SET Durum=@d WHERE Id=@id";
            Db.Execute(sql, new SqlParameter("@d", cmbStatus.SelectedItem.ToString()), new SqlParameter("@id", id));
            LoadOrders();
        }
    }
}
