using Microsoft.AspNetCore.Identity;
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
        public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] firstNames = { "Петър", "Иван", "Георги", "Мария", "Елена", "Николай", "Валерия", "Димитър", "Александра", "Стефан", "Александър" };
            string[] lastNames = { "Иванов", "Петров", "Георгиев", "Николова", "Петрова", "Иванова", "Костов", "Ангелов", "Попова", "Димитров", "Павлов" };
            string[] usernames = { "ivanov123", "petrov456", "georgiev789", "nikolova234", "petrova567", "ivanova890", "kostov123", "angelov456", "popova789", "dimitrov234", "alexpavlov234" };
            string[] emails = { "ivanov123@gmail.com", "petrov456@gmail.com", "georgiev789@gmail.com", "nikolova234@gmail.com", "petrova567@gmail.com", "ivanova890@gmail.com", "kostov123@gmail.com", "angelov456@gmail.com", "popova789@gmail.com", "dimitrov234@gmail.com", "alexpavlov234@gmail.com" };
            // Validate that the arrays are the same length
            if (firstNames.Length != lastNames.Length || firstNames.Length != usernames.Length || firstNames.Length != emails.Length)
            {
                throw new ArgumentException("The arrays must be the same length");
            }
            for (int i = 0; i < firstNames.Length; i++)
            {
                string firstName = firstNames[i];
                string lastName = lastNames[i];
                string username = usernames[i];
                string email = emails[i];
                string password = $"{username.Substring(0, 1).ToUpper() + username.Substring(1, 3)}123@&";
                // Validate that the username and email are unique
                ApplicationUser userByUsername = await userManager.FindByNameAsync(username);
                ApplicationUser userByEmail = await userManager.FindByEmailAsync(email);
                if (userByUsername != null || userByEmail != null)
                {
                    continue; // Skip creating the user if the username or email already exists
                }
                ApplicationUser newUser = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };
                _ = await userManager.CreateAsync(newUser, password);
                if (email == "alexpavlov234@gmail.com")
                {
                    _ = await userManager.AddToRoleAsync(newUser, "Admin");
                }
                else if (i % 2 == 0)
                {
                    _ = await userManager.AddToRoleAsync(newUser, "Admin");
                }
                else
                {
                    _ = await userManager.AddToRoleAsync(newUser, "User");
                }
            }
        }
        public static void SeedKeyValues(Context context)
        {
            List<KeyType> keyTypes = new List<KeyType>()
            {
                new KeyType { KeyTypeName = "Тип на проект", KeyTypeIntCode = "ProjectType", Description = "Тип на проекта", IsSystem = true },
                new KeyType { KeyTypeName = "Статус на задача", KeyTypeIntCode = "IssueStatus", Description = "Статус на задача", IsSystem = true },
                new KeyType { KeyTypeName = "Приоритет на задача", KeyTypeIntCode = "IssuePriority", Description = "Приоритет на задача", IsSystem = true }
            };
            foreach (KeyType keyType in keyTypes)
            {
                if (!context.KeyType.Any(x => x.KeyTypeIntCode == keyType.KeyTypeIntCode))
                {
                    _ = context.KeyType.Add(keyType);
                }
            }
            KeyValue[] keyValues = new KeyValue[]
            {
                new KeyValue { KeyType = keyTypes[1], Name = "Нова", NameEN = "New", IsActive = true, KeyValueIntCode = "New", Description = "Нова" },
                new KeyValue { KeyType = keyTypes[1], Name = "В изпълнение", NameEN = "In execution", IsActive = true, KeyValueIntCode = "InExecution", Description = "В изпълнение" },
                new KeyValue { KeyType = keyTypes[1], Name = "За преглед", NameEN = "For review", IsActive = true, KeyValueIntCode = "ForReview", Description = "За преглед" },
                new KeyValue { KeyType = keyTypes[1], Name = "Затворена", NameEN = "Closed", IsActive = true, KeyValueIntCode = "Closed", Description = "Затворена" },
                new KeyValue { KeyType = keyTypes[1], Name = "Върната за корекция", NameEN = "Returned for correction", IsActive = true, KeyValueIntCode = "ReturnedForCorrection", Description = "Върната за корекция" },
                new KeyValue { KeyType = keyTypes[0], Name = "Работен", NameEN = "Working", IsActive = true, KeyValueIntCode = "Working", Description = "Проект свързан с работа" },
                new KeyValue { KeyType = keyTypes[0], Name = "Учебен", NameEN = "Educational", IsActive = true, KeyValueIntCode = "Educational", Description = "Проект свързан с учебен процес/училище" },
                new KeyValue { KeyType = keyTypes[0], Name = "Личен", NameEN = "Personal", IsActive = true, KeyValueIntCode = "Personal", Description = "Проект свързан с личния живот" },
                new KeyValue { KeyType = keyTypes[2], Name = "Висок", NameEN = "High", IsActive = true, KeyValueIntCode = "High", Description = "Висок приоритет" },
                new KeyValue { KeyType = keyTypes[2], Name = "Нисък", NameEN = "Low", IsActive = true, KeyValueIntCode = "Low", Description = "Нисък приоритет" },
                new KeyValue { KeyType = keyTypes[2], Name = "Нормален", NameEN = "Normal", IsActive = true, KeyValueIntCode = "Normal", Description = "Нормален приоритет" }
            };
            foreach (KeyValue keyValue in keyValues)
            {
                if (!context.KeyValue.Any(x => x.KeyValueIntCode == keyValue.KeyValueIntCode))
                {
                    _ = context.KeyValue.Add(keyValue);
                }
            }
            _ = context.SaveChanges();
        }
    }
}
