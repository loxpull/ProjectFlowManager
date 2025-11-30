using ProjectFlowManager.ViewModels;
using System;
using System.Linq;

namespace ProjectFlowManager.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Создаем базу данных, если она не существует
            context.Database.EnsureCreated();

            // Проверяем, есть ли уже данные в базе
            if (context.Projects.Any())
            {
                return; // База уже инициализирована
            }

            // Создаем тестовые проекты
            var projects = new Project[]
            {
                new Project
                {
                    Name = "Website Redesign",
                    Description = "Complete redesign of company website with modern UI/UX",
                    Tasks = new System.Collections.Generic.List<TaskItem>
                    {
                        new TaskItem { Title = "Create wireframes and mockups", IsCompleted = true, DueDate = DateTime.Now.AddDays(-5) },
                        new TaskItem { Title = "Design color scheme and typography", IsCompleted = true, DueDate = DateTime.Now.AddDays(-2) },
                        new TaskItem { Title = "Implement responsive layout", IsCompleted = false, DueDate = DateTime.Now.AddDays(10) },
                        new TaskItem { Title = "Add interactive elements", IsCompleted = false, DueDate = DateTime.Now.AddDays(15) },
                        new TaskItem { Title = "Testing and bug fixes", IsCompleted = false, DueDate = DateTime.Now.AddDays(20) },
                        new TaskItem { Title = "Deploy to production", IsCompleted = false, DueDate = DateTime.Now.AddDays(25) }
                    }
                },
                new Project
                {
                    Name = "Mobile App Development",
                    Description = "Build cross-platform mobile application for iOS and Android",
                    Tasks = new System.Collections.Generic.List<TaskItem>
                    {
                        new TaskItem { Title = "Define app requirements", IsCompleted = true, DueDate = DateTime.Now.AddDays(-10) },
                        new TaskItem { Title = "Create user stories", IsCompleted = true, DueDate = DateTime.Now.AddDays(-7) },
                        new TaskItem { Title = "Set up development environment", IsCompleted = true, DueDate = DateTime.Now.AddDays(-3) },
                        new TaskItem { Title = "Implement core features", IsCompleted = false, DueDate = DateTime.Now.AddDays(30) },
                        new TaskItem { Title = "API integration", IsCompleted = false, DueDate = DateTime.Now.AddDays(40) },
                        new TaskItem { Title = "User testing phase", IsCompleted = false, DueDate = DateTime.Now.AddDays(50) },
                        new TaskItem { Title = "App store submission", IsCompleted = false, DueDate = DateTime.Now.AddDays(60) }
                    }
                },
                new Project
                {
                    Name = "Marketing Campaign",
                    Description = "Q4 marketing campaign for product launch",
                    Tasks = new System.Collections.Generic.List<TaskItem>
                    {
                        new TaskItem { Title = "Define target audience", IsCompleted = true, DueDate = DateTime.Now.AddDays(-15) },
                        new TaskItem { Title = "Create campaign strategy", IsCompleted = false, DueDate = DateTime.Now.AddDays(5) },
                        new TaskItem { Title = "Design marketing materials", IsCompleted = false, DueDate = DateTime.Now.AddDays(12) },
                        new TaskItem { Title = "Schedule social media posts", IsCompleted = false, DueDate = DateTime.Now.AddDays(18) },
                        new TaskItem { Title = "Monitor campaign performance", IsCompleted = false, DueDate = DateTime.Now.AddDays(25) }
                    }
                },
                new Project
                {
                    Name = "Team Training Program",
                    Description = "Develop and implement training program for new team members",
                    Tasks = new System.Collections.Generic.List<TaskItem>
                    {
                        new TaskItem { Title = "Identify training needs", IsCompleted = true, DueDate = DateTime.Now.AddDays(-8) },
                        new TaskItem { Title = "Create training materials", IsCompleted = false, DueDate = DateTime.Now.AddDays(7) },
                        new TaskItem { Title = "Schedule training sessions", IsCompleted = false, DueDate = DateTime.Now.AddDays(14) },
                        new TaskItem { Title = "Conduct training", IsCompleted = false, DueDate = DateTime.Now.AddDays(21) },
                        new TaskItem { Title = "Gather feedback", IsCompleted = false, DueDate = DateTime.Now.AddDays(28) },
                        new TaskItem { Title = "Update materials based on feedback", IsCompleted = false, DueDate = DateTime.Now.AddDays(35) }
                    }
                }
            };

            context.Projects.AddRange(projects);
            context.SaveChanges();

            Console.WriteLine("Database initialized with sample data.");
        }
    }
}