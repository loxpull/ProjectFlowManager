using ProjectFlowManager.ViewModels;
using Xunit;

namespace ProjectFlowManager.Tests
{
    public class MainViewModelTests
    {
        [Fact]
        public void Constructor_InitializesCollections()
        {
            // Arrange & Act
            var viewModel = new MainViewModel();

            // Assert
            Assert.NotNull(viewModel.Projects);
            Assert.NotNull(viewModel.AllTasks);
            Assert.Empty(viewModel.Projects);
            Assert.Empty(viewModel.AllTasks);
        }

        [Fact]
        public void Properties_CanBeSet()
        {
            // Arrange
            var viewModel = new MainViewModel();

            // Act
            viewModel.ActiveProjectsCount = 5;
            viewModel.OverdueTasksCount = 3;

            // Assert
            Assert.Equal(5, viewModel.ActiveProjectsCount);
            Assert.Equal(3, viewModel.OverdueTasksCount);
        }

        [Fact]
        public void Commands_AreInitialized()
        {
            // Arrange & Act
            var viewModel = new MainViewModel();

            // Assert
            Assert.NotNull(viewModel.LoadDataCommand);
            Assert.NotNull(viewModel.AddProjectCommand);
            Assert.NotNull(viewModel.EditProjectCommand);
            Assert.NotNull(viewModel.DeleteProjectCommand);
            Assert.NotNull(viewModel.AddTaskCommand);
        }
    }
}