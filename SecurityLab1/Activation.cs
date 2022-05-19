using System;
using System.IO;
using System.Text.Json;

namespace SecurityLab1
{
    class Activation
    {
        public Activation(bool isActivated, string password, string keyWord)
        {
            IsActivated = isActivated;
            Password = password;
            KeyWord = keyWord;
        }

        public bool IsActivated { get; private set; }

        public string Password { get; private set; }

        public string KeyWord { get; private set; }

        public bool CheckPassword(string password)
        {
            bool result = Vigenere.Encode(password, KeyWord) == Password;

            Console.WriteLine(Vigenere.Encode(password, KeyWord));

            if (result == true)
            {
                IsActivated = true;
                var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Activation.json");
                File.WriteAllText(path, JsonSerializer.Serialize(this));
            }

            return result;
        }
    }
}
