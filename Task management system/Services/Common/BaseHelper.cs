namespace Task_management_system.Pages.Common
{
    public class BaseHelper
    {



        public static string GeneratePassword()
        {
            // Константи с малките, големите букви, цифрите и специалните знаци
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numberChars = "0123456789";
            const string specialChars = "!@#%^*";
            // Дължина на генерираната парола
            const int passwordLength = 12;
            // Масив съдържащ генерираната парола
            char[] password = new char[passwordLength];
            // Масив съдържащ всички валидни набори от символи
            var chars = new[] { lowerChars, upperChars, numberChars, specialChars };
            // Инстанция на генератор за случайни числа
            var random = new Random();
            // Поставяне на малка и голяма буква в случаен индекс
            int lowerIndex = random.Next(0, passwordLength);
            int upperIndex = random.Next(0, passwordLength);
            password[lowerIndex] = lowerChars[random.Next(0, lowerChars.Length)];
            password[upperIndex] = upperChars[random.Next(0, upperChars.Length)];
            // Цикъл за генериране на паролата
            for (int i = 0; i < passwordLength; i++)
            {
                if (i != lowerIndex && i != upperIndex)
                {
                    // Избиране на случаен набор от символи
                    string set = chars[random.Next(0, chars.Length)];

                    // Добавяне на случаен символ от избрания набор
                    password[i] = set[random.Next(0, set.Length)];
                }
            }
            // Решултатът е паролата в случайна последователност
            return new string(password.OrderBy(x => random.Next()).ToArray());
        }
    }

}
