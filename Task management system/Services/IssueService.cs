using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Issue = Task_management_system.Models.Issue;
namespace Task_management_system.Services;
public class IssueService : Controller, IIssueService
{
    private readonly Context _context;
    public IssueService(Context context)
    {
        _context = context;
    }
    public string CreateIssue(Issue issue)
    {
        try
        {
            _context.DetachAllEntities();
            issue.Subject = issue.Subject.Trim();
            ApplicationUser? assignedTo = _context.Users.FirstOrDefault(u => u.Id == issue.AssignedТoId);
            if (assignedTo != null)
            {
                issue.AssignedТo = assignedTo;
            }
            ApplicationUser? assignee = _context.Users.FirstOrDefault(u => u.Id == issue.Assignee.Id);
            if (assignee != null)
            {
                issue.Assignee = assignee;
            }
            issue.Subject = issue.Subject.Trim();
            Project? project = _context.Projects.FirstOrDefault(p => p.ProjectId == issue.ProjectId);
            if (project == null)
            {
                return "Проект с това ID не съществува. Моля, опитайте отново.";
            }
            issue.Project = project;
            _ = _context.Issues.Add(issue);
            _ = _context.SaveChanges();
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
            _ = _context.SaveChanges();
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
    public List<Issue> GetAllIssues(string userId)
    {
        List<Issue> issues = _context.Issues.AsNoTracking().Include(x => x.Subtasks).Include(t => t.Project).Include(t => t.Assignee).Include(t => t.AssignedТo).Where(t => t.AssignedТoId == userId).ToList();
        return issues;
    }
    public Issue GetIssueById(int taskId)
    {
        Issue issue1 = _context.Issues.AsNoTracking().Where(x => x.IssueId == taskId).Include(x => x.Subtasks).Include(t => t.Project).Include(t => t.Assignee).Include(t => t.AssignedТo).FirstOrDefault()!;
        return issue1;
    }
    public string UpdateIssue(Issue issue)
    {
        try
        {
            _context.DetachAllEntities();
            issue.Subject = issue.Subject.Trim();
            issue.ProjectId = issue.Project.ProjectId;
            issue.EndTime = new DateTime(issue.EndTime.Year, issue.EndTime.Month, issue.EndTime.Day, 23, 59, 59);
            ApplicationUser? assignedTo = _context.Users.FirstOrDefault(u => u.Id == issue.AssignedТoId);
            if (assignedTo != null)
            {
                issue.AssignedТo = assignedTo;
            }
            Issue? local = _context.Set<Issue>().Local.FirstOrDefault(entry => entry.IssueId.Equals(issue.IssueId));
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }
            _context.Entry(issue).State = EntityState.Modified;
            _context.Entry(issue.Project).State = EntityState.Modified;
            _ = _context.SaveChanges();
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
            subtask.Subject = subtask.Subject.Trim();
            Issue? local = _context.Set<Issue>().Local
                .FirstOrDefault(entry => entry.IssueId.Equals(subtask.Issue.IssueId));
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }
            _context.Entry(subtask).State = EntityState.Added;
            _ = _context.Subtasks.Add(subtask);
            _ = _context.SaveChanges();
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
            subtask.Subject = subtask.Subject.Trim();
            Subtask? local = _context.Set<Subtask>().Local
                .FirstOrDefault(entry => entry.SubtaskId.Equals(subtask.SubtaskId));
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }
            _context.Entry(subtask).State = EntityState.Modified;
            _ = _context.SaveChanges();
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
            _ = _context.SaveChanges();
            return "Успешно изтриване на подзадача!";
        }
        catch
        {
            return "Неуспешно изтриване на подзадача!";
        }
    }
    public Subtask GetSubtaskById(int subtaskId)
    {
        return _context.Subtasks.Where(x => x.SubtaskId == subtaskId).AsNoTracking().Include(s => s.Issue).FirstOrDefault()!;
    }
    public List<Issue> GetAllIssuesByProjectAndApplicationUser(int projectId, string userId)
    {
        //TODO: Think of the assignee!
        List<Issue> issues = _context.Issues.AsNoTracking().Include(x => x.Subtasks).Include(t => t.Project).Include(t => t.Assignee).Include(t => t.AssignedТo).Where(x => x.ProjectId == projectId && x.AssignedТoId == userId).ToList();
        return issues;
    }
}