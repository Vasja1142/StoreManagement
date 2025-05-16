// SearchResultsForm.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using StoreManagement.Domain; // Для Goods

namespace StoreManagement.UI
{
    public partial class SearchResultsForm : Form
    {
        public SearchResultsForm(IEnumerable<Goods> results, string searchTerm)
        {
            InitializeComponent();
            this.Text = $"Результаты поиска: '{searchTerm}'";
            SetupColumns();
            LoadResults(results);
        }

        private void SetupColumns()
        {
            dgvSearchResults.AutoGenerateColumns = false;
            dgvSearchResults.Columns.Clear();

            // Используем те же имена свойств, что и в MainForm для GoodsDisplayItem
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTypeRes", HeaderText = "Тип", DataPropertyName = "TypeName", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNameRes", HeaderText = "Имя", DataPropertyName = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPriceRes", HeaderText = "Цена", DataPropertyName = "Price", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "colManRes", HeaderText = "Производитель", DataPropertyName = "Manufacturer", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSpecRes", HeaderText = "Доп. Инфо", DataPropertyName = "SpecificInfo", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, MinimumWidth = 150 });
        }

        private void LoadResults(IEnumerable<Goods> results)
        {
            var displayList = results.Select(g => new GoodsDisplayItem(g)).ToList();
            dgvSearchResults.DataSource = displayList;
            if (!displayList.Any())
            {
                MessageBox.Show("Товары, соответствующие критериям поиска, не найдены.", "Поиск", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик для кнопки Закрыть (если добавили ее)
        // private void btnClose_Click(object sender, EventArgs e)
        // {
        //    this.Close();
        // }
    }
}