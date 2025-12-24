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
    public class TaskViewModel : BaseViewModel
    {
        private readonly ProjectFlowManagerEntities _context;
        private Tasks _task;
        private bool _isEditMode;
        private int? _projectId;

        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public DateTime? Deadline { get; set; }
        public bool NoDeadline { get; set; } = true;

        public List<string> TaskStatuses { get; } = new List<string>
        {
            "К выполнению", "В работе", "На проверке", "Завершена"
        };

        public List<int> Difficulties { get; } = new List<int> { 1, 2, 3, 4, 5 };

        public string SelectedStatus { get; set; }
        public int? SelectedDifficulty { get; set; } = 3;

        public ObservableCollection<Projects> Projects { get; set; }
        public Projects SelectedProject { get; set; }
        public bool IsProjectSelectable { get; set; } = true;

        public string ValidationMessage { get; set; }
        public bool HasValidationError => !string.IsNullOrEmpty(ValidationMessage);

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        
        public TaskViewModel()
        {
            _context = new ProjectFlowManagerEntities();
            Projects = new ObservableCollection<Projects>();
            _task = new Tasks { CreatedDate = DateTime.Now };
            SelectedStatus = TaskStatuses[0];
            SelectedDifficulty = 3;

            LoadProjects();

            SaveCommand = new RelayCommand(SaveTask, CanSaveTask);
            CancelCommand = new RelayCommand(Cancel);
        }

        
        public TaskViewModel(int? projectId) : this()
        {
            _projectId = projectId;

            if (projectId.HasValue)
            {
                SelectedProject = Projects.FirstOrDefault(p => p.Id == projectId.Value);
                IsProjectSelectable = false;
            }
        }

        
        public TaskViewModel(Tasks task) : this()
        {
            if (task != null)
            {
                _task = task;
                _isEditMode = true;
                LoadTaskData();
            }
        }

        private void LoadProjects()
        {
            try
            {
                var projects = _context.Projects.ToList();
                foreach (var project in projects)
                {
                    Projects.Add(project);
                }

                if (Projects.Any() && SelectedProject == null)
                {
                    SelectedProject = Projects.First();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки проектов: {ex.Message}");
            }
        }

        private void LoadTaskData()
        {
            TaskName = _task.Name;
            TaskDescription = _task.Description;
            SelectedStatus = GetTaskStatusName(_task.Status);
            SelectedDifficulty = _task.Difficulty;
            Deadline = _task.TaskDeadline;
            NoDeadline = !Deadline.HasValue;

            SelectedProject = Projects.FirstOrDefault(p => p.Id == _task.ProjectId);
            IsProjectSelectable = false;
        }

        private string GetTaskStatusName(int? status)
        {
            if (!status.HasValue) return TaskStatuses[0];
            return status >= 1 && status <= TaskStatuses.Count ?
                TaskStatuses[status.Value - 1] : TaskStatuses[0];
        }

        private bool CanSaveTask()
        {
            ValidationMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(TaskName))
            {
                ValidationMessage = "Название задачи обязательно";
                return false;
            }

            if (SelectedProject == null)
            {
                ValidationMessage = "Необходимо выбрать проект";
                return false;
            }

            if (!NoDeadline && Deadline.HasValue && Deadline.Value < DateTime.Now.Date)
            {
                ValidationMessage = "Дедлайн не может быть раньше текущей даты";
                return false;
            }

            return true;
        }

        private void SaveTask()
        {
            if (!CanSaveTask())
                return;

            try
            {
                _task.Name = TaskName;
                _task.Description = TaskDescription;
                _task.Status = TaskStatuses.IndexOf(SelectedStatus) + 1;
                _task.Difficulty = SelectedDifficulty;
                _task.ProjectId = SelectedProject.Id;
                _task.TaskDeadline = NoDeadline ? null : Deadline;

                if (!_isEditMode)
                {
                    _context.Tasks.Add(_task);
                }

                _context.SaveChanges();

                UpdateProjectProgress(_task.ProjectId);

                CloseWindow(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения задачи: {ex.Message}");
            }
        }

        private void UpdateProjectProgress(int projectId)
        {
            try
            {
                var project = _context.Projects
                    .Include(p => p.Tasks)
                    .FirstOrDefault(p => p.Id == projectId);

                if (project != null && project.Tasks.Any())
                {
                    int completedTasks = project.Tasks.Count(t => t.Status == 4);
                    project.Progress = (int?)((double)completedTasks / project.Tasks.Count() * 100);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления прогресса: {ex.Message}");
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