using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Data;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Models;
using Task_management_system.Pages;
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

    //public string CreateTask(Issue issue)
    //{
    //    Issue task1 = _context.Issues.FirstOrDefault((c) => (c.Subject == issue.Subject));
    //    if (task1 == null)
    //    {
    //        try
    //        {
    //            _context.Issues.Add(issue);
    //            _context.SaveChanges();
    //            return "Успешно добавяне на задача!";
    //        } catch { 
    //            return "Неуспешно добавяне на задача!";

    //        }
    //    } else
    //    {
    //        return "Вече съществува такъв задача!";
    //    }
    //}

    public string CreateTask(Issue issue)
    {
        try
        {
            var assignedTo = _context.Users.FirstOrDefault(u => u.Id == issue.AssignedТo.Id);
            issue.AssignedТo = assignedTo;
            var assignee = _context.Users.FirstOrDefault(u => u.Id == issue.Assignee.Id);
            issue.Assignee = assignee;

            var project = _context.Projects.FirstOrDefault(p => p.ProjectId == issue.ProjectId);
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



    public void DeleteTask(Issue issue)
    {
        _context.Issues.Remove(issue);
        _context.SaveChanges();
    }

    public void DeleteTask(int IssueId)
    {
        _context.Issues.Remove(new Issue { IssueId = IssueId });
        _context.SaveChanges();
    }

    public List<Issue> GetAllTasks()
    {
        List<Issue> issues = _context.Issues.ToList();
        return issues;
    }

    public Issue GetTaskById(int TaskId)
    {  Issue issue1 = _context.Issues.Where(x => x.IssueId == TaskId).FirstOrDefault();
       return issue1;
    }

    public Issue GetTaskByTaskName(string TaskName)
    {
        Issue issue1 = _context.Issues.Where(x => x.Subject == TaskName).FirstOrDefault();
        return issue1;
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

    public string CreateSubtask(Subtask subtask)
    {
        Issue? local = _context.Set<Issue>().Local.FirstOrDefault(entry => entry.IssueId.Equals(subtask.Issue.IssueId));
        if (local != null)
        {
            // detach
            _context.Entry(local).State = EntityState.Detached;
        }
        _context.Entry(subtask).State = EntityState.Modified;
     
        _context.Subtasks.Add(subtask);
        _context.SaveChanges();
        return "Yeah!";
    }

    public void CreateSubtask(int TaskId, Subtask subtask)
    {
        Issue issue = GetTaskById(TaskId);
        issue.Subtasks.Add(subtask);
        _context.Issues.Update(issue);
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

    public Subtask GetSubtaskByTaskName(string subtaskSubject)
    {
        return _context.Subtasks.Where(x => x.Subject == subtaskSubject).FirstOrDefault();
    }
}