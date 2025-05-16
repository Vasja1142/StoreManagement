// EditGoodForm.cs
using System;
using System.Windows.Forms;
using StoreManagement.Domain;

namespace StoreManagement.UI
{
    public partial class EditGoodForm : Form
    {
        public Goods? CurrentGoods { get; private set; }
        private bool _isEditMode = false;

        // Конструктор для добавления нового товара
        public EditGoodForm()
        {
            InitializeComponent();
            _isEditMode = false;
            this.Text = "Добавить новый товар";
            InitializeComboBox();
            UpdateSpecificPanels();
        }

        // Конструктор для редактирования существующего товара
        public EditGoodForm(Goods goodsToEdit)
        {
            InitializeComponent();
            _isEditMode = true;
            this.Text = "Редактировать товар";
            CurrentGoods = goodsToEdit ?? throw new ArgumentNullException(nameof(goodsToEdit));
            InitializeComboBox();
            LoadGoodsData();
            UpdateSpecificPanels();
        }

        private void InitializeComboBox()
        {
            cmbGoodsType.Items.Clear();
            cmbGoodsType.Items.Add("Продукт (Product)");
            cmbGoodsType.Items.Add("Молочный продукт (DairyProduct)");
            cmbGoodsType.Items.Add("Игрушка (Toy)");

            if (_isEditMode && CurrentGoods != null)
            {
                if (CurrentGoods is DairyProduct)
                    cmbGoodsType.SelectedItem = "Молочный продукт (DairyProduct)";
                else if (CurrentGoods is Product)
                    cmbGoodsType.SelectedItem = "Продукт (Product)";
                else if (CurrentGoods is Toy)
                    cmbGoodsType.SelectedItem = "Игрушка (Toy)";

                cmbGoodsType.Enabled = false;
            }
            else
            {
                cmbGoodsType.SelectedIndex = 0;
                cmbGoodsType.Enabled = true;
            }
        }

        private void LoadGoodsData()
        {
            if (CurrentGoods == null) return;

            txtName.Text = CurrentGoods.Name;
            numPrice.Value = CurrentGoods.Price; // Убедитесь, что Price не выходит за Minimum/Maximum numPrice
            txtManufacturer.Text = CurrentGoods.Manufacturer;

            if (CurrentGoods is Product product)
            {
                // Проверка на MinDate для DateTimePicker
                if (product.ExpirationDate >= dtpExpirationDate.MinDate && product.ExpirationDate <= dtpExpirationDate.MaxDate)
                {
                    dtpExpirationDate.Value = product.ExpirationDate;
                }
                else
                {
                    dtpExpirationDate.Value = DateTime.Today.AddDays(1); // Значение по умолчанию, если дата некорректна
                }

                if (product is DairyProduct dairy)
                {
                    numFatContent.Value = Math.Max(numFatContent.Minimum, Math.Min(numFatContent.Maximum, (decimal)dairy.FatContent));
                    numVolume.Value = Math.Max(numVolume.Minimum, Math.Min(numVolume.Maximum, (decimal)dairy.Volume));
                }
            }
            else if (CurrentGoods is Toy toy)
            {
                numAgeRestriction.Value = Math.Max(numAgeRestriction.Minimum, Math.Min(numAgeRestriction.Maximum, toy.AgeRestriction));
                txtMaterial.Text = toy.Material;
            }
        }

        private void cmbGoodsType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSpecificPanels();
        }

