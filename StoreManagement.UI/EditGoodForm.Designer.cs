namespace StoreManagement.UI
{
    partial class EditGoodForm
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
            label1 = new Label();
            txtName = new TextBox();
            label2 = new Label();
            numPrice = new NumericUpDown();
            label3 = new Label();
            txtManufacturer = new TextBox();
            label4 = new Label();
            cmbGoodsType = new ComboBox();
            panelProductSpecifics = new Panel();
            dtpExpirationDate = new DateTimePicker();
            label5 = new Label();
            panelDairySpecifics = new Panel();
            numVolume = new NumericUpDown();
            label7 = new Label();
            numFatContent = new NumericUpDown();
            label6 = new Label();
            panelToySpecifics = new Panel();
            txtMaterial = new TextBox();
            label9 = new Label();
            numAgeRestriction = new NumericUpDown();
            label8 = new Label();
            btnOK = new Button();
            btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)numPrice).BeginInit();
            panelProductSpecifics.SuspendLayout();
            panelDairySpecifics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numVolume).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFatContent).BeginInit();
            panelToySpecifics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numAgeRestriction).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(43, 55);
            label1.Name = "label1";
            label1.Size = new Size(42, 20);
            label1.TabIndex = 0;
            label1.Text = "Имя:";
            // 
            // txtName
            // 
            txtName.Location = new Point(178, 52);
            txtName.Margin = new Padding(3, 4, 3, 4);
            txtName.Name = "txtName";
            txtName.Size = new Size(138, 27);
            txtName.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(43, 113);
            label2.Name = "label2";
            label2.Size = new Size(48, 20);
            label2.TabIndex = 2;
            label2.Text = "Цена:";
            // 
            // numPrice
            // 
            numPrice.DecimalPlaces = 2;
            numPrice.Location = new Point(179, 106);
            numPrice.Margin = new Padding(3, 4, 3, 4);
            numPrice.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numPrice.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            numPrice.Name = "numPrice";
            numPrice.Size = new Size(137, 27);
            numPrice.TabIndex = 3;
            numPrice.Value = new decimal(new int[] { 1, 0, 0, 131072 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(41, 168);
            label3.Name = "label3";
            label3.Size = new Size(121, 20);
            label3.TabIndex = 4;
            label3.Text = "Производитель:";
            // 
            // txtManufacturer
            // 
            txtManufacturer.Location = new Point(178, 168);
            txtManufacturer.Margin = new Padding(3, 4, 3, 4);
            txtManufacturer.Name = "txtManufacturer";
            txtManufacturer.Size = new Size(138, 27);
            txtManufacturer.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(41, 223);
            label4.Name = "label4";
            label4.Size = new Size(90, 20);
            label4.TabIndex = 6;
            label4.Text = "Тип товара:";
            // 
            // cmbGoodsType
            // 
            cmbGoodsType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGoodsType.FormattingEnabled = true;
            cmbGoodsType.Location = new Point(178, 220);
            cmbGoodsType.Margin = new Padding(3, 4, 3, 4);
            cmbGoodsType.Name = "cmbGoodsType";
            cmbGoodsType.Size = new Size(138, 28);
            cmbGoodsType.TabIndex = 7;
            cmbGoodsType.SelectedIndexChanged += cmbGoodsType_SelectedIndexChanged;
            // 
            // panelProductSpecifics
            // 
            panelProductSpecifics.Controls.Add(dtpExpirationDate);
            panelProductSpecifics.Controls.Add(label5);
            panelProductSpecifics.Location = new Point(340, 38);
            panelProductSpecifics.Margin = new Padding(3, 4, 3, 4);
            panelProductSpecifics.Name = "panelProductSpecifics";
            panelProductSpecifics.Size = new Size(312, 60);
            panelProductSpecifics.TabIndex = 8;
            // 
            // dtpExpirationDate
            // 
            dtpExpirationDate.Format = DateTimePickerFormat.Short;
            dtpExpirationDate.Location = new Point(156, 14);
            dtpExpirationDate.Margin = new Padding(3, 4, 3, 4);
            dtpExpirationDate.Name = "dtpExpirationDate";
            dtpExpirationDate.Size = new Size(137, 27);
            dtpExpirationDate.TabIndex = 1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(13, 18);
            label5.Name = "label5";
            label5.Size = new Size(113, 20);
            label5.TabIndex = 0;
            label5.Text = "Срок годности:";
            // 
            // panelDairySpecifics
            // 
            panelDairySpecifics.Controls.Add(numVolume);
            panelDairySpecifics.Controls.Add(label7);
            panelDairySpecifics.Controls.Add(numFatContent);
            panelDairySpecifics.Controls.Add(label6);
            panelDairySpecifics.Location = new Point(340, 113);
            panelDairySpecifics.Margin = new Padding(3, 4, 3, 4);
            panelDairySpecifics.Name = "panelDairySpecifics";
            panelDairySpecifics.Size = new Size(312, 100);
            panelDairySpecifics.TabIndex = 9;
            // 
            // numVolume
            // 
            numVolume.DecimalPlaces = 3;
            numVolume.Location = new Point(156, 56);
            numVolume.Margin = new Padding(3, 4, 3, 4);
            numVolume.Minimum = new decimal(new int[] { 1, 0, 0, 196608 });
            numVolume.Name = "numVolume";
            numVolume.Size = new Size(137, 27);
            numVolume.TabIndex = 3;
            numVolume.Value = new decimal(new int[] { 1, 0, 0, 196608 });
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(13, 62);
            label7.Name = "label7";
            label7.Size = new Size(82, 20);
            label7.TabIndex = 2;
            label7.Text = "Объем (л):";
            // 
            // numFatContent
            // 
            numFatContent.DecimalPlaces = 1;
            numFatContent.Location = new Point(156, 12);
            numFatContent.Margin = new Padding(3, 4, 3, 4);
            numFatContent.Name = "numFatContent";
            numFatContent.Size = new Size(137, 27);
            numFatContent.TabIndex = 1;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(13, 19);
            label6.Name = "label6";
            label6.Size = new Size(108, 20);
            label6.TabIndex = 0;
            label6.Text = "Жирность (%):";
            // 
            // panelToySpecifics
            // 
            panelToySpecifics.Controls.Add(txtMaterial);
            panelToySpecifics.Controls.Add(label9);
            panelToySpecifics.Controls.Add(numAgeRestriction);
            panelToySpecifics.Controls.Add(label8);
            panelToySpecifics.Location = new Point(340, 223);
            panelToySpecifics.Margin = new Padding(3, 4, 3, 4);
            panelToySpecifics.Name = "panelToySpecifics";
            panelToySpecifics.Size = new Size(312, 113);
            panelToySpecifics.TabIndex = 10;
            // 
            // txtMaterial
            // 
            txtMaterial.Location = new Point(156, 61);
            txtMaterial.Margin = new Padding(3, 4, 3, 4);
            txtMaterial.Name = "txtMaterial";
            txtMaterial.Size = new Size(137, 27);
            txtMaterial.TabIndex = 3;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(14, 64);
            label9.Name = "label9";
            label9.Size = new Size(81, 20);
            label9.TabIndex = 2;
            label9.Text = "Материал:";
            // 
            // numAgeRestriction
            // 
            numAgeRestriction.Location = new Point(156, 17);
            numAgeRestriction.Margin = new Padding(3, 4, 3, 4);
            numAgeRestriction.Maximum = new decimal(new int[] { 18, 0, 0, 0 });
            numAgeRestriction.Name = "numAgeRestriction";
            numAgeRestriction.Size = new Size(137, 27);
            numAgeRestriction.TabIndex = 1;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(13, 19);
            label8.Name = "label8";
            label8.Size = new Size(137, 20);
            label8.TabIndex = 0;
            label8.Text = "Возраст. огр. (лет):";
            // 
            // btnOK
            // 
            btnOK.Location = new Point(202, 384);
            btnOK.Margin = new Padding(3, 4, 3, 4);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(86, 31);
            btnOK.TabIndex = 11;
            btnOK.Text = "ОК";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(325, 384);
            btnCancel.Margin = new Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(86, 31);
            btnCancel.TabIndex = 12;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // EditGoodForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(693, 436);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(panelToySpecifics);
            Controls.Add(panelDairySpecifics);
            Controls.Add(panelProductSpecifics);
            Controls.Add(cmbGoodsType);
            Controls.Add(label4);
            Controls.Add(txtManufacturer);
            Controls.Add(label3);
            Controls.Add(numPrice);
            Controls.Add(label2);
            Controls.Add(txtName);
            Controls.Add(label1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "EditGoodForm";
            Text = "EditGoodForm";
            ((System.ComponentModel.ISupportInitialize)numPrice).EndInit();
            panelProductSpecifics.ResumeLayout(false);
            panelProductSpecifics.PerformLayout();
            panelDairySpecifics.ResumeLayout(false);
            panelDairySpecifics.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numVolume).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFatContent).EndInit();
            panelToySpecifics.ResumeLayout(false);
            panelToySpecifics.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numAgeRestriction).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtName;
        private Label label2;
        private NumericUpDown numPrice;
        private Label label3;
        private TextBox txtManufacturer;
        private Label label4;
        private ComboBox cmbGoodsType;
        private Panel panelProductSpecifics;
        private DateTimePicker dtpExpirationDate;
        private Label label5;
        private Panel panelDairySpecifics;
        private NumericUpDown numVolume;
        private Label label7;
        private NumericUpDown numFatContent;
        private Label label6;
        private Panel panelToySpecifics;
        private TextBox txtMaterial;
        private Label label9;
        private NumericUpDown numAgeRestriction;
        private Label label8;
        private Button btnOK;
        private Button btnCancel;
    }
}