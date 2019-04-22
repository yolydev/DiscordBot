using MusicBot.Core.UserAccounts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MusicBot.Core
{
    public class DataStorage
    {
        public static void SaveUserAccounts(IEnumerable<UserAccount> account, string filePath)
        {
            string json = JsonConvert.SerializeObject(account);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<UserAccount> LoadUserAccount(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }
        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
