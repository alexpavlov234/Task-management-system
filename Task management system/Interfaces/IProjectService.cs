using Task_management_system.Models;

internal interface IProjectService
{
    string CreateProject(Project project);
    string DeleteProject(int ProjectId);
    List<Project> GetAllProjects();
    Project GetProjectById(int ProjectId);
    string UpdateProject(Project project);

}