namespace StoreManagement.UI
{
    partial class MainForm
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
            menuStrip1 = new MenuStrip();
            toolStripMenuItem1 = new ToolStripMenuItem();
            menuFileLoad = new ToolStripMenuItem();
            menuFileSave = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            menuFileExit = new ToolStripMenuItem();
            коллекцияToolStripMenuItem = new ToolStripMenuItem();
            menuCollectionAutoGenerate = new ToolStripMenuItem();
            menuCollectionAdd = new ToolStripMenuItem();
            menuCollectionEdit = new ToolStripMenuItem();
            menuCollectionRemove = new ToolStripMenuItem();
            menuCollectionClear = new ToolStripMenuItem();
            операцииToolStripMenuItem = new ToolStripMenuItem();
            menuOperationsSearch = new ToolStripMenuItem();
            menuOperationsSort = new ToolStripMenuItem();
            menuOperationsFilter = new ToolStripMenuItem();
            menuOperationsLinq = new ToolStripMenuItem();
            отчетыToolStripMenuItem = new ToolStripMenuItem();
            menuReportsViewJournal = new ToolStripMenuItem();
            menuReportsSaveLinq = new ToolStripMenuItem();
            menuReportsViewLinq = new ToolStripMenuItem();
            dgvGoods = new DataGridView();
            StatusStrip = new StatusStrip();
            statusLabelItemCount = new ToolStripStatusLabel();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGoods).BeginInit();
            StatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, коллекцияToolStripMenuItem, операцииToolStripMenuItem, отчетыToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { menuFileLoad, menuFileSave, toolStripMenuItem2, menuFileExit });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(48, 20);
            toolStripMenuItem1.Text = "Файл";
            // 
            // menuFileLoad
            // 
            menuFileLoad.Name = "menuFileLoad";
            menuFileLoad.Size = new Size(207, 22);
            menuFileLoad.Text = "Загрузить коллекцию...";
            menuFileLoad.Click += menuFileLoad_Click;
            // 
            // menuFileSave
            // 
            menuFileSave.Name = "menuFileSave";
            menuFileSave.Size = new Size(207, 22);
            menuFileSave.Text = "Сохранить коллекцию...";
            menuFileSave.Click += menuFileSave_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(204, 6);
            // 
            // menuFileExit
            // 
            menuFileExit.Name = "menuFileExit";
            menuFileExit.Size = new Size(207, 22);
            menuFileExit.Text = "Выход";
            menuFileExit.Click += menuFileExit_Click;
            // 
            // коллекцияToolStripMenuItem
            // 
            коллекцияToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { menuCollectionAutoGenerate, menuCollectionAdd, menuCollectionEdit, menuCollectionRemove, menuCollectionClear });
            коллекцияToolStripMenuItem.Name = "коллекцияToolStripMenuItem";
            коллекцияToolStripMenuItem.Size = new Size(79, 20);
            коллекцияToolStripMenuItem.Text = "Коллекция";
            // 
            // menuCollectionAutoGenerate
            // 
            menuCollectionAutoGenerate.Name = "menuCollectionAutoGenerate";
            menuCollectionAutoGenerate.Size = new Size(231, 22);
            menuCollectionAutoGenerate.Text = "Авто-генерация...";
            menuCollectionAutoGenerate.Click += menuCollectionAutoGenerate_Click;
            // 
            // menuCollectionAdd
            // 
            menuCollectionAdd.Name = "menuCollectionAdd";
            menuCollectionAdd.Size = new Size(231, 22);
            menuCollectionAdd.Text = "Добавить товар...";
            menuCollectionAdd.Click += menuCollectionAdd_Click;
            // 
            // menuCollectionEdit
            // 
            menuCollectionEdit.Name = "menuCollectionEdit";
            menuCollectionEdit.Size = new Size(231, 22);
            menuCollectionEdit.Text = "Редактировать выбранный...";
            menuCollectionEdit.Click += menuCollectionEdit_Click;
            // 
            // menuCollectionRemove
            // 
            menuCollectionRemove.Name = "menuCollectionRemove";
            menuCollectionRemove.Size = new Size(231, 22);
            menuCollectionRemove.Text = "Удалить выбранный";
            menuCollectionRemove.Click += menuCollectionRemove_Click;
            // 
            // menuCollectionClear
            // 
            menuCollectionClear.Name = "menuCollectionClear";
            menuCollectionClear.Size = new Size(231, 22);
            menuCollectionClear.Text = "Очистить коллекцию";
            menuCollectionClear.Click += menuCollectionClear_Click;
            // 
            // операцииToolStripMenuItem
            // 
            операцииToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { menuOperationsSearch, menuOperationsSort, menuOperationsFilter, menuOperationsLinq });
            операцииToolStripMenuItem.Name = "операцииToolStripMenuItem";
            операцииToolStripMenuItem.Size = new Size(75, 20);
            операцииToolStripMenuItem.Text = "Операции";
            // 
            // menuOperationsSearch
            // 
            menuOperationsSearch.Name = "menuOperationsSearch";
            menuOperationsSearch.Size = new Size(220, 22);
            menuOperationsSearch.Text = "Поиск товара...";
            menuOperationsSearch.Click += menuOperationsSearch_Click;
            // 
            // menuOperationsSort
            // 
            menuOperationsSort.Name = "menuOperationsSort";
            menuOperationsSort.Size = new Size(220, 22);
            menuOperationsSort.Text = "Сортировать коллекцию...";
            menuOperationsSort.Click += menuOperationsSort_Click;
            // 
            // menuOperationsFilter
            // 
            menuOperationsFilter.Name = "menuOperationsFilter";
            menuOperationsFilter.Size = new Size(220, 22);
            menuOperationsFilter.Text = "Фильтровать коллекцию...";
            menuOperationsFilter.Click += menuOperationsFilter_Click;
            // 
            // menuOperationsLinq
            // 
            menuOperationsLinq.Name = "menuOperationsLinq";
            menuOperationsLinq.Size = new Size(220, 22);
            menuOperationsLinq.Text = "Выполнить LINQ-запросы";
            menuOperationsLinq.Click += menuOperationsLinq_Click;
            // 
            // отчетыToolStripMenuItem
            // 
            отчетыToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { menuReportsViewJournal, menuReportsSaveLinq, menuReportsViewLinq });
            отчетыToolStripMenuItem.Name = "отчетыToolStripMenuItem";
            отчетыToolStripMenuItem.Size = new Size(60, 20);
            отчетыToolStripMenuItem.Text = "Отчеты";
            // 
            // menuReportsViewJournal
            // 
            menuReportsViewJournal.Name = "menuReportsViewJournal";
            menuReportsViewJournal.Size = new Size(250, 22);
            menuReportsViewJournal.Text = "Просмотреть журнал операций";
            menuReportsViewJournal.Click += menuReportsViewJournal_Click;
            // 
            // menuReportsSaveLinq
            // 
            menuReportsSaveLinq.Name = "menuReportsSaveLinq";
            menuReportsSaveLinq.Size = new Size(250, 22);
            menuReportsSaveLinq.Text = "Сохранить LINQ-отчет...";
            menuReportsSaveLinq.Click += menuReportsSaveLinq_Click;
            // 
            // menuReportsViewLinq
            // 
            menuReportsViewLinq.Name = "menuReportsViewLinq";
            menuReportsViewLinq.Size = new Size(250, 22);
            menuReportsViewLinq.Text = "Просмотреть LINQ-отчет";
            menuReportsViewLinq.Click += menuReportsViewLinq_Click;
            // 
            // dgvGoods
            // 
            dgvGoods.AllowUserToAddRows = false;
            dgvGoods.AllowUserToDeleteRows = false;
            dgvGoods.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGoods.Dock = DockStyle.Fill;
            dgvGoods.Location = new Point(0, 24);
            dgvGoods.MultiSelect = false;
            dgvGoods.Name = "dgvGoods";
            dgvGoods.ReadOnly = true;
            dgvGoods.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGoods.Size = new Size(800, 426);
            dgvGoods.TabIndex = 1;
            // 
            // StatusStrip
            // 
            StatusStrip.Items.AddRange(new ToolStripItem[] { statusLabelItemCount });
            StatusStrip.Location = new Point(0, 428);
            StatusStrip.Name = "StatusStrip";
            StatusStrip.Size = new Size(800, 22);
            StatusStrip.TabIndex = 2;
            StatusStrip.Text = "statusStrip1";
            // 
            // statusLabelItemCount
            // 
            statusLabelItemCount.Name = "statusLabelItemCount";
            statusLabelItemCount.Size = new Size(79, 17);
            statusLabelItemCount.Text = "Элементов: 0";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(StatusStrip);
            Controls.Add(dgvGoods);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "MainForm";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGoods).EndInit();
            StatusStrip.ResumeLayout(false);
            StatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem menuFileLoad;
        private ToolStripMenuItem menuFileSave;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem menuFileExit;
        private ToolStripMenuItem коллекцияToolStripMenuItem;
        private ToolStripMenuItem menuCollectionAutoGenerate;
        private ToolStripMenuItem menuCollectionAdd;
        private ToolStripMenuItem menuCollectionEdit;
        private ToolStripMenuItem menuCollectionRemove;
        private ToolStripMenuItem menuCollectionClear;
        private ToolStripMenuItem операцииToolStripMenuItem;
        private ToolStripMenuItem menuOperationsSearch;
        private ToolStripMenuItem menuOperationsSort;
        private ToolStripMenuItem menuOperationsFilter;
        private ToolStripMenuItem menuOperationsLinq;
        private ToolStripMenuItem отчетыToolStripMenuItem;
        private ToolStripMenuItem menuReportsViewJournal;
        private ToolStripMenuItem menuReportsSaveLinq;
        private ToolStripMenuItem menuReportsViewLinq;
        private DataGridView dgvGoods;
        private StatusStrip StatusStrip;
        private ToolStripStatusLabel statusLabelItemCount;
    }
}