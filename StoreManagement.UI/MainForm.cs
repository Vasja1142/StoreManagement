// MainForm.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO; // Для Path и File
using System.Linq;
using System.Text; // Для StringBuilder и Encoding
using System.Threading.Tasks;
using System.Windows.Forms;
using StoreManagement.Collections;
using StoreManagement.Domain;
using StoreManagement.Domain.Comparers;
using StoreManagement.Services;
// using Microsoft.VisualBasic;


namespace StoreManagement.UI
{
    public partial class MainForm : Form
    {
        private MyNewCollection<Goods> _goodsCollection;
        private Journal _journal;
        private const string DefaultJournalFileName = "store_gui_journal.log";
        private List<Goods>? _currentlyDisplayedSortedList = null;
        private string _lastLinqResults = string.Empty; // Для хранения результатов LINQ

        public MainForm()
        {
            InitializeComponent();
            InitializeCoreLogic();
            SetupDataGridViewColumns();
            RefreshDisplay();
        }

        private void InitializeCoreLogic()
        {
            _goodsCollection = new MyNewCollection<Goods>("Основная Коллекция (GUI)");
            _journal = new Journal(DefaultJournalFileName);
            _goodsCollection.CollectionCountChanged += GoodsCollection_CollectionChanged;
            _goodsCollection.CollectionReferenceChanged += GoodsCollection_CollectionChanged;

            _goodsCollection.CollectionCountChanged += _journal.CollectionCountChangedHandler;
            _goodsCollection.CollectionReferenceChanged += _journal.CollectionReferenceChangedHandler;
            Console.WriteLine($"Журнал GUI будет записываться в файл: {_journal.GetActualLogFilePath()}");
        }

