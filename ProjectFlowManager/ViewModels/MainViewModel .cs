using ProjectFlowManager.Data;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ProjectFlowManager.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ProjectFlowManagerEntities _context;

        public ObservableCollection<Projects> Projects { get; set; }
        public ObservableCollection<Tasks> AllTasks { get; set; }

        private int _activeProjectsCount;
        public int ActiveProjectsCount
        {
            get => _activeProjectsCount;
            set => SetProperty(ref _activeProjectsCount, value);
        }

        private int _overdueTasksCount;
        public int OverdueTasksCount
        {
            get => _overdueTasksCount;
            set => SetProperty(ref _overdueTasksCount, value);
        }

        public ICommand LoadDataCommand { get; }
        public ICommand AddProjectCommand { get; }
        public ICommand EditProjectCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand AddTaskCommand { get; }

        public MainViewModel()
        {
            _context = new ProjectFlowManagerEntities();
            Projects = new ObservableCollection<Projects>();
            AllTasks = new ObservableCollection<Tasks>();

            LoadDataCommand = new RelayCommand(LoadData);
            AddProjectCommand = new RelayCommand(AddProject);
            EditProjectCommand = new RelayCommand<Projects>(EditProject);
            DeleteProjectCommand = new RelayCommand<Projects>(DeleteProject);
            AddTaskCommand = new RelayCommand(AddTask);

            LoadData();
        }

        internal void LoadData()
        {
            try
            {
                Projects.Clear();
                AllTasks.Clear();

                var projects = _context.Projects
                    .Include(p => p.Tasks)
                    .ToList();

                foreach (var project in projects)
                {
                    Projects.Add(project);
                    foreach (var task in project.Tasks)
                    {
                        AllTasks.Add(task);
                    }
                }

                CalculateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        internal void CalculateStatistics()
        {
            try
            {
                ActiveProjectsCount = Projects.Count(p => p.Status == 2);
                OverdueTasksCount = AllTasks.Count(t =>
                    t.TaskDeadline.HasValue &&
                    t.TaskDeadline.Value < DateTime.Now &&
                    t.Status != 4);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расчета статистики: {ex.Message}");
            }
        }

        private void AddProject()
        {
            try
            {
                var projectWindow = new Views.ProjectWindow();
                if (projectWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия окна проекта: {ex.Message}");
            }
        }

        internal void EditProject(Projects project)
        {
            try
            {
                if (project == null) return;

                var projectWindow = new Views.ProjectWindow(project);
                if (projectWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка редактирования проекта: {ex.Message}");
            }
        }

        private void DeleteProject(Projects project)
        {
            try
            {
                if (project == null) return;

                var result = MessageBox.Show(
                    $"Удалить проект '{project.Name}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Projects.Remove(project);
                    _context.SaveChanges();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления проекта: {ex.Message}");
            }
        }

        private void AddTask()
        {
            try
            {
                var taskWindow = new Views.AddEditTaskWindow();
                if (taskWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия окна задачи: {ex.Message}");
            }
        }
    }
}