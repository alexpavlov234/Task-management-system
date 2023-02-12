using Task_management_system.Models;

internal interface IProjectService
{
    string CreateProject(Project project);
    void DeleteProject(Project project);
    void DeleteProject(int ProjectId);
    List<Project> GetAllProjects();
    Project GetProjectById(int ProjectId);
    Project GetProjectByProjectName(string ProjectName);
    string UpdateProject(Project project);

}