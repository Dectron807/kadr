using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace kadr
{
    public partial class ProjectPage : Window
    {
        private string connectionString = "Data Source=DESKTOP-0JE8UH5\\SQLEXPRESS;Initial Catalog=kadr;Integrated Security=True"; // Строка подключения к базе данных
        private int selectedProjectId = -1; // Для хранения ID выбранного проекта

        public ProjectPage()
        {
            InitializeComponent();
            LoadProjects(); // Загрузка проектов при открытии страницы
        }

        // Метод для загрузки списка проектов
        private void LoadProjects()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Project", connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                ProjectDataGrid.ItemsSource = dt.DefaultView; // Заполняем DataGrid данными
            }
        }

        // Добавление нового проекта
        // Добавление нового проекта с ручным установлением ID
        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            string projectName = txtProjectName.Text;
            string projectDescription = txtProjectDescription.Text;

            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(projectDescription))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            int newProjectId;
            // Получите максимальный ID и увеличьте его на 1
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Получаем максимальный ProjectID
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(ProjectID), 0) + 1 FROM Project", connection);
                newProjectId = (int)cmd.ExecuteScalar();

                // Добавляем новый проект
                SqlCommand insertCmd = new SqlCommand("INSERT INTO Project (ProjectID, ProjectName, Description) VALUES (@ProjectID, @ProjectName, @Description)", connection);
                insertCmd.Parameters.AddWithValue("@ProjectID", newProjectId);
                insertCmd.Parameters.AddWithValue("@ProjectName", projectName);
                insertCmd.Parameters.AddWithValue("@Description", projectDescription);

                int result = insertCmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Проект добавлен успешно.");
                    LoadProjects(); // Обновляем список после добавления
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении проекта.");
                }
            }
        }


        // Редактирование выбранного проекта
        private void EditProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProjectId == -1)
            {
                MessageBox.Show("Пожалуйста, выберите проект для редактирования.");
                return;
            }

            string projectName = txtProjectName.Text;
            string projectDescription = txtProjectDescription.Text;

            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(projectDescription))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Project SET ProjectName = @ProjectName, Description = @Description WHERE ProjectID = @ProjectID", connection);
                cmd.Parameters.AddWithValue("@ProjectName", projectName);
                cmd.Parameters.AddWithValue("@Description", projectDescription);
                cmd.Parameters.AddWithValue("@ProjectID", selectedProjectId);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Проект успешно обновлен.");
                    LoadProjects(); // Обновляем список после редактирования
                    ClearFields();
                    selectedProjectId = -1;
                }
                else
                {
                    MessageBox.Show("Ошибка при редактировании проекта.");
                }
            }
        }

        // Удаление выбранного проекта
        private void DeleteProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProjectId == -1)
            {
                MessageBox.Show("Пожалуйста, выберите проект для удаления.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Project WHERE ProjectID = @ProjectID", connection);
                cmd.Parameters.AddWithValue("@ProjectID", selectedProjectId);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Проект успешно удален.");
                    LoadProjects(); // Обновляем список после удаления
                    ClearFields();
                    selectedProjectId = -1;
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении проекта.");
                }
            }
        }

        // Обработка выбора проекта в таблице (DataGrid)
        private void ProjectDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProjectDataGrid.SelectedItem is DataRowView row)
            {
                selectedProjectId = Convert.ToInt32(row["ProjectID"]);
                txtProjectName.Text = row["ProjectName"].ToString();
                txtProjectDescription.Text = row["Description"].ToString();
            }
        }

        // Метод для очистки полей ввода
        private void ClearFields()
        {
            txtProjectName.Text = string.Empty;
            txtProjectDescription.Text = string.Empty;
        }

        // Переход на главное окно
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
