using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using SiparisTakip_Odev.Data;

namespace SiparisTakip_Odev.Forms
{
    public class FrmRegister : Form
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnSave;

        public FrmRegister()
        {
            Text = "Kayýt";
            Width = 300;
            Height = 180;
            StartPosition = FormStartPosition.CenterParent;

            var lbl1 = new Label { Text = "Kullanýcý Adý:", Left = 10, Top = 20 };
            txtUser = new TextBox { Left = 110, Top = 18, Width = 150 };
            var lbl2 = new Label { Text = "Þifre:", Left = 10, Top = 50 };
            txtPass = new TextBox { Left = 110, Top = 48, Width = 150 };
            btnSave = new Button { Text = "Kaydet", Left = 110, Top = 80 };
            btnSave.Click += BtnSave_Click;
            Controls.AddRange(new Control[] { lbl1, txtUser, lbl2, txtPass, btnSave });
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
