using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Interfaces;
using Task_management_system.Models;

public class ProjectService : Controller, IProjectService
{
    private readonly Context _context;
    private readonly IEmailSender _emailSender;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IKeyValueService _keyValueService;

    public ProjectService(Context context, UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender, IKeyValueService keyValueService)
    {
        _context = context;
        _userManager = userManager;
        _userStore = userStore;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _keyValueService = keyValueService;
    }

    public string CreateProject(Project project)
    {
        try
        {
            _context.DetachAllEntities();
            project.ProjectType = _keyValueService.GetKeyValueById(project.ProjectTypeId);
            if (_context.Projects.Any(p => p.ProjectName == project.ProjectName))
            {
                return "Вече съществува такъв проект!";
            }

            List<ApplicationUserProject> projectParticipants = new List<ApplicationUserProject>();
            projectParticipants.AddRange(project.ProjectParticipants.Select(i => new ApplicationUserProject()
            {
                ProjectId = i.ProjectId,
                Project = i.Project,
                User = i.User,
                UserId = i.UserId
            }));
            project.ProjectParticipants = new List<ApplicationUserProject>();
            ApplicationUser? projectOwner = _context.Users.FirstOrDefault(u => u.Id == project.ProjectOwner.Id);

            project.ProjectOwner = projectOwner;

            _context.Entry(project).State = EntityState.Added;
            _context.Projects.Add(project);

            foreach (ApplicationUserProject participant in projectParticipants)
            {
                ApplicationUser participantUser = _context.Users.Where(x => x.Id == participant.UserId).First();
                _context.Add(new ApplicationUserProject { User = participantUser, Project = project });
            }


            _context.SaveChanges();
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

            List<ApplicationUserProject> participants = _context.ApplicationUserProjects.Where(pp => pp.ProjectId == project.ProjectId).ToList();

            foreach (ApplicationUserProject? participant in participants)
            {
                _context.ApplicationUserProjects.Remove(participant);
            }

            _context.Projects.Remove(project);


            _context.SaveChanges();
            return "Успешно изтриване на проекта.";
        }
        catch (Exception)
        {
            return "Неуспешно изтриване на проекта. Моля, опитайте отново.";
        }
    }


    public List<Project> GetAllProjects()
    {
        // return _context.Projects.Include(x => x.ProjectParticipants).ThenInclude(pp => pp.User).ToList();
        List<Project> projects = _context.Projects.AsNoTracking().Include(x => x.ProjectParticipants)
            .ThenInclude(pp => pp.User).Include(p => p.ProjectType).Include(p => p.ProjectOwner).Include(p => p.Issues).ThenInclude(i => i.AssignedТo).Include(p => p.Issues).ThenInclude(i => i.Assignee).Include(i => i.Issues).ThenInclude(y => y.Subtasks).ToList();


        return projects;
    }

    public Project GetProjectById(int ProjectId)
    {
        return _context.Projects.AsNoTracking().Where(x => x.ProjectId == ProjectId).FirstOrDefault();
    }

    public string UpdateProject(Project project)
    {
        try
        {
            _context.DetachAllEntities();
            project.ProjectType = _keyValueService.GetKeyValueById(project.ProjectTypeId);

            List<ApplicationUserProject> projectParticipants = new List<ApplicationUserProject>();
            projectParticipants.AddRange(project.ProjectParticipants.Select(i => new ApplicationUserProject()
            {
                ProjectId = i.ProjectId,
                Project = i.Project,
                User = i.User,
                UserId = i.UserId
            }));
            project.ProjectParticipants = new List<ApplicationUserProject>();
            ApplicationUser? projectOwner = _context.Users.FirstOrDefault(u => u.Id == project.ProjectOwner.Id);
            _context.Entry(project.ProjectOwner).State = EntityState.Detached;


            project.ProjectOwner = projectOwner;
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
                _context.ApplicationUserProjects.Remove(participant);
            }

            foreach (ApplicationUserProject participant in projectParticipants)
            {
                ApplicationUser participantUser = _context.Users.Where(x => x.Id == participant.UserId).First();
                _context.Add(new ApplicationUserProject { User = participantUser, Project = project });
            }



            _context.SaveChanges();
            _context.Entry(project.ProjectOwner).State = EntityState.Detached;
            _context.DetachAllEntities();
            return "Успешно актуализиране на проекта!";
        }
        catch (Exception)
        {
            return "Неуспешно актуализиране на проекта. Моля, опитайте отново.";
        }
    }







}