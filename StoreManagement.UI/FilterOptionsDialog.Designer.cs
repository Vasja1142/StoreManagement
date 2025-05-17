namespace StoreManagement.UI
{
    partial class FilterOptionsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            cmbFilterType = new ComboBox();
            groupBox2 = new GroupBox();
            numMaxPrice = new NumericUpDown();
            numMinPrice = new NumericUpDown();
            label2 = new Label();
            label1 = new Label();
            chkFilterByPrice = new CheckBox();
            btnCancel = new Button();
            btnOk = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMaxPrice).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMinPrice).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cmbFilterType);
            groupBox1.Location = new Point(43, 40);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(272, 100);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Фильтр по типу";
            // 
            // cmbFilterType
            // 
            cmbFilterType.FormattingEnabled = true;
            cmbFilterType.Location = new Point(120, 29);
            cmbFilterType.Margin = new Padding(3, 4, 3, 4);
            cmbFilterType.Name = "cmbFilterType";
            cmbFilterType.Size = new Size(138, 28);
            cmbFilterType.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(numMaxPrice);
            groupBox2.Controls.Add(numMinPrice);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(chkFilterByPrice);
            groupBox2.Location = new Point(43, 161);
            groupBox2.Margin = new Padding(3, 4, 3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 4, 3, 4);
            groupBox2.Size = new Size(272, 192);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Фильтр по цене";
            // 
            // numMaxPrice
            // 
            numMaxPrice.Location = new Point(120, 100);
            numMaxPrice.Margin = new Padding(3, 4, 3, 4);
            numMaxPrice.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numMaxPrice.Name = "numMaxPrice";
            numMaxPrice.Size = new Size(137, 27);
            numMaxPrice.TabIndex = 4;
            // 
            // numMinPrice
            // 
            numMinPrice.Location = new Point(120, 61);
            numMinPrice.Margin = new Padding(3, 4, 3, 4);
            numMinPrice.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numMinPrice.Name = "numMinPrice";
            numMinPrice.Size = new Size(137, 27);
            numMinPrice.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(32, 103);
            label2.Name = "label2";
            label2.Size = new Size(88, 20);
            label2.TabIndex = 2;
            label2.Text = "Макс. цена:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(32, 65);
            label1.Name = "label1";
            label1.Size = new Size(84, 20);
            label1.TabIndex = 1;
            label1.Text = "Мин. цена:";
            // 
            // chkFilterByPrice
            // 
            chkFilterByPrice.AutoSize = true;
            chkFilterByPrice.Location = new Point(32, 28);
            chkFilterByPrice.Margin = new Padding(3, 4, 3, 4);
            chkFilterByPrice.Name = "chkFilterByPrice";
            chkFilterByPrice.Size = new Size(181, 24);
            chkFilterByPrice.TabIndex = 0;
            chkFilterByPrice.Text = "Фильтровать по цене";
            chkFilterByPrice.UseVisualStyleBackColor = true;
            chkFilterByPrice.CheckedChanged += chkFilterByPrice_CheckedChanged;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(208, 387);
            btnCancel.Margin = new Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(86, 31);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(115, 387);
            btnOk.Margin = new Padding(3, 4, 3, 4);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(86, 31);
            btnOk.TabIndex = 6;
            btnOk.Text = "ОК";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOkFilter_Click;
            // 
            // FilterOptionsDialog
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(374, 439);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FilterOptionsDialog";
            Text = "FilterOptionsDialog";
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numMaxPrice).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMinPrice).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private ComboBox cmbFilterType;
        private GroupBox groupBox2;
        private Label label2;
        private Label label1;
        private CheckBox chkFilterByPrice;
        private NumericUpDown numMaxPrice;
        private NumericUpDown numMinPrice;
        private Button btnCancel;
        private Button btnOk;
    }
}