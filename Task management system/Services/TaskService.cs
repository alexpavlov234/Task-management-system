using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Data;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Models;
using Task = Task_management_system.Models.Task;

public class TaskService : Controller, ITaskService
{
    private readonly Context _context;
    private readonly IEmailSender _emailSender;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;

    public TaskService(Context context, UserManager<ApplicationUser> userManager,
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

    public void CreateTask(Task task)
    {
        Task task1 = _context.Tasks.FirstOrDefault((c) => (c.TaskName == task.TaskName));
        if (task1 == null)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }
    }

    public void DeleteTask(Task task)
    {
        _context.Tasks.Remove(task);
        _context.SaveChanges();
    }

    public void DeleteTask(int TaskId)
    {
        _context.Tasks.Remove(new Task { TaskId = TaskId });
        _context.SaveChanges();
    }

    public List<Task> GetAllTasks()
    {
        return _context.Tasks.ToList();
    }

    public Task GetTaskById(int TaskId)
    {
        return _context.Tasks.Where(x => x.TaskId == TaskId).FirstOrDefault();
    }

    public Task GetTaskByTaskName(string TaskName)
    {
        return _context.Tasks.Where(x => x.TaskName == TaskName).FirstOrDefault();
    }

    public void UpdateTask(Task task)
    {
        Task? local = _context.Set<Task>().Local.FirstOrDefault(entry => entry.TaskId.Equals(task.TaskId));
        if (local != null)
        {
            // detach
            _context.Entry(local).State = EntityState.Detached;
        }
        _context.Entry(task).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void CreateSubtask(Task task, Subtask subtask)
    {
        task.Subtasks.Add(subtask);
        _context.Tasks.Update(task);
        _context.SaveChanges();
    }

    public void CreateSubtask(int TaskId, Subtask subtask)
    {
        Task task = GetTaskById(TaskId);
        task.Subtasks.Add(subtask);
        _context.Tasks.Update(task);
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