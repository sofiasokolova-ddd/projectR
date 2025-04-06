using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        private string connectionString = @"Server=MSI;Database=smartmi;Trusted_Connection=True;"; // Замените строку подключения на вашу
        private string productName;

        public Form3(string productName)
        {
            InitializeComponent();
            this.productName = productName;
            LoadProductDetails();
        }

        private void LoadProductDetails()
        {
            string query = @"
                SELECT 
                    p.prodname AS 'Название',
                    p.descrint AS 'Описание',
                    p.price AS 'Цена',
                    p.stock AS 'Наличие',
                    c.cat_name AS 'Категория'
                FROM PRODUCTS p
                INNER JOIN CATEGORY c ON p.category_id = c.category_id
                WHERE p.prodname = @productName";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@productName", productName);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    // Проверяем, что данные были найдены
                    if (table.Rows.Count > 0)
                    {
                        // Отображаем данные в элементах управления Form3
                        // Например, если у вас есть TextBox-ы для отображения данных:
                        textBoxProductName.Text = table.Rows[0]["Название"].ToString();
                        richTextBoxDescription.Text = table.Rows[0]["Описание"].ToString();
                        textBoxPrice.Text = table.Rows[0]["Цена"].ToString();
                        textBoxStock.Text = table.Rows[0]["Наличие"].ToString();
                        textBoxCategory.Text = table.Rows[0]["Категория"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Информация о продукте не найдена.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных о продукте: " + ex.Message);
            }
        }
    }
}
