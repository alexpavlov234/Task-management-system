using Task_management_system.Models;

internal interface IIssueService
{
    string CreateSubtask(Subtask subtask);
    void CreateSubtask(int TaskId, Subtask subtask);
    string CreateTask(Issue issue);
    void DeleteSubtask(Subtask subtask);
    void DeleteTask(int TaskId);
    List<Issue> GetAllTasks();
    Subtask GetSubtaskById(int SubtaskId);
    Subtask GetSubtaskByTaskName(string SubtaskName);
    Issue GetTaskById(int TaskId);
    void UpdateSubtask(Subtask subtask);
    void UpdateTask(Issue issue);
}