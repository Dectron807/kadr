using System.Windows;

namespace kadr
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Переход на страницу сотрудников
        private void EmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            EmployeePage employeePage = new EmployeePage();
            employeePage.Show();
            this.Close();
        }

        // Переход на страницу проектов
        private void ProjectButton_Click(object sender, RoutedEventArgs e)
        {
            ProjectPage projectPage = new ProjectPage();
            projectPage.Show();
            this.Close();
        }

        // Переход на страницу отделов
        private void DepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            DepartmentPage departmentPage = new DepartmentPage();
            departmentPage.Show();
            this.Close();
        }

        // Переход на страницу должностей
        private void PositionButton_Click(object sender, RoutedEventArgs e)
        {
            PositionPage positionPage = new PositionPage();
            positionPage.Show();
            this.Close();
        }
    }
}
