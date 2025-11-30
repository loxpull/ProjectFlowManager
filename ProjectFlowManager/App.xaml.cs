using ProjectFlowManager.Data;
using ProjectFlowManager.Repositories;
using ProjectFlowManager.Services;
using ProjectFlowManager.ViewModels;
using System.Windows;

namespace ProjectFlowManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем контекст базы данных
            var dbContext = new AppDbContext();

            // Инициализируем базу данных
            DbInitializer.Initialize(dbContext);

            // Создаем зависимости
            var projectRepo = new ProjectRepository(dbContext);
            var progressService = new ProgressService();
            var mainVm = new MainViewModel(projectRepo, progressService);

            // Создаем и показываем главное окно
            var mainWindow = new MainWindow { DataContext = mainVm };
            mainWindow.Show();
        }
    }
}