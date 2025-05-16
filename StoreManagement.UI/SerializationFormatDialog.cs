// SerializationFormatDialog.cs
using System;
using System.Windows.Forms;
// Нет необходимости в 'using StoreManagement.Services;' здесь, если мы используем полное имя
// или если бы не было конфликта и 'StoreManagement.Services.SerializationFormat' был единственным.

namespace StoreManagement.UI
{
    // Класс формы ДОЛЖЕН БЫТЬ ПЕРВЫМ в файле для корректной работы дизайнера
    public partial class SerializationFormatDialog : Form
    {
        // Используем полное имя для нашего enum, чтобы избежать неоднозначности
        public StoreManagement.Services.SerializationFormat SelectedFormat { get; private set; }

        public SerializationFormatDialog()
        {
            InitializeComponent();
            InitializeFormatComboBox();
        }

        private void InitializeFormatComboBox()
        {
            // Заполняем ComboBox значениями из нашего enum SerializationFormat
            cmbFormat.DataSource = Enum.GetValues(typeof(StoreManagement.Services.SerializationFormat));
            cmbFormat.SelectedIndex = 0; // Выбираем Binary по умолчанию
        }

        // Убедитесь, что имя этого обработчика (btnOk_Click) соответствует
        // имени, которое вы дали кнопке "ОК" в дизайнере и для которого вы создали событие Click.
        // Если вы назвали кнопку btnOk, то и обработчик должен быть btnOk_Click.
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cmbFormat.SelectedItem != null)
            {
                // Приводим к нашему enum SerializationFormat
                SelectedFormat = (StoreManagement.Services.SerializationFormat)cmbFormat.SelectedItem;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите формат.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Не закрываем диалог, DialogResult не устанавливается
            }
        }

        // Если для кнопки "Отмена" (например, btnCancel) свойство DialogResult установлено в Cancel
        // в дизайнере, то отдельный обработчик Click для простого закрытия не нужен.
        // private void btnCancel_Click(object sender, EventArgs e)
        // {
        //     this.DialogResult = DialogResult.Cancel;
        // }
    }
}