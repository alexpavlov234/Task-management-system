﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Data;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Models;

public class ProjectService : Controller, IProjectService
{
    private readonly Context _context;
    private readonly IEmailSender _emailSender;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;

    public ProjectService(Context context, UserManager<ApplicationUser> userManager,
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

    public void CreateProject(Project project)
    {
        Project project1 = _context.Projects.FirstOrDefault((c) => (c.ProjectName == project.ProjectName));
        if (project1 == null)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
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
        return _context.Projects.ToList();
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
        var local = _context.Set<Project>().Local.FirstOrDefault(entry => entry.ProjectId.Equals(project.ProjectId));
        if (local != null)
        {
            // detach
            _context.Entry(local).State = EntityState.Detached;
        }
        _context.Entry(project).State = EntityState.Modified;
        _context.SaveChanges();
    }
}