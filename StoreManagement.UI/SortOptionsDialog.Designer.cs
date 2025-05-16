namespace StoreManagement.UI
{
    partial class SortOptionsDialog
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
            rbSortByName = new RadioButton();
            rbSortByPrice = new RadioButton();
            btnCancel = new Button();
            btnOk = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(33, 40);
            label1.Name = "label1";
            label1.Size = new Size(184, 15);
            label1.TabIndex = 0;
            label1.Text = "Выберите поле для сортировки:";
            // 
            // rbSortByName
            // 
            rbSortByName.AutoSize = true;
            rbSortByName.Location = new Point(33, 74);
            rbSortByName.Name = "rbSortByName";
            rbSortByName.Size = new Size(82, 19);
            rbSortByName.TabIndex = 1;
            rbSortByName.TabStop = true;
            rbSortByName.Text = "По Имени";
            rbSortByName.UseVisualStyleBackColor = true;
            // 
            // rbSortByPrice
            // 
            rbSortByPrice.AutoSize = true;
            rbSortByPrice.Location = new Point(33, 111);
            rbSortByPrice.Name = "rbSortByPrice";
            rbSortByPrice.Size = new Size(72, 19);
            rbSortByPrice.TabIndex = 2;
            rbSortByPrice.TabStop = true;
            rbSortByPrice.Text = "По Цене";
            rbSortByPrice.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.OK;
            btnCancel.Location = new Point(121, 156);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnOkSort_Click;
            // 
            // btnOk
            // 
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(40, 156);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 4;
            btnOk.Text = "ОК";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOkSort_Click;
            // 
            // SortOptionsDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(250, 230);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(rbSortByPrice);
            Controls.Add(rbSortByName);
            Controls.Add(label1);
            Name = "SortOptionsDialog";
            Text = "SortOptionsDialog";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private RadioButton rbSortByName;
        private RadioButton rbSortByPrice;
        private Button btnCancel;
        private Button btnOk;
    }
}