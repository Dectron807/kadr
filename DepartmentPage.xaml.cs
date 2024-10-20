using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace kadr
{
    public partial class DepartmentPage : Window
    {
        private string connectionString = "Data Source=DESKTOP-0JE8UH5\\SQLEXPRESS;Initial Catalog=kadr;Integrated Security=True";
        private int selectedDepartmentId = -1;

        public DepartmentPage()
        {
            InitializeComponent();
            LoadDepartments(); // Загрузка отделов при открытии страницы
        }

        private void LoadDepartments()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Department", connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                DepartmentDataGrid.ItemsSource = dt.DefaultView; // Заполняем DataGrid данными
            }
        }

        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            string departmentName = txtDepartmentName.Text;
            int managerId;

            if (string.IsNullOrEmpty(departmentName) || !int.TryParse(txtManagerID.Text, out managerId))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            int newDepartmentId = GetNextDepartmentId(); // Получаем новый ID

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Department (DepartmentID, DepartmentName, ManagerID) VALUES (@DepartmentID, @DepartmentName, @ManagerID)", connection);
                cmd.Parameters.AddWithValue("@DepartmentID", newDepartmentId);
                cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
                cmd.Parameters.AddWithValue("@ManagerID", managerId);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Отдел добавлен успешно.");
                    LoadDepartments(); // Обновляем список после добавления
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении отдела.");
                }
            }
        }

        private int GetNextDepartmentId()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(DepartmentID), 0) + 1 FROM Department", connection);
                return (int)cmd.ExecuteScalar(); // Возвращаем максимальный ID, увеличенный на 1
            }
        }


        private void EditDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDepartmentId == -1)
            {
                MessageBox.Show("Пожалуйста, выберите отдел для редактирования.");
                return;
            }

            string departmentName = txtDepartmentName.Text;
            int managerId;

            if (string.IsNullOrEmpty(departmentName) || !int.TryParse(txtManagerID.Text, out managerId))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Department SET DepartmentName = @DepartmentName, ManagerID = @ManagerID WHERE DepartmentID = @DepartmentID", connection);
                cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
                cmd.Parameters.AddWithValue("@ManagerID", managerId);
                cmd.Parameters.AddWithValue("@DepartmentID", selectedDepartmentId);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Отдел успешно обновлен.");
                    LoadDepartments(); // Обновляем список после редактирования
                    ClearFields();
                    selectedDepartmentId = -1;
                }
                else
                {
                    MessageBox.Show("Ошибка при редактировании отдела.");
                }
            }
        }

        private void DeleteDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDepartmentId == -1)
            {
                MessageBox.Show("Пожалуйста, выберите отдел для удаления.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Department WHERE DepartmentID = @DepartmentID", connection);
                cmd.Parameters.AddWithValue("@DepartmentID", selectedDepartmentId);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Отдел успешно удален.");
                    LoadDepartments(); // Обновляем список после удаления
                    ClearFields();
                    selectedDepartmentId = -1;
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении отдела.");
                }
            }
        }

        private void DepartmentDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DepartmentDataGrid.SelectedItem is DataRowView row)
            {
                selectedDepartmentId = Convert.ToInt32(row["DepartmentID"]);
                txtDepartmentName.Text = row["DepartmentName"].ToString();
                txtManagerID.Text = row["ManagerID"].ToString();
            }
        }

        private void ClearFields()
        {
            txtDepartmentName.Text = string.Empty;
            txtManagerID.Text = string.Empty;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
