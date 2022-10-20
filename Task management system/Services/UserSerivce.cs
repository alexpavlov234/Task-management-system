using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_management_system.Data;
using Task_management_system.Interfaces;
using Task_management_system.Models;

namespace Task_management_system.Services
{
    public class UserService : Controller, IUserService
    {
        [Inject]
        private Context context { get; set; }

        public UserService(Context context)
        {
            this.context = context;
        }

        //public void DeleteUser(int id)
        //{
        //    User car = context.Users.FirstOrDefault(p => p.UserId == id);
        //    if (car != null)
        //    {
        //        context.Users.Remove(car);
        //        context.SaveChanges();
        //    }
        //}

        //public List<User> GetUsers()
        //{
        //    return context.Users.ToList();
        //}

        //public void InsertUser(User user)
        //{
        //    User testUser = context.Users.FirstOrDefault((c) => (c.UserEmail == user.UserEmail));
        //    if (testUser == null)
        //    {
        //        context.Users.Add(user);
        //        context.SaveChanges();
        //    }
        //}

        //public User GetSingleUser(int id)
        //{
        //    User user = context.Users.FirstOrDefault((c) => (c.UserId == id));
        //    return new User()
        //    {
        //        UserId = user.UserId,
        //        UserFirstName = user.UserFirstName,
        //        UserLastName = user.UserLastName,
        //        Username = user.Username,
        //        UserEmail = user.UserEmail,
        //        UserPassword = user.UserPassword,
        //        UserRole = user.UserRole
        //    };
        //}
        //public User GetSingleUser(string username)
        //{
        //    User user = context.Users.FirstOrDefault((c) => c.Username.Equals(username));
        //    return new User()
        //    {
        //        UserId = user.UserId,
        //        UserFirstName = user.UserFirstName,
        //        UserLastName = user.UserLastName,
        //        Username = user.Username,
        //        UserEmail = user.UserEmail,
        //        UserPassword = user.UserPassword,
        //        UserRole = user.UserRole
        //    };
        //}

        //public void UpdateUser(int id, User user)
        //{
        //    var local = context.Set<User>().Local.FirstOrDefault(entry => entry.UserId.Equals(user.UserId));
        //    // check if local is not null
        //    if (local != null)
        //    {
        //        // detach
        //        context.Entry(local).State = EntityState.Detached;
        //    }
        //    context.Entry(user).State = EntityState.Modified;
        //    context.SaveChanges();
        //}
    }
}