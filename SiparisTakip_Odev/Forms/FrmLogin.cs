using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SiparisTakip_Odev.Data;

namespace SiparisTakip_Odev.Forms
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;
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
