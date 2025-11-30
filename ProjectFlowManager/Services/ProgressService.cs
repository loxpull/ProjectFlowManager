using System.Linq;
using ProjectFlowManager.ViewModels;

namespace ProjectFlowManager.Services
{
    public class ProgressService
    {
        public double CalculateProgress(Project project)
        {
            if (project.Tasks == null || project.Tasks.Count == 0) return 0.0;

            var total = project.Tasks.Count;
            var done = project.Tasks.Count(t => t.IsCompleted);
            return (double)done / total * 100.0;
        }
    }
}