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
            groupBox1.Location = new Point(38, 30);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(238, 75);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Фильтр по типу";
            // 
            // cmbFilterType
            // 
            cmbFilterType.FormattingEnabled = true;
            cmbFilterType.Location = new Point(105, 22);
            cmbFilterType.Name = "cmbFilterType";
            cmbFilterType.Size = new Size(121, 23);
            cmbFilterType.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(numMaxPrice);
            groupBox2.Controls.Add(numMinPrice);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(chkFilterByPrice);
            groupBox2.Location = new Point(38, 121);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(238, 144);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Фильтр по цене";
            // 
            // numMaxPrice
            // 
            numMaxPrice.Location = new Point(105, 75);
            numMaxPrice.Name = "numMaxPrice";
            numMaxPrice.Size = new Size(120, 23);
            numMaxPrice.TabIndex = 4;
            // 
            // numMinPrice
            // 
            numMinPrice.Location = new Point(105, 46);
            numMinPrice.Name = "numMinPrice";
            numMinPrice.Size = new Size(120, 23);
            numMinPrice.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(28, 77);
            label2.Name = "label2";
            label2.Size = new Size(71, 15);
            label2.TabIndex = 2;
            label2.Text = "Макс. цена:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(28, 49);
            label1.Name = "label1";
            label1.Size = new Size(67, 15);
            label1.TabIndex = 1;
            label1.Text = "Мин. цена:";
            // 
            // chkFilterByPrice
            // 
            chkFilterByPrice.AutoSize = true;
            chkFilterByPrice.Location = new Point(28, 21);
            chkFilterByPrice.Name = "chkFilterByPrice";
            chkFilterByPrice.Size = new Size(143, 19);
            chkFilterByPrice.TabIndex = 0;
            chkFilterByPrice.Text = "Фильтровать по цене";
            chkFilterByPrice.UseVisualStyleBackColor = true;
            chkFilterByPrice.CheckedChanged += chkFilterByPrice_CheckedChanged;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(182, 290);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(101, 290);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 6;
            btnOk.Text = "ОК";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOkFilter_Click;
            // 
            // FilterOptionsDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
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