using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form4 : Form
    {
        private string connectionString = @"Server=MSI;Database=smartmi;Trusted_Connection=True;"; // Замените строку подключения на вашу
        public int UserId { get; private set; } = 0; // ID пользователя, 0 если не аутентифицирован
        public string Username { get; private set; } = ""; // Имя пользователя
        public string Email { get; private set; } = ""; // Email пользователя

        public Form4()
        {
            InitializeComponent();
            labelUsername.Visible = false; // Скрываем метку имени пользователя при загрузке формы
            labelEmail.Visible = false; // Скрываем метку email при загрузке формы
        }

        private void labelEmail_Click(object sender, EventArgs e)
        {

        }

        private void buttonLogin_Click_1(object sender, EventArgs e)
        {
            string email = textBoxLogin.Text; // Используем поле для ввода email
            string password = textBoxPassword.Text;

            string query = "SELECT user_id, username, email FROM USERS WHERE email = @email AND userpass = @password";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", password);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Аутентификация успешна
                            UserId = Convert.ToInt32(reader["user_id"]);
                            Username = reader["username"].ToString();
                            Email = reader["email"].ToString();

                            // Показываем MessageBox с успешным входом
                            MessageBox.Show($"Успешный вход!\nИмя пользователя: {Username}\nEmail: {Email}", "Вход выполнен");

                            // Отображаем информацию о пользователе на форме
                            labelUsername.Text = $"Имя пользователя: {Username}";
                            labelEmail.Text = $"Email: {Email}";

                            labelUsername.Visible = true;
                            labelEmail.Visible = true;
                        }
                        else
                        {
                            // Неверные учетные данные
                            MessageBox.Show("Неверный email или пароль.", "Ошибка входа");
                            UserId = 0;
                            Username = "";
                            Email = "";

                            labelUsername.Visible = false;
                            labelEmail.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при входе: " + ex.Message, "Ошибка");
            }
        }
    }
}
