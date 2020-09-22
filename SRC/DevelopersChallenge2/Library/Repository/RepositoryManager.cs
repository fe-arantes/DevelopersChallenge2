using Accountant.Library.Model;
using System;
using System.IO;
using System.Text.Json;

namespace Accountant.Library.Repository
{
    public class RepositoryManager
    {
        private readonly string databasePath;
        private readonly string databaseFile;

        public RepositoryManager()
        {
            databasePath = Path.Combine(Environment.CurrentDirectory, @"OFX\");
            databaseFile = "database.json";
        }

        public string CreateFile(string jsonFile)
        {
            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(databasePath);
            }

            File.WriteAllText(databasePath + databaseFile, jsonFile);

            return string.Empty;
        }

        public string GetFile()
        {
            if (!File.Exists(databasePath + databaseFile))
            {
                return string.Empty;
            }

            using var file = new StreamReader(databasePath + databaseFile);

            return file.ReadToEnd();
        }

    }
}
