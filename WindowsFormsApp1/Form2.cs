using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private string connectionString = @"Server=MSI;Database=smartmi;Trusted_Connection=True;"; // Замените строку подключения на вашу

        public Form2()
        {
            InitializeComponent();
            LoadOrderData();
        }

        private void LoadOrderData()
        {
            string query = @"
                SELECT order_id, ord_date, total, status, username from [order], users where [order].user_id=users.user_id";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // Открываем соединение
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Обработка кликов по ячейкам, если необходимо
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите строку.");
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Получаем order_id из выбранной строки
                    int order_id = int.Parse(dataGridView1.SelectedRows[0].Cells["order_id"].Value.ToString());
                    string query = @"
                SELECT ord_date, status, total, prodname, kolvo 
                FROM order_items 
                JOIN [order] ON order_items.order_id = [order].order_id 
                JOIN products ON order_items.product_id = products.product_id 
                WHERE [order].order_id = @order_id";

                    connection.Open(); // Открываем соединение

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@order_id", order_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Создаем новый текстовый файл
                            using (StreamWriter writer = new StreamWriter("OrderDetails.txt"))
                            {
                                if (reader.HasRows) // Проверяем, есть ли строки
                                {
                                    // Размещаем данные из первой строки
                                    reader.Read(); // Читаем первую строку
                                    writer.WriteLine($"Отчет по заказу №{order_id}");
                                    writer.WriteLine($"Дата заказа: {reader["ord_date"]}");
                                    writer.WriteLine($"Статус заказа: {reader["status"]}");
                                    writer.WriteLine($"Состав заказа");

                                    // Переменная для хранения суммы заказа
                                    var total = reader["total"];

                                    // Записываем информацию о каждом продукте начиная со второй строки
                                    do
                                    {
                                        if (reader["prodname"] != DBNull.Value && reader["kolvo"] != DBNull.Value)
                                        {
                                            writer.WriteLine($"      - {reader["prodname"]} x{reader["kolvo"]}");
                                        }
                                    }
                                    while (reader.Read()); // Читаем все оставшиеся строки

                                    // Записываем сумму заказа после всех продуктов
                                    writer.WriteLine($"Сумма заказа: {total}");

                                    MessageBox.Show("Данные успешно сохранены в файл OrderDetails.txt.");
                                }
                                else
                                {
                                    MessageBox.Show("Заказ не найден.");
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5(); // Создаем экземпляр второй формы
            form5.Show();
        }
    }
}
