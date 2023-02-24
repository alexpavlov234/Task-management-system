﻿using Microsoft.AspNetCore.Identity;
using Task_management_system.Areas.Identity;
using Task_management_system.Models;

namespace Task_management_system.Data
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            _ = await roleManager.CreateAsync(new IdentityRole("Admin"));
            _ = await roleManager.CreateAsync(new IdentityRole("User"));

        }
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            ApplicationUser defaultUser = new ApplicationUser
            {
                UserName = "alexpavlov234",
                Email = "alexpavlov234@gmail.com",
                FirstName = "Александър",
                LastName = "Павлов",
                EmailConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                ApplicationUser user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    _ = await userManager.CreateAsync(defaultUser, "Alex123@&");
                    _ = await userManager.AddToRoleAsync(defaultUser, "Admin");
                }

            }
        }

        public static void SeedKeyValuesAsync(Context context)
        {
            KeyType projectKeyType = new KeyType { KeyTypeName = "Тип на проект", KeyTypeIntCode = "ProjectType", Description = "Тип на проекта", IsSystem = true };
            KeyType issueStatusKeyType = new KeyType { KeyTypeName = "Статус на задача", KeyTypeIntCode = "IssueStatus", Description = "Статус на задача", IsSystem = true };
            KeyType issuePriorityKeyType = new KeyType { KeyTypeName = "Приоритет на задача", KeyTypeIntCode = "IssuePriority", Description = "Приоритет на задача", IsSystem = true };

            _ = context.KeyType.Add(projectKeyType);
            _ = context.KeyType.Add(issueStatusKeyType);
            _ = context.KeyType.Add(issuePriorityKeyType);
            _ = context.KeyValue.Add(new KeyValue { KeyType = issueStatusKeyType, Name = "Нова", NameEN = "New", IsActive = true, KeyValueIntCode = "New", Description = "Нова" });
            _ = context.KeyValue.Add(new KeyValue { KeyType = issueStatusKeyType, Name = "В изпълнение", NameEN = "In execution", IsActive = true, KeyValueIntCode = "InExecution", Description = "В изпълнение" });
            _ = context.KeyValue.Add(new KeyValue { KeyType = issueStatusKeyType, Name = "За преглед", NameEN = "For review", IsActive = true, KeyValueIntCode = "ForReview", Description = "За преглед" });
            _ = context.KeyValue.Add(new KeyValue { KeyType = issueStatusKeyType, Name = "Затворена", NameEN = "Closed", IsActive = true, KeyValueIntCode = "Closed", Description = "Затворена" });
            _ = context.KeyValue.Add(new KeyValue { KeyType = issueStatusKeyType, Name = "Върната за корекция", NameEN = "Returned for correction", IsActive = true, KeyValueIntCode = "ReturnedForCorrection", Description = "Върната за корекция" });
            _ = context.KeyValue.Add(new KeyValue { KeyType = projectKeyType, Name = "Работен", NameEN = "Working", IsActive = true, KeyValueIntCode = "Working", Description = "Проект свързан с работа" });
            _ = context.KeyValue.Add(new KeyValue { KeyType = projectKeyType, Name = "Учебен", NameEN = "Educational", IsActive = true, KeyValueIntCode = "Educational", Description = "Проект свързан с учебен процес/училище" });
            _ = context.KeyValue.Add(new KeyValue { KeyType = projectKeyType, Name = "Личен", NameEN = "Personal", IsActive = true, KeyValueIntCode = "Personal", Description = "Проект свързан с личния живот" });
            _ = context.KeyValue.Add(new KeyValue { KeyType = issuePriorityKeyType, Name = "Висок", NameEN = "High", IsActive = true, KeyValueIntCode = "High", Description = "Висок приоритет" });
            _ = context.KeyValue.Add(new KeyValue { KeyType = issuePriorityKeyType, Name = "Нисък", NameEN = "Low", IsActive = true, KeyValueIntCode = "Low", Description = "Нисък приоритет" });
            _ = context.KeyValue.Add(new KeyValue { KeyType = issuePriorityKeyType, Name = "Нормален", NameEN = "Normal", IsActive = true, KeyValueIntCode = "Normal", Description = "Нормален приоритет" });
            _ = context.SaveChanges();
        }
    }
}
