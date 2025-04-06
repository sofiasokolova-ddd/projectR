using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form5 : Form
    {
        private string ConnectionString = "Server=MSI;Database=smartmi;Trusted_Connection=True;Encrypt=False;";

        public Form5()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"
                SELECT 
                    p.product_id, 
                    p.prodname, 
                    c.cat_name, 
                    p.descrint, 
                    p.price, 
                    p.stock 
                FROM 
                    PRODUCTS p
                INNER JOIN 
                    CATEGORY c 
                ON 
                    p.category_id = c.category_id";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Óñòàíàâëèâàåì èñòî÷íèê äàííûõ äëÿ DataGridView
                    dataGridView1.DataSource = dataTable;

                    // Ïåðåóïîðÿäî÷èâàåì ñòîëáöû
                    if (dataGridView1.Columns.Count > 0)
                    {
                        dataGridView1.DataSource = dataTable;
                        dataGridView1.Columns[0].Name = "Id";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Îøèáêà ïðè çàãðóçêå äàííûõ: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // 1. Äîáàâëÿåì òîâàð â òàáëèöó PRODUCTS
                    string query1 = "INSERT INTO PRODUCTS (prodname, descrint, price, stock) VALUES (@Name, @Description, @Price, @Stock)";
                    SqlCommand command1 = new SqlCommand(query1, connection);
                    command1.Parameters.AddWithValue("@Name", txtName.Text);
                    command1.Parameters.AddWithValue("@Description", txtDescription.Text);
                    command1.Parameters.AddWithValue("@Price", decimal.Parse(txtPrice.Text));
                    command1.Parameters.AddWithValue("@Stock", int.Parse(txtStock.Text));
                    command1.ExecuteNonQuery();

                    // 2. Äîáàâëÿåì êàòåãîðèþ â òàáëèöó CATEGORY
                    string query2 = "INSERT INTO CATEGORY (cat_name) VALUES (@CatName)";
                    SqlCommand command2 = new SqlCommand(query2, connection); // Èñïîëüçóåì íîâóþ êîìàíäó
                    command2.Parameters.AddWithValue("@CatName", textBox1.Text); // Óáåäèòåñü, ÷òî òåêñòîâîå ïîëå íå ïóñòîå
                    command2.ExecuteNonQuery();
                }

                MessageBox.Show("Òîâàð è êàòåãîðèÿ óñïåøíî äîáàâëåíû!");
                LoadProducts(); // Îáíîâëåíèå äàííûõ â DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show("Îøèáêà ïðè äîáàâëåíèè òîâàðà èëè êàòåãîðèè: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Âûáåðèòå òîâàð äëÿ óäàëåíèÿ.");
                return;
            }

            int productId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM PRODUCTS WHERE product_id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", productId);
                    command.ExecuteNonQuery();
                }
                MessageBox.Show("Òîâàð óñïåøíî óäàëåí!");
                LoadProducts(); // Îáíîâëåíèå äàííûõ â DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show("Îøèáêà ïðè óäàëåíèè òîâàðà: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
