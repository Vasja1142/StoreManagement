// SortOptionsDialog.cs
using System;
using System.Windows.Forms;

namespace StoreManagement.UI
{
    // Класс формы ПЕРВЫЙ
    public partial class SortOptionsDialog : Form
    {
        public SortCriteria SelectedCriteria { get; private set; }

        public SortOptionsDialog()
        {
            InitializeComponent();
            rbSortByName.Checked = true;
            this.Text = "Параметры сортировки";
        }

        // Имя обработчика должно совпадать с дизайнерским
        private void btnOkSort_Click(object sender, EventArgs e) // Пример имени
        {
            if (rbSortByName.Checked)
                SelectedCriteria = SortCriteria.ByName;
            else if (rbSortByPrice.Checked)
                SelectedCriteria = SortCriteria.ByPrice;

            this.DialogResult = DialogResult.OK;
        }
    }

    // Enum ПОСЛЕ класса формы
    public enum SortCriteria { ByName, ByPrice }
}