        private void UpdateSpecificPanels()
        {
            string? selectedType = cmbGoodsType.SelectedItem?.ToString();

            panelProductSpecifics.Visible = false;
            panelDairySpecifics.Visible = false;
            panelToySpecifics.Visible = false;

            if (selectedType == "Продукт (Product)")
            {
                panelProductSpecifics.Visible = true;
            }
            else if (selectedType == "Молочный продукт (DairyProduct)")
            {
                panelProductSpecifics.Visible = true;
                panelDairySpecifics.Visible = true;
            }
            else if (selectedType == "Игрушка (Toy)")
            {
                panelToySpecifics.Visible = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Имя товара не может быть пустым.", "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtName.Focus();
                return;
            }
            if (numPrice.Value <= 0)
            {
                MessageBox.Show("Цена должна быть больше нуля.", "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numPrice.Focus();
                return;
            }


            try
            {
                string name = txtName.Text;
                decimal price = numPrice.Value;
                string manufacturer = txtManufacturer.Text;
                string? selectedType = cmbGoodsType.SelectedItem?.ToString();

                // Сбрасываем CurrentGoods перед попыткой создания/обновления в режиме добавления,
                // чтобы убедиться, что он не содержит "мусор" от предыдущих неудачных попыток (если были).
                // В режиме редактирования он уже должен быть установлен.
                if (!_isEditMode)
                {
                    CurrentGoods = null;
                }

                if (_isEditMode && CurrentGoods != null)
                {
                    // Логика обновления существующего CurrentGoods
                    CurrentGoods.Name = name;
                    CurrentGoods.Price = price;
                    CurrentGoods.Manufacturer = manufacturer;

                    if (CurrentGoods is Product product)
                    {
                        product.ExpirationDate = dtpExpirationDate.Value;
                        if (product is DairyProduct dairy)
                        {
                            dairy.FatContent = (double)numFatContent.Value;
                            dairy.Volume = (double)numVolume.Value;
                        }
                    }
                    else if (CurrentGoods is Toy toy)
                    {
                        toy.AgeRestriction = (int)numAgeRestriction.Value;
                        toy.Material = txtMaterial.Text;
                    }
                    // CurrentGoods уже обновлен
                }
                else // Добавление нового
                {
                    switch (selectedType)
                    {
                        case "Продукт (Product)":
                            CurrentGoods = new Product(name, price, manufacturer, dtpExpirationDate.Value);
                            break;
                        case "Молочный продукт (DairyProduct)":
                            CurrentGoods = new DairyProduct(name, price, manufacturer, dtpExpirationDate.Value,
                                (double)numFatContent.Value, (double)numVolume.Value);
                            break;
                        case "Игрушка (Toy)":
                            CurrentGoods = new Toy(name, price, manufacturer, (int)numAgeRestriction.Value, txtMaterial.Text);
                            break;
                        default:
                            MessageBox.Show($"Не выбран или неизвестный тип товара. Выбрано: '{selectedType ?? "NULL"}'", "Ошибка типа в EditGoodForm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // Важно: выходим, DialogResult не будет OK
                    }
                }

                // ОТЛАДКА: Проверяем CurrentGoods ПЕРЕД установкой DialogResult
                if (CurrentGoods == null)
                {
                    MessageBox.Show("ВНИМАНИЕ в EditGoodForm: CurrentGoods IS NULL ПЕРЕД DialogResult.OK!", "ОШИБКА EditGoodForm", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    // Если CurrentGoods null, DialogResult не должен быть OK, поэтому можно здесь сделать return
                    // или убедиться, что DialogResult не устанавливается в OK.
                    // Но если мы дошли сюда после switch, он должен быть не null (кроме default case, где есть return).
                    // Эта проверка больше для паранойи или если логика редактирования не присвоит CurrentGoods.
                }
                else
                {
                    MessageBox.Show($"EditGoodForm: CurrentGoods ГОТОВ ('{CurrentGoods.Name ?? "ИМЯ NULL"}', Тип: {CurrentGoods.GetType().Name}). Устанавливаем DialogResult.OK.", "Отладка EditGoodForm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // CurrentGoods может быть частично инициализирован или null, не полагаемся на него
                MessageBox.Show($"Ошибка данных в EditGoodForm (ArgumentOutOfRange): {ex.Message}", "Ошибка валидации EditGoodForm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // this.DialogResult НЕ устанавливаем в OK
            }
            catch (Exception ex)
            {
                // CurrentGoods может быть частично инициализирован или null
                MessageBox.Show($"Произошла ошибка в EditGoodForm: {ex.Message}", "Ошибка EditGoodForm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // this.DialogResult НЕ устанавливаем в OK
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; // Явно устанавливаем для кнопки Отмена
            this.Close();
        }
    }
}