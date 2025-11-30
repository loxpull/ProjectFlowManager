using ProjectFlowManager.ViewModels;
using ProjectFlowManager.Repositories;
using ProjectFlowManager.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectFlowManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IProjectRepository _repo;
        private readonly ProgressService _progressService;

        public ObservableCollection<Project> Projects { get; } = new ObservableCollection<Project>();

        private Project _selectedProject;
        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedProgress));
                OnPropertyChanged(nameof(CompletedTasksCount));
            }
        }

        public double SelectedProgress => SelectedProject != null ? _progressService.CalculateProgress(SelectedProject) : 0.0;

        public int CompletedTasksCount => SelectedProject?.Tasks?.Count(t => t.IsCompleted) ?? 0;

        public RelayCommand RefreshCommand { get; }
        public RelayCommand ToggleTaskCommand { get; }
        public RelayCommand AddProjectCommand { get; }

        public MainViewModel(IProjectRepository repo, ProgressService progressService)
        {
            _repo = repo;
            _progressService = progressService;

            RefreshCommand = new RelayCommand(async _ => await LoadAsync());
            ToggleTaskCommand = new RelayCommand(async param => await ToggleTaskAsync(param));
            AddProjectCommand = new RelayCommand(async _ => await AddSampleProject());

            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            Projects.Clear();
            var list = await _repo.GetAllAsync();
            foreach (var p in list) Projects.Add(p);

            if (Projects.Count > 0 && SelectedProject == null)
                SelectedProject = Projects[0];

            OnPropertyChanged(nameof(SelectedProgress));
            OnPropertyChanged(nameof(CompletedTasksCount));
        }

        private async Task ToggleTaskAsync(object param)
        {
            if (param is TaskItem task)
            {
                task.IsCompleted = !task.IsCompleted;
                await _repo.UpdateAsync(task.Project);
                await _repo.SaveChangesAsync();
                OnPropertyChanged(nameof(SelectedProgress));
                OnPropertyChanged(nameof(CompletedTasksCount));
            }
        }

        private async Task AddSampleProject()
        {
            var p = new Project
            {
                Name = "New Project",
                Description = "Создано из UI",
                Tasks = new System.Collections.Generic.List<TaskItem>
                {
                    new TaskItem { Title = "First task" }
                }
            };

            await _repo.AddAsync(p);
            await _repo.SaveChangesAsync();
            await LoadAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}