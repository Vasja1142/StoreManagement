// FilterOptionsDialog.cs
using System;
using System.Windows.Forms;

namespace StoreManagement.UI
{
    // Класс формы ДОЛЖЕН БЫТЬ ПЕРВЫМ в файле для корректной работы дизайнера
    public partial class FilterOptionsDialog : Form
    {
        public FilterOptions CurrentFilterOptions { get; private set; }

        public FilterOptionsDialog() // Конструктор по умолчанию
        {
            InitializeComponent(); // Обязательно!
            CurrentFilterOptions = new FilterOptions();
            InitializeControls();
            this.Text = "Параметры фильтрации";
        }

        // Конструктор для передачи существующих опций (если вы его используете)
        public FilterOptionsDialog(FilterOptions existingOptions) : this()
        {
            CurrentFilterOptions = existingOptions ?? new FilterOptions();
            LoadOptionsToControls(); // Загружаем переданные опции в контролы
        }

        private void InitializeControls()
        {
            // Убедитесь, что элементы управления (cmbFilterType, chkFilterByPrice и т.д.)
            // уже существуют к моменту вызова этого метода (т.е. InitializeComponent() был вызван)
            cmbFilterType.DataSource = Enum.GetValues(typeof(FilterGoodsType));
            cmbFilterType.SelectedItem = CurrentFilterOptions.GoodsType;

            chkFilterByPrice.Checked = CurrentFilterOptions.FilterByPrice;
            numMinPrice.Value = Math.Max(numMinPrice.Minimum, Math.Min(numMinPrice.Maximum, CurrentFilterOptions.MinPrice));
            numMaxPrice.Value = Math.Max(numMaxPrice.Minimum, Math.Min(numMaxPrice.Maximum, CurrentFilterOptions.MaxPrice));

            UpdatePriceControlsState();
        }

        private void LoadOptionsToControls()
        {
            cmbFilterType.SelectedItem = CurrentFilterOptions.GoodsType;
            chkFilterByPrice.Checked = CurrentFilterOptions.FilterByPrice;
            numMinPrice.Value = Math.Max(numMinPrice.Minimum, Math.Min(numMinPrice.Maximum, CurrentFilterOptions.MinPrice));
            numMaxPrice.Value = Math.Max(numMaxPrice.Minimum, Math.Min(numMaxPrice.Maximum, CurrentFilterOptions.MaxPrice));
            UpdatePriceControlsState();
        }

        private void chkFilterByPrice_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePriceControlsState();
        }

        private void UpdatePriceControlsState()
        {
            numMinPrice.Enabled = chkFilterByPrice.Checked;
            numMaxPrice.Enabled = chkFilterByPrice.Checked;
        }

        // Имя обработчика должно совпадать с тем, что сгенерировал дизайнер
        // для вашей кнопки "ОК"
        private void btnOkFilter_Click(object sender, EventArgs e) // Пример имени, проверьте ваше
        {
            CurrentFilterOptions.GoodsType = (FilterGoodsType)cmbFilterType.SelectedItem;
            CurrentFilterOptions.FilterByPrice = chkFilterByPrice.Checked;

            if (chkFilterByPrice.Checked)
            {
                if (numMinPrice.Value > numMaxPrice.Value)
                {
                    MessageBox.Show("Минимальная цена не может быть больше максимальной.", "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                CurrentFilterOptions.MinPrice = numMinPrice.Value;
                CurrentFilterOptions.MaxPrice = numMaxPrice.Value;
            }
            this.DialogResult = DialogResult.OK;
        }

        // Если для кнопки "Отмена" DialogResult установлен в Cancel в дизайнере,
        // отдельный обработчик не нужен.
    }

    // Вспомогательные enum и классы могут идти ПОСЛЕ класса формы
    public enum FilterGoodsType { All, Product, DairyProduct, Toy }

    public class FilterOptions
    {
        public FilterGoodsType GoodsType { get; set; } = FilterGoodsType.All;
        public bool FilterByPrice { get; set; } = false;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = decimal.MaxValue;
    }
}