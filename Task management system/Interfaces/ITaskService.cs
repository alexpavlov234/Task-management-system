using Task_management_system.Models;

internal interface ITaskService
{
    void CreateSubtask(Task_management_system.Models.Task task, Subtask subtask);
    void CreateSubtask(int TaskId, Subtask subtask);
    void CreateTask(Task_management_system.Models.Task task);
    void DeleteSubtask(Subtask subtask);
    void DeleteTask(int TaskId);
    List<Task_management_system.Models.Task> GetAllTasks();
    Subtask GetSubtaskById(int SubtaskId);
    Subtask GetSubtaskByTaskName(string SubtaskName);
    Task_management_system.Models.Task GetTaskById(int TaskId);
    void UpdateSubtask(Subtask subtask);
    void UpdateTask(Task_management_system.Models.Task task);
}