// JournalViewForm.cs
using System;
using System.Windows.Forms;

namespace StoreManagement.UI
{
    public partial class JournalViewForm : Form
    {
        public JournalViewForm(string journalContent)
        {
            InitializeComponent();
            txtJournalContent.Text = journalContent;
            this.Text = "Просмотр журнала операций";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}