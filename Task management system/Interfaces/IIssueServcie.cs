using Task_management_system.Models;

internal interface IIssueService
{
    string CreateSubtask(Subtask subtask);
    string CreateIssue(Issue issue);
    void DeleteSubtask(Subtask subtask);
    void DeleteIssue(Issue issue);
    List<Issue> GetAllIssues();
    Subtask GetSubtaskById(int SubtaskId);
    Issue GetIssueById(int IssueId);
    void UpdateSubtask(Subtask subtask);
    void UpdateIssue(Issue issue);
}