        private void SetupDataGridViewColumns()
        {
            dgvGoods.AutoGenerateColumns = false;
            dgvGoods.Columns.Clear();
            dgvGoods.Columns.Add(new DataGridViewTextBoxColumn { Name = "colType", HeaderText = "Тип", DataPropertyName = "TypeName", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgvGoods.Columns.Add(new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "Имя", DataPropertyName = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvGoods.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPrice", HeaderText = "Цена", DataPropertyName = "Price", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgvGoods.Columns.Add(new DataGridViewTextBoxColumn { Name = "colManufacturer", HeaderText = "Производитель", DataPropertyName = "Manufacturer", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgvGoods.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSpecificInfo", HeaderText = "Доп. Инфо", DataPropertyName = "SpecificInfo", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, MinimumWidth = 150 });
        }

        private void GoodsCollection_CollectionChanged(object source, CollectionHandlerEventArgs args)
        {
            _currentlyDisplayedSortedList = null;
            if (this.InvokeRequired) { this.Invoke(new Action(RefreshDisplay)); }
            else { RefreshDisplay(); }
        }

        public void RefreshDisplay()
        {
            var displayListSource = new List<Goods>();
            if (_goodsCollection != null)
            {
                if (_currentlyDisplayedSortedList != null) { displayListSource.AddRange(_currentlyDisplayedSortedList); }
                else { displayListSource.AddRange(_goodsCollection.OrderBy(g => g.Name)); }
            }
            var finalDisplayItems = displayListSource.Select(g => new GoodsDisplayItem(g)).ToList();
            string? selectedGoodName = null;
            if (dgvGoods.CurrentRow?.DataBoundItem is GoodsDisplayItem currentDisplayItem) { selectedGoodName = currentDisplayItem.Name; }
            dgvGoods.DataSource = null;
            dgvGoods.DataSource = finalDisplayItems;
            if (selectedGoodName != null)
            {
                foreach (DataGridViewRow row in dgvGoods.Rows)
                {
                    if (row.DataBoundItem is GoodsDisplayItem displayItem && displayItem.Name == selectedGoodName)
                    {
                        row.Selected = true;
                        if (dgvGoods.Columns.Count > 0) dgvGoods.CurrentCell = row.Cells[0];
                        break;
                    }
                }
            }
            statusLabelItemCount.Text = $"Элементов в коллекции: {_goodsCollection?.Count ?? 0}";
        }

        // --- Обработчики меню Файл ---
        private void menuFileExit_Click(object sender, EventArgs e) => Application.Exit();
        private void menuFileSave_Click(object sender, EventArgs e)
        {
            if (_goodsCollection.Count == 0) { MessageBox.Show("Коллекция пуста.", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            StoreManagement.Services.SerializationFormat selectedFormat;
            using (var formatDialog = new SerializationFormatDialog { Text = "Выберите формат для сохранения" })
            {
                if (formatDialog.ShowDialog(this) != DialogResult.OK) return;
                selectedFormat = formatDialog.SelectedFormat;
            }
            using (var saveFileDialog = new SaveFileDialog { Title = "Сохранить коллекцию как...", FileName = "goods_collection" })
            {
                saveFileDialog.Filter = selectedFormat switch
                {
                    StoreManagement.Services.SerializationFormat.Binary => "Двоичные файлы (*.bin)|*.bin",
                    StoreManagement.Services.SerializationFormat.Json => "JSON файлы (*.json)|*.json",
                    StoreManagement.Services.SerializationFormat.Xml => "XML файлы (*.xml)|*.xml",
                    _ => "Все файлы (*.*)|*.*"
                } + "|Все файлы (*.*)|*.*";
                saveFileDialog.DefaultExt = selectedFormat.ToString().ToLower();
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        PersistenceService.SaveCollection(_goodsCollection, saveFileDialog.FileName, selectedFormat);
                        MessageBox.Show($"Коллекция сохранена в: {saveFileDialog.FileName}\nФормат: {selectedFormat}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }
        private void menuFileLoad_Click(object sender, EventArgs e)
        {
            string? originalCollectionName = _goodsCollection?.Name;
            if (_goodsCollection.Count > 0 && MessageBox.Show("Текущая коллекция содержит данные. Заменить?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;
            StoreManagement.Services.SerializationFormat selectedFormat;
            using (var formatDialog = new SerializationFormatDialog { Text = "Выберите формат для загрузки" })
            {
                if (formatDialog.ShowDialog(this) != DialogResult.OK) return;
                selectedFormat = formatDialog.SelectedFormat;
            }
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Загрузить коллекцию из файла...";
                openFileDialog.Filter = selectedFormat switch
                {
                    StoreManagement.Services.SerializationFormat.Binary => "Двоичные файлы (*.bin)|*.bin",
                    StoreManagement.Services.SerializationFormat.Json => "JSON файлы (*.json)|*.json",
                    StoreManagement.Services.SerializationFormat.Xml => "XML файлы (*.xml)|*.xml",
                    _ => "Все файлы (*.*)|*.*"
                } + "|Все файлы (*.*)|*.*";
                openFileDialog.DefaultExt = selectedFormat.ToString().ToLower();
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    // filePath определяется ЗДЕСЬ и доступна внутри этого блока if
                    string filePath = openFileDialog.FileName;
                    MyNewCollection<Goods> loadedCollection = null; // Объявляем здесь

                    try
                    {
                        // Загружаем во временную коллекцию
                        loadedCollection = PersistenceService.LoadCollection<Goods>(
                            originalCollectionName ?? "Загруженная Коллекция",
                            filePath, // <--- filePath ЗДЕСЬ ДОСТУПНА
                            selectedFormat
                        );

                        // Если загрузка прошла успешно (PersistenceService не бросил исключение и вернул коллекцию)
                        // PersistenceService.LoadCollection в случае неудачи (файл не найден/ошибка)
                        // возвращает новую ПУСТУЮ коллекцию, а не null, и выводит сообщение в консоль.
                        // Поэтому проверка loadedCollection != null не совсем точна, если он всегда возвращает объект.
                        // Лучше ориентироваться на отсутствие исключений.

                        // 1. Отписываемся от событий ТЕКУЩЕЙ (старой) _goodsCollection
                        if (_goodsCollection != null)
                        {
                            _goodsCollection.CollectionCountChanged -= GoodsCollection_CollectionChanged;
                            _goodsCollection.CollectionReferenceChanged -= GoodsCollection_CollectionChanged;
                            _goodsCollection.CollectionCountChanged -= _journal.CollectionCountChangedHandler;
                            _goodsCollection.CollectionReferenceChanged -= _journal.CollectionReferenceChangedHandler;
                        }

                        // 2. Заменяем ссылку на коллекцию
                        _goodsCollection = loadedCollection; // loadedCollection здесь не будет null, если PersistenceService так работает

                        // 3. Подписываемся на события НОВОЙ _goodsCollection
                        if (_goodsCollection != null) // Все равно полезно проверить
                        {
                            _goodsCollection.CollectionCountChanged += GoodsCollection_CollectionChanged;
                            _goodsCollection.CollectionReferenceChanged += GoodsCollection_CollectionChanged;
                            _goodsCollection.CollectionCountChanged += _journal.CollectionCountChangedHandler;
                            _goodsCollection.CollectionReferenceChanged += _journal.CollectionReferenceChangedHandler;
                        }

                        _currentlyDisplayedSortedList = null;
                        RefreshDisplay();
                        MessageBox.Show($"Коллекция успешно загружена из файла:\n{filePath}\nФормат: {selectedFormat}",
                                        "Загрузка успешна", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке коллекции: {ex.Message}\n\nВозможно, файл поврежден или имеет неверный формат.",
                                        "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        // Если произошла ошибка, _goodsCollection осталась старой (или той, что была до try).
                        // Если мы УСПЕЛИ отписаться от старой _goodsCollection, а потом произошла ошибка 
                        // ДО присвоения _goodsCollection = loadedCollection, то нужно восстановить подписки.
                        // Однако, в текущей структуре, если PersistenceService бросает исключение,
                        // мы не дойдем до блока отписки/присвоения/подписки.
                        // Если же ошибка произошла ПОСЛЕ присвоения _goodsCollection = loadedCollection,
                        // но ДО подписки на новую (маловероятно), то нужно восстанавливать.
                        // Для большей надежности, если ошибка, можно попытаться переподписаться на _goodsCollection,
                        // какой бы она ни была на данный момент.
                        if (_goodsCollection != null)
                        {
                            // Сначала отписываемся, чтобы избежать двойной подписки
                            _goodsCollection.CollectionCountChanged -= GoodsCollection_CollectionChanged;
                            _goodsCollection.CollectionReferenceChanged -= GoodsCollection_CollectionChanged;
                            _goodsCollection.CollectionCountChanged -= _journal.CollectionCountChangedHandler;
                            _goodsCollection.CollectionReferenceChanged -= _journal.CollectionReferenceChangedHandler;
                            // Затем подписываемся снова
                            _goodsCollection.CollectionCountChanged += GoodsCollection_CollectionChanged;
                            _goodsCollection.CollectionReferenceChanged += GoodsCollection_CollectionChanged;
                            _goodsCollection.CollectionCountChanged += _journal.CollectionCountChangedHandler;
                            _goodsCollection.CollectionReferenceChanged += _journal.CollectionReferenceChangedHandler;
                        }
                        RefreshDisplay();
                    }
                }
            }
        }

        // --- Обработчики меню Коллекция ---
        private void menuCollectionAdd_Click(object sender, EventArgs e)
        {
            using (EditGoodForm addForm = new EditGoodForm())
            {
                if (addForm.ShowDialog(this) == DialogResult.OK)
                {
                    Goods? newGood = addForm.CurrentGoods;
                    if (newGood != null)
                    {
                        if (string.IsNullOrWhiteSpace(newGood.Name)) { MessageBox.Show("Имя не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                        if (_goodsCollection.Any(g => g.Name.Equals(newGood.Name, StringComparison.OrdinalIgnoreCase)))
                        { MessageBox.Show($"Товар с именем '{newGood.Name}' уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        else { _goodsCollection.Add(newGood); MessageBox.Show($"Товар '{newGood.Name}' добавлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                    }
                }
            }
        }
        private void menuCollectionEdit_Click(object sender, EventArgs e)
        {
            if (dgvGoods.CurrentRow == null || !(dgvGoods.CurrentRow.DataBoundItem is GoodsDisplayItem selectedDisplayItem))
            { MessageBox.Show("Выберите товар.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            Goods originalGood = selectedDisplayItem.OriginalGood;
            string oldName = originalGood.Name;
            Goods? goodToEdit = originalGood.Clone() as Goods;
            if (goodToEdit == null) { MessageBox.Show("Ошибка клонирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            using (EditGoodForm editForm = new EditGoodForm(goodToEdit))
            {
                if (editForm.ShowDialog(this) == DialogResult.OK)
                {
                    Goods? editedGood = editForm.CurrentGoods;
                    if (editedGood != null)
                    {
                        if (string.IsNullOrWhiteSpace(editedGood.Name)) { MessageBox.Show("Имя не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                        if (!editedGood.Name.Equals(oldName, StringComparison.OrdinalIgnoreCase) &&
                            _goodsCollection.Any(g => g.Name.Equals(editedGood.Name, StringComparison.OrdinalIgnoreCase)))
                        { MessageBox.Show($"Товар с именем '{editedGood.Name}' уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                        Goods? actualOriginalInCollection = _goodsCollection.FirstOrDefault(g => g.Name.Equals(oldName, StringComparison.OrdinalIgnoreCase));
                        if (actualOriginalInCollection != null && _goodsCollection.Remove(actualOriginalInCollection))
                        { _goodsCollection.Add(editedGood); MessageBox.Show($"Товар '{editedGood.Name}' обновлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                        else { MessageBox.Show($"Ошибка обновления '{oldName}'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                }
            }
        }
        private void menuCollectionRemove_Click(object sender, EventArgs e)
        {
            if (dgvGoods.CurrentRow == null || !(dgvGoods.CurrentRow.DataBoundItem is GoodsDisplayItem selectedDisplayItem))
            { MessageBox.Show("Выберите товар.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            Goods goodToRemove = selectedDisplayItem.OriginalGood;
            if (MessageBox.Show($"Удалить '{goodToRemove.Name}'?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_goodsCollection.Remove(goodToRemove)) { MessageBox.Show($"Товар '{goodToRemove.Name}' удален.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                else { MessageBox.Show($"Ошибка удаления '{goodToRemove.Name}'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
        private void menuCollectionClear_Click(object sender, EventArgs e)
        {
            if (_goodsCollection.Count == 0) { MessageBox.Show("Коллекция уже пуста.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            if (MessageBox.Show("Очистить всю коллекцию?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            { _goodsCollection.Clear(); MessageBox.Show("Коллекция очищена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }
        private void menuCollectionAutoGenerate_Click(object sender, EventArgs e)
        {
            string input = ""; try { input = Microsoft.VisualBasic.Interaction.InputBox("Количество элементов:", "Авто-генерация", "5"); }
            catch (Exception) { MessageBox.Show("InputBox не доступен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (int.TryParse(input, out int count) && count > 0 && count <= 100)
            {
                if (_goodsCollection.Count > 0 && MessageBox.Show("Очистить текущую коллекцию?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                { _goodsCollection.Clear(); }
                Random random = new Random(Guid.NewGuid().GetHashCode());
                for (int i = 0; i < count; i++)
                {
                    Goods newGood; bool unique; int attempts = 0;
                    do { newGood = CreateRandomGoodsForGui(random); unique = !_goodsCollection.Any(g => g.Name.Equals(newGood.Name, StringComparison.OrdinalIgnoreCase)); attempts++; if (!unique && attempts > 10) { newGood.Name += $"_u{random.Next(100)}"; unique = true; } } while (!unique);
                    _goodsCollection.Add(newGood);
                }
                MessageBox.Show($"Добавлено {count} элементов.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (!string.IsNullOrEmpty(input)) { MessageBox.Show("Неверный ввод (1-100).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private Goods CreateRandomGoodsForGui(Random random)
        {
            Goods newItem; int typeChoice = random.Next(3);
            switch (typeChoice) { case 0: newItem = new DairyProduct(); break; case 1: newItem = new Toy(); break; default: newItem = new Product(); break; }
            newItem.RandomInit(); newItem.Name += $"_{random.Next(10000, 99999)}"; return newItem;
        }

        // --- Обработчики меню Операции ---
        private void menuOperationsSearch_Click(object sender, EventArgs e)
        {
            if (_goodsCollection.Count == 0) { MessageBox.Show("Коллекция пуста.", "Поиск", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            string searchTerm = ""; try { searchTerm = Microsoft.VisualBasic.Interaction.InputBox("Имя для поиска:", "Поиск товара", ""); }
            catch (Exception) { MessageBox.Show("InputBox не доступен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrWhiteSpace(searchTerm)) return;

            var searchResults = _goodsCollection.Where(g => g.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0).OrderBy(g => g.Name).ToList();
            if (searchResults.Any()) { using (var resultsForm = new SearchResultsForm(searchResults, searchTerm)) resultsForm.ShowDialog(this); }
            else { MessageBox.Show($"Товары с '{searchTerm}' не найдены.", "Поиск", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }
        private void menuOperationsSort_Click(object sender, EventArgs e)
        {
            if (_goodsCollection.Count < 2) { MessageBox.Show("Нужно хотя бы 2 элемента для сортировки.", "Сортировка", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            SortCriteria criteria;
            using (SortOptionsDialog sortDialog = new SortOptionsDialog())
            {
                if (sortDialog.ShowDialog(this) != DialogResult.OK) { _currentlyDisplayedSortedList = null; RefreshDisplay(); return; }
                criteria = sortDialog.SelectedCriteria;
            }
            List<Goods> sortedList;
            if (criteria == SortCriteria.ByName) { sortedList = _goodsCollection.OrderBy(g => g, new SortByNameComparer()).ToList(); }
            else { sortedList = _goodsCollection.OrderBy(g => g).ToList(); }
            _currentlyDisplayedSortedList = sortedList;
            RefreshDisplay();
            MessageBox.Show($"Отображение отсортировано по {(criteria == SortCriteria.ByName ? "Имени" : "Цене")}.", "Сортировка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void menuOperationsFilter_Click(object sender, EventArgs e)
        {
            if (_goodsCollection.Count == 0) { MessageBox.Show("Коллекция пуста.", "Фильтрация", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            FilterOptions filterOptions;
            using (var filterDialog = new FilterOptionsDialog())
            {
                if (filterDialog.ShowDialog(this) != DialogResult.OK) return;
                filterOptions = filterDialog.CurrentFilterOptions;
            }
            IEnumerable<Goods> filteredResults = _goodsCollection;
            switch (filterOptions.GoodsType)
            {
                case FilterGoodsType.Product: filteredResults = filteredResults.Where(g => g.GetType() == typeof(Product)); break;
                case FilterGoodsType.DairyProduct: filteredResults = filteredResults.OfType<DairyProduct>(); break;
                case FilterGoodsType.Toy: filteredResults = filteredResults.OfType<Toy>(); break;
            }
            if (filterOptions.FilterByPrice)
            { filteredResults = filteredResults.Where(g => g.Price >= filterOptions.MinPrice && g.Price <= filterOptions.MaxPrice); }
            var finalResultsList = filteredResults.OrderBy(g => g.Name).ToList();
            if (finalResultsList.Any()) { using (var resultsForm = new SearchResultsForm(finalResultsList, "Результаты фильтрации")) resultsForm.ShowDialog(this); }
            else { MessageBox.Show("Товары по фильтру не найдены.", "Фильтрация", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }
        private void menuOperationsLinq_Click(object sender, EventArgs e)
        {
            if (_goodsCollection.Count == 0)
            {
                MessageBox.Show("Коллекция пуста. Выполнение LINQ-запросов невозможно.", "LINQ-запросы", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _lastLinqResults = "Коллекция пуста."; // Сохраняем это для отчета
                return;
            }

            StringBuilder resultsBuilder = new StringBuilder();
            resultsBuilder.AppendLine("===== Результаты LINQ-запросов =====");
            resultsBuilder.AppendLine($"Всего элементов в коллекции: {_goodsCollection.Count}");
            resultsBuilder.AppendLine("------------------------------------");

            try
            {
                var mostExpensiveToy = _goodsCollection.OfType<Toy>()
                                                     .OrderByDescending(t => t.Price)
                                                     .FirstOrDefault();
                resultsBuilder.AppendLine("\n1. Самая дорогая игрушка:");
                if (mostExpensiveToy != null) { resultsBuilder.AppendLine($"   - {mostExpensiveToy.Name}, Цена: {mostExpensiveToy.Price:C}, Произв.: {mostExpensiveToy.Manufacturer}"); }
                else { resultsBuilder.AppendLine("   - Игрушки не найдены."); }

                int dairyProductCount = _goodsCollection.OfType<DairyProduct>().Count();
                resultsBuilder.AppendLine($"\n2. Количество молочных продуктов: {dairyProductCount} шт.");

                decimal averagePrice = _goodsCollection.Any() ? _goodsCollection.Average(g => g.Price) : 0m;
                resultsBuilder.AppendLine($"\n3. Средняя цена всех товаров: {averagePrice:C}");

                resultsBuilder.AppendLine("\n4. Группировка по производителю (количество товаров):");
                var groupedByManufacturer = _goodsCollection.GroupBy(g => g.Manufacturer)
                                            .Select(group => new { Manufacturer = group.Key, Count = group.Count() })
                                            .OrderBy(g => g.Manufacturer);
                if (groupedByManufacturer.Any())
                { foreach (var group in groupedByManufacturer) { resultsBuilder.AppendLine($"   - {group.Manufacturer}: {group.Count} шт."); } }
                else { resultsBuilder.AppendLine("   - Данные для группировки отсутствуют."); }
                resultsBuilder.AppendLine("------------------------------------");

                MessageBox.Show(resultsBuilder.ToString(), "Результаты LINQ-запросов", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении LINQ-запросов: {ex.Message}", "Ошибка LINQ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resultsBuilder.AppendLine($"\nОШИБКА: {ex.Message}");
            }
            _lastLinqResults = resultsBuilder.ToString();
        }


        // --- Обработчики меню Отчеты ---
        private void menuReportsViewJournal_Click(object sender, EventArgs e)
        {
            if (_journal == null) { MessageBox.Show("Журнал не инициализирован.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            try
            {
                string journalContent = _journal.ReadLogFile();
                if (string.IsNullOrEmpty(journalContent) || journalContent.StartsWith("[ОШИБКА ЧТЕНИЯ ЖУРНАЛА]") || journalContent.Contains("не найден"))
                { MessageBox.Show(string.IsNullOrWhiteSpace(journalContent) ? "Файл журнала пуст или не найден." : journalContent, "Журнал", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                using (JournalViewForm viewForm = new JournalViewForm(journalContent)) viewForm.ShowDialog(this);
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка чтения журнала: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void menuReportsSaveLinq_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_lastLinqResults) || (_lastLinqResults == "Коллекция пуста." && _goodsCollection.Count == 0))
            {
                MessageBox.Show("Сначала выполните LINQ-запросы (Операции -> Выполнить LINQ-запросы) или коллекция пуста.",
                                "Сохранение LINQ-отчета", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Сохранить LINQ-отчет как...";
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.FileName = "linq_report";
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(saveFileDialog.FileName, _lastLinqResults, Encoding.UTF8);
                        MessageBox.Show($"LINQ-отчет сохранен в: {saveFileDialog.FileName}", "Отчет сохранен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show($"Ошибка сохранения LINQ-отчета: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }
        private void menuReportsViewLinq_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Открыть LINQ-отчет...";
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                openFileDialog.DefaultExt = "txt";
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        string reportContent = File.ReadAllText(openFileDialog.FileName, Encoding.UTF8);
                        if (string.IsNullOrWhiteSpace(reportContent)) { MessageBox.Show("Файл отчета пуст.", "Просмотр отчета", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                        using (JournalViewForm reportViewForm = new JournalViewForm(reportContent))
                        { reportViewForm.Text = $"Просмотр LINQ-отчета: {Path.GetFileName(openFileDialog.FileName)}"; reportViewForm.ShowDialog(this); }
                    }
                    catch (Exception ex) { MessageBox.Show($"Ошибка чтения LINQ-отчета: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        // TODO: Реализовать menuHelpAbout_Click
    }

    // GoodsDisplayItem (без изменений)
    public class GoodsDisplayItem
    {
        private Goods _good;
        public string TypeName => _good.GetType().Name;
        public string Name => _good.Name;
        public decimal Price => _good.Price;
        public string Manufacturer => _good.Manufacturer;
        public string SpecificInfo
        {
            get
            {
                if (_good is Product product)
                {
                    string info = $"Срок годн.: {product.ExpirationDate:yyyy-MM-dd}";
                    if (product is DairyProduct dairy) { info += $", Жирн.: {dairy.FatContent:F1}%, Объем: {dairy.Volume:F2}л"; }
                    return info;
                }
                if (_good is Toy toy) { return $"Возраст: {toy.AgeRestriction}+, Матер.: {toy.Material}"; }
                return string.Empty;
            }
        }
        public Goods OriginalGood => _good;
        public GoodsDisplayItem(Goods good) { _good = good ?? throw new ArgumentNullException(nameof(good)); }
    }
}