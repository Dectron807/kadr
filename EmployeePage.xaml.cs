using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace kadr
{
    public partial class EmployeePage : Window
    {
        private string connectionString = "Data Source=DESKTOP-0JE8UH5\\SQLEXPRESS;Initial Catalog=kadr;Integrated Security=True"; // Укажите строку подключения
        private int selectedEmployeeId = -1;

        public EmployeePage()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Employee", connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                EmployeeDataGrid.ItemsSource = dt.DefaultView; // Заполняем DataGrid данными
            }
        }

        private void ClearFields()
        {
            txtLastName.Clear();
            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            datePickerBirthDate.SelectedDate = null;
            datePickerHireDate.SelectedDate = null;
            txtPositionID.Clear();
            txtDepartmentID.Clear();
            selectedEmployeeId = -1; // Сбрасываем выбранный ID
        }

        private int GetNextEmployeeId()
        {
            int nextId = 1; // Значение по умолчанию, если в таблице нет записей
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT MAX(EmployeeID) FROM Employee", connection);
                object result = cmd.ExecuteScalar();

                if (result != DBNull.Value)
                {
                    nextId = Convert.ToInt32(result) + 1; // Увеличиваем максимальный ID на 1
                }
            }
            return nextId;
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            string lastName = txtLastName.Text;
            string firstName = txtFirstName.Text;
            string middleName = txtMiddleName.Text;
            string phone = txtPhone.Text;
            string email = txtEmail.Text;
            DateTime birthDate = datePickerBirthDate.SelectedDate ?? DateTime.MinValue;
            DateTime hireDate = datePickerHireDate.SelectedDate ?? DateTime.MinValue;
            int positionID;
            int departmentID;

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName) ||
                !int.TryParse(txtPositionID.Text, out positionID) || !int.TryParse(txtDepartmentID.Text, out departmentID))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            int newEmployeeId = GetNextEmployeeId(); // Получаем новый ID

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Employee (EmployeeID, LastName, FirstName, MiddleName, BirthDate, Phone, Email, HireDate, PositionID, DepartmentID) VALUES (@EmployeeID, @LastName, @FirstName, @MiddleName, @BirthDate, @Phone, @Email, @HireDate, @PositionID, @DepartmentID)", connection);
                cmd.Parameters.AddWithValue("@EmployeeID", newEmployeeId);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@MiddleName", middleName);
                cmd.Parameters.AddWithValue("@BirthDate", birthDate);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@HireDate", hireDate);
                cmd.Parameters.AddWithValue("@PositionID", positionID);
                cmd.Parameters.AddWithValue("@DepartmentID", departmentID);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Сотрудник добавлен успешно.");
                    LoadEmployees(); // Обновляем список после добавления
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении сотрудника.");
                }
            }
        }

        private void EditEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEmployeeId == -1)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника для редактирования.");
                return;
            }

            string lastName = txtLastName.Text;
            string firstName = txtFirstName.Text;
            string middleName = txtMiddleName.Text;
            string phone = txtPhone.Text;
            string email = txtEmail.Text;
            DateTime birthDate = datePickerBirthDate.SelectedDate ?? DateTime.MinValue;
            DateTime hireDate = datePickerHireDate.SelectedDate ?? DateTime.MinValue;
            int positionID;
            int departmentID;

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName) ||
                !int.TryParse(txtPositionID.Text, out positionID) || !int.TryParse(txtDepartmentID.Text, out departmentID))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Employee SET LastName = @LastName, FirstName = @FirstName, MiddleName = @MiddleName, BirthDate = @BirthDate, Phone = @Phone, Email = @Email, HireDate = @HireDate, PositionID = @PositionID, DepartmentID = @DepartmentID WHERE EmployeeID = @EmployeeID", connection);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@MiddleName", middleName);
                cmd.Parameters.AddWithValue("@BirthDate", birthDate);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@HireDate", hireDate);
                cmd.Parameters.AddWithValue("@PositionID", positionID);
                cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                cmd.Parameters.AddWithValue("@EmployeeID", selectedEmployeeId);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Сотрудник обновлен успешно.");
                    LoadEmployees(); // Обновляем список после редактирования
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении сотрудника.");
                }
            }
        }

        private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEmployeeId == -1)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника для удаления.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Employee WHERE EmployeeID = @EmployeeID", connection);
                cmd.Parameters.AddWithValue("@EmployeeID", selectedEmployeeId);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Сотрудник удален успешно.");
                    LoadEmployees(); // Обновляем список после удаления
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении сотрудника.");
                }
            }
        }

        private void EmployeeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is DataRowView row)
            {
                selectedEmployeeId = Convert.ToInt32(row["EmployeeID"]);
                txtLastName.Text = row["LastName"].ToString();
                txtFirstName.Text = row["FirstName"].ToString();
                txtMiddleName.Text = row["MiddleName"].ToString();
                txtPhone.Text = row["Phone"].ToString();
                txtEmail.Text = row["Email"].ToString();
                datePickerBirthDate.SelectedDate = Convert.ToDateTime(row["BirthDate"]);
                datePickerHireDate.SelectedDate = Convert.ToDateTime(row["HireDate"]);
                txtPositionID.Text = row["PositionID"].ToString();
                txtDepartmentID.Text = row["DepartmentID"].ToString();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
