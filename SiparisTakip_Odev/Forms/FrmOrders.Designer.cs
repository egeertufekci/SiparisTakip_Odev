namespace SiparisTakip_Odev.Forms
{
    partial class FrmOrders
    {
        // Designer does not require a Dispose override in this minimal form

        private void InitializeComponent()
        {
            this.Text = "Sipariþlerim";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.ClientSize = new System.Drawing.Size(820, 600);

            var lblOrders = new System.Windows.Forms.Label(); lblOrders.Text = "Sipariþler"; lblOrders.Left = 10; lblOrders.Top = 10; lblOrders.Width = 200; lblOrders.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            this.dgvOrders = new System.Windows.Forms.DataGridView(); this.dgvOrders.Left = 10; this.dgvOrders.Top = 40; this.dgvOrders.Width = 780; this.dgvOrders.Height = 220; this.dgvOrders.ReadOnly = true; this.dgvOrders.AllowUserToAddRows = false; this.dgvOrders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill; this.dgvOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect; this.dgvOrders.MultiSelect = false;

            var lblDetails = new System.Windows.Forms.Label(); lblDetails.Text = "Sipariþ Detaylarý"; lblDetails.Left = 10; lblDetails.Top = 270; lblDetails.Width = 200; lblDetails.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            this.dgvDetails = new System.Windows.Forms.DataGridView(); this.dgvDetails.Left = 10; this.dgvDetails.Top = 300; this.dgvDetails.Width = 780; this.dgvDetails.Height = 260; this.dgvDetails.ReadOnly = true; this.dgvDetails.AllowUserToAddRows = false; this.dgvDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            this.Controls.AddRange(new System.Windows.Forms.Control[] { lblOrders, this.dgvOrders, lblDetails, this.dgvDetails });
        }

        private System.Windows.Forms.DataGridView dgvOrders;
        private System.Windows.Forms.DataGridView dgvDetails;
    }
}
