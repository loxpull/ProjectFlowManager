using ProjectFlowManager.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ProjectFlowManager.Repositories
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllAsync();
        Task<Project> GetByIdAsync(int id);
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}