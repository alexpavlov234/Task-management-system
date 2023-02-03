using Task_management_system.Models;

internal interface IIssueService
{
    void CreateSubtask(Issue issue, Subtask subtask);
    void CreateSubtask(int TaskId, Subtask subtask);
    string CreateTask(Issue issue);
    void DeleteSubtask(Subtask subtask);
    void DeleteTask(Issue issue);
    List<Issue> GetAllTasks();
    Subtask GetSubtaskById(int SubtaskId);
    Subtask GetSubtaskByTaskName(string SubtaskName);
    Issue GetTaskById(int TaskId);
    Issue GetTaskByTaskName(string Subject);
    void UpdateSubtask(Subtask subtask);
    void UpdateTask(Issue issue);
}