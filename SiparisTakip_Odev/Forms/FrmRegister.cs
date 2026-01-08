using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using SiparisTakip_Odev.Data;

namespace SiparisTakip_Odev.Forms
{
    public partial class FrmRegister : Form
    {
        public FrmRegister()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Alanlar boþ olamaz");
                return;
            }

            var sql = "INSERT INTO Kullanicilar (KullaniciAdi,Sifre,Rol) VALUES (@k,@s,@r)";
            var rows = Db.Execute(sql,
                new SqlParameter("@k", txtUser.Text),
                new SqlParameter("@s", txtPass.Text),
                new SqlParameter("@r", "User"));
            if (rows > 0)
            {
                MessageBox.Show("Kayýt baþarýlý");
                Close();
            }
            else
            {
                MessageBox.Show("Kayýt baþarýsýz");
            }
        }
    }
}
