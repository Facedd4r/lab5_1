using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace lab5_1
{
    public class Task3
    {
        public static void ExecuteBackupProcedure()
        {
            string sourcePath = @"F:\SourceFolder";
            string backupBasePath = @"F:\BackupFolder";

            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine("Ошибка: исходная директория не существует.");
                return;
            }

            InitializeBackupDirectory(backupBasePath);

            int latestVersion = DiscoverLatestVersion(backupBasePath);
            int nextVersionNumber = latestVersion + 1;

            bool modificationsDetected = false;

            if (latestVersion > 0)
            {
                string previousBackupPath = Path.Combine(backupBasePath, latestVersion.ToString());
                modificationsDetected = CompareWithPreviousBackup(sourcePath, previousBackupPath);
            }
            else
            {
                modificationsDetected = true;
            }

            if (modificationsDetected)
            {
                PerformBackupCreation(sourcePath, backupBasePath, nextVersionNumber);
                Console.WriteLine($"Создана резервная копия версии {nextVersionNumber}");
            }
            else
            {
                Console.WriteLine("Резервное копирование не требуется - файлы идентичны.");
            }
        }

        private static void InitializeBackupDirectory(string backupPath)
        {
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
        }

        private static int DiscoverLatestVersion(string backupRoot)
        {
            if (!Directory.Exists(backupRoot))
                return 0;

            var versionDirectories = Directory.GetDirectories(backupRoot)
                .Select(dir => Path.GetFileName(dir))
                .Where(name => int.TryParse(name, out _))
                .Select(name => int.Parse(name))
                .ToArray();

            return versionDirectories.Length > 0 ? versionDirectories.Max() : 0;
        }

        private static bool CompareWithPreviousBackup(string currentSource, string previousBackup)
        {
            var currentFiles = CollectFileInformation(currentSource);
            var previousFiles = CollectFileInformation(previousBackup);

            if (currentFiles.Count != previousFiles.Count)
                return true;

            foreach (var fileEntry in currentFiles)
            {
                if (!previousFiles.TryGetValue(fileEntry.Key, out string previousHash))
                    return true;

                if (fileEntry.Value != previousHash)
                    return true;
            }

            return false;
        }

        private static Dictionary<string, string> CollectFileInformation(string directoryPath)
        {
            var fileDetails = new Dictionary<string, string>();

            if (!Directory.Exists(directoryPath))
                return fileDetails;

            string[] files = Directory.GetFiles(directoryPath);

            using MD5 hashAlgorithm = MD5.Create();

            foreach (string filePath in files)
            {
                string fileName = Path.GetFileName(filePath);
                string fileHash = CalculateFileHash(filePath, hashAlgorithm);
                fileDetails[fileName] = fileHash;
            }

            return fileDetails;
        }

        private static string CalculateFileHash(string filePath, MD5 hasher)
        {
            try
            {
                using FileStream fileStream = File.OpenRead(filePath);
                byte[] hashBytes = hasher.ComputeHash(fileStream);
                return Convert.ToHexString(hashBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при вычислении хеша файла {filePath}: {ex.Message}");
                return string.Empty;
            }
        }

        private static void PerformBackupCreation(string sourceDir, string backupDir, int version)
        {
            string versionDirectory = Path.Combine(backupDir, version.ToString());

            try
            {
                Directory.CreateDirectory(versionDirectory);

                foreach (string sourceFile in Directory.GetFiles(sourceDir))
                {
                    string destinationFile = Path.Combine(versionDirectory, Path.GetFileName(sourceFile));
                    File.Copy(sourceFile, destinationFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании резервной копии: {ex.Message}");
                throw;
            }
        }
    }
}