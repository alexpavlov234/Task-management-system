using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Data;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Models;
using Issue = Task_management_system.Models.Issue;

public class IssueService : Controller, IUssueService
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

    public void CreateTask(Issue issue)
    {
        Issue task1 = _context.Tasks.FirstOrDefault((c) => (c.IssueName == issue.IssueName));
        if (task1 == null)
        {
            _context.Tasks.Add(issue);
            _context.SaveChanges();
        }
    }

    public void DeleteTask(Issue issue)
    {
        _context.Tasks.Remove(issue);
        _context.SaveChanges();
    }

    public void DeleteTask(int IssueId)
    {
        _context.Tasks.Remove(new Issue { IssueId = IssueId });
        _context.SaveChanges();
    }

    public List<Issue> GetAllTasks()
    {
        return _context.Tasks.ToList();
    }

    public Issue GetTaskById(int TaskId)
    {
        return _context.Tasks.Where(x => x.IssueId == TaskId).FirstOrDefault();
    }

    public Issue GetTaskByTaskName(string TaskName)
    {
        return _context.Tasks.Where(x => x.IssueName == TaskName).FirstOrDefault();
    }

    public void UpdateTask(Issue issue)
    {
        Issue? local = _context.Set<Issue>().Local.FirstOrDefault(entry => entry.IssueId.Equals(issue.IssueId));
        if (local != null)
        {
            // detach
            _context.Entry(local).State = EntityState.Detached;
        }
        _context.Entry(issue).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void CreateSubtask(Issue issue, Subtask subtask)
    {
        issue.Subtasks.Add(subtask);
        _context.Tasks.Update(issue);
        _context.SaveChanges();
    }

    public void CreateSubtask(int TaskId, Subtask subtask)
    {
        Issue issue = GetTaskById(TaskId);
        issue.Subtasks.Add(subtask);
        _context.Tasks.Update(issue);
        _context.SaveChanges();
    }

    public void UpdateSubtask(Subtask subtask)
    {
        Subtask? local = _context.Set<Subtask>().Local.FirstOrDefault(entry => entry.SubtaskId.Equals(subtask.SubtaskId));
        if (local != null)
        {
            // detach
            _context.Entry(local).State = EntityState.Detached;
        }
        _context.Entry(subtask).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void DeleteSubtask(Subtask subtask)
    {
        _context.Subtasks.Remove(subtask);
        _context.SaveChanges();
    }

    public Subtask GetSubtaskById(int SubtaskId)
    {
        return _context.Subtasks.Where(x => x.SubtaskId == SubtaskId).FirstOrDefault();
    }

    public Subtask GetSubtaskByTaskName(string SubtaskName)
    {
        return _context.Subtasks.Where(x => x.SubtaskName == SubtaskName).FirstOrDefault();
    }
}