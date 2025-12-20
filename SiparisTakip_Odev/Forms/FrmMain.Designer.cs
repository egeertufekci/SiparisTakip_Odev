namespace SiparisTakip_Odev.Forms
{
    partial class FrmMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvProducts = new System.Windows.Forms.DataGridView();
            this.dgvCart = new System.Windows.Forms.DataGridView();
            this.nudQty = new System.Windows.Forms.NumericUpDown();
            this.btnAddToCart = new System.Windows.Forms.Button();
            this.btnRemoveFromCart = new System.Windows.Forms.Button();
            this.btnCreateOrder = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnMyOrders = new System.Windows.Forms.Button();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQty)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvProducts
            // 
            this.dgvProducts.AllowUserToAddRows = false;
            this.dgvProducts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProducts.ColumnHeadersHeight = 34;
            this.dgvProducts.Location = new System.Drawing.Point(12, 39);
            this.dgvProducts.Name = "dgvProducts";
            this.dgvProducts.ReadOnly = true;
            this.dgvProducts.RowHeadersWidth = 62;
            this.dgvProducts.Size = new System.Drawing.Size(600, 500);
            this.dgvProducts.TabIndex = 0;
            // 
            // dgvCart
            // 
            this.dgvCart.AllowUserToAddRows = false;
            this.dgvCart.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCart.ColumnHeadersHeight = 34;
            this.dgvCart.Location = new System.Drawing.Point(620, 39);
            this.dgvCart.Name = "dgvCart";
            this.dgvCart.ReadOnly = true;
            this.dgvCart.RowHeadersWidth = 62;
            this.dgvCart.Size = new System.Drawing.Size(350, 400);
            this.dgvCart.TabIndex = 1;
            // 
            // nudQty
            // 
            this.nudQty.Location = new System.Drawing.Point(618, 480);
            this.nudQty.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudQty.Name = "nudQty";
            this.nudQty.Size = new System.Drawing.Size(60, 26);
            this.nudQty.TabIndex = 2;
            this.nudQty.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnAddToCart
            // 
            this.btnAddToCart.Location = new System.Drawing.Point(684, 480);
            this.btnAddToCart.Name = "btnAddToCart";
            this.btnAddToCart.Size = new System.Drawing.Size(120, 23);
            this.btnAddToCart.TabIndex = 3;
            this.btnAddToCart.Text = "Sepete Ekle";
            // 
            // btnRemoveFromCart
            // 
            this.btnRemoveFromCart.Location = new System.Drawing.Point(826, 480);
            this.btnRemoveFromCart.Name = "btnRemoveFromCart";
            this.btnRemoveFromCart.Size = new System.Drawing.Size(140, 23);
            this.btnRemoveFromCart.TabIndex = 4;
            this.btnRemoveFromCart.Text = "Sepetten Kaldýr";
            // 
            // btnCreateOrder
            // 
            this.btnCreateOrder.Location = new System.Drawing.Point(846, 440);
            this.btnCreateOrder.Name = "btnCreateOrder";
            this.btnCreateOrder.Size = new System.Drawing.Size(120, 23);
            this.btnCreateOrder.TabIndex = 5;
            this.btnCreateOrder.Text = "Sipariþ Ver";
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(846, 5);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(120, 23);
            this.btnLogout.TabIndex = 6;
            this.btnLogout.Text = "Çýkýþ";
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTotal.ForeColor = System.Drawing.Color.Green;
            this.lblTotal.Location = new System.Drawing.Point(616, 442);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(350, 24);
            this.lblTotal.TabIndex = 7;
            this.lblTotal.Text = "Toplam: 0.00";
            // 
            // btnMyOrders
            // 
            this.btnMyOrders.Location = new System.Drawing.Point(745, 516);
            this.btnMyOrders.Name = "btnMyOrders";
            this.btnMyOrders.Size = new System.Drawing.Size(120, 23);
            this.btnMyOrders.TabIndex = 8;
            this.btnMyOrders.Text = "Sipariþlerim";
            // 
            // cmbTheme
            // 
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.Items.AddRange(new object[] {
            "Light",
            "Dark",
            "Blue",
            "Green"});
            this.cmbTheme.Location = new System.Drawing.Point(12, 5);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(150, 28);
            this.cmbTheme.TabIndex = 9;
            // 
            // FrmMain
            // 
            this.ClientSize = new System.Drawing.Size(978, 560);
            this.Controls.Add(this.dgvProducts);
            this.Controls.Add(this.dgvCart);
            this.Controls.Add(this.nudQty);
            this.Controls.Add(this.btnAddToCart);
            this.Controls.Add(this.btnRemoveFromCart);
            this.Controls.Add(this.btnCreateOrder);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.btnMyOrders);
            this.Controls.Add(this.cmbTheme);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ürünler";
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQty)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridView dgvProducts;
        private System.Windows.Forms.DataGridView dgvCart;
        private System.Windows.Forms.NumericUpDown nudQty;
        private System.Windows.Forms.Button btnAddToCart;
        private System.Windows.Forms.Button btnRemoveFromCart;
        private System.Windows.Forms.Button btnCreateOrder;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnMyOrders;
        private System.Windows.Forms.ComboBox cmbTheme;
    }
}
