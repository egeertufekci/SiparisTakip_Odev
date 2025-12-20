using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SiparisTakip_Odev.Data;

namespace SiparisTakip_Odev.Forms
{
    public class FrmLogin : Form
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;
        private Button btnRegister;

        public FrmLogin()
        {
            Text = "Giriþ";
            Width = 360;
            Height = 220;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var lbl1 = new Label { Text = "Kullanýcý Adý:", Left = 10, Top = 20 };
            txtUser = new TextBox { Left = 110, Top = 18, Width = 150 };
            var lbl2 = new Label { Text = "Þifre:", Left = 10, Top = 50 };
            txtPass = new TextBox { Left = 110, Top = 48, Width = 150, UseSystemPasswordChar = true };
            btnLogin = new Button { Text = "Giriþ", Left = 110, Top = 80 };
            btnRegister = new Button { Text = "Kayýt Ol", Left = 190, Top = 80 };

            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;

            Controls.AddRange(new Control[] { lbl1, txtUser, lbl2, txtPass, btnLogin, btnRegister });
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            var reg = new FrmRegister();
            reg.ShowDialog(this);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var sql = "SELECT Id, Rol FROM Kullanicilar WHERE KullaniciAdi=@k AND Sifre=@s";
            var dt = Db.GetTable(sql,
                new SqlParameter("@k", txtUser.Text),
                new SqlParameter("@s", txtPass.Text));
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Kullanýcý adý veya þifre hatalý");
                return;
            }

            var id = Convert.ToInt32(dt.Rows[0]["Id"]);
            var rol = dt.Rows[0]["Rol"].ToString();

            Hide();
            if (rol == "Admin")
            {
                var a = new FrmAdmin(this);
                a.FormClosed += (s, ea) => this.Show();
                a.Show();
            }
            else
            {
                var m = new FrmMain(this, id);
                m.FormClosed += (s, ea) => this.Show();
                m.Show();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // If user clicked X on login form, confirm exit
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var res = MessageBox.Show("Uygulamadan çýkmak istediðinize emin misiniz?", "Çýkýþ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                Application.Exit();
            }
            base.OnFormClosing(e);
        }
    }
}
