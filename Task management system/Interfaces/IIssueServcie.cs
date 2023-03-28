using Task_management_system.Models;

namespace Task_management_system.Interfaces;

internal interface IIssueService
{
    string CreateSubtask(Subtask subtask);
    string CreateIssue(Issue issue);
    string DeleteSubtask(Subtask subtask);
    string DeleteIssue(Issue issue);
    List<Issue> GetAllIssues();
    List<Issue> GetAllIssues(string userId);
    List<Issue> GetAllIssuesByProjectAndApplicationUser(int projectId, string userId);
    Subtask GetSubtaskById(int SubtaskId);
    Issue GetIssueById(int IssueId);
    string UpdateSubtask(Subtask subtask);
    string UpdateIssue(Issue issue);
}