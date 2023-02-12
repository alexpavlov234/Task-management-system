using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        project.ProjectType = _keyValueService.GetKeyValueById(project.ProjectTypeId);
        Project project1 = _context.Projects.FirstOrDefault((c) => (c.ProjectName == project.ProjectName));

        if (project1 == null)
        {
            var project2 = new Project();
            project2.ProjectName = project.ProjectName;
            project2.ProjectTypeId = project.ProjectTypeId;
            project2.StartDate = project.StartDate;
            project2.EndDate = project.EndDate;
            project2.ProjectDescription = project.ProjectDescription;
            project2.ProjectOwner = _context.Users.Where(x => x.Id == project.ProjectOwner.Id).First();
            _context.Projects.Add(project2);
            _context.SaveChanges();

            var participants = project.ProjectParticipants;

            foreach (var participant in participants)
            {
                var participantUser = _context.Users.Where(x => x.Id == participant.UserId).First();
                _context.Entry(participantUser).State = EntityState.Detached;

                _context.Add(new ApplicationUserProject { UserId = participantUser.Id, ProjectId = project2.ProjectId });
            }


            _context.SaveChanges();

            return "Успешно създаване на проект!";
        }
        else
        {
            return "Вече съществува такъв проект!";
        }
    }




    public void DeleteProject(Project project)
    {
        _context.Projects.Remove(project);
        _context.SaveChanges();
    }

    public void DeleteProject(int ProjectId)
    {
        _context.Projects.Remove(new Project { ProjectId = ProjectId });
        _context.SaveChanges();
    }

    public List<Project> GetAllProjects()
    {
        // return _context.Projects.Include(x => x.ProjectParticipants).ThenInclude(pp => pp.User).ToList();
        var projects = _context.Projects.Include(x => x.ProjectParticipants)
            .ThenInclude(pp => pp.User)
            .ToList();


        return projects;
    }

    public Project GetProjectById(int ProjectId)
    {
        return _context.Projects.Where(x => x.ProjectId == ProjectId).FirstOrDefault();
    }

    public Project GetProjectByProjectName(string ProjectName)
    {
        return _context.Projects.Where(x => x.ProjectName == ProjectName).FirstOrDefault();
    }

    public void UpdateProject(Project project)
    {
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
        var projectOwner = _context.Users.FirstOrDefault(u => u.Id == project.ProjectOwner.Id);
        _context.Entry(project.ProjectOwner).State = EntityState.Detached;
        project.ProjectOwner = projectOwner;
        Project? local = _context.Set<Project>().Local.FirstOrDefault(entry => entry.ProjectId.Equals(project.ProjectId));
        if (local != null)
        {
            // detach
            _context.Entry(local).State = EntityState.Detached;
        }
        _context.Entry(project).State = EntityState.Modified;
        _context.SaveChanges();

        var participantsToRemove = _context.ApplicationUserProjects.Where(pp => pp.ProjectId == project.ProjectId).ToList();
        foreach (var participant in participantsToRemove)
        {
            _context.ApplicationUserProjects.Remove(participant);
        }
        _context.SaveChanges();

        foreach (var participant in projectParticipants)
        {
            var participantUser = _context.Users.Where(x => x.Id == participant.UserId).First();
            _context.Add(new ApplicationUserProject { User = participantUser, Project = project });
        }

        _context.SaveChanges();
    }






}