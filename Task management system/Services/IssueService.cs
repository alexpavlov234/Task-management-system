using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Data;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Models;
using Issue = Task_management_system.Models.Issue;

public class IssueService : Controller, IIssueService
{
    private readonly Context _context;
    private readonly IEmailSender _emailSender;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;

    public IssueService(Context context, UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender)
    {
        _context = context;
        _userManager = userManager;
        _userStore = userStore;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }


    public string CreateIssue(Issue issue)
    {
        try
        {
            _context.DetachAllEntities();
            ApplicationUser? assignedTo = _context.Users.FirstOrDefault(u => u.Id == issue.AssignedТo.Id);
            issue.AssignedТo = assignedTo;
            ApplicationUser? assignee = _context.Users.FirstOrDefault(u => u.Id == issue.Assignee.Id);
            issue.Assignee = assignee;
            issue.Subject = issue.Subject.Trim();
            Project? project = _context.Projects.FirstOrDefault(p => p.ProjectId == issue.ProjectId);
            if (project == null)
            {
                return "Проект с това ID не съществува. Моля, опитайте отново.";
            }
            issue.Project = project;

            _context.Issues.Add(issue);
            _context.SaveChanges();
            return "Успешно създадена задача!";
        }
        catch (Exception)
        {
            return "Неуспешно създаване на задача. Моля, опитайте отново.";
        }
    }

    public string DeleteIssue(Issue issue)
    {
        try
        {
            _context.DetachAllEntities();
            Issue? local = _context.Set<Issue>().Local.FirstOrDefault(entry => entry.IssueId.Equals(issue.IssueId));
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(issue).State = EntityState.Deleted;
            _context.SaveChanges();
            return "Успешно изтриване на задача!";
        }
        catch
        {
            return "Неуспешно изтриване на задача!";
        }
    }

    public List<Issue> GetAllIssues()
    {
        List<Issue> issues = _context.Issues.AsNoTracking().Include(x => x.Subtasks).Include(t => t.Project).Include(t => t.Assignee).Include(t => t.AssignedТo).ToList();

        return issues;
    }

    public Issue GetIssueById(int TaskId)
    {
        Issue issue1 = _context.Issues.AsNoTracking().Where(x => x.IssueId == TaskId).Include(x => x.Subtasks).Include(t => t.Project).Include(t => t.Assignee).Include(t => t.AssignedТo).FirstOrDefault();
        return issue1;
    }



    public string UpdateIssue(Issue issue)
    {
        try
        {
            _context.DetachAllEntities();
            Issue? local = _context.Set<Issue>().Local.FirstOrDefault(entry => entry.IssueId.Equals(issue.IssueId));
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(issue).State = EntityState.Modified;
            _context.SaveChanges();
            return "Успешно актуализиране на задача!";
        }
        catch
        {
            return "Неуспешно актуализиране на задача!";
        }
    }

    public string CreateSubtask(Subtask subtask)
    {
        try
        {
            _context.DetachAllEntities();
            Issue? local = _context.Set<Issue>().Local
                .FirstOrDefault(entry => entry.IssueId.Equals(subtask.Issue.IssueId));
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(subtask).State = EntityState.Added;

            _context.Subtasks.Add(subtask);
            _context.SaveChanges();
            return "Успешно създаване на подзадача!";
        }
        catch
        {
            return "Неуспешно създаване на подзадача!";
        }


    }



    public string UpdateSubtask(Subtask subtask)
    {
        try
        {
            Subtask? local = _context.Set<Subtask>().Local
                .FirstOrDefault(entry => entry.SubtaskId.Equals(subtask.SubtaskId));
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(subtask).State = EntityState.Modified;
            _context.SaveChanges();
            return "Успешно актуализиране на подзадача!";
        }
        catch
        {
            return "Неуспешно актуализиране на подзадача!";
        }
    }

    public string DeleteSubtask(Subtask subtask)
    {
        try
        {
            _context.DetachAllEntities();
            Subtask? local = _context.Set<Subtask>().Local.FirstOrDefault(entry => entry.SubtaskId.Equals(subtask.SubtaskId));
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }
            _context.Entry(subtask).State = EntityState.Deleted;
            _context.SaveChanges();
            return "Успешно изтриване на подзадача!";
        }
        catch
        {
            return "Неуспешно изтриване на подзадача!";
        }
    }

    public Subtask GetSubtaskById(int SubtaskId)
    {
        return _context.Subtasks.Where(x => x.SubtaskId == SubtaskId).AsNoTracking().Include(s => s.Issue).FirstOrDefault();
    }


}