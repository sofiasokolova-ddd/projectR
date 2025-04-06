using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private string connectionString = @"Server=MSI;Database=smartmi;Trusted_Connection=True;";


        public Form1()
        {
            InitializeComponent();
            LoadComboBoxes();
            LoadDataGrid();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AllowUserToAddRows = false; // Запрет добавления новых строк пользователем
            dataGridView1.AllowUserToDeleteRows = false; // Запрет удаления строк пользователем
            dataGridView1.ReadOnly = true; // Только для чтения
        }

        private void LoadComboBoxes()
        {
            string categoryQuery = "SELECT cat_name FROM CATEGORY";
            List<string> priceRanges = new List<string> { "Нет требований", "0-100", "101-500", "501-1000", "1000+" };
            string[] stockOptions = { "Нет требований", "Нет", "Мало", "Есть" };

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Load Categories
                    using (SqlCommand command = new SqlCommand(categoryQuery, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        comboBox1.Items.Clear();
                        comboBox1.Items.Add("Нет требований"); // Add "Нет требований"
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader["cat_name"].ToString());
                        }
                    }
                    if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
                }

                // Load Price Ranges
                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(priceRanges.ToArray());
                if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;

                // Load Stock Options
                comboBox3.Items.Clear();
                comboBox3.Items.AddRange(stockOptions);
                if (comboBox3.Items.Count > 0) comboBox3.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке ComboBox-ов: " + ex.Message);
            }
        }

        private void LoadDataGrid()
        {
            string query = @"
                SELECT 
                    c.cat_name AS 'Категория',
                    p.prodname AS 'Название',
                    p.price AS 'Цена',
                    p.stock AS 'Наличие'
                FROM PRODUCTS p
                INNER JOIN CATEGORY c ON p.category_id = c.category_id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных в DataGrid: " + ex.Message);
                }
            }
        }

        private void ApplyFilters()
        {
            string categoryFilter = comboBox1.SelectedItem?.ToString();
            string priceRangeFilter = comboBox2.SelectedItem?.ToString();
            string stockFilter = comboBox3.SelectedItem?.ToString();

            string query = @"
                SELECT 
                    c.cat_name AS 'Категория',
                    p.prodname AS 'Название',
                    p.price AS 'Цена',
                    p.stock AS 'Наличие'
                FROM PRODUCTS p
                INNER JOIN CATEGORY c ON p.category_id = c.category_id
                WHERE 1=1"; // Always true, makes adding AND clauses easier

            // Add category filter
            if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter != "Нет требований")
            {
                query += $" AND c.cat_name = '{categoryFilter}'";
            }

            // Add price range filter
            if (!string.IsNullOrEmpty(priceRangeFilter) && priceRangeFilter != "Нет требований")
            {
                switch (priceRangeFilter)
                {
                    case "0-100":
                        query += " AND p.price BETWEEN 0 AND 100";
                        break;
                    case "101-500":
                        query += " AND p.price BETWEEN 101 AND 500";
                        break;
                    case "501-1000":
                        query += " AND p.price BETWEEN 501 AND 1000";
                        break;
                    case "1000+":
                        query += " AND p.price >= 1000";
                        break;
                }
            }

            // Add stock filter
            if (!string.IsNullOrEmpty(stockFilter) && stockFilter != "Нет требований")
            {
                switch (stockFilter)
                {
                    case "Нет":
                        query += " AND p.stock = 0";
                        break;
                    case "Мало":
                        query += " AND p.stock > 0 AND p.stock <= 10";
                        break;
                    case "Есть":
                        query += " AND p.stock > 10";
                        break;
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при применении фильтров: " + ex.Message);
                }
            }
        }




        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, что клик был по допустимой строке
            if (e.RowIndex >= 0)
            {
                // Получаем название продукта из выбранной строки
                string productName = dataGridView1.Rows[e.RowIndex].Cells["Название"].Value.ToString();

                // Создаем экземпляр Form3, передавая название продукта
                Form3 form3 = new Form3(productName);
                form3.Show();
            }
        }



        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Здесь можно обработать выбор категории, если нужно
            //MessageBox.Show($"Вы выбрали категорию: {comboBox1.SelectedItem}");
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(); // Создаем экземпляр второй формы
            form2.Show(); // Отображаем вторую форму
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Проверяем, аутентифицирован ли пользователь
            if (((Form4)Application.OpenForms["Form4"])?.UserId > 0)
            {
                // Пользователь аутентифицирован, выполняем действие
                // ...
            }
            else
            {
                // Пользователь не аутентифицирован, выводим сообщение
                MessageBox.Show("Пожалуйста, зайдите в личный кабинет, прежде чем формировать заказ.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Проверяем, аутентифицирован ли пользователь
            if (((Form4)Application.OpenForms["Form4"])?.UserId > 0)
            {
                // Пользователь аутентифицирован, выполняем действие
                // ...
            }
            else
            {
                // Пользователь не аутентифицирован, выводим сообщение
                MessageBox.Show("Пожалуйста, зайдите в личный кабинет, прежде чем формировать заказ.");
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.ShowDialog(); // Используем ShowDialog, чтобы форма была модальной
        }


        private void button4_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрана ли строка в dataGridView1
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем название продукта из выбранной строки
                string productName = dataGridView1.SelectedRows[0].Cells["Название"].Value.ToString();

                // Создаем экземпляр Form3, передавая название продукта
                Form3 form3 = new Form3(productName);
                form3.Show();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт в таблице.");
            }
        }

    }
}
