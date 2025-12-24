using ProjectFlowManager.Data;
using ProjectFlowManager.ViewModels;
using System.Windows;

namespace ProjectFlowManager.Views
{
    public partial class ProjectWindow : Window
    {
        public ProjectWindow()
        {
            InitializeComponent();
            DataContext = new ProjectViewModel();
        }

        public ProjectWindow(Projects project)
        {
            InitializeComponent();
            DataContext = new ProjectViewModel(project);
        }
    }
}