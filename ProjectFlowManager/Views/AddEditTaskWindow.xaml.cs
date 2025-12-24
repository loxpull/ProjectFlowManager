using ProjectFlowManager.Data;
using ProjectFlowManager.ViewModels;
using System.Windows;

namespace ProjectFlowManager.Views
{
    public partial class AddEditTaskWindow : Window
    {
        public AddEditTaskWindow()
        {
            InitializeComponent();
            DataContext = new TaskViewModel();
        }

        public AddEditTaskWindow(int? projectId)
        {
            InitializeComponent();
            DataContext = new TaskViewModel(projectId);
        }

        public AddEditTaskWindow(Tasks task)
        {
            InitializeComponent();
            DataContext = new TaskViewModel(task);
        }
    }
}