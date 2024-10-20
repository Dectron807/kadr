using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace kadr
{
    public partial class PositionPage : Window
    {
        private string connectionString = "Data Source=DESKTOP-0JE8UH5\\SQLEXPRESS;Initial Catalog=kadr;Integrated Security=True";
        private int selectedPositionId = -1;

        public PositionPage()
        {
            InitializeComponent();
            LoadPositions(); // Загрузка должностей при открытии страницы
        }

        private void LoadPositions()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Position", connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                PositionDataGrid.ItemsSource = dt.DefaultView; // Заполняем DataGrid данными
            }
        }

        private void AddPositionButton_Click(object sender, RoutedEventArgs e)
        {
            string positionName = txtPositionName.Text;
            decimal salary;

            if (string.IsNullOrEmpty(positionName) || !decimal.TryParse(txtSalary.Text, out salary))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            int newPositionId = GetNextPositionId(); // Получаем новый ID

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Position (PositionID, PositionName, Salary) VALUES (@PositionID, @PositionName, @Salary)", connection);
                cmd.Parameters.AddWithValue("@PositionID", newPositionId);
                cmd.Parameters.AddWithValue("@PositionName", positionName);
                cmd.Parameters.AddWithValue("@Salary", salary);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Должность добавлена успешно.");
                    LoadPositions(); // Обновляем список после добавления
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении должности.");
                }
            }
        }

        private int GetNextPositionId()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(PositionID), 0) + 1 FROM Position", connection);
                return (int)cmd.ExecuteScalar(); // Возвращаем максимальный ID, увеличенный на 1
            }
        }

        private void EditPositionButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPositionId == -1)
            {
                MessageBox.Show("Пожалуйста, выберите должность для редактирования.");
                return;
            }

            string positionName = txtPositionName.Text;
            decimal salary;

            if (string.IsNullOrEmpty(positionName) || !decimal.TryParse(txtSalary.Text, out salary))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Position SET PositionName = @PositionName, Salary = @Salary WHERE PositionID = @PositionID", connection);
                cmd.Parameters.AddWithValue("@PositionName", positionName);
                cmd.Parameters.AddWithValue("@Salary", salary);
                cmd.Parameters.AddWithValue("@PositionID", selectedPositionId);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Должность успешно обновлена.");
                    LoadPositions(); // Обновляем список после редактирования
                    ClearFields();
                    selectedPositionId = -1;
                }
                else
                {
                    MessageBox.Show("Ошибка при редактировании должности.");
                }
            }
        }

        private void DeletePositionButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPositionId == -1)
            {
                MessageBox.Show("Пожалуйста, выберите должность для удаления.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Position WHERE PositionID = @PositionID", connection);
                cmd.Parameters.AddWithValue("@PositionID", selectedPositionId);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Должность успешно удалена.");
                    LoadPositions(); // Обновляем список после удаления
                    ClearFields();
                    selectedPositionId = -1;
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении должности.");
                }
            }
        }

        private void PositionDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PositionDataGrid.SelectedItem is DataRowView row)
            {
                selectedPositionId = Convert.ToInt32(row["PositionID"]);
                txtPositionName.Text = row["PositionName"].ToString();
                txtSalary.Text = row["Salary"].ToString();
            }
        }

        private void ClearFields()
        {
            txtPositionName.Text = string.Empty;
            txtSalary.Text = string.Empty;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
