namespace StoreManagement.UI
{
    partial class JournalViewForm
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
            txtJournalContent = new TextBox();
            btnClose = new Button();
            SuspendLayout();
            // 
            // txtJournalContent
            // 
            txtJournalContent.Dock = DockStyle.Fill;
            txtJournalContent.Location = new Point(0, 0);
            txtJournalContent.Multiline = true;
            txtJournalContent.Name = "txtJournalContent";
            txtJournalContent.ReadOnly = true;
            txtJournalContent.ScrollBars = ScrollBars.Both;
            txtJournalContent.Size = new Size(763, 427);
            txtJournalContent.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.Location = new Point(369, 392);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 23);
            btnClose.TabIndex = 1;
            btnClose.Text = "Закрыть";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // JournalViewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(763, 427);
            Controls.Add(btnClose);
            Controls.Add(txtJournalContent);
            Name = "JournalViewForm";
            Text = "JournalViewForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtJournalContent;
        private Button btnClose;
    }
}