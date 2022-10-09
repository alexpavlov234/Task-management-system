using Task_management_system.Models;

namespace Task_management_system.Interfaces
{
    internal interface IUserService
    {
        void DeleteUser(int id);
        List<User> GetUsers();
        void InsertUser(User user);
        User SingleUser(int id);
        void UpdateUser(int id, User user);
    }
}