using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectFlowManager.Data;
using ProjectFlowManager.ViewModels;

namespace ProjectFlowManager.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _db;

        public ProjectRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _db.Projects
                .Include(p => p.Tasks)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Project> GetByIdAsync(int id)
        {
            return await _db.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Project project)
        {
            await _db.Projects.AddAsync(project);
        }

        public async Task UpdateAsync(Project project)
        {
            _db.Projects.Update(project);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var project = await _db.Projects.FindAsync(id);
            if (project != null)
            {
                _db.Projects.Remove(project);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}