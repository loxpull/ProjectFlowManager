using ProjectFlowManager.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ProjectFlowManager.ViewModels
{
    public class ProjectViewModel : BaseViewModel
    {
        private readonly ProjectFlowManagerEntities _context;
        private Projects _project;
        private bool _isEditMode;

        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public int? ProjectProgress { get; set; }
        public string TasksSummary { get; set; }

        public List<string> Categories { get; } = new List<string>
        {
            "Работа", "Личное", "Хобби", "Обучение", "Другое"
        };

        public List<string> Priorities { get; } = new List<string>
        {
            "Низкий", "Средний", "Высокий", "Критический"
        };

        public List<string> ProjectStatuses { get; } = new List<string>
        {
            "Черновик", "Активный", "На паузе", "Завершен"
        };

        public string SelectedCategory { get; set; }
        public string SelectedPriority { get; set; }
        public string SelectedStatus { get; set; }

        public ObservableCollection<Tasks> Tasks { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddTaskToProjectCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand UpdateTaskStatusesCommand { get; }

        public ProjectViewModel() : this(null) { }

        public ProjectViewModel(Projects project)
        {
            _context = new ProjectFlowManagerEntities();
            Tasks = new ObservableCollection<Tasks>();

            if (project != null)
            {
                _project = project;
                _isEditMode = true;
                LoadProjectData();
            }
            else
            {
                _project = new Projects
                {
                    CreatedDate = DateTime.Now,
                    Progress = 0
                };
                SelectedCategory = Categories[0];
                SelectedPriority = Priorities[1];
                SelectedStatus = ProjectStatuses[0];
            }

            SaveCommand = new RelayCommand(SaveProject);
            CancelCommand = new RelayCommand(Cancel);
            AddTaskToProjectCommand = new RelayCommand(AddTask);
            EditTaskCommand = new RelayCommand<Tasks>(EditTask);
            DeleteTaskCommand = new RelayCommand<Tasks>(DeleteTask);
            UpdateTaskStatusesCommand = new RelayCommand(UpdateTaskStatuses);
        }

        private void LoadProjectData()
        {
            ProjectName = _project.Name;
            ProjectDescription = _project.Description;

            SelectedCategory = string.IsNullOrEmpty(_project.Category) ? Categories[0] : _project.Category;
            SelectedPriority = GetPriorityName(_project.Priority);
            SelectedStatus = GetStatusName(_project.Status);

            
            var tasks = _context.Tasks
                .Where(t => t.ProjectId == _project.Id)
                .ToList();

            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }

            CalculateProgress();
        }

        private string GetPriorityName(int? priority)
        {
            if (!priority.HasValue) return Priorities[1];
            return priority >= 1 && priority <= Priorities.Count ?
                Priorities[priority.Value - 1] : Priorities[1];
        }

        private string GetStatusName(int? status)
        {
            if (!status.HasValue) return ProjectStatuses[0];
            return status >= 1 && status <= ProjectStatuses.Count ?
                ProjectStatuses[status.Value - 1] : ProjectStatuses[0];
        }

        private void CalculateProgress()
        {
            if (Tasks.Any())
            {
                int completedTasks = Tasks.Count(t => t.Status == 4);
                ProjectProgress = (int?)((double)completedTasks / Tasks.Count * 100);
                TasksSummary = $"Задач: {Tasks.Count}, Завершено: {completedTasks}";
            }
            else
            {
                ProjectProgress = 0;
                TasksSummary = "Задач нет";
            }

            OnPropertyChanged(nameof(ProjectProgress));
            OnPropertyChanged(nameof(TasksSummary));
        }

        private void SaveProject()
        {
            if (string.IsNullOrWhiteSpace(ProjectName))
            {
                MessageBox.Show("Название проекта обязательно", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _project.Name = ProjectName;
            _project.Description = ProjectDescription;
            _project.Category = SelectedCategory;
            _project.Priority = Priorities.IndexOf(SelectedPriority) + 1;
            _project.Status = ProjectStatuses.IndexOf(SelectedStatus) + 1;
            _project.Progress = ProjectProgress;

            try
            {
                if (!_isEditMode)
                {
                    _context.Projects.Add(_project);
                }

                _context.SaveChanges();
                CloseWindow(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения проекта: {ex.Message}");
            }
        }

        private void AddTask()
        {
            try
            {
                var taskWindow = new Views.AddEditTaskWindow(_project.Id);
                if (taskWindow.ShowDialog() == true)
                {
                    RefreshTasks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления задачи: {ex.Message}");
            }
        }

        private void EditTask(Tasks task)
        {
            try
            {
                if (task == null) return;

                var taskWindow = new Views.AddEditTaskWindow(task);
                if (taskWindow.ShowDialog() == true)
                {
                    RefreshTasks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка редактирования задачи: {ex.Message}");
            }
        }

        private void DeleteTask(Tasks task)
        {
            try
            {
                if (task == null) return;

                var result = MessageBox.Show($"Удалить задачу '{task.Name}'?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Tasks.Remove(task);
                    _context.SaveChanges();
                    Tasks.Remove(task);
                    CalculateProgress();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления задачи: {ex.Message}");
            }
        }

        private void UpdateTaskStatuses()
        {
            try
            {
                _context.SaveChanges();
                CalculateProgress();
                MessageBox.Show("Статусы задач обновлены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления статусов: {ex.Message}");
            }
        }

        private void RefreshTasks()
        {
            try
            {
                Tasks.Clear();
                var tasks = _context.Tasks
                    .Where(t => t.ProjectId == _project.Id)
                    .ToList();

                foreach (var task in tasks)
                {
                    Tasks.Add(task);
                }
                CalculateProgress();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления списка задач: {ex.Message}");
            }
        }

        private void Cancel()
        {
            CloseWindow(false);
        }

        private void CloseWindow(bool dialogResult)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = dialogResult;
                    window.Close();
                    break;
                }
            }
        }
    }
}