using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Interfaces;
using Task_management_system.Models;

namespace Task_management_system.Services;

public class ProjectService : Controller, IProjectService
{
    private readonly Context _context;
    private readonly IKeyValueService _keyValueService;

    public ProjectService(Context context, IKeyValueService keyValueService)
    {
        _context = context;
        _keyValueService = keyValueService;
    }

    public string CreateProject(Project project)
    {
        try
        {
            _context.DetachAllEntities();
            project.ProjectName = project.ProjectName.Trim();
            project.ProjectType = _keyValueService.GetKeyValueById(project.ProjectTypeId);
            if (_context.Projects.Any(p => p.ProjectName == project.ProjectName))
            {
                return "Вече съществува такъв проект!";
            }

            List<ApplicationUserProject> projectParticipants = new List<ApplicationUserProject>();
            if (project.ProjectParticipants != null)
            {
                projectParticipants.AddRange(project.ProjectParticipants.Select(i => new ApplicationUserProject()
                {
                    ProjectId = i.ProjectId,
                    Project = i.Project,
                    User = i.User,
                    UserId = i.UserId
                }));
            }

            project.ProjectParticipants = new List<ApplicationUserProject>();
            ApplicationUser? projectOwner = _context.Users.FirstOrDefault(u => u.Id == project.ProjectOwner.Id);

            if (projectOwner != null)
            {
                project.ProjectOwner = projectOwner;
            }

            project.ProjectName = project.ProjectName.Trim();
            _context.Entry(project).State = EntityState.Added;
            _ = _context.Projects.Add(project);

            foreach (ApplicationUserProject participant in projectParticipants)
            {
                ApplicationUser participantUser = _context.Users.Where(x => x.Id == participant.UserId).First();
                _ = _context.Add(new ApplicationUserProject { User = participantUser, Project = project });
            }


            _ = _context.SaveChanges();
            _context.DetachAllEntities();
            return "Успешно създаване на проект!";
        }
        catch
        {
            return "Неуспешно създаване на проект!";
        }
    }

    public string DeleteProject(int id)
    {
        try
        {

            _context.DetachAllEntities();
            Project? project = _context.Projects.Find(id);
            if (project == null)
            {
                return "Проектът не е намерен.";
            }
            else
            {
                EntityEntry<Project> entry = this._context.Entry(project);

                if (entry.State == EntityState.Detached)
                {
                    this._context.Set<Project>().Attach(project);
                }

                entry.State = EntityState.Deleted;


                List<ApplicationUserProject> participants = _context.ApplicationUserProjects
                    .Where(pp => pp.ProjectId == project.ProjectId).ToList();

                foreach (ApplicationUserProject? participant in participants)
                {
                    _ = _context.Entry(participant).State = EntityState.Deleted;
                }


                //_ = _context.Projects.Remove(project);


                _ = _context.SaveChanges();
                return "Успешно изтриване на проект.";
            }
        }
        catch (Exception)
        {
            return "Неуспешно изтриване на проект. Моля, изтрийте всички свързани задачи!";
        }
    }


    public List<Project> GetAllProjects()
    {
        // return _context.Projects.Include(x => x.ProjectParticipants).ThenInclude(pp => pp.User).ToList();
        List<Project> projects = _context.Projects.AsNoTracking().Include(x => x.ProjectParticipants)!
            .ThenInclude(pp => pp.User).Include(p => p.ProjectType).Include(p => p.ProjectOwner).Include(p => p.Issues).ThenInclude(i => i.AssignedТo).Include(p => p.Issues).ThenInclude(i => i.Assignee).Include(i => i.Issues).ThenInclude(y => y.Subtasks).ToList();


        return projects;
    }

    public Project GetProjectById(int projectId)
    {
        return _context.Projects.AsNoTracking().Include(x => x.ProjectParticipants)!
            .ThenInclude(pp => pp.User).Include(p => p.ProjectType).Include(p => p.ProjectOwner).Include(p => p.Issues).ThenInclude(i => i.AssignedТo).Include(p => p.Issues).ThenInclude(i => i.Assignee).Include(i => i.Issues).ThenInclude(y => y.Subtasks).FirstOrDefault()!;
    }

    public string UpdateProject(Project project)
    {
        try
        {
            _context.DetachAllEntities();
            project.ProjectName = project.ProjectName.Trim();
            project.ProjectType = _keyValueService.GetKeyValueById(project.ProjectTypeId);

            List<ApplicationUserProject> projectParticipants = new List<ApplicationUserProject>();
            if (project.ProjectParticipants != null)
            {
                projectParticipants.AddRange(project.ProjectParticipants.Select(i => new ApplicationUserProject()
                {
                    ProjectId = i.ProjectId,
                    Project = i.Project,
                    User = i.User,
                    UserId = i.UserId
                }));
            }

            project.ProjectParticipants = new List<ApplicationUserProject>();
            ApplicationUser? projectOwner = _context.Users.FirstOrDefault(u => u.Id == project.ProjectOwner.Id);
            _context.Entry(project.ProjectOwner).State = EntityState.Detached;


            if (projectOwner != null)
            {
                project.ProjectOwner = projectOwner;
            }

            _context.Entry(project.ProjectOwner).State = EntityState.Modified;
            Project? local = _context.Set<Project>().Local.FirstOrDefault(entry => entry.ProjectId.Equals(project.ProjectId));
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }
            _context.Entry(project).State = EntityState.Modified;

            List<ApplicationUserProject> participantsToRemove = _context.ApplicationUserProjects.Where(pp => pp.ProjectId == project.ProjectId).ToList();

            foreach (ApplicationUserProject? participant in participantsToRemove)
            {
                _ = _context.ApplicationUserProjects.Remove(participant);
            }

            foreach (ApplicationUserProject participant in projectParticipants)
            {
                ApplicationUser participantUser = _context.Users.Where(x => x.Id == participant.UserId).First();
                ApplicationUserProject applicationUserProject = new ApplicationUserProject { User = participantUser, Project = project };
                ApplicationUserProject? localApplicationUserProject = _context.Set<ApplicationUserProject>().Local.FirstOrDefault(entry => entry.ProjectId.Equals(applicationUserProject.Project.ProjectId) && entry.UserId.Equals(applicationUserProject.User.Id));
                if (localApplicationUserProject != null)
                {
                    // detach
                    _context.Entry(applicationUserProject).State = EntityState.Detached;
                }
                _context.Entry(applicationUserProject).State = EntityState.Added;

            }



            _ = _context.SaveChanges();
            _context.Entry(project.ProjectOwner).State = EntityState.Detached;
            _context.DetachAllEntities();
            return "Успешно актуализиране на проект!";
        }
        catch (Exception)
        {
            return "Неуспешно актуализиране на проект. Моля, опитайте отново.";
        }
    }







}