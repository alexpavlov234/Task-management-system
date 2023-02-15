using Task_management_system.Models;

internal interface IIssueService
{
    string CreateSubtask(Subtask subtask);
    string CreateIssue(Issue issue);
    string DeleteSubtask(Subtask subtask);
    string DeleteIssue(Issue issue);
    List<Issue> GetAllIssues();
    Subtask GetSubtaskById(int SubtaskId);
    Issue GetIssueById(int IssueId);
    string UpdateSubtask(Subtask subtask);
    string UpdateIssue(Issue issue);